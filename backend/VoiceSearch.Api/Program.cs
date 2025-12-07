using Microsoft.EntityFrameworkCore;
using VoiceSearch.Api.Data;
using VoiceSearch.Api.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
config.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB - SQLite for starter
builder.Services.AddDbContext<ApplicationDbContext>(opt => 
    opt.UseSqlite(config.GetConnectionString("DefaultConnection") ?? "Data Source=voice_search.db"));

builder.Services.Configure<FormOptions>(options => {
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmbeddingService, OpenAiEmbeddingService>();
builder.Services.AddScoped<ISearchService, QdrantSearchService>();
builder.Services.AddScoped<ISttService, AzureSttService>();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.Run();
