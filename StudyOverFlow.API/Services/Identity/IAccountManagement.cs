using StudyOverFlow.API.Model;
using StudyOverFlow.DTOs.Account;

namespace StudyOverFlow.Client.Identity
{

    public interface IAccountManagement
    {
        Task<AuthResult> LoginAsync(LoginDto credentials);
        Task<AuthResult> RegisterAsync(RegisterDto registerDto);
        System.Threading.Tasks.Task LogoutAsync();
    }
}
