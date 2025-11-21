using Dominio.Entidades;
using Dominio.Entidades.Pagamentos;

namespace Application.DTOs
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }
        public StatusPedido Status { get; set; }
        public List<ItemCarrinhoDTO> Itens { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal Total { get; set; }
        public PagamentoDTO? Pagamento { get; set; }
        public string? EnderecoEntrega { get; set; }
        public string? Observacoes { get; set; }
    }

    public class CriarPedidoDTO
    {
        public int ClienteId { get; set; }
        public string? EnderecoEntrega { get; set; }
        public string? Observacoes { get; set; }
        public string? CodigoCupom { get; set; }
        public string CepDestino { get; set; } = string.Empty;
        public TipoFrete TipoFrete { get; set; }
    }

    public class FinalizarPedidoDTO
    {
        public int PedidoId { get; set; }
        public PagamentoDTO? Pagamento { get; set; }
    }

    public abstract class PagamentoDTO
    {
        public decimal Valor { get; set; }
        public abstract string TipoPagamento { get; }
    }

    public class PagamentoPixDTO : PagamentoDTO
    {
        public string ChavePix { get; set; } = string.Empty;
        public override string TipoPagamento => "PIX";
    }

    public class PagamentoCartaoDTO : PagamentoDTO
    {
        public string NumeroCartao { get; set; } = string.Empty;
        public string NomeTitular { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }
        public TipoCartao Tipo { get; set; }
        public int Parcelas { get; set; } = 1;
        public override string TipoPagamento => $"Cartão {Tipo}";
    }

    public enum TipoFrete
    {
        Pac = 1,
        Sedex = 2,
        FreteGratis = 3
    }
}