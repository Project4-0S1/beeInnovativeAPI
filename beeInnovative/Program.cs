using beeInnovative.DAL;
using beeInnovative.DAL.Data;
using beeInnovative.DAL.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString
    = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<BeeInnovativeContext>(options =>
    options.UseSqlServer(connectionString));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200", "https://beeinnovative.netlify.app") // Allow your Angular app
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BeeInnovative", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowSpecificOrigin");

using (var scope = app.Services.CreateScope())
{
    var myContext = scope.ServiceProvider.GetRequiredService<BeeInnovativeContext>();
    DBInitializer.Initialize(myContext);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
