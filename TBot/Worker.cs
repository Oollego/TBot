using System.Security.Cryptography;
using System.Text;
using TBot.Domain.Services;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TBot.Domain.Enum;
using TBot.Domain.Entity;
using Microsoft.IdentityModel.Tokens;
using TBot.Domain.Dto;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBot
{

    public class Worker : BackgroundService
    {
        private TelegramBotClient bot = default!;
        private User? me;

        public readonly IAuthService _authService;
        public readonly IAppService _appService;
        public readonly IStorageService _storageService;
        public readonly IRoleService _roleService;

        public Worker(IAuthService authService, IAppService secretService, IStorageService storageService, IRoleService roleService)
        {
            _authService = authService;
            _appService = secretService;
            _storageService = storageService;
            _roleService = roleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var token = Environment.GetEnvironmentVariable("TOKEN") ?? "";
            
            bot = new TelegramBotClient(token, cancellationToken: stoppingToken);
            me = await bot.GetMe();

            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;
            bot.OnError += OnError;
            
            await Task.Delay(-1);

            //await stoppingToken.CancelAsync();
        }

        async static Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine("Error: " + exception.Message);
            await Task.CompletedTask;
        }

        async Task OnMessage(Message msg, UpdateType type)
        {
            if (msg == null)
                return;

            // Если сообщение не содержит текст – просто выводим тип
            if (msg.Text is not { } text)
            {
                Console.WriteLine($"Received a message of type {msg.Type}");
                return;
            }

            // Если сообщение начинается с '/', считаем его командой
            if (text.StartsWith('/'))
            {
                // Отделяем команду и аргументы
                var space = text.IndexOf(' ');
                if (space < 0)
                    space = text.Length;
                var command = text[..space].ToLower();
                var argsText = text[space..].Trim();

                // Если команда адресована конкретному боту
                if (command.LastIndexOf('@') is > 0 and int at)
                {
                    if (command[(at + 1)..].Equals(me?.Username, StringComparison.OrdinalIgnoreCase))
                        command = command[..at];
                    else
                        return; // команда не для этого бота
                }
                await OnCommand(msg, command, argsText);
            }
            else
            {
                // Если не команда – выводим справку
                await bot!.SendMessage(msg.Chat.Id, "Введите /help для списка команд.");
            }
        }

        async static Task OnUpdate(Update update)
        {
            // Все команды идут через OnMessage, поэтому просто выводим тип обновления.
            if (update.Type != UpdateType.Message)
            {
                Console.WriteLine($"Received update: {update.Type}");
            }

            await Task.CompletedTask;
        }

        // Обработчик команд
        async Task OnCommand(Message msg, string command, string args)
        {

            switch (command)
            {
                case "/help":
                    await bot!.SendMessage(msg.Chat.Id,
                        "/register <name>, <password> - регистрация\n" +
                        "/login <password> - авторизоваться\n" +
                        "/setapp <AppName> <AppBundle> - задать параметры приложения\n" +
                        "/getkeys - показать сгенерированные Secret и Secret-Key-Param\n" +
                        "/setserver <host> <login> <password> - задать параметры SFTP-сервера\n" +
                        "/upload - сгенерировать PHP-скрипт и загрузить его на сервер\n" +
                        "/lastloads - показать последние 10 загрузок (только админ)\n" +
                        "/setrole <userId> <Guest|User|Admin> - изменить роль пользователя (только админ)");
                    break;

                case "/register":
                    await HandleRegister(msg, args);
                    break;

                case "/login":
                    await HandleLogin(msg, args);
                    break;

                case "/setapp":
                    await HandleSetApp(msg, args);
                    break;

                case "/getkeys":
                    await HandleGetKeys(msg);
                    break;

                case "/setserver":
                    await HandleSetServer(msg, args);
                    break;

                case "/upload":
                    await HandleUpload(msg);
                    break;

                case "/lastloads":
                    await HandleLastUploads(msg);
                    break;

                case "/setrole":
                    await HandleSetRole(msg, args);
                    break;

                default:
                    await bot!.SendMessage(msg.Chat.Id, "Неизвестная команда. Введите /help для списка команд.");
                    break;
            }
        }

        // Команда /register param: password 
        async Task HandleRegister(Message msg, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                await bot!.SendMessage(msg.Chat.Id, "Укажите пароль.");
            }

            User? telegramUser = msg.From;
            string name = telegramUser!.Username ?? telegramUser.FirstName;

            string password = args.Trim();

            var result = await _authService.RegisterAsync(name, password, msg.Chat.Id);

            await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
        }

        // Команда /login param: name, password
        async Task HandleLogin(Message msg, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                await bot!.SendMessage(msg.Chat.Id, "УУкажите пароль. Пример: /login userpass");
                return;
            }

            string password = args.Trim();

            var result = await _authService.LoginAsync(password, msg.Chat.Id);

           
            await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
          
        }

        // Команда /setapp AppName, AppBundle
        async Task HandleSetApp(Message msg, string args)
        {
            var argsArray = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (argsArray.Length != 2 || argsArray is null)
            {
                await bot!.SendMessage(msg.Chat.Id, "Укажите два параметра: /setapp AppName AppBundle");
                return;
            }

            string appName = argsArray[0];
            string appBundle = argsArray[1];

            var result = await _appService.SetAppAsync(appName, appBundle, msg.Chat.Id);


            await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
        }

        // Команда /getkeys — показать сгенерированные Secret и Secret-Key-Param
        async Task HandleGetKeys(Message msg)
        {
            var result = await _appService.GetSecretAsync( msg.Chat.Id);

            if (!result.IsSuccess || result.Data is null)
            {
                await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
            }
            else
            {
                await bot!.SendMessage(msg.Chat.Id, $"Ваши ключи:\nSecret: {result.Data.Secret}\nSecret-Key-Param: {result.Data.SecretKeyParam}");
            }
        }

        // Команда /setserver param - host, login, password
        async Task HandleSetServer(Message msg, string args)
        {
            var argsArray = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (argsArray.Length < 3)
            {
                await bot!.SendMessage(msg.Chat.Id, "Укажите три параметра: /setserver <host> <login> <password>");
                return;
            }
            string host = argsArray[0];
            string login = argsArray[1];
            string password = argsArray[2];

            var result = await _storageService.SetStorageDataAsync(host, login, password, msg.Chat.Id);

            await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
        }

        // Команда /upload — генерация PHP-скрипта и загрузка на FTP
        async Task HandleUpload(Message msg)
        {
            var result = await _storageService.SaveDataAsync(msg.Chat.Id);

            await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
        }

        // Команда /lastloads — просмотр последних 10 загрузок (только для Админа)
        async Task HandleLastUploads(Message msg)
        {
            var result = await _storageService.GetLastUploads(msg.Chat.Id);

            if (!result.IsSuccess)
            {
                await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
            }

           await bot.SendMessage(msg.Chat.Id, GetHtmlString(result.Data!), ParseMode.Html, protectContent: true);
        }

        // Команда /setrole param - userId, Guest|User|Admin — изменение роли (только для Админа)
        async Task HandleSetRole(Message msg, string args)
        {
            var argsArray = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (argsArray.Length != 2)
            {
                await bot!.SendMessage(msg.Chat.Id, "Укажите три параметра: /setrole <userId> <role>");
                return;
            }
            long id = Convert.ToInt64(argsArray[0]);
            string role = argsArray[1];

            var result = await _roleService.ChangeRoleAsync(msg.Chat.Id, role, id);

            await bot!.SendMessage(msg.Chat.Id, result.ResultMessage);
        }

        private static string GetHtmlString(IEnumerable<HistoryDto> historyList)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>История запросов:</b>");

            foreach (var history in historyList)
            {
                sb.AppendLine($"<b>Пользователь:</b> <code>{history.Username}</code>");
                sb.AppendLine($"<b>Приложение:</b> <code>{history.AppName ?? "N/A"}</code>");
                sb.AppendLine($"<b>Bundle:</b> <code>{history.AppBundle ?? "N/A"}</code>");
                sb.AppendLine($"<b>Дата:</b> <code>{history.Timestamp:yyyy-MM-dd HH:mm:ss} UTC</code>");
                sb.AppendLine("<br>");
            }
            return sb.ToString();
        }
    }
}
