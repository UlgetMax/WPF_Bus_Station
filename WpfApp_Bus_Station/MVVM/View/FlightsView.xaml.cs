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
    /// Логика взаимодействия для FlightsView.xaml
    /// </summary>
    public partial class FlightsView : UserControl
    {
        DataBase_BusStation dataBase = new DataBase_BusStation();

        public FlightsView()
        {
            InitializeComponent();
            LoadFlightsData();
            PopulateTimeComboBoxes();
        }

        private void LoadFlightsData()
        {
            try
            {
                dataBase.openConnection();

                string query = "SELECT reis_id AS [ID_Рейса], num_reis AS [Номер рейса], punkt_otpravlenia AS [Пункт отправки], punkt_naznachenia AS [Пункт назначения], data_reis AS [Дата рейса], vremya_otpravlenia AS [Время отправки], vremya_pribytia AS [Время прибытия], stoimost_bileta AS [Стоимость билета], kolvo_mest AS [Кол-во мест], status_reis AS [Статус рейса] FROM Flights";
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

        private void PopulateTimeComboBoxes()
        {
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 15)
                {
                    string time = $"{hour:D2}:{minute:D2}";
                    comboBoxTimeOtpravl.Items.Add(time);
                    comboBoxTimePrib.Items.Add(time);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataBase.openConnection();

                string findMinFreeIdQuery = @"
                    SELECT ISNULL((SELECT MIN(T1.reis_id + 1) 
                                   FROM Flights T1 
                                   LEFT JOIN Flights T2 ON T1.reis_id + 1 = T2.reis_id 
                                   WHERE T2.reis_id IS NULL), 1) AS FreeID";

                SqlCommand findMinFreeIdCommand = new SqlCommand(findMinFreeIdQuery, dataBase.GetConnection());
                int newId = (int)findMinFreeIdCommand.ExecuteScalar();

                string enableIdentityInsertQuery = "SET IDENTITY_INSERT Flights ON";
                string disableIdentityInsertQuery = "SET IDENTITY_INSERT Flights OFF";
                string insertQuery = "INSERT INTO Flights (reis_id, num_reis, punkt_otpravlenia, punkt_naznachenia, data_reis, vremya_otpravlenia, vremya_pribytia, stoimost_bileta, kolvo_mest, status_reis) VALUES (@ID, @NumReis, @PunktOtpravlenia, @PunktNaznachenia, @DataReis, @TimeOtpravl, @TimePrib, @StoimostBileta, @KolvoMest, @StatusReis)";

                SqlCommand enableIdentityInsertCommand = new SqlCommand(enableIdentityInsertQuery, dataBase.GetConnection());
                SqlCommand disableIdentityInsertCommand = new SqlCommand(disableIdentityInsertQuery, dataBase.GetConnection());
                SqlCommand insertCommand = new SqlCommand(insertQuery, dataBase.GetConnection());

                insertCommand.Parameters.AddWithValue("@ID", newId);
                insertCommand.Parameters.AddWithValue("@NumReis", textBoxNumReis.Text);
                insertCommand.Parameters.AddWithValue("@PunktOtpravlenia", textBoxPunktOtpravlenia.Text);
                insertCommand.Parameters.AddWithValue("@PunktNaznachenia", textBoxPunktNaznachenia.Text);
                insertCommand.Parameters.AddWithValue("@DataReis", datePickerDataReis.SelectedDate.HasValue ? (object)datePickerDataReis.SelectedDate.Value.Date : DBNull.Value);
                insertCommand.Parameters.AddWithValue("@TimeOtpravl", comboBoxTimeOtpravl.Text);
                insertCommand.Parameters.AddWithValue("@TimePrib", comboBoxTimePrib.Text);
                insertCommand.Parameters.AddWithValue("@StoimostBileta", textBoxStoimostBileta.Text);
                insertCommand.Parameters.AddWithValue("@KolvoMest", textBoxKolvoMest.Text);
                insertCommand.Parameters.AddWithValue("@StatusReis", ((ComboBoxItem)comboBoxStatusReis.SelectedItem).Content.ToString());

                enableIdentityInsertCommand.ExecuteNonQuery();
                insertCommand.ExecuteNonQuery();
                disableIdentityInsertCommand.ExecuteNonQuery();

                dataBase.closeConnection();
                LoadFlightsData();
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добалении записи: " + ex.Message);
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

                    string query = "UPDATE Flights SET num_reis = @NumReis, punkt_otpravlenia = @PunktOtpravlenia, punkt_naznachenia = @PunktNaznachenia, data_reis = @DataReis, vremya_otpravlenia = @TimeOtpravl, vremya_pribytia = @TimePrib, stoimost_bileta = @StoimostBileta, kolvo_mest = @KolvoMest, status_reis = @StatusReis WHERE reis_id = @ID";

                    SqlCommand sqlCommand = new SqlCommand(query, dataBase.GetConnection());

                    sqlCommand.Parameters.AddWithValue("@ID", row["ID_Рейса"]);
                    sqlCommand.Parameters.AddWithValue("@NumReis", textBoxNumReis.Text);
                    sqlCommand.Parameters.AddWithValue("@PunktOtpravlenia", textBoxPunktOtpravlenia.Text);
                    sqlCommand.Parameters.AddWithValue("@PunktNaznachenia", textBoxPunktNaznachenia.Text);
                    sqlCommand.Parameters.AddWithValue("@DataReis", datePickerDataReis.SelectedDate.HasValue ? (object)datePickerDataReis.SelectedDate.Value.Date : DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@TimeOtpravl", comboBoxTimeOtpravl.Text);
                    sqlCommand.Parameters.AddWithValue("@TimePrib", comboBoxTimePrib.Text);
                    sqlCommand.Parameters.AddWithValue("@StoimostBileta", textBoxStoimostBileta.Text);
                    sqlCommand.Parameters.AddWithValue("@KolvoMest", textBoxKolvoMest.Text);
                    sqlCommand.Parameters.AddWithValue("@StatusReis", ((ComboBoxItem)comboBoxStatusReis.SelectedItem).Content.ToString());

                    sqlCommand.ExecuteNonQuery();

                    dataBase.closeConnection();
                    LoadFlightsData();
                    ClearInputFields();

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
            if (int.TryParse(textBoxIdDelete.Text, out int id))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы уверены, что хотите удалить запись? Это также удалит связанные записи в таблице Tickets.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        dataBase.openConnection();

                        // Удаление связанных записей в таблице Tickets
                        string deleteTicketsQuery = "DELETE FROM Tickets WHERE reis_id = @ID";
                        SqlCommand deleteTicketsCommand = new SqlCommand(deleteTicketsQuery, dataBase.GetConnection());
                        deleteTicketsCommand.Parameters.AddWithValue("@ID", id);
                        deleteTicketsCommand.ExecuteNonQuery();

                        // Удаление записи в таблице Flights
                        string deleteFlightsQuery = "DELETE FROM Flights WHERE reis_id = @ID";
                        SqlCommand deleteFlightsCommand = new SqlCommand(deleteFlightsQuery, dataBase.GetConnection());
                        deleteFlightsCommand.Parameters.AddWithValue("@ID", id);
                        deleteFlightsCommand.ExecuteNonQuery();

                        dataBase.closeConnection();
                        LoadFlightsData();

                        ClearInputFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении записи: " + ex.Message);
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
            textBoxNumReis.Text = string.Empty;
            textBoxPunktOtpravlenia.Text = string.Empty;
            textBoxPunktNaznachenia.Text = string.Empty;
            textBoxStoimostBileta.Text = string.Empty;
            comboBoxTimeOtpravl.SelectedItem = null;
            comboBoxTimePrib.SelectedItem = null;
            textBoxKolvoMest.Text = string.Empty;
            textBoxIdDelete.Text = string.Empty;
            datePickerDataReis.SelectedDate = null;
        }
    }
}
