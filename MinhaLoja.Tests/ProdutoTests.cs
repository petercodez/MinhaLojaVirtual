using Microsoft.EntityFrameworkCore;
using MinhaLoja.Core.Data; // Ajuste se o namespace do seu AppDbContext for diferente
using MinhaLoja.Core.Models; // Ajuste se o namespace da sua classe Produto for diferente

namespace MinhaLoja.Tests;

public class ProdutoTests
{
    // Método auxiliar que cria um banco de dados "falso" na memória RAM para cada teste
    private AppDbContext ObterContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Gera um banco novo pra não dar conflito
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Caso1_AdicionarProduto_DeveSalvarNoBanco()
    {
        // Arrange (Preparação)
        var context = ObterContextoEmMemoria();
        var produto = new Produto { Nome = "Guitarra Gibson", Preco = 8900, CategoriaId = 1 };

        // Act (Ação)
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        // Assert (Verificação)
        var produtoSalvo = await context.Produtos.FirstOrDefaultAsync(p => p.Nome == "Guitarra Gibson");
        Assert.NotNull(produtoSalvo);
        Assert.Equal(8900, produtoSalvo.Preco);
    }

    [Fact]
    public async Task Caso2_BuscarProdutoPorId_DeveRetornarProdutoCorreto()
    {
        var context = ObterContextoEmMemoria();
        var produto = new Produto { Nome = "Fender Stratocaster", Preco = 7200, CategoriaId = 1 };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        var produtoBuscado = await context.Produtos.FindAsync(produto.Id);

        Assert.NotNull(produtoBuscado);
        Assert.Equal("Fender Stratocaster", produtoBuscado.Nome);
    }

    [Fact]
    public async Task Caso3_ListarTodos_DeveRetornarListaCompleta()
    {
        var context = ObterContextoEmMemoria();
        context.Produtos.Add(new Produto { Nome = "Produto A", Preco = 10, CategoriaId = 1 });
        context.Produtos.Add(new Produto { Nome = "Produto B", Preco = 20, CategoriaId = 1 });
        await context.SaveChangesAsync();

        var lista = await context.Produtos.ToListAsync();

        Assert.Equal(2, lista.Count);
    }

    [Fact]
    public async Task Caso4_AtualizarProduto_DeveAlterarDadosNoBanco()
    {
        var context = ObterContextoEmMemoria();
        var produto = new Produto { Nome = "Pedal Boss", Preco = 500, CategoriaId = 2 };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        // Alterando o preço
        produto.Preco = 450;
        context.Produtos.Update(produto);
        await context.SaveChangesAsync();

        var produtoAtualizado = await context.Produtos.FindAsync(produto.Id);
        Assert.Equal(450, produtoAtualizado!.Preco);
    }

    [Fact]
    public async Task Caso5_ExcluirProduto_DeveRemoverDoBanco()
    {
        var context = ObterContextoEmMemoria();
        var produto = new Produto { Nome = "Cabo P10", Preco = 50, CategoriaId = 2 };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        // Excluindo
        context.Produtos.Remove(produto);
        await context.SaveChangesAsync();

        var produtoRemovido = await context.Produtos.FindAsync(produto.Id);
        Assert.Null(produtoRemovido); // A verificação passa se o produto for nulo (não existir mais)
    }
}