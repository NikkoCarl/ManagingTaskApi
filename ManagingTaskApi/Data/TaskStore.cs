using TaskManagementApi.Models;

namespace TaskManagementApi.Data;

public static class TaskStore
{
    public static List<TaskItem> Tasks { get; } = new()
    {
        new TaskItem { Id = 1, Title = "First Task", IsCompleted = false },
        new TaskItem { Id = 2, Title = "Second Task", IsCompleted = true }
    };
}