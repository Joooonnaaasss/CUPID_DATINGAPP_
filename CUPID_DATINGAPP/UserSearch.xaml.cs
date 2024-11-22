using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CUPID_DATINGAPP
{
    public partial class UserSearch : UserControl
    {
        private ObservableCollection<PublicUser> Matches = new ObservableCollection<PublicUser>();
        private string connectionString;

        public UserSearch()
        {
            InitializeComponent();

            // Verbindungszeichenfolge aus App.config laden
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            // Setze die ItemSource der ListView
            UserListView.ItemsSource = Matches;

            // Lade Matches, wenn die Ansicht geöffnet wird
            _ = LoadAllMatchesAsync();
        }

        // Lade alle Matches aus der Datenbank
        private async Task LoadAllMatchesAsync()
        {
            try
            {
                Matches.Clear(); // Vorherige Einträge entfernen

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // SQL-Abfrage, um alle Matches zu laden
                    string query = @"
                        SELECT DISTINCT u.UserID, u.Username
                        FROM users u
                        INNER JOIN matches m ON u.UserID = m.User_Id_matches_1 OR u.UserID = m.User_Id_matches_2";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Matches.Add(new PublicUser
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Matches: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
