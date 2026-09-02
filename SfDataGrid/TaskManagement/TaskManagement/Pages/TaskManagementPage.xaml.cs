using System.Globalization;
using TaskManagement.ViewModels;

namespace TaskManagement.Pages;

public partial class TaskManagementPage : ContentPage
{
    private TaskManagementViewModel _viewModel;

    public TaskManagementPage()
    {
        InitializeComponent();
        _viewModel = new TaskManagementViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTasksAsync();
    }

}

public class PriorityColorConverter : IValueConverter
{
    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "High" => Colors.Red,
            "Medium" => Colors.Orange,
            "Low" => Colors.Green,
            _ => Colors.Black
        };
    }

    public object ConvertBack(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {
        return null;
    }

}

public class ProgressColorConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value == null)
            return Colors.Black;

        int progress = System.Convert.ToInt32(value);

        return progress switch
        {
            >= 80 => Color.FromArgb("#10B981"), // Green
            >= 60 => Color.FromArgb("#0EA5E9"), // Sky Blue
            >= 40 => Color.FromArgb("#F59E0B"), // Orange
            _ => Color.FromArgb("#EF4444")      // Red
        };
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return null;
    }
}

public class StatusColorConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return value?.ToString()?.Trim() switch
        {
            "Done" => Color.FromArgb("#059669"),          // Green
            "Completed" => Color.FromArgb("#059669"),

            "In Progress" => Color.FromArgb("#2563EB"),   // Blue

            "Review" => Color.FromArgb("#D97706"),        // Orange

            "To Do" => Color.FromArgb("#64748B"),         // Gray
            "Pending" => Color.FromArgb("#64748B"),

            "Overdue" => Color.FromArgb("#DC2626"),       // Red

            "Cancelled" => Color.FromArgb("#7C3AED"),     // Purple

            _ => Color.FromArgb("#374151")
        };
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        return null;
    }
}


