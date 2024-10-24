using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace To_Do_List
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализация базы данных
            var databaseService = new DatabaseService();
            databaseService.InitializeDatabase();

            // Создание главного окна
            var mainWindow = new MainWindow();
            var viewModel = new ViewModel();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
