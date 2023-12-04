using AlphabetUpdateServer;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Models.ChecksumStorages;
using AlphabetUpdateServer.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Models
builder.Services.AddTransient<IBucketFactory, BucketFactory>();
builder.Services.AddTransient<IFileChecksumStorageManager, FileChecksumStorageManager>();

// Repositories
builder.Services.AddTransient<IBucketRepository, BucketDbRepository>();
builder.Services.AddTransient<IBucketFileRepository, BucketFileDbRepository>();
builder.Services.AddTransient<ICachedFileChecksumRepository, CachedFileChecksumDbRepository>();
builder.Services.AddTransient<IFileChecksumStorageRepository, FileChecksumStorageDbRepository>();
builder.Services.AddTransient<IUserRepository, UserDbRepository>();

// Application
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt
    .UseLazyLoadingProxies()
    .UseSqlite("Data Source=local.db"));

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
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
