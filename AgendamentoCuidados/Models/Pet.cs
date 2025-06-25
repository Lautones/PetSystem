using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendamentoCuidados.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string? Nome { get; set; } 
        public string? Especie { get; set; }
        public string? Tutor { get; set; }
        public string? EmailTutor { get; set; }
        public string? Raca { get; set; }
        public DateTime DataNascimento { get; set; }
        public double Peso {get; set; }
        public string? Cor { get; set; }
        public string? Descricao { get; set; }
        public string? Imagem { get; set; }
        
    }

}