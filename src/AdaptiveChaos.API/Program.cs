using AdaptiveChaos.API.Extension;
using AdaptiveChaos.Infrastructure.Implementation;
using AdaptiveChaos.Infrastructure.Interfaces;
using AdaptiveChaos.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
builder.Services.AddSingleton<IChaosPolicyFactory, ChaosPolicyFactory>();
builder.Services.AddScoped<ChuckNorrisService>();
builder.Services.AddChaosPolicy("ChuckNorrisService", logger);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ChaosDelegatingHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Wrap every policy in the policy registry in Simmy chaos injectors.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
