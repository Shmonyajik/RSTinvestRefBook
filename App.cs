using RSTinvestRefBook.Services;
using System;
using System.Windows;

namespace RSTinvestRefBook
{
    public class App: Application
    {
        readonly MainWindow mainWindow;
        readonly IErrorLogger errorLogger;

        public App(MainWindow mainWindow, IErrorLogger errorLogger)
        {
            this.mainWindow = mainWindow;
            this.errorLogger = errorLogger;
            
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            mainWindow.ShowDialog();
            

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                errorLogger.LogError(exception);
            }
        }
    }
}