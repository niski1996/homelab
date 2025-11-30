namespace HomeLabGymApi.Models;

public class WorkoutSession
{
    public Guid Id { get; set; }
    
    public Guid? WorkoutTemplateId { get; set; }
    
    public DateTimeOffset SessionDate { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual WorkoutTemplate? WorkoutTemplate { get; set; }
    public virtual ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();
}
