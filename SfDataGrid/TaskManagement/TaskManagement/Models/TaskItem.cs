using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManagement.Models;

public class TaskItem : INotifyPropertyChanged
{
    public int TaskId { get; set; }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    private string _projectName = string.Empty;
    public string ProjectName
    {
        get => _projectName;
        set
        {
            _projectName = value;
            OnPropertyChanged();
        }
    }

    private string _priority = "Medium";
    public string Priority
    {
        get => _priority;
        set
        {
            _priority = value;
            OnPropertyChanged();
        }
    }

    private string _status = "To Do";
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsOverdue));
        }
    }

    public string Category => Status;

    private string _assignedUser = string.Empty;
    public string AssignedUser
    {
        get => _assignedUser;
        set
        {
            _assignedUser = value;
            OnPropertyChanged();
        }
    }

    private string _team = string.Empty;
    public string Team
    {
        get => _team;
        set
        {
            _team = value;
            OnPropertyChanged();
        }
    }

    private DateTime _startDate = DateTime.Today;

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            _startDate = value;

            OnPropertyChanged();
            NotifyScheduleChanges();
        }
    }

    private DateTime _dueDate = DateTime.Today.AddDays(7);

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            _dueDate = value;

            OnPropertyChanged();
            NotifyScheduleChanges();
        }
    }

    private int _estimatedHours;

    public int EstimatedHours
    {
        get => _estimatedHours;
        set
        {
            if (_estimatedHours == value)
                return;

            _estimatedHours = value;

            RecalculateTaskHealth();

            OnPropertyChanged();
            OnPropertyChanged(nameof(EfficiencyPercentage));
        }
    }

    private int _actualHours;

    public int ActualHours
    {
        get => _actualHours;
        set
        {
            _actualHours = value;

            RecalculateCompletion();
            RecalculateTaskHealth();

            OnPropertyChanged();
            OnPropertyChanged(nameof(EfficiencyPercentage));
        }
    }

    private void RecalculateTaskHealth()
    {
        if (EstimatedHours <= 0)
        {
            CompletionPercentage = 0;
            return;
        }

        CompletionPercentage =
            Math.Min(
                100,
                Math.Round(
                    ((double)ActualHours /
                     EstimatedHours) * 100,
                    0));
    }
    private void RecalculateCompletion()
    {
        if (EstimatedHours <= 0)
        {
            CompletionPercentage = 0;
            return;
        }

        CompletionPercentage =
            Math.Min(
                100,
                Math.Round(
                    ((double)ActualHours /
                     EstimatedHours) * 100,
                    0));
    }

    private double _completionPercentage;

    public double CompletionPercentage
    {
        get => _completionPercentage;
        set
        {
            if (_completionPercentage == value)
                return;

            _completionPercentage = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(TaskHealth));
        }
    }


    private DateTime _createdDate = DateTime.Now;

    public DateTime CreatedDate
    {
        get => _createdDate;
        set
        {
            _createdDate = value;
            OnPropertyChanged();
        }
    }

    private DateTime _updatedDate = DateTime.Now;

    public DateTime UpdatedDate
    {
        get => _updatedDate;
        set
        {
            _updatedDate = value;
            OnPropertyChanged();
        }
    }

    // =====================================================
    // SCHEDULE DETAILS
    // =====================================================

    public int DurationDays
    {
        get
        {
            return Math.Max(
                1,
                (DueDate.Date - StartDate.Date).Days + 1);
        }
    }

    public int RemainingDays
    {
        get
        {
            return (DueDate.Date - DateTime.Today).Days;
        }
    }

    public string DueStatus
    {
        get
        {
            if (RemainingDays < 0)
                return "🔴 Overdue";

            if (RemainingDays == 0)
                return "🟠 Due Today";

            if (RemainingDays <= 3)
                return $"🟡 {RemainingDays} day(s) remaining";

            return $"🟢 {RemainingDays} days remaining";
        }
    }

    public string ScheduleStatus
    {
        get
        {
            if (DurationDays <= 3)
                return "Urgent";

            if (DurationDays <= 7)
                return "Short Term";

            if (DurationDays <= 14)
                return "Normal";

            return "Long Running";
        }
    }

    public double TimelineProgress
    {
        get
        {
            if (DueDate <= StartDate)
                return 0;

            double totalDays =
                (DueDate - StartDate).TotalDays;

            double elapsed =
                (DateTime.Today - StartDate).TotalDays;

            return Math.Clamp(
                elapsed / totalDays,
                0,
                1);
        }
    }

    // =====================================================
    // TASK HEALTH
    // =====================================================

    public string TaskHealth
    {
        get
        {
            if (CompletionPercentage >= 90)
                return "Excellent";

            if (CompletionPercentage >= 60)
                return "Good";

            if (CompletionPercentage >= 30)
                return "Needs Attention";

            return "Critical";
        }
    }

    public double EfficiencyPercentage
    {
        get
        {
            if (EstimatedHours <= 0)
                return 0;

            return Math.Round(
                ((double)ActualHours /
                 EstimatedHours) * 100,
                 1);
        }
    }

    public bool IsOverdue =>
        DueDate.Date < DateTime.Today &&
        Status != "Done";

    // =====================================================
    // NOTIFICATION HELPERS
    // =====================================================

    private void NotifyScheduleChanges()
    {
        OnPropertyChanged(nameof(DurationDays));
        OnPropertyChanged(nameof(RemainingDays));
        OnPropertyChanged(nameof(DueStatus));
        OnPropertyChanged(nameof(ScheduleStatus));
        OnPropertyChanged(nameof(TimelineProgress));
        OnPropertyChanged(nameof(IsOverdue));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}