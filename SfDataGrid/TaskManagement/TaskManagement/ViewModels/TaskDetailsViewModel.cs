using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.ViewModels;

public class TaskDetailsViewModel : BaseViewModel
{
    private readonly TaskService _taskService;

    private TaskItem _task = new();
    private bool _isEditMode;
    private bool _isScheduleEditing;

    private string _selectedStatus = "To Do";
    private string _selectedPriority = "Medium";

    #region Commands

    public ICommand LoadTaskCommand { get; }

    public IAsyncRelayCommand SaveTaskCommand { get; }

    public IAsyncRelayCommand DeleteTaskCommand { get; }

    public IAsyncRelayCommand GoBackCommand { get; }

    public IAsyncRelayCommand ApplyScheduleCommand { get; }

    public IAsyncRelayCommand EnableScheduleEditCommand { get; }

    public IAsyncRelayCommand CancelScheduleEditCommand { get; }

    #endregion

    #region Task

    public TaskItem Task
    {
        get => _task;
        set
        {
            if (_task != null)
            {
                _task.PropertyChanged -= Task_PropertyChanged;
            }

            if (SetProperty(ref _task, value))
            {
                _task.PropertyChanged += Task_PropertyChanged;

                RefreshScheduleOverview();
            }
        }
    }


    private void Task_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(TaskItem.StartDate):
            case nameof(TaskItem.DueDate):
            case nameof(TaskItem.EstimatedHours):
            case nameof(TaskItem.ActualHours):
            case nameof(TaskItem.CompletionPercentage):

