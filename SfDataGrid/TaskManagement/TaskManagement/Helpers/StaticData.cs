namespace TaskManagement.Helpers;

public static class StaticData
{
    public static List<string> Statuses { get; } = new List<string>
    {
        "To Do",
        "In Progress",
        "Review",
        "Done"
    };

    public static List<string> Priorities { get; } = new List<string>
    {
        "High",
        "Medium",
        "Low"
    };

    public static List<string> Projects { get; } = new List<string>
    {
        "Project Phoenix",
        "Project Aurora",
        "Project Titan",
        "Web Platform"
    };

    public static List<string> Teams { get; } = new List<string>
    {
        "Platform Team",
        "Backend Team",
        "Frontend Team",
        "DevOps Team"
    };

    public static List<string> Users { get; } = new List<string>
    {
        "John Smith",
        "Sarah Johnson",
        "Mike Davis",
        "Emily Chen",
        "Alex Rodriguez"
    };
}
