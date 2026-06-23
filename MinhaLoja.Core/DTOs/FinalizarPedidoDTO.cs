namespace MinhaLoja.Core.DTOs;

public class FinalizarPedidoDTO
{
    // Manda só os IDs para a API calcular o preço real no banco de dados
    public List<int> ProdutoIds { get; set; } = new();
}