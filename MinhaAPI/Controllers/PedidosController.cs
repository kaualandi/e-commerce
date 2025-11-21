using Application;
using Application.DTOs;
using Dominio.Entidades;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MinhaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> ObterTodos()
        {
            var pedidos = await _pedidoService.ObterTodosAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDTO>> ObterPorId(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> ObterPedidosDoCliente(int clienteId)
        {
            var pedidos = await _pedidoService.ObterPedidosDoClienteAsync(clienteId);
            return Ok(pedidos);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar(CriarPedidoDTO criarPedidoDto)
        {
            try
            {
                var id = await _pedidoService.CriarPedidoAsync(criarPedidoDto);
                return CreatedAtAction(nameof(ObterPorId), new { id }, id);
            }
            catch (ClienteNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (CarrinhoVazioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (EstoqueInsuficienteException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/finalizar")]
        public async Task<ActionResult> FinalizarPedido(int id, [FromBody] PagamentoDTO pagamentoDto)
        {
            try
            {
                var finalizarDto = new FinalizarPedidoDTO { PedidoId = id, Pagamento = pagamentoDto };
                var sucesso = await _pedidoService.ProcessarPagamentoAsync(finalizarDto);
                
                if (sucesso)
                    return Ok(new { Mensagem = "Pedido finalizado com sucesso" });
                else
                    return BadRequest(new { Mensagem = "Falha no processamento do pagamento" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (PagamentoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> AtualizarStatus(int id, [FromBody] StatusPedido novoStatus)
        {
            try
            {
                await _pedidoService.AtualizarStatusAsync(id, novoStatus);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}