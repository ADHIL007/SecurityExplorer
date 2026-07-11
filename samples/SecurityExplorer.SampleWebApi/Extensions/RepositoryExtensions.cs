using SecurityExplorer.SampleWebApi.Repositories;

namespace SecurityExplorer.SampleWebApi.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddDepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
