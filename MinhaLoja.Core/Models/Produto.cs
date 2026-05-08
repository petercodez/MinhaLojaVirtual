using MinhaLoja.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinhaLoja.Core.Models;

public class Produto
{
    public int Id { get; set; }
    public  string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string ImageUrl { get; set; } = string.Empty; //"/images/placeholder.png";
    public int Estoque { get; set; }
    public int CategoriaId { get; set; } // Esta é a Chave Estrangeira que falta
    public Categoria Categoria { get; set; } = null!; // Propriedade de navegação
}
