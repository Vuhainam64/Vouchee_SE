using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Business.Services.Impls;
using Vouchee.Business.Services;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Helpers;
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
            services.AddScoped(typeof(VoucheeContext));
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IVoucherTypeService, VoucherTypeService>();
            services.AddScoped<IShopPromotionService, ShopPromotionService>();
            services.AddScoped<IVoucherCodeService, VoucherCodeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IModalService, ModalService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITopUpRequestService, TopUpRequestService>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();
            services.AddScoped<IPartnerTransactionService, PartnerTransactionService>();
            services.AddScoped<IModalPromotionService, ModalPromotionService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IDeviceTokenService, DeviceTokenService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IExcelExportService, ExcelExportService>();
            services.AddScoped<IWithdrawService, WithdrawService>();
            services.AddScoped<IDashboardService, DashboardService>();
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
