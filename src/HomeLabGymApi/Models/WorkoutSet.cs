using System.Text.Json;

namespace HomeLabGymApi.Models;

public class WorkoutSet
{
    public Guid Id { get; set; }
    
    public Guid SessionExerciseId { get; set; }
    
    public int SetNumber { get; set; }
    
    public bool Completed { get; set; }
    
    public JsonDocument Metrics { get; set; } = JsonDocument.Parse("{}");
    
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual SessionExercise SessionExercise { get; set; } = null!;
}
