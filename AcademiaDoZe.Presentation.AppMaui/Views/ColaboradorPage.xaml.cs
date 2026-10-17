using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

// Larissa Maciel
public partial class ColaboradorPage : ContentPage
{
    public ColaboradorPage(ColaboradorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ColaboradorViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}