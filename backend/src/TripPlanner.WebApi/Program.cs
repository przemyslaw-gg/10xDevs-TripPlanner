using TripPlanner.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigurePipeline();

app.Run();

// Make the implicit Program class public for integration tests
public partial class Program { }
