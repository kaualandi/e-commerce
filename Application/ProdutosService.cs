using Application.DTOs;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces;

namespace Application
{
    public class ProdutosService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterTodosAsync()
        {
            var produtos = await _produtoRepository.ObterTodosAsync();
            return produtos.Select(MapearParaDTO);
        }

        public async Task<ProdutoDTO?> ObterPorIdAsync(int id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            return produto != null ? MapearParaDTO(produto) : null;
        }

        public async Task<IEnumerable<ProdutoDTO>> ObterPorCategoriaAsync(string categoria)
        {
            var produtos = await _produtoRepository.ObterPorCategoriaAsync(categoria);
            return produtos.Select(MapearParaDTO);
        }

        public async Task<IEnumerable<ProdutoDTO>> PesquisarAsync(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return Enumerable.Empty<ProdutoDTO>();

            var produtos = await _produtoRepository.PesquisarAsync(termo);
            return produtos.Select(MapearParaDTO);
        }

        public async Task<int> CriarAsync(CriarProdutoDTO criarProdutoDto)
        {
            var produto = new Produto(
                criarProdutoDto.Nome,
                criarProdutoDto.Preco,
                criarProdutoDto.Estoque,
                criarProdutoDto.Descricao,
                criarProdutoDto.Categoria
            );

            return await _produtoRepository.AdicionarAsync(produto);
        }

        public async Task AtualizarAsync(int id, AtualizarProdutoDTO atualizarProdutoDto)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
                throw new ProdutoNaoEncontradoException(id);

            if (!string.IsNullOrWhiteSpace(atualizarProdutoDto.Nome))
                produto.Nome = atualizarProdutoDto.Nome;

            if (atualizarProdutoDto.Preco.HasValue)
                produto.Preco = atualizarProdutoDto.Preco.Value;

            if (atualizarProdutoDto.Estoque.HasValue)
                produto.Estoque = atualizarProdutoDto.Estoque.Value;

            if (!string.IsNullOrWhiteSpace(atualizarProdutoDto.Descricao))
                produto.Descricao = atualizarProdutoDto.Descricao;

            if (!string.IsNullOrWhiteSpace(atualizarProdutoDto.Categoria))
                produto.Categoria = atualizarProdutoDto.Categoria;

            if (atualizarProdutoDto.Ativo.HasValue)
                produto.Ativo = atualizarProdutoDto.Ativo.Value;

            await _produtoRepository.AtualizarAsync(produto);
        }

        public async Task RemoverAsync(int id)
        {
            if (!await _produtoRepository.ExisteAsync(id))
                throw new ProdutoNaoEncontradoException(id);

            await _produtoRepository.RemoverAsync(id);
        }

        public async Task AdicionarEstoqueAsync(int id, int quantidade)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
                throw new ProdutoNaoEncontradoException(id);

            produto.AdicionarEstoque(quantidade);
            await _produtoRepository.AtualizarAsync(produto);
        }

        private static ProdutoDTO MapearParaDTO(Produto produto)
        {
            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Estoque = produto.Estoque,
                Descricao = produto.Descricao,
                Categoria = produto.Categoria,
                Ativo = produto.Ativo
            };
        }
    }
}
