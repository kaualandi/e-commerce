using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MinhaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculosController : ControllerBase
    {
        public CalculosController()
        {
            
        }

        [HttpGet("dividir/{a}/{b}")]
        public ActionResult QualquerNome(int a, int b)
        {
            try
            {
                int c = a / b;
                return Ok($"A soma de {a} + {b} = {c} ");
            }
            catch (Exception ex)
            {
                return BadRequest($"Deu ruim. {ex.Message}");
            }

        }
    }
}
