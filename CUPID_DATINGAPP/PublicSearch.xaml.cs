using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CUPID_DATINGAPP
{
    public partial class PublicSearch : UserControl
    {
        private ObservableCollection<PublicUser> Users = new ObservableCollection<PublicUser>();
        private string connectionString;

        public PublicSearch()
        {
            InitializeComponent();

            // Verbindungszeichenfolge aus App.config laden
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            UserListView.ItemsSource = Users;
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                await LoadUsersAsync(searchText);
            }
            else
            {
                Users.Clear(); // Liste leeren, wenn kein Text in der Suche ist
            }
        }

        private async Task LoadUsersAsync(string searchQuery)
        {
            try
            {
                Users.Clear(); // Alte Einträge löschen

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT UserID, Username, profile_photo FROM users WHERE Username LIKE @searchText";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@searchText", "%" + searchQuery + "%");

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var profilePhoto = reader.IsDBNull(reader.GetOrdinal("profile_photo"))
                                    ? null
                                    : (byte[])reader["profile_photo"]; // Profilbild als Byte-Array

                                Users.Add(new PublicUser
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    ProfilePhoto = profilePhoto
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Benutzerabfrage: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class PublicUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public byte[] ProfilePhoto { get; set; }
    }
}
