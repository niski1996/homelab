namespace HomeLabGymApi.DTOs;

public class WorkoutTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<TemplateExerciseDto> Exercises { get; set; } = new();
}

public class CreateWorkoutTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public List<CreateTemplateExerciseDto> Exercises { get; set; } = new();
}

public class UpdateWorkoutTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public List<CreateTemplateExerciseDto> Exercises { get; set; } = new();
}

public class TemplateExerciseDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public ExerciseDto Exercise { get; set; } = null!;
    public int OrderIndex { get; set; }
    public List<TemplateSetDto> Sets { get; set; } = new();
}

public class CreateTemplateExerciseDto
{
    public Guid ExerciseId { get; set; }
    public int OrderIndex { get; set; }
    public List<CreateTemplateSetDto> Sets { get; set; } = new();
}

public class TemplateSetDto
{
    public Guid Id { get; set; }
    public int SetNumber { get; set; }
    public object Metrics { get; set; } = new();
}

public class CreateTemplateSetDto
{
    public int SetNumber { get; set; }
    public object Metrics { get; set; } = new();
}
