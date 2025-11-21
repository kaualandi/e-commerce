using Dominio.Exceptions;

namespace Dominio.Entidades.Pagamentos
{
    public abstract class Pagamento
    {
        private decimal _valor;

        public int Id { get; set; }
        public decimal Valor 
        { 
            get => _valor; 
            set => _valor = value > 0 ? value : throw new PrecoInvalidoException(value);
        }
        public DateTime DataProcessamento { get; set; }
        public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;

        protected Pagamento()
        {
        }

        protected Pagamento(decimal valor)
        {
            Valor = valor;
            DataProcessamento = DateTime.Now;
        }

        public abstract bool ProcessarPagamento();
        public abstract string ObterDescricaoMetodo();
        public abstract decimal CalcularTaxa();

        public virtual void ConfirmarPagamento()
        {
            if (Status != StatusPagamento.Processando)
                throw new PagamentoInvalidoException("Pagamento deve estar em processamento para ser confirmado");
            
            Status = StatusPagamento.Aprovado;
        }

        public virtual void RejeitarPagamento(string motivo)
        {
            Status = StatusPagamento.Rejeitado;
        }
    }

    public enum StatusPagamento
    {
        Pendente,
        Processando,
        Aprovado,
        Rejeitado,
        Cancelado
    }
}