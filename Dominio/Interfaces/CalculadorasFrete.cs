namespace Dominio.Interfaces
{
    public class FreteCorreios : ICalculadoraFrete
    {
        public decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor)
        {
            // Simulação de cálculo dos Correios
            var distancia = SimularDistancia(cepOrigem, cepDestino);
            var valorBase = peso * 0.5m;
            var valorDistancia = distancia * 0.1m;
            
            return Math.Round(valorBase + valorDistancia, 2);
        }

        public string ObterDescricao()
        {
            return "Correios PAC";
        }

        public TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino)
        {
            var distancia = SimularDistancia(cepOrigem, cepDestino);
            return TimeSpan.FromDays(distancia < 100 ? 3 : 7);
        }

        private int SimularDistancia(string cepOrigem, string cepDestino)
        {
            // Simulação simples baseada nos CEPs
            var hash1 = cepOrigem.GetHashCode();
            var hash2 = cepDestino.GetHashCode();
            return Math.Abs((hash1 - hash2) % 1000) + 50;
        }
    }

    public class FreteSedex : ICalculadoraFrete
    {
        public decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor)
        {
            var freteCorreios = new FreteCorreios();
            return freteCorreios.CalcularFrete(cepOrigem, cepDestino, peso, valor) * 1.5m; // SEDEX é 50% mais caro
        }

        public string ObterDescricao()
        {
            return "Correios SEDEX";
        }

        public TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino)
        {
            var freteCorreios = new FreteCorreios();
            var tempoCorreios = freteCorreios.ObterTempoEstimado(cepOrigem, cepDestino);
            return TimeSpan.FromDays(Math.Max(1, tempoCorreios.Days / 2)); // SEDEX é mais rápido
        }
    }

    public class FreteGratis : ICalculadoraFrete
    {
        private readonly decimal _valorMinimoFreteGratis;

        public FreteGratis(decimal valorMinimoFreteGratis = 100m)
        {
            _valorMinimoFreteGratis = valorMinimoFreteGratis;
        }

        public decimal CalcularFrete(string cepOrigem, string cepDestino, decimal peso, decimal valor)
        {
            return valor >= _valorMinimoFreteGratis ? 0 : new FreteCorreios().CalcularFrete(cepOrigem, cepDestino, peso, valor);
        }

        public string ObterDescricao()
        {
            return $"Frete Grátis (pedidos acima de {_valorMinimoFreteGratis:C})";
        }

        public TimeSpan ObterTempoEstimado(string cepOrigem, string cepDestino)
        {
            return new FreteCorreios().ObterTempoEstimado(cepOrigem, cepDestino);
        }
    }
}