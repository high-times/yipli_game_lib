using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FmResponseFile
{
    private static string fileName;
    private static string filePath;

    public static string FileName { get => fileName; set => fileName = value; }
    public static string FilePath { get => filePath; set => filePath = value; }

    private static void CreateLogFile()
    {
        // File name and File path
        //FileName = "FmResponseLogs_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".txt";
        FileName = "FmResponseLogs.txt";
        FilePath = Application.dataPath + "/TroubleshootingModule/" + FileName;

        // Create File if doesn't exit
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "Troubleshoot logs : " + DateTime.Now + " \n\n");
        }
        else
        {
            File.WriteAllText(FilePath, "Troubleshoot logs : " + DateTime.Now + " \n\n");
        }
    }

    public static void WriteResponseToFile(List<string> fmResponseList)
    {
        CreateLogFile();

        foreach (string s in fmResponseList)
        {
            File.AppendAllText(FilePath, s);
            File.AppendAllText(FilePath, "\n");
        }
    }

    public static async System.Threading.Tasks.Task UploadLogsAsync(string userID)
    {
        CreateLogFile();

        if (File.Exists(FilePath))
        {
            await FirebaseDBHandler.UploadLogsFileToDB(userID, FileName, FilePath);
        }

        Debug.LogError("File Upload Finished");
        //DeleteLogFile();
    }

    private static void DeleteLogFile()
    {
        File.Delete(FilePath);
    }
}
