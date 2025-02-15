using TBot.Application.DependencyInjection;
using TBot.DAL.DependencyInjection;

namespace TBot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();

            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddDataAccessLayer(builder.Configuration);

            var host = builder.Build();
            host.Run();
        }
    }
}