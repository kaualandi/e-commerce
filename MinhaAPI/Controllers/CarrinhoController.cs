using Application;
using Application.DTOs;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MinhaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        private readonly CarrinhoService _carrinhoService;

        public CarrinhoController(CarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<CarrinhoDTO>> ObterCarrinhoDoCliente(int clienteId)
        {
            try
            {
                var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(clienteId);
                return Ok(carrinho);
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("cliente/{clienteId}/itens")]
        public async Task<ActionResult> AdicionarItem(int clienteId, AdicionarItemCarrinhoDTO adicionarItemDto)
        {
            try
            {
                await _carrinhoService.AdicionarItemAsync(clienteId, adicionarItemDto);
                return NoContent();
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ProdutoNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (EstoqueInsuficienteException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (QuantidadeInvalidaException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("cliente/{clienteId}/itens/{produtoId}")]
        public async Task<ActionResult> RemoverItem(int clienteId, int produtoId)
        {
            try
            {
                await _carrinhoService.RemoverItemAsync(clienteId, produtoId);
                return NoContent();
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("cliente/{clienteId}/itens")]
        public async Task<ActionResult> AlterarQuantidadeItem(int clienteId, AlterarQuantidadeItemDTO alterarQuantidadeDto)
        {
            try
            {
                await _carrinhoService.AlterarQuantidadeItemAsync(clienteId, alterarQuantidadeDto);
                return NoContent();
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (EstoqueInsuficienteException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (QuantidadeInvalidaException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("cliente/{clienteId}")]
        public async Task<ActionResult> LimparCarrinho(int clienteId)
        {
            try
            {
                await _carrinhoService.LimparCarrinhoAsync(clienteId);
                return NoContent();
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}