using System; // Grundlegende Funktionen wie Exception Handling und Datum/Zeit.
using System.Windows; // WPF-Bibliothek für Fenster, Nachrichten und Benutzeroberfläche.
using System.Windows.Controls; // WPF-Steuerelemente wie UserControl und TextBlock.
using MySql.Data.MySqlClient; // Bibliothek für den Zugriff auf MySQL-Datenbanken.

namespace CUPID_DATINGAPP
{
    public partial class HomeWindow : UserControl
    {
        // Verbindung zur MySQL-Datenbank, definiert in der App-Konfiguration.
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // ID des aktuell angezeigten Benutzers (für Matches und Drops).
        private int currentDisplayedUserId = -1;

        public HomeWindow()
        {
            InitializeComponent(); // Initialisiert die grafischen Komponenten.
            LoadUserProfile(); // Lädt ein Benutzerprofil beim Öffnen des Fensters.
        }

        /// <summary>
        /// Lädt ein Benutzerprofil aus der Datenbank.
        /// Schließt Benutzer aus, die bereits geliked oder gedroppt wurden.
        /// </summary>
        private void LoadUserProfile()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT UserID, FirstName, Biography, Hobbys, Skills, DateOfBirth 
                FROM users 
                WHERE UserID != @LoggedInUserId 
                AND Gender = @TargetGender
                AND UserID NOT IN (
                    SELECT User_Id_matches_2 FROM matches WHERE User_Id_matches_1 = @LoggedInUserId
                    UNION
                    SELECT User_Id_drop_2 FROM drops WHERE User_Id_drop_1 = @LoggedInUserId
                )
                ORDER BY RAND() LIMIT 1;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LoggedInUserId", UserSync.CurrentUser.Instance.UserId);
                        cmd.Parameters.AddWithValue("@TargetGender", DetermineTargetGender());

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Benutzer gefunden: Fülle die UI
                                currentDisplayedUserId = Convert.ToInt32(reader["UserID"]);
                                TextBlockFirstName.Text = reader["FirstName"].ToString();
                                AboutMeText.Text = reader["Biography"].ToString();
                                HobbiesText.Text = reader["Hobbys"].ToString();
                                SkillsText.Text = reader["Skills"].ToString();

                                if (DateTime.TryParse(reader["DateOfBirth"].ToString(), out DateTime dateOfBirth))
                                {
                                    TextBlockDateOfBirth.Text = $"Geburtsdatum: {dateOfBirth:dd.MM.yyyy}";
                                }
                                else
                                {
                                    TextBlockDateOfBirth.Text = "Geburtsdatum: Unbekannt";
                                }
                            }
                            else
                            {
                                // Keine Benutzer gefunden
                                ShowNoMoreUsersScreen();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Benutzerdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void ShowNoMoreUsersScreen()
        {
            // Zeige eine Nachricht an
            MessageBox.Show("Es gibt momentan keine weiteren passenden Benutzer. Schau später noch einmal vorbei!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optional: Leere die Benutzeroberfläche oder zeige ein Bild/Animation
            TextBlockFirstName.Text = "Keine weiteren Benutzer verfügbar";
            AboutMeText.Text = "";
            HobbiesText.Text = "";
            SkillsText.Text = "";
            TextBlockDateOfBirth.Text = "";

            // Du kannst auch eine Animation oder ein Bild anzeigen, falls gewünscht
            // Example: Show a placeholder image
            // PlaceholderImage.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Event-Handler für den "Gefällt mir"-Button.
        /// Erstellt ein Match in der Datenbank.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                INSERT INTO matches (User_Id_matches_1, User_Id_matches_2) 
                VALUES (@UserId1, @UserId2);";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId1", UserSync.CurrentUser.Instance.UserId); // Eingeloggter Benutzer
                        cmd.Parameters.AddWithValue("@UserId2", currentDisplayedUserId); // Angezeigter Benutzer

                        cmd.ExecuteNonQuery();
                    }

                    // Lade den nächsten Benutzer
                    LoadUserProfile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Erstellen eines Matches: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Event-Handler für den "Ablehnen"-Button.
        /// Erstellt einen Eintrag in der Drops-Tabelle.
        /// </summary>
        /// <summary>
        /// Event-Handler für den "Ablehnen"-Button.
        /// Erstellt einen Eintrag in der Drops-Tabelle.
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                INSERT INTO drops (User_Id_drop_1, User_Id_drop_2) 
                VALUES (@UserId1, @UserId2);";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId1", UserSync.CurrentUser.Instance.UserId); // Eingeloggter Benutzer
                        cmd.Parameters.AddWithValue("@UserId2", currentDisplayedUserId); // Angezeigter Benutzer

                        cmd.ExecuteNonQuery();
                    }

                    // Lade den nächsten Benutzer
                    LoadUserProfile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Droppen eines Benutzers: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Bestimmt das Zielgeschlecht basierend auf den Präferenzen des Benutzers.
        /// </summary>
        /// <returns>Das Zielgeschlecht als String.</returns>
        private string DetermineTargetGender()
        {
            string targetAudience = UserSync.CurrentUser.Instance.TargetAudience;
            string userGender = UserSync.CurrentUser.Instance.Gender;

            if (targetAudience == "männlich")
                return "männlich";
            else if (targetAudience == "weiblich")
                return "weiblich";
            else
                return userGender == "männlich" ? "weiblich" : "männlich"; // Standard: Zeige das andere Geschlecht
        }
    }
}
