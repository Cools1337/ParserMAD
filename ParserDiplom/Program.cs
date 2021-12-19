using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ParserDiplom
{
    class Program
    {
        
        public class Company
        {
            public List<List<string>> UrlCompanies { get; set; }
            public string Letters { get; set; }
            const int pages = 50;

            public Company(string letters)
            {
                Letters = letters;
            }
            public void Parsing()
            {
                UrlCompanies = new List<List<string>>();
                string host = @"https://hh.ru/employers_list?areaId=113&letter=";
                int counter = 0;
                foreach (var item in Letters)
                {
                    List<string> urlTemp = new List<string>() { host + item };

                    UrlCompanies.Add(urlTemp);
                    for (int i = 0; i < pages; i++)
                    {
                        string test = $"{UrlCompanies[0]}&page={i}";
                        UrlCompanies[counter].Add(GetStringFromHtml($"{urlTemp[0]}&page={i}"));
                    }
                    counter++;
                }
                Console.WriteLine($"спарсили буковки:{Letters}");
                CreateTXTFile(UrlCompanies, Letters);
            }
            private void CreateTXTFile(List<List<string>> company, string name)
            {
                FileStream fs = new FileStream($"{name}.txt", FileMode.Create);
                StreamWriter fnew = new StreamWriter(fs);
                foreach (var item in company)
                {
                    foreach (var val in item)
                    {
                        MatchCollection mc = Regex.Matches(val, @"(href=""/employer/[\d]*"">)");
                        foreach (Match m in mc)
                        {
                            fnew.WriteLine(m.Groups[1].Value);
                        }
                    }
                }
                fnew.Close();
            }
            string GetStringFromHtml(string url)
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.GetEncoding("utf-8");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    byte[] htmlData = new byte[1] { 126 };
                    try
                    {
                        htmlData = client.DownloadData(url);
                    }
                    catch (Exception) { }
                    return Encoding.GetEncoding("utf-8").GetString(htmlData);
                }
            }
        }
        static void Main(string[] args)
        {
            List<string> letters = new List<string>() { "АБВГДЕЖЗ", "ИЙКЛМНОПР", "СТУФХЦЧШЩ", "ЭЮЯABCDEF", "GHIJKLMNO", "PQRSTUVWXYZ" };
            foreach (var item in letters)
            {
                Company company = new Company(item);
                Thread myThread = new Thread(new ThreadStart(company.Parsing));
                myThread.Start();
            }
        }
        
    }
}
