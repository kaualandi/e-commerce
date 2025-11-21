using Dominio.Exceptions;

namespace Dominio.Entidades.Pagamentos
{
    public class PagamentoPix : Pagamento
    {
        private string _chavePix = string.Empty;

        public string ChavePix 
        { 
            get => _chavePix; 
            set => _chavePix = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Chave PIX não pode ser vazia");
        }
        
        public string QrCode { get; set; } = string.Empty;
        public DateTime? DataVencimento { get; set; }

        public PagamentoPix() : base()
        {
        }

        public PagamentoPix(decimal valor, string chavePix) : base(valor)
        {
            ChavePix = chavePix;
            DataVencimento = DateTime.Now.AddMinutes(30); // PIX expira em 30 minutos
        }

        public override bool ProcessarPagamento()
        {
            try
            {
                Status = StatusPagamento.Processando;
                
                // Simula processamento PIX
                if (DateTime.Now > DataVencimento)
                {
                    RejeitarPagamento("PIX expirado");
                    return false;
                }

                GerarQrCode();
                
                // Simula aprovação automática para PIX
                ConfirmarPagamento();
                return true;
            }
            catch (Exception)
            {
                RejeitarPagamento("Erro no processamento PIX");
                return false;
            }
        }

        public override string ObterDescricaoMetodo()
        {
            return "PIX";
        }

        public override decimal CalcularTaxa()
        {
            return 0; // PIX não tem taxa
        }

        private void GerarQrCode()
        {
            // Simula geração de QR Code
            QrCode = $"PIX_{ChavePix}_{Valor}_{DateTime.Now:yyyyMMddHHmmss}";
        }

        public bool EstaExpirado()
        {
            return DataVencimento.HasValue && DateTime.Now > DataVencimento;
        }
    }
}