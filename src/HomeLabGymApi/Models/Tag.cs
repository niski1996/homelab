using System.ComponentModel.DataAnnotations;

namespace HomeLabGymApi.Models;

public class Tag
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<ExerciseTag> ExerciseTags { get; set; } = new List<ExerciseTag>();
}
