using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);
        Task<IEnumerable<Produto>> ObterTodosAsync();
        Task<IEnumerable<Produto>> ObterPorCategoriaAsync(string categoria);
        Task<IEnumerable<Produto>> PesquisarAsync(string termo);
        Task<int> AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task RemoverAsync(int id);
        Task<bool> ExisteAsync(int id);
    }
}
