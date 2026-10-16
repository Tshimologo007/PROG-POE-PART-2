using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceCompetitionAPI.Data;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ResultsController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("/api/participant/{participantID}/results")]
        public async Task<IActionResult> GetParticipantResults(int participantID)
        {
            var participant = await _context.Participants
                .FindAsync(participantID);

            if (participant == null)
                return NotFound("Participant not found.");

            var results = await (
                from r in _context.Results
                join en in _context.Enrolments
                    on r.enrolmentID equals en.enrolmentID
                where en.participantID == participantID
                select r
            ).ToListAsync();

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResult([FromBody] Result result)
        {
            var enrolment = await _context.Enrolments
                .FindAsync(result.enrolmentID);

            if (enrolment == null)
                return NotFound("Enrolment not found.");

            var existingResult = await _context.Results
                .AnyAsync(r => r.enrolmentID == result.enrolmentID);

            if (existingResult)
                return Conflict("A result already exists for this enrolment.");

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        [HttpPut("{resultID}")]
        public async Task<IActionResult> UpdateResult(
    int resultID, [FromBody] Result updatedResult)
        {
            var result = await _context.Results.FindAsync(resultID);

            if (result == null)
                return NotFound("Result not found.");

            result.finish_time = updatedResult.finish_time;
            result.distance = updatedResult.distance;
            result.completionStatus = updatedResult.completionStatus;
            result.position = updatedResult.position;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{eventID}/results")]
        public async Task<IActionResult> GetEventResults(int eventID)
        {
            var raceEvent = await _context.Events.FindAsync(eventID);

            if (raceEvent == null)
                return NotFound("Event not found.");

            var results = await (
                from r in _context.Results
                join en in _context.Enrolments
                    on r.enrolmentID equals en.enrolmentID
                join ec in _context.EventCategories
                    on en.eventCategoryID equals ec.eventCategoryID
                where ec.eventID == eventID
                select r
            ).ToListAsync();

            return Ok(results);
        }

    }
}
