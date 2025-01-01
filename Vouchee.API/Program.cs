using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Vouchee.API.AppStarts;
using Vouchee.Business.Middelwares;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using Hangfire;
using Hangfire.SqlServer;
using Vouchee.Business.Services;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddDependencyInjection(builder.Configuration);
builder.Services.AddDistributedMemoryCache();

builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("PROD"), new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.Zero,
              UseRecommendedIsolationLevel = true,
              UsePageLocksOnDequeue = true,
              DisableGlobalLocks = true
          }));
builder.Services.AddHangfireServer();

IConfiguration configuration = builder.Configuration;

// Add other services
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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("https://phatnq-test.web.app", "https://www.vouchee.shop", "https://vouchee.shop", "http://localhost:3000")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                      });
});

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddSingleton(new SmtpClient
{
    Host = "smtp.gmail.com",
    Port = 587,
    Credentials = new NetworkCredential("advouchee@gmail.com", "advouchee123"),
    EnableSsl = true
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("MyAllowSpecificOrigins");

app.UseAuthentication();

app.UseMiddleware<AuthorizeMiddleware>();

app.UseAuthorization();

// Hangfire Dashboard with no authentication
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllUsersAuthorizationFilter() }
});

RecurringJob.AddOrUpdate<IWithdrawService>
    (
        "Withdraw_AllWallets",
        service => service.CreateWithdrawRequestInAllWalletAsync(),
        Cron.Weekly(DayOfWeek.Tuesday, 10, 0), // Every Tuesday at 10:00 AM
        TimeZoneInfo.Local
    );

RecurringJob.AddOrUpdate<IShopPromotionService>(
    "Update_Expired_Promotions",
    service => service.UpdateExpiredPromotionAsync(),
    Cron.Daily(0, 0), // Every day at 12:00 AM
    TimeZoneInfo.Local
);


app.MapControllers();

app.Run();

// Custom filter to allow all users
public class AllowAllUsersAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(Hangfire.Dashboard.DashboardContext context)
    {
        return true; // Allow everyone
    }
}
