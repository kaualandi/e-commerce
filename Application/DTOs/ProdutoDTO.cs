namespace Application.DTOs
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }

    public class CriarProdutoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }

    public class AtualizarProdutoDTO
    {
        public string? Nome { get; set; }
        public decimal? Preco { get; set; }
        public int? Estoque { get; set; }
        public string? Descricao { get; set; }
        public string? Categoria { get; set; }
        public bool? Ativo { get; set; }
    }
}