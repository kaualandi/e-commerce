using Application;
using Application.DTOs;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MinhaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ClientesService _clientesService;

        public ClientesController(ClientesService clientesService)
        {
            _clientesService = clientesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> ObterTodos()
        {
            var clientes = await _clientesService.ObterTodosAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> ObterPorId(int id)
        {
            var cliente = await _clientesService.ObterPorIdAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<ClienteDTO>> ObterPorEmail(string email)
        {
            var cliente = await _clientesService.ObterPorEmailAsync(email);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar(CriarClienteDTO criarClienteDto)
        {
            try
            {
                var id = await _clientesService.CriarAsync(criarClienteDto);
                return CreatedAtAction(nameof(ObterPorId), new { id }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(int id, AtualizarClienteDTO atualizarClienteDto)
        {
            try
            {
                await _clientesService.AtualizarAsync(id, atualizarClienteDto);
                return NoContent();
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Remover(int id)
        {
            try
            {
                await _clientesService.RemoverAsync(id);
                return NoContent();
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}