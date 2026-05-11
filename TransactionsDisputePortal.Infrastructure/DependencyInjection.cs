using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Application.Common.Services;
using TransactionsDisputePortal.Domain.Common.Interfaces;
using TransactionsDisputePortal.Domain.Repositories;
using TransactionsDisputePortal.Infrastructure.Persistence;
using TransactionsDisputePortal.Infrastructure.Repositories;
using TransactionsDisputePortal.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TransactionsDisputePortal.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            
            // Register repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IDisputeRepository, DisputeRepository>();
            services.AddScoped<IDisputeHistoryRepository, DisputeHistoryRepository>();
            services.AddScoped<IDisputeAttachmentRepository, DisputeAttachmentRepository>();
            
            // Register authentication services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            
            // Register file storage service
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            
            // Register lookup service as singleton (loads data once and caches it)
            services.AddSingleton<ILookupService, LookupService>();
            
            return services;
        }
    }
}