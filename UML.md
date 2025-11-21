# Diagrama de Classes - Sistema E-commerce

```mermaid
classDiagram
    class Produto {
        -string _nome
        -decimal _preco
        -int _estoque
        +int Id
        +string Nome
        +decimal Preco
        +int Estoque
        +string Descricao
        +string Categoria
        +bool Ativo
        +Produto()
        +Produto(string nome, decimal preco, int estoque, string descricao, string categoria)
        +void ReducirEstoque(int quantidade)
        +void AdicionarEstoque(int quantidade)
        +bool TemEstoqueSuficiente(int quantidade)
    }

    class Cliente {
        -string _nome
        -string _email
        -string _cpf
        +int Id
        +string Nome
        +string Email
        +string Cpf
        +string Telefone
        +DateTime DataCadastro
        +bool Ativo
        +Cliente()
        +Cliente(string nome, string email, string cpf, string telefone)
    }

    class ItemCarrinho {
        -int _quantidade
        -decimal _precoUnitario
        +int Id
        +int ProdutoId
        +Produto Produto
        +int Quantidade
        +decimal PrecoUnitario
        +decimal Subtotal
        +ItemCarrinho()
        +ItemCarrinho(Produto produto, int quantidade)
        +void AlterarQuantidade(int novaQuantidade)
        +void AtualizarPreco(decimal novoPreco)
    }

    class Carrinho {
        -List~ItemCarrinho~ _itens
        +int Id
        +int ClienteId
        +Cliente Cliente
        +DateTime DataCriacao
        +DateTime? DataAtualizacao
        +IReadOnlyList~ItemCarrinho~ Itens
        +decimal Total
        +int QuantidadeTotalItens
        +bool EstaVazio
        +Carrinho()
        +Carrinho(int clienteId)
        +void AdicionarItem(Produto produto, int quantidade)
        +void RemoverItem(int produtoId)
        +void AlterarQuantidadeItem(int produtoId, int novaQuantidade)
        +void Limpar()
        +void ValidarParaFinalizacao()
    }

    class Pedido {
        -List~ItemCarrinho~ _itens
        +int Id
        +int ClienteId
        +Cliente Cliente
        +DateTime DataPedido
        +StatusPedido Status
        +decimal SubTotal
        +decimal ValorFrete
        +decimal ValorDesconto
        +decimal Total
        +IReadOnlyList~ItemCarrinho~ Itens
        +Pagamento Pagamento
        +string EnderecoEntrega
        +string Observacoes
        +Pedido()
        +Pedido(int clienteId, IEnumerable~ItemCarrinho~ itens)
        +Pedido CriarDe(Carrinho carrinho)$
        +void DefinirFrete(decimal valorFrete)
        +void AplicarDesconto(decimal valorDesconto)
        +void DefinirPagamento(Pagamento pagamento)
        +bool ProcessarPagamento()
        +void Cancelar(string motivo)
        +void IniciarPreparacao()
        +void IniciarTransito()
        +void ConfirmarEntrega()
    }

    class Pagamento {
        <<abstract>>
        -decimal _valor
        +int Id
        +decimal Valor
        +DateTime DataProcessamento
        +StatusPagamento Status
        #Pagamento()
        #Pagamento(decimal valor)
        +bool ProcessarPagamento()*
        +string ObterDescricaoMetodo()*
        +decimal CalcularTaxa()*
        +void ConfirmarPagamento()
        +void RejeitarPagamento(string motivo)
    }

    class PagamentoPix {
        -string _chavePix
        +string ChavePix
        +string QrCode
        +DateTime? DataVencimento
        +PagamentoPix()
        +PagamentoPix(decimal valor, string chavePix)
        +bool ProcessarPagamento()
        +string ObterDescricaoMetodo()
        +decimal CalcularTaxa()
        +bool EstaExpirado()
    }

    class PagamentoCartao {
        -string _numeroCartao
        -string _nomeTitular
        -string _cvv
        +string NumeroCartao
        +string NomeTitular
        +string Cvv
        +DateTime DataVencimento
        +TipoCartao Tipo
        +int Parcelas
        +PagamentoCartao()
        +PagamentoCartao(decimal valor, string numeroCartao, string nomeTitular, string cvv, DateTime dataVencimento, TipoCartao tipo, int parcelas)
        +bool ProcessarPagamento()
        +string ObterDescricaoMetodo()
        +decimal CalcularTaxa()
        +decimal CalcularValorComTaxa()
        +string ObterNumeroMascarado()
    }

    class ICalculadoraFrete {
        <<interface>>
        +decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor)
        +string ObterDescricao()
        +TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino)
    }

    class FreteCorreios {
        +decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor)
        +string ObterDescricao()
        +TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino)
    }

    class FreteSedex {
        +decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor)
        +string ObterDescricao()
        +TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino)
    }

    class ICalculadoraDesconto {
        <<interface>>
        +decimal CalcularDesconto(decimal valorTotal, string codigoCupom)
        +bool ValidarCupom(string codigoCupom)
        +string ObterDescricao()
    }

    class DescontoCupom {
        -Dictionary~string, tuple~ _cupons
        +DescontoCupom()
        +decimal CalcularDesconto(decimal valorTotal, string codigoCupom)
        +bool ValidarCupom(string codigoCupom)
        +string ObterDescricao()
        +IReadOnlyDictionary~string, tuple~ ObterCuponsValidos()
    }

    class StatusPedido {
        <<enumeration>>
        Pendente
        ProcessandoPagamento
        Confirmado
        EmPreparacao
        EmTransito
        Entregue
        Cancelado
        PagamentoRejeitado
    }

    class StatusPagamento {
        <<enumeration>>
        Pendente
        Processando
        Aprovado
        Rejeitado
        Cancelado
    }

    class TipoCartao {
        <<enumeration>>
        Debito
        Credito
    }

    %% Relacionamentos
    Cliente ||--o{ Carrinho : "1..1"
    Cliente ||--o{ Pedido : "1..*"
    Carrinho ||--o{ ItemCarrinho : "0..*"
    Pedido ||--o{ ItemCarrinho : "1..*"
    ItemCarrinho }o--|| Produto : "1..1"
    Pedido ||--o| Pagamento : "0..1"
    
    %% Herança
    Pagamento <|-- PagamentoPix
    Pagamento <|-- PagamentoCartao
    
    %% Implementações
    ICalculadoraFrete <|.. FreteCorreios
    ICalculadoraFrete <|.. FreteSedex
    ICalculadoraDesconto <|.. DescontoCupom
    
    %% Enumerações
    Pedido --> StatusPedido
    Pagamento --> StatusPagamento
    PagamentoCartao --> TipoCartao
```

## Multiplicidades e Relacionamentos:

### Relacionamentos de Composição/Agregação:
- **Cliente** para **Carrinho**: 1..1 (um cliente pode ter no máximo um carrinho ativo)
- **Cliente** para **Pedido**: 1..* (um cliente pode ter vários pedidos)
- **Carrinho** para **ItemCarrinho**: 0..* (carrinho pode estar vazio ou ter vários itens)
- **Pedido** para **ItemCarrinho**: 1..* (pedido deve ter pelo menos um item)
- **ItemCarrinho** para **Produto**: 1..1 (cada item referencia exatamente um produto)
- **Pedido** para **Pagamento**: 0..1 (pedido pode não ter pagamento ainda ou ter um)

### Hierarquia de Herança:
- **Pagamento** é classe abstrata com métodos virtuais e abstratos
- **PagamentoPix** e **PagamentoCartao** herdam de **Pagamento**
- Implementação de polimorfismo através dos métodos abstratos

### Interfaces para Extensibilidade:
- **ICalculadoraFrete** implementada por diferentes estratégias de frete
- **ICalculadoraDesconto** implementada por diferentes tipos de desconto
- Pattern Strategy para cálculos variáveis sem condicionais espalhadas