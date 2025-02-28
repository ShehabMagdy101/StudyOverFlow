namespace StudyOverFlow.DTOs.Manage;

public class SubjectDto
{
    public int? SubjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsChecked { get; set; }
    public string? UserId { get; set; }

    public List<TaskDto>? Tasks { get; set; }
    public List<NoteDto>? Notes { get; set; }
    public List<MaterialObjDto>? MaterialObjs { get; set; }
}