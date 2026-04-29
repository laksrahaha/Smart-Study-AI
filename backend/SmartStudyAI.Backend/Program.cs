var builder = WebApplication.CreateBuilder(args);

// ADD THIS (enables controllers)
builder.Services.AddControllers();

var app = builder.Build();

// ADD THIS (maps your controllers like CourseController)
app.MapControllers();

app.Run();
