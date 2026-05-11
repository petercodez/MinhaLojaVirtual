using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaLoja.Core.DTOs
{
    public class ProdutoCreateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }    
    }
}