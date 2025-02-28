using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.Model;
using StudyOverFlow.DTOs.Manage;
using System.Security.Claims;
namespace StudyOverFlow.API.Controllers;

[Route("api/[controller]")]
public class SubjectController : Controller
{
    public ApplicationDbContext _dbcontext;
    public IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    public SubjectController(ApplicationDbContext dbcontext, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _dbcontext = dbcontext;
        _mapper = mapper;
        _userManager = userManager;
    }
    [Authorize]
    [HttpGet("GetUserSubjects")]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAllSubjects()
    {

        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();

        var subjects = await _dbcontext.Subjects
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<SubjectDto>>(subjects));


    }
    [Authorize]
    [HttpGet("GetOneUserSubject/{subjectId}")]
    public async Task< ActionResult<Subject>> GetOneSubject(int subjectId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();

        var subject = await _dbcontext.Subjects
          .Include(s => s.MaterialObjs)
          .Include(s => s.Tasks)
          .Include(s => s.Notes)
          .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.UserId == userId);

        return subject is null
            ? NotFound()
            : Ok(_mapper.Map<SubjectDto>(subject));
    }



    [Authorize]
    [HttpPost("CreateSubjectForUser")]
    public async Task <ActionResult> CreateSubject([FromBody]SubjectDto model)
    {

        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var exits =_dbcontext.Subjects.FirstOrDefault(c => c.Title == model.Title && c.UserId == userId);
        if (exits is not null)
            return BadRequest("their is a subject already exists with the same name.");
        var subject = _mapper.Map<Subject>(model);
        subject.UserId = userId;

        _dbcontext.Subjects.Add(subject);
        await _dbcontext.SaveChangesAsync();

        var createdSubject = _mapper.Map<SubjectDto>(subject);
        return CreatedAtAction(
            nameof(GetOneSubject),
            new { subjectId = createdSubject.SubjectId },
            createdSubject);


     
    }
    [HttpPut("UpdateSubject/{subjectId}")]
    public async Task<IActionResult> UpdateSubject(int subjectId, [FromBody]SubjectDto subjectDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized(); 

        var existingSubject = await _dbcontext.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.UserId == userId);

        if (existingSubject is null) return NotFound();

        var anotherSubject = await _dbcontext.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId != subjectId && s.UserId == userId && s.Title == subjectDto.Title);
        if (anotherSubject is not null)
            return BadRequest("their is a subject already exists with the same name.");

        _mapper.Map(subjectDto, existingSubject);
        await _dbcontext.SaveChangesAsync();

        return NoContent();
    }




    [HttpDelete("DeleteSubject/{subjectId}")]
    public async Task<IActionResult> DeleteSubject(int subjectId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();

        var subject = await _dbcontext.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.UserId == userId);

        if (subject is null) return NotFound();

        _dbcontext.Subjects.Remove(subject);
        await _dbcontext.SaveChangesAsync();

        return NoContent();
    }





}