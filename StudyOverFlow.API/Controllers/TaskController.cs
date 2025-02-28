using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.Model;
using StudyOverFlow.API.Services;
using StudyOverFlow.API.Services.Caching;
using StudyOverFlow.DTOs.Manage;
using System.Security.Claims;
using System.Threading.Tasks;
using Task = StudyOverFlow.API.Model.Task;

namespace StudyOverFlow.API.Controllers;

[Route("api/[controller]")]
public class TaskController : Controller
{
    public ApplicationDbContext _dbcontext;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAutomationService _autoService;

    public TaskController(ApplicationDbContext dbcontext, IMapper mapper, IAutomationService autoService, UserManager<ApplicationUser> userManager)
    {
        _dbcontext = dbcontext;
        _mapper = mapper;
        _autoService = autoService;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet("GetUserAllTasks")]
    public ActionResult<IEnumerable<TaskDto>> GetAllTasks()
    {
        var user = User.FindFirst(ClaimTypes.NameIdentifier);
        if (user is null)
        {
            return BadRequest();
        }
        string id = user.Value;
        var subjects = _dbcontext.Tasks
            .Where(c => c.UserId == id).ToList();

        return Ok(_mapper.Map<IEnumerable<TaskDto>>(subjects));
    }






    [Authorize]
    [HttpGet("GetOneUserTask/{taskid}")]
    public ActionResult<TaskDto> GetOneTask(int taskid)
    {

        var user = User.FindFirst(ClaimTypes.NameIdentifier);
        if (user is null)
        {
            return BadRequest();
        }
        string id = user.Value;


        var task = _dbcontext.Tasks.FirstOrDefault(c => c.UserId == id && c.TaskId == taskid);

        if (task is null)
            return NotFound("there is no Task with that name");
        return Ok(_mapper.Map<TaskDto>(task));
    }

    [Authorize]
    [HttpPost("EditTask")]
    public ActionResult EditTask(TaskDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (model.TaskId is null)
            return BadRequest("Task Id should be provided to preform this operation");
        var user = User.FindFirst(ClaimTypes.NameIdentifier);
        if (user is null)
        {
            return BadRequest();
        }
        string id = user.Value;

        var task = _dbcontext.Tasks.FirstOrDefault(c => c.TaskId == model.TaskId);

        if (task is null)
            return BadRequest("there isn't any task with that id");

        task.Title = model.Title;
        task.Description = model.Description;
        task.IsChecked = model.IsChecked;

        _dbcontext.Update(task);
        _dbcontext.SaveChanges();
        return Ok();

    }











    [Authorize]
    [HttpPost("AddTaskToKanbanList")]
    public ActionResult AddTaskToKanbanList(int taskId, string KanbanListTitle, int? controlIndex = null)
    {
        var result = _autoService.AddTaskToKanbanList(taskId, KanbanListTitle, controlIndex);
        if (!result.success)
            return BadRequest(result.message);
        return Ok();
    }
    [Authorize]
    [HttpPost("RemoveTaskToKanbanList")]
    public ActionResult RemoveTaskToKanbanList(int taskId)
    {
        var result = _autoService.RemoveTaskToKanbanList(taskId);
        if (!result.success)
            return BadRequest(result.message);
        return Ok();
    }


    [Authorize]
    [HttpPost("CreateTaskForUser")]
    public ActionResult CreateTaskForUser([FromBody] TaskDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = User.FindFirst(ClaimTypes.NameIdentifier);
        if (user is null)
        {
            return BadRequest();
        }
        string id = user.Value;
        model.UserId = id;
        _dbcontext.Add(_mapper.Map<Task>(model));

        _dbcontext.SaveChanges();
        return Ok("Task succesfully created");
    }
    [Authorize]
    [HttpPost("AddTagToTask")]
    public ActionResult AddTagToTask([FromBody] AddTagToTaskRequest request)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return Unauthorized();
        var result = _autoService.AddTagToTask(request,userId );
        if (!result.success)
            return BadRequest(result.message);
        return Ok();

    }





}