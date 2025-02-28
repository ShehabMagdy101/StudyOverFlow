namespace StudyOverFlow.API.Model
{
    public class Calendar
    {
        [Key]
        public int CalendarId {  get; set; }    
        public string Duration { get; set; }    
        public TimeOnly start { get; set; } 
        public TimeOnly End { get; set; }
        public string UserId { get; set; }    
        public ApplicationUser User { get; set; }   
        public List<Event> Events { get; set; }  = new List<Event> ();

    }
}
