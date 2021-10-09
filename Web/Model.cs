using System;
using System.Linq;

namespace Web
{

    struct WeekSchedule
    {
        static MainLesson[] ml;
        public static MainLesson GetDaySchedule(int day)
        {
            ml = SetWeekSch();
            for (int i = 0; i < ml.Length; i++)
            {
                if (i + 1 == day)
                {
                    return ml[i];
                }

            }
            return new MainLesson();
        }
        private static MainLesson[] SetWeekSch()
        {
            ml = new MainLesson[]{
                new MainLesson
                {
                    Pair = new string[] { "1", "2", "3", "4", "5", "6" },
                    Place = new string[] { "", "220", "115", "223", "", "" },
                    Subject = new string[] { "", "Иностранный язык", "Технология машиностроения", "Программирование", "", "" }
                },
                new MainLesson
                {
                    Pair = new string[] { "1", "2", "3", "4", "5", "6" },
                    Place = new string[] { "", "", "304", "115", "", "116" },
                    Subject = new string[] { "", "", "Инженерный дизайн", "МДК 03. 01", "Физкультура", "Технологическая оснастка" }
                },
                new MainLesson
                {
                    Pair = new string[] { "1", "2", "3", "4", "5", "6" },
                    Place = new string[] { "", "", "304", "115", "115", "" },
                    Subject = new string[] { "", "", "Инженерный дизайн", "МДК 03. 01", "МДК 03. 01", "Воспитательный час" }
                },
                new MainLesson
                {
                    Pair = new string[] { "1", "2", "3", "4", "5", "6" },
                    Place = new string[] { "", "115", "105", "223", "304", "" },
                    Subject = new string[] { "", "МДК 03. 01", "Основы философии", "Программирование", "Компьютерная графика", "" }
                },
                new MainLesson
                {
                    Pair = new string[] { "1", "2", "3", "4", "5", "6" },
                    Place = new string[] { "", "", "303", "116", "116", "115" },
                    Subject = new string[] { "", "", "Компьютерная графика", "Программирование", "Технологическая оснастка", "МДК 03. 01" }
                }
            };
            return ml;
        }
    }
    struct MainLesson
    {
        public string[] Pair { get; set; }
        public string[] Place { get; set; }
        public string[] Subject { get; set; }
        public string[] Teacher { get; set; }
    }
    struct Lessons
    {
        public string RecDate { get; set; }
        public string Group { get; set; }
        public string[] Pair { get; set; }
        public string[] Place { get; set; }
        public string[] Subject { get; set; }
        public string[] Teacher { get; set; }
        public void Print()
        {
            Console.WriteLine($"Группа: {Group}");
            Console.WriteLine($"Пары: {Pair}");
            Console.WriteLine($"Аудитория: {Place}");
            Console.WriteLine($"Предметы: {Subject}");
            Console.WriteLine($"Преподаватель: {Teacher}");

        }
        public string GetStringToSend()
        {

            int intDay = 0;
            string body = "";
            int minPairs = 1;// с какой пары приходить, с какой пары начинается изменения
            DateTime now = DateTime.Now;
            MainLesson ml = new MainLesson();
            DateTime minTime = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);

            string head = $"Актуальность: {RecDate}\nГруппа: {Group}\n";
            var d = from c in RecDate where char.IsDigit(c) select c;
            string textDate = "";
            foreach (var item in d)
            {
                textDate += item.ToString();
            }
            Int32.TryParse(textDate, out intDay);
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                ml = WeekSchedule.GetDaySchedule(1);
            else if ((now < minTime))
                ml = WeekSchedule.GetDaySchedule(((int)now.DayOfWeek));
            else
            {
                if (((int)now.DayOfWeek) == 5)
                    ml = WeekSchedule.GetDaySchedule(1);
                else
                    ml = WeekSchedule.GetDaySchedule(((int)now.DayOfWeek) + 1);

            }

            int hasMovePairs = 0;
            //int hasMovePairsToSubject = 0;


            for (int i = 0; i < Subject.Length; i++)
            {
                if (Subject[i].Contains("Прийти"))
                {
                    body += "* * * * * " + Subject[i] + " * * * * *";
                    var t = from c in Subject[i] where char.IsDigit(c) select c;
                    hasMovePairs = 1;
                    //hasMovePairsToSubject = 1;
                    foreach (var item in t)
                    {
                        Int32.TryParse(item.ToString(), out minPairs);
                        break;
                    }
                }
            }
            for (int i = 0; i < ml.Pair.Length; i++)
            {

                if (minPairs <= i + 1)
                {
                    bool hasVal = false;
                    for (int j = 0; j < Pair.Length; j++)
                    {

                        int pairAct = 0;
                        string[] pairActStr;
                        try
                        {
                            pairAct = int.Parse(Pair[j]);
                            if (pairAct == i + 1)
                            {
                                body += "\n_________________________________";
                                body += $"\nПара: {Pair[j]}\n" +
                                        $"Аудитория: {Place[j]}\nПредмет: {Subject[j + hasMovePairs] }\nПреподаватель: {Teacher[j]}";
                                hasVal = true;
                                minPairs++;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Pair[j] == "")
                            {
                                //minPairs++;
                                //hasVal = true;
                                continue;
                            }
                            else
                            {
                                pairActStr = Pair[j].Split(',');
                                //pairAct = int.Parse(pairActStr[0]);
                                foreach (string s in pairActStr)
                                {
                                    if (int.Parse(s) == i + 1)
                                    {
                                        pairAct = int.Parse(s);
                                        if (j + hasMovePairs + 1 > Subject.Length)
                                        {
                                            hasMovePairs = 0;
                                        }
                                        body += "\n_________________________________";
                                        body += $"\nПара: {pairAct}\n" +
                                                $"Аудитория: {Place[j]}\nПредмет: {Subject[j + hasMovePairs] }\nПреподаватель: {Teacher[j]}";
                                        hasVal = true;
                                        minPairs++;
                                        break;
                                    }
                                    else
                                    {
                                        hasVal = false;
                                    }
                                }
                            }
                        }
                    }
                    if (!hasVal)
                    {
                        //Console.WriteLine("Печатать с расписания");
                        if (ml.Subject[i] != "")
                        {
                            body += "\n_________________________________";
                            body += $"\nПара: {ml.Pair[i]}\n" +
                                    $"Аудитория: {ml.Place[i]}\nПредмет: {ml.Subject[i]}";
                            hasVal = false;
                        }

                    }
                }
                else
                {
                    // не печатать потому что приходить позже надо 
                }

            }
            string text = head + body;
            return text;
        }



        public static string GetSchedule()
        {
            string shedule = "1 пара: 08:30 - 09:40\n" +
                "перемена: 10 минут\n" +
                "2 пара: 09:50 - 11:00\n" +
                "перемена: 10 минут\n" +
                "3 пара: 11:10 - 12:20\n" +
                "перемена: 30 минут\n" +
                "4 пара: 12:50 - 14:00\n" +
                "перемена: 10 минут\n" +
                "5 пара: 14:10 - 15:20\n" +
                "перемена: 10 минут\n" +
                "6 пара: 15:30 - 16:40\n";
            return shedule;
        }
    }
}

