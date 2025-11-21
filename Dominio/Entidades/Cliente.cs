namespace Dominio.Entidades
{
    public class Cliente
    {
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private string _cpf = string.Empty;

        public int Id { get; set; }
        
        public string Nome 
        { 
            get => _nome; 
            set => _nome = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Nome não pode ser vazio");
        }
        
        public string Email 
        { 
            get => _email; 
            set => _email = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Email não pode ser vazio");
        }
        
        public string Cpf 
        { 
            get => _cpf; 
            set => _cpf = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("CPF não pode ser vazio");
        }
        
        public string Telefone { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public bool Ativo { get; set; } = true;

        public Cliente()
        {
        }

        public Cliente(string nome, string email, string cpf, string telefone = "")
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Telefone = telefone;
        }
    }
}