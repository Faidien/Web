using System;
using System.Globalization;
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
        /// <summary>
        /// Получение дня недели в соотвествии с правилами
        /// </summary>
        /// <returns>Если день недели - СБ, ВС, ПТ после 17.00 - вернет "Понедельник". Если любой другой день и время меньше 17.00 - возвращает текущий день. Иначе - завтрашний день</returns>
        private static DayOfWeek GetDayOfWeek()
        {
            DateTime now = DateTime.Now;
            DayOfWeek today = now.DayOfWeek;
            DateTime minTime = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
            DateTime tomorrow = new DateTime(now.Year, now.Month, now.Day + 1, now.Hour, now.Minute, now.Second);

            if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday || (today == DayOfWeek.Friday && now > minTime))
                return DayOfWeek.Monday;
            else if ((now < minTime))
                return today;
            else
                return tomorrow.DayOfWeek;
        }

        /// <summary>
        /// Получение номера дня недели
        /// </summary>
        /// <param name="day"></param>
        /// <returns>Номер дня недели</returns>
        private int GetIntOfDay(DayOfWeek day) => (int)day;

        /// <summary>
        /// Получение название дня недели
        /// </summary>
        /// <param name="day"></param>
        /// <returns>Название дня недели в соответсвии с текущими региональными настройками языка и формата даты</returns>
        private string GetStringOfDay(DayOfWeek day) => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day);

        /// <summary>
        /// Получить строки с актуальным, корректным расписанием.
        /// </summary>
        /// <returns>Текст готового расписания с учетом замен</returns>
        public string GetStringToSend()
        {
            string text = HeadBuild() + BodyBuild();
            return text;
        }
        /// <summary>
        /// Получение номера пары, с которой начинается замены. 
        /// </summary>
        /// <returns>Если замен по группе нет: -1. Если нет указания прийти к конкретной паре: 0. Иначе вернет номер пары. </returns>
        private int GetMinPairs()
        {
            int minPairs = 0;
            if (Subject != null)
            {
                for (int i = 0; i < Subject.Length; i++)
                {
                    if (Subject[i].Contains("Прийти"))
                    {
                        var t = from c in Subject[i] where char.IsDigit(c) select c;
                        foreach (var item in t)
                        {
                            int.TryParse(item.ToString(), out minPairs);
                            break;
                        }
                    }
                }
            }
            else
                return -1;
            return minPairs;
        }

        /// <summary>
        /// Получение числа на которое нужно "сдвинуть" предметы для корректного вывода.
        /// </summary>
        /// <returns>Вернет 1 в случае наличия ненужного текста в предметах(обычно это одна строка, в которой написано, к какой паре прийти). Иначе вернет 0</returns>
        private int GetOffsetSubject()
        {
            if (GetMinPairs() > 0)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Выбор источника для печати расписания
        /// </summary>
        /// <returns>Вернет true, если замен не было. Иначе false</returns>
        private bool IsPrintMainSchedule()
        {
            return GetMinPairs() == -1;
        }

        /// <summary>
        /// Построение "заголовка" сообщения.
        /// </summary>
        /// <returns>Текст заголовка: дата актуальности, группа, к какой паре прийти</returns>
        private string HeadBuild()
        {
            int pair = GetMinPairs();
            string s = "* * * * * Прийти ";
            if (pair <= 0)
                s += "по расписанию * * * * ";
            else
                s += $"к {pair} паре";
            string dayOfWeek = GetStringOfDay(GetDayOfWeek());
            return $"Актуальность: {RecDate}, {dayOfWeek}.\nГруппа: ТМ-129\n" + s;
        }

        private int GetCurrentPair(int pos, int mainPos)
        {
            int pairAct = 1;
            string[] pairActStr = GetPairString(pos);
            foreach (string s in pairActStr)
            {
                if (int.TryParse(s, out pairAct) && pairAct == mainPos + 1)
                    return pairAct;
            }
            return pairAct;
        }

        private string[] GetPairString(int pos)
        {
            string[] pairActStr = new string[] { };
            if (Pair[pos].Contains(','))
                pairActStr = Pair[pos].Split(',');
            else
                pairActStr.Append(Pair[pos]);
            return pairActStr;


        }

        /// <summary>
        /// Построение "тела" сообщения.
        /// </summary>
        /// <returns>Текс пар по порядку с заменами, если были.</returns>
        private string BodyBuild()
        {
            string text = "";
            int minPairs = GetMinPairs();
            bool isPrintMain = IsPrintMainSchedule();
            int numOffsetSubject = GetOffsetSubject();
            int pairAct = 1;
            for (int i = 0; i < 6; i++)
            {
                bool hasReplacement = false;
                if (!isPrintMain)
                {
                    if (minPairs <= i + 1)
                    {
                        for (int j = 0; j < Pair.Length; j++)
                        {
                            pairAct = GetCurrentPair(j, i);
                            if (j + numOffsetSubject + 1 > Subject.Length)
                            {
                                numOffsetSubject = 0;
                            }
                            text += GetReplacementSchedule(j, pairAct, numOffsetSubject);
                            hasReplacement = true;
                            minPairs++;
                        }
                    }
                    if (!hasReplacement)
                    {
                        text += GetOroginalSchedule(i);
                    }
                }
                else
                    text += GetOroginalSchedule(i);
            }
            return text;
        }
        /// <summary>
        /// Получение занятий с оригинального расписания
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Текст занятия по позиции</returns>
        private string GetOroginalSchedule(int pos)
        {
            string text = "";
            MainLesson ml = WeekSchedule.GetDaySchedule(GetIntOfDay(GetDayOfWeek()));
            if (ml.Subject[pos] != "")
            {
                text += "\n_________________________________";
                text += $"\nПара: {ml.Pair[pos]}\n" +
                        $"Аудитория: {ml.Place[pos]}\nПредмет: {ml.Subject[pos]}";
            }
            return text;
        }

        /// <summary>
        /// Получение занятий с замен
        /// </summary>
        /// <param name="pos">Позиция в массиве </param>
        /// <param name="pairAct">Пара</param>
        /// <param name="numMovePairs">Число сдвига предметов</param>
        /// <returns>Текст занятия с замен на сайте</returns>
        private string GetReplacementSchedule(int pos, int pairAct, int numMovePairs)
        {
            string text = "";
            text += "\n_________________________________";
            text += $"\nПара: {pairAct}\n" +
                    $"Аудитория: {Place[pos]}\nПредмет: {Subject[pos + numMovePairs] }\nПреподаватель: {Teacher[pos]}";
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

