using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendamentoCuidados.Models;
using AgendamentoCuidados.Repositories;

namespace AgendamentoCuidados.Services
{
    public class AutoAgendamentoService
    {
        private readonly AgendamentoRepository _agendamentoRepository;
        private readonly ValidatorService _validatorService;

        public AutoAgendamentoService(AgendamentoRepository agendamentoRepository, ValidatorService validatorService)
        {
            _agendamentoRepository = agendamentoRepository;
            _validatorService = validatorService;

        }

        public async Task AgendamentoAutomatico(Pet pet)
        {
            var agendamentos = new List<Agendamento>();

            if(_validatorService.ERecemNascido(pet.DataNascimento))
            {
                agendamentos = new List<Agendamento>
                {
                    new Agendamento
                    {
                        PetId = pet.Id,
                        Tipo = "Checkup inicial",
                        Data = DateTime.Now.AddDays(2),
                        Observacao = "Agendamento automático para realizar o checkup geral."

                    },

                    new Agendamento
                    {
                        PetId = pet.Id,
                        Tipo = "Vacina Polivalente",
                        Data = DateTime.Now.AddDays(3),
                        Observacao = "Agendamento automático para a primeira vacina."

                    }

                };

            }
            else if(_validatorService.ESenior(pet.DataNascimento))
            {
                agendamentos.Add(new Agendamento
                {
                    PetId = pet.Id,
                    Tipo = "Checkup completo",
                    Data = DateTime.Now.AddDays(2),
                    Observacao = "Agendamento automático para realizar o checkup completo."

                });

            }
            else
            {
                agendamentos.Add(new Agendamento
                {
                    PetId = pet.Id,
                    Tipo = "Checkup de rotina",
                    Data = DateTime.Now.AddDays(5),
                    Observacao = "Agendamento automático para realizar o checkup de rotina."

                });

            }

            if(agendamentos.Any())
            {
                await _agendamentoRepository.AdicionarAgendamentos(agendamentos);

            }

        }

    }

}