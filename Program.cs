using HostelManage.Models;
using HostelManage.Data;
using HostelManage.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers to handle API requests.
builder.Services.AddControllers();

// Configure file upload limit to allow larger file uploads (100 MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});

// Configure Swagger/OpenAPI for API documentation.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services, such as EmailService
builder.Services.AddTransient<EmailService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();

// Configure CORS policy for handling different origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()   // Allow requests from any origin
                  .AllowAnyHeader()   // Allow any headers
                  .AllowAnyMethod();  // Allow any HTTP methods
        }
        else
        {
            policy.WithOrigins("http://localhost:3000")  // Corrected space issue in allowed origin
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();  // Allow credentials for specific origins
        }
    });
});

// Add logging configuration to track issues during runtime
builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger
    app.UseSwaggerUI(); // Enable Swagger UI
}

// Enable HTTPS redirection, and use the configured CORS policy
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

// Add Authentication and Authorization middlewares (if you're using JWT or other methods)
app.UseAuthentication();
app.UseAuthorization();

// Enable detailed error messages in development for easier debugging
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Map controllers to API endpoints
app.MapControllers();

// Run the app
app.Run();
