using Microsoft.AspNetCore.Identity;

namespace TaskTrackerApi.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; set; }
        = new List<TaskItem>();
}