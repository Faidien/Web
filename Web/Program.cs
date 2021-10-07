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
            string token = File.ReadAllText(@"D:\token.txt");
            bot = new TelegramBotClient(token);
            Console.WriteLine("Бот запущен!");
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            //bot.SendTextMessageAsync(-536570900, $"Так как произошли изменения в сайте, бот работает неправильно. ");
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
            string lesson = Data.GetUpdate();
            if (e == null)
                bot.SendTextMessageAsync(-536570900, $"{lesson}");
            else
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"{lesson}");
        }

        [Obsolete]
        private static void SendAlert()
        {
            while (true)
            {
                int sleepTime = 60000;
                string hour = DateTime.Now.Hour.ToString();
                string min = DateTime.Now.Minute.ToString();
                if ((hour == "7" || hour == "19") /*&& isSent == false*/)
                {
                    switch (min)
                    {
                        case "5":
                            SendLessons();
                            sleepTime *= 12 * 60 * 12;
                            break;
                        default:
                            break;
                    }
                }

                Thread.Sleep(sleepTime);
            }
        }




    }
}

