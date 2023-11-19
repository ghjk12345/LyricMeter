using Microsoft.OpenApi.Models;

var API_NAME = "Lyric API";

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:4200")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// swagger
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = API_NAME, Version = "v1" });
    });
}

var app = builder.Build();

app.UseCors("AllowOrigin");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", API_NAME);
    });
}

app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
