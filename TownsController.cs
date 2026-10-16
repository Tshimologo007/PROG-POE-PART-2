using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.Models;

[ApiController]
[Route("api/towns")]
public class TownsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TownsController(
        ApplicationDbContext context)
    {
        _context = context;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetTowns()
    {
        var towns = await _context.Towns.ToListAsync();
        return Ok(towns);
    }

    
    [HttpGet("{townID}")]
    public async Task<IActionResult> GetTown(int townID)
    {
        var town = await _context.Towns.FindAsync(townID);

        if (town == null)
            return NotFound();

        return Ok(town);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateTown([FromBody] Town town)
    {
        _context.Towns.Add(town);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTown),
            new { townID = town.townID },
            town);
    }

    [HttpPut("{townID}")]
    public async Task<IActionResult> UpdateTown(
        int townID, [FromBody] Town updatedTown)
    {
        var town = await _context.Towns.FindAsync(townID);

        if (town == null)
            return NotFound();

        town.town_Name = updatedTown.town_Name;
        town.province = updatedTown.province;
        town.postal_code = updatedTown.postal_code;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    
    [HttpDelete("{townID}")]
    public async Task<IActionResult> DeleteTown(int townID)
    {
        var town = await _context.Towns.FindAsync(townID);

        if (town == null)
            return NotFound();

        _context.Towns.Remove(town);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

