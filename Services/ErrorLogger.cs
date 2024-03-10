using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Services
{
    public class ErrorLogger : IErrorLogger
    {
        private readonly string logFilePath;

        public ErrorLogger()
        {
            logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ConfigurationErrorsException("LogFilePath is not specified in App.config");
            }
        }
        public void LogError(Exception ex)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath);
            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (StreamWriter writer = new StreamWriter(fullPath, true))
            {
               writer.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}");
               writer.WriteLine($"StackTrace: {ex.StackTrace}");
               writer.WriteLine();
            }    
        
        }
    }
}
