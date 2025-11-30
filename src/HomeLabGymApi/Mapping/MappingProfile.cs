using AutoMapper;
using HomeLabGymApi.Models;
using HomeLabGymApi.DTOs;
using System.Text.Json;

namespace HomeLabGymApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Exercise mappings
        CreateMap<Exercise, ExerciseDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ExerciseTags.Select(et => et.Tag)))
            .ForMember(dest => dest.Links, opt => opt.MapFrom(src => src.Links))
            .ForMember(dest => dest.AdditionalSettings, opt => opt.MapFrom<AdditionalSettingsResolver>());

        CreateMap<CreateExerciseDto, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExerciseTags, opt => opt.Ignore())
            .ForMember(dest => dest.Links, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateExercises, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExercises, opt => opt.Ignore())
            .ForMember(dest => dest.AdditionalSettings, opt => opt.MapFrom<CreateAdditionalSettingsResolver>());

        CreateMap<UpdateExerciseDto, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExerciseTags, opt => opt.Ignore())
            .ForMember(dest => dest.Links, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateExercises, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExercises, opt => opt.Ignore())
            .ForMember(dest => dest.AdditionalSettings, opt => opt.MapFrom<UpdateAdditionalSettingsResolver>());

        // Tag mappings
        CreateMap<Tag, TagDto>();
        CreateMap<CreateTagDto, Tag>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExerciseTags, opt => opt.Ignore());

        // ExerciseLink mappings
        CreateMap<ExerciseLink, ExerciseLinkDto>();
        CreateMap<CreateExerciseLinkDto, ExerciseLink>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExerciseId, opt => opt.Ignore())
            .ForMember(dest => dest.Exercise, opt => opt.Ignore());

        // WorkoutTemplate mappings
        CreateMap<WorkoutTemplate, WorkoutTemplateDto>()
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.TemplateExercises));

        CreateMap<CreateWorkoutTemplateDto, WorkoutTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateExercises, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutSessions, opt => opt.Ignore());

        CreateMap<UpdateWorkoutTemplateDto, WorkoutTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateExercises, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutSessions, opt => opt.Ignore());

        // TemplateExercise mappings
        CreateMap<TemplateExercise, TemplateExerciseDto>()
            .ForMember(dest => dest.Sets, opt => opt.MapFrom(src => src.TemplateSets));

        CreateMap<CreateTemplateExerciseDto, TemplateExercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutTemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutTemplate, opt => opt.Ignore())
            .ForMember(dest => dest.Exercise, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateSets, opt => opt.Ignore());

        // TemplateSet mappings
        CreateMap<TemplateSet, TemplateSetDto>()
            .ForMember(dest => dest.Metrics, opt => opt.MapFrom<TemplateSetMetricsResolver>());

        CreateMap<CreateTemplateSetDto, TemplateSet>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateExerciseId, opt => opt.Ignore())
            .ForMember(dest => dest.TemplateExercise, opt => opt.Ignore())
            .ForMember(dest => dest.Metrics, opt => opt.MapFrom<CreateTemplateSetMetricsResolver>());

        // WorkoutSession mappings
        CreateMap<WorkoutSession, WorkoutSessionDto>()
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.SessionExercises));

        CreateMap<CreateWorkoutSessionDto, WorkoutSession>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutTemplate, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExercises, opt => opt.Ignore());

        CreateMap<UpdateWorkoutSessionDto, WorkoutSession>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutTemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.SessionDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutTemplate, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExercises, opt => opt.Ignore());

        // SessionExercise mappings
        CreateMap<SessionExercise, SessionExerciseDto>()
            .ForMember(dest => dest.Sets, opt => opt.MapFrom(src => src.WorkoutSets));

        CreateMap<CreateSessionExerciseDto, SessionExercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutSessionId, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutSession, opt => opt.Ignore())
            .ForMember(dest => dest.Exercise, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutSets, opt => opt.Ignore());

        // WorkoutSet mappings
        CreateMap<WorkoutSet, WorkoutSetDto>()
            .ForMember(dest => dest.Metrics, opt => opt.MapFrom<WorkoutSetMetricsResolver>());

        CreateMap<CreateWorkoutSetDto, WorkoutSet>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExerciseId, opt => opt.Ignore())
            .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.SessionExercise, opt => opt.Ignore())
            .ForMember(dest => dest.Metrics, opt => opt.MapFrom<CreateWorkoutSetMetricsResolver>());

        CreateMap<UpdateWorkoutSetDto, WorkoutSet>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExerciseId, opt => opt.Ignore())
            .ForMember(dest => dest.SetNumber, opt => opt.Ignore())
            .ForMember(dest => dest.SessionExercise, opt => opt.Ignore())
            .ForMember(dest => dest.Metrics, opt => opt.MapFrom<UpdateWorkoutSetMetricsResolver>());
    }
}

