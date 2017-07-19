﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockyConsole
{
    class Program
    {        
        static void Main(string[] args)
        {
            var objDownloader = new BhavcopyDownloader();
            DateTime startDate = new DateTime(2017, 7, 9); //(2015, 1, 1);
            DateTime endDate = DateTime.Now;
            var allweekdays = new List<DateTime>();
            while (endDate >= startDate)
            {
                startDate = startDate.AddDays(1);

                if(startDate.DayOfWeek >= DayOfWeek.Monday && startDate.DayOfWeek <= DayOfWeek.Friday)
                    allweekdays.Add(startDate);
            }

            ////download
            //foreach(var date in allweekdays)
            //{
            //    objDownloader.Download(date);

            //    //if (objDownloader.Download(date))
            //    //    if (objDownloader.Unzip(date))
            //    //        continue;
            //}

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
            


            Console.ReadKey();
        }
    }
}