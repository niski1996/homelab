namespace HomeLabGymApi.Models;

public class SessionExercise
{
    public Guid Id { get; set; }
    
    public Guid WorkoutSessionId { get; set; }
    public Guid ExerciseId { get; set; }
    
    public int OrderIndex { get; set; }
    
    // Navigation properties
    public virtual WorkoutSession WorkoutSession { get; set; } = null!;
    public virtual Exercise Exercise { get; set; } = null!;
    public virtual ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
}
