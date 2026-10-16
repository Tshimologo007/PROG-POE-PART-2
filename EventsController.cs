using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Models;
using RaceCompetitionAPI.Data;


namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public EventsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events.ToListAsync();

            return (events);
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<Event>> GetEventbyID(int eventId)
        {
            var events = await _context.Events.FirstOrDefaultAsync(i => i.eventID == eventId);

            if (events == null)
            {
                return NotFound($"Event with id {eventId} was not found.");
            }

            return (events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody]Event newEvent)
        {
            _context.Events.Add(newEvent);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvents),
                new { eventID = newEvent.eventID },
                newEvent);
        }

        [HttpPut("{eventID}")]
        public async Task<ActionResult<Event>> UpdateEvent(int eventID, Event updatedEvent)
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(i => i.eventID == eventID);

            if (existingEvent == null) 
            {
                return NotFound($"Event id {eventID} was not found.");
            }

            existingEvent.event_Name = updatedEvent.event_Name;
            existingEvent.organiserID = updatedEvent.organiserID;
            existingEvent.description = updatedEvent.description;
            existingEvent.event_Date = updatedEvent.event_Date;
            existingEvent.event_Type = updatedEvent.event_Type;
            existingEvent.venue = updatedEvent.venue;
            
            await _context.SaveChangesAsync();
                
            return NoContent();
        }

        [HttpDelete("{eventID}")]
            public async Task<IActionResult> DeleteEvent(int eventID) 
        {
            var deleteEvent = await _context.Events.FirstOrDefaultAsync(e => e.eventID == eventID);

            if (deleteEvent == null) 
            {
                return NotFound($"Event with id {eventID} was not found");
            }

            _context.Events.Remove(deleteEvent);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{eventID}")]
        public async Task<IActionResult> CreateCategory(int eventID, [FromBody] int categoryID) 
        {
            var events = await _context.Events.FirstOrDefaultAsync(e => e.eventID == eventID);

            if (events == null)
            {
                NotFound($"event id {eventID} is not found.");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(e => e.categoryID == categoryID);

            if (category == null)
            {
                NotFound($"Category id {categoryID} is not found.");
            }

            var association = new EventCategory
            {
                eventID = eventID,
                categoryID = categoryID
            };

            _context.EventCategories.Add(association);

            await _context.SaveChangesAsync();

            return Ok(association);
        }

        [HttpDelete("{eventID}/categories/{categoryID}")]
        public async Task<IActionResult> DeleteEventCategory(int eventID, int categoryID)
        {
            var association = await _context.EventCategories.FirstOrDefaultAsync(ec =>ec.eventID == eventID && ec.categoryID == categoryID);

            if (association == null)
            {
                return NotFound();
            }

            _context.EventCategories.Remove(association);

            await _context.SaveChangesAsync();

            return NoContent();
        }

     


    }
}
