namespace HomeLab.Models;

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

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class UpdateTagDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class ExerciseLinkDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateExerciseLinkDto
{
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
}

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

public class TemplateExerciseDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public int OrderIndex { get; set; }
    public ExerciseDto Exercise { get; set; } = null!;
    public List<TemplateSetDto> Sets { get; set; } = new();
}

public class TemplateSetDto
{
    public Guid Id { get; set; }
    public int SetNumber { get; set; }
    public object Metrics { get; set; } = new();
}

public class CreateWorkoutTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public List<CreateTemplateExerciseDto> Exercises { get; set; } = new();
}

public class CreateTemplateExerciseDto
{
    public Guid ExerciseId { get; set; }
    public int OrderIndex { get; set; }
    public List<CreateTemplateSetDto> Sets { get; set; } = new();
}

public class CreateTemplateSetDto
{
    public int SetNumber { get; set; }
    public object Metrics { get; set; } = new();
}

public class WorkoutSessionDto
{
    public Guid Id { get; set; }
    public Guid? WorkoutTemplateId { get; set; }
    public WorkoutTemplateDto? WorkoutTemplate { get; set; }
    public DateTimeOffset SessionDate { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<SessionExerciseDto> Exercises { get; set; } = new();
}

public class CreateWorkoutSessionDto
{
    public Guid? WorkoutTemplateId { get; set; }
    public DateTimeOffset SessionDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateWorkoutSessionDto
{
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
}

public class SessionExerciseDto
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public ExerciseDto Exercise { get; set; } = null!;
    public int OrderIndex { get; set; }
    public List<WorkoutSetDto> Sets { get; set; } = new();
}

public class CreateSessionExerciseDto
{
    public Guid ExerciseId { get; set; }
    public int OrderIndex { get; set; }
}

public class WorkoutSetDto
{
    public Guid Id { get; set; }
    public int SetNumber { get; set; }
    public bool Completed { get; set; }
    public object Metrics { get; set; } = new();
    public string? Notes { get; set; }
}
