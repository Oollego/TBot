//using Renci.SshNet;
//using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Services;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TBot.Domain.Enum;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TBot.Application.Services
{


    // Класс для хранения данных пользователя
    public class UserData
    {
        public long UserId { get; set; }
        public string Username { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.Guest;
        // Параметры для PHP-скрипта
        public string? AppName { get; set; }
        public string? AppBundle { get; set; }
        public string? Secret { get; set; }
        public string? SecretKeyParam { get; set; }
        // Параметры SFTP-сервера
        public string? SftpHost { get; set; }
        public string? SftpLogin { get; set; }
        public string? SftpPassword { get; set; }
    }

    // Запись об успешной загрузке (для истории, просматриваемой администратором)
    public class UploadRecord
    {
        public long UserId { get; set; }
        public string Username { get; set; } = "";
        public string? AppName { get; set; }
        public string? AppBundle { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class TelegramService : ITelegramService
    { 
        // "База" пользователей и история загрузок
        private static readonly Dictionary<long, UserData> Users = new Dictionary<long, UserData>();
        private static readonly List<UploadRecord> Uploads = new List<UploadRecord>();

        // Предопределённые пароли для авторизации (в реальном приложении – лучше использовать БД пользователей)
        private const string UserPassword = "userpass";
        private const string AdminPassword = "adminpass";

        // Наш Telegram-бот клиент
        private static TelegramBotClient bot = default!;
        private static User? me;

        public async Task StartBotAsync()
        {
            var token = Environment.GetEnvironmentVariable("TOKEN") ?? "";
            using var cts = new CancellationTokenSource();

            bot = new TelegramBotClient(token, cancellationToken: cts.Token);
            me = await bot.GetMe();
            //Console.WriteLine($"@{me.Username} is running... Press Escape to terminate");
            Console.WriteLine($"@{me.Username} is running... Press Ctrl+C to stop.");

                // Подписываемся на события
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;
            bot.OnError += OnError;
                //bot.StartReceiving(cancellationToken: cts.Token);

            await Task.Delay(-1);
                //while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
            //await cts.CancelAsync();
        }

        async static Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine("Error: " + exception.Message);
            await Task.Delay(2000);
        }

        async static Task OnMessage(Message msg, UpdateType type)
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
        static async Task OnCommand(Message msg, string command, string args)
        {


        

            switch (command)
            {
                case "/help":
                    await bot!.SendMessage(msg.Chat.Id,
                        "/login <password> - авторизоваться\n" +
                        "/setapp <AppName> <AppBundle> - задать параметры приложения\n" +
                        "/mykeys - показать сгенерированные Secret и Secret-Key-Param\n" +
                        "/setserver <host> <login> <password> - задать параметры SFTP-сервера\n" +
                        "/upload - сгенерировать PHP-скрипт и загрузить его на сервер\n" +
                        "/lastuploads - (админ) показать последние 10 загрузок\n" +
                        "/setrole <userId> <Guest|User|Admin> - (админ) изменить роль пользователя");
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

                case "/mykeys":
                    await HandleMyKeys(msg);
                    break;

                case "/setserver":
                    await HandleSetServer(msg, args);
                    break;

                case "/upload":
                    await HandleUpload(msg);
                    break;

                case "/lastuploads":
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

        // Команда /register param: name, password 
        static async Task HandleRegister(Message msg, string args)
        {

        }

        // Команда /login param: name, password
        static async Task HandleLogin(Message msg, string args)
        {

        }

        // Команда /setapp AppName, AppBundle
        static async Task HandleSetApp(Message msg, string args)
        {
 
        }

        // Команда /mykeys — показать сгенерированные Secret и Secret-Key-Param
        static async Task HandleMyKeys(Message msg)
        {
 
        }

        // Команда /setserver param - host, login, password
        static async Task HandleSetServer(Message msg, string args)
        {

        }

        // Команда /upload — генерация PHP-скрипта и загрузка на FTP
        static async Task HandleUpload(Message msg)
        {

        }

        // Команда /lastuploads — просмотр последних 10 загрузок (только для Админа)
        static async Task HandleLastUploads(Message msg)
        {

        }

        // Команда /setrole param - userId, Guest|User|Admin — изменение роли (только для Админа)
        static async Task HandleSetRole(Message msg, string args)
        {

        }



    

        // Генерация PHP-скрипта на основе шаблона
        static string GeneratePhpScript(string appName, string appBundle, string secret, string secretKeyParam)
        {
            return $@"<?php
                    $appName = '{appName}';
                    $appBundle = '{appBundle}';
                    $secretKey = '{secret}';
                    if($secretKey == $_GET['{secretKeyParam}']){{echo 'Привет я приложение '. $appName .' моя ссылка на гугл плей https://play.google.com/store/apps/details?id='. $appBundle ;
                    }}";
        }

        static string GenerateSecret()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                return Convert.ToBase64String(rsa.ExportCspBlob(false));
            }

        }

        static string GenerateSecretKeyParam()
        {
            return Guid.NewGuid().ToString();
        }



    }

}
