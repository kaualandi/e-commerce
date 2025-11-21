using Dominio.Exceptions;

namespace Dominio.Entidades.Pagamentos
{
    public class PagamentoCartao : Pagamento
    {
        private string _numeroCartao = string.Empty;
        private string _nomeTitular = string.Empty;
        private string _cvv = string.Empty;

        public string NumeroCartao 
        { 
            get => _numeroCartao; 
            set => _numeroCartao = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Número do cartão não pode ser vazio");
        }
        
        public string NomeTitular 
        { 
            get => _nomeTitular; 
            set => _nomeTitular = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Nome do titular não pode ser vazio");
        }
        
        public string Cvv 
        { 
            get => _cvv; 
            set => _cvv = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("CVV não pode ser vazio");
        }
        
        public DateTime DataVencimento { get; set; }
        public TipoCartao Tipo { get; set; }
        public int Parcelas { get; set; } = 1;

        public PagamentoCartao() : base()
        {
        }

        public PagamentoCartao(decimal valor, string numeroCartao, string nomeTitular, string cvv, DateTime dataVencimento, TipoCartao tipo, int parcelas = 1) 
            : base(valor)
        {
            NumeroCartao = numeroCartao;
            NomeTitular = nomeTitular;
            Cvv = cvv;
            DataVencimento = dataVencimento;
            Tipo = tipo;
            Parcelas = parcelas > 0 ? parcelas : throw new ArgumentException("Número de parcelas deve ser maior que zero");
        }

        public override bool ProcessarPagamento()
        {
            try
            {
                Status = StatusPagamento.Processando;
                
                if (!ValidarCartao())
                {
                    RejeitarPagamento("Dados do cartão inválidos");
                    return false;
                }

                // Simula processamento da operadora
                if (SimularProcessamentoOperadora())
                {
                    ConfirmarPagamento();
                    return true;
                }
                else
                {
                    RejeitarPagamento("Transação negada pela operadora");
                    return false;
                }
            }
            catch (Exception ex)
            {
                RejeitarPagamento($"Erro no processamento: {ex.Message}");
                return false;
            }
        }

        public override string ObterDescricaoMetodo()
        {
            return $"Cartão {Tipo} - {Parcelas}x";
        }

        public override decimal CalcularTaxa()
        {
            return Tipo switch
            {
                TipoCartao.Debito => 0.02m, // 2%
                TipoCartao.Credito when Parcelas == 1 => 0.03m, // 3% à vista
                TipoCartao.Credito => 0.05m, // 5% parcelado
                _ => 0
            };
        }

        private bool ValidarCartao()
        {
            // Validações básicas
            if (NumeroCartao.Length < 13 || NumeroCartao.Length > 19)
                return false;
            
            if (Cvv.Length < 3 || Cvv.Length > 4)
                return false;
            
            if (DataVencimento < DateTime.Now.Date)
                return false;
            
            return true;
        }

        private bool SimularProcessamentoOperadora()
        {
            // Simula uma taxa de aprovação de 90%
            var random = new Random();
            return random.Next(1, 11) <= 9;
        }

        public decimal CalcularValorComTaxa()
        {
            return Valor + (Valor * CalcularTaxa());
        }

        public string ObterNumeroMascarado()
        {
            if (NumeroCartao.Length < 4)
                return "****";
            
            return "**** **** **** " + NumeroCartao[^4..];
        }
    }

    public enum TipoCartao
    {
        Debito,
        Credito
    }
}