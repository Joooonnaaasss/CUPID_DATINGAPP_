using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

            // Setze die ItemSource für die ListView
            UserListView.ItemsSource = Users;

            // Lade Matches beim Start
            _ = LoadMatchesAsync();
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
                Users.Clear(); // Matches erneut laden, wenn kein Suchtext eingegeben ist
                await LoadMatchesAsync();
            }
        }

        private async Task LoadMatchesAsync()
        {
            try
            {
                Users.Clear(); // Alte Einträge löschen

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                        SELECT u.UserID, u.Username, u.profile_photo 
                        FROM users u
                        INNER JOIN matches m ON u.UserID = m.User_Id_matches_2
                        WHERE m.User_Id_matches_1 = @LoggedInUserId";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LoggedInUserId", UserSync.CurrentUser.Instance.UserId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Users.Add(new PublicUser
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    ProfilePhoto = reader.IsDBNull(reader.GetOrdinal("profile_photo"))
                                        ? null
                                        : (byte[])reader["profile_photo"]
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
                                Users.Add(new PublicUser
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    ProfilePhoto = reader.IsDBNull(reader.GetOrdinal("profile_photo"))
                                        ? null
                                        : (byte[])reader["profile_photo"]
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

        public BitmapImage ProfileImage => ConvertToImage(ProfilePhoto);

        private BitmapImage ConvertToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            using (var ms = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}
