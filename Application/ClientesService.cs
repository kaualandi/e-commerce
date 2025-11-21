using Application.DTOs;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces;

namespace Application
{
    public class ClientesService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<ClienteDTO>> ObterTodosAsync()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return clientes.Select(MapearParaDTO);
        }

        public async Task<ClienteDTO?> ObterPorIdAsync(int id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            return cliente != null ? MapearParaDTO(cliente) : null;
        }

        public async Task<ClienteDTO?> ObterPorEmailAsync(string email)
        {
            var cliente = await _clienteRepository.ObterPorEmailAsync(email);
            return cliente != null ? MapearParaDTO(cliente) : null;
        }

        public async Task<int> CriarAsync(CriarClienteDTO criarClienteDto)
        {
            // Validar se email já existe
            if (await _clienteRepository.ExisteEmailAsync(criarClienteDto.Email))
                throw new ArgumentException($"Email '{criarClienteDto.Email}' já está em uso");

            // Validar se CPF já existe
            if (await _clienteRepository.ExisteCpfAsync(criarClienteDto.Cpf))
                throw new ArgumentException($"CPF '{criarClienteDto.Cpf}' já está em uso");

            var cliente = new Cliente(
                criarClienteDto.Nome,
                criarClienteDto.Email,
                criarClienteDto.Cpf,
                criarClienteDto.Telefone
            );

            return await _clienteRepository.AdicionarAsync(cliente);
        }

        public async Task AtualizarAsync(int id, AtualizarClienteDTO atualizarClienteDto)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new ClienteNaoEncontradoException(id);

            if (!string.IsNullOrWhiteSpace(atualizarClienteDto.Nome))
                cliente.Nome = atualizarClienteDto.Nome;

            if (!string.IsNullOrWhiteSpace(atualizarClienteDto.Email))
            {
                // Verificar se o novo email já existe (exceto para o próprio cliente)
                var clienteComEmail = await _clienteRepository.ObterPorEmailAsync(atualizarClienteDto.Email);
                if (clienteComEmail != null && clienteComEmail.Id != id)
                    throw new ArgumentException($"Email '{atualizarClienteDto.Email}' já está em uso");

                cliente.Email = atualizarClienteDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(atualizarClienteDto.Telefone))
                cliente.Telefone = atualizarClienteDto.Telefone;

            if (atualizarClienteDto.Ativo.HasValue)
                cliente.Ativo = atualizarClienteDto.Ativo.Value;

            await _clienteRepository.AtualizarAsync(cliente);
        }

        public async Task RemoverAsync(int id)
        {
            if (!await _clienteRepository.ExisteAsync(id))
                throw new ClienteNaoEncontradoException(id);

            await _clienteRepository.RemoverAsync(id);
        }

        private static ClienteDTO MapearParaDTO(Cliente cliente)
        {
            return new ClienteDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Cpf = cliente.Cpf,
                Telefone = cliente.Telefone,
                DataCadastro = cliente.DataCadastro,
                Ativo = cliente.Ativo
            };
        }
    }
}