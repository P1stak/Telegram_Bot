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
        static int quizStep = 0;
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
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Первый вопрос:\nСтолица Бразилии?\n1) Москва, 2) Рио, 3) Бразилиа, 4) Сингапур"); //1
                        quizStep = 1;
                        break;


                    case var text when quizStep == 1:
                        if (text.StartsWith("3"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Верно!");
                            count++;
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Второй вопрос:\nКакой самый большой планетой в Солнечной системе?\n1) Земля, 2) Юпитер, 3) Марс, 4) Венера"); //2
                            quizStep = 2;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Неверно!");
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Второй вопрос:\nКакой самый большой планетой в Солнечной системе?\n1) Земля, 2) Юпитер, 3) Марс, 4) Венера");
                            quizStep = 2;
                        }
                        break;

                    case var text when quizStep == 2:
                        if (text.StartsWith("2"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Верно!");
                            count++;
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Третий вопрос:\nКакой самый высокий горой в мире?\n1) Килиманджаро, 2) Эверест, 3) Монблан, 4) Денали"); //3
                            quizStep = 3;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Неверно!");
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Третий вопрос:\nКакой самый высокий горой в мире?\n1) Эверест, 2) Килиманджаро, 3) Монблан, 4) Денали");
                            quizStep = 3;
                        }
                        break;

                    case var text when quizStep == 3:
                        if (text.StartsWith("2"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Верно!");
                            count++;
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Четверный вопрос:\nКакой химический элемент имеет атомный номер 1 ?\n1) Кислород, 2) Водород, 3) Гелий, 4) Углерод");
                            quizStep = 4;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Неверно!");
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Четверный вопрос:\nКакой химический элемент имеет атомный номер 1 ?\n1) Кислород, 2) Водород, 3) Гелий, 4) Углерод");
                            quizStep = 4;
                        }
                        break;

                    case var text when quizStep == 4:
                        if (text.StartsWith("2"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Верно!");
                            count++;
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Викторина завершена. Количество правильных ответов {count}");
                            quizStep = 5;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Неверно!");
                            quizStep = 5;
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Викторина завершена. Количество правильных ответов {count}");
                        }
                        break;
                }
                if (message.Text.ToLower().Contains("/weather"))
                {
                    var ss = message.Text.Split();
                    if (ss.Length > 1)
                    {
                        var w = WeatherApi(ss[1]);
                        if (w.Result != "Ошибка при получении данных о погоде.")
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Ощущается как {w.Result}  ℃");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, w.Result);
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Введите название города для прогноза погоды.");
                    }
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
 
