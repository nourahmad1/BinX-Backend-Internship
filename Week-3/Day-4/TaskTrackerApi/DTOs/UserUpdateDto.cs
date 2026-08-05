using System.ComponentModel.DataAnnotations;

namespace TaskTrackerApi.DTOs;

public class UserUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}