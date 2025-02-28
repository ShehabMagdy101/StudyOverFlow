using AutoMapper;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.Model;
using StudyOverFlow.API.Services;
using StudyOverFlow.API.Services.Caching;
using StudyOverFlow.DTOs.Manage;
using System.Globalization;
using System.Security.Claims;

namespace StudyOverFlow.API.Controllers
{
    [Route("api/[controller]")]
    public class SchedulerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cache;
        private readonly IAutomationService _autoService;

        public SchedulerController(ApplicationDbContext dbcontext, IMapper mapper, IRedisCacheService cache, IAutomationService autoService)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
            _cache = cache;
            _autoService = autoService;
        }
        [Authorize]
        [HttpPost("AddEvent")]
        public ActionResult AddEvent([FromBody] EventDto model)
        {
            var ev = _dbcontext.Events.FirstOrDefault(c => c.Title == model.Title);
            if (ev is not null)
            {
                ModelState.AddModelError("Event", "Event Name Already Exist.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }




            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (user is null)
                return BadRequest();
            var FindUser = _dbcontext.Users.Include(c => c.Calendar)
                .FirstOrDefault(c => c.Id == user.Value);
            if (FindUser is null)
                return BadRequest();
            Model.Calendar calender;
            if (FindUser.Calendar is null)
            {
                var record = _dbcontext.Calendars.Add(new Model.Calendar()
                {
                    UserId = FindUser.Id,
                    Duration = "2",
                    start = new TimeOnly(8, 0),
                    End = new TimeOnly(23, 59)

                });
                _dbcontext.SaveChanges();
                calender = record.Entity;
            }
            calender = FindUser.Calendar!;

            if (calender.Events.Any(c => c.Title == model.Title))
                return BadRequest("this event already exist");


            if (!_dbcontext.Colors.Select(c => c.ColorId).Contains(model.ColorId))
                return BadRequest("color don't exits in the current context.");
            if (model.TotalCount.HasValue && model.TotalCount > 0)
            {
                var eventsToAdd = new List<Model.Event>();

                for (int i = 0; i < model.TotalCount; i++)
                {

                    var Event = _mapper.Map<Model.Event>(model);
                    Event.TotalCount = i + 1;
                    Event.CurrentCount += 1;
                    Event.Date = Event.Date.ToUniversalTime().AddDays(7 * i);
                    Event.CalendarId = calender.CalendarId;
                    eventsToAdd.Add(Event);

                }

                _dbcontext.AddRange(eventsToAdd);
                _dbcontext.SaveChanges();

                foreach (var Event in eventsToAdd)
                {

                    if (Event.SubjectId.HasValue || Event.TagId.HasValue || Event.KanbanListId.HasValue)
                    {
                        var result = _autoService.AutomateTask(Event, FindUser.Id);
                        if (!result.success)
                        {
                            // Rollback the transaction if automation fails
                            _dbcontext.Database.CurrentTransaction?.Rollback();
                            return BadRequest(result.message);
                        }
                    }

                }

                return Ok();
            }
            if ((!model.TotalCount.HasValue) && model.SubjectId.HasValue || model.TagId.HasValue || model.KanbanListId.HasValue)
                return BadRequest("can't automate event that have no count value");
            var mEvent = _mapper.Map<Model.Event>(model);
            mEvent.Date = mEvent.Date.ToUniversalTime();
            mEvent.CalendarId = calender.CalendarId;
            _dbcontext.Add(mEvent);
            _dbcontext.SaveChanges();
            return Ok();

        }


        [Authorize]
        [HttpPut("MoveEventsOneweek")]
        public ActionResult MoveEventsOneweek()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("GetAllEvents")]
        public ActionResult<IEnumerable<EventDto>> GetAllEvents()
        {

            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (user is null)
                return BadRequest();
            var FindUser = _dbcontext.Users.Include(c => c.Calendar)
                .ThenInclude(c => c.Events)
               .FirstOrDefault(c => c.Id == user.Value);
            if (FindUser is null)
                return BadRequest();
            var Events = _mapper.Map<List<EventDto>>(FindUser.Calendar.Events);
            return Ok(Events);

        }


        [Authorize]
        [HttpGet("GetCurrentWeekEvents/{currentDate?}")]
        public ActionResult<IEnumerable<Model.Event>> GetCurrentWeekEvents(DateTime? currentDate = null)
        {
            currentDate ??= DateTime.UtcNow;
            // Calculate the start of the week (previous Saturday)
            int daysToSubtract = (currentDate.Value.DayOfWeek - DayOfWeek.Saturday + 7) % 7;
            DateTime startOfWeek = currentDate.Value.AddDays(-daysToSubtract).Date.ToUniversalTime(); // Midnight of Saturday

            // Calculate the end of the week (next Friday)
            DateTime endOfWeek = startOfWeek.AddDays(6).Date.ToUniversalTime(); // Midnight of Friday

            // Filter tasks where Date is between start and end (inclusive)
            return _dbcontext.Events.Where(t => (t.Date.Date >= startOfWeek && t.Date.Date <= endOfWeek) || t.TotalCount == null).ToList();
        }


        [Authorize]
        [HttpPut("EditEvent")]
        public ActionResult EditEvent([FromBody] EventDto model)
        {
            if (model.EventId is null)
            {
                ModelState.AddModelError("Event Id", "Event Id not exist in the current request.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (user is null)
                return BadRequest();
            var FindUser = _dbcontext.Users.AsNoTracking().Include(c => c.Calendar)
                .ThenInclude(c => c.Events).FirstOrDefault(c => c.Id == user!.Value);
            if (FindUser is null)
                return NotFound();
            if (FindUser.Calendar is null)
                return NotFound();


            var Event = FindUser.Calendar.Events.FirstOrDefault(c => c.EventId == model.EventId);
            if (Event is null)
                return NotFound();
            if (!_dbcontext.Colors.Select(c => c.ColorId).Contains(model.ColorId))
                return BadRequest("color don't exits in the current context.");
            _mapper.Map(model, Event);

            _dbcontext.Update(Event);
            _dbcontext.SaveChanges();
            return Ok();


        }
        //[Authorize]
        //[HttpPost("DeleteEvent")]
        //public ActionResult RemoveEvent(int id)
        //{

        //}


    }
}
