using AlphabetUpdateServer;
using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Models.ChecksumStorages;
using AlphabetUpdateServer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

// Models
builder.Services.AddTransient<IBucketFactory, BucketFactory>();
builder.Services.AddTransient<IFileChecksumStorageFactory, FileChecksumStorageManager>();

// Repositories
builder.Services.AddTransient<IBucketRepository, BucketDbRepository>();
builder.Services.AddTransient<IBucketFileRepository, BucketFileDbRepository>();
builder.Services.AddTransient<ICachedFileChecksumRepository, CachedFileChecksumDbRepository>();
builder.Services.AddTransient<IFileChecksumStorageRepository, FileChecksumStorageDbRepository>();

// Application
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt
    .UseLazyLoadingProxies()
    .UseSqlite("Data Source=local.db"));
builder.Services.AddDefaultIdentity<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
