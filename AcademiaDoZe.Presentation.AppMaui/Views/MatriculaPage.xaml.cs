using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

// Larissa Maciel
public partial class MatriculaPage : ContentPage
{
    public MatriculaPage(MatriculaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MatriculaViewModel viewModel)
        {
            await viewModel.InitializeAsync();
            // garante que a data fim é calculada ao carregar uma matrícula existente
            CalculateFimFromPlanoAndInicio();
        }
    }

    private void PlanoPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (BindingContext is MatriculaViewModel vm && PlanoPicker.SelectedItem is AppMatriculaPlano plano)
        {
            vm.Matricula.Plano = plano;
        }
        CalculateFimFromPlanoAndInicio();
    }

    private void InicioDatePicker_DateSelected(object? sender, DateChangedEventArgs e)
    {
        if (BindingContext is MatriculaViewModel vm && e.NewDate.HasValue)
        {
            vm.Matricula.DataInicio = DateOnly.FromDateTime(e.NewDate.Value);
        }
        CalculateFimFromPlanoAndInicio();
    }

    private void CalculateFimFromPlanoAndInicio()
    {
        if (BindingContext is not MatriculaViewModel vm) return;

        var plano = vm.Matricula.Plano;
        var inicio = vm.Matricula.DataInicio;

        var fim = plano switch
        {
            AppMatriculaPlano.Mensal => inicio.AddMonths(1),
            AppMatriculaPlano.Trimestral => inicio.AddMonths(3),
            AppMatriculaPlano.Semestral => inicio.AddMonths(6),
            AppMatriculaPlano.Anual => inicio.AddYears(1),
            _ => inicio,
        };

        // atualiza o DTO e o DatePicker (somente leitura)
        vm.Matricula.DataFim = fim;
        try
        {
            FimDatePicker.Date = fim.ToDateTime(new TimeOnly(0, 0));
        }
        catch { }
    }
}