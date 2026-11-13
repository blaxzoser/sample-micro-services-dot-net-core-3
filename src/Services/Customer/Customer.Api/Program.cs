using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Practice mode: drop and re-create the database on every start, so each run begins
// with fresh seed data (the exercises assume this). Real services use migrations instead.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

app.MapControllers();

app.Run();

public partial class Program { }   // lets tests use WebApplicationFactory<Program>
