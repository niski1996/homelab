using System.Net.Http.Json;
using System.Text.Json;
using HomeLab.Models;

namespace HomeLab.Services;

public class GymApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public GymApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    // Exercises
    public async Task<List<ExerciseDto>> GetExercisesAsync(string? category = null, string? name = null, string[]? tags = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrEmpty(category)) query.Add($"category={Uri.EscapeDataString(category)}");
        if (!string.IsNullOrEmpty(name)) query.Add($"name={Uri.EscapeDataString(name)}");
        if (tags?.Length > 0) query.AddRange(tags.Select(tag => $"tags={Uri.EscapeDataString(tag)}"));
        
        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
        var response = await _httpClient.GetFromJsonAsync<List<ExerciseDto>>($"api/exercises{queryString}", _jsonOptions);
        return response ?? new List<ExerciseDto>();
    }

    public async Task<ExerciseDto?> GetExerciseAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ExerciseDto>($"api/exercises/{id}", _jsonOptions);
    }

    public async Task<ExerciseDto?> CreateExerciseAsync(CreateExerciseDto exercise)
    {
        var response = await _httpClient.PostAsJsonAsync("api/exercises", exercise, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ExerciseDto>(_jsonOptions);
    }

    public async Task UpdateExerciseAsync(Guid id, UpdateExerciseDto exercise)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/exercises/{id}", exercise, _jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteExerciseAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/exercises/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Tags
    public async Task<List<TagDto>> GetTagsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<TagDto>>("api/tags", _jsonOptions);
        return response ?? new List<TagDto>();
    }

    public async Task<TagDto?> CreateTagAsync(CreateTagDto tag)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tags", tag, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TagDto>(_jsonOptions);
    }

    public async Task UpdateTagAsync(Guid id, UpdateTagDto tag)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tags/{id}", tag, _jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTagAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/tags/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Workout Templates
    public async Task<List<WorkoutTemplateDto>> GetWorkoutTemplatesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<WorkoutTemplateDto>>("api/workout-templates", _jsonOptions);
        return response ?? new List<WorkoutTemplateDto>();
    }

    public async Task<WorkoutTemplateDto?> GetWorkoutTemplateAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<WorkoutTemplateDto>($"api/workout-templates/{id}", _jsonOptions);
    }
}
