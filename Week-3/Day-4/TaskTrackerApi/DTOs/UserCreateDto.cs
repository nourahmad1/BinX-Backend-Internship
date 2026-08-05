using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApi.DTOs;

public class UserCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
}