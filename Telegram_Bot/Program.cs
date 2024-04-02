using System;
using System.ComponentModel;
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
        static bool isGameStart = false;
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

            //if (isGameStart == false)
            //{
            //    string[] phrases = System.IO.File.ReadAllLines(@"../../PatternError.txt");

            //    Random random = new Random();
            //    int index = random.Next(phrases.Length);
            //    string randomPhrase = phrases[index];
            //    await botClient.SendTextMessageAsync(message.Chat.Id, randomPhrase);
            //} //ответы на не верные запросы


            if (message != null && message.Text != null)
            {
                isGameStart = false;

                Console.WriteLine("Логи");
                Console.WriteLine($"{message.Chat.FirstName}    |    {message.Text}");

                switch (message.Text.ToLower())
                {

                    case "привет":
                        
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Здравствуйте! Я бот Александр, азвлекательный бот!");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Чтобы увидеть мой функционал => /help");
                        break;

                    case "/help":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "\nДоступные команды:\n/quiz Викторина\n/weather Погода (Чтобы узнать погоду, допиши город)");
                        break;

                    case "/quiz":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Начнем викторину!");
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Столица Бразилии? Варианты ответов: 1) Москва, 2) Рио, 3) Бразилиа, 4) Сингапур");
                        break;

                    case "3":
                        
                        if (message.Text.ToLower().Contains("3"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Верно!");
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Викторина завершена!");
                            await botClient.SendTextMessageAsync(message.Chat.Id, "\nДоступные команды:\n/quiz Викторина\n/weather Погода (Чтобы узнать погоду, допиши город)");
                            isGameStart = false;
                            break;
                        }
                        else if(message.Text.ToLower().Contains("1") | message.Text.ToLower().Contains("2") | message.Text.ToLower().Contains("4"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Не верно!!");
                            return;
                        }
                        break;
                }
            }

            // погода
            if (message.Text.ToLower().Contains("/weather"))
            {
                var ss = message.Text.Split();
                if (ss[1] != null)
                {
                    var w = WeatherApi(ss[1]);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ощущается как " + w.Result + " ℃");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Error!");
                }
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