using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Desafio_API.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do pet é obrigatório.")]
        public string Nome { get; set; } 

        [Required(ErrorMessage = "A espécie do pet é obrigatório.")]  
        public string Especie { get; set; }

        [Required(ErrorMessage = "O nome do tutor é obrigatório.")]
        public string Tutor { get; set; }

        [Required(ErrorMessage = "O email do tutor é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        public string EmailTutor { get; set; }

        [Required(ErrorMessage = "A raça do pet é obrigatória.")]
        public string Raca { get; set; }

        private DateTime? _dataNascimento; 

        [Required(ErrorMessage = "A data de nascimento no formato 'yyyy-MM-dd' é obrigatória.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DataNascimento
        {
            get => _dataNascimento;
            set
            {
                if(value.HasValue && value.Value > DateTime.Today)
                {
                    throw new ValidationException("A data de nascimento não pode ser maior que a data atual.");

                }

                //Não vou colocar um limite de quantos anos o pet pode viver. Assim, fica aberto para inserir novas espécies no futuro - com a longividade bem variável.

                _dataNascimento = value;

            }

        }

        [Range(0.05, 100, ErrorMessage = "O peso deve estar entre 0.05 e 100 Kg.")]
        public double Peso {get; set; }

        public string Cor { get; set; }

        public string Descricao { get; set; }
        
        public string Imagem { get; set; }

    }

}