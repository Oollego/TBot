//using Renci.SshNet;
//using Renci.SshNet.Messages;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using TBot.Domain.Services;
//using Telegram.Bot.Polling;
//using Telegram.Bot.Types;
//using Telegram.Bot;
//using Telegram.Bot.Types.Enums;
//using TBot.Domain.Enum;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace TBot.Application.Services
//{


//    // Класс для хранения данных пользователя
//    //public class UserData
//    //{
//    //    public long UserId { get; set; }
//    //    public string Username { get; set; } = "";
//    //    public UserRole Role { get; set; } = UserRole.Guest;
//    //    // Параметры для PHP-скрипта
//    //    public string? AppName { get; set; }
//    //    public string? AppBundle { get; set; }
//    //    public string? Secret { get; set; }
//    //    public string? SecretKeyParam { get; set; }
//    //    // Параметры SFTP-сервера
//    //    public string? SftpHost { get; set; }
//    //    public string? SftpLogin { get; set; }
//    //    public string? SftpPassword { get; set; }
//    //}

//    // Запись об успешной загрузке (для истории, просматриваемой администратором)
//    //public class UploadRecord
//    //{
//    //    public long UserId { get; set; }
//    //    public string Username { get; set; } = "";
//    //    public string? AppName { get; set; }
//    //    public string? AppBundle { get; set; }
//    //    public DateTime Timestamp { get; set; }
//    //}
//    public class TelegramService : ITelegramService
//    { 
//        // "База" пользователей и история загрузок
//        private static readonly Dictionary<long, UserData> Users = new Dictionary<long, UserData>();
//        private static readonly List<UploadRecord> Uploads = new List<UploadRecord>();

//        // Предопределённые пароли для авторизации (в реальном приложении – лучше использовать БД пользователей)
//        private const string UserPassword = "userpass";
//        private const string AdminPassword = "adminpass";

//        // Наш Telegram-бот клиент
//        private static TelegramBotClient bot = default!;
//        private static User? me;

//        public async Task StartBotAsync()
//        {
//            var token = Environment.GetEnvironmentVariable("TOKEN") ?? "";
//            using var cts = new CancellationTokenSource();

//            bot = new TelegramBotClient(token, cancellationToken: cts.Token);
//            me = await bot.GetMe();
//            //Console.WriteLine($"@{me.Username} is running... Press Escape to terminate");
//            Console.WriteLine($"@{me.Username} is running... Press Ctrl+C to stop.");

//                // Подписываемся на события
//            bot.OnMessage += OnMessage;
//            bot.OnUpdate += OnUpdate;
//            bot.OnError += OnError;
//                //bot.StartReceiving(cancellationToken: cts.Token);

//            await Task.Delay(-1);
//                //while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
//            //await cts.CancelAsync();
//        }

//        async static Task OnError(Exception exception, HandleErrorSource source)
//        {
//            Console.WriteLine("Error: " + exception.Message);
//            await Task.Delay(2000);
//        }

//        async static Task OnMessage(Message msg, UpdateType type)
//        {
//            if (msg == null)
//                return;

//            // Если сообщение не содержит текст – просто выводим тип
//            if (msg.Text is not { } text)
//            {
//                Console.WriteLine($"Received a message of type {msg.Type}");
//                return;
//            }

//            // Если сообщение начинается с '/', считаем его командой
//            if (text.StartsWith('/'))
//            {
//                // Отделяем команду и аргументы
//                var space = text.IndexOf(' ');
//                if (space < 0)
//                    space = text.Length;
//                var command = text[..space].ToLower();
//                var argsText = text[space..].Trim();

//                // Если команда адресована конкретному боту
//                if (command.LastIndexOf('@') is > 0 and int at)
//                {
//                    if (command[(at + 1)..].Equals(me?.Username, StringComparison.OrdinalIgnoreCase))
//                        command = command[..at];
//                    else
//                        return; // команда не для этого бота
//                }
//                await OnCommand(msg, command, argsText);
//            }
//            else
//            {
//                // Если не команда – выводим справку
//                await bot!.SendMessage(msg.Chat.Id, "Введите /help для списка команд.");
//            }
//        }

//        async static Task OnUpdate(Update update)
//        {
//            // Все команды идут через OnMessage, поэтому просто выводим тип обновления.
//            if (update.Type != UpdateType.Message)
//                {
//                    Console.WriteLine($"Received update: {update.Type}");
//                }
           
//            await Task.CompletedTask;
//        }


//        // Обработчик команд
//        static async Task OnCommand(Message msg, string command, string args)
//        {
//            // Получаем или создаем запись о пользователе
//            var user = GetOrCreateUser(msg.From!);

//            Console.WriteLine($"User {user.Username} ({user.UserId}) with role {user.Role} sent command: {command} {args}");