                RefreshScheduleOverview();
                break;
        }
    }


    #endregion

    #region Properties

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    public string SelectedPriority
    {
        get => _selectedPriority;
        set => SetProperty(ref _selectedPriority, value);
    }

    public bool IsScheduleEditing
    {
        get => _isScheduleEditing;
        set
        {
            if (SetProperty(ref _isScheduleEditing, value))
            {
                OnPropertyChanged(nameof(ScheduleButtonText));
            }
        }
    }

    #endregion

    #region Schedule Overview

    public string ScheduleButtonText =>
        IsScheduleEditing
            ? "Apply Schedule"
            : "Edit Schedule";

    public string StartDateDisplay =>
        Task.StartDate.ToString("dd MMM yyyy");

    public string DueDateDisplay =>
        Task.DueDate.ToString("dd MMM yyyy");

    public string DurationText =>
        $"{Task.DurationDays} Days";

    public string ScheduleCategory =>
        Task.ScheduleStatus;

    public string DueStatus =>
        Task.DueStatus;

    public double TimelinePercent =>
        Task.TimelineProgress;

    public string TimelineProgressText =>
        $"{Math.Round(Task.TimelineProgress * 100)}% Complete";

    public Color DueStatusColor
    {
        get
        {
            if (Task.RemainingDays < 0)
                return Colors.Red;

            if (Task.RemainingDays <= 3)
                return Colors.Orange;

            return Colors.Green;
        }
    }

    #endregion

    public TaskDetailsViewModel()
    {
        _taskService =
            ServiceHelper.GetService<TaskService>()
            ?? throw new InvalidOperationException();

        LoadTaskCommand =
            new AsyncRelayCommand<int>(LoadTask);

        SaveTaskCommand =
            new AsyncRelayCommand(SaveTask);

        DeleteTaskCommand =
            new AsyncRelayCommand(DeleteTask);

        GoBackCommand =
            new AsyncRelayCommand(GoBack);

        ApplyScheduleCommand =
            new AsyncRelayCommand(ApplySchedule);

        EnableScheduleEditCommand =
            new AsyncRelayCommand(() =>
            {
                IsScheduleEditing = true;
                RefreshScheduleOverview();
                return System.Threading.Tasks.Task.CompletedTask;
            });

        CancelScheduleEditCommand =
            new AsyncRelayCommand(() =>
            {
                IsScheduleEditing = false;
                RefreshScheduleOverview();
                return System.Threading.Tasks.Task.CompletedTask;
            });

        Task = new TaskItem
        {
            Status = "To Do",
            Priority = "Medium",
            StartDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        SelectedStatus = Task.Status;
        SelectedPriority = Task.Priority;

        IsEditMode = false;
    }

    private void RefreshScheduleOverview()
    {
        OnPropertyChanged(nameof(StartDateDisplay));
        OnPropertyChanged(nameof(DueDateDisplay));
        OnPropertyChanged(nameof(DurationText));
        OnPropertyChanged(nameof(ScheduleCategory));
        OnPropertyChanged(nameof(DueStatus));
        OnPropertyChanged(nameof(DueStatusColor));
        OnPropertyChanged(nameof(TimelinePercent));
        OnPropertyChanged(nameof(TimelineProgressText));
        OnPropertyChanged(nameof(ScheduleButtonText));
    }

    private async Task LoadTask(int taskId)
    {
        try
        {
            IsBusy = true;

            var task =
                await _taskService.GetTaskByIdAsync(taskId);

            if (task != null && task.TaskId != 0)
            {
                Task = task;

                SelectedStatus = task.Status;
                SelectedPriority = task.Priority;

                IsEditMode = true;

                CalculateTaskHealth();
                RefreshScheduleOverview();
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

    private async Task ApplySchedule()
    {
        try
        {
            if (!IsScheduleEditing)
            {
                IsScheduleEditing = true;
                return;
            }

            if (Task.DueDate < Task.StartDate)
            {
                await Shell.Current.DisplayAlert(
                    "Validation",
                    "Due Date cannot be earlier than Start Date.",
                    "OK");

                return;
            }

            Task.UpdatedDate = DateTime.Now;

            IsScheduleEditing = false;

            RefreshScheduleOverview();

            await Shell.Current.DisplayAlert(
                "Success",
                $"Schedule Updated\n\n" +
                $"Duration : {Task.DurationDays} Days\n" +
                $"Type : {Task.ScheduleStatus}\n" +
                $"Status : {Task.DueStatus}",
                "OK");
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }

    private void CalculateTaskHealth()
    {
        if (Task.EstimatedHours <= 0)
        {
            Task.CompletionPercentage = 0;
            return;
        }

        Task.CompletionPercentage =
            Math.Min(
                100,
                Math.Round(
                    ((double)Task.ActualHours /
                     Task.EstimatedHours) * 100,
                    0));
    }

    private async Task SaveTask()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Task.Title))
            {
                await Shell.Current.DisplayAlert(
                    "Validation",
                    "Task title is required.",
                    "OK");

                return;
            }

            if (Task.DueDate < Task.StartDate)
            {
                await Shell.Current.DisplayAlert(
                    "Validation",
                    "Due Date cannot be earlier than Start Date.",
                    "OK");

                return;
            }

            Task.Status = SelectedStatus;
            Task.Priority = SelectedPriority;

            CalculateTaskHealth();

            Task.UpdatedDate = DateTime.Now;

            if (Task.TaskId == 0)
            {
                Task.CreatedDate = DateTime.Now;

                await _taskService.CreateTaskAsync(Task);
            }
            else
            {
                await _taskService.UpdateTaskAsync(Task);
            }

            await Shell.Current.DisplayAlert(
                "Success",
                "Task saved successfully.",
                "OK");

            await Shell.Current.GoToAsync("//Tasks");
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }

    private async Task DeleteTask()
    {
        if (!IsEditMode || Task.TaskId == 0)
            return;

        bool confirm =
            await Shell.Current.DisplayAlert(
                "Delete Task",
                $"Are you sure you want to delete '{Task.Title}'?",
                "Yes",
                "No");

        if (!confirm)
            return;

        try
        {
            await _taskService.DeleteTaskAsync(Task.TaskId);

            await Shell.Current.DisplayAlert(
                "Success",
                "Task deleted successfully.",
                "OK");

            await Shell.Current.GoToAsync("//Tasks");
        }
        catch (Exception ex)
        {
            await HandleException(ex);
        }
    }

    private async Task GoBack()
    {
        bool leave =
            await Shell.Current.DisplayAlert(
                "Discard Changes",
                "Leave without saving?",
                "Yes",
                "No");

        if (leave)
        {
            await Shell.Current.GoToAsync("//Tasks");
        }
    }
}