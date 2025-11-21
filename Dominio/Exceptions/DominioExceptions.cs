namespace Dominio.Exceptions
{
    public class EstoqueInsuficienteException : Exception
    {
        public EstoqueInsuficienteException(string produto, int quantidadeDisponivel, int quantidadeSolicitada) 
            : base($"Estoque insuficiente para o produto '{produto}'. Disponível: {quantidadeDisponivel}, Solicitado: {quantidadeSolicitada}")
        {
        }
    }

    public class ProdutoNaoEncontradoException : Exception
    {
        public ProdutoNaoEncontradoException(int produtoId) 
            : base($"Produto com ID {produtoId} não foi encontrado")
        {
        }
    }

    public class ClienteNaoEncontradoException : Exception
    {
        public ClienteNaoEncontradoException(int clienteId) 
            : base($"Cliente com ID {clienteId} não foi encontrado")
        {
        }
    }

    public class CarrinhoVazioException : Exception
    {
        public CarrinhoVazioException() 
            : base("Não é possível finalizar um pedido com carrinho vazio")
        {
        }
    }

    public class QuantidadeInvalidaException : ArgumentException
    {
        public QuantidadeInvalidaException(int quantidade) 
            : base($"Quantidade deve ser maior que zero. Valor informado: {quantidade}")
        {
        }
    }

    public class PrecoInvalidoException : ArgumentException
    {
        public PrecoInvalidoException(decimal preco) 
            : base($"Preço deve ser maior que zero. Valor informado: {preco:C}")
        {
        }
    }

    public class PagamentoInvalidoException : Exception
    {
        public PagamentoInvalidoException(string motivo) 
            : base($"Pagamento inválido: {motivo}")
        {
        }
    }
}