using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HomeLabGymApi.Data;
using HomeLabGymApi.Models;
using HomeLabGymApi.DTOs;

namespace HomeLabGymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public WorkoutSessionsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutSessionDto>>> GetWorkoutSessions(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool? isCompleted = null,
        [FromQuery] Guid? templateId = null)
    {
        var query = _context.WorkoutSessions
            .Include(ws => ws.WorkoutTemplate)
            .Include(ws => ws.SessionExercises.OrderBy(se => se.OrderIndex))
            .ThenInclude(se => se.Exercise)
            .Include(ws => ws.SessionExercises)
            .ThenInclude(se => se.WorkoutSets.OrderBy(set => set.SetNumber))
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(ws => ws.SessionDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(ws => ws.SessionDate <= toDate.Value);
        }

        if (isCompleted.HasValue)
        {
            query = query.Where(ws => ws.IsCompleted == isCompleted.Value);
        }

        if (templateId.HasValue)
        {
            query = query.Where(ws => ws.WorkoutTemplateId == templateId.Value);
        }

        var sessions = await query.OrderByDescending(ws => ws.SessionDate).ToListAsync();
        return Ok(_mapper.Map<List<WorkoutSessionDto>>(sessions));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutSessionDto>> GetWorkoutSession(Guid id)
    {
        var session = await _context.WorkoutSessions
            .Include(ws => ws.WorkoutTemplate)
            .Include(ws => ws.SessionExercises.OrderBy(se => se.OrderIndex))
            .ThenInclude(se => se.Exercise)
            .ThenInclude(e => e.ExerciseTags)
            .ThenInclude(et => et.Tag)
            .Include(ws => ws.SessionExercises)
            .ThenInclude(se => se.WorkoutSets.OrderBy(set => set.SetNumber))
            .FirstOrDefaultAsync(ws => ws.Id == id);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<WorkoutSessionDto>(session));
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSessionDto>> CreateWorkoutSession(CreateWorkoutSessionDto createDto)
    {
        var session = _mapper.Map<WorkoutSession>(createDto);

        // If creating from template, copy exercises and sets
        if (createDto.WorkoutTemplateId.HasValue)
        {
            var template = await _context.WorkoutTemplates
                .Include(wt => wt.TemplateExercises.OrderBy(te => te.OrderIndex))
                .ThenInclude(te => te.TemplateSets.OrderBy(ts => ts.SetNumber))
                .FirstOrDefaultAsync(wt => wt.Id == createDto.WorkoutTemplateId.Value);

            if (template == null)
            {
                return BadRequest("Template not found");
            }

            foreach (var templateExercise in template.TemplateExercises)
            {
                var sessionExercise = new SessionExercise
                {
                    WorkoutSession = session,
                    ExerciseId = templateExercise.ExerciseId,
                    OrderIndex = templateExercise.OrderIndex
                };

                foreach (var templateSet in templateExercise.TemplateSets)
                {
                    var workoutSet = new WorkoutSet
                    {
                        SessionExercise = sessionExercise,
                        SetNumber = templateSet.SetNumber,
                        Metrics = templateSet.Metrics, // Copy planned metrics
                        Completed = false
                    };
                    sessionExercise.WorkoutSets.Add(workoutSet);
                }

                session.SessionExercises.Add(sessionExercise);
            }
        }

        _context.WorkoutSessions.Add(session);
        await _context.SaveChangesAsync();

        var resultDto = await GetWorkoutSession(session.Id);
        return CreatedAtAction(nameof(GetWorkoutSession), new { id = session.Id }, resultDto.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkoutSession(Guid id, UpdateWorkoutSessionDto updateDto)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        _mapper.Map(updateDto, session);
        session.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteWorkoutSession(Guid id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        session.IsCompleted = true;
        session.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Workout session completed" });
    }

    [HttpPost("{sessionId}/exercises/{exerciseId}/sets")]
    public async Task<ActionResult<WorkoutSetDto>> AddSetToExercise(
        Guid sessionId, 
        Guid exerciseId, 
        CreateWorkoutSetDto setDto)
    {
        var sessionExercise = await _context.SessionExercises
            .Include(se => se.WorkoutSets)
            .FirstOrDefaultAsync(se => se.WorkoutSessionId == sessionId && se.Id == exerciseId);

        if (sessionExercise == null)
        {
            return NotFound("Session exercise not found");
        }

        var nextSetNumber = sessionExercise.WorkoutSets.Count + 1;

        var workoutSet = _mapper.Map<WorkoutSet>(setDto);
        workoutSet.SessionExerciseId = exerciseId;
        workoutSet.SetNumber = nextSetNumber;

        _context.WorkoutSets.Add(workoutSet);
        await _context.SaveChangesAsync();

        var setDto_result = _mapper.Map<WorkoutSetDto>(workoutSet);
        return Ok(setDto_result);
    }

    [HttpPut("{sessionId}/exercises/{exerciseId}/sets/{setId}")]
    public async Task<IActionResult> UpdateWorkoutSet(
        Guid sessionId, 
        Guid exerciseId, 
        Guid setId, 
        UpdateWorkoutSetDto setDto)
    {
        var workoutSet = await _context.WorkoutSets
            .Include(ws => ws.SessionExercise)
            .FirstOrDefaultAsync(ws => ws.Id == setId && 
                                     ws.SessionExercise.Id == exerciseId && 
                                     ws.SessionExercise.WorkoutSessionId == sessionId);

        if (workoutSet == null)
        {
            return NotFound();
        }

        _mapper.Map(setDto, workoutSet);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutSession(Guid id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        _context.WorkoutSessions.Remove(session);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
