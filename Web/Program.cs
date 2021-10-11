using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Threading;

namespace Web
{

    internal class Program
    {
        static TelegramBotClient bot;
        [Obsolete]
        static void Main()
        {

            string token = File.ReadAllText(@"\token.txt");
            bot = new TelegramBotClient(token);
            Console.WriteLine("Бот запущен!");
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            //SendLessons();
            SendAlert();
            Console.ReadLine();
        }


        [Obsolete]
        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} " +
                $"{e.Message.Chat.Id} {e.Message.Text}";
            Console.Write("Сообщение от пользователя: \t");
            Console.WriteLine($"{text} Type msg: {e.Message.Type}");
            string msg = e.Message.Text;
            switch (msg)
            {
                case null:
                    return;
                case "/update":
                    SendLessons(e);
                    break;

                case "/schedule":
                    bot.SendTextMessageAsync(e.Message.Chat.Id, Lessons.GetSchedule());
                    break;
                case "Алло":
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.Chat.FirstName}, да здрасьте, здрасьте!");
                    break;

                default:
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.Chat.FirstName}, пока что реализована только команда /update и /schedule");
                    //bot.SendTextMessageAsync(e.Message.Chat.Id, $"Бот еще в стадии тестирования и разработки. По вопросам и предложениям писать @Fixway");
                    break;
            }
        }

        [Obsolete]
        private static void SendLessons(MessageEventArgs e = null)
        {
            long adminID = 329606681;
            string lesson = "";
            try
            {
                lesson = Data.GetUpdate();
            }
            catch (Exception ex)
            {
                bot.SendTextMessageAsync(adminID, $"Аварийное завершение программы! \nСообщение: {ex.Message}" +
                    $"\nДоп.инфа: {ex.ToString()}");
                Environment.Exit(-1);
            }
            finally
            {
                if (e == null)
                {
                    //bot.SendTextMessageAsync(-536570900, $"Добрый вечер!");
                    bot.SendTextMessageAsync(-536570900, $"{lesson}");

                }

                else
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{lesson}");
            }

        }

        [Obsolete]
        private static void SendAlert()
        {
            while (true)
            {
                int sleepTime = 60000;
                TimeSpan sleep = new TimeSpan();
                DateTime now = DateTime.Now;
                DateTime morningAlertTime = new DateTime(now.Year, now.Month, now.Day, 7, 5, 0);
                DateTime eveninAlertTime = new DateTime(now.Year, now.Month, now.Day, 19, 5, 0);
                DateTime tomorrow = new DateTime(now.Year, now.Month, now.Day + 1, 0, 0, 0);

                int hour = now.Hour;
                int min = now.Minute;
                int sec = now.Second;

                if ((hour == 7 && min == 5))
                {
                    bot.SendTextMessageAsync(-536570900, $"Доброе утро!");
                    SendLessons();
                    break;
                }
                else if ((hour == 19 && min == 5))
                {
                    bot.SendTextMessageAsync(-536570900, $"Добрый вечер!");
                    SendLessons();
                    break;
                }
                else
                {
                    if (now <= morningAlertTime)
                        sleep = morningAlertTime.Subtract(now);
                    if (now <= eveninAlertTime)
                        sleep = eveninAlertTime.Subtract(now);
                    else
                        sleep = tomorrow - now;
                    sleepTime = (sleep.Hours * 60 * 60 + sleep.Minutes * 60 + sleep.Seconds) * 1000;
                }
                Thread.Sleep(sleepTime);
            }
        }




    }
}

