using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HomeLabGymApi.Data;
using HomeLabGymApi.Models;
using HomeLabGymApi.DTOs;

namespace HomeLabGymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TagsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var tags = await _context.Tags.ToListAsync();
        return Ok(_mapper.Map<List<TagDto>>(tags));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        
        if (tag == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<TagDto>(tag));
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createDto)
    {
        // Check if tag with same name already exists
        if (await _context.Tags.AnyAsync(t => t.Name == createDto.Name))
        {
            return BadRequest("Tag with this name already exists");
        }

        var tag = _mapper.Map<Tag>(createDto);
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var tagDto = _mapper.Map<TagDto>(tag);
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tagDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(Guid id, CreateTagDto updateDto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }

        // Check if another tag with same name exists
        if (await _context.Tags.AnyAsync(t => t.Name == updateDto.Name && t.Id != id))
        {
            return BadRequest("Tag with this name already exists");
        }

        _mapper.Map(updateDto, tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
