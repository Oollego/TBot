using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TBot.Application.Services;
using TBot.Domain.Services;


namespace TBot.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IHashService, HashService>();
            services.AddSingleton<IAppService, AppService>();
            services.AddSingleton<ISecretService, SecretService>();
            services.AddSingleton<IPhpGeneratorService, PhpGeneratorService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<ISecretService, SecretService>();
            services.AddSingleton<ISftpService, SftpService>();
            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IUserService, UserService>();
        }
    }
}
