using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MinhaLoja.Core.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // Use o mesmo caminho do banco que você definiu no Web
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=lojavirtual_db;Username=postgres;Password=AdminLoja@123");

        return new AppDbContext(optionsBuilder.Options);
    }
}