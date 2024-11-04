using Microsoft.Extensions.DependencyInjection;

namespace Prisma.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseContext<TContext>(this IServiceCollection services, Action<DatabaseContextOptions> options)
            where TContext : DatabaseContext
        {
            var databaseContextOptions = new DatabaseContextOptions<TContext>();
            options(databaseContextOptions);

            var databaseContextInstance = (TContext)Activator.CreateInstance(typeof(TContext), databaseContextOptions)!;

            services.AddScoped(serviceProvider => databaseContextInstance);

            return services;
        }
    }
}
