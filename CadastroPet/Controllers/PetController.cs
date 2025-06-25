using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio_API.Services;
using Repositories;
using System.Text.RegularExpressions;
using System.Globalization;
using Swashbuckle.AspNetCore.Annotations;
using Desafio_API.Models;


namespace Desafio_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly PetService _petService;
        private readonly PetRepository _petRepository;

        public PetController(PetRepository petRepository, PetService petService)
        {
            _petRepository = petRepository;
            _petService = petService;

        }

        [HttpPost]
        [SwaggerOperation
        (
            Summary = "Cadastro do pet",
            Description = "Registra um pet da raça cachorro ou gato. Os seguintes dados são obrigatórios: nome, espécie, raça, data de nascimento, peso, tutor e email do tutor. <br>Os campos cor e descrição do pet são opcionais. <br>Inserir a data no formato 'yyyy-MM-dd'."

        )]
        public async Task<IActionResult> CadastrarPet([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            try
            {
                var imagemUrl = await _petService.PegarImagemPorRaca(pet.Especie, pet.Raca);

                if (string.IsNullOrWhiteSpace(imagemUrl))
                {
                    return NotFound(new { Message = "Raça desconhecida." });

                }

                pet.Imagem = imagemUrl;

                _petRepository.AddPet(pet);

                Response.StatusCode = 201;
                return new ObjectResult(new { Message = "Pet cadastrado com sucesso." });

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao realizar o cadastro do pet." });

            }

        }

        [HttpGet]
        [SwaggerOperation
        (
            Summary = "Lista todos os pets cadastrados",
            Description = "Retorna uma lista de todos os pets cadastrados no sistema, ordenados pelo ID em ordem crescente."

        )]
        public IActionResult MostrarPets()
        {
            try
            {
                var pets = _petRepository.GetAllPets();

                if (!pets.Any())
                {
                    return NoContent();

                }

                return Ok(pets);

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao exibir a lista de pets." });
            }

        }

        [HttpGet("ids")]
        [SwaggerOperation
        (
            Summary = "Lista todos os ids dos pets cadastrados",
            Description = "Retorna uma lista de todos os ids dos pets cadastrados no banco de dados."

        )]
        public IActionResult MostrarIds()
        {
            try
            {
                var ids = _petRepository.MostrarIds();

                if (!ids.Any())
                {
                    return NoContent();

                }

                return Ok(ids);

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao exibir a lista de Ids." });
            }

        }

        [HttpGet("{id}")]
        [SwaggerOperation
        (
            Summary = "Obtém os dados de um pet cadastrado",
            Description = "Retorna as informações de um pet com base no ID fornecido."

        )]
        public IActionResult MostrarPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID inválido." });

            }
            try
            {
                Pet pet = _petRepository.GetById(id);

                if (pet == null)
                {
                    return NotFound(new { Messege = "Pet não encontrado." });

                }

                return Ok(pet);

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao exibir o item pelo ID." });
            }

        }

        [HttpGet("raca/{raca}")]
        [SwaggerOperation
        (
            Summary = "Lista todos os pets de uma raça específica",
            Description = "Retorna uma lista de todos os pets cadastrados que pertencem à raça fornecida."

        )]
        public IActionResult MostrarPorRaca(string raca)
        {
            if (string.IsNullOrWhiteSpace(raca))
            {
                return BadRequest(new { Message = "A raça não pode estar vazia na URL." });

            }

            try
            {
                var petPorRaca = _petRepository.GetByBreed(raca);

                if (!petPorRaca.Any())
                {
                    return NotFound(new { Message = "Nenhum pet dessa raça foi encontrado." });

                }

                return Ok(petPorRaca);

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao exibir os pets por raça." });

            }

        }

        [HttpGet("especie/{especie}")]
        [SwaggerOperation
        (
            Summary = "Lista todos os pets de uma espécie específica",
            Description = "Retorna uma lista de todos os pets cadastrados que pertencem à espécie fornecida."

        )]
        public IActionResult MostrarPorEspecie(string especie)
        {
            if (string.IsNullOrWhiteSpace(especie))
            {
                return BadRequest(new { Message = "A espécie não pode estar vazia na URL." });

            }

            try
            {
                var petPorEspecie = _petRepository.GetBySpecies(especie);

                if (!petPorEspecie.Any())
                {
                    return NotFound(new { Message = "Nenhum pet dessa especie foi encontrado." });

                }

                return Ok(petPorEspecie);

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao exibir os pets por espécie." });

            }

        }

        [HttpDelete("{id}")]
        [SwaggerOperation
        (
            Summary = "Exclui um pet cadastrado",
            Description = "Remove do banco de dados o pet correspondente ao ID fornecido."

        )]
        public IActionResult DeletarDadosCachorro(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID inválido." });

            }

            try
            {
                Pet pet = _petRepository.GetById(id);

                if (pet == null)
                {
                    return NotFound(new { Message = "Pet não encontrado para o ID informado." });
                }

                _petRepository.RemovePet(pet);

                return Ok(new { Message = "Pet excluído com sucesso." });

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao excluir o pet pelo ID." });

            }

        }

        [HttpPut]
        [SwaggerOperation
        (
            Summary = "Edita os dados de um pet cadastrado",
            Description = "Permite a atualização dos dados de um pet com base no ID fornecido.<br>Inserir a data no formato 'yyyy-MM-dd'."

        )]
        public async Task<IActionResult> AlterarDados([FromBody] Pet pet, int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "ID inválido." });

            }

            try
            {
                var animal = _petRepository.GetById(id);

                if (animal == null)
                {
                    return NotFound(new { Message = "Pet não encontrado para o ID informado." });

                }

                animal.Nome = pet.Nome ?? animal.Nome;

                animal.Especie = pet.Especie ?? animal.Especie;

                if (animal.Raca != pet.Raca && pet.Raca != null)
                {
                    var novaImagem = await _petService.PegarImagemPorRaca(animal.Especie, pet.Raca);

                    if (novaImagem == null)
                    {
                        return NotFound(new { Message = "Raça desconhecida" });

                    }

                    animal.Imagem = novaImagem;
                    animal.Raca = pet.Raca;

                }

                animal.Tutor = pet.Tutor ?? animal.Tutor;

                if (!string.IsNullOrWhiteSpace(pet.EmailTutor) && animal.EmailTutor != pet.EmailTutor)
                {
                    string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

                    if (!Regex.IsMatch(pet.EmailTutor, emailPattern))
                    {
                        return NotFound(new { Message = "O formato do email está inválido." });

                    }

                    animal.EmailTutor = pet.EmailTutor;

                }

                if (pet.DataNascimento.HasValue && pet.DataNascimento.Value.Date != animal.DataNascimento.Value.Date)
                {
                    DateTime dataNascimento = pet.DataNascimento.Value;

                    if (dataNascimento > DateTime.Today)
                    {
                        return BadRequest(new { Message = "A data de nascimento no formato 'yyyy-MM-dd' não pode ser maior que a data atual." });

                    }

                    string dataNascimentoStr = dataNascimento.ToString("yyyy-MM-dd");
                    DateTime dataValidada;

                    bool isValid = DateTime.TryParseExact(dataNascimentoStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataValidada);

                    if (!isValid)
                    {
                        return BadRequest(new { Message = "A data de nascimento fornecida é inválida " });

                    }

                    animal.DataNascimento = dataNascimento;

                }

                if (pet.Peso < 0.05 || pet.Peso > 100)
                {
                    return BadRequest(new { Message = "O peso deve estar entre 0.05 e 100 Kg." });

                }

                animal.Peso = pet.Peso;

                animal.Cor = pet.Cor ?? animal.Cor;

                animal.Descricao = pet.Descricao ?? animal.Descricao;

                _petRepository.SaveChanges();

                return Ok(new { Message = "Dados alterados com sucesso" });

            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Ocorreu um erro ao atualizar os dados do pet." });

            }

        }

    }

}