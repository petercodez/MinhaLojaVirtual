using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MinhaLoja.Core.Models;

namespace MinhaLoja.Core.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "O Slug deve conter apenas letras minúsculas, números e hífens")]
        public string Slug { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}