using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MinhaLoja.Core.Data;
using MinhaLoja.Core.Models;
using MinhaLoja.Core.DTOs;

namespace MinhaLoja.Web.Controllers;

[Authorize] // <-- ESTA É A FECHADURA! Qualquer requisição sem token será barrada.
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // READ (GET) - Retorna a lista de produtos limpa
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoReadDTO>>> GetProdutos()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .ToListAsync();

        // Mapeamento: Model -> DTO
        var produtosDTO = produtos.Select(p => new ProdutoReadDTO
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Preco = p.Preco,
            CategoriaNome = p.Categoria != null ? p.Categoria.Nome : "Sem Categoria"
        }).ToList();

        return Ok(produtosDTO);
    }

    // READ (GET por ID) - Retorna apenas um produto limpo
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoReadDTO>> GetProduto(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null) return NotFound();

        // Mapeamento: Model -> DTO
        var produtoDTO = new ProdutoReadDTO
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            CategoriaNome = produto.Categoria != null ? produto.Categoria.Nome : "Sem Categoria"
        };

        return Ok(produtoDTO);
    }

    // CREATE (POST) - Recebe o DTO, salva o Model, retorna o DTO
    [HttpPost]
    public async Task<ActionResult<ProdutoReadDTO>> PostProduto(ProdutoCreateDTO dto)
    {
        // Mapeamento de Entrada: DTO -> Model
        var produto = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            CategoriaId = dto.CategoriaId
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        // Carrega os dados da Categoria do banco para podermos retornar o Nome dela
        await _context.Entry(produto).Reference(p => p.Categoria).LoadAsync();

        // Mapeamento de Saída: Model -> DTO
        var retorno = new ProdutoReadDTO 
        { 
            Id = produto.Id, 
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            CategoriaNome = produto.Categoria != null ? produto.Categoria.Nome : "Sem Categoria"
        };

        return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, retorno);
    }

    // UPDATE (PUT) - Atualiza os dados usando o DTO
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduto(int id, ProdutoCreateDTO dto)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        // Atualizamos o Model existente com os dados novos do DTO
        produto.Nome = dto.Nome;
        produto.Descricao = dto.Descricao;
        produto.Preco = dto.Preco;
        produto.CategoriaId = dto.CategoriaId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Produtos.Any(e => e.Id == id)) return NotFound();
            else throw;
        }

        return NoContent(); // 204 NoContent é o padrão de sucesso para PUT
    }

    // DELETE - Remove do banco
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 NoContent é o padrão de sucesso para DELETE
    }
}