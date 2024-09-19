using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Helpers;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.API.AppStarts
{
    public static class DependencyInjectionResolver
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient(typeof(VoucheeContext));

            services.AddSingleton(typeof(BaseDAO<>));

            services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // VOUCHER
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IVoucherService, VoucherService>();
        }
    }
}
