using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;

        public RoutesController(
           ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{eventID}/routes")]
        public async Task<IActionResult> CreateRoute(int eventID, [FromBody] Routes route)
        {
            var raceEvent = await _context.Events.FindAsync(eventID);

            if (raceEvent == null)
                return NotFound("Event not found.");

            route.eventID = eventID;

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return Ok(route);
        }


        [HttpPut("{eventID}/routes")]
        public async Task<IActionResult> UpdateRoute(int eventID, [FromBody] Routes updatedRoute)
        {
            var raceEvent = await _context.Events.FindAsync(eventID);

            if (raceEvent == null)
                return NotFound("Event not found.");

            var route = await _context.Routes.FirstOrDefaultAsync(
                r => r.RoutesID == updatedRoute.RoutesID
                  && r.eventID == eventID);

            if (route == null)
                return NotFound("Route not found for this event.");

            route.route_Name = updatedRoute.route_Name;
            route.route_Description = updatedRoute.route_Description;
            route.mapURL = updatedRoute.mapURL;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("{eventID}/routes")]
        public async Task<IActionResult> GetEventRoutes(int eventID)
        {
            var raceEvent = await _context.Events.FindAsync(eventID);

            if (raceEvent == null)
                return NotFound("Event not found.");

            var routes = await _context.Routes
                .Where(r => r.eventID == eventID)
                .ToListAsync();

            return Ok(routes);
        }

    }
}
