using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortener.Data;
using UrlShortener.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]));
builder.Services.AddScoped<UrlRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

