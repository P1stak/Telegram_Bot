using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram_Bot
{
    internal class Program
    {
        private static TelegramBotClient botClient;
        static int count = 0;
        static async Task Main(string[] args)
        {
            var token = System.IO.File.ReadAllText(@"../../TGToken.txt");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient = new TelegramBotClient(token);
            Console.WriteLine("Запускается сервер...");

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions);

            Console.WriteLine("Сервер запущен!");
            Console.ReadLine();

        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            if (message != null && message.Text != null)
            {

                Console.WriteLine("Логи");
                Console.WriteLine($"{message.Chat.FirstName}    |    {message.Text}");

                switch (message.Text)
                {

                    case "привет":
                    case "/start":

                        await botClient.SendTextMessageAsync(message.Chat.Id, "Здравствуйте! Я бот Александр, азвлекательный бот!");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Чтобы увидеть мой функционал => /help");
                        break;

                    case "/help":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "\nДоступные команды:\n/quiz Викторина\n/weather Погода (Чтобы узнать погоду, допиши город)");
                        break;

                    case "/quiz": //первая викторина
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Начнем первую викторину!");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Столица Бразилии?\n1) Москва, 2) Рио, 3) Бразилиа, 4) Сингапур");
                        break;
                }
                
            }
            switch (message.Text)
            {
                case "3":
                    count++;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Правильно 1");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Вторая викторина!");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Где находится самая высокая гора России?\n1) Камчатка, 2) Урал, 3) Алтай, 4) Кавказ");
                    message.Text = null;
                    break;
            }
            switch (message.Text)
            {
                case "1":
                    count++;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Правильно 2");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Третья викторина!");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Какой самый большой планетой в Солнечной системе?\n 1) Земля, 2) Юрпитер, 3) Марс, 4) Венера");
                    message.Text = null;
                    break;
            }
            switch (message.Text)
            {
                case "2":
                    count++;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Правильно 3");
                    message.Text = null;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы ответили на {count} вопросов");
                    break;

            }

        }
        private static async Task<string> WeatherApi(string city)
            {
                string apiKey = "e59e576611bc9df0421b81d6f5daab2f"; // Замените YOUR_API_KEY на ваш API ключ
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=ru";

                var res = "Ошибка при получении данных о погоде.";

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var pattern = "feels_like.*?[,]";
                        string json = await response.Content.ReadAsStringAsync();
                        Match m = Regex.Match(json, pattern);

                        var s = m.Value;
                        Console.WriteLine(s.Replace("feels_like\":", "").Replace(",", ""));

                        var FeelsLike = s.Replace("feels_like\":", "").Replace(",", "");

                        res = FeelsLike;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при получении данных о погоде.");
                    }
                }
                return res;
            } //api weather

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                Console.WriteLine("Сервер не работает!");
                return Task.CompletedTask;
            }
        
    }
}
 
