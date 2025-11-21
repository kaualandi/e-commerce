using Dominio.Exceptions;

namespace Dominio.Entidades
{
    public class Carrinho
    {
        private readonly List<ItemCarrinho> _itens = new();

        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        public IReadOnlyList<ItemCarrinho> Itens => _itens.AsReadOnly();
        
        public decimal Total => _itens.Sum(i => i.Subtotal);
        
        public int QuantidadeTotalItens => _itens.Sum(i => i.Quantidade);
        
        public bool EstaVazio => !_itens.Any();

        public Carrinho()
        {
        }

        public Carrinho(int clienteId)
        {
            if (clienteId <= 0)
                throw new ArgumentException("ClienteId deve ser maior que zero");
            
            ClienteId = clienteId;
        }

        public void AdicionarItem(Produto produto, int quantidade)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));
            
            if (!produto.TemEstoqueSuficiente(quantidade))
                throw new EstoqueInsuficienteException(produto.Nome, produto.Estoque, quantidade);

            var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id);
            
            if (itemExistente != null)
            {
                var novaQuantidade = itemExistente.Quantidade + quantidade;
                if (!produto.TemEstoqueSuficiente(novaQuantidade))
                    throw new EstoqueInsuficienteException(produto.Nome, produto.Estoque, novaQuantidade);
                
                itemExistente.AlterarQuantidade(novaQuantidade);
            }
            else
            {
                _itens.Add(new ItemCarrinho(produto, quantidade));
            }
            
            DataAtualizacao = DateTime.Now;
        }

        public void RemoverItem(int produtoId)
        {
            var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item != null)
            {
                _itens.Remove(item);
                DataAtualizacao = DateTime.Now;
            }
        }

        public void AlterarQuantidadeItem(int produtoId, int novaQuantidade)
        {
            var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item == null)
                throw new ArgumentException($"Item com produto ID {produtoId} não encontrado no carrinho");
            
            item.AlterarQuantidade(novaQuantidade);
            DataAtualizacao = DateTime.Now;
        }

        public void Limpar()
        {
            _itens.Clear();
            DataAtualizacao = DateTime.Now;
        }

        public void ValidarParaFinalizacao()
        {
            if (EstaVazio)
                throw new CarrinhoVazioException();
            
            foreach (var item in _itens)
            {
                if (item.Produto != null && !item.Produto.TemEstoqueSuficiente(item.Quantidade))
                    throw new EstoqueInsuficienteException(item.Produto.Nome, item.Produto.Estoque, item.Quantidade);
            }
        }
    }
}