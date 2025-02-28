
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyOverFlow.API.Controllers;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.Model;
using StudyOverFlow.DTOs.Manage;

namespace StudyOverFlow.API.Services
{
    public class AutomationService : IAutomationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBackgroundJobClient _jobClient;
       
        private readonly JobStorage _jobStorage;
        public AutomationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public  (bool success, string? message) AddTaskToKanbanList(int taskId, string KanbanListTitle, int? controlIndex = null)
        {
            var task = _dbContext.Tasks.FirstOrDefault(c => c.TaskId == taskId);
            if (task is null)
                return (false,"their is no Task with that id");
            var Kanban = _dbContext.KanbanLists.FirstOrDefault(c => c.Title == KanbanListTitle);
            if (Kanban is null)
                return (false,"their is no Kanban list with that title");
            var existpreivously = _dbContext.TaskKanbanLists.AsNoTracking().FirstOrDefault(c => c.TaskId == taskId);
            if (existpreivously is not null)
            {
                _dbContext.Remove(existpreivously);
                _dbContext.SaveChanges();
                _dbContext.TaskKanbanLists.Where(c => c.KanbanListId == existpreivously.KanbanListId && c.Index >= existpreivously.Index)
                    .ExecuteUpdate(x => x.SetProperty(c => c.Index, e => e.Index - 1));
                _dbContext.SaveChanges();
            }

            var taskkanbanlist = _dbContext.TaskKanbanLists.Where(c => c.KanbanListId == Kanban.KanbanListId).ToList();
            var mainindex = (taskkanbanlist.Count() <= 0) ? 0 : taskkanbanlist.Max(c => c.Index);

            if (controlIndex is not null)
            {
                _dbContext.TaskKanbanLists.Where(c => c.KanbanListId == Kanban.KanbanListId && c.Index >= controlIndex)
                    .ExecuteUpdate(x => x.SetProperty(c => c.Index, e => e.Index + 1));
                _dbContext.SaveChanges();
            }
            _dbContext.TaskKanbanLists.Add(new TaskKanbanList()
            {
                KanbanListId = taskId,
                TaskId = taskId,
                Index = mainindex + 1
            });
            _dbContext.SaveChanges();
            return (true, null);
        }



