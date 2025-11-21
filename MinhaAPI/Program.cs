using Application;
using Dominio.Interfaces;
using Infraestrutura.Repositorios;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Repositories
builder.Services.AddSingleton<IProdutoRepository, ProdutoRepository>();
builder.Services.AddSingleton<IClienteRepository, ClienteRepository>();
builder.Services.AddSingleton<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddSingleton<IPedidoRepository, PedidoRepository>();

// Calculadoras (Strategy Pattern)
builder.Services.AddScoped<ICalculadoraFrete, FreteCorreios>();
builder.Services.AddScoped<ICalculadoraDesconto, DescontoCupom>();

// Services
builder.Services.AddScoped<ProdutosService>();
builder.Services.AddScoped<ClientesService>();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<PedidoService>();

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();
