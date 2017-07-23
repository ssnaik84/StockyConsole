using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startDate = new DateTime(2017, 7, 14);
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            //get latest date
            var latestDates = new DatabaseProcessor().GetLatestDates(1);
            if (latestDates != null && latestDates.Count() > 0)
            {
                startDate = Convert.ToDateTime(latestDates[0]).AddDays(1);
            }

            var needToUpdateDB = true;

            if(startDate > endDate)
            {
                Console.WriteLine("start date > end date, no need to update");
                needToUpdateDB = false;
            }

            if (needToUpdateDB)
            {
                //DownloadAndSaveDB(startDate, endDate);
                SaveFromJarDB(startDate, endDate);
            }


            //get latest 5 dates
            latestDates = new DatabaseProcessor().GetLatestDates(5);

            string date_2, date_1, date0, date1, date2;
            if(latestDates.Count == 5)
            {
                date2 = latestDates[0];
                date1 = latestDates[1];
                date0 = latestDates[2];
                date_1 = latestDates[3];
                date_2 = latestDates[4];
            }
            else
            {
                Console.WriteLine("no last 5 days");
                Console.ReadKey();
                return;
            }

            CleanScreen();

            while (true)
            {
                var keyinfo = Console.ReadKey();

                switch(keyinfo.KeyChar)
                {
                    case 'q':
                        return;
                    case 'c':
                        CleanScreen();
                        break;
                    case 'i':
                        OpenCandleStickPattern(CandleStickPattern.InvertedHammer, date_2, date_1, date0, date1, date2);
                        break;
                    case 'a':
                        OpenCandleStickPattern(CandleStickPattern.MyChoice3Negative2Postive, date_2, date_1, date0, date1, date2);
                        break;
                    case 'b':
                        OpenCandleStickPattern(CandleStickPattern.MyChoice2Negative3Postive, date_2, date_1, date0, date1, date2);
                        break;

                }
            }
            
        }

        private static void OpenCandleStickPattern(CandleStickPattern pattern, string date_2, string date_1, string date0, string date1, string date2)
        {
            List<string> stockSymbols = null;
            switch (pattern)
            {
                case CandleStickPattern.InvertedHammer:
                    stockSymbols = new DatabaseProcessor().InvertedHammer(date_2, date_1, date0, date1, date2);
                    break;
                case CandleStickPattern.MyChoice3Negative2Postive:
                    stockSymbols = new DatabaseProcessor().MyChoice3Negative2Postive(date_2, date_1, date0, date1, date2);
                    break;
                case CandleStickPattern.MyChoice2Negative3Postive:
                    stockSymbols = new DatabaseProcessor().MyChoice2Negative3Postive(date_2, date_1, date0, date1, date2);
                    break;


            }
            //open urls in browsers
            OpenURLs(stockSymbols);
        }

        private static void CleanScreen()
        {
            Console.Clear();
            Console.WriteLine("Menu");
            Console.WriteLine("==============");
            Console.WriteLine("i = Inverted Hammer");
            Console.WriteLine("a = 3 negative 2 positive");
            Console.WriteLine("b = 2 negative 3 positive");
            Console.WriteLine("==============");
            Console.WriteLine("c = clear screen");
            Console.WriteLine("q = quit or exit");
        }

        private static void OpenURLs(List<string> symbols)
        {
            if(symbols != null)
            foreach (var symbol in symbols)
            {
                var url = "http://nseguide.com/charts.php?name=" + symbol;
                //System.Diagnostics.Process.Start(url);
                Process.Start("chrome", url);

            }
        }


        private static void SaveFromJarDB(DateTime startDate, DateTime endDate)
        {
            var objDownloader = new BhavcopyDownloader();

            var allweekdays = new List<DateTime>();
            while (endDate >= startDate)
            {
                if (startDate.DayOfWeek >= DayOfWeek.Monday && startDate.DayOfWeek <= DayOfWeek.Friday)
                    allweekdays.Add(startDate);

                startDate = startDate.AddDays(1);
            }
            
            //read file and insert data
            foreach (var date in allweekdays)
            {
                objDownloader.ReadNSEEODFileAndInsertData(date);
            }
        }




        private static void DownloadAndSaveDB(DateTime startDate, DateTime endDate)
        {
            var objDownloader = new BhavcopyDownloader();
            
            var allweekdays = new List<DateTime>();
            while (endDate >= startDate)
            {
                if (startDate.DayOfWeek >= DayOfWeek.Monday && startDate.DayOfWeek <= DayOfWeek.Friday)
                    allweekdays.Add(startDate);

                startDate = startDate.AddDays(1);
            }

            //download
            foreach (var date in allweekdays)
            {
                objDownloader.Download(date);
            }

            //unzip
            foreach (var date in allweekdays)
            {
                objDownloader.Unzip(date);
            }

            //read file and insert data
            foreach (var date in allweekdays)
            {
                objDownloader.ReadFileAndInsertData(date);
            }
        }

    }

    public enum CandleStickPattern
    {
        InvertedHammer = 1,


        MyChoice3Negative2Postive = 101,
        MyChoice2Negative3Postive = 102,
    }
}
