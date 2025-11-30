namespace HomeLabGymApi.Models;

public class ExerciseTag
{
    public Guid ExerciseId { get; set; }
    public Guid TagId { get; set; }
    
    // Navigation properties
    public virtual Exercise Exercise { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
