using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RSTinvestRefBook.Models;
using RSTinvestRefBook.Repositories;
using RSTinvestRefBook.Services;
using System;

namespace RSTinvestRefBook
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<BaseRepository<Position>, CSVPositionRepository>();
                    services.AddSingleton<IRefBookService, RefBookService>();
                    services.AddSingleton<IErrorLogger,  ErrorLogger>();
                })
                .Build();
        
            var app = host.Services.GetService<App>();
           
            app?.Run();
        }
    }
}
