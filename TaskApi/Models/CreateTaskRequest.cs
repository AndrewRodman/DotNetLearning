using System.ComponentModel.DataAnnotations;

namespace TaskApi.Models;

public class CreateTaskRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}