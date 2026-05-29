using Blazored.LocalStorage;
using MinhaLoja.Core.DTOs;

namespace MinhaLoja.web.Services;

public class CarrinhoService
{
    private readonly ILocalStorageService _localStorage;
    private const string CartKey = "carrinho_compras";

    // Esse evento é o "alarme" que vai fazer o numerozinho amarelo no menu superior atualizar sozinho
    public event Action? OnChange;

    public CarrinhoService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task AdicionarAoCarrinho(ProdutoReadDTO produto)
    {
        var carrinho = await ObterCarrinho();
        
        // Verifica se a guitarra já está no carrinho. Se não estiver, adiciona.
        if (!carrinho.Any(p => p.Id == produto.Id))
        {
            carrinho.Add(produto);
            
            // Salva de volta no cofre do navegador
            await _localStorage.SetItemAsync(CartKey, carrinho);
            
            // Dispara o alarme para o Blazor atualizar as telas
            OnChange?.Invoke(); 
        }
    }

    public async Task<List<ProdutoReadDTO>> ObterCarrinho()
    {
        // Tenta ler do cofre. Se voltar nulo, cria uma lista vazia.
        var carrinho = await _localStorage.GetItemAsync<List<ProdutoReadDTO>>(CartKey);
        return carrinho ?? new List<ProdutoReadDTO>();
    }
}