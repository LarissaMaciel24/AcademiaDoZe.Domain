using AcademiaDoZe.Application.DTOs;

//Larissa Maciel
namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoService
{
    Task<bool> AlunoEstaDentroAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<bool> ColaboradorEstaDentroAsync(int colaboradorId, CancellationToken cancellationToken = default);
    Task<AcessoResultadoDto> RegistrarAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<AcessoResultadoDto> RegistrarColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AcessoRegistroDto>> ObterRegistrosDeHojeAsync(CancellationToken cancellationToken = default);
}