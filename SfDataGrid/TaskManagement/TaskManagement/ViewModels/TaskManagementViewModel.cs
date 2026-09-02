using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public class TaskManagementViewModel : BaseViewModel
{
    private readonly TaskService _taskService;

    private string _searchText = string.Empty;
    private string? _selectedPriority;
    private string? _selectedStatus;
    private TaskItem? _selectedTask;
    private bool _isRefreshing;

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    public ObservableCollection<string> Projects { get; } = new();

    public ObservableCollection<string> AssignedUsers { get; } = new();

    public ObservableCollection<string> Teams { get; } = new();

    public ObservableCollection<string> Priorities { get; } = new();

    public ObservableCollection<string> Statuses { get; } = new();

    #region KPI Properties

    public int TotalTasks => Tasks.Count;

    public int InProgressCount =>
        Tasks.Count(x => x.Status == "In Progress");

    public int CompletedCount =>
        Tasks.Count(x => x.Status == "Done");

    public int OverdueCount =>
        Tasks.Count(x => x.IsOverdue);

    #endregion

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                SearchCommand.Execute(null);
            }
        }
    }

    public string? SelectedPriority
    {
        get => _selectedPriority;
        set
        {

            if (SetProperty(ref _selectedPriority, value))
            {
                if (!_isRefreshing)
                {
                    ApplyFiltersCommand.Execute(null);
                }
            }

        }
    }

    public string? SelectedStatus
    {
        get => _selectedStatus;
        set
        {

            if (SetProperty(ref _selectedStatus, value))
            {
                if (!_isRefreshing)
                {
                    ApplyFiltersCommand.Execute(null);
                }
            }

        }
    }

    public TaskItem? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (SetProperty(ref _selectedTask, value) &&
                value != null)
            {
                EditTaskCommand.Execute(value);
            }
        }
    }

    public IAsyncRelayCommand LoadTasksCommand { get; }

    public IAsyncRelayCommand SearchCommand { get; }

    public IAsyncRelayCommand ApplyFiltersCommand { get; }

    public IAsyncRelayCommand CreateTaskCommand { get; }

    public ICommand DeleteTaskCommand { get; }

    public ICommand EditTaskCommand { get; }

    public TaskManagementViewModel()
    {
        _taskService =
            ServiceHelper.GetService<TaskService>()
            ?? throw new InvalidOperationException();

        LoadTasksCommand =
            new AsyncRelayCommand(LoadTasksAsync);

        SearchCommand =
            new AsyncRelayCommand(PerformSearch);

        ApplyFiltersCommand =
            new AsyncRelayCommand(ApplyFilters);

        CreateTaskCommand =
            new AsyncRelayCommand(CreateNewTask);

        DeleteTaskCommand =
            new AsyncRelayCommand<TaskItem>(DeleteTask);

        EditTaskCommand =
            new AsyncRelayCommand<TaskItem>(EditTask);

        _ = LoadTasksAsync();
    }

    private void RefreshDashboardCounts()
    {
        OnPropertyChanged(nameof(TotalTasks));
        OnPropertyChanged(nameof(InProgressCount));
        OnPropertyChanged(nameof(CompletedCount));
        OnPropertyChanged(nameof(OverdueCount));
    }

    public async Task LoadTasksAsync()
    {

        try
        {
            IsBusy = true;

            _isRefreshing = true;

            // Clear filters from ComboBoxes
            SelectedPriority = null;
            SelectedStatus = null;

            _isRefreshing = false;

            var tasks =
                await _taskService.GetAllTasksAsync();

            Tasks.Clear();

            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }

            LoadFilterOptions();

            RefreshDashboardCounts();
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
        finally
        {
            IsBusy = false;
            _isRefreshing = false;
        }


    }

    private void LoadFilterOptions()
    {
        Priorities.Clear();

        foreach (var item in _taskService.GetDistinctPriorities())
        {
            Priorities.Add(item);
        }

        Statuses.Clear();

        foreach (var item in _taskService.GetDistinctStatuses())
        {
            Statuses.Add(item);
        }

        Projects.Clear();

        foreach (var item in _taskService.GetDistinctProjects())
        {
            Projects.Add(item);
        }

        AssignedUsers.Clear();

        foreach (var item in _taskService.GetDistinctUsers())
        {
            AssignedUsers.Add(item);
        }

        Teams.Clear();

        foreach (var item in _taskService.GetDistinctTeams())
        {
            Teams.Add(item);
        }
    }

    private async Task PerformSearch()
    {
        try
        {
            IsBusy = true;

            var results =
                await _taskService.SearchTasksAsync(SearchText);

            Tasks.Clear();

            foreach (var task in results)
            {
                Tasks.Add(task);
            }

            RefreshDashboardCounts();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            await HandleException(ex);
        }
    }

    private async Task ApplyFilters()
    {
        try
        {
            IsBusy = true;

            var results =
                await _taskService.FilterTasksAsync(
                    priority: SelectedPriority,
                    status: SelectedStatus);

            Tasks.Clear();

            foreach (var task in results)
            {
                Tasks.Add(task);
            }

            RefreshDashboardCounts();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            await HandleException(ex);
        }
    }

    private async Task DeleteTask(TaskItem? task)
    {
        if (task == null)
            return;

        bool confirm =
            await Shell.Current.DisplayAlert(
                "Delete Task",
                $"Are you sure you want to delete '{task.Title}'?",
                "Yes",
                "No");

        if (!confirm)
            return;

        try
        {
            await _taskService.DeleteTaskAsync(task.TaskId);

            Tasks.Remove(task);

            RefreshDashboardCounts();

            await Shell.Current.DisplayAlert(
                "Success",
                "Task deleted successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }

    private async Task EditTask(TaskItem? task)
    {
        if (task == null)
            return;

        await Shell.Current.GoToAsync(
            $"///taskdetails?taskId={task.TaskId}");
    }

    private async Task CreateNewTask()
    {
        await Shell.Current.GoToAsync(
            "///taskdetails");
    }
}