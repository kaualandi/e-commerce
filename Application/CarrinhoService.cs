using Application.DTOs;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces;

namespace Application
{
    public class CarrinhoService
    {
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IClienteRepository _clienteRepository;

        public CarrinhoService(
            ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository,
            IClienteRepository clienteRepository)
        {
            _carrinhoRepository = carrinhoRepository;
            _produtoRepository = produtoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<CarrinhoDTO?> ObterCarrinhoDoClienteAsync(int clienteId)
        {
            var carrinho = await _carrinhoRepository.ObterPorClienteIdAsync(clienteId);
            return carrinho != null ? await MapearParaDTOAsync(carrinho) : null;
        }

        public async Task<CarrinhoDTO> ObterOuCriarCarrinhoAsync(int clienteId)
        {
            // Verificar se cliente existe
            if (!await _clienteRepository.ExisteAsync(clienteId))
                throw new ClienteNaoEncontradoException(clienteId);

            var carrinho = await _carrinhoRepository.ObterPorClienteIdAsync(clienteId);
            
            if (carrinho == null)
            {
                carrinho = new Carrinho(clienteId);
                carrinho.Id = await _carrinhoRepository.AdicionarAsync(carrinho);
            }

            return await MapearParaDTOAsync(carrinho);
        }

        public async Task AdicionarItemAsync(int clienteId, AdicionarItemCarrinhoDTO adicionarItemDto)
        {
            var carrinho = await ObterCarrinhoOuCriar(clienteId);
            var produto = await _produtoRepository.ObterPorIdAsync(adicionarItemDto.ProdutoId);
            
            if (produto == null)
                throw new ProdutoNaoEncontradoException(adicionarItemDto.ProdutoId);

            carrinho.AdicionarItem(produto, adicionarItemDto.Quantidade);
            await _carrinhoRepository.AtualizarAsync(carrinho);
        }

        public async Task RemoverItemAsync(int clienteId, int produtoId)
        {
            var carrinho = await ObterCarrinhoOuCriar(clienteId);
            carrinho.RemoverItem(produtoId);
            await _carrinhoRepository.AtualizarAsync(carrinho);
        }

        public async Task AlterarQuantidadeItemAsync(int clienteId, AlterarQuantidadeItemDTO alterarQuantidadeDto)
        {
            var carrinho = await ObterCarrinhoOuCriar(clienteId);
            carrinho.AlterarQuantidadeItem(alterarQuantidadeDto.ProdutoId, alterarQuantidadeDto.NovaQuantidade);
            await _carrinhoRepository.AtualizarAsync(carrinho);
        }

        public async Task LimparCarrinhoAsync(int clienteId)
        {
            var carrinho = await ObterCarrinhoOuCriar(clienteId);
            carrinho.Limpar();
            await _carrinhoRepository.AtualizarAsync(carrinho);
        }

        private async Task<Carrinho> ObterCarrinhoOuCriar(int clienteId)
        {
            var carrinho = await _carrinhoRepository.ObterPorClienteIdAsync(clienteId);
            
            if (carrinho == null)
            {
                if (!await _clienteRepository.ExisteAsync(clienteId))
                    throw new ClienteNaoEncontradoException(clienteId);

                carrinho = new Carrinho(clienteId);
                carrinho.Id = await _carrinhoRepository.AdicionarAsync(carrinho);
            }

            // Carregar produtos para os itens
            foreach (var item in carrinho.Itens)
            {
                if (item.Produto == null)
                {
                    item.Produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                }
            }

            return carrinho;
        }

        private async Task<CarrinhoDTO> MapearParaDTOAsync(Carrinho carrinho)
        {
            var itens = new List<ItemCarrinhoDTO>();
            
            foreach (var item in carrinho.Itens)
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

            return new CarrinhoDTO
            {
                Id = carrinho.Id,
                ClienteId = carrinho.ClienteId,
                Itens = itens,
                Total = carrinho.Total,
                QuantidadeTotalItens = carrinho.QuantidadeTotalItens,
                DataCriacao = carrinho.DataCriacao,
                DataAtualizacao = carrinho.DataAtualizacao
            };
        }
    }
}