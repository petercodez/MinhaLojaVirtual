using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaLoja.Core.DTOs;
using MinhaLoja.Core.Models;
using MinhaLoja.Core.Data;

namespace MinhaLoja.web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Exige o Token JWT do usuário
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("finalizar")]
    public async Task<IActionResult> FinalizarPedido([FromBody] FinalizarPedidoDTO dto)
    {
        // 1. Pega o ID de login do usuário logado
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (usuarioId == null) return Unauthorized();

        // 2. Procura a ficha de Cliente desse usuário para amarrar no seu Pedido.ClienteId
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        if (cliente == null) 
            return BadRequest(new { Mensagem = "Ficha de cliente não encontrada." });

        if (dto.ProdutoIds == null || !dto.ProdutoIds.Any())
            return BadRequest(new { Mensagem = "O carrinho está vazio." });

        // 3. Busca os preços REAIS e atualizados no banco de dados
        var produtos = await _context.Produtos
            .Where(p => dto.ProdutoIds.Contains(p.Id))
            .ToListAsync();

        // 4. Instancia a sua classe Pedido
        var novoPedido = new Pedido
        {
            ClienteId = cliente.Id, // Usa a chave estrangeira perfeita que você criou
            DataPedido = DateTime.UtcNow,
            Status = StatusPedido.AguardadoPagamento, // Usa o seu Enum!
            Itens = new List<PedidoItem>()
        };

        decimal totalDoPedido = 0;

        // 5. Preenche os itens do pedido (Sua classe PedidoItem)
        foreach (var id in dto.ProdutoIds)
        {
            var produtoReal = produtos.FirstOrDefault(p => p.Id == id);
            if (produtoReal != null)
            {
                novoPedido.Itens.Add(new PedidoItem
                {
                    ProdutoId = produtoReal.Id,
                    PrecoUnitario = produtoReal.Preco,
                    Quantidade = 1 // Cada clique de compra no site adicionou 1 unidade
                });
                
                totalDoPedido += produtoReal.Preco;
            }
        }

        novoPedido.ValorTotal = totalDoPedido;

        // 6. Grava no PostgreSQL!
        _context.Pedidos.Add(novoPedido);
        await _context.SaveChangesAsync();

        return Ok(new { Mensagem = "Pedido realizado com sucesso!", PedidoId = novoPedido.Id });
    }

    [HttpGet("meus-pedidos")]
    public async Task<IActionResult> ObterMeusPedidos()
    {
        // 1. Descobre quem é o usuário logado
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (usuarioId == null) return Unauthorized();

        // 2. Acha a ficha de cliente dele
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        if (cliente == null) return Ok(new List<PedidoReadDTO>()); // Se não tem ficha, não tem pedido

        // 3. Busca os pedidos dele, incluindo os itens e as informações dos produtos
        var pedidos = await _context.Pedidos
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto) // Faz o JOIN com a tabela de Produtos para pegar o Nome
            .Where(p => p.ClienteId == cliente.Id)
            .OrderByDescending(p => p.DataPedido) // Os mais recentes aparecem primeiro!
            .Select(p => new PedidoReadDTO
            {
                Id = p.Id,
                DataPedido = p.DataPedido,
                ValorTotal = p.ValorTotal,
                Status = p.Status.ToString(), // Converte o seu Enum para texto
                Itens = p.Itens.Select(i => new ItemPedidoReadDTO
                {
                    NomeProduto = i.Produto.Nome,
                    PrecoUnitario = i.PrecoUnitario,
                    Quantidade = i.Quantidade
                }).ToList()
            })
            .ToListAsync();

        return Ok(pedidos);
    }
}