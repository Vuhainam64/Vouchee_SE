using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Business.Services.Impls;
using Vouchee.Business.Services;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Helpers;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;
using Vouchee.Business.Models;

namespace Vouchee.API.AppStarts
{
    public static class ServiceExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient(typeof(VoucheeContext));

            services.AddSingleton(typeof(BaseDAO<>));

            services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // FILE UPLOAD
            services.AddScoped<IFileUploadService, FileUploadService>();

            // VOUCHER
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IVoucherService, VoucherService>();
        }

        public static void AddSwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RevenueSharingInvest.API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer YOUR_TOKEN_HERE\"",

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                           new OpenApiSecurityScheme
                             {
                                 Reference = new OpenApiReference
                                 {
                                     Type = ReferenceType.SecurityScheme,
                                     Id = "Bearer"
                                 }
                             },
                             new string[] {}
                     }
                 });
            });
        }

        public static void AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            ///Firebase storage
            var firebaseSettingSection = configuration.GetSection("FirebaseSettings");
            services.Configure<FirebaseSettings>(firebaseSettingSection);
            var firebaseSettings = firebaseSettingSection.Get<FirebaseSettings>();

            //Firebase authentication
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = firebaseSettings.ProjectId,
                ServiceAccountId = firebaseSettings.ServiceAccountId
            });
        }
    }
}
