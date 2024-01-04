using Infrastructure.DB;
using Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ItemMessagingConfig>(builder.Configuration.GetSection("AzureSB"));

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ItemMessaging>();

var host = builder.Build();
host.Run();
