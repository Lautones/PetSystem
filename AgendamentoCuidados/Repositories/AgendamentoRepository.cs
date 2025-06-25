using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendamentoCuidados.Data;
using AgendamentoCuidados.Models;
using AgendamentoCuidados.Services;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoCuidados.Repositories
{
    public class AgendamentoRepository
    {
        private readonly ApplicationDbContext _database;

        public AgendamentoRepository (ApplicationDbContext database)
        {
            _database = database;

        }

        public async Task AdicionarAgendamentos (List<Agendamento> agendamentos)
        {
            try
            {
                await _database.Agendamentos.AddRangeAsync(agendamentos);
                await _database.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar os agendamentos no Banco de Dados.", ex);

            }

        }

        public async Task AdicionarAgendamento(Agendamento agendamento)
        {
            try
            {
                await _database.Agendamentos.AddAsync(agendamento);
                await _database.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar o agendamento no Banco de Dados.", ex);

            }

        }

        public async Task<List<Agendamento>> ListarAgendamentos()
        {
            try
            {
                return await _database.Agendamentos.ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao retornar a lista de agendamentos do Banco de Dados.", ex);

            }

        }

        public async Task<List<Agendamento>> ListarAgendamentosPorPetId(int id)
        {
            try
            {
                return await _database.Agendamentos.Where(pet => pet.PetId == id).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao retornar a lista de agendamentos do Banco de Dados.", ex);

            }

        }

        public async Task<Agendamento> ListarAgendamentoPorId(int id)
        {
            try
            {
                return await _database.Agendamentos.FirstAsync(ag => ag.Id == id);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao retornar a lista de agendamentos do Banco de Dados.", ex);

            }

        }

        public async Task DeletarAgendamento(Agendamento agendamento)
        {
            try
            {
                _database.Agendamentos.Remove(agendamento);
                await _database.SaveChangesAsync(); 

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar informações do Banco de Dados.", ex);

            }

        }

        public async Task Salvar()
        {
            try
            {
                await _database.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao salvar informações no Banco de Dados.", ex);

            }

        }

    }

}