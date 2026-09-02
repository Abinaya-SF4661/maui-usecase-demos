using TaskManagement.ViewModels;
using TaskManagement.Helpers;

namespace TaskManagement.Pages;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel _viewModel;

    public DashboardPage()
    {
        InitializeComponent();
        _viewModel = new DashboardViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDashboardDataAsync();
    }
}
