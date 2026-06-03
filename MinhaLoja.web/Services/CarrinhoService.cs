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

    public async Task RemoverDoCarrinho(ProdutoReadDTO produto)
    {
        // 1. Pega a lista atual do cofre
        var carrinho = await ObterCarrinho();
        
        // 2. Procura a guitarra exata que o usuário quer apagar
        var itemARemover = carrinho.FirstOrDefault(p => p.Id == produto.Id);
        
        if (itemARemover != null)
        {
            // 3. Remove da lista e salva a lista atualizada de volta no cofre
            carrinho.Remove(itemARemover);
            await _localStorage.SetItemAsync(CartKey, carrinho);
            
            // 4. Dispara o alarme para o NavMenu atualizar o numerozinho amarelo lá em cima!
            OnChange?.Invoke(); 
        }
    }
    
    public async Task LimparCarrinho()
    {
        // Remove a chave inteira do cofre do navegador
        await _localStorage.RemoveItemAsync(CartKey);
        
        // Toca o alarme para o Menu Superior zerar o número
        OnChange?.Invoke();
    }

    public async Task<List<ProdutoReadDTO>> ObterCarrinho()
    {
        // Tenta ler do cofre. Se voltar nulo, cria uma lista vazia.
        var carrinho = await _localStorage.GetItemAsync<List<ProdutoReadDTO>>(CartKey);
        return carrinho ?? new List<ProdutoReadDTO>();
    }
}