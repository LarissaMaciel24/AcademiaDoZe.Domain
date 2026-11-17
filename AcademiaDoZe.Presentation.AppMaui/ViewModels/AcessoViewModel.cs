using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

// Larissa Maciel
public class PessoaAcessoItem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public ArquivoDto? Foto { get; set; }
    public bool EhAluno { get; set; }

    // O Picker mostra o texto do ToString()
    public override string ToString() => Nome;
}

public partial class AcessoViewModel : BaseViewModel
{
    private readonly IAcessoService _acessoService;
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;

    private List<PessoaAcessoItem> _todasPessoas = [];

    public ObservableCollection<PessoaAcessoItem> Pessoas { get; } = [];
    public ObservableCollection<AcessoRegistroDto> Registros { get; } = [];

    private bool _modoAluno = true;
    public string ModoTexto => _modoAluno ? "Modo atual selecionado: Aluno" : "Modo atual selecionado: Colaborador";
    public string TextoSelecione => _modoAluno ? "Selecione o Aluno:" : "Selecione o Colaborador:";

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value ?? string.Empty))
                AplicarFiltro();
        }
    }

    private PessoaAcessoItem? _pessoaSelecionada;
    public PessoaAcessoItem? PessoaSelecionada
    {
        get => _pessoaSelecionada;
        set
        {
            if (SetProperty(ref _pessoaSelecionada, value))
            {
                OnPropertyChanged(nameof(TemPessoa));
                LimparResultado();
                _ = AtualizarSituacaoAsync();
            }
        }
    }

    public bool TemPessoa => PessoaSelecionada != null;

    private bool _dentro;
    public bool Dentro
    {
        get => _dentro;
        set
        {
            if (SetProperty(ref _dentro, value))
            {
                OnPropertyChanged(nameof(TextoSituacao));
                OnPropertyChanged(nameof(TextoBotao));
            }
        }
    }

    public string TextoSituacao => Dentro ? "DENTRO DA ACADEMIA" : "FORA DA ACADEMIA";
    public string TextoBotao => Dentro ? "Registrar Saída" : "Registrar Entrada";

    // Mensagem de resultado (faixa colorida)
    private bool _resultadoVisivel;
    public bool ResultadoVisivel { get => _resultadoVisivel; set => SetProperty(ref _resultadoVisivel, value); }

    private string _resultadoTitulo = string.Empty;
    public string ResultadoTitulo { get => _resultadoTitulo; set => SetProperty(ref _resultadoTitulo, value); }

    private string _resultadoMensagem = string.Empty;
    public string ResultadoMensagem { get => _resultadoMensagem; set => SetProperty(ref _resultadoMensagem, value); }

    private Color _resultadoCor = Colors.Transparent;
    public Color ResultadoCor { get => _resultadoCor; set => SetProperty(ref _resultadoCor, value); }

    public bool SemRegistros => Registros.Count == 0;

    public AcessoViewModel(
        IAcessoService acessoService,
        IAlunoService alunoService,
        IColaboradorService colaboradorService)
    {
        _acessoService = acessoService;
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        Title = "Controle de Acesso";
    }

    [RelayCommand]
    private async Task InicializarAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            await CarregarPessoasAsync();
            await CarregarRegistrosAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelecionarAlunoAsync() => await TrocarModoAsync(true);

    [RelayCommand]
    private async Task SelecionarColaboradorAsync() => await TrocarModoAsync(false);

    private async Task TrocarModoAsync(bool alunos)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _modoAluno = alunos;
            OnPropertyChanged(nameof(ModoTexto));
            OnPropertyChanged(nameof(TextoSelecione));

            _todasPessoas = [];
            SearchText = string.Empty;
            PessoaSelecionada = null;
            LimparResultado();

            await CarregarPessoasAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (IsBusy || PessoaSelecionada == null)
            return;

        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var resultado = PessoaSelecionada.EhAluno
                ? await _acessoService.RegistrarAlunoAsync(PessoaSelecionada.Id, cts.Token)
                : await _acessoService.RegistrarColaboradorAsync(PessoaSelecionada.Id, cts.Token);

            Dentro = resultado.Dentro;
            MostrarResultado(resultado);
            await CarregarRegistrosAsync();
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync("Tempo Esgotado", "O registro do acesso expirou. Verifique a conexão com o banco.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao registrar acesso: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AtualizarRegistrosAsync() => await CarregarRegistrosAsync();

    private async Task CarregarPessoasAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (_modoAluno)
            {
                var alunos = await _alunoService.ObterTodosAsync(cts.Token);

                _todasPessoas = [.. alunos.Select(a => new PessoaAcessoItem
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Cpf = a.Cpf,
                    Foto = a.Foto,
                    EhAluno = true
                })];
            }
            else
            {
                var colaboradores = await _colaboradorService.ObterTodosAsync(cts.Token);

                _todasPessoas = [.. colaboradores.Select(c => new PessoaAcessoItem
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cpf = c.Cpf,
                    Foto = c.Foto,
                    EhAluno = false
                })];
            }

            AplicarFiltro();
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync("Tempo Esgotado", "O carregamento das pessoas expirou. Verifique a conexão.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao carregar pessoas: {ex.Message}", "OK");
        }
    }

    private async Task CarregarRegistrosAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var registros = await _acessoService.ObterRegistrosDeHojeAsync(cts.Token);

            Registros.Clear();
            foreach (var registro in registros)
                Registros.Add(registro);

            OnPropertyChanged(nameof(SemRegistros));
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync("Tempo Esgotado", "O carregamento dos acessos expirou. Verifique a conexão.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao carregar acessos: {ex.Message}", "OK");
        }
    }

    // Filtra por nome ou CPF. Se tiver texto digitado, já seleciona o primeiro resultado.
    private void AplicarFiltro()
    {
        var termo = SearchText.Trim();
        var digitos = new string([.. termo.Where(char.IsDigit)]);

        IEnumerable<PessoaAcessoItem> filtradas = _todasPessoas;

        if (termo.Length > 0)
        {
            filtradas = _todasPessoas.Where(p =>
                p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                (digitos.Length > 0 && p.Cpf.Contains(digitos)));
        }

        var lista = filtradas.ToList();

        Pessoas.Clear();
        foreach (var pessoa in lista)
            Pessoas.Add(pessoa);

        PessoaSelecionada = termo.Length > 0 ? lista.FirstOrDefault() : null;
    }

    private async Task AtualizarSituacaoAsync()
    {
        if (PessoaSelecionada == null)
        {
            Dentro = false;
            return;
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            Dentro = PessoaSelecionada.EhAluno
                ? await _acessoService.AlunoEstaDentroAsync(PessoaSelecionada.Id, cts.Token)
                : await _acessoService.ColaboradorEstaDentroAsync(PessoaSelecionada.Id, cts.Token);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao consultar a situação da pessoa: {ex.Message}", "OK");
        }
    }

    private void MostrarResultado(AcessoResultadoDto resultado)
    {
        ResultadoTitulo = resultado.Titulo;
        ResultadoMensagem = resultado.Mensagem;
        ResultadoCor = resultado.Sucesso ? Color.FromArgb("#0B6B3A") : Color.FromArgb("#9B1C31");
        ResultadoVisivel = true;
    }

    private void LimparResultado()
    {
        ResultadoVisivel = false;
        ResultadoTitulo = string.Empty;
        ResultadoMensagem = string.Empty;
    }
}