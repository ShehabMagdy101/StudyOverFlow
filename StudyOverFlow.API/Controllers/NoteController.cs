using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.Model;
using StudyOverFlow.API.Services.Caching;
using StudyOverFlow.DTOs.Manage;
using System.Net;
using System.Security.Claims;


namespace StudyOverFlow.API.Controllers
{

    [Route("api/[controller]")]
    public class NoteController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cache;
        public NoteController(ApplicationDbContext dbcontext, IMapper mapper, IRedisCacheService cache)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
            _cache = cache;
        }
        [Authorize]
        [HttpGet("GetUserAllNotes")]
        public ActionResult<IEnumerable<NoteDto>> GetAllNotes()
        {
            
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user is null)
            {
                return BadRequest();
            }
            string id = user.Value;
            string CachingKey = $"{id}_Notes";
            var notes = _cache.GetData<IEnumerable<NoteDto>>(CachingKey);
            if(notes is not null)
            {
                return Ok(notes);   
            }

            notes = _dbcontext.Notes
                .Where(c => c.UserId == id).Select(c => new NoteDto
                {
                    NoteId = c.NoteId,
                    Text = c.text
                }).ToList();

            _cache.SetData(CachingKey, notes);
            return Ok(notes);
        }
      


        [Authorize]
        [HttpGet("GetUserGeneralNotes")]
        public ActionResult<IEnumerable<NoteDto>> GetAllGeneralNotes()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user is null)
            {
                return BadRequest();
            }
            string id = user.Value;
            var notes = _dbcontext.Notes
                .Where(c => c.UserId == id && c.SubjectId == null && c.TaskId== null).Select(c => new NoteDto
                {
                    NoteId = c.NoteId
                }).ToList();


            return Ok(_mapper.Map<IEnumerable<NoteDto>>(notes));
        }




        [Authorize]
        [HttpPost("CreateNoteForUser")]
        public ActionResult CreateNote([FromBody] NoteDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            var m = ClaimTypes.NameIdentifier;
            var user = User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier);
            if (user is null)
            {
                return BadRequest();
            }
            _dbcontext.Add(new Note
            {
                
                DateTime = DateTime.UtcNow,
                text = model.Text,
                UserId= user.Value,
                SubjectId= model.SubjectId ,
                TaskId= model.TaskId    
            });
            _dbcontext.SaveChanges();
            _cache.DeleteData($"{user.Value}_Notes");

            return Ok();
        }


        




    }
}
