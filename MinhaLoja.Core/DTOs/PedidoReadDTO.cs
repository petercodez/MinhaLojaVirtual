namespace MinhaLoja.Core.DTOs;

public class PedidoReadDTO
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ItemPedidoReadDTO> Itens { get; set; } = new();
}

public class ItemPedidoReadDTO
{
    public string NomeProduto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
}