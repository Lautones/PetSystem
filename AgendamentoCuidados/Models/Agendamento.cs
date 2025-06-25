using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AgendamentoCuidados.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        
        public int PetId { get; set; }

        [Required(ErrorMessage = "O tipo de atendimento é obrigatório. (banho, vacina, etc.)")]
        public string? Tipo { get; set; }

        private DateTime? _data;

        [Required(ErrorMessage = "A data da consulta no formato 'yyyy-MM-dd HH:mm' é obrigatório.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? Data 
        { 
            get => _data; 
            set
            {
                if(value.HasValue && value.Value < DateTime.Today)
                {
                    throw new ValidationException("A data do agendamento não pode ser menor que a data atual.");

                }

                _data = value;

            }          
            
        }

        public string? Observacao {get; set;}
        
    }

}