using AutoMapper;
using StudyOverFlow.API.Model;
using StudyOverFlow.DTOs.Account;
using StudyOverFlow.DTOs.Manage;

namespace StudyOverFlow.API.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {

            
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(Dest => Dest.Email, src => src.MapFrom(c => c.Email))
                //.ForMember(Dest => Dest.FirstName, src => src.MapFrom(c => c.FirstName))
                //.ForMember(Dest => Dest.LastName, src => src.MapFrom(c => c.LastName))
                .ForMember(Dest => Dest.UserName, src => src.MapFrom(c => c.UserName))
                .ForMember(Dest => Dest.PhoneNumber, src => src.MapFrom(c => c.Phone))
                ;

            CreateMap<Model.Task, TaskDto>()

                .ReverseMap();

            CreateMap<Tag, TagDto>()
                .ReverseMap();
            CreateMap<Event, EventDto>()
    .ForMember(Dest => Dest.DurationSpan, src => src.MapFrom(c => new WriteObject { Hours= c.DurationSpan.Hours, Minutes= c.DurationSpan.Minutes}   ))
    ;



            CreateMap<EventDto, Event>()
                .ForMember(Dest => Dest.DurationSpan, src => src.MapFrom(c => new TimeSpan(c.DurationSpan.Hours, c.DurationSpan.Minutes, 0)))
                .ForMember(dest =>dest.SubjectId , src=>src.MapFrom(c=>c.SubjectId))
                .ForMember(dest =>dest.KanbanListId , src=>src.MapFrom(c=>c.KanbanListId))
                .ForMember(dest =>dest.TagId , src=>src.MapFrom(c=>c.TagId))
                ;
            CreateMap<Model.Note, NoteDto>()
                .ForMember(dest => dest.Text, src => src.MapFrom(c => c.text))
                .ReverseMap();
            //CreateMap<Model.Subject, SubjectDto>().ReverseMap();   
            CreateMap<Subject, SubjectDto>()
           .AfterMap((src, dest, context) =>
           {
               if (src.Tasks != null)
               {
                   dest.Tasks = src.Tasks.Select(task => context.Mapper.Map<TaskDto>(task)).ToList();
               }

               if (src.Notes != null)
               {
                   dest.Notes = src.Notes.Select(note =>
                   {
                       var noteDto = context.Mapper.Map<NoteDto>(note);
                       noteDto.SubjectId = src.SubjectId;  // Ensure SubjectId is set
                       return noteDto;
                   }).ToList();
               }

               if (src.MaterialObjs != null)
               {
                   dest.MaterialObjs = src.MaterialObjs.Select(obj => context.Mapper.Map<MaterialObjDto>(obj)).ToList();
               }
           }).ReverseMap();










        }
    }
}
