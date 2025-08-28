using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;

    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;

        // Testa a conexão ao criar o contexto
        try
        {
            this.Database.CanConnect();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Não foi possível conectar ao banco de dados MySQL. " +
                "Verifique se o servidor está rodando e se a connection string está correta.", ex);
        }
    }

    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("MySql");
            if (string.IsNullOrEmpty(stringConexao))
            {
                throw new InvalidOperationException("A connection string do MySQL não foi encontrada.");
            }

            optionsBuilder.UseMySql(
                stringConexao,
                new MySqlServerVersion(new Version(8, 0, 43)) // versão do seu MySQL
            );
        }
    }
}