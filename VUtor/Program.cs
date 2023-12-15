using DataAccessLibrary;
using DataAccessLibrary.ConnectionRepo;
using DataAccessLibrary.Data;
using DataAccessLibrary.FileRepo;
using DataAccessLibrary.FolderRepo;
using DataAccessLibrary.GenericRepo;
using DataAccessLibrary.Models;
using DataAccessLibrary.ProfileRepo;
using DataAccessLibrary.RatingRepo;
using DataAccessLibrary.Search;
using DataAccessLibrary.WebSearch;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("AzureSql") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString), ServiceLifetime.Transient);
var storageConnectionString = builder.Configuration.GetConnectionString("Storage");
builder.Services.AddAzureClients(options =>
{
    options.AddBlobServiceClient(storageConnectionString);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<ProfileEntity>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<IConnectionsRepository, ConnectionsRepository>();
builder.Services.AddScoped<ISearch, Search>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<RatingRepository>();
builder.Services.AddScoped<StudyGroupRepository>();
builder.Services.AddControllers();

var app = builder.Build();
// Apply migrations at runtime
using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //context.Database.Migrate();
};
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
