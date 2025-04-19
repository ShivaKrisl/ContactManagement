using Microsoft.EntityFrameworkCore;
using Entities_Core;
using Service_Contracts;
using Service_Classes;
using Repository_Contracts;
using Repository_Classes;
using Serilog;
using Serilog.AspNetCore;
using ContactManagement.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider service, LoggerConfiguration configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration) // take log setting from appsettings.json
    .ReadFrom.Services(service); // make our services available to Serilog
} );

//builder.Host.ConfigureLogging(provider =>
//{
//    provider.ClearProviders();  // clear default providers
//    provider.AddConsole(); // add console logging
//    provider.AddDebug(); // add debug logging

//});
builder.Services.AddLogging(); // add logging as service

builder.Services.AddHttpLogging(logging => // add HttpLogging Service
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();

builder.Services.AddScoped<IPersonsAdderService, PersonsAdderService>();
builder.Services.AddScoped<IPersonsDeleterService, PersonsDeleteService>();
builder.Services.AddScoped<IPersonsGetterService, PersonsGetterService>();
builder.Services.AddScoped<IPersonsSorterService, PersonsSorterService>();
builder.Services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();

if(builder.Environment.IsEnvironment("Testing") == false)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error"); // add built in error handling middleware 
    app.UseExceptionalHandlingMiddleware(); // add custom middleware to handle exceptions
}

app.UseHttpLogging();
app.UseSerilogRequestLogging(); // add Serilog middleware to log requests

// logs
//app.Logger.LogInformation("Log Info message");
//app.Logger.LogWarning("Log Warning message");
//app.Logger.LogError("Log Error message");
//app.Logger.LogCritical("Log Critical message");
//app.Logger.LogDebug("Log Debug message");
//app.Logger.LogTrace("Log Trace message");


app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } // Make the Program class partial to allow for extension in tests