        public (bool success, string? message) AutomateTask(Model.Event Event, string UserId)
        {
            try
            {
                // Validate input
                if (Event == null || string.IsNullOrEmpty(UserId))
                    return (false, "Invalid input parameters.");

                // Check if Subject, Tag, or KanbanList exists in a single query
                if (Event.SubjectId.HasValue && !_dbContext.Subjects.Any(c => c.SubjectId == Event.SubjectId))
                    return (false, "Subject doesn't exist in the current context.");

                if (Event.TagId.HasValue && !_dbContext.Tags.Any(c => c.TagId == Event.TagId))
                    return (false, "Tag doesn't exist in the current context.");

                if (Event.KanbanListId.HasValue && !_dbContext.KanbanLists.Any(c => c.KanbanListId == Event.KanbanListId))
                    return (false, "Kanban doesn't exist in the current context.");

                // Schedule the background job
                BackgroundJob.Schedule<IAutomationService>(
                      service => service.aaa(Event.EventId, Event.Date.AddTicks(Event.DurationSpan.Ticks), UserId), // Pass only necessary data
                      Event.Date.AddTicks(Event.DurationSpan.Ticks) - DateTime.UtcNow 
                );
                return (true, "Task automated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public void aaa(int eventId, DateTime dateTime, string UserId)
        {
            try
            {
                // Fetch the event from the database
                var Event = _dbContext.Events.FirstOrDefault(e => e.EventId == eventId);
                if (Event == null)
                    throw new InvalidOperationException("Event not found.");

                // Create the task
                var Autotask = new Model.Task
                {
                    Title = $"{Event.Title} . {Event.TotalCount}",
                    StartDate = Event.Date + Event.DurationSpan,
                    EndDate = (Event.Date + Event.DurationSpan).AddDays(7),
                    IsChecked = false,
                    UserId = UserId,
                    SubjectId = Event.SubjectId
                };

                // Add the task to the database
                var track = _dbContext.Add(Autotask);
                _dbContext.SaveChanges();

                // Add to Kanban list if applicable
                if (Event.KanbanListId.HasValue)
                {
                    var kanbanTitle = _dbContext.KanbanLists
                        .FirstOrDefault(c => c.KanbanListId == Event.KanbanListId);
                    if (kanbanTitle != null)
                    {
                        AddTaskToKanbanList(track.Entity.TaskId, kanbanTitle.Title);
                    }
                }

                // Add tag if applicable
                if (Event.TagId.HasValue)
                {
                    _dbContext.TaskTags.Add(new TaskTag
                    {
                        TagId = Event.TagId.Value,
                        TaskId = track.Entity.TaskId,
                    });
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                throw new InvalidOperationException("Failed to automate task.", ex);
            }
        }









        //public (bool success,string? massage) AutomateTask (Model.Event Event, DateTime dateTime ,string UserId)
        //{
        //    if (Event.SubjectId.HasValue || Event.TagId.HasValue || Event.KanbanListId.HasValue)
        //    {
        //        if (Event.SubjectId.HasValue &&
        //            _dbContext.Subjects.Any(c => c.SubjectId == Event.SubjectId))
        //            return (false,"Subject Don't exist in the current context");

        //        if (Event.TagId.HasValue &&
        //            _dbContext.Tags.Any(c => c.TagId == Event.TagId))
        //            return (false,"tag Don't exist in the current context");
        //        if (Event.KanbanListId.HasValue &&
        //            _dbContext.KanbanLists.Any(c => c.KanbanListId == Event.KanbanListId))
        //            return (false,"Kanban Don't exist in the current context");
        //        BackgroundJob.Schedule<IAutomationService>(service => service.aaa(Event, dateTime, UserId),
        //            DateTime.UtcNow-dateTime );



        //    }
        //    return (false , " this Event Can't be automated");
        //}
        //public  void aaa(Model.Event Event, DateTime dateTime, string UserId)
        //{
        //    var Autotask = new Model.Task
        //    {
        //        Title = Event.Title + " . " + Event.TotalCount,
        //        StartDate = Event.Date + Event.DurationSpan,
        //        EndDate = (Event.Date + Event.DurationSpan).AddDays(7),
        //        IsChecked = false,
        //        UserId = UserId,
        //        SubjectId = Event.SubjectId.HasValue ? Event.SubjectId : null

        //    };

        //    var track = _dbContext.Add(Autotask);
        //    _dbContext.SaveChanges();

        //    if (Event.KanbanListId.HasValue)
        //    {
        //        var kantitle = _dbContext.KanbanLists.FirstOrDefault(c => c.KanbanListId == Event.KanbanListId);
        //        if (kantitle == null)
        //            return;

        //        AddTaskToKanbanList(track.Entity.TaskId, kantitle.Title);

        //    }
        //    if (Event.TagId.HasValue)
        //    {
        //        _dbContext.TaskTags.Add(new TaskTag
        //        {
        //            TagId = Event.TagId.Value,
        //            TaskId = track.Entity.TaskId,
        //        });
        //    }
        //}
        public (bool success, string? message) RemoveTaskToKanbanList(int taskId)
        {
            var task = _dbContext.Tasks.FirstOrDefault(c => c.TaskId == taskId);
            if (task is null)
                return (false,"their is no Task with that id");

            var existpreivously = _dbContext.TaskKanbanLists.AsNoTracking().FirstOrDefault(c => c.TaskId == taskId);
            if (existpreivously is not null)
            {
                _dbContext.Remove(existpreivously);
                _dbContext.SaveChanges();
                _dbContext.TaskKanbanLists.Where(c => c.KanbanListId == existpreivously.KanbanListId && c.Index >= existpreivously.Index)
                    .ExecuteUpdate(x => x.SetProperty(c => c.Index, e => e.Index - 1));
                _dbContext.SaveChanges();
            }
            return (true, null);
        }
       
        public (bool success, string? message) AddTagToTask( AddTagToTaskRequest request,string userId)
        {
           
            var task =  _dbContext.Tasks
                .FirstOrDefault(t => t.TaskId == request.TaskId && t.UserId == userId);
            var tag = _dbContext.Tags
                .FirstOrDefault(t => t.TagId == request.TagId && t.UserId == userId);

            if (task == null || tag == null)
                return (false,"Task or tag don't exitst .");

   
            var existingTaskTag =  _dbContext.TaskTags
                .FirstOrDefault(tt => tt.TaskId == request.TaskId && tt.TagId == request.TagId);

            if (existingTaskTag != null)
                return (false,"This tag is already assigned to the task.");

            var taskTag = new TaskTag
            {
                TaskId = request.TaskId,
                TagId = request.TagId
            };

            _dbContext.TaskTags.Add(taskTag);
            _dbContext.SaveChanges();
            return(true,null);
        }





    }
}
