using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Web
{

    struct DataTable
    {
        public string Title { get; set; }
        public HtmlNode tableNodes { get; set; }

    }
    internal class Data
    {
        public static HtmlNodeCollection GetDoc()
        {
            WebClient web = new();
            using (Stream str = web.OpenRead("https://volit.ru/замена-занятий/"))
            {
                HtmlDocument doc = new();
                doc.Load(str);
                var docc = doc.DocumentNode.SelectNodes(".//div[@class='entry']");
                return docc;
            }
        }
        public static List<DataTable> GetData()
        {
            HtmlNodeCollection doc = GetDoc();
            string pText = "";
            List<DataTable> dt = new List<DataTable>();

            foreach (var item in doc)
            {
                var t = item.ChildNodes;
                if (t != null && t.Count > 0)
                {
                    foreach (var p in t)
                    {
                        if (p != null)
                        {
                            if (p.Name == "p")
                            {
                                if (!p.InnerText.ToLower().Contains("загрузка"))
                                    pText += p.InnerText.Replace("\n", " ").Replace("&#8211;", " ") + " ";
                            }
                            else if (p.Name == "table")
                            {
                                foreach (var table in p.ChildNodes)
                                {
                                    if (table.Name == "tbody")
                                    {
                                        dt.Add(new DataTable { Title = pText, tableNodes = table });
                                        pText = "";
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return dt;
        }
        public static List<Lessons> GetLessons()
        {
            List<Lessons> lsy = new List<Lessons>();
            List<DataTable> dt = GetData();
            string group = "";
            string pair = "";
            string place = "";
            string subj = "";
            string teacher = "";
            bool isMerged = false;

            foreach (var item in dt)
            {
                var table = item.tableNodes.SelectNodes(".//tr");
                foreach (var rows in table)
                {
                    var row = rows.SelectNodes(".//td");
                    if (row != null && row.Count > 0)
                    {
                        if (row.Count == 4)
                        {
                            if (!isMerged)
                                lsy.Remove(lsy[lsy.Count - 1]);
                            pair += "\n" + row[0].InnerText.Replace("&nbsp;", "");
                            place += "\n" + row[1].InnerText.Replace("&nbsp;", "");
                            subj += "\n" + row[2].InnerText.Replace("&nbsp;", "");
                            teacher += "\n" + row[3].InnerText.Replace("&nbsp;", "");
                            isMerged = true;
                            continue;
                        }
                        else
                        {
                            if (isMerged)
                            {
                                //добавление данных из собранных строк
                                lsy.Add(new Lessons
                                {
                                    RecDate = item.Title,
                                    Group = group,
                                    Pair = pair.Split('\n'),
                                    Place = place.Split('\n'),
                                    Subject = subj.Split('\n'),
                                    Teacher = teacher.Split('\n'),
                                });
                                isMerged = false;
                                //очищение строк для след итерации
                                group = "";
                                pair = "";
                                place = "";
                                subj = "";
                                teacher = "";
                            }
                            //добавление текущих данных из парсинга
                            lsy.Add(new Lessons
                            {
                                RecDate = item.Title,
                                Group = row[0].InnerText,
                                Pair = row[1].InnerText.Replace("&nbsp;", "").Split('\n'),
                                Place = row[2].InnerText.Replace("&nbsp;", "").Split('\n'),
                                Subject = row[3].InnerText.Replace("&nbsp;", "").Split('\n'),
                                Teacher = row[4].InnerText.Replace("&nbsp;", "").Split('\n'),
                            });
                            group = row[0].InnerText;
                            pair = row[1].InnerText.Replace("&nbsp;", "");
                            place = row[2].InnerText.Replace("&nbsp;", "");
                            subj = row[3].InnerText.Replace("&nbsp;", "");
                            teacher = row[4].InnerText.Replace("&nbsp;", "");
                        }
                    }
                }
            }
            return lsy;
        }
        public static string GetUpdate(bool isNeedIpdate, string group = "ТМ-129")
        {
            List<Lessons> lst = GetLessons();
            foreach (var item in lst)
            {
                if (item.Group.Contains(group))
                {
                    return item.GetStringToSend();
                }
            }
            return "На сайте нет данных по заменам!";
        }


    }
}
