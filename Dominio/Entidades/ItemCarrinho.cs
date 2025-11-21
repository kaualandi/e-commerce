using Dominio.Exceptions;

namespace Dominio.Entidades
{
    public class ItemCarrinho
    {
        private int _quantidade;
        private decimal _precoUnitario;

        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        
        public int Quantidade 
        { 
            get => _quantidade; 
            set => _quantidade = value > 0 ? value : throw new QuantidadeInvalidaException(value);
        }
        
        public decimal PrecoUnitario 
        { 
            get => _precoUnitario; 
            set => _precoUnitario = value > 0 ? value : throw new PrecoInvalidoException(value);
        }

        public decimal Subtotal => Quantidade * PrecoUnitario;

        public ItemCarrinho()
        {
        }

        public ItemCarrinho(Produto produto, int quantidade)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            ProdutoId = produto.Id;
            Produto = produto;
            Quantidade = quantidade;
            PrecoUnitario = produto.Preco;
        }

        public void AlterarQuantidade(int novaQuantidade)
        {
            if (Produto != null && !Produto.TemEstoqueSuficiente(novaQuantidade))
                throw new EstoqueInsuficienteException(Produto.Nome, Produto.Estoque, novaQuantidade);
            
            Quantidade = novaQuantidade;
        }

        public void AtualizarPreco(decimal novoPreco)
        {
            PrecoUnitario = novoPreco;
        }
    }
}