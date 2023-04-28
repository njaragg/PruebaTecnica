using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using tarea.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// agregamos el contexto de la base de datos, con el nombre de esa y el nombre del conectionString
builder.Services.AddDbContext<PruebaTecnicaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PruebaTecnicaSQL"));

});

// Evitamos el problema con el cycle json (es similar al @JSONIgnore en java)
builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//services cors
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app cors
app.UseCors("corsapp");
app.UseHttpsRedirection();
app.UseAuthorization();
//app.UseCors(prodCorsPolicy);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
