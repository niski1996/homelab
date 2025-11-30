namespace HomeLabGymApi.DTOs;

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

public class CreateWorkoutSetDto
{
    public int SetNumber { get; set; }
    public object Metrics { get; set; } = new();
    public string? Notes { get; set; }
}

public class UpdateWorkoutSetDto
{
    public bool Completed { get; set; }
    public object Metrics { get; set; } = new();
    public string? Notes { get; set; }
}
