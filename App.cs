using System;
using System.Collections.Generic;
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
            this.mainWindow = mainWindow;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var filePath = System.Configuration.ConfigurationManager.AppSettings.Get("RefBookFilePath").ToString();
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine("Id,Name,Quantity,IsAcceptance");
                }
            }
            mainWindow.Show();  
            base.OnStartup(e);
        }
    }
}
