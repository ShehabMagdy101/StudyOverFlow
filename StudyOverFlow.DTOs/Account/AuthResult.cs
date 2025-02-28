namespace StudyOverFlow.DTOs.Account
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string? jtoken {  get; set; }   
        public string[] ErrorList { get; set; } = [];
        public string? massage {  get; set; }
    }
}
