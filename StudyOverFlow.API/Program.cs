using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

using Microsoft.IdentityModel.Tokens;
using StudyOverFlow.API.Data;
using StudyOverFlow.API.MiddleWare;
using StudyOverFlow.API.Model;
using StudyOverFlow.API.Profile;
using StudyOverFlow.API.Services;
using StudyOverFlow.API.Services.Caching;
using StudyOverFlow.API.settings;
using StudyOverFlow.Client.Identity;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<JwtService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();
builder.Services.AddScoped<IAutomationService,AutomationService>();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();
//builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsFactory>();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<IRedisCacheService, RedisCachService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddEndpointsApiExplorer();


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
if(string.IsNullOrEmpty(connectionString))
    connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddHangfire(config =>
    config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(connectionString));

builder.Services.AddHangfireServer();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseVector()));
//builder.Services.AddStackExchangeRedisCache( options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Connection string 'redis' not found.");
//    options.InstanceName = "StudyOverFlow";

//});
builder.Services.AddMemoryCache();
//builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
//builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

//builder.Services.AddHttpClient("Auth", options => options.BaseAddress = new Uri("https://localhost:7157/"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
    };
});
builder.Services.AddCors(option => option.AddDefaultPolicy(
                     builder =>builder
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .WithOrigins("https://localhost:7241")
                         .AllowCredentials()
                     
                     ));
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();

app.UseAuthorization();
app.UseHangfireServer();
app.UseHangfireDashboard("/queue");

//BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget"));

////Delayed
//BackgroundJob.Schedule(() => Console.WriteLine("Delayed"), TimeSpan.FromDays(1));

////Recurring
//RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job"), Cron.Minutely);

////Continuation
//var id = BackgroundJob.Enqueue(() => Console.WriteLine("Hello, "));
//BackgroundJob.ContinueWith(id, () => Console.WriteLine("world!"));

app.MapControllers();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
