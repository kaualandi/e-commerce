namespace Dominio.Interfaces
{
    public interface ICalculadoraFrete
    {
        decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor);
        string ObterDescricao();
        TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino);
    }

    public interface ICalculadoraDesconto
    {
        decimal CalcularDesconto(decimal valorTotal, string? codigoCupom = null);
        bool ValidarCupom(string codigoCupom);
        string ObterDescricao();
    }

    public interface IProcessadorPagamento
    {
        Task<bool> ProcessarPagamentoAsync(Entidades.Pagamentos.Pagamento pagamento);
        decimal CalcularTaxa(Entidades.Pagamentos.Pagamento pagamento);
        string ObterProvedor();
    }

    public interface IValidadorEstoque
    {
        bool ValidarDisponibilidade(int produtoId, int quantidade);
        Task<bool> ReservarEstoqueAsync(int produtoId, int quantidade);
        Task LiberarReservaAsync(int produtoId, int quantidade);
    }
}