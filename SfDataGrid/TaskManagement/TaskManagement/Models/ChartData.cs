namespace TaskManagement.Models;

/// <summary>
/// Represents task status data for chart binding
/// </summary>
public class TaskStatusData
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Represents task priority data for chart binding
/// </summary>
public class TaskPriorityData
{
    public string Priority { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Represents project progress data for chart binding
/// </summary>
public class ProjectProgressData
{
    public string Project { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

/// <summary>
/// Represents team performance data for chart binding
/// </summary>
public class TeamPerformanceData
{
    public string User { get; set; } = string.Empty;
    public int CompletedTasks { get; set; }
}
