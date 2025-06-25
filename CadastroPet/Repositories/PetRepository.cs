using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CadastroPet.Services;
using Desafio_API.Data;
using Desafio_API.Models;

namespace Repositories
{
    public class PetRepository
    {
        private readonly ApplicationDbContext _database;
        private readonly RabbitMqService _rabbitMqService;

        public PetRepository(ApplicationDbContext database, RabbitMqService rabbitMqService)
        {
            _database = database;
            _rabbitMqService = rabbitMqService;

        }

        public List<Pet> GetAllPets()
        {
            try
            {
                return _database.Pets.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao retornar a lista de pets do Banco de Dados.", ex);

            }

        }

        public void AddPet(Pet pet)
        {
            try
            {
                _database.Pets.Add(pet);
                _database.SaveChanges();

                _rabbitMqService.PublicarEventoPetCriado(pet);

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao adicionar o pet no Banco de Dados.", ex);

            }

        }

        public Pet GetById(int id)
        {
            try
            {
                return _database.Pets.FirstOrDefault(cao => cao.Id ==id);

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao retornar o pet do Banco de Dados.", ex);

            }

        }

        public List<int> MostrarIds()
        {
            try
            {
                return _database.Pets.Select(cao => cao.Id).ToList();

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao retornar os IDs dos pets do Banco de Dados.", ex);

            }

        }

        public List<Pet> GetByBreed(string breed)
        {
            try
            {
                return _database.Pets.Where(cao => cao.Raca.ToLower() == breed.ToLower()).ToList();

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao retornar a lista de pets pela raça do Banco de Dados.", ex);

            }

        }

        public List<Pet> GetBySpecies(string specie)
        {
            try
            {
                return _database.Pets.Where(cao => cao.Especie.ToLower() == specie.ToLower()).ToList();

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao retornar a lista de pets pela especie do Banco de Dados.", ex);

            }

        }

        public void RemovePet(Pet pet)
        {
            try
            {
                _database.Pets.Remove(pet);
                _database.SaveChanges();

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao deletar o pet no Banco de Dados.", ex);
            }

        }

        public void SaveChanges()
        {
            try
            {
                _database.SaveChanges();

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao salvar informações no Banco de Dados.", ex);

            }
            
        }

    }
    
}