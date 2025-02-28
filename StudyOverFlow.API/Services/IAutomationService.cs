using Microsoft.AspNetCore.Mvc;
using StudyOverFlow.DTOs.Manage;

namespace StudyOverFlow.API.Services
{
    public interface IAutomationService
    {
        public (bool success, string? message) AutomateTask(Model.Event Event, string UserId);

        public void aaa(int Event, DateTime dateTime, string UserId);
        public (bool success, string? message) AddTaskToKanbanList(int taskId, string KanbanListTitle, int? controlIndex = null);
        public (bool success, string? message) RemoveTaskToKanbanList(int taskId);

        public (bool success, string? message) AddTagToTask(AddTagToTaskRequest request, string userId);


    }
}
