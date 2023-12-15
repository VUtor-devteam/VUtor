using DataAccessLibrary;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.FileRepo;
using Microsoft.AspNetCore.Builder;
using DataAccessLibrary.FolderRepo;
using DataAccessLibrary.Search;
using DataAccessLibrary.WebSearch;
using DataAccessLibrary.ProfileRepo;
using DataAccessLibrary.RatingRepo;
using DataAccessLibrary.GenericRepo;
using Blazored.Modal;
using Azure.Identity;
using Microsoft.AspNetCore.Components.Server.Circuits;
using VUtor.Handlers;
using Serilog;
using Serilog.Events;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.AspNetCore.Diagnostics;
using VUtor.Services.ExceptionTracker;
using VUtor.Services;

var builder = WebApplication.CreateBuilder(args);
var credential = new DefaultAzureCredential();
var token = await credential.GetTokenAsync(
     new Azure.Core.TokenRequestContext(
         new[] { "https://database.windows.net/.default" }));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.ApplicationInsights(new TelemetryConfiguration
    {
        InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"]
    }, TelemetryConverter.Traces)
    .CreateLogger();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("AzureSql") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
  
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
builder.Services.AddSingleton<CircuitHandler, CustomHandler>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<ISearch, Search>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<RatingRepository>();
builder.Services.AddScoped<StudyGroupRepository>();
builder.Services.AddScoped<IExceptionTracker, ExceptionTracker>();
builder.Services.AddControllers();
builder.Services.AddBlazoredModal();

var app = builder.Build();
// Apply migrations at runtime
using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
};
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionTracker = context.RequestServices.GetRequiredService<IExceptionTracker>();
            exceptionTracker.ExceptionOccurred = true;

            context.Response.StatusCode = 500; 
            context.Response.ContentType = "text/html";

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is Exception ex)
            {
                Log.Error(ex, "An unhandled exception has occurred while executing the request.");
            }

            // Redirect to error page
            context.Response.Redirect("/Error");
        });
    });
} // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();


app.UseHttpsRedirection();
app.UseDatabaseErrorPage();
app.UseStaticFiles();
app.UseWebSockets();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
Log.CloseAndFlush();