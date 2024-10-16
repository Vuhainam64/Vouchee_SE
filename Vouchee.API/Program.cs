using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Vouchee.API.AppStarts;
using Vouchee.Business.Middelwares;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
builder.Services.AddDependencyInjection(builder.Configuration);
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

// Define CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("https://www.vouchee.shop", "https://vouchee.shop", "http://localhost:*")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials(); // If you need to allow cookies or credentials
                      });
});

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Use generic type for middleware

// Use CORS with the defined policy
app.UseCors("MyAllowSpecificOrigins");

app.UseAuthentication();

app.UseMiddleware<AuthorizeMiddleware>(); // Place before UseAuthorization

app.UseAuthorization();

app.MapControllers();

app.Run();
