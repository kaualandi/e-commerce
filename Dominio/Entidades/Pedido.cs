using Dominio.Entidades.Pagamentos;
using Dominio.Exceptions;

namespace Dominio.Entidades
{
    public class Pedido
    {
        private readonly List<ItemCarrinho> _itens = new();

        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        
        public decimal SubTotal => _itens.Sum(i => i.Subtotal);
        public decimal ValorFrete { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal Total => SubTotal + ValorFrete - ValorDesconto;
        
        public IReadOnlyList<ItemCarrinho> Itens => _itens.AsReadOnly();
        
        public Pagamento? Pagamento { get; set; }
        public string? EnderecoEntrega { get; set; }
        public string? Observacoes { get; set; }

        public Pedido()
        {
        }

        public Pedido(int clienteId, IEnumerable<ItemCarrinho> itens)
        {
            if (clienteId <= 0)
                throw new ArgumentException("ClienteId deve ser maior que zero");
            
            ClienteId = clienteId;
            AdicionarItens(itens);
        }

        public static Pedido CriarDe(Carrinho carrinho)
        {
            if (carrinho == null)
                throw new ArgumentNullException(nameof(carrinho));
            
            carrinho.ValidarParaFinalizacao();
            
            return new Pedido(carrinho.ClienteId, carrinho.Itens);
        }

        private void AdicionarItens(IEnumerable<ItemCarrinho> itens)
        {
            if (itens == null || !itens.Any())
                throw new CarrinhoVazioException();
            
            _itens.AddRange(itens);
        }

        public void DefinirFrete(decimal valorFrete)
        {
            if (valorFrete < 0)
                throw new ArgumentException("Valor do frete não pode ser negativo");
            
            ValorFrete = valorFrete;
        }

        public void AplicarDesconto(decimal valorDesconto)
        {
            if (valorDesconto < 0)
                throw new ArgumentException("Valor do desconto não pode ser negativo");
            
            if (valorDesconto > SubTotal)
                throw new ArgumentException("Desconto não pode ser maior que o subtotal");
            
            ValorDesconto = valorDesconto;
        }

        public void DefinirPagamento(Pagamento pagamento)
        {
            if (pagamento == null)
                throw new ArgumentNullException(nameof(pagamento));
            
            if (Math.Abs(pagamento.Valor - Total) > 0.01m)
                throw new PagamentoInvalidoException($"Valor do pagamento ({pagamento.Valor:C}) não confere com o total do pedido ({Total:C})");
            
            Pagamento = pagamento;
        }

        public bool ProcessarPagamento()
        {
            if (Pagamento == null)
                throw new PagamentoInvalidoException("Pagamento não definido");
            
            if (Status != StatusPedido.Pendente)
                throw new InvalidOperationException($"Não é possível processar pagamento para pedido com status {Status}");
            
            Status = StatusPedido.ProcessandoPagamento;
            
            if (Pagamento.ProcessarPagamento())
            {
                Status = StatusPedido.Confirmado;
                ReduzirEstoqueDosItens();
                return true;
            }
            else
            {
                Status = StatusPedido.PagamentoRejeitado;
                return false;
            }
        }

        private void ReduzirEstoqueDosItens()
        {
            foreach (var item in _itens)
            {
                if (item.Produto != null)
                {
                    item.Produto.ReducirEstoque(item.Quantidade);
                }
            }
        }

        public void Cancelar(string motivo = "")
        {
            if (Status == StatusPedido.Entregue)
                throw new InvalidOperationException("Não é possível cancelar um pedido já entregue");
            
            Status = StatusPedido.Cancelado;
            
            // Se o pedido estava confirmado, devolver estoque
            if (Status == StatusPedido.Confirmado || Status == StatusPedido.EmPreparacao || Status == StatusPedido.EmTransito)
            {
                foreach (var item in _itens)
                {
                    if (item.Produto != null)
                    {
                        item.Produto.AdicionarEstoque(item.Quantidade);
                    }
                }
            }
        }

        public void IniciarPreparacao()
        {
            if (Status != StatusPedido.Confirmado)
                throw new InvalidOperationException($"Não é possível iniciar preparação de pedido com status {Status}");
            
            Status = StatusPedido.EmPreparacao;
        }

        public void IniciarTransito()
        {
            if (Status != StatusPedido.EmPreparacao)
                throw new InvalidOperationException($"Não é possível iniciar trânsito de pedido com status {Status}");
            
            Status = StatusPedido.EmTransito;
        }

        public void ConfirmarEntrega()
        {
            if (Status != StatusPedido.EmTransito)
                throw new InvalidOperationException($"Não é possível confirmar entrega de pedido com status {Status}");
            
            Status = StatusPedido.Entregue;
        }
    }

    public enum StatusPedido
    {
        Pendente,
        ProcessandoPagamento,
        Confirmado,
        EmPreparacao,
        EmTransito,
        Entregue,
        Cancelado,
        PagamentoRejeitado
    }
}