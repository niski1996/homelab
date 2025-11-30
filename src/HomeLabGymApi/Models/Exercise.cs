using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HomeLabGymApi.Models;

public class Exercise
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [StringLength(100)]
    public string? Category { get; set; }
    
    [Required]
    [StringLength(50)]
    public string ExerciseType { get; set; } = string.Empty; // Strength/Time/Distance/Mixed/Custom
    
    public JsonDocument? AdditionalSettings { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<ExerciseTag> ExerciseTags { get; set; } = new List<ExerciseTag>();
    public virtual ICollection<ExerciseLink> Links { get; set; } = new List<ExerciseLink>();
    public virtual ICollection<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>();
    public virtual ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();
}
