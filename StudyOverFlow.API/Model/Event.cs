namespace StudyOverFlow.API.Model
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }  
        public string Title { get; set; }   
        public string? Description { get;set; }
        public int? TotalCount { get; set; }
        public int? CurrentCount { get; set; }
       
        public DateTime Date { get; set; } 
        public TimeSpan DurationSpan { get; set; }
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public int? KanbanListId { get; set; }
        public KanbanList? KanbanList { get; set; }

        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; }  

        public int ColorId { get; set; }    
        public Color Color { get; set; }   
        public List<Task>? tasks { get; set; }   

    }
}
public enum Day
{
    Saturday,
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday

}