using ContentManagement.Application.Services;
using ContentManagement.Data;
using ContentManagement.Data.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connection =
    builder.Configuration.GetConnectionString("CMConnection") ?? string.Empty;
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));
builder.Services.AddControllers();

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddTransient<IArticleService, ArticleService>();
builder.Services.AddScoped<GroupValidationService>();
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ConfigureEndpoints(context);
    });

    x.AddConsumer<GroupDeletedEventConsumer>();
    x.AddRequestClient<CheckGroupRequest>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();