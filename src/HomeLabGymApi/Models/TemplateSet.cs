using System.Text.Json;

namespace HomeLabGymApi.Models;

public class TemplateSet
{
    public Guid Id { get; set; }
    
    public Guid TemplateExerciseId { get; set; }
    
    public int SetNumber { get; set; }
    
    public JsonDocument Metrics { get; set; } = JsonDocument.Parse("{}");
    
    // Navigation properties
    public virtual TemplateExercise TemplateExercise { get; set; } = null!;
}
