using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Desafio_API.Models;

namespace Desafio_API.Services
{
    public class PetService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public PetService()
        {
            httpClient = new HttpClient();

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        
            apiKey = config["ApiSettings:ApiKey"];

        }

        public async Task<List<RacaApi>> PegarTodasAsRacas(string especie)
        {
            try
            {
                string urlRacas;

                if(especie == "Cachorro")
                {
                    urlRacas = "https://api.thedogapi.com/v1/breeds";

                }
                else if(especie == "Gato")
                {
                    urlRacas = "https://api.thecatapi.com/v1/breeds";

                }
                else
                {
                    throw new Exception("Raça Inválida.");

                }

                var resposta = await httpClient.GetAsync(urlRacas);

                if(!resposta.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Erro ao buscar raças: {resposta.StatusCode}");
                }

                var json = await resposta.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<RacaApi>>(json); 
                
            }
            catch(HttpRequestException)
            {
                return null;

            }
            catch(Exception)
            {
                return null;

            }

        }

        public async Task<string> PegarImagemPorRaca(string especie, string raca)
        {
            try
            {
                var racas = await PegarTodasAsRacas(especie);

                if(racas == null) return null;

                var racaEncontrada = racas.FirstOrDefault(r => r.Raca.ToLower() == raca.ToLower());

                if (racaEncontrada != null)
                {
                    return especie == "Cachorro" 
                        ? $"https://cdn2.thedogapi.com/images/{racaEncontrada.IdImagem}.jpg" 
                        : $"https://cdn2.thecatapi.com/images/{racaEncontrada.IdImagem}.jpg";
                    
                }  

                return null;
                
            }
            catch(HttpRequestException)
            {
                return null;

            }
            catch(Exception)
            {
                return null;

            }

        }

        public async Task<List<string>> PegarFotosPorId(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return null;

            }
            
            bool eCachorro = int.TryParse(id, out int resutado);

            string urlImagens = eCachorro
                ? $"https://api.thedogapi.com/v1/images/search?limit=10&breed_ids={id}"
                : $"https://api.thecatapi.com/v1/images/search?limit=10&breed_ids={id}";
            
            try
            {
                string json = await httpClient.GetStringAsync(urlImagens);
              
                var imagens = JsonSerializer.Deserialize<List<Imagens>>(json);

                if(imagens != null)
                {
                    return imagens.Select(i => i.Url).ToList();

                }

                return new List<string>();
               
            }
            catch (Exception)
            {
                return new List<string>();

            }

        }

    }

}