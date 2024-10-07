

using Vouchee.API.AppStarts;
using Vouchee.Business.Middelwares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDependencyInjection();
builder.Services.AddDistributedMemoryCache();

IConfiguration configuration = builder.Configuration;

builder.Services.AddSwaggerServices(configuration);
// builder.Services.AddFirebaseAuthentication(configuration);
builder.Services.AddSettingObjects(configuration);
builder.Services.AddJWTServices(configuration);

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

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Use generic type for middleware

app.UseCors(options =>
{
    options.AllowAnyOrigin() // Adjust CORS policy as needed
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseAuthentication();

app.UseMiddleware<AuthorizeMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
