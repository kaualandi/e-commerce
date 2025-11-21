using Application;
using Application.DTOs;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MinhaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutosService _produtosService;

        public ProdutosController(ProdutosService produtosService)
        {
            _produtosService = produtosService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterTodos()
        {
            var produtos = await _produtosService.ObterTodosAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> ObterPorId(int id)
        {
            try
            {
                var produto = await _produtosService.ObterPorIdAsync(id);
                if (produto == null)
                    return NotFound();

                return Ok(produto);
            }
            catch (ProdutoNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterPorCategoria(string categoria)
        {
            var produtos = await _produtosService.ObterPorCategoriaAsync(categoria);
            return Ok(produtos);
        }

        [HttpGet("pesquisar")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Pesquisar([FromQuery] string termo)
        {
            var produtos = await _produtosService.PesquisarAsync(termo);
            return Ok(produtos);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar(CriarProdutoDTO criarProdutoDto)
        {
            try
            {
                var id = await _produtosService.CriarAsync(criarProdutoDto);
                return CreatedAtAction(nameof(ObterPorId), new { id }, id);
            }
            catch (PrecoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(int id, AtualizarProdutoDTO atualizarProdutoDto)
        {
            try
            {
                await _produtosService.AtualizarAsync(id, atualizarProdutoDto);
                return NoContent();
            }
            catch (ProdutoNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (PrecoInvalidoException ex)
            {
                return BadRequest(ex.Message);
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
                await _produtosService.RemoverAsync(id);
                return NoContent();
            }
            catch (ProdutoNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}/estoque")]
        public async Task<ActionResult> AdicionarEstoque(int id, [FromBody] int quantidade)
        {
            try
            {
                await _produtosService.AdicionarEstoqueAsync(id, quantidade);
                return NoContent();
            }
            catch (ProdutoNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            catch (QuantidadeInvalidaException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
