using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaLoja.Core.Data;
using MinhaLoja.Core.Models;

namespace MinhaLoja.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/categorias
    // Retorna todas as categorias cadastradas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
    {
        return await _context.Categorias.ToListAsync();
    }

    // GET: api/categorias/1
    // Retorna uma categoria específica e, opcionalmente, seus produtos
    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Produtos) // Inclui a lista de produtos desta categoria
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound();
        }

        return categoria;
    }
}