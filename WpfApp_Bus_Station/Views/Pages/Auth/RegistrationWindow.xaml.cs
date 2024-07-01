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
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        DataBase_BusStation dataBase = new DataBase_BusStation();


        public RegistrationWindow()
        {
            InitializeComponent();
        }


        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            AuthWindows authWindows = new AuthWindows();
            authWindows.Show();
            Close();

        }


        private void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Password;
            //string rol = comboBoxRol.Text.Trim().ToLower();


            if (login.Length < 4)
            {
                textBoxLogin.ToolTip = "Мало символов!";
                textBoxLogin.Background = Brushes.DarkRed;
            }
            else if (password.Length < 5)
            {
                textBoxPassword.ToolTip = "Мало символов!";
                textBoxPassword.Background = Brushes.DarkRed;
            }
            else
            {
                textBoxLogin.ToolTip = "Введите имя!";
                textBoxPassword.ToolTip = "Введите пароль!";
                //comboBoxRol.ToolTip = "Введите свою роль!";

                textBoxLogin.Background = Brushes.Transparent;
                textBoxPassword.Background = Brushes.Transparent;
                //comboBoxRol.Background = Brushes.Transparent;
                dataBase.openConnection();
                if (!checkuser(login))
                {
                    try
                    {
                        // Получение минимального доступного ID
                        int freeID = GetMinFreeUserID();

                        string enableIdentityInsertQuery = "SET IDENTITY_INSERT Users ON";
                        string disableIdentityInsertQuery = "SET IDENTITY_INSERT Users OFF";
                        string querystring = "INSERT INTO Users(user_id, login, password, rol) VALUES(@id, @login, @password, @rol)";

                        using (SqlCommand enableIdentityInsertCommand = new SqlCommand(enableIdentityInsertQuery, dataBase.GetConnection()))
                        using (SqlCommand disableIdentityInsertCommand = new SqlCommand(disableIdentityInsertQuery, dataBase.GetConnection()))
                        using (SqlCommand command = new SqlCommand(querystring, dataBase.GetConnection()))
                        {
                            enableIdentityInsertCommand.ExecuteNonQuery();

                            command.Parameters.AddWithValue("@id", freeID);
                            command.Parameters.AddWithValue("@login", login);
                            command.Parameters.AddWithValue("@password", password);
                            command.Parameters.AddWithValue("@rol", "пользователь");

                            if (command.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Аккаунт успешно создан!");

                                // Переадресация в кабинет пользователя
                                MainWindow mainWindow = new MainWindow();
                                mainWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка!", "Аккаунт не создан");
                            }

                            disableIdentityInsertCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        dataBase.closeConnection();
                    }
                }
                else
                {
                    dataBase.closeConnection();
                }
            }
        }



        private Boolean checkuser(string login)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable table = new DataTable();
            string querystring = "SELECT user_id, login FROM Users WHERE login = @login COLLATE SQL_Latin1_General_CP1_CS_AS";

            using (SqlCommand command = new SqlCommand(querystring, dataBase.GetConnection()))
            {
                command.Parameters.AddWithValue("@login", login);

                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(table);
            }

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Ошибка, такой пользователь уже существует!");
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetMinFreeUserID()
        {
            int minFreeID = 1; // Начнем с ID = 1

            try
            {
                string query = @"
                    SELECT ISNULL(MIN(T1.user_id + 1), 1)
                    FROM Users T1
                    LEFT JOIN Users T2 ON T1.user_id + 1 = T2.user_id
                    WHERE T2.user_id IS NULL";

                using (SqlCommand command = new SqlCommand(query, dataBase.GetConnection()))
                {
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        minFreeID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске минимального свободного ID: " + ex.Message);
            }

            return minFreeID;
        }



        //private Boolean checkuser(string login, string password, string rol)
        //{
        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
        //    DataTable table = new DataTable();
        //    string querystring = $"SELECT user_id, login, password, rol FROM Users WHERE login = @login COLLATE SQL_Latin1_General_CP1_CS_AS AND password = @password COLLATE SQL_Latin1_General_CP1_CS_AS AND rol = @rol";

        //    using (SqlCommand command = new SqlCommand(querystring, dataBase.GetConnection()))
        //    {
        //        command.Parameters.AddWithValue("@login", login);
        //        command.Parameters.AddWithValue("@password", password);
        //        command.Parameters.AddWithValue("@rol", rol);

        //        dataAdapter.SelectCommand = command;

        //        dataAdapter.Fill(table);
        //    }

        //    if (table.Rows.Count > 0)
        //    {
        //        MessageBox.Show("Ошибка, такой пользователь уже есть!!!");
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }
}
