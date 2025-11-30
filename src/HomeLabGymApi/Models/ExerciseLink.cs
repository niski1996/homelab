using System.ComponentModel.DataAnnotations;

namespace HomeLabGymApi.Models;

public class ExerciseLink
{
    public Guid Id { get; set; }
    
    public Guid ExerciseId { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Url { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    // Navigation properties
    public virtual Exercise Exercise { get; set; } = null!;
}
