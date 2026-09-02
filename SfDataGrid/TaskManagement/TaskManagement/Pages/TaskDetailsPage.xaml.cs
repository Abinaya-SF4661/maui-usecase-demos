using TaskManagement.ViewModels;
using TaskManagement.Helpers;
using System.Globalization;
using System.Collections.Generic;

namespace TaskManagement.Pages;

// Make StaticData accessible for XAML x:Static binding
public static partial class StaticData
{
    public static List<string> Projects => Helpers.StaticData.Projects;
    public static List<string> Statuses => Helpers.StaticData.Statuses;
    public static List<string> Priorities => Helpers.StaticData.Priorities;
    public static List<string> Users => Helpers.StaticData.Users;
    public static List<string> Teams => Helpers.StaticData.Teams;
}

public partial class TaskDetailsPage : ContentPage, IQueryAttributable
{
    private TaskDetailsViewModel _viewModel;

    public TaskDetailsPage()
    {
        InitializeComponent();
        _viewModel = new TaskDetailsViewModel();
        BindingContext = _viewModel;
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("taskId", out var value) &&
            int.TryParse(value?.ToString(), out int taskId))
        {
            _viewModel.LoadTaskCommand?.Execute(taskId);
        }
    }

}

// Local converter for this page
public class TaskDetailsPagePercentConverter : IValueConverter
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
