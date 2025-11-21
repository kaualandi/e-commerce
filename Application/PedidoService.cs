using Application.DTOs;
using Dominio.Entidades;
using Dominio.Entidades.Pagamentos;
using Dominio.Exceptions;
using Dominio.Interfaces;

namespace Application
{
    public class PedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICalculadoraFrete _calculadoraFrete;
        private readonly ICalculadoraDesconto _calculadoraDesconto;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            IClienteRepository clienteRepository,
            IProdutoRepository produtoRepository,
            ICalculadoraFrete calculadoraFrete,
            ICalculadoraDesconto calculadoraDesconto)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _clienteRepository = clienteRepository;
            _produtoRepository = produtoRepository;
            _calculadoraFrete = calculadoraFrete;
            _calculadoraDesconto = calculadoraDesconto;
        }

        public async Task<IEnumerable<PedidoDTO>> ObterPedidosDoClienteAsync(int clienteId)
        {
            var pedidos = await _pedidoRepository.ObterPorClienteIdAsync(clienteId);
            return await Task.WhenAll(pedidos.Select(MapearParaDTOAsync));
        }

        public async Task<PedidoDTO?> ObterPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            return pedido != null ? await MapearParaDTOAsync(pedido) : null;
        }

        public async Task<IEnumerable<PedidoDTO>> ObterTodosAsync()
        {
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            return await Task.WhenAll(pedidos.Select(MapearParaDTOAsync));
        }

        public async Task<int> CriarPedidoAsync(CriarPedidoDTO criarPedidoDto)
        {
            // Verificar se cliente existe
            if (!await _clienteRepository.ExisteAsync(criarPedidoDto.ClienteId))
                throw new ClienteNaoEncontradoException(criarPedidoDto.ClienteId);

            // Obter carrinho do cliente
            var carrinho = await _carrinhoRepository.ObterPorClienteIdAsync(criarPedidoDto.ClienteId);
            if (carrinho == null || carrinho.EstaVazio)
                throw new CarrinhoVazioException();

            // Carregar produtos nos itens do carrinho
            await CarregarProdutosDoCarrinho(carrinho);

            // Criar pedido
            var pedido = Pedido.CriarDe(carrinho);
            pedido.EnderecoEntrega = criarPedidoDto.EnderecoEntrega;
            pedido.Observacoes = criarPedidoDto.Observacoes;

            // Calcular frete
            var calculadoraFrete = ObterCalculadoraFrete(criarPedidoDto.TipoFrete);
            var valorFrete = calculadoraFrete.CalcularFrete("01000-000", criarPedidoDto.CepDestino, 1, carrinho.Total);
            pedido.DefinirFrete(valorFrete);

            // Aplicar desconto se houver cupom
            if (!string.IsNullOrWhiteSpace(criarPedidoDto.CodigoCupom))
            {
                var valorDesconto = _calculadoraDesconto.CalcularDesconto(pedido.SubTotal, criarPedidoDto.CodigoCupom);
                pedido.AplicarDesconto(valorDesconto);
            }

            var pedidoId = await _pedidoRepository.AdicionarAsync(pedido);
            
            // Limpar carrinho após criar pedido
            carrinho.Limpar();
            await _carrinhoRepository.AtualizarAsync(carrinho);

            return pedidoId;
        }

        public async Task<bool> ProcessarPagamentoAsync(FinalizarPedidoDTO finalizarPedidoDto)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(finalizarPedidoDto.PedidoId);
            if (pedido == null)
                throw new ArgumentException($"Pedido {finalizarPedidoDto.PedidoId} não encontrado");

            var pagamento = CriarPagamento(finalizarPedidoDto.Pagamento, pedido.Total);
            pedido.DefinirPagamento(pagamento);

            var sucesso = pedido.ProcessarPagamento();
            await _pedidoRepository.AtualizarAsync(pedido);

            return sucesso;
        }

        public async Task AtualizarStatusAsync(int pedidoId, StatusPedido novoStatus)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);
            if (pedido == null)
                throw new ArgumentException($"Pedido {pedidoId} não encontrado");

            switch (novoStatus)
            {
                case StatusPedido.EmPreparacao:
                    pedido.IniciarPreparacao();
                    break;
                case StatusPedido.EmTransito:
                    pedido.IniciarTransito();
                    break;
                case StatusPedido.Entregue:
                    pedido.ConfirmarEntrega();
                    break;
                case StatusPedido.Cancelado:
                    pedido.Cancelar();
                    break;
            }

            await _pedidoRepository.AtualizarAsync(pedido);
        }

        private async Task CarregarProdutosDoCarrinho(Carrinho carrinho)
        {
            foreach (var item in carrinho.Itens)
            {
                if (item.Produto == null)
                {
                    item.Produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                }
            }
        }

        private ICalculadoraFrete ObterCalculadoraFrete(TipoFrete tipoFrete)
        {
            return tipoFrete switch
            {
                TipoFrete.Pac => new FreteCorreios(),
                TipoFrete.Sedex => new FreteSedex(),
                TipoFrete.FreteGratis => new FreteGratis(),
                _ => new FreteCorreios()
            };
        }

        private static Pagamento CriarPagamento(PagamentoDTO? pagamentoDto, decimal valor)
        {
            return pagamentoDto switch
            {
                PagamentoPixDTO pix => new PagamentoPix(valor, pix.ChavePix),
                PagamentoCartaoDTO cartao => new PagamentoCartao(
                    valor, cartao.NumeroCartao, cartao.NomeTitular, 
                    cartao.Cvv, cartao.DataVencimento, cartao.Tipo, cartao.Parcelas),
                _ => throw new PagamentoInvalidoException("Tipo de pagamento não suportado")
            };
        }

        private async Task<PedidoDTO> MapearParaDTOAsync(Pedido pedido)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(pedido.ClienteId);
            
            var itens = new List<ItemCarrinhoDTO>();
            foreach (var item in pedido.Itens)
            {
                var produto = item.Produto ?? await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                
                itens.Add(new ItemCarrinhoDTO
                {
                    Id = item.Id,
                    ProdutoId = item.ProdutoId,
                    NomeProduto = produto?.Nome ?? "Produto não encontrado",
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario,
                    Subtotal = item.Subtotal
                });
            }

            PagamentoDTO? pagamentoDto = null;
            if (pedido.Pagamento != null)
            {
                pagamentoDto = pedido.Pagamento switch
                {
                    PagamentoPix pix => new PagamentoPixDTO { Valor = pix.Valor, ChavePix = pix.ChavePix },
                    PagamentoCartao cartao => new PagamentoCartaoDTO 
                    { 
                        Valor = cartao.Valor,
                        NumeroCartao = cartao.ObterNumeroMascarado(),
                        NomeTitular = cartao.NomeTitular,
                        Tipo = cartao.Tipo,
                        Parcelas = cartao.Parcelas
                    },
                    _ => null
                };
            }

            return new PedidoDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = cliente?.Nome ?? "Cliente não encontrado",
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                Itens = itens,
                SubTotal = pedido.SubTotal,
                ValorFrete = pedido.ValorFrete,
                ValorDesconto = pedido.ValorDesconto,
                Total = pedido.Total,
                Pagamento = pagamentoDto,
                EnderecoEntrega = pedido.EnderecoEntrega,
                Observacoes = pedido.Observacoes
            };
        }
    }
}