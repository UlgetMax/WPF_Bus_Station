using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp_Bus_Station.MVVM.View;
using WpfApp_Bus_Station.MVVM.ViewModel;
using WpfApp_Bus_Station.Views.Pages.Auth;

namespace WpfApp_Bus_Station
{
    public partial class MainWindow : Window
    {

        DataBase_BusStation dataBase = new DataBase_BusStation();

        public MainWindow()
        {
            InitializeComponent();
            //TestDbConnection();
            //this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(); 
        }

        private void TestDbConnection()
        {
            bool isConnected = dataBase.TestConnection();
            if (isConnected)
            {
                MessageBox.Show("Подключение к базе данных успешно установлено.");
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к базе данных.");
            }
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            AuthWindows authWindows = new AuthWindows();    
            authWindows.Show();
            Close();
        }
    }
}
