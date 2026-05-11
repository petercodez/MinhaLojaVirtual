using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinhaLoja.Core.Models;

namespace MinhaLoja.Core.Data;

// O segredo está aqui: herdar de DbContext
public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Se essa linha não estiver aqui, o EF acha que o banco está vazio
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Seed de Categorias
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Guitarras", Slug = "guitarras", Descricao = "Instrumentos de cordas e acessórios" },
            new Categoria { Id = 2, Nome = "Informática", Slug = "informatica", Descricao = "Componentes e periféricos" }
        );

        // 2. Seed de Produtos
        modelBuilder.Entity<Produto>().HasData(
            new Produto 
            { 
                Id = 1, 
                Nome = "Gibson Les Paul Tribute", 
                Preco = 8500.00m, 
                CategoriaId = 1, // Relaciona com Guitarras
                Descricao = "Acabamento Satin, captadores 490R e 490T." 
            },
            new Produto 
            { 
                Id = 2, 
                Nome = "Fender Stratocaster Player", 
                Preco = 7200.00m, 
                CategoriaId = 1, 
                Descricao = "Corpo em Alder, braço em Maple." 
            },
            new Produto 
            { 
                Id = 3, 
                Nome = "SSD 1TB NVMe", 
                Preco = 450.00m, 
                CategoriaId = 2, // Relaciona com Informática
                Descricao = "Velocidade de leitura de até 3500MB/s." 
            }
        );
    }
}