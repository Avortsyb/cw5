using Auth.Application.Services;
using Auth.Data;
using Auth.Data.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Contracts.Utils;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connection =
    builder.Configuration.GetConnectionString("AuthConnection") ?? string.Empty;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });

    x.AddRequestClient<AuthRequest>(); 
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();