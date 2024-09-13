using AlphabetUpdateServer;
using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using AlphabetUpdateServer.Models.ChecksumStorages;
using AlphabetUpdateServer.Services.ChecksumStorages;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("FISH_");

// Application
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Postgres") 
               ?? throw new InvalidOperationException("ConnectionString Postgres was empty"))
    .EnableSensitiveDataLogging(true));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddHttpClient();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache")
        ?? throw new InvalidOperationException("ConnectionString RedisCache was empty");
    options.InstanceName = "FishServer";
});
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

// Authentication / Authorization
var jwtOptions = builder.Configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>() ?? 
    throw new InvalidOperationException("Cannot find Jwt configuration.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = JwtAuthService.CreateSecurityKey(jwtOptions.Key),
        ValidAudience = jwtOptions.Audience,
        ValidIssuer = jwtOptions.Issuer,
    };
});
builder.Services.AddDefaultIdentity<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "FiSH API",
        Description = "REST API for FiSH server",
    });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Services
builder.Services.AddTransient<ChecksumStorageBucketService>();

builder.Services.AddTransient<ChecksumStorageService>();
builder.Services.AddTransient<ChecksumStorageFileCacheFactory>();
builder.Services.AddTransient<IChecksumStorageProvider, ObjectChecksumStorageProvider>();
builder.Services.AddTransient<IChecksumStorageProvider, RFilesChecksumStorageProvider>();
builder.Services.AddTransient<ObjectChecksumStorageService>();
builder.Services.AddTransient<RFilesChecksumStorageService>();

builder.Services.AddSingleton<JwtAuthService>();

// Configurations
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = false;

    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/healthz");
app.UseExceptionHandler("/api/error");
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
