namespace HomeLabGymApi.Models;

public class TemplateExercise
{
    public Guid Id { get; set; }
    
    public Guid WorkoutTemplateId { get; set; }
    public Guid ExerciseId { get; set; }
    
    public int OrderIndex { get; set; }
    
    // Navigation properties
    public virtual WorkoutTemplate WorkoutTemplate { get; set; } = null!;
    public virtual Exercise Exercise { get; set; } = null!;
    public virtual ICollection<TemplateSet> TemplateSets { get; set; } = new List<TemplateSet>();
}
