namespace StudyOverFlow.API.Model
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public string Name { get; set; } = null!;
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public string? type { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }
        public List<TaskTag> TaskTags { get; set; }
        public List<Event> Events { get; set; }


    }
}
