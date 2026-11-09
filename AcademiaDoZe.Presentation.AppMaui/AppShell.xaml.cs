using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }
    private static void RegisterRoutes()
    {
#pragma warning disable CA1416
        Routing.RegisterRoute("logradouro", typeof(LogradouroPage));
        Routing.RegisterRoute("colaborador", typeof(ColaboradorPage));
        Routing.RegisterRoute("aluno", typeof(AlunoPage));
        Routing.RegisterRoute("aluno", typeof(AlunoPage));
        Routing.RegisterRoute("matricula", typeof(MatriculaPage));
#pragma warning restore CA1416
    }
}