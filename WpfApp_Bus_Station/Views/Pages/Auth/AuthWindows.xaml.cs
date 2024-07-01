using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace WpfApp_Bus_Station.Views.Pages.Auth
{
    public partial class AuthWindows : Window
    {
        DataBase_BusStation dataBase = new DataBase_BusStation();

        public AuthWindows()
        {
            InitializeComponent();
            //TestDbConnection();
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
                //MessageBox.Show("Не удалось подключиться к базе данных.");
            }
        }

        //private void Button_Login_Click(object sender, RoutedEventArgs e)
        //{
        //    SqlDataAdapter adapter = new SqlDataAdapter();
        //    DataTable table = new DataTable();

        //    string login = textBoxLogin.Text.Trim();
        //    string password = textBoxPassword.Password;
        //    //string rol = comboBoxRol.Text.Trim().ToLower();

        //    //string querystring = $"select user_id, login, password, rol from Users where login = '{login}' and password = '{password}' and rol = '{rol}'";
        //    string querystring = $"SELECT user_id, login, password, rol FROM Users WHERE login = '{login}' COLLATE SQL_Latin1_General_CP1_CS_AS AND password = '{password}' COLLATE SQL_Latin1_General_CP1_CS_AS AND rol = '{rol}'";

        //    SqlCommand sqlCommand = new SqlCommand(querystring, dataBase.GetConnection());

        //    adapter.SelectCommand = sqlCommand;
        //    adapter.Fill(table);
        //    if (table.Rows.Count == 1)
        //    {
        //        MessageBox.Show("Вы вошли!");

        //        //Переадресация в кабинет пользователя
        //        MainWindow mainWindow = new MainWindow();
        //        mainWindow.Show();
        //        Close();

        //    }
        //    else
        //    {
        //        MessageBox.Show("Ошибка");
        //    }

        //    if (login.Length < 4)
        //    {
        //        textBoxLogin.ToolTip = "Мало символов!";
        //        textBoxLogin.Background = Brushes.DarkRed;
        //    }
        //    else if (password.Length < 5)
        //    {
        //        textBoxPassword.ToolTip = "Мало символов!";
        //        textBoxPassword.Background = Brushes.DarkRed;
        //    }
        //    //else if (rol != "д" || rol != "п")
        //    //{
        //    //    textBoxRol.ToolTip = "Выбрите что-то из д|п";
        //    //    textBoxRol.Background = Brushes.DarkRed;
        //    //}
        //    else
        //    {
        //        textBoxLogin.ToolTip = "Введите имя!";
        //        textBoxPassword.ToolTip = "Введите пароль!";
        //        //comboBoxRol.ToolTip = "Введите свою роль!";

        //        textBoxLogin.Background = Brushes.Transparent;
        //        textBoxPassword.Background = Brushes.Transparent;
        //        //comboBoxRol.Background = Brushes.Transparent;
        //    }

        //}

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Password;

            if (login.Length < 4)
            {
                textBoxLogin.ToolTip = "Мало символов!";
                textBoxLogin.Background = Brushes.DarkRed;
                return;
            }
            else if (password.Length < 5)
            {
                textBoxPassword.ToolTip = "Мало символов!";
                textBoxPassword.Background = Brushes.DarkRed;
                return;
            }
            else
            {
                textBoxLogin.ToolTip = "Введите имя!";
                textBoxPassword.ToolTip = "Введите пароль!";

                textBoxLogin.Background = Brushes.Transparent;
                textBoxPassword.Background = Brushes.Transparent;
            }

            string querystring = $"SELECT user_id, login, password, rol FROM Users WHERE login = @login COLLATE SQL_Latin1_General_CP1_CS_AS AND password = @password COLLATE SQL_Latin1_General_CP1_CS_AS";

            SqlCommand sqlCommand = new SqlCommand(querystring, dataBase.GetConnection());
            sqlCommand.Parameters.AddWithValue("@login", login);
            sqlCommand.Parameters.AddWithValue("@password", password);

            adapter.SelectCommand = sqlCommand;
            adapter.Fill(table);

            if (table.Rows.Count == 1)
            {
                string role = table.Rows[0]["rol"].ToString();
                MessageBox.Show("Вы вошли!");

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка");
            }
        }


        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            //Войти на форму регистрации

            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            Close();
        }
    }
}
