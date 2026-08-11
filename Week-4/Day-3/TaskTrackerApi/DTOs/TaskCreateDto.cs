using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApi.DTOs;

public class TaskCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;
}