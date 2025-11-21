using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObterPorIdAsync(int id);
        Task<Cliente?> ObterPorEmailAsync(string email);
        Task<Cliente?> ObterPorCpfAsync(string cpf);
        Task<IEnumerable<Cliente>> ObterTodosAsync();
        Task<int> AdicionarAsync(Cliente cliente);
        Task AtualizarAsync(Cliente cliente);
        Task RemoverAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<bool> ExisteEmailAsync(string email);
        Task<bool> ExisteCpfAsync(string cpf);
    }

    public interface ICarrinhoRepository
    {
        Task<Carrinho?> ObterPorIdAsync(int id);
        Task<Carrinho?> ObterPorClienteIdAsync(int clienteId);
        Task<int> AdicionarAsync(Carrinho carrinho);
        Task AtualizarAsync(Carrinho carrinho);
        Task RemoverAsync(int id);
        Task<bool> ExisteAsync(int id);
    }

    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsync(int id);
        Task<IEnumerable<Pedido>> ObterPorClienteIdAsync(int clienteId);
        Task<IEnumerable<Pedido>> ObterTodosAsync();
        Task<IEnumerable<Pedido>> ObterPorStatusAsync(StatusPedido status);
        Task<int> AdicionarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
        Task RemoverAsync(int id);
        Task<bool> ExisteAsync(int id);
    }
}