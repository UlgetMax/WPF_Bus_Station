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
    /// Логика взаимодействия для PassengersView.xaml
    /// </summary>
    public partial class PassengersView : UserControl
    {
        DataBase_BusStation dataBase = new DataBase_BusStation();

        public PassengersView()
        {
            InitializeComponent();
            LoadPassengersData();
        }


        private void LoadPassengersData()
        {
            try
            {
                dataBase.openConnection();

                string query = "SELECT passazhir_id AS [ID], fio AS [ФИО], pasportnye_data AS [Паспортные данные], kontaknye_data AS [Контактные данные] FROM Passengers";
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

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataBase.openConnection();

                // Находим минимально возможный свободный ID
                string findMinFreeIdQuery = @"
                            SELECT ISNULL((SELECT MIN(T1.passazhir_id + 1) 
                            FROM Passengers T1 
                            LEFT JOIN Passengers T2 ON T1.passazhir_id + 1 = T2.passazhir_id 
                            WHERE T2.passazhir_id IS NULL), 1) AS FreeID";

                SqlCommand findMinFreeIdCommand = new SqlCommand(findMinFreeIdQuery, dataBase.GetConnection());
                int newId = (int)findMinFreeIdCommand.ExecuteScalar();

                // Включаем IDENTITY_INSERT, вставляем новую запись, затем отключаем IDENTITY_INSERT
                string enableIdentityInsertQuery = "SET IDENTITY_INSERT Passengers ON";
                string disableIdentityInsertQuery = "SET IDENTITY_INSERT Passengers OFF";
                string insertQuery = "INSERT INTO Passengers (passazhir_id, fio, pasportnye_data, kontaknye_data) VALUES (@ID, @FIO, @PassportData, @ContactData)";

                SqlCommand enableIdentityInsertCommand = new SqlCommand(enableIdentityInsertQuery, dataBase.GetConnection());
                SqlCommand disableIdentityInsertCommand = new SqlCommand(disableIdentityInsertQuery, dataBase.GetConnection());
                SqlCommand insertCommand = new SqlCommand(insertQuery, dataBase.GetConnection());

                insertCommand.Parameters.AddWithValue("@ID", newId);
                insertCommand.Parameters.AddWithValue("@FIO", textBoxFIO.Text);
                insertCommand.Parameters.AddWithValue("@PassportData", textBoxPassportData.Text);
                insertCommand.Parameters.AddWithValue("@ContactData", textBoxKontaknye_data.Text);

                enableIdentityInsertCommand.ExecuteNonQuery();
                insertCommand.ExecuteNonQuery();
                disableIdentityInsertCommand.ExecuteNonQuery();

                dataBase.closeConnection();
                LoadPassengersData();
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении данных: " + ex.Message);
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

                    string query = "UPDATE Passengers SET fio = @FIO, pasportnye_data = @PassportData, kontaknye_data = @ContactData WHERE passazhir_id = @ID";
                    SqlCommand sqlCommand = new SqlCommand(query, dataBase.GetConnection());
                    sqlCommand.Parameters.AddWithValue("@ID", row["ID"]);
                    sqlCommand.Parameters.AddWithValue("@FIO", textBoxFIO.Text);
                    sqlCommand.Parameters.AddWithValue("@PassportData", textBoxPassportData.Text);
                    sqlCommand.Parameters.AddWithValue("@ContactData", textBoxKontaknye_data.Text);

                    sqlCommand.ExecuteNonQuery();

                    dataBase.closeConnection();
                    LoadPassengersData();
                    ClearInputFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для обновления.");
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(textBoxIdDelete.Text, out int id))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы уверены, что хотите удалить запись? Это действие не может быть отменено.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dataBase.openConnection();

                        // Удаление связанных записей в таблице Tickets
                        string deleteTicketsQuery = "DELETE FROM Tickets WHERE passazhir_id = @ID";
                        SqlCommand deleteTicketsCommand = new SqlCommand(deleteTicketsQuery, dataBase.GetConnection());
                        deleteTicketsCommand.Parameters.AddWithValue("@ID", id);
                        deleteTicketsCommand.ExecuteNonQuery();

                        // Удаление записи в таблице Passengers
                        string deletePassengersQuery = "DELETE FROM Passengers WHERE passazhir_id = @ID";
                        SqlCommand deletePassengersCommand = new SqlCommand(deletePassengersQuery, dataBase.GetConnection());
                        deletePassengersCommand.Parameters.AddWithValue("@ID", id);
                        deletePassengersCommand.ExecuteNonQuery();

                        dataBase.closeConnection();
                        LoadPassengersData();
                        ClearInputFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Удаление отменено.");
                }
            }
            else
            {
                MessageBox.Show("Введите корректный ID.");
            }
        }



        private void ClearInputFields()
        {
            textBoxFIO.Text = string.Empty;
            textBoxPassportData.Text = string.Empty;
            textBoxKontaknye_data.Text = string.Empty;
            textBoxIdDelete.Text = string.Empty;
        }
    }
}
