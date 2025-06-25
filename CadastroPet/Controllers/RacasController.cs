using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Desafio_API.Controllers
{
    [Route("api/racas")]
    [ApiController]
    public class RacasController : ControllerBase
    {
        private readonly PetService _petService;

        public RacasController(PetService petService)
        {
            _petService = petService;

        }

        [HttpGet("cachorro")]
        [SwaggerOperation
        (
            Summary = "Lista todas as raças de cachorro",
            Description = "Retorna todas as raças de cachorro disponíveis na The Dog API."
        
        )]
        public async Task<IActionResult> MostrarRacasCachorros()
        {
            try
            {
                var racasCachorro = await _petService.PegarTodasAsRacas("Cachorro");

                if( racasCachorro == null)
                {
                    return NotFound(new { Message = "Nenhuma raça encontrada" });

                }

                var nomesRacas = racasCachorro.Select(r => new 
                {
                    Id = r.Id,
                    Nome = r.Raca
                    

                }).ToList();

                return Ok(nomesRacas);

            }
            catch
            {
                return StatusCode(500);

            }

        }

        [HttpGet("gato")]
        [SwaggerOperation
        (
            Summary = "Lista todas as raças de gato",
            Description = "Retorna todas as raças de gato disponíveis na The Cat API."
        
        )]
        public async Task<IActionResult> MostrarRacasGatos()
        {
            try
            {
                var racasCachorro = await _petService.PegarTodasAsRacas("Gato");

                if( racasCachorro == null)
                {
                    return NotFound(new { Message = "Nenhuma raça encontrada." });

                }

                var nomesRacas = racasCachorro.Select(r => new 
                {
                    Id = r.Id,
                    Nome = r.Raca
                    

                }).ToList();

                return Ok(nomesRacas);

            }
            catch
            {
                return StatusCode(500);

            }

        }

        [HttpGet("imagens/{id}")]
        [SwaggerOperation
        (
            Summary = "Busca imagens do pet pelo ID",
            Description = "Retorna até 10 fotos da raça do pet com base no ID fornecido."
        
        )]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var imagens = await _petService.PegarFotosPorId(id);

                if(imagens == null || imagens.Count == 0)
                {
                    return NotFound(new { Message = "Nenhuma imagem encontrada com esse id." });
                    
                }

                return Ok(imagens);

            }
            catch
            {
                return StatusCode(500);

            }

        }

    }
    
}