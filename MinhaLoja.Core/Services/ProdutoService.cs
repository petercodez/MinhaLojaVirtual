using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinhaLoja.Core.Models;

namespace MinhaLoja.Core.Services
{
    public class ProdutoService
    {
        private readonly List<Produto> _produtos = new()
        {
            new Produto { Id = 1, Nome = "Monitor Zowie 240hz", Preco = 2.500m, Descricao = "IPS 1ms." },
            new Produto { Id = 2, Nome = "Mouse Logitech Superlight 2", Preco = 800m, Descricao = "Mouse sem fio, 1600DPI, Óptico" }
        };

        public List<Produto> ListarProdutos() => _produtos;

        public Produto? ObterPorId(int id) => _produtos.FirstOrDefault(p => p.Id == id);
    }
}