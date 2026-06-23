using System.Net.Http.Json;
using Blazored.LocalStorage;
using MinhaLoja.Core.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace MinhaLoja.web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        // Injeção do HttpClient (para fazer as requisições) e o LocalStorage (para salvar o token)
        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        // Método que vai ser chamado pela tela de Login
        public async Task<bool> LoginAsync(LoginDTO loginDto)
        {
            // Bate na API de login
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            // Se a API retornar 200 OK
            if (response.IsSuccessStatusCode)
            {
                // Lê o token que veio na resposta
                var authContent = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
                
                if (authContent != null && !string.IsNullOrEmpty(authContent.Token))
                {
                    // Salva o token no cofre do navegador com o nome "authToken"
                    await _localStorage.SetItemAsync("authToken", authContent.Token);
                    ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(authContent.Token);
                    return true; // Login efetivado
                }
            }

            return false; // Login falhou (senha errada, etc.)
        }

        // Método para o botão de "Sair"
        public async Task LogoutAsync()
        {
            // Remove o token do cofre
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
    }
}