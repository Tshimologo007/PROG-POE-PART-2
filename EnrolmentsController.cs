using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Models;
using RaceCompetitionAPI.Data;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrolment([FromBody] Enrolment enrolment)
        {
            var participant = await _context.Participants.FindAsync(enrolment.participantID);

            if (participant == null)
            {
                return NotFound("Participant not found.");
            }

            var eventCategory = await _context.EventCategories.FindAsync(enrolment.eventCategoryID);

            if (eventCategory == null)
            {
                return NotFound("Event Category not found.");
            }

            _context.Enrolments.Add(enrolment);

            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }

    }
}
