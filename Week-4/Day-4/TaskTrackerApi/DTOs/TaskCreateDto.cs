namespace TaskTrackerApi.DTOs;

public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
}