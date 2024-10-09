

using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Vouchee.API.AppStarts;
using Vouchee.Business.Middelwares;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDependencyInjection();
builder.Services.AddDistributedMemoryCache();

IConfiguration configuration = builder.Configuration;

builder.Services.AddSwaggerServices(configuration);
builder.Services.AddFirebaseAuthentication(configuration);
builder.Services.AddSettingObjects(configuration);
builder.Services.AddJWTServices(configuration);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp-keys\"))
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

IWebHostEnvironment env = app.Environment;

// Configure the HTTP request pipeline.
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;  // Make Swagger UI the root ("/")
    });
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
