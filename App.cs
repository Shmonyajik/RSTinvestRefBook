using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RSTinvestRefBook
{
    public class App: Application
    {
        readonly MainWindow mainWindow;

        public App(MainWindow mainWindow)
        {
            Console.WriteLine("Запустилось!");
            var regexPattern = ConfigurationManager.AppSettings["HexIdRegexPattern"];
            var filePath = ConfigurationManager.AppSettings["RefBookFilePath"];
            if (string.IsNullOrEmpty(regexPattern))
            {
                throw new ConfigurationErrorsException("HexIdRegexPattern не найден в файле конфигурации.");
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ConfigurationErrorsException("RefBookFilePath не найден в файле конфигурации.");
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл справочника по пути {filePath} не обнаружен.");
            }
            this.mainWindow = mainWindow;
        }
        protected override void OnStartup(StartupEventArgs e)
        {

            mainWindow.Show();  
            base.OnStartup(e);
        }
    }
}