using BookingService;
using BookingService.Data;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();




builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiGatewayPolicy", builder =>
    {
        builder.WithOrigins("http://nginx", "https://nginx") // NGINX service in Docker
            .AllowAnyHeader()
            .AllowAnyMethod();
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


builder.WebHost.ConfigureKestrel((context, options) =>
{
    var env = context.HostingEnvironment;

    if (env.IsDevelopment())
    {
        options.ListenAnyIP(8082);
        options.ListenAnyIP(8445, listenOptions =>
        {
            listenOptions.UseHttps(); // Development uses .NET dev certs
        });
    }
    else
    {
        var certPath = context.Configuration["CERTPATH"];
        var certPassword = context.Configuration["PASSPATH"];

        if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(certPassword))
        {
            throw new InvalidOperationException(
                $"Certificate path or password is not configured. Path: {certPath}, Password: {(string.IsNullOrEmpty(certPassword) ? "Not Provided" : "Provided")}");
        }

        options.ListenAnyIP(8082); // HTTP port
        options.ListenAnyIP(8445, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    }
});



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


app.UseAuthentication();
app.UseAuthorization();

app.UseCors("ApiGatewayPolicy");
app.MapControllers();



app.Run();

