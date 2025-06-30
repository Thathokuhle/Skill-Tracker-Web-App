using BusinessLogic.AppSettings;
using BusinessLogic.LogicInterfaces;
using BusinessLogic.Login;
using BusinessLogic.Managers.AccountManagers;
using BusinessLogic.Services;
using BusinessLogic.URLEncryptionBusiness;
using DataLogic;
using DataLogic.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.ImplementedRepositories.AccountRepositories;
using Repository.RepositoryInterfaces;
using SkillTrackerApp.DataLogic.User;
using SkillTrackerApp.Middleware;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//------------------------------------------------------------
// App Configuration and Environment
//------------------------------------------------------------
var Configuration = builder.Configuration;


//------------------------------------------------------------
// CORS Configuration
//------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(AppSettings.BaseUrl, "https://localhost:7231")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("ReportGetPolicy", policy =>
    {
        policy.WithOrigins(AppSettings.BaseUrl, "https://localhost:7231")
              .WithMethods("GET")
              .AllowAnyHeader();
    });

    options.AddPolicy("AllowAllGet", policy =>
    {
        policy.AllowAnyOrigin()
              .WithMethods("GET")
              .AllowAnyHeader();
    });

    options.AddPolicy("TelerikPolicy", policy =>
    {
        policy.WithOrigins(AppSettings.BaseUrl, "https://localhost:7231")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//Configure Identity Options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

//------------------------------------------------------------
// Identity, Authentication & Authorization
//------------------------------------------------------------
builder.Services.AddDbContext<DefaultContext>(options =>
    options.UseSqlServer(AppSettings.DbConnectionString));
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(AppSettings.DbConnectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = AppSettings.GoogleClientId;
        options.ClientSecret = AppSettings.GoogleSecret;
        options.CorrelationCookie.SameSite = SameSiteMode.None;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = AppSettings.BaseUrl,
            ValidAudience = AppSettings.BaseUrl,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.GetJWTTokenKey()))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BothAuthSchemes", policy =>
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, IdentityConstants.ApplicationScheme)
              .RequireAuthenticatedUser());
});

//------------------------------------------------------------
// Dependency Injection Setup
//------------------------------------------------------------
SetupInjectionDependency(builder);

//------------------------------------------------------------
// Register Configuration
//------------------------------------------------------------
builder.Services.AddSingleton<IConfiguration>(Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
//------------------------------------------------------------
// Middleware
//------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseAuthentication();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
//------------------------------------------------------------
// Routing
//------------------------------------------------------------
app.MapDefaultControllerRoute();
app.MapControllers();
app.MapRazorPages();

app.Run();


//------------------------------------------------------------
// Method: Dependency Injection Setup
//------------------------------------------------------------
void SetupInjectionDependency(WebApplicationBuilder builder)
{
    builder.Services.AddTransient<iAccountRepository, AccountRepository>();
    builder.Services.AddTransient<iAccountManager, AccountManager>();

    builder.Services.AddTransient<IUserBusiness, UserBusiness>();

    builder.Services.AddScoped<UserService>();

    builder.Services.AddSingleton<IEncryptDecrypt>(provider => new EncryptDecrypt(AppSettings.MVCSecret));
}