namespace StudyOverFlow.API.Model
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        public bool IsChecked { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }   
        public int? SubjectId { get; set; }  
        public Subject? Subject { get; set; }
        public List<Note> Notes { get; set; }
        public List<MaterialObj> MaterialObjs { get; set; }
        public List<TaskTag> TaskTags { get; set; }
        public List<TaskKanbanList> TaskKanbanLists { get; set; }
        public int? EventId { get; set; }    
        public Event? Event { get; set; }    
    }
}
