using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Contact_Manager_Final_Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var connString = new SqliteConnectionStringBuilder
            {
                DataSource = "contactDatabase.db"
            }.ToString();

            Trace.WriteLine($"Connection: {connString}");

            using (var conn = new SqliteConnection(connString))
            {
                conn.Open();
                Trace.WriteLine($"Connection established");

                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"
                        CREATE TABLE IF NOT EXISTS Contacts
                        (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT NOT NULL,
                            LastName TEXT NOT NULL,
                            Email TEXT NOT NULL,
                            PhoneNumber TEXT NOT NULL
                        );
                    ";
                cmd.ExecuteNonQuery();

                Trace.WriteLine("Table created!");
            }
        }


        class Contact
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }

        private void AddContact_btn_Click(object sender, RoutedEventArgs e)
        {
            Contact newContact = new Contact();

            newContact.FirstName = FirstName_txt.Text;
            newContact.LastName = LastName_txt.Text;
            newContact.Email = Email_txt.Text;
            newContact.PhoneNumber = PhoneNumber_txt.Text;

            AddToDatabase(newContact);
        }

        private void AddToDatabase(Contact contact)
        {
            var connString = new SqliteConnectionStringBuilder
            {
                DataSource = "contactDatabase.db"
            }.ToString();

            using (var conn = new SqliteConnection(connString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"
                        INSERT INTO Contacts (FirstName, LastName, Email, PhoneNumber)
                        VALUES (@FirstName, @LastName, @Email, @PhoneNumber);
                    ";

                cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
                cmd.Parameters.AddWithValue("@LastName", contact.LastName);
                cmd.Parameters.AddWithValue("@Email", contact.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);

                cmd.ExecuteNonQuery();
                Trace.WriteLine("Contact Added!");


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


                        Trace.WriteLine($"First Name: {firstName} Last Name: {lastName} E-Mail: {email} Phone Number: {phoneNumber}");
                    }
                }

            }
        }

        private void FindByFirstName_btn_Click(object sender, RoutedEventArgs e)
        {
            string contactToFind = EditContact_txt.Text;
            DisplayAll_lbl.Content = "";

            if (contactToFind != null)
            {
                var connString = new SqliteConnectionStringBuilder
                {
                    DataSource = "contactDatabase.db"
                }.ToString();

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    var cmdReadData = conn.CreateCommand();
                    cmdReadData.CommandText =
                        $@"
                        SELECT *
                        FROM Contacts
                        Where Firstname = '{contactToFind}';
                    ";

                    using (var reader = cmdReadData.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var firstName = reader.GetString(1);
                            var lastName = reader.GetString(2);
                            var email = reader.GetString(3);
                            var phoneNumber = reader.GetString(4);


                            DisplayAll_lbl.Content += $"Contact found:\nFirst Name: {firstName} | Last Name: {lastName} | E-mail: {email} | Phone Number: {phoneNumber}";
                        }
                    }
                }

                if (DisplayAll_lbl.Content == string.Empty)
                {
                    DisplayAll_lbl.Content = "Contact not found.";
                }
            }
            else
            {

            }
        }

        private void EditContact_btn_Click(object sender, RoutedEventArgs e)
        {
            string contactToFind = EditContact_txt.Text;
            string Fn = "";
            string Ln = "";
            string Em = "";
            string Ph = "";

            if (contactToFind != null)
            {
                var connString = new SqliteConnectionStringBuilder
                {
                    DataSource = "contactDatabase.db"
                }.ToString();

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    var cmdReadData = conn.CreateCommand();
                    cmdReadData.CommandText =
                        $@"
                        SELECT *
                        FROM Contacts
                        Where FirstName = '{contactToFind}';
                    ";

                    using (var reader = cmdReadData.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Fn = reader.GetString(1);
                            Ln = reader.GetString(2);
                            Em = reader.GetString(3);
                            Ph = reader.GetString(4);

                        }
                    }


                    if (!string.IsNullOrEmpty(FirstName_txt.Text))
                    {
                        Fn = FirstName_txt.Text;
                    }
                    if (!string.IsNullOrEmpty(LastName_txt.Text))
                    {
                        Ln = LastName_txt.Text;
                    }
                    if (!string.IsNullOrEmpty(Email_txt.Text))
                    {
                        Em = Email_txt.Text;
                    }
                    if (!string.IsNullOrEmpty(PhoneNumber_txt.Text))
                    {
                        Ph = PhoneNumber_txt.Text;
                    }
                    EditContactDatabase(contactToFind, Fn, Ln, Em, Ph);
                }
            }
            else
            {
                MessageBox.Show("Textbox is empty.");
            }
        }

        private void EditContactDatabase(string contactToEdit, string Fn, string Ln, string Em, string Ph)
        {
            
            var connString = new SqliteConnectionStringBuilder
            {
                DataSource = "contactDatabase.db"
            }.ToString();

            using (var conn = new SqliteConnection(connString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    $@"
                UPDATE Contacts
                SET FirstName = '{Fn}',
                    LastName = '{Ln}',
                    Email = '{Em}',
                    PhoneNumber = '{Ph}'
                WHERE FirstName = '{contactToEdit}';
            ";

                cmd.ExecuteNonQuery();
                Trace.WriteLine("Contact Updated in Database!");
            }
        }

        private void DeleteContact_btn_Click(object sender, RoutedEventArgs e)
        {
            string contactToFind = EditContact_txt.Text;

            if (contactToFind != null)
            {
                var connString = new SqliteConnectionStringBuilder
                {
                    DataSource = "contactDatabase.db"
                }.ToString();

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText =
                        $@"
                            DELETE FROM Contacts
                            WHERE FirstName = '{contactToFind}';
                        ";

                    cmd.ExecuteNonQuery();
                    Trace.WriteLine("Contact Removed from Database!");
                }
            }
            else
            {
                MessageBox.Show("Textbox is empty.");
            }
        }

        private void DisplayAll_btn_Click(object sender, RoutedEventArgs e)
        {
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

        private void CSV_btn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = (CSV_txt.Text).Trim('"');

            string firstName = "";
            string lastName = "";
            string email = "";
            string phoneNumber = "";

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        string[] values = line.Split(',');

                        firstName = values[0];
                        lastName = values[1];
                        email = values[2];
                        phoneNumber = values[3];
                    }
                }
                var connString = new SqliteConnectionStringBuilder
                {
                    DataSource = "contactDatabase.db"
                }.ToString();

                using (var conn = new SqliteConnection(connString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText =
                        @"
                        INSERT INTO Contacts (FirstName, LastName, Email, PhoneNumber)
                        VALUES (@FirstName, @LastName, @Email, @PhoneNumber);
                    ";

                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    cmd.ExecuteNonQuery();
                    Trace.WriteLine("Contact Added!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing contacts: {ex.Message}");
            }
        }

        private void Export_btn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = (CSV_txt.Text).Trim('"');

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

                        try
                        {
                            using (StreamWriter sw = new StreamWriter(filePath))
                            {
                                sw.Write($"{firstName},{lastName},{email},{phoneNumber}\n");

                                Console.WriteLine("Data written to the file successfully.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void NewWindow_btn_Click(object sender, RoutedEventArgs e)
        {
            DetailsWindow detailsWindow = new DetailsWindow();
            detailsWindow.Show();

            this.Hide();
        }
    }
}
