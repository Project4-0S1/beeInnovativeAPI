using System.Text.Json.Serialization;
using beeInnovative.DAL;
using beeInnovative.DAL.Data;
using beeInnovative.DAL.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString
    = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BeeInnovativeContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

// Add CORS policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//        builder => builder.WithOrigins("http://localhost:4200", "https://beeinnovative.netlify.app") // Allow your Angular app
//                          .AllowAnyHeader()
//                          .AllowAnyMethod());
//});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BeeInnovative",
        Version = "v1"
    });
});

var app = builder.Build();

// Use CORS
app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.WithOrigins("http://localhost:4200", "https://localhost:4200", "https://beeinnovative.netlify.app");
});

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var myContext = scope.ServiceProvider.GetRequiredService<BeeInnovativeContext>();
    DBInitializer.Initialize(myContext);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();