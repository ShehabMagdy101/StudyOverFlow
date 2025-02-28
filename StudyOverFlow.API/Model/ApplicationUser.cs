using Microsoft.AspNetCore.Identity;

namespace StudyOverFlow.API.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageURL {get;set;}
        public List<Subject> Subjects { get; set; }
        public List<Task> Tasks { get; set; }
        public List<KanbanList> KanbanLists { get; set; }
        public Calendar Calendar { get; set; }
        public List<langchain_pg_embedding> Embeddings { get; set; }   
        public List<Note> Notes { get; set; }   
        

        public string FullName()
        {
            return FirstName+ " "+ LastName;
        }
    }
}
