using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudyOverFlow.API.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using StudyOverFlow.API.Model;
using StudyOverFlow.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using StudyOverFlow.DTOs.Account;
using StudyOverFlow.DTOs.Consts;

namespace StudyOverFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AccountController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly JwtService _jwtService;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailService;
        private IConfiguration _config;
        private readonly IMapper _mapper;
        public AccountController(JwtService jwtService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,

            IEmailSender emailService,
            IConfiguration config,
            IMapper mapper,
            IEmailBodyBuilder emailBodyBuilder,
            IWebHostEnvironment webHostEnvironment)
        {
            _jwtService = jwtService;
            _signManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
            _mapper = mapper;
            _emailBodyBuilder = emailBodyBuilder;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            if(User is null)
                return Unauthorized(null);

            if (!User.Identity.IsAuthenticated)
                return Unauthorized(null);






            var m = User.FindFirst(ClaimTypes.Email);
            if (m is null)
                return Unauthorized(null);
            var user = await _userManager.FindByEmailAsync(m.Value);
            
            if (user is null)
            {
                return Unauthorized(null);
            }
            //  var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            // var userRole = await _userManager.GetRolesAsync(user);
            return await CreateApplicationUserDto(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid Email or Password !");
           // if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email !");

            var result = await _signManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid Email or Password !");
            var userRole = await _userManager.GetRolesAsync(user);
            //await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity( new Claim[] { 
            //new Claim (ClaimTypes.NameIdentifier , user.Id),
            //new Claim (ClaimTypes.Email, user.Email),
            
            //})));
            return await CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (await CheckEmailExistAsync(model.Email)) return BadRequest("Email is already exist, please try with another email !");
            var userToAdd = _mapper.Map<ApplicationUser>(model);
            //    new User
            //{
            //    FullName = model.FullName,
            //    UserName = model.Email.ToLower(),
            //    Email = model.Email.ToLower(),
            //};
            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(userToAdd, UserRole.User);
            return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, please check your email to confirm !" }));

            //try
            //{
            //    if (await SendConfirmEmailAsync(userToAdd))
            //    {
            //        return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, please check your email to confirm !" }));
            //    }
            //    return BadRequest("Failed to create account, please try again or contact with admin !");
            //}
            //catch (Exception)
            //{
            //    return BadRequest("Failed to create account, please try again or contact with admin !");
            //}
        }

        /*app.UseCors(opt =>
             {
                 opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]);
             });*/
        [Authorize]

        [HttpGet("Info")]
        public async Task<IActionResult> Info()
        {
            return Ok();
        }
        [AllowAnonymous]
        
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]ConfirmEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email has not been registered yet !");

            if (user != null)
            {
                if (user.EmailConfirmed == true)
                {
                    return BadRequest("Your email has been confirmed before, please login to your account !");
                }
                else
                {
                    try
                    {
                        var decodeTokenByte = WebEncoders.Base64UrlDecode(model.Token);
                        var decodeToken = Encoding.UTF8.GetString(decodeTokenByte);

                        var result = await _userManager.ConfirmEmailAsync(user, decodeToken);
                        if (result.Succeeded)
                        {
                            //user.IsActive = true;
                            return Ok(new JsonResult(new { title = "Email Confirmed", message = "Your email has been confirmed, you can login now" }));
                        }
                        return BadRequest("Invalid token, please check your email and try again");
                    }
                    catch (Exception)
                    {
                        return BadRequest("Invalid token, please check your email and try again");
                    }
                }
            }
            return BadRequest("Invalid token, please check your email and try again");
        }

        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email !");
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized("This email has not been registered yet !");

            if (user.EmailConfirmed == true) return BadRequest("Your email has been confirmed before, plese login to your account");

            try
            {
                if (await SendConfirmEmailAsync(user))
                {
                    return Ok(new JsonResult(new { title = "Confirmation link sent", message = "Please confirm your email address !" }));

                }
                return BadRequest("Error, please contact admin.");

            }
            catch (Exception)
            {
                return BadRequest("Error, please contact admin.");
            }

        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUserNameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email is invalid !");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("Error, User not found !");
            if (user.EmailConfirmed == false) return BadRequest("Please confirm your email first");

            try
            {
                if (await SendForgotPasswordOrUserName(user))
                {
                    return Ok(new JsonResult(new { title = "Success", message = "Please check your email to reset" }));
                }
                return BadRequest("Error, please contact admin.");
            }
            catch (Exception)
            {
                return BadRequest("Error, please contact admin.");

            }

        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("User not found !");
            if (user.EmailConfirmed == false) return BadRequest("Please confirm your email first !");
            try
            {
                var decodeTokenByte = WebEncoders.Base64UrlDecode(model.Token);
                var decodeToken = Encoding.UTF8.GetString(decodeTokenByte);

                var result = await _userManager.ResetPasswordAsync(user, decodeToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Reset password success", message = "Your password has been reset" }));
                }
                return BadRequest("Invalid token, please check your email and try again");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token, please check your email and try again");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] object empty)
        {
            if (empty is not null)
            {
                await _signManager.SignOutAsync();
                
                return Ok();
            }

            return Unauthorized();
        }


        #region Private Helper Method
        private async Task<UserDto> CreateApplicationUserDto(ApplicationUser user)
        {
            return new UserDto
            {
                Email = user.Email,
                JWT = await _jwtService.CreateJwt(user),
            };
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        private async Task<bool> SendConfirmEmailAsync(ApplicationUser userToAdd)
        {
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(userToAdd);
            //token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));


            //var body = $"<p>Hello: {userToAdd.FirstName + " "+ userToAdd.LastName}</p> " +
            //    $"<p>Please click <a href =\"{url}\">here</a> to confirm your email</p>" +
            //    "<p>Thank you</p>" +
            //    $"<br>{_config["Email:ApplicationName"]}";

            //var emaiSend = new EmailSendDto(userToAdd.Email, "CONFIRM YOUR EMAIL", body);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userToAdd);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var m =_webHostEnvironment.WebRootPath;
            string callbackUrl = Url.Action(nameof(ConfirmEmail),  "Account", new {token = code ,email = userToAdd.Email},Request.Scheme);
           // var url = $"https://localhost:7157/api/account/confirm-email?token={code}&email={userToAdd.Email}";
            //var callbackUrl2 = new Uri(Url.Link("confirm-email", new { email = userToAdd.Email , token = code }));


            string body = _emailBodyBuilder.EmailBody
              ("https://res.cloudinary.com/macto/image/upload/v1693393451/Ok-rafiki_fw7rpd.png"
              , $"Hey {userToAdd.FirstName + " " + userToAdd.LastName}, thanks for joining us"
              , "Please Confirm your email"
              , $"{HtmlEncoder.Default.Encode(callbackUrl)}"
            , "Active account");
            try
            {
                await _emailService.SendEmailAsync(userToAdd.Email, "Confirm your email", body);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;



            //await _emailService.SendEmail(emaiSend);
        }


        private async Task<bool> SendForgotPasswordOrUserName(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_config["JWT:ClientUrl"]}/{_config["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";
            var body = $"<p>Hello: {user.FullName()}</p> " +
                $"<p>Please click <a href =\"{url}\">here</a> to reset your password</p>" +
                "<p>Thank you</p>" +
                $"<br>{_config["Email:ApplicationName"]}";

            //var emaiSend = new EmailSendDto(user.Email, "RESET PASSWORD", body);
            try
            {
                await _emailService.SendEmailAsync(user.Email, "Confirm your email", body);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