//            switch (command)
//            {
//                case "/help":
//                    await bot!.SendMessage(msg.Chat.Id,
//                        "/login <password> - авторизоваться\n" +
//                        "/setapp <AppName> <AppBundle> - задать параметры приложения\n" +
//                        "/mykeys - показать сгенерированные Secret и Secret-Key-Param\n" +
//                        "/setserver <host> <login> <password> - задать параметры SFTP-сервера\n" +
//                        "/upload - сгенерировать PHP-скрипт и загрузить его на сервер\n" +
//                        "/lastuploads - (админ) показать последние 10 загрузок\n" +
//                        "/setrole <userId> <Guest|User|Admin> - (админ) изменить роль пользователя");
//                    break;

//                case "/register":
//                    await HandleRegister(msg, args);
//                    break;

//                case "/login":
//                    await HandleLogin(msg, args, user);
//                    break;

//                case "/setapp":
//                    await HandleSetApp(msg, args, user);
//                    break;

//                case "/mykeys":
//                    await HandleMyKeys(msg, user);
//                    break;

//                case "/setserver":
//                    await HandleSetServer(msg, args, user);
//                    break;

//                case "/upload":
//                    await HandleUpload(msg, user);
//                    break;

//                case "/lastuploads":
//                    await HandleLastUploads(msg, user);
//                    break;

//                case "/setrole":
//                    await HandleSetRole(msg, args, user);
//                    break;

//                default:
//                    await bot!.SendMessage(msg.Chat.Id, "Неизвестная команда. Введите /help для списка команд.");
//                    break;
//            }
//        }

//        // Команда /register param: name, password 
//        static async Task HandleRegister(Message msg, string args)
//        {

//        }

//        // Команда /login param: name, password
//        static async Task HandleLogin(Message msg, string args, UserData user)
//        {
//            if (string.IsNullOrWhiteSpace(args))
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Укажите пароль. Пример: /login userpass");
//                return;
//            }

//            if (args.Trim() == AdminPassword)
//            {
//                user.Role = UserRole.Admin;
//                await bot!.SendMessage(msg.Chat.Id, "Вы авторизованы как Администратор.");
//            }
//            else if (args.Trim() == UserPassword)
//            {
//                user.Role = UserRole.User;
//                await bot!.SendMessage(msg.Chat.Id, "Вы авторизованы как Пользователь.");
//            }
//            else
//            {
//                await bot!.SendMessage(msg.Chat.Id, $"Неверный пароль.");

//            }
//        }

//        // Команда /setapp AppName, AppBundle
//        static async Task HandleSetApp(Message msg, string args, UserData user)
//        {
//            if (user.Role == UserRole.Guest)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Доступ запрещён. Авторизуйтесь командой /login.");
//                return;
//            }

//            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
//            if (parts.Length < 2)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Укажите два параметра: /setapp <AppName> <AppBundle>");
//                return;
//            }

//            user.AppName = parts[0];
//            user.AppBundle = parts[1];

//            // Генерация секретных ключей
//            user.Secret = GenerateSecret();
//            user.SecretKeyParam = GenerateSecretKeyParam();

//            await bot!.SendMessage(msg.Chat.Id, $"Параметры установлены:\nAppName: {user.AppName}\nAppBundle: {user.AppBundle}\n" +
//                $"Secret: {user.Secret}\nSecret-Key-Param: {user.SecretKeyParam}");
//        }

//        // Команда /mykeys — показать сгенерированные Secret и Secret-Key-Param
//        static async Task HandleMyKeys(Message msg, UserData user)
//        {
//            if (user.Role == UserRole.Guest)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Доступ запрещён. Авторизуйтесь командой /login.");
//                return;
//            }
//            if (user.Secret == null || user.SecretKeyParam == null)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Параметры приложения не установлены. Используйте /setapp.");
//                return;
//            }

//            await bot!.SendMessage(msg.Chat.Id, $"Ваши ключи:\nSecret: {user.Secret}\nSecret-Key-Param: {user.SecretKeyParam}");
//        }

//        // Команда /setserver param - host, login, password
//        static async Task HandleSetServer(Message msg, string args, UserData user)
//        {
//            if (user.Role == UserRole.Guest)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Доступ запрещён. Авторизуйтесь командой /login.");
//                return;
//            }

//            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
//            if (parts.Length < 3)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Укажите три параметра: /setserver <host> <login> <password>");
//                return;
//            }
//            user.SftpHost = parts[0];
//            user.SftpLogin = parts[1];
//            user.SftpPassword = parts[2];

//            await bot!.SendMessage(msg.Chat.Id, "Параметры SFTP-сервера установлены.");
//        }

//        // Команда /upload — генерация PHP-скрипта и загрузка на FTP
//        static async Task HandleUpload(Message msg, UserData user)
//        {
//            if (user.Role == UserRole.Guest)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Доступ запрещён. Авторизуйтесь командой /login.");
//                return;
//            }

//            if (user.AppName == null || user.AppBundle == null ||
//                user.Secret == null || user.SecretKeyParam == null)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Сначала задайте параметры приложения командой /setapp.");
//                return;
//            }
//            if (user.SftpHost == null || user.SftpLogin == null || user.SftpPassword == null)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Сначала задайте параметры SFTP-сервера командой /setserver.");
//                return;
//            }

