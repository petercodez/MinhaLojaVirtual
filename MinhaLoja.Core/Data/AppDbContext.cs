using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinhaLoja.Core.Models;

namespace MinhaLoja.Core.Data;

// O segredo está aqui: herdar de DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Se essa linha não estiver aqui, o EF acha que o banco está vazio
    public DbSet<Produto> Produtos { get; set; }
}