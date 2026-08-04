using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApi.DTOs;

public class TaskCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;


    public int UserId { get; set; }
}