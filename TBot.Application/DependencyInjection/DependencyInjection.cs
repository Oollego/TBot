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
            services.AddSingleton<ITelegramService, TelegramService>();
        }
    }
}
