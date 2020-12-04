using System;
using System.IO;

namespace yipli.Windows
{
    public static class FileReadWrite
    {
        static readonly string myDocLoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static readonly string yipliFolder = "Yipli";
        static readonly string yipliFile = "userinfo.txt";

        static readonly string webDownloadPage = "https://www.playyipli.com/";
        static readonly string webFaqPage = "https://swaraj429.github.io/yipli-new-website/faqs.html";
        static readonly string webtncPage = "http://playyipli.com/termsconditions.html";
        static readonly string webppPage = "https://playyipli.com/privacypolicy";

        public static string WebDownloadPage => webDownloadPage;

        public static string WebFaqPage => webFaqPage;

        public static string WebtncPage => webtncPage;

        public static string WebppPage => webppPage;

        public static string ReadFromFile()
        {
            try
            {
                string linedata = null;

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                if (File.Exists(myDocLoc + "/Yipli/userinfo.txt"))
                {
                    using (StreamReader sr = new StreamReader(myDocLoc + "/Yipli/userinfo.txt"))
                    {
                        string line;

                        // Read and display lines from the file until 
                        // the end of the file is reached. 
                        while ((line = sr.ReadLine()) != null)
                        {
                            linedata = line.Substring(17);
                        }
                    }

                    return linedata;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                UnityEngine.Debug.LogError("Reading Failed : " + e.Message);

                return null;
            }
        }

        public static void WriteToFile(string userID)
        {
            if (!Directory.Exists(myDocLoc + "/" + yipliFolder))
            {
                Directory.CreateDirectory(myDocLoc + "/" + yipliFolder);
                var yipliFileToCreate = File.Create(myDocLoc + "/" + yipliFolder + "/" + yipliFile);

                yipliFileToCreate.Close();
            }

            string writeLine = "Current UserID : " + userID;
            //UnityEngine.Debug.LogError("writeline is : " + writeLine);

            StreamWriter sw = new StreamWriter(myDocLoc + "/Yipli/userinfo.txt");
            sw.WriteLine(writeLine);
            sw.Close();

            ReadFromFile();
        }

        public static bool ValidateFile(string fileLocation)
        {
            return File.Exists(fileLocation);
        }
    }
}
