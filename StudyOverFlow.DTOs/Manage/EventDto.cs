namespace StudyOverFlow.DTOs.Manage
{
    public class EventDto
    {

        public int? EventId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? TotalCount { get; set; }
        public int? CurrentCount { get; set; } = 0;
        public DateTime Date { get; set; }

        public WriteObject DurationSpan { get; set; }
        public int? SubjectId { get; set; }

        public int? TagId { get; set; }
        public int? KanbanListId { get; set; }
        public int ColorId { get; set; }
    }
}
