using Dominio.Exceptions;

namespace Dominio.Entidades
{
    public class Produto
    {
        private string _nome = string.Empty;
        private decimal _preco;
        private int _estoque;

        public int Id { get; set; }
        
        public string Nome 
        { 
            get => _nome; 
            set => _nome = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Nome não pode ser vazio");
        }
        
        public decimal Preco 
        { 
            get => _preco; 
            set => _preco = value > 0 ? value : throw new PrecoInvalidoException(value);
        }
        
        public int Estoque 
        { 
            get => _estoque; 
            set => _estoque = value >= 0 ? value : throw new ArgumentException("Estoque não pode ser negativo");
        }
        
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public Produto()
        {
        }

        public Produto(string nome, decimal preco, int estoque, string descricao = "", string categoria = "")
        {
            Nome = nome;
            Preco = preco;
            Estoque = estoque;
            Descricao = descricao;
            Categoria = categoria;
        }

        public void ReducirEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new QuantidadeInvalidaException(quantidade);
            
            if (Estoque < quantidade)
                throw new EstoqueInsuficienteException(Nome, Estoque, quantidade);
            
            Estoque -= quantidade;
        }

        public void AdicionarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new QuantidadeInvalidaException(quantidade);
            
            Estoque += quantidade;
        }

        public bool TemEstoqueSuficiente(int quantidade)
        {
            return Estoque >= quantidade;
        }
    }
}
