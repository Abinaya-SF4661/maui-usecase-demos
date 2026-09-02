using TaskManagement.ViewModels;
using TaskManagement.Helpers;
using System.Globalization;

namespace TaskManagement.Pages;

public partial class KanbanBoardPage : ContentPage
{
    private KanbanViewModel _viewModel;
    public KanbanBoardPage()
    {
        InitializeComponent();
        _viewModel = new KanbanViewModel();
        BindingContext = _viewModel;

        Resources.Add("PriorityColorConverter", new KanbanPriorityColorConverter());
        Resources.Add("DaysColorConverter", new KanbanDaysColorConverter());
        Resources.Add("PercentConverter", new KanbanPercentConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTasksAsync();
    }

    public async Task ReloadPageAsync()
    {
        BindingContext = null;

        _viewModel = new KanbanViewModel();

        BindingContext = _viewModel;

        await _viewModel.LoadTasksAsync();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await ReloadPageAsync();

    }

    // Local converters for this page
    public class KanbanPriorityColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string priority)
            {
                return priority switch
                {
                    "High" => Colors.Red,
                    "Medium" => Colors.Orange,
                    "Low" => Colors.Green,
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class KanbanDaysColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int days)
            {
                return days switch
                {
                    < 0 => Colors.Red,
                    < 3 => Colors.Orange,
                    _ => Colors.Green
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class KanbanPercentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double percent)
                return percent / 100.0;
            return 0.0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
