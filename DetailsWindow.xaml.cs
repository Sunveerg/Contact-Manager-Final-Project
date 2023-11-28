using Microsoft.Data.Sqlite;
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

namespace Contact_Manager_Final_Project
{
    public partial class DetailsWindow : Window
    {
        public DetailsWindow()
        {
            InitializeComponent();

            DisplayAll_lbl.Content = string.Empty;
            var connString = new SqliteConnectionStringBuilder
            {
                DataSource = "contactDatabase.db"
            }.ToString();

            using (var conn = new SqliteConnection(connString))
            {
                conn.Open();
                var cmdReadData = conn.CreateCommand();
                cmdReadData.CommandText =
                    @"
                        SELECT *
                        FROM Contacts
                    ";

                using (var reader = cmdReadData.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var firstName = reader.GetString(1);
                        var lastName = reader.GetString(2);
                        var email = reader.GetString(3);
                        var phoneNumber = reader.GetString(4);


                        DisplayAll_lbl.Content += $"First Name: {firstName} | Last Name: {lastName} | E-mail: {email} | Phone Number: {phoneNumber}\n";
                    }
                }
            }
        }

        private void Return_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }
    }
}
