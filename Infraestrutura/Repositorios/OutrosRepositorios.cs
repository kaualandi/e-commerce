using Dominio.Entidades;
using Dominio.Interfaces;

namespace Infraestrutura.Repositorios
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly List<Cliente> _clientes = new();
        private int _proximoId = 1;

        public Task<Cliente?> ObterPorIdAsync(int id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(cliente);
        }

        public Task<Cliente?> ObterPorEmailAsync(string email)
        {
            var cliente = _clientes.FirstOrDefault(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(cliente);
        }

        public Task<Cliente?> ObterPorCpfAsync(string cpf)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Cpf == cpf);
            return Task.FromResult(cliente);
        }

        public Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return Task.FromResult(_clientes.Where(c => c.Ativo).AsEnumerable());
        }

        public Task<int> AdicionarAsync(Cliente cliente)
        {
            cliente.Id = _proximoId++;
            _clientes.Add(cliente);
            return Task.FromResult(cliente.Id);
        }

        public Task AtualizarAsync(Cliente cliente)
        {
            var indice = _clientes.FindIndex(c => c.Id == cliente.Id);
            if (indice >= 0)
            {
                _clientes[indice] = cliente;
            }
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Id == id);
            if (cliente != null)
            {
                cliente.Ativo = false; // Soft delete
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExisteAsync(int id)
        {
            var existe = _clientes.Any(c => c.Id == id);
            return Task.FromResult(existe);
        }

        public Task<bool> ExisteEmailAsync(string email)
        {
            var existe = _clientes.Any(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(existe);
        }

        public Task<bool> ExisteCpfAsync(string cpf)
        {
            var existe = _clientes.Any(c => c.Cpf == cpf);
            return Task.FromResult(existe);
        }
    }

    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly List<Carrinho> _carrinhos = new();
        private int _proximoId = 1;

        public Task<Carrinho?> ObterPorIdAsync(int id)
        {
            var carrinho = _carrinhos.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(carrinho);
        }

        public Task<Carrinho?> ObterPorClienteIdAsync(int clienteId)
        {
            var carrinho = _carrinhos.FirstOrDefault(c => c.ClienteId == clienteId);
            return Task.FromResult(carrinho);
        }

        public Task<int> AdicionarAsync(Carrinho carrinho)
        {
            carrinho.Id = _proximoId++;
            _carrinhos.Add(carrinho);
            return Task.FromResult(carrinho.Id);
        }

        public Task AtualizarAsync(Carrinho carrinho)
        {
            var indice = _carrinhos.FindIndex(c => c.Id == carrinho.Id);
            if (indice >= 0)
            {
                _carrinhos[indice] = carrinho;
            }
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int id)
        {
            _carrinhos.RemoveAll(c => c.Id == id);
            return Task.CompletedTask;
        }

        public Task<bool> ExisteAsync(int id)
        {
            var existe = _carrinhos.Any(c => c.Id == id);
            return Task.FromResult(existe);
        }
    }

    public class PedidoRepository : IPedidoRepository
    {
        private readonly List<Pedido> _pedidos = new();
        private int _proximoId = 1;

        public Task<Pedido?> ObterPorIdAsync(int id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(pedido);
        }

        public Task<IEnumerable<Pedido>> ObterPorClienteIdAsync(int clienteId)
        {
            var pedidos = _pedidos.Where(p => p.ClienteId == clienteId).OrderByDescending(p => p.DataPedido).AsEnumerable();
            return Task.FromResult(pedidos);
        }

        public Task<IEnumerable<Pedido>> ObterTodosAsync()
        {
            return Task.FromResult(_pedidos.OrderByDescending(p => p.DataPedido).AsEnumerable());
        }

        public Task<IEnumerable<Pedido>> ObterPorStatusAsync(StatusPedido status)
        {
            var pedidos = _pedidos.Where(p => p.Status == status).OrderByDescending(p => p.DataPedido).AsEnumerable();
            return Task.FromResult(pedidos);
        }

        public Task<int> AdicionarAsync(Pedido pedido)
        {
            pedido.Id = _proximoId++;
            _pedidos.Add(pedido);
            return Task.FromResult(pedido.Id);
        }

        public Task AtualizarAsync(Pedido pedido)
        {
            var indice = _pedidos.FindIndex(p => p.Id == pedido.Id);
            if (indice >= 0)
            {
                _pedidos[indice] = pedido;
            }
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int id)
        {
            _pedidos.RemoveAll(p => p.Id == id);
            return Task.CompletedTask;
        }

        public Task<bool> ExisteAsync(int id)
        {
            var existe = _pedidos.Any(p => p.Id == id);
            return Task.FromResult(existe);
        }
    }
}