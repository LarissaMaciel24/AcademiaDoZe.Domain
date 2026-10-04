namespace AcademiaDoZe.Presentation.AppMaui
{
    // Microsoft.Maui.Controls.Application evita conflito com o namespace da camada "Application"
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}