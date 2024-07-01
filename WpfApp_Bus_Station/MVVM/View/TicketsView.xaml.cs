using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace WpfApp_Bus_Station.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для TicketsView.xaml
    /// </summary>
    public partial class TicketsView : UserControl
    {
        DataBase_BusStation dataBase = new DataBase_BusStation();
        public TicketsView()
        {
            InitializeComponent();
            LoadTicketsData();
            LoadComboBoxData();
        }

        private void LoadTicketsData()
        {
            try
            {
                dataBase.openConnection();

                string query = "SELECT bilet_id AS [ID], reis_id AS [ID_Рейса], passazhir_id AS [ID_Пассажира], user_id AS [ID_Пользователя], mesto AS [Место] FROM Tickets";
                SqlCommand sqlCommand = new SqlCommand(query, dataBase.GetConnection());
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridView.ItemsSource = dataTable.DefaultView;

                dataBase.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                dataBase.openConnection();

                // Load Reis IDs
                string queryReis = "SELECT reis_id FROM Flights";
                SqlCommand sqlCommandReis = new SqlCommand(queryReis, dataBase.GetConnection());
                SqlDataAdapter dataAdapterReis = new SqlDataAdapter(sqlCommandReis);
                DataTable dataTableReis = new DataTable();
                dataAdapterReis.Fill(dataTableReis);

                comboBoxReisID.ItemsSource = dataTableReis.DefaultView;
                comboBoxReisID.DisplayMemberPath = "reis_id";
                comboBoxReisID.SelectedValuePath = "reis_id";

                // Load Passazhir IDs and FIO
                string queryPassazhir = "SELECT passazhir_id, fio FROM Passengers";
                SqlCommand sqlCommandPassazhir = new SqlCommand(queryPassazhir, dataBase.GetConnection());
                SqlDataAdapter dataAdapterPassazhir = new SqlDataAdapter(sqlCommandPassazhir);
                DataTable dataTablePassazhir = new DataTable();
                dataAdapterPassazhir.Fill(dataTablePassazhir);

                comboBoxPassazhirID.ItemsSource = dataTablePassazhir.DefaultView;
                comboBoxPassazhirID.DisplayMemberPath = "fio";
                comboBoxPassazhirID.SelectedValuePath = "passazhir_id";

                // Load User IDs and Login
                string queryUser = "SELECT user_id, login FROM Users";
                SqlCommand sqlCommandUser = new SqlCommand(queryUser, dataBase.GetConnection());
                SqlDataAdapter dataAdapterUser = new SqlDataAdapter(sqlCommandUser);
                DataTable dataTableUser = new DataTable();
                dataAdapterUser.Fill(dataTableUser);

                comboBoxUserID.ItemsSource = dataTableUser.DefaultView;
                comboBoxUserID.DisplayMemberPath = "login";
                comboBoxUserID.SelectedValuePath = "user_id";

                dataBase.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных для ComboBox: " + ex.Message);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataBase.openConnection();

                string query = "INSERT INTO Tickets (reis_id, passazhir_id, user_id, mesto) VALUES (@ReisID, @PassazhirID, @UserID, @Mesto)";
                SqlCommand sqlCommand = new SqlCommand(query, dataBase.GetConnection());
                sqlCommand.Parameters.AddWithValue("@ReisID", comboBoxReisID.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@PassazhirID", comboBoxPassazhirID.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@UserID", comboBoxUserID.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Mesto", textBoxMesto.Text);

                sqlCommand.ExecuteNonQuery();

                dataBase.closeConnection();
                LoadTicketsData();
                ClearInputFields();

                //MessageBox.Show("Запись успешно добавлена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении записи: " + ex.Message);
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                try
                {
                    DataRowView row = (DataRowView)dataGridView.SelectedItem;
                    dataBase.openConnection();

                    string query = "UPDATE Tickets SET reis_id = @ReisID, passazhir_id = @PassazhirID, user_id = @UserID, mesto = @Mesto WHERE bilet_id = @ID";
                    SqlCommand sqlCommand = new SqlCommand(query, dataBase.GetConnection());

                    sqlCommand.Parameters.AddWithValue("@ID", row["ID"]);
                    sqlCommand.Parameters.AddWithValue("@ReisID", comboBoxReisID.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@PassazhirID", comboBoxPassazhirID.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@UserID", comboBoxUserID.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@Mesto", textBoxMesto.Text);

                    sqlCommand.ExecuteNonQuery();

                    dataBase.closeConnection();
                    LoadTicketsData();
                    ClearInputFields();

                    //MessageBox.Show("Запись успешно обновлена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении записи: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для обновления!");
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(textBoxBiletID.Text, out int id))
            {
                try
                {
                    dataBase.openConnection();

                    string query = "DELETE FROM Tickets WHERE bilet_id = @ID";
                    SqlCommand sqlCommand = new SqlCommand(query, dataBase.GetConnection());
                    sqlCommand.Parameters.AddWithValue("@ID", id);

                    sqlCommand.ExecuteNonQuery();

                    dataBase.closeConnection();
                    LoadTicketsData();
                    ClearInputFields();

                    //MessageBox.Show("Запись успешно удалена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Введите корректный ID.");
            }
        }

        private void ClearInputFields()
        {
            comboBoxReisID.SelectedIndex = -1;
            comboBoxPassazhirID.SelectedIndex = -1;
            comboBoxUserID.SelectedIndex = -1;
            textBoxMesto.Clear();
            textBoxBiletID.Clear();
        }
    }
}
