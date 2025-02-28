using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.Model;
using StudyOverFlow.DTOs.Consts;
using StudyOverFlow.DTOs.Manage;

namespace StudyOverFlow.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public TagController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

    
        [HttpGet("GetTags")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var userId = _userManager.GetUserId(User);
            var tags =  await _context.Tags
                .Where(t => t.UserId == userId)
                .Include(t => t.Color)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TagDto>>(tags));
        }

        [HttpGet("GetTag/{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var userId = _userManager.GetUserId(User);
            var tag = await _context.Tags
                .Include(t => t.Color)
                .FirstOrDefaultAsync(t => t.TagId == id && t.UserId == userId);

            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPost("CreateTag")]
        public async Task<ActionResult<Tag>> CreateTag([FromBody] TagDto model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Tag tag = _mapper.Map<Tag>(model);  
            
            tag.UserId = userId;


            var tagNumber =  _context.Tags
                .Count(t => t.UserId == userId);
            if (tagNumber >= (int)Numbers.MaxTags)
                return BadRequest($"max NumberofTag is {(int)Numbers.MaxTags}.");
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = tag.TagId }, tag);
        }

        [HttpPut("UpdateTag/{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] TagDto model)
        {
           

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();


            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Tag tag= _mapper.Map<Tag>(model);   
            tag.TagId= id;
            tag.UserId = userId;
            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost("DeleteTag/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var userId = _userManager.GetUserId(User);
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.TagId == id && t.UserId == userId);

            if (tag == null)
                return NotFound();

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }


}
