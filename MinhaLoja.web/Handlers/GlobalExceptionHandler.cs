using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MinhaLoja.web.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    // Injetamos o Logger para podermos gravar o erro real no terminal do servidor
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // 1. Gravamos o erro técnico no console para o desenvolvedor ver
        _logger.LogError(exception, "Ocorreu um erro inesperado: {Message}", exception.Message);

        // 2. Montamos uma resposta "elegante" padrão de mercado (ProblemDetails)
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro Interno do Servidor",
            Detail = "Ops! Algo deu errado no nosso lado. Por favor, tente novamente mais tarde."
        };

        // 3. Devolvemos o JSON amigável para o cliente
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornamos 'true' para avisar o .NET que o erro já foi tratado e a vida segue
        return true; 
    }
}