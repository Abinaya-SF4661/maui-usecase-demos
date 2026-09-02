using System.Collections.ObjectModel;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly TaskService _taskService;

    private int _totalTasks;
    private int _completedTasks;
    private int _inProgressTasks;
    private int _toDoTasks;
    private int _overdueTasks;
    private int _activeProjects;

    public ICommand RefreshCommand { get; }



    public int TotalTasks
    {
        get => _totalTasks;
        set => SetProperty(ref _totalTasks, value);
    }

    public int CompletedTasks
    {
        get => _completedTasks;
        set => SetProperty(ref _completedTasks, value);
    }

    public int InProgressTasks
    {
        get => _inProgressTasks;
        set => SetProperty(ref _inProgressTasks, value);
    }

    public int ToDoTasks
    {
        get => _toDoTasks;
        set => SetProperty(ref _toDoTasks, value);
    }

    public int OverdueTasks
    {
        get => _overdueTasks;
        set => SetProperty(ref _overdueTasks, value);
    }

    public int ActiveProjects
    {
        get => _activeProjects;
        set => SetProperty(ref _activeProjects, value);
    }

    public ObservableCollection<TaskStatusData> TasksByStatus { get; } = new();
    public ObservableCollection<TaskPriorityData> TasksByPriority { get; } = new();
    public ObservableCollection<ProjectProgressData> ProjectProgress { get; } = new();
    public ObservableCollection<TeamPerformanceData> TeamPerformance { get; } = new();

    public DashboardViewModel()
    {
        _taskService = ServiceHelper.GetService<TaskService>();

        RefreshCommand = new Command(async () => await RefreshDashboard());

        LoadDashboard();

    }


    private void LoadDashboard()
    {
        TotalTasks = _taskService.GetTotalTasks();
        CompletedTasks = _taskService.GetCompletedTasks();
        InProgressTasks = _taskService.GetInProgressTasks();
        OverdueTasks = _taskService.GetOverdueTasks();

        OnPropertyChanged(nameof(TotalTasks));
        OnPropertyChanged(nameof(CompletedTasks));
        OnPropertyChanged(nameof(InProgressTasks));
        OnPropertyChanged(nameof(OverdueTasks));
    }


    private async Task RefreshDashboard()
    {
        await Task.Delay(500); // Optional loading effect

        LoadDashboard();

        await Shell.Current.DisplayAlert(
            "Dashboard",
            "Dashboard refreshed successfully",
            "OK");
    }


    public async Task LoadDashboardDataAsync()
    {
        try
        {
            IsBusy = true;

            // Load summary cards
            TotalTasks = _taskService.GetTotalTasks();
            CompletedTasks = _taskService.GetCompletedTasks();
            InProgressTasks = _taskService.GetInProgressTasks();
            ToDoTasks = _taskService.GetToDoTasks();
            OverdueTasks = _taskService.GetOverdueTasks();
            ActiveProjects = _taskService.GetActiveProjects();

            // Load chart data
            TasksByStatus.Clear();
            foreach (var item in _taskService.GetTasksByStatus())
                TasksByStatus.Add(item);

            TasksByPriority.Clear();
            foreach (var item in _taskService.GetTasksByPriority())
                TasksByPriority.Add(item);

            ProjectProgress.Clear();
            foreach (var item in _taskService.GetProjectProgress())
                ProjectProgress.Add(item);

            TeamPerformance.Clear();
            foreach (var item in _taskService.GetTeamPerformance())
                TeamPerformance.Add(item);

            IsBusy = false;
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }
}
