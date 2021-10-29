using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Threading;
using System.Collections.Generic;

namespace Web
{

    class Program
    {
        static long adminID = 979093450;
        static int sleepTime = 60000;
        static TelegramBotClient bot;

        [Obsolete]
         static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            string token = File.ReadAllText(@"\token.txt");
            bot = new TelegramBotClient(token);
            Console.WriteLine("Бот запущен!");
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            SendAlert();
            Console.ReadLine();
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            bot.SendTextMessageAsync(adminID, $"Бота выключили!\nВремя выключения = {now.Hour}:{now.Minute}:{now.Second}");
        }
        /// <summary>
        /// Слушает входящие сообщения от пользователей.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]

        private static void MessageListener(object sender, MessageEventArgs e)
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
                //case "/turnOff":
                //    if (e.Message.Chat.Id == adminID)
                //    {
                //        bot.StopReceiving();
                //        bot.SendTextMessageAsync(e.Message.Chat.Id, "Бот будет выключен!");
                //        //Environment.Exit(0);
                //    }
                //else
                //    bot.SendTextMessageAsync(e.Message.Chat.Id, "Команда доступна только для администратора!");
                //break;
                case "/timeupd":
                    DateTime now = DateTime.Now;
                    TimeSpan sleep = new TimeSpan();
                    DateTime eveninAlertTime = new DateTime(now.Year, now.Month, now.Day, 19, 5, 0);
                    sleep = eveninAlertTime.Subtract(now);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"До обновления расписания осталось - {sleep.Hours}:{sleep.Minutes}:{sleep.Seconds}");
                    break;
                case "/schedule":
                    bot.SendTextMessageAsync(e.Message.Chat.Id, Lessons.GetSchedule());
                    break;
                case "/fupd":
                    SendLessons(e, true);
                    break;
                //case "Алло":
                //    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.Chat.FirstName}, да здрасьте, здрасьте!");
                //    break;

                default:
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.Chat.FirstName}, пока что реализована только команда /update, /schedule, /timeupd");
                    //bot.SendStickerAsync(e.Message.Chat.Id,)
                    break;
            }
        }
        /// <summary>
        /// Отправка расписания в ответ на команду апдейт
        /// </summary>
        /// <param name="e"></param>
        [Obsolete]
        private static void SendLessons(MessageEventArgs e = null, bool modeNum = false)
        {
            string lesson = "";
            try
            {
                lesson = Data.GetUpdate(modeNum);
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
        /// <summary>
        /// Отправка сообщения по расписанию - в 7 утра и вечера.
        /// </summary>
        [Obsolete]
        private static void SendAlert()
        {
            while (true)
            {

                bool isNeedUpdated = true;
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
                    SendLessons(modeNum: isNeedUpdated);
                    break;
                }
                else
                {
                    if (now <= morningAlertTime)
                        sleep = morningAlertTime.Subtract(now);
                    else if (now <= eveninAlertTime)
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

