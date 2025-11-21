using Dominio.Entidades;
using Dominio.Interfaces;

namespace Infraestrutura.Repositorios
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly List<Produto> _produtos = new();
        private int _proximoId = 1;

        public Task<Produto?> ObterPorIdAsync(int id)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(produto);
        }

        public Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return Task.FromResult(_produtos.Where(p => p.Ativo).AsEnumerable());
        }

        public Task<IEnumerable<Produto>> ObterPorCategoriaAsync(string categoria)
        {
            var produtos = _produtos.Where(p => p.Ativo && 
                                         string.Equals(p.Categoria, categoria, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(produtos);
        }

        public Task<IEnumerable<Produto>> PesquisarAsync(string termo)
        {
            var produtos = _produtos.Where(p => p.Ativo && 
                                         (p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                                          p.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase)));
            return Task.FromResult(produtos);
        }

        public Task<int> AdicionarAsync(Produto produto)
        {
            produto.Id = _proximoId++;
            _produtos.Add(produto);
            return Task.FromResult(produto.Id);
        }

        public Task AtualizarAsync(Produto produto)
        {
            var indice = _produtos.FindIndex(p => p.Id == produto.Id);
            if (indice >= 0)
            {
                _produtos[indice] = produto;
            }
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int id)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            if (produto != null)
            {
                produto.Ativo = false; // Soft delete
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExisteAsync(int id)
        {
            var existe = _produtos.Any(p => p.Id == id);
            return Task.FromResult(existe);
        }
    }
}
