namespace HomeLabGymApi.DTOs;

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
