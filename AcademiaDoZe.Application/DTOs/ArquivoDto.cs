namespace AcademiaDoZe.Application.DTOs;

//Larissa Maciel
public class ArquivoDto
{
    public required string Nome { get; set; }
    public required string Extensao { get; set; }
    public required byte[] Conteudo { get; set; }
}