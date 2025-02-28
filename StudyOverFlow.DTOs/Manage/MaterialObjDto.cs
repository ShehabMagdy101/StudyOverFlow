namespace StudyOverFlow.DTOs.Manage
{
    public class MaterialObjDto
    {

        public int? MaterialId { get; set; }
        public string Name { get; set; } = null!;

        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public double? size { get; set; }
    }
}
