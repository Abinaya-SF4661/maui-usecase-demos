using TaskManagement.Models;

namespace TaskManagement.Services;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TaskService
{
    private static List<TaskItem> _tasks = new();
    private static int _nextId = 1;
    private static readonly string[] Projects = { "Project Phoenix", "Project Aurora", "Project Titan", "Web Platform" };
    private static readonly string[] Teams = { "Platform Team", "Backend Team", "Frontend Team", "DevOps Team" };
    private static readonly string[] Users = { "John Smith", "Sarah Johnson", "Mike Davis", "Emily Chen", "Alex Rodriguez" };
    private static readonly string[] Priorities = { "High", "Medium", "Low" };
    private static readonly string[] Statuses = { "To Do", "In Progress", "Review", "Done" };

    public TaskService()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        if (_tasks.Count > 0) return;

        var random = new Random();

        for (int i = 0; i < 25; i++)
        {
            var startDate = DateTime.Now.AddDays(random.Next(-30, 10));
            var dueDate = startDate.AddDays(random.Next(5, 30));

            _tasks.Add(new TaskItem
            {
                TaskId = _nextId++,
                Title = GenerateTaskTitle(i),
                Description = GenerateDescription(i),
                ProjectName = Projects[random.Next(Projects.Length)],
                Priority = Priorities[random.Next(Priorities.Length)],
                Status = Statuses[random.Next(Statuses.Length)],
                AssignedUser = Users[random.Next(Users.Length)],
                Team = Teams[random.Next(Teams.Length)],
                StartDate = startDate,
                DueDate = dueDate,
                EstimatedHours = random.Next(8, 80),
                ActualHours = random.Next(0, 80),
                CompletionPercentage = random.Next(0, 101),
                CreatedDate = startDate.AddDays(-random.Next(1, 10)),
                UpdatedDate = DateTime.Now.AddDays(-random.Next(0, 5))
            });
        }
    }

    private string GenerateTaskTitle(int index)
    {
        var titles = new[]
        {
            "Implement User Authentication",
            "Design Database Schema",
            "Create API Endpoints",
            "Set up CI/CD Pipeline",
            "Write Unit Tests",
            "Code Review",
            "Bug Fix - Login Issue",
            "Performance Optimization",
            "Update Documentation",
            "Security Audit",
            "Refactor Legacy Code",
            "Deploy to Production",
            "Customer Feedback Implementation",
            "System Architecture Review",
            "Mobile App Development",
            "Database Migration",
            "API Rate Limiting",
            "Cache Implementation",
            "Load Testing",
            "Create Dashboard",
            "Implement Notifications",
            "User Profile Management",
            "Two-Factor Authentication",
            "Export Data Feature",
            "Analytics Dashboard"
        };
        return titles[index % titles.Length];
    }

    private string GenerateDescription(int index)
    {
        var descriptions = new[]
        {
            "Develop OAuth authentication workflow",
            "Design and normalize database tables",
            "Create RESTful API endpoints for tasks",
            "Set up GitHub Actions for automated builds",
            "Write comprehensive unit tests",
            "Review code changes and approve PRs",
            "Fix critical login authentication bug",
            "Optimize query performance",
            "Update API documentation",
            "Perform security penetration testing",
            "Clean up and optimize codebase",
            "Deploy latest version to production",
            "Implement user-requested features",
            "Review system design and scalability",
            "Develop mobile application",
            "Migrate data from legacy system",
            "Implement API rate limiting",
            "Implement caching strategy",
            "Perform load testing on API",
            "Create admin dashboard",
            "Add email notifications",
            "Build user profile UI",
            "Enable two-factor authentication",
            "Add data export functionality",
            "Build analytics dashboard"
        };
        return descriptions[index % descriptions.Length];
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        await Task.Delay(300); // Simulate API delay
        return _tasks.OrderByDescending(t => t.UpdatedDate).ToList();
    }

    public async Task<TaskItem> GetTaskByIdAsync(int taskId)
    {
        await Task.Delay(100);
        return _tasks.FirstOrDefault(t => t.TaskId == taskId) ?? new TaskItem();
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        await Task.Delay(200);
        task.TaskId = _nextId++;
        task.CreatedDate = DateTime.Now;
        task.UpdatedDate = DateTime.Now;
        _tasks.Add(task);
        return task;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await Task.Delay(200);
        var existingTask = _tasks.FirstOrDefault(t => t.TaskId == task.TaskId);
        if (existingTask != null)
        {
            task.UpdatedDate = DateTime.Now;
            var index = _tasks.IndexOf(existingTask);
            _tasks[index] = task;
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        await Task.Delay(200);
        var task = _tasks.FirstOrDefault(t => t.TaskId == taskId);
        if (task != null)
        {
            _tasks.Remove(task);
            return true;
        }
        return false;
    }

    public async Task<List<TaskItem>> SearchTasksAsync(string searchText)
    {
        await Task.Delay(200);
        if (string.IsNullOrWhiteSpace(searchText))
            return _tasks;

        var lowerSearch = searchText.ToLower();
        return _tasks.Where(t =>
            t.Title.ToLower().Contains(lowerSearch) ||
            t.Description.ToLower().Contains(lowerSearch) ||
            t.AssignedUser.ToLower().Contains(lowerSearch) ||
            t.ProjectName.ToLower().Contains(lowerSearch)
        ).ToList();
    }

    public async Task<List<TaskItem>> FilterTasksAsync(string? priority = null, string? status = null, string? project = null, string? assignedUser = null)
    {
        await Task.Delay(200);
        var result = _tasks.AsEnumerable();

        if (!string.IsNullOrEmpty(priority))
            result = result.Where(t => t.Priority == priority);

        if (!string.IsNullOrEmpty(status))
            result = result.Where(t => t.Status == status);

        if (!string.IsNullOrEmpty(project))
            result = result.Where(t => t.ProjectName == project);

        if (!string.IsNullOrEmpty(assignedUser))
            result = result.Where(t => t.AssignedUser == assignedUser);

        return result.OrderByDescending(t => t.UpdatedDate).ToList();
    }

    public List<string> GetDistinctProjects() => _tasks.Select(t => t.ProjectName).Distinct().ToList();
    public List<string> GetDistinctUsers() => _tasks.Select(t => t.AssignedUser).Distinct().ToList();
    public List<string> GetDistinctTeams() => _tasks.Select(t => t.Team).Distinct().ToList();
    public List<string> GetDistinctPriorities() => Priorities.ToList();
    public List<string> GetDistinctStatuses() => Statuses.ToList();

    public int GetTotalTasks() => _tasks.Count;
    public int GetCompletedTasks() => _tasks.Count(t => t.Status == "Done");
    public int GetInProgressTasks() => _tasks.Count(t => t.Status == "In Progress");
    public int GetToDoTasks() => _tasks.Count(t => t.Status == "To Do");
    public int GetOverdueTasks() => _tasks.Count(t => t.IsOverdue);
    public int GetActiveProjects() => GetDistinctProjects().Count;

    public List<TaskStatusData> GetTasksByStatus()
    {
        return new List<TaskStatusData>
        {
            new TaskStatusData { Status = "To Do", Count = GetToDoTasks() },
            new TaskStatusData { Status = "In Progress", Count = GetInProgressTasks() },
            new TaskStatusData { Status = "Review", Count = _tasks.Count(t => t.Status == "Review") },
            new TaskStatusData { Status = "Done", Count = GetCompletedTasks() }
        };
    }

    public List<TaskPriorityData> GetTasksByPriority()
    {
        return Priorities.Select(p => new TaskPriorityData
        {
            Priority = p,
            Count = _tasks.Count(t => t.Priority == p)
        }).ToList();
    }

    public List<ProjectProgressData> GetProjectProgress()
    {
        var projects = GetDistinctProjects();
        return projects.Select(p =>
        {
            var tasksInProject = _tasks.Where(t => t.ProjectName == p).ToList();
            var avgCompletion = tasksInProject.Count > 0 ? tasksInProject.Average(t => t.CompletionPercentage) : 0;
            return new ProjectProgressData { Project = p, Percentage = avgCompletion };
        }).ToList();
    }

    public List<TeamPerformanceData> GetTeamPerformance()
    {
        return GetDistinctUsers()
            .Select(u => new TeamPerformanceData
            {
                User = u,
                CompletedTasks = _tasks.Count(t => t.AssignedUser == u && t.Status == "Done")
            })
            .OrderByDescending(x => x.CompletedTasks)
            .ToList();
    }
}