// Value Resolvers for JSON handling
public class AdditionalSettingsResolver : IValueResolver<Exercise, ExerciseDto, object?>
{
    public object? Resolve(Exercise source, ExerciseDto destination, object? destMember, ResolutionContext context)
    {
        return source.AdditionalSettings != null ? 
            JsonSerializer.Deserialize<object>(source.AdditionalSettings.RootElement.GetRawText()) : 
            null;
    }
}

public class CreateAdditionalSettingsResolver : IValueResolver<CreateExerciseDto, Exercise, JsonDocument?>
{
    public JsonDocument? Resolve(CreateExerciseDto source, Exercise destination, JsonDocument? destMember, ResolutionContext context)
    {
        return source.AdditionalSettings != null ? 
            JsonDocument.Parse(JsonSerializer.Serialize(source.AdditionalSettings)) : 
            null;
    }
}

public class UpdateAdditionalSettingsResolver : IValueResolver<UpdateExerciseDto, Exercise, JsonDocument?>
{
    public JsonDocument? Resolve(UpdateExerciseDto source, Exercise destination, JsonDocument? destMember, ResolutionContext context)
    {
        return source.AdditionalSettings != null ? 
            JsonDocument.Parse(JsonSerializer.Serialize(source.AdditionalSettings)) : 
            null;
    }
}

public class TemplateSetMetricsResolver : IValueResolver<TemplateSet, TemplateSetDto, object>
{
    public object Resolve(TemplateSet source, TemplateSetDto destination, object destMember, ResolutionContext context)
    {
        return JsonSerializer.Deserialize<object>(source.Metrics.RootElement.GetRawText());
    }
}

public class CreateTemplateSetMetricsResolver : IValueResolver<CreateTemplateSetDto, TemplateSet, JsonDocument>
{
    public JsonDocument Resolve(CreateTemplateSetDto source, TemplateSet destination, JsonDocument destMember, ResolutionContext context)
    {
        return JsonDocument.Parse(JsonSerializer.Serialize(source.Metrics));
    }
}

public class WorkoutSetMetricsResolver : IValueResolver<WorkoutSet, WorkoutSetDto, object>
{
    public object Resolve(WorkoutSet source, WorkoutSetDto destination, object destMember, ResolutionContext context)
    {
        return JsonSerializer.Deserialize<object>(source.Metrics.RootElement.GetRawText());
    }
}

public class CreateWorkoutSetMetricsResolver : IValueResolver<CreateWorkoutSetDto, WorkoutSet, JsonDocument>
{
    public JsonDocument Resolve(CreateWorkoutSetDto source, WorkoutSet destination, JsonDocument destMember, ResolutionContext context)
    {
        return JsonDocument.Parse(JsonSerializer.Serialize(source.Metrics));
    }
}

public class UpdateWorkoutSetMetricsResolver : IValueResolver<UpdateWorkoutSetDto, WorkoutSet, JsonDocument>
{
    public JsonDocument Resolve(UpdateWorkoutSetDto source, WorkoutSet destination, JsonDocument destMember, ResolutionContext context)
    {
        return JsonDocument.Parse(JsonSerializer.Serialize(source.Metrics));
    }
}
