using TBot.Domain.Services;

namespace TBot
{
    public class Worker : BackgroundService
    {
        private readonly ITelegramService _telegramService;

        public Worker( ITelegramService telegramService )
        {
            _telegramService = telegramService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                await _telegramService.StartBotAsync();
        }
    }
}
