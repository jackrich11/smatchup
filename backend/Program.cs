
using System.Text;
using backend.Settings;
using backend.Factories;
using backend.Factories.MongoFactories;
using Microsoft.Extensions.Options;
using backend.Setting;
using backend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UrlSettings>(
    builder.Configuration.GetSection("Urls"));

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.Configure<DiscordSettings>(
    builder.Configuration.GetSection("DiscordSettings"));

builder.Services.AddSingleton<IFactory, MongoFactory>();

builder.Services.AddHttpClient("discord", (service, client) => {
    var settings = service.GetRequiredService<IOptions<DiscordSettings>>().Value;

    var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.ClientId + ":" + settings.ClientSecret));

    client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
    client.BaseAddress = new Uri(settings.BaseUrl);
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            if(builder.Configuration["AllowedOrigins"] is not null)
                policy.WithOrigins(builder.Configuration["AllowedOrigins"]!);
            policy.AllowCredentials();
            policy.WithHeaders("Content-Type", "x-requested-with", "x-signalr-user-agent");
            policy.WithMethods("GET", "POST", "PUT", "DELETE");
        });
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MatchupHub>("/message");

app.Run();
