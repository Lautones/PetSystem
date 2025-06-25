using System.Text.Json.Serialization;

namespace Desafio_API.Models
{
    public class RacaApi
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonPropertyName("name")]
        public string Raca { get; set; }

        [JsonPropertyName("reference_image_id")]
        public string IdImagem { get; set; }
        
    }

}