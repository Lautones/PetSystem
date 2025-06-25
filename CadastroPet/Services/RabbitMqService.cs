using System;
using System.Text;
using System.Text.Json;
using CadastroPet.Models;
using Desafio_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repositories;

namespace CadastroPet.Services
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        //tive que implementar para resolver depêndencia circular
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqService(IOptions<RabbitMqConfig> config, IServiceProvider serviceProvider)
        {
            try
            {
                _serviceProvider = serviceProvider;
                var rabbitMqConfig = config.Value;

                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMqConfig.HostName,
                    Port = rabbitMqConfig.Port,
                    UserName = rabbitMqConfig.UserName,
                    Password = rabbitMqConfig.Password

                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "pet_exchange", type: ExchangeType.Direct);

                // Fila para criar
                _channel.QueueDeclare(queue: "pet_created_queue",
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                _channel.QueueBind(queue: "pet_created_queue",
                                   exchange: "pet_exchange",
                                   routingKey: "pet_created");

                // Fila para responder
                _channel.QueueDeclare(queue: "pet_info_request_queue",
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                _channel.QueueBind(queue: "pet_info_request_queue",
                                   exchange: "pet_exchange",
                                   routingKey: "pet_info_request");

                ResponderComInfoPet();


            }
            catch (Exception ex)
            {
                Dispose();
                throw new Exception($"Erro ao conectar ao RabbitMQ: {ex.Message}");

            }

        }

        public void PublicarEventoPetCriado(Pet pet)
        {
            try
            {
                var mensagem = JsonSerializer.Serialize(pet);
                var body = Encoding.UTF8.GetBytes(mensagem);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(exchange: "pet_exchange",
                                    routingKey: "pet_created",
                                    basicProperties: properties,
                                    body: body);

            }
            catch(Exception ex)
            {
                throw new Exception($"Erro ao publicar o evento 'pet_created': {ex.Message}");
            }

        }

        public void ResponderComInfoPet()
        {
            var consumidor = new EventingBasicConsumer(_channel);

            consumidor.Received += (model, ea) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var petRepository = scope.ServiceProvider.GetRequiredService<PetRepository>();

                    try
                    {
                        var body = ea.Body.ToArray();
                        var mensagem = Encoding.UTF8.GetString(body);

                        var solicitacao = JsonSerializer.Deserialize<SolicitacaoPet>(mensagem);

                        if(solicitacao == null || solicitacao.PetId <= 0)
                        {
                            Console.WriteLine("Pedido inválido. Escreva um Id válido.");

                            _channel.BasicNack(ea.DeliveryTag, false, true);

                            return;

                        }

                        var pet = petRepository.GetById(solicitacao.PetId);

                        if(pet != null)
                        {
                            var resposta = JsonSerializer.Serialize(pet);
                            var bodyResposta = Encoding.UTF8.GetBytes(resposta);

                            var properties = _channel.CreateBasicProperties();
                            properties.Persistent = true;

                            _channel.BasicPublish(exchange: "pet_exchange",
                                                routingKey: "pet_info_response",
                                                basicProperties: properties,
                                                body: bodyResposta);

                            _channel.BasicAck(ea.DeliveryTag, multiple: false);
                        
                        }
                        
                        else
                        {
                            Console.WriteLine("Pedido inválido. O pet com o id fornecido não está listado no Banco de Dados.");

                            var respostaNull = JsonSerializer.Serialize(new { PetId = 0 });
                            var bodyRespostaNull = Encoding.UTF8.GetBytes(respostaNull);

                            var properties = _channel.CreateBasicProperties();
                            properties.Persistent = true;

                            _channel.BasicPublish(exchange: "pet_exchange",
                                                routingKey: "pet_info_response",
                                                basicProperties: properties,
                                                body: bodyRespostaNull);
                                                
                            _channel.BasicAck(ea.DeliveryTag, multiple: false); 

                        }

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");

                        _channel.BasicNack(ea.DeliveryTag, false, false);

                    }

                }

            };

            try
            {
                _channel.BasicConsume(queue: "pet_info_request_queue",
                                      autoAck: false,
                                      consumer: consumidor);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao consumir mensagens: {ex.Message}");

            }

        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            
        }

    }

}