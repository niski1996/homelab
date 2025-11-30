using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HomeLabGymApi.Data;
using HomeLabGymApi.Models;
using HomeLabGymApi.DTOs;

namespace HomeLabGymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ExercisesController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetExercises(
        [FromQuery] string? category = null,
        [FromQuery] string? name = null,
        [FromQuery] string[]? tags = null)
    {
        var query = _context.Exercises
            .Include(e => e.ExerciseTags)
            .ThenInclude(et => et.Tag)
            .Include(e => e.Links)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(e => e.Category != null && e.Category.Contains(category));
        }

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(e => e.Name.Contains(name));
        }

        if (tags != null && tags.Length > 0)
        {
            query = query.Where(e => e.ExerciseTags.Any(et => tags.Contains(et.Tag.Name)));
        }

        var exercises = await query.ToListAsync();
        return Ok(_mapper.Map<List<ExerciseDto>>(exercises));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExerciseDto>> GetExercise(Guid id)
    {
        var exercise = await _context.Exercises
            .Include(e => e.ExerciseTags)
            .ThenInclude(et => et.Tag)
            .Include(e => e.Links)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ExerciseDto>(exercise));
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> CreateExercise(CreateExerciseDto createDto)
    {
        var exercise = _mapper.Map<Exercise>(createDto);
        
        // Handle tags
        foreach (var tagName in createDto.TagNames)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
            }
            exercise.ExerciseTags.Add(new ExerciseTag { Exercise = exercise, Tag = tag });
        }

        // Handle links
        foreach (var linkDto in createDto.Links)
        {
            var link = _mapper.Map<ExerciseLink>(linkDto);
            link.Exercise = exercise;
            exercise.Links.Add(link);
        }

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        var resultDto = await GetExercise(exercise.Id);
        return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, resultDto.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExercise(Guid id, UpdateExerciseDto updateDto)
    {
        var exercise = await _context.Exercises
            .Include(e => e.ExerciseTags)
            .Include(e => e.Links)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null)
        {
            return NotFound();
        }

        // Update basic properties
        _mapper.Map(updateDto, exercise);

        // Clear existing tags and links
        exercise.ExerciseTags.Clear();
        exercise.Links.Clear();

        // Add new tags
        foreach (var tagName in updateDto.TagNames)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
            }
            exercise.ExerciseTags.Add(new ExerciseTag { Exercise = exercise, Tag = tag });
        }

        // Add new links
        foreach (var linkDto in updateDto.Links)
        {
            var link = _mapper.Map<ExerciseLink>(linkDto);
            link.Exercise = exercise;
            exercise.Links.Add(link);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExercise(Guid id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
        {
            return NotFound();
        }

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
