using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using StudyOverFlow.DTOs;
using StudyOverFlow.DTOs.Account;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudyOverFlow.Client.Identity;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
{
    private HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string LocalStorageKey = "auth";
    private bool _authenticated;
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
    {
        this._httpClient = httpClientFactory.CreateClient("Auth");
        _localStorage = localStorage;
    }




    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {


        _authenticated = false;

        var user = _unauthenticated;

        try
        {
            string token = await _localStorage.GetItemAsStringAsync(LocalStorageKey);

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(user);




            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Content = null,
                RequestUri = new Uri("api/account/info", uriKind: UriKind.Relative)

            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var userResponse = await _httpClient.SendAsync(requestMessage);

            if (userResponse.StatusCode == HttpStatusCode.OK)
            {
                var (name, email) = GetClaims(token);
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                    return new AuthenticationState(user);

                var claims = SetClaimPrincipal(name, email);

                if (claims is null)
                    return new AuthenticationState(user);
                else
                    return new AuthenticationState(claims);
            }

           
        }
        catch (Exception ex)
        {
            //Logging
        }

        return new AuthenticationState(user);
    }

    public async Task UpdateAuthenticationState(string JwtToken)
    {
        if (!string.IsNullOrEmpty(JwtToken))
        {
            var (name, email) = GetClaims(JwtToken);
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                return;

            var claims = SetClaimPrincipal(name, email);
            if (claims is null)
                return;
            await _localStorage.SetItemAsStringAsync(LocalStorageKey, JwtToken);
        }
        else
        {
            await _localStorage.RemoveItemAsync(LocalStorageKey);
        }
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    private ClaimsPrincipal SetClaimPrincipal(string name, string email)
    {
        if (name is null || email is null)
            return new ClaimsPrincipal();
        return new ClaimsPrincipal(new ClaimsIdentity(
            [ new(ClaimTypes.Name, name),
              new(ClaimTypes.Email, email)], "JwtAuth"


            ));
    }

    private static (string, string) GetClaims(string jwttoken)
    {
        if (string.IsNullOrEmpty(jwttoken)) return (null!, null!);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwttoken);
        var m = token.Claims.FirstOrDefault(o => o.Type == "name");
        var name = m.Value;
        var email = token.Claims.FirstOrDefault(o => o.Type == "email").Value;
        return (name, email);
    }
    //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //{
    //    _authenticated = false;


    //    var user = _unauthenticated;

    //    try
    //    {

    //        var requestMessage = new HttpRequestMessage
    //        {
    //            Method = HttpMethod.Get,
    //            Content =null,
    //            RequestUri = new Uri( "api/account/refresh-user-token",uriKind: UriKind.Relative)

    //        };

    //        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tooken);



    //        //var userResponse = await _httpClient.GetAsync("api/account/refresh-user-token");
    //        var userResponse = await _httpClient.SendAsync(requestMessage);

    //        //      var userResponsem = await _httpClientm.GetAsync("manage/info");

    //        userResponse.EnsureSuccessStatusCode();

    //        var userJson = await userResponse.Content.ReadAsStringAsync();
    //        var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, _jsonSerializerOptions);

    //        if (userInfo is not null)
    //        {
    //            _authenticated = true;

    //            var claims = new List<Claim>
    //            {
    //                new(ClaimTypes.Name, userInfo.Email),
    //                new(ClaimTypes.Email, userInfo.Email),
    //            };

    //            var claimsIdentity = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
    //            user = new ClaimsPrincipal(claimsIdentity);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        //Logging
    //        return new AuthenticationState(user);
    //    }

    //    return new AuthenticationState(user);
    //}

    public async Task<AuthResult> LoginAsync(LoginDto credentials)
    {
        try
        {
            var result = await _httpClient.PostAsJsonAsync("api/Account/login", new
            {
                credentials.Email,
                credentials.Password,
            });
            
            
            if (result.IsSuccessStatusCode)
            {

                var user = await result.Content.ReadFromJsonAsync<UserDto>();
                // await UpdateAuthenticationState(user.JWT);
                //await _jSRuntime.InvokeVoidAsync("localStorage.setItem", "aspNettokenKey", user.JWT);
                //  await localStorageService.SetItem("aspNettokenKey",user.JWT);

                // Token = user.JWT;
                await _localStorage.SetItemAsStringAsync(LocalStorageKey, user.JWT);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthResult { Succeeded = true };
            }
        }
        catch (Exception ex)
        {
            //Logging
        }

        return new AuthResult
        {
            Succeeded = false,
            ErrorList = ["Invalid email or password"]
        };
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto registerDto)
    {
        string[] defaultErrors = ["An unknown error prevented registration"];

        try
        {
            var result = await _httpClient.PostAsJsonAsync("api/Account/register", registerDto);

            if (result.IsSuccessStatusCode)
            {
                return new AuthResult { Succeeded = true ,massage = await result.Content.ReadAsStringAsync() };
            }

            var details = await result.Content.ReadAsStringAsync();
            var problemDetails = JsonDocument.Parse(details);

            var errors = new List<string>();
            var errorList = problemDetails.RootElement.GetProperty("errors");

            foreach (var error in errorList.EnumerateObject())
            {
                if (error.Value.ValueKind == JsonValueKind.String)
                {
                    errors.Add(error.Value.GetString()!);
                }
                else if (error.Value.ValueKind == JsonValueKind.Array)
                {
                    var allErrors = error.Value
                        .EnumerateArray()
                        .Select(e => e.GetString() ?? string.Empty)
                        .Where(e => !string.IsNullOrEmpty(e));

                    errors.AddRange(allErrors);
                }
            }

            return new AuthResult
            {
                Succeeded = false,
                ErrorList = [.. errors]
            };
        }
        catch
        {
            //Logging
        }

        return new AuthResult
        {
            Succeeded = false,
            ErrorList = defaultErrors
        };
    }

    public async Task LogoutAsync()
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("api/account/logout", emptyContent);
        await _localStorage.RemoveItemAsync(LocalStorageKey);
        
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}