using Microsoft.EntityFrameworkCore;
using ApiIntegrationService.Data;
using ApiIntegrationService.Services;
using ApiIntegrationService.Security;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the database context
builder.Services.AddDbContext<ApiIntegrationService.Data.AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25)) // Replace with your MySQL version
    )
);
builder.Services.Configure<ChargebeeSettings>(builder.Configuration.GetSection("ChargebeeSettings"));
builder.Services.Configure<FreshdeskSetttings>(builder.Configuration.GetSection("FreshdeskSettings"));
builder.Services.Configure<ActiveCustomersSettings>(
           builder.Configuration.GetSection("ActiveCustomersSettings"));
builder.Services.AddSingleton<Network>();
builder.Services.AddSingleton<GenerateSignature>();
builder.Services.AddSingleton<ApiIntegrationService.Services.DataMatchService>();

// Add services for background worker
builder.Services.AddHostedService<ApiIntegrationService.Services.DataSyncWorker>();
//builder.Services.AddHostedService<ApiIntegrationService.Services.Network>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply CORS globally before other middleware
app.UseCors("AllowFrontend");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Create an endpoint to get Freshdesk data
app.MapGet("/freshdesk", async (ApiIntegrationService.Data.AppDbContext context) =>
{
    return await context.freshdeskcustomerscontacts.ToListAsync();
}).WithName("GetFreshdeskData").WithOpenApi();

// Create an endpoint to get Chargebee data
app.MapGet("/chargebee", async (ApiIntegrationService.Data.AppDbContext context) =>
{
    return await context.ChargebeeCustomers.ToListAsync();
}).WithName("GetChargebeeData").WithOpenApi();
// Create an endpoint to trigger the customer data sync
app.MapPost("/synccustomerdata", async (AppDbContext context) =>
{
    return await context.CustomerContacts.ToListAsync();
}).WithName("SyncCustomerData").WithOpenApi();
;
app.MapGet("/updatedcustomers", async (AppDbContext context) =>
{
    return await context.UpdatedContacts.ToListAsync();
}).WithName("GetUpdatedCustomers").WithOpenApi();


app.Run();
