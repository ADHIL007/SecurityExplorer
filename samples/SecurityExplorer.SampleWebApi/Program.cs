using Microsoft.EntityFrameworkCore;
using SecurityExplorer.SampleWebApi.Data;
using SecurityExplorer.SampleWebApi.Extensions;
using SecurityExplorer.SampleWebApi.Middleware;
using Serilog;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddAppServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerJwtSecurity();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDepositories();
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();
app.Run();

