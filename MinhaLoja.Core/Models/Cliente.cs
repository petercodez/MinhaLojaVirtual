using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Text;

namespace MinhaLoja.Core.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(200, MinimumLength = 3)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(14)] // Formato: 000.000.000-00
        public string CPF { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefone { get; set; }

        [Required]
        [StringLength(250)]
        public string EnderecoCompleto { get; set; } = string.Empty;

        // Chave Estrangeira para Usuario (1:1)
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario{ get; set; } = null!;

        // Propriedade de Navegação: Um Cliente pode ter muitos pedidos (1:N)
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}