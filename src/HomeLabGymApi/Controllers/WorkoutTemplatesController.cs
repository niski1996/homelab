using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HomeLabGymApi.Data;
using HomeLabGymApi.Models;
using HomeLabGymApi.DTOs;

namespace HomeLabGymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutTemplatesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public WorkoutTemplatesController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutTemplateDto>>> GetWorkoutTemplates(
        [FromQuery] string? name = null,
        [FromQuery] string? category = null)
    {
        var query = _context.WorkoutTemplates
            .Include(wt => wt.TemplateExercises)
            .ThenInclude(te => te.Exercise)
            .Include(wt => wt.TemplateExercises)
            .ThenInclude(te => te.TemplateSets)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(wt => wt.Name.Contains(name));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(wt => wt.Category != null && wt.Category.Contains(category));
        }

        var templates = await query.OrderBy(wt => wt.Name).ToListAsync();
        return Ok(_mapper.Map<List<WorkoutTemplateDto>>(templates));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutTemplateDto>> GetWorkoutTemplate(Guid id)
    {
        var template = await _context.WorkoutTemplates
            .Include(wt => wt.TemplateExercises.OrderBy(te => te.OrderIndex))
            .ThenInclude(te => te.Exercise)
            .ThenInclude(e => e.ExerciseTags)
            .ThenInclude(et => et.Tag)
            .Include(wt => wt.TemplateExercises)
            .ThenInclude(te => te.TemplateSets.OrderBy(ts => ts.SetNumber))
            .FirstOrDefaultAsync(wt => wt.Id == id);

        if (template == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<WorkoutTemplateDto>(template));
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutTemplateDto>> CreateWorkoutTemplate(CreateWorkoutTemplateDto createDto)
    {
        var template = _mapper.Map<WorkoutTemplate>(createDto);

        // Add template exercises if provided
        if (createDto.Exercises?.Any() == true)
        {
            foreach (var exerciseDto in createDto.Exercises)
            {
                // Verify exercise exists
                var exercise = await _context.Exercises.FindAsync(exerciseDto.ExerciseId);
                if (exercise == null)
                {
                    return BadRequest($"Exercise with ID {exerciseDto.ExerciseId} not found");
                }

                var templateExercise = new TemplateExercise
                {
                    WorkoutTemplate = template,
                    ExerciseId = exerciseDto.ExerciseId,
                    OrderIndex = exerciseDto.OrderIndex
                };

                // Add template sets if provided
                if (exerciseDto.Sets?.Any() == true)
                {
                    foreach (var setDto in exerciseDto.Sets)
                    {
                        var templateSet = _mapper.Map<TemplateSet>(setDto);
                        templateSet.TemplateExercise = templateExercise;
                        templateExercise.TemplateSets.Add(templateSet);
                    }
                }

                template.TemplateExercises.Add(templateExercise);
            }
        }

        _context.WorkoutTemplates.Add(template);
        await _context.SaveChangesAsync();

        var resultDto = await GetWorkoutTemplate(template.Id);
        return CreatedAtAction(nameof(GetWorkoutTemplate), new { id = template.Id }, resultDto.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkoutTemplate(Guid id, UpdateWorkoutTemplateDto updateDto)
    {
        var template = await _context.WorkoutTemplates
            .Include(wt => wt.TemplateExercises)
            .ThenInclude(te => te.TemplateSets)
            .FirstOrDefaultAsync(wt => wt.Id == id);

        if (template == null)
        {
            return NotFound();
        }

        // Update basic properties
        template.Name = updateDto.Name;
        template.Description = updateDto.Description;
        template.Category = updateDto.Category;
        template.UpdatedAt = DateTimeOffset.UtcNow;

        // Update template exercises if provided
        if (updateDto.Exercises != null)
        {
            // Clear existing exercises and sets
            _context.TemplateSets.RemoveRange(
                template.TemplateExercises.SelectMany(te => te.TemplateSets));
            _context.TemplateExercises.RemoveRange(template.TemplateExercises);

            // Add new exercises
            foreach (var exerciseDto in updateDto.Exercises)
            {
                var exercise = await _context.Exercises.FindAsync(exerciseDto.ExerciseId);
                if (exercise == null)
                {
                    return BadRequest($"Exercise with ID {exerciseDto.ExerciseId} not found");
                }

                var templateExercise = new TemplateExercise
                {
                    WorkoutTemplate = template,
                    ExerciseId = exerciseDto.ExerciseId,
                    OrderIndex = exerciseDto.OrderIndex
                };

                if (exerciseDto.Sets?.Any() == true)
                {
                    foreach (var setDto in exerciseDto.Sets)
                    {
                        var templateSet = _mapper.Map<TemplateSet>(setDto);
                        templateSet.TemplateExercise = templateExercise;
                        templateExercise.TemplateSets.Add(templateSet);
                    }
                }

                template.TemplateExercises.Add(templateExercise);
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutTemplate(Guid id)
    {
        var template = await _context.WorkoutTemplates.FindAsync(id);
        if (template == null)
        {
            return NotFound();
        }

        _context.WorkoutTemplates.Remove(template);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/exercises")]
    public async Task<IActionResult> AddExerciseToTemplate(Guid id, CreateTemplateExerciseDto exerciseDto)
    {
        var template = await _context.WorkoutTemplates.FindAsync(id);
        if (template == null)
        {
            return NotFound("Template not found");
        }

        var exercise = await _context.Exercises.FindAsync(exerciseDto.ExerciseId);
        if (exercise == null)
        {
            return BadRequest("Exercise not found");
        }

        var templateExercise = new TemplateExercise
        {
            WorkoutTemplateId = id,
            ExerciseId = exerciseDto.ExerciseId,
            OrderIndex = exerciseDto.OrderIndex
        };

        _context.TemplateExercises.Add(templateExercise);
        await _context.SaveChangesAsync();

        return Ok(new { Id = templateExercise.Id });
    }

    [HttpDelete("{templateId}/exercises/{exerciseId}")]
    public async Task<IActionResult> RemoveExerciseFromTemplate(Guid templateId, Guid exerciseId)
    {
        var templateExercise = await _context.TemplateExercises
            .FirstOrDefaultAsync(te => te.WorkoutTemplateId == templateId && te.Id == exerciseId);

        if (templateExercise == null)
        {
            return NotFound();
        }

        _context.TemplateExercises.Remove(templateExercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
