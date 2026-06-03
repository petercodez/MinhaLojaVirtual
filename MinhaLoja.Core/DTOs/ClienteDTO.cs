// pacote de dados oficial que vai viajar pela rede entre a tela e o banco de dados

namespace MinhaLoja.Core.DTOs;

public class ClienteDTO
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string EnderecoCompleto { get; set; } = string.Empty;
}