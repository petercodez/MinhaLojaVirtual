using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace MinhaLoja.web.Services
{
    public class JwtDelegatingHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public JwtDelegatingHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Tenta pegar o token do cofre do navegador
            var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

            // Se o token existir, anexa ele no cabeçalho (Header) da requisição HTTP
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Deixa a requisição seguir a sua viagem normalmente para a API
            return await base.SendAsync(request, cancellationToken);
        }
    }
}