namespace HomeLabGymApi.DTOs;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
    public object? AdditionalSettings { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<TagDto> Tags { get; set; } = new();
    public List<ExerciseLinkDto> Links { get; set; } = new();
}

public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
    public object? AdditionalSettings { get; set; }
    public List<string> TagNames { get; set; } = new();
    public List<CreateExerciseLinkDto> Links { get; set; } = new();
}

public class UpdateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
    public object? AdditionalSettings { get; set; }
    public List<string> TagNames { get; set; } = new();
    public List<CreateExerciseLinkDto> Links { get; set; } = new();
}
