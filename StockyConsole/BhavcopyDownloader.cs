using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockyConsole
{
    public class BhavcopyDownloader
    {
        string BhavCopyURL = "https://www.nseindia.com/archives/equities/bhavcopy/pr/"; // PR{0}.zip";
        //https://www.nseindia.com/archives/equities/bhavcopy/pr/PR080617.zip

        string ZipDownloadPath = "C:\\BhavcopyDownloader\\ZipFiles\\";
        string extractPath = "C:\\BhavcopyDownloader\\Extract\\";

        string zipFileNameFormat = "PR{0}.zip";
        string csvFileNameFormat = "Pd{0}.csv";

        string txtFilePath = "C:\\Users\\Swapnil\\Documents\\GitHubVisualStudio\\StockyConsole\\NSE-EOD\\Eod\\";
        string txtFileNameFormat = "{0}.txt";
        string fileName = ""; 


        public bool Download(DateTime date)
        {
            string dateString = date.ToString("ddMMyy");
            fileName = string.Format(zipFileNameFormat, dateString);

            WebClient wb = new WebClient();
            wb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");

            try
            {
                wb.DownloadFile(BhavCopyURL + fileName, ZipDownloadPath + fileName);
                Console.WriteLine(fileName + " downloaded..!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(BhavCopyURL + fileName + " Exception: " + ex.Message);
                return false;
            }

            return true;
        }

        public bool IsFileExists(string fileName)
        {

            return false;
        }

        public bool Unzip(DateTime date)
        {
            string dateString = date.ToString("ddMMyy");
            fileName = string.Format(zipFileNameFormat, dateString);

            try
            {
                //System.IO.Compression.ZipFile.ExtractToDirectory(ZipDownloadPath + fileName, extractPath);

                using (ZipArchive archive = ZipFile.OpenRead(ZipDownloadPath + fileName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        //if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            

            return true;
        }


        public bool ReadFileAndInsertData(DateTime date)
        {
            string dateString = date.ToString("ddMMyy");
            fileName = string.Format(csvFileNameFormat, dateString);

            IList<string> lines = null;
            try
            {
                lines = File.ReadLines(extractPath + fileName).ToList();
            }
            catch(Exception ex)
            {
                return false;
            }

            var objDatabase = new DatabaseProcessor();

            foreach (string line in lines)
            {
                if (line != null && line.Contains(",") && line.Trim().Count() > 0)
                {
                    var fields = line.Split(',');
                    if(fields.Count() > 0)
                    {
                        if (fields[0].Trim().Equals("MKT") || fields[0].Trim().Equals("")) continue;

                        objDatabase.InsertEOD(date, fields[0].Trim(), fields[1].Trim(), fields[2].Trim(), fields[3].Trim(), Convert.ToDouble(fields[4].Trim()), Convert.ToDouble(fields[5].Trim()),
                            Convert.ToDouble(fields[6].Trim()), Convert.ToDouble(fields[7].Trim()), Convert.ToDouble(fields[8].Trim()), Convert.ToDouble(fields[9].Trim()),
                            Convert.ToDouble(fields[10].Trim()), fields[11].Trim(), fields[12].Trim(), Convert.ToInt64(fields[13].Trim()), Convert.ToDouble(fields[14].Trim()), Convert.ToDouble(fields[15].Trim()));
                    }
                }
                Console.WriteLine(line);
            }

            return true;
        }

        internal bool ReadNSEEODFileAndInsertData(DateTime date)
        {
            string dateString = date.ToString("ddMMMyyyy");
            fileName = string.Format(txtFileNameFormat, dateString);

            IList<string> lines = null;
            try
            {
                lines = File.ReadLines(txtFilePath + fileName).ToList();
            }
            catch (Exception ex)
            {
                return false;
            }

            var objDatabase = new DatabaseProcessor();

            foreach (string line in lines)
            {
                if (line != null && line.Contains(",") && line.Trim().Count() > 0)
                {
                    var fields = line.Split(',');
                    if (fields.Count() > 0)
                    {
                        //if (fields[0].Trim().Equals("MKT") || fields[0].Trim().Equals("")) continue;

                        objDatabase.InsertEOD(date, "", "", fields[0].Trim(), "", 0, Convert.ToDouble(fields[2].Trim()),
                            Convert.ToDouble(fields[3].Trim()), Convert.ToDouble(fields[4].Trim()), Convert.ToDouble(fields[5].Trim()), 0,
                            Convert.ToDouble(fields[6].Trim()), "", "", 0, 0, 0);
                    }
                }
                Console.WriteLine(line);
            }

            return true;
        }
    }
    
}
