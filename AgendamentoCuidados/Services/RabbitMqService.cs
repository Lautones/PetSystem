using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AgendamentoCuidados.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace AgendamentoCuidados.Services
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        public RabbitMqService(IOptions<RabbitMqConfig> config, IServiceProvider serviceProvider)
        {
            try
            {
                var rabbitMqConfig = config.Value;
                _serviceProvider = serviceProvider;

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

                _channel.QueueDeclare(queue: "pet_created_queue",
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                _channel.QueueBind(queue: "pet_created_queue",
                                   exchange: "pet_exchange",
                                   routingKey: "pet_created");

                _channel.QueueDeclare(queue: "pet_info_response_queue",
                                      durable : true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                _channel.QueueBind(queue: "pet_info_response_queue",
                                   exchange: "pet_exchange",
                                   routingKey: "pet_info_response");

                ConsumirEventoPetCreated();

            }
            catch (Exception ex)
            {
                Dispose();
                throw new Exception("Erro ao conectar ao RabbitMQ.", ex);

            }

        }

        public void ConsumirEventoPetCreated()
        {
            var consumidor = new EventingBasicConsumer(_channel);

            consumidor.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var mensagem = Encoding.UTF8.GetString(body);
                    
                    var pet = JsonSerializer.Deserialize<Pet>(mensagem);

                    if (pet != null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var agendamentoService = scope.ServiceProvider.GetRequiredService<AutoAgendamentoService>();

                            await agendamentoService.AgendamentoAutomatico(pet);

                        }

                    }

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");

                    _channel.BasicNack(ea.DeliveryTag, false, true);

                }

            };

            try
            {
                _channel.BasicConsume(queue: "pet_created_queue",
                                      autoAck: false,
                                      consumer: consumidor);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consumir mensagens: {ex.Message}");
                
            }

        }

        public async Task<Pet?> PedirInformacoesDoPet(int petId)
        {
            try
            {
                var pedido = new {PetId = petId};
                var mensagem = JsonSerializer.Serialize(pedido);
                var body = Encoding.UTF8.GetBytes(mensagem);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(exchange: "pet_exchange",
                                      routingKey: "pet_info_request",
                                      basicProperties: properties,
                                      body: body);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao solicitar informações: {ex.Message}");

            }

            return await ConsumirInformacoesPet();

        }

        public async Task<Pet?> ConsumirInformacoesPet()
        {
            var consumidor = new EventingBasicConsumer(_channel);
            var tarefa = new TaskCompletionSource<Pet?>();
            var timerBreak = new CancellationTokenSource(TimeSpan.FromSeconds(10)); 

            consumidor.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var mensagem = Encoding.UTF8.GetString(body);
                    

                    if (mensagem.Trim().ToLower() == "null" || mensagem.Contains("\"PetId\":0"))
                    {
                        tarefa.TrySetResult(null);

                    }
                    else
                    {
                        var pet = JsonSerializer.Deserialize<Pet>(mensagem);
                        tarefa.TrySetResult(pet);

                    }

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");

                    _channel.BasicNack(ea.DeliveryTag, false, true);
                    tarefa.TrySetException(ex);

                }

            };
            try
            {
                _channel.BasicConsume(queue: "pet_info_response_queue",
                                      autoAck: false,
                                      consumer: consumidor);

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro ao consumir mensagens: {ex.Message}");

            }

            timerBreak.Token.Register(() => tarefa.TrySetCanceled(), useSynchronizationContext: false);

            return await tarefa.Task;

        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close(); 

        }

    }

}