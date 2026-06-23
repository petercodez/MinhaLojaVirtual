using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaLoja.Core.Models;

// Enum para gerenciar os estados do pedido (pode colocar em um arquivo separado na pasta Enums)
public enum StatusPedido
{
    AguardadoPagamento = 1,
    Processando = 2,
    Enviado = 3,
    Entregue = 4,
    Cancelado = 5
}


public class Pedido
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(18,2)")] // Garante precisão financeira
    public decimal ValorTotal { get; set; }

    [Required]
    public StatusPedido Status { get; set; } = StatusPedido.AguardadoPagamento;

    // Chave Estrangeira para Cliente (N:1)
    [Required]
    public int ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente Cliente { get; set; } = null!;

    // Propriedade de Navegação: Um pedido contém vários itens (1:N)
    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();  
}

// Classe auxiliar necessário para o relacionamento do Pedido
// Aggregate (Agregado)
public class PedidoItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PedidoId { get; set; }

    [ForeignKey(nameof(PedidoId))]
    public Pedido Pedido { get; set; } = null!;

    [Required]
    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    [Required]
    public int Quantidade { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecoUnitario { get; set; }
}
