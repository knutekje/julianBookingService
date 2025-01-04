using BookingService;
using BookingService.Data;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8082); // Explicitly bind to port 8082
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://192.168.100.109:8081",
                    "http://localhost:8081",
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "http://localhost:5174",
                    "http://localhost:5172")
                .AllowAnyHeader()
                .AllowAnyMethod()
                ;
        });
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add DbContext
    
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<IBookingService, BookingService.Services.BookingService>();
builder.Services.AddHttpClient<RoomServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://room-service:8081/");
});
builder.Services.AddScoped<IRoomServiceClient, RoomServiceClient>();



// Add Controllers
builder.Services.AddControllers();
var app = builder.Build();

if(!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())

    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
        dbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.MapControllers();
app.UseHttpsRedirection();



app.Run();

