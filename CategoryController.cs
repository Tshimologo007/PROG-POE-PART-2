using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Data;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CategoryController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        
        [HttpGet("{categoryID}")]
        public async Task<IActionResult> GetCategory(int categoryID)
        {
            var category = await _context.Categories.FindAsync(categoryID);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { categoryID = category.categoryID },
                category);
        }

       
        [HttpPut("{categoryID}")]
        public async Task<IActionResult> UpdateCategory(
            int categoryID, [FromBody] Category updatedCategory)
        {
            var category = await _context.Categories.FindAsync(categoryID);

            if (category == null)
                return NotFound();

            category.category_Name = updatedCategory.category_Name;
            category.description = updatedCategory.description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{categoryID}")]
        public async Task<IActionResult> DeleteCategory(int categoryID)
        {
            var category = await _context.Categories.FindAsync(categoryID);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
