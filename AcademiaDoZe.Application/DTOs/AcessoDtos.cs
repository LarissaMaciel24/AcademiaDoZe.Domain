namespace AcademiaDoZe.Application.DTOs;

//Larissa Maciel
public class AcessoResultadoDto
{
    public bool Sucesso { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;

    // Situação da pessoa DEPOIS da operação (true = dentro da academia)
    public bool Dentro { get; set; }
}

public class AcessoRegistroDto
{
    public required string Nome { get; set; }
    public required string Mensagem { get; set; }
    public DateTime DataHora { get; set; }
    public string Hora => DataHora.ToString("HH:mm:ss");
}