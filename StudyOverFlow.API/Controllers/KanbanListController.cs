using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyOverFlow.API.Data;
using StudyOverFlow.DTOs.Account;
using StudyOverFlow.API.Model;
using System.Security.Claims;



namespace StudyOverFlow.API.Controllers
{
    
    [Route("api/[controller]")]

    public class KanbanListController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;

        public KanbanListController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet("AddKanban")]
        public ActionResult AddKanban(string kanbanName)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            if (user is null)
            {
                return BadRequest();
            }
            string id = user.Value;
            var kans = _dbContext.KanbanLists.Where(c => c.UserId == id).ToList();
            if(kans.Count() > 0 && kans.Exists(c=>c.Title == kanbanName))
            {
                return BadRequest("there already exits kanbans with the same name");
            }
            _dbContext.Add(new KanbanList()
            {
                Title = kanbanName, 
                UserId= id,
                ListOrder = (kans.Count()>0) ?kans.Max(c=>c.ListOrder)+1 : 1,
            });
            _dbContext.SaveChanges();
            return Ok();

        }
    }
}
