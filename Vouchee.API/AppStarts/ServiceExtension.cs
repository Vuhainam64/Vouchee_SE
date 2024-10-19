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
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace Vouchee.API.AppStarts
{
    public static class ServiceExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<VoucheeContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("PROD"))
            //           .EnableSensitiveDataLogging() // Enable sensitive data logging
            //           .LogTo(Console.WriteLine, LogLevel.Information)); // Log SQL commands

            services.AddSingleton(typeof(BaseDAO<>));

            services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // FILE UPLOAD
            services.AddScoped<IFileUploadService, FileUploadService>();

            // AUTH
            services.AddScoped<IAuthService, AuthService>();

            // VOUCHER
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();

            // ORDER
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // ROLE
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // SHOP
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            // SUPPLIER
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();

            // USER
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            // VOUCHER
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();

            // VOUCHER TYPE
            services.AddScoped<IVoucherTypeService, VoucherTypeService>();
            services.AddScoped<IVoucherTypeRepository, VoucherTypeRepository>();

            // PROMOTION
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();

            // VOUCHER CODE
            services.AddScoped<IVoucherCodeService, VoucherCodeService>();
            services.AddScoped<IVoucherCodeRepository, VoucherCodeRepository>();

            // CATEGORY
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // BRAND
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IBrandRepository, BrandRepository>();

            // ORDER DETAIL
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            // SUB VOUCHER
            services.AddScoped<ISubVoucherService, SubVoucherService>();
            services.AddScoped<ISubVoucherRepository, SubVoucherRepository>();

            // IMAGE
            services.AddScoped<IImageRepository, ImageRepository>();
        }

        public static void AddSwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vouchee.API", Version = "v1" });
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

            var check = GoogleCredential.GetApplicationDefault();

            //Firebase authentication
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = firebaseSettings.ProjectId,
                ServiceAccountId = firebaseSettings.ServiceAccountId
            });
        }

        public static void AddSettingObjects(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Strongly Typed Settings Objects
            ///Secret key
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
        }

        public static void AddJWTServices(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            //JWT
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }

    }
}
