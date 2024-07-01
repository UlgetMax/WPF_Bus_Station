using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

namespace WpfApp_Bus_Station
{
    internal class DataBase_BusStation
    {

        SqlConnection sqlConnection = new SqlConnection(@"Data Source = DESKTOP-H0C1G4O\SQL2022; Initial Catalog = Dispetcher_Bus_Station; Integrated Security = True");


        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }


        public bool TestConnection()
        {
            try
            {
                openConnection();
                closeConnection();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
                return false;
            }
        }

    }
}
