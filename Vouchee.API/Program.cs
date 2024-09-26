using Google.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vouchee.API.AppStarts;
using Vouchee.Business.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDependencyInjection();

IConfiguration configuration = builder.Configuration;
builder.Services.AddSwaggerServices(configuration);
builder.Services.AddFirebaseAuthentication(configuration);
builder.Services.AddSettingObjects(configuration);

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();
IWebHostEnvironment env = app.Environment;

// Configure the HTTP request pipeline.
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vouchee.API v1"));
}
else if (env.IsProduction())
{
    // In production, you might want to use a more user-friendly exception handling page.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Add HSTS in production for security
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Use generic type for middleware

app.UseCors(options =>
{
    options.AllowAnyOrigin() // Adjust CORS policy as needed
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
