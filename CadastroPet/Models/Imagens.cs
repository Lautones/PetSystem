using System.Text.Json.Serialization;

namespace Desafio_API.Models
{
    public class Imagens
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}