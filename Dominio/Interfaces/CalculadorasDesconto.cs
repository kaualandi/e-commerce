namespace Dominio.Interfaces
{
    public class DescontoPorcentagem : ICalculadoraDesconto
    {
        private readonly decimal _percentual;
        private readonly decimal _valorMinimo;

        public DescontoPorcentagem(decimal percentual, decimal valorMinimo = 0)
        {
            _percentual = percentual;
            _valorMinimo = valorMinimo;
        }

        public decimal CalcularDesconto(decimal valorTotal, string? codigoCupom = null)
        {
            if (valorTotal < _valorMinimo)
                return 0;

            return valorTotal * (_percentual / 100);
        }

        public bool ValidarCupom(string codigoCupom)
        {
            return true; // Para desconto percentual simples, sempre válido
        }

        public string ObterDescricao()
        {
            return $"Desconto de {_percentual}% (valor mínimo: {_valorMinimo:C})";
        }
    }

    public class DescontoValorFixo : ICalculadoraDesconto
    {
        private readonly decimal _valorDesconto;
        private readonly decimal _valorMinimo;

        public DescontoValorFixo(decimal valorDesconto, decimal valorMinimo)
        {
            _valorDesconto = valorDesconto;
            _valorMinimo = valorMinimo;
        }

        public decimal CalcularDesconto(decimal valorTotal, string? codigoCupom = null)
        {
            if (valorTotal < _valorMinimo)
                return 0;

            return Math.Min(_valorDesconto, valorTotal);
        }

        public bool ValidarCupom(string codigoCupom)
        {
            return true;
        }

        public string ObterDescricao()
        {
            return $"Desconto fixo de {_valorDesconto:C} (valor mínimo: {_valorMinimo:C})";
        }
    }

    public class DescontoCupom : ICalculadoraDesconto
    {
        private readonly Dictionary<string, (decimal percentual, DateTime validade, decimal valorMinimo)> _cupons;

        public DescontoCupom()
        {
            _cupons = new Dictionary<string, (decimal, DateTime, decimal)>
            {
                { "BEMVINDO10", (10, DateTime.Now.AddDays(30), 50) },
                { "BLACKFRIDAY", (20, DateTime.Now.AddDays(7), 100) },
                { "FRETE15", (15, DateTime.Now.AddDays(15), 0) }
            };
        }

        public decimal CalcularDesconto(decimal valorTotal, string? codigoCupom = null)
        {
            if (string.IsNullOrWhiteSpace(codigoCupom) || !ValidarCupom(codigoCupom))
                return 0;

            var cupom = _cupons[codigoCupom.ToUpper()];
            
            if (valorTotal < cupom.valorMinimo)
                return 0;

            return valorTotal * (cupom.percentual / 100);
        }

        public bool ValidarCupom(string codigoCupom)
        {
            if (string.IsNullOrWhiteSpace(codigoCupom))
                return false;

            var codigo = codigoCupom.ToUpper();
            
            if (!_cupons.ContainsKey(codigo))
                return false;

            return _cupons[codigo].validade > DateTime.Now;
        }

        public string ObterDescricao()
        {
            return "Desconto por cupom promocional";
        }

        public IReadOnlyDictionary<string, (decimal percentual, DateTime validade, decimal valorMinimo)> ObterCuponsValidos()
        {
            return _cupons.Where(c => c.Value.validade > DateTime.Now)
                          .ToDictionary(c => c.Key, c => c.Value);
        }
    }
}