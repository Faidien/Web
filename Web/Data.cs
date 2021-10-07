using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;

namespace Web
{
    internal class Data
    {
        public static HtmlDocument GetDoc()
        {
            WebClient web = new();
            using (Stream str = web.OpenRead("https://new.volit.ru/замена-занятий/"))
            {
                HtmlAgilityPack.HtmlDocument doc = new();
                doc.Load(str);
                //var doc2 = XDocument.Load("https://new.volit.ru/замена-занятий/");
                // var style = doc2.Elements("div")
                //.FirstOrDefault(e => e.Attribute("class")?.Value == "entry")
                //?.Value;

                return doc;
            }
        }
        public static string GetUpdate(string group = "ТМ-129")
        {
            Lessons les;
            string text;
            HtmlDocument doc = GetDoc();
            //GetGroupList();
            HtmlNodeCollection span = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[2]/div/div/div/p[3]");
            HtmlNodeCollection span2 = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[2]/div/div/div/table[1]/tbody");
            text = span[0].InnerText.ToString();

            for (int i = 0; i < span2[0].ChildNodes.Count; i++)
            {
                if (span2[0].ChildNodes[i].Name == "tr")
                {
                    if (span2[0].ChildNodes[i].ChildNodes[1].InnerText == group)
                    {
                        {
                            les = new Lessons
                            {
                                RecDate = text.Substring(0, text.IndexOf('&')),
                                Group = span2[0].ChildNodes[i].ChildNodes[1].InnerText.ToString().Replace("&nbsp;", "").Replace("\n", @"\t"),
                                Pair = span2[0].ChildNodes[i].ChildNodes[3].InnerText.ToString().Replace("&nbsp;", "").Split('\n'),
                                Place = span2[0].ChildNodes[i].ChildNodes[5].InnerText.ToString().Replace("&nbsp;", "").Split("\n"),
                                Subject = span2[0].ChildNodes[i].ChildNodes[7].InnerText.ToString().Replace("&nbsp;", "").Split("\n"),
                                Teacher = span2[0].ChildNodes[i].ChildNodes[9].InnerText.ToString().Replace("&nbsp;", "").Split("\n")
                            };
                            return les.GetStringToSend();
                        }
                    }
                }
            }
            les = new Lessons { Group = "", Pair = null, Place = null, RecDate = "Нет данных", Subject = null, Teacher = null };
            return les.GetStringToSend();
        }
        //public static void /*List<Lessons> */GetUpdatev2(string group = "ТМ-129")
        //{
        //    Lessons les;
        //    string text;
        //    HtmlDocument doc = GetDoc();
        //    HtmlNodeCollection span = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[2]/div/div/div/p[3]");
        //    HtmlNodeCollection span2 = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[2]/div/div/div/table[1]/tbody");
        //    text = span[0].InnerText.ToString();

        //    for (int i = 0; i < span2[0].ChildNodes.Count; i++)
        //    {
        //        if (span2[0].ChildNodes[i].Name == "tr")
        //        {
        //            if (span2[0].ChildNodes[i].ChildNodes[1].InnerText == group)
        //            {
        //                string recDate = text.Substring(0, text.IndexOf('&'));
        //                string Group = span2[0].ChildNodes[i].ChildNodes[1].InnerText.ToString().Replace("&nbsp;", "");
        //                string Pair = span2[0].ChildNodes[i].ChildNodes[3].InnerText.ToString();
        //                string Place = span2[0].ChildNodes[i].ChildNodes[5].InnerText.ToString().Replace("&nbsp;", "");
        //                string Subject = span2[0].ChildNodes[i].ChildNodes[7].InnerText.ToString().Replace("&nbsp;", "");
        //                string Teacher = span2[0].ChildNodes[i].ChildNodes[9].InnerText.ToString().Replace("&nbsp;", "");
        //                if (Pair.Contains("&nbsp;"))
        //                {
        //                    string s = text.Substring(0, text.IndexOf('&'));
        //                    //return span2[0].ChildNodes[i].ChildNodes[7].InnerText.ToString().Replace("&nbsp;", "").Substring(0,);
        //                }
        //                else
        //                {
        //                    les = new Lessons
        //                    {
        //                        recDate = text.Substring(0, text.IndexOf('&')),
        //                        //Group = span2[0].ChildNodes[i].ChildNodes[1].InnerText.ToString().Replace("&nbsp;", "").Replace(@"\n", @"\t"),
        //                        //Pair = span2[0].ChildNodes[i].ChildNodes[3].InnerText.ToString().Replace("&nbsp;", "").Replace(@"\n", @"\t"),
        //                        //Place = span2[0].ChildNodes[i].ChildNodes[5].InnerText.ToString().Replace("&nbsp;", "").Replace(@"\n", @"\t"),
        //                        //Subject = span2[0].ChildNodes[i].ChildNodes[7].InnerText.ToString().Replace("&nbsp;", "").Replace(@"\n", @"\t"),
        //                        //Teacher = span2[0].ChildNodes[i].ChildNodes[9].InnerText.ToString().Replace("&nbsp;", "").Replace(@"\n", @"\t")
        //                    };
        //                    //return les.GetStringToSend();
        //                }

        //                Console.WriteLine();
        //            }

        //        }
        //    }
        //    //les = new Lessons { Group = "", Pair = "", Place = "", recDate = "Нет данных", Subject = "", Teacher = "" };
        //    //return les.GetStringToSend();
        //}
        public static void /*string*/  /*List<string>*/ GetGroupList()
        {
            HtmlDocument doc = GetDoc();
            HtmlNodeCollection span2 = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div[4]/div[2]/div/div/div/table[1]/tbody");
        }
    }
}
