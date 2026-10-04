using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    // Rota das páginas que não estão no menu (cadastro/edição)
    private static void RegisterRoutes()
    {
#pragma warning disable CA1416
        Routing.RegisterRoute("logradouro", typeof(LogradouroPage));
#pragma warning restore CA1416
    }
}