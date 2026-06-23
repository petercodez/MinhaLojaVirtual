using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MinhaLoja.web.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    // Injetação do Logger para gravar o erro real no terminal do servidor
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // 1. Gravação do erro técnico no console para o desenvolvedor ver
        _logger.LogError(exception, "Ocorreu um erro inesperado: {Message}", exception.Message);

        // 2. Resposta (ProblemDetails)
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro Interno do Servidor",
            Detail = "Ops! Algo deu errado no nosso lado. Por favor, tente novamente mais tarde."
        };

        // 3. JSON devolvido para o cliente
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornam 'true' para avisar o .NET que o erro já foi tratado
        return true; 
    }
}