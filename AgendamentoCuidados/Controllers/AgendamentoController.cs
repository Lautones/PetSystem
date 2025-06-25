using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AgendamentoCuidados.Models;
using AgendamentoCuidados.Repositories;
using AgendamentoCuidados.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AgendamentoCuidados.Controllers
{
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        private readonly AgendamentoRepository _repository;
        private readonly RabbitMqService _rabbitService;

        public AgendamentoController(AgendamentoRepository repository, RabbitMqService rabbitService)
        {
            _repository = repository;
            _rabbitService = rabbitService;	

        }

        [HttpGet]
        [SwaggerOperation
        (
            Summary = "Lista todos os agendamentos",
            Description = "Retorna uma lista de agendamentos cadastrados no sistema, ordenados pelo ID em ordem crescente."
        
        )]
        public async Task<IActionResult> MostrarAgendamentos()
        {
            try
            {
                var agendamentos = await _repository.ListarAgendamentos();

                if(!agendamentos.Any())
                {
                    return NoContent();
                }

                return Ok(agendamentos);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro interno: {ex.Message}");

            }

        }

        [HttpGet("{petId}")]
        [SwaggerOperation
        (
            Summary = "Lista todos os agendamentos de um pet com base no PetId",
            Description = "Retorna uma lista de agendamentos cadastrados no sistema, ordenados pelo PetId em ordem crescente."
        
        )]
        public async Task<IActionResult> MostrarAgendamentosDoPet(int petId)
        {
            try
            {
                var agendamentos = await _repository.ListarAgendamentosPorPetId(petId);

                if(!agendamentos.Any())
                {
                    return NoContent();
                }

                return Ok(agendamentos);

            }
            catch (Exception)
            {
                return StatusCode(500, new {Message = "Ocorreu um erro ao realizar o agendamento do pet"});

            }

        }

        [HttpPost]
        [SwaggerOperation
        (
            Summary = "Cria um agendamento manual de cuidados para um pet",
            Description = "Cria um agendamento manual para um pet previamente cadastrado no serviço 'CadastroPet', permitindo escolher a data, o tipo de cuidado (vacina, check-up, banho, etc.) e uma observação opcional."

        )]
        public async Task<IActionResult> AgendamentoManual([FromBody] Agendamento agendamento)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            try
            {
                Pet? pet = await _rabbitService.PedirInformacoesDoPet(agendamento.PetId);

               if(pet == null)
                {
                    return NotFound(new { Message = "Nenhum pet encontrado com esse Id."});

                }

                await _repository.AdicionarAgendamento(agendamento);

                return Created(string.Empty, new { Message = "Agendamento realizado." });

            }
            catch (Exception)
            {
                return StatusCode(500, new {Message = "Ocorreu um erro ao realizar o agendamento do pet"});
                
            }

        }

        [HttpPut("{id}")]
        [SwaggerOperation
        (
            Summary = "Edita os dados de agendamento cadastrado",
            Description = "Permite a atualização dos dados de um agendamento com base no ID fornecido.<br>Inserir a data no formato 'yyyy-MM-dd'."

        )]
        public async Task<IActionResult> AlterarDadosAgendamento([FromBody] Agendamento agendamento, int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID inválido." });

            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            try
            {
                var agendaAnterior = await _repository.ListarAgendamentoPorId(id);

                if (agendaAnterior == null)
                {
                    return NotFound(new { Message = "Agendamento não encontrado para o ID informado"});

                }
                
                if(agendamento.PetId != agendaAnterior.PetId && agendamento.PetId > 0)
                {
                    Pet? petAuxiliar = await _rabbitService.PedirInformacoesDoPet(agendamento.PetId);

                    if (petAuxiliar == null)
                    {
                        return NotFound(new { Message = "Nenhum pet encontrado com esse Id."});

                    }

                    agendaAnterior.PetId = agendamento.PetId;

                }

                agendaAnterior.Data = agendamento.Data ?? agendaAnterior.Data;

                agendaAnterior.Tipo = agendamento.Tipo ?? agendaAnterior.Tipo;

                agendaAnterior.Observacao = agendamento.Observacao ?? agendaAnterior.Observacao;

                await _repository.Salvar();

                return Ok(new { Message = "Dados alterados com sucesso" });

            }
            catch(Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao atualizar os dados do pet." });

            }

        }

        [HttpDelete("{id}")]
        [SwaggerOperation
        (
            Summary = "Exclui um agendamento cadastrado",
            Description = "Remove do banco de dados o agendamento correspondente ao ID fornecido."
        
        )]
        public async Task<IActionResult> DeletarAgendamento(int id)
        {
            if(id <= 0)
            {
                return BadRequest(new {Message = "ID inválido"});

            }

            try
            {
                Agendamento agendamento = await _repository.ListarAgendamentoPorId(id);


                if(agendamento == null)
                {
                    return NotFound(new { Message = "Agendamento não encontrado para o ID informado." });

                }
                
                await _repository.DeletarAgendamento(agendamento);                

                return Ok(new {Message = "Agendamento excluído com sucesso." }); 

            }
            catch(Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao excluir o agendamento pelo ID." });

            }
        
        }      

    }

}