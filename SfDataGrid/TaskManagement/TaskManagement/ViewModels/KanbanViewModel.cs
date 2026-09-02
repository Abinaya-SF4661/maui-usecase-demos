using System.Collections.ObjectModel;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public class KanbanViewModel : BaseViewModel
{
    private readonly TaskService _taskService;
    private string _searchText = string.Empty;

    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<string> Projects { get; } = new();
    public ObservableCollection<string> AssignedUsers { get; } = new();
    public ObservableCollection<string> Priorities { get; } = new();
    private bool _isRefreshing;


    private string? _selectedPriority;
    public string? SelectedPriority
    {
        get => _selectedPriority;
        set
        {
            if (SetProperty(ref _selectedPriority, value))
            {

                if (!_isRefreshing)
                {
                    _ = ApplyFilters();
                }

            }
        }
    }

    private string? _selectedUser;
    public string? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (SetProperty(ref _selectedUser, value))
            {

                if (!_isRefreshing)
                {
                    _ = ApplyFilters();
                }

            }
        }
    }



    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                if (_isRefreshing)
                    return;

                _ = PerformSearch();
            }
        }
    }

    private async Task ApplyFilters()
    {
        try
        {
            IsBusy = true;

            // No filters selected -> show everything
            if (string.IsNullOrEmpty(SelectedPriority) &&
                string.IsNullOrEmpty(SelectedUser))
            {
                await LoadTasksAsync();
                return;
            }

            var results = await _taskService.FilterTasksAsync(
                priority: SelectedPriority,
                assignedUser: SelectedUser);

            Tasks.Clear();

            foreach (var task in results)
            {
                Tasks.Add(task);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }



    public IAsyncRelayCommand SearchCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public ICommand FilterByPriorityCommand { get; }
    public ICommand FilterByStatusCommand { get; }

    public KanbanViewModel()
    {
        _taskService = ServiceHelper.GetService<TaskService>() ?? throw new InvalidOperationException();

        SearchCommand = new AsyncRelayCommand(PerformSearch);

        RefreshCommand = new AsyncRelayCommand(async () =>
        {
            await RefreshDataAsync();
        });

        FilterByPriorityCommand = new AsyncRelayCommand<string>(FilterByPriority);
        FilterByStatusCommand = new AsyncRelayCommand<string>(FilterByStatus);
    }


    private async Task RefreshDataAsync()
    {
        try
        {
            IsBusy = true;

            _isRefreshing = true;

            // Reset all filters
            SearchText = string.Empty;
            SelectedPriority = null;
            SelectedUser = null;

            // Reset collections
            Tasks.Clear();
            Projects.Clear();
            AssignedUsers.Clear();
            Priorities.Clear();

            // Reload everything exactly like first render
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
        finally
        {
            _isRefreshing = false;
            IsBusy = false;
        }
    }


    public async Task LoadTasksAsync()
    {
        try
        {
            IsBusy = true;

            Tasks.Clear();

            var tasks = await _taskService.GetAllTasksAsync();

            foreach (var task in tasks)
            {
                if (string.IsNullOrWhiteSpace(task.Status))
                {
                    task.Status = "To Do";
                }

                Tasks.Add(task);
            }

            LoadFilterOptions();

            // Notify UI
            OnPropertyChanged(nameof(Tasks));
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }


    private string GetValidStatus(string status)
    {
        return status switch
        {
            "To Do" => "To Do",
            "In Progress" => "In Progress",
            "Review" => "Review",
            "Done" => "Done",
            _ => "To Do"
        };
    }


    private void LoadFilterOptions()
    {
        Projects.Clear();
        foreach (var project in _taskService.GetDistinctProjects())
            Projects.Add(project);

        AssignedUsers.Clear();
        foreach (var user in _taskService.GetDistinctUsers())
            AssignedUsers.Add(user);

        Priorities.Clear();
        foreach (var priority in _taskService.GetDistinctPriorities())
            Priorities.Add(priority);
    }

    private async Task PerformSearch()
    {

        try
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadTasksAsync();
                return;
            }

            var results = await _taskService.SearchTasksAsync(SearchText);

            Tasks.Clear();

            foreach (var task in results)
            {
                Tasks.Add(task);
            }
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
        finally
        {
            IsBusy = false;
        }

    }

    private async Task FilterByPriority(string? priority)
    {
        try
        {
            IsBusy = true;
            var results = await _taskService.FilterTasksAsync(priority: priority);
            Tasks.Clear();
            foreach (var task in results)
                Tasks.Add(task);
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }

    private async Task FilterByStatus(string? status)
    {
        try
        {
            IsBusy = true;
            var results = await _taskService.FilterTasksAsync(status: status);
            Tasks.Clear();
            foreach (var task in results)
                Tasks.Add(task);
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }

    public async Task UpdateTaskStatusAsync(TaskItem task, string newStatus)
    {

        task.Status = GetValidStatus(newStatus);

        await _taskService.UpdateTaskAsync(task);

        await LoadTasksAsync();

    }
}

// AsyncRelayCommand implementation
public class AsyncRelayCommand : IAsyncRelayCommand
{
    private readonly Func<Task> _execute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;

    public AsyncRelayCommand(Func<Task> execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => !_isExecuting;

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        _isExecuting = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        try
        {
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Task ExecuteAsync(object? parameter)
    {
        Execute(parameter);
        return Task.CompletedTask;
    }
}

public class AsyncRelayCommand<T> : IAsyncRelayCommand
{
    private readonly Func<T?, Task> _execute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;

    public AsyncRelayCommand(Func<T?, Task> execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => !_isExecuting;

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        _isExecuting = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        try
        {
            await _execute((T?)parameter);
        }
        finally
        {
            _isExecuting = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Task ExecuteAsync(object? parameter)
    {
        Execute(parameter);
        return Task.CompletedTask;
    }
}

public interface IAsyncRelayCommand : ICommand
{
    Task ExecuteAsync(object? parameter);
}