//            var phpScript = GeneratePhpScript(user.AppName, user.AppBundle, user.Secret, user.SecretKeyParam);

//            // Пытаемся загрузить файл через SFTP
//            try
//            {
//                await UploadFileSftp(user, phpScript);
//                // Сохраняем запись о загрузке
//                Uploads.Add(new UploadRecord
//                {
//                    UserId = user.UserId,
//                    Username = user.Username,
//                    AppName = user.AppName,
//                    AppBundle = user.AppBundle,
//                    Timestamp = DateTime.UtcNow
//                });
//                await bot!.SendMessage(msg.Chat.Id, "Файл успешно загружен на сервер.");
//            }
//            catch (Exception ex)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Ошибка при загрузке файла: " + ex.Message);
//            }
//        }

//        // Команда /lastuploads — просмотр последних 10 загрузок (только для Админа)
//        static async Task HandleLastUploads(Message msg, UserData user)
//        {
//            if (user.Role != UserRole.Admin)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Доступ запрещён.");
//                return;
//            }
//            var last10 = Uploads.OrderByDescending(u => u.Timestamp).Take(10).ToList();
//            if (!last10.Any())
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Нет записей о загрузках.");
//                return;
//            }
//            var sb = new StringBuilder("Последние загрузки:\n");
//            foreach (var record in last10)
//            {
//                sb.AppendLine($"Пользователь: {record.Username} | AppName: {record.AppName} | AppBundle: {record.AppBundle} | {record.Timestamp}");
//            }
//            await bot!.SendMessage(msg.Chat.Id, sb.ToString());
//        }

//        // Команда /setrole param - userId, Guest|User|Admin — изменение роли (только для Админа)
//        static async Task HandleSetRole(Message msg, string args, UserData user)
//        {
//            if (user.Role != UserRole.Admin)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Доступ запрещён.");
//                return;
//            }
//            var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
//            if (parts.Length < 2)
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Использование: /setrole <userId> <Guest|User|Admin>");
//                return;
//            }
//            if (!long.TryParse(parts[0], out long targetUserId))
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Некорректный userId.");
//                return;
//            }
//            if (!Enum.TryParse<UserRole>(parts[1], true, out var newRole))
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Укажите корректную роль: Guest, User или Admin.");
//                return;
//            }
//            if (Users.TryGetValue(targetUserId, out var targetUser))
//            {
//                targetUser.Role = newRole;
//                await bot!.SendMessage(msg.Chat.Id, $"Пользователь {targetUser.Username} теперь имеет роль {newRole}.");
//            }
//            else
//            {
//                await bot!.SendMessage(msg.Chat.Id, "Пользователь не найден.");
//            }
//        }



//        // Получаем пользователя из "базы" или создаем нового
//        static UserData GetOrCreateUser(Telegram.Bot.Types.User telegramUser)
//        {
//            if (!Users.TryGetValue(telegramUser.Id, out var user))
//            {
//                user = new UserData
//                {
//                    UserId = telegramUser.Id,
//                    Username = telegramUser.Username ?? telegramUser.FirstName,
//                    Role = UserRole.Guest
//                };
//                Users.Add(telegramUser.Id, user);
//            }
//            return user;
//        }

//        // Генерация PHP-скрипта на основе шаблона
//        static string GeneratePhpScript(string appName, string appBundle, string secret, string secretKeyParam)
//        {
//            return $@"<?php
//                    $appName = '{appName}';
//                    $appBundle = '{appBundle}';
//                    $secretKey = '{secret}';
//                    if($secretKey == $_GET['{secretKeyParam}']){{echo 'Привет я приложение '. $appName .' моя ссылка на гугл плей https://play.google.com/store/apps/details?id='. $appBundle ;
//                    }}";
//        }

//        static string GenerateSecret()
//        {
//            using (var rsa = new RSACryptoServiceProvider(2048))
//            {
//                return Convert.ToBase64String(rsa.ExportCspBlob(false));
//            }

//        }

//        static string GenerateSecretKeyParam()
//        {
//            return Guid.NewGuid().ToString();
//        }

//        // Загрузка файла через SFTP с использованием SSH.NET
//        static async Task UploadFileSftp(UserData user, string fileContent)
//        {
//              //  Оборачиваем SftpClient в Task.Run, так как библиотека синхронная
//            //await Task.Run(() =>
//            //{
//                //using var sftp = new SftpClient(user.SftpHost, user.SftpLogin, user.SftpPassword);
//                //sftp.Connect();
//                //// В данном примере файл сохраняется в корневой директории под именем script.php.
//                //// При необходимости можно сделать динамическое имя или задать путь.
//                //using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
//                //sftp.UploadFile(ms, "script.php", true);
//                //sftp.Disconnect();
//            //});
//        }

//    }

//}
