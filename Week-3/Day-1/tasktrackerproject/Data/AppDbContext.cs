using TaskTrackerApi.Models;

namespace TaskTrackerApi.Data;

public static class AppData
{
    public static List<User> Users = new()
    {
        new User
        {
            Id = 1,
            Name = "Ahmad",
            Email = "ahmad@test.com"
        }
    };


    public static List<TaskItem> Tasks = new()
    {
        new TaskItem
        {
            Id = 1,
            Title = "Learn REST API",
            Description = "Understand HTTP methods",
            IsCompleted = false,
            UserId = 1
        }
    };
}