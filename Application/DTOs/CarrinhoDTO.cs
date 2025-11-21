namespace Application.DTOs
{
    public class CarrinhoDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public List<ItemCarrinhoDTO> Itens { get; set; } = new();
        public decimal Total { get; set; }
        public int QuantidadeTotalItens { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class ItemCarrinhoDTO
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class AdicionarItemCarrinhoDTO
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }

    public class AlterarQuantidadeItemDTO
    {
        public int ProdutoId { get; set; }
        public int NovaQuantidade { get; set; }
    }
}