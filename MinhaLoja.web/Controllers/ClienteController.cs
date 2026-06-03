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
[Authorize] // Exige que o token JWT (Login) seja enviado na requisição
public class ClienteController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClienteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("meu-perfil")]
    public async Task<IActionResult> ObterMeuPerfil()
    {
        // Pega o ID do usuário diretamente do "crachá" (Token JWT)
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (usuarioId == null) return Unauthorized();

        // Procura no PostgreSQL se já existe uma ficha de Cliente atrelada a esse ID de Login
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (cliente == null)
            return NotFound(new { Mensagem = "Ficha cadastral não encontrada." });

        // Se achou, empacota no DTO e manda pra tela
        var dto = new ClienteDTO
        {
            NomeCompleto = cliente.NomeCompleto,
            CPF = cliente.CPF,
            Telefone = cliente.Telefone ?? string.Empty,
            EnderecoCompleto = cliente.EnderecoCompleto
        };

        return Ok(dto);
    }

    [HttpPost("salvar-perfil")]
    public async Task<IActionResult> SalvarPerfil([FromBody] ClienteDTO dto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (usuarioId == null) return Unauthorized();

        // Verifica se o cliente já existe
        var clienteExistente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (clienteExistente != null)
        {
            // Se já existe, apenas ATUALIZA os dados (edição)
            clienteExistente.NomeCompleto = dto.NomeCompleto;
            clienteExistente.CPF = dto.CPF;
            clienteExistente.Telefone = dto.Telefone;
            clienteExistente.EnderecoCompleto = dto.EnderecoCompleto;
            
            _context.Clientes.Update(clienteExistente);
        }
        else
        {
            // Se não existe, CRIA uma nova ficha (primeira vez)
            var novoCliente = new Cliente
            {
                NomeCompleto = dto.NomeCompleto,
                CPF = dto.CPF,
                Telefone = dto.Telefone,
                EnderecoCompleto = dto.EnderecoCompleto,
                UsuarioId = usuarioId
            };
            
            await _context.Clientes.AddAsync(novoCliente);
        }

        // Salva tudo de fato no PostgreSQL
        await _context.SaveChangesAsync();

        return Ok(new { Mensagem = "Dados salvos com sucesso!" });
    }
}