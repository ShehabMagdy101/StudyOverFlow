using StudyOverFlow.DTOs.Consts;

namespace StudyOverFlow.API.Model
{
    public class KanbanList
    {
        [Key]
        public int KanbanListId { get; set; }
        public string Title { get; set; } = CDatabase.KanBanDefaultName;
        public int ListOrder { get; set; }  
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }  
        public List<TaskKanbanList> TaskKanbanLists { get; set; }
        public List<Event> Events { get; set; }
    }
}
