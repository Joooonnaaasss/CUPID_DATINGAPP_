using System; // Grundlegende .NET-Bibliothek, bietet Funktionen wie Exception-Handling, Datum/Zeit und andere grundlegende Dienste.
using System.Windows; // Bibliothek für die Benutzeroberfläche (UI) in WPF (Windows Presentation Foundation), die Fenster und Steuerelemente bereitstellt.
using System.Windows.Controls; // WPF-Steuerelemente, wie UserControl und TextBlock, die zur Erstellung der Benutzeroberfläche verwendet werden.
using System.Windows.Media.Imaging; // Bibliothek zum Arbeiten mit Bitmap-Images, etwa für das Laden und Anzeigen von Bildern in der Anwendung.
using MySql.Data.MySqlClient; // Bibliothek, um auf MySQL-Datenbanken zuzugreifen und SQL-Befehle auszuführen.
using System.IO; // Bibliothek für Datei- und Streamoperationen, hier speziell zur Umwandlung von Bytearrays in Streams.

namespace CUPID_DATINGAPP
{
    // Definiert eine Benutzeroberfläche für das Home-Fenster der Dating-App.
    public partial class HomeWindow : UserControl
    {
        // Verbindungszeichenfolge zur MySQL-Datenbank. Diese wird aus der App-Konfiguration geladen.
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // Variable zum Speichern der ID des aktuell angezeigten Benutzers. Initialwert -1, falls kein Benutzer angezeigt wird.
        private int currentDisplayedUserId = -1;

        // Konstruktor der HomeWindow-Klasse, der beim Öffnen des Home-Fensters aufgerufen wird.
        public HomeWindow()
        {
            InitializeComponent(); // Initialisiert alle grafischen Komponenten, die für die Benutzeroberfläche definiert sind.
            LoadUserProfile(); // Ruft die Methode auf, um ein Benutzerprofil zu laden, wenn das Fenster geöffnet wird.
        }

        /// <summary>
        /// Lädt ein Benutzerprofil aus der MySQL-Datenbank. Es werden Benutzer ausgeschlossen, die bereits vom eingeloggten Benutzer geliked oder abgelehnt wurden.
        /// </summary>
        private void LoadUserProfile()
        {
            try
            {
                // Verwende eine "using"-Anweisung, um sicherzustellen, dass die Datenbankverbindung korrekt geschlossen wird.
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Öffne die Verbindung zur MySQL-Datenbank.

                    // SQL-Abfrage, um ein Benutzerprofil zu laden, das noch nicht vom aktuellen Benutzer geliked oder abgelehnt wurde.
                    string query = @"
                    SELECT UserID, FirstName, Biography, Hobbys, Skills, DateOfBirth, profile_photo
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
                        // Setze die Parameter für die SQL-Abfrage, um SQL-Injection zu vermeiden.
                        cmd.Parameters.AddWithValue("@LoggedInUserId", UserSync.CurrentUser.Instance.UserId); // ID des eingeloggten Benutzers.
                        cmd.Parameters.AddWithValue("@TargetGender", DetermineTargetGender()); // Geschlechtspräferenz des Benutzers.

                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Führt die Abfrage aus und liest die Ergebnismenge.
                        {
                            if (reader.Read()) // Überprüft, ob ein Benutzer gefunden wurde.
                            {
                                currentDisplayedUserId = Convert.ToInt32(reader["UserID"]); // Speichert die ID des angezeigten Benutzers.

                                // Versuche, das Geburtsdatum zu lesen und das Alter des Benutzers zu berechnen.
                                if (DateTime.TryParse(reader["DateOfBirth"].ToString(), out DateTime dateOfBirth))
                                {
                                    int age = CalculateAge(dateOfBirth); // Berechne das Alter.
                                    TextBlockFirstName.Text = $"{reader["FirstName"]}, {age}"; // Setze den Vornamen und das Alter des Benutzers im UI.
                                    TextBlockDateOfBirth.Text = $"Geburtsdatum: {dateOfBirth:dd.MM.yyyy}"; // Setze das Geburtsdatum im UI.
                                }
                                else
                                {
                                    // Falls das Geburtsdatum nicht verfügbar ist, setze nur den Vornamen.
                                    TextBlockFirstName.Text = reader["FirstName"].ToString();
                                    TextBlockDateOfBirth.Text = "Geburtsdatum: Unbekannt";
                                }

                                // Setze die restlichen Benutzerinformationen (Biographie, Hobbys, Fähigkeiten).
                                AboutMeText.Text = reader["Biography"].ToString();
                                HobbiesText.Text = reader["Hobbys"].ToString();
                                SkillsText.Text = reader["Skills"].ToString();

                                // Überprüfe, ob ein Profilbild vorhanden ist und lade es.
                                if (!reader.IsDBNull(reader.GetOrdinal("profile_photo")))
                                {
                                    byte[] imageBytes = (byte[])reader["profile_photo"]; // Byte-Array für das Bild.
                                    if (imageBytes != null && imageBytes.Length > 0) // Stelle sicher, dass das Bild nicht leer ist.
                                    {
                                        using (MemoryStream ms = new MemoryStream(imageBytes))
                                        {
                                            ms.Position = 0; // Setze den Stream-Zeiger auf den Anfang des Streams.
                                            BitmapImage bitmap = new BitmapImage();
                                            bitmap.BeginInit(); // Initialisiere das Bitmap-Objekt.
                                            bitmap.StreamSource = ms; // Setze den Stream als Quelle des Bitmaps.
                                            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Lade das Bitmap komplett in den Speicher.
                                            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat; // Behalte das Pixel-Format bei.
                                            bitmap.EndInit(); // Beende die Initialisierung des Bitmap-Objekts.
                                            bitmap.Freeze(); // Stelle das Bitmap-Objekt für den Zugriff von verschiedenen Threads bereit.
                                            ProfilePhotoImage.Source = bitmap; // Setze das Bitmap als Quelle für das Profilbild im UI.
                                        }
                                    }
                                }
                                else
                                {
                                    // Setze ein Standardbild, falls kein Profilbild vorhanden ist.
                                    ProfilePhotoImage.Source = new BitmapImage(new Uri("pack://application:,,,/CUPID_DATINGAPP;component/Pictures/profil.png"));
                                }
                            }
                            else
                            {
                                // Falls kein weiterer Benutzer gefunden wurde, zeige eine entsprechende Nachricht an.
                                ShowNoMoreUsersScreen();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung: Zeige eine Nachricht an, falls das Laden der Benutzerdaten fehlschlägt.
                MessageBox.Show($"Fehler beim Laden der Benutzerdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Berechnet das Alter basierend auf dem Geburtsdatum.
        /// </summary>
        /// <param name="birthDate">Das Geburtsdatum des Benutzers.</param>
        /// <returns>Das Alter in Jahren.</returns>
        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today; // Hol das heutige Datum.
            int age = today.Year - birthDate.Year; // Berechne das vorläufige Alter.
            if (birthDate.Date > today.AddYears(-age)) age--; // Falls der Geburtstag im aktuellen Jahr noch nicht war, reduziere das Alter um eins.
            return age; // Gib das berechnete Alter zurück.
        }

        /// <summary>
        /// Zeigt eine Nachricht an, wenn keine weiteren Benutzer gefunden wurden.
        /// </summary>
        private void ShowNoMoreUsersScreen()
        {
            // Zeige eine Nachricht an, dass keine weiteren Benutzer verfügbar sind.
            MessageBox.Show("Es gibt momentan keine weiteren passenden Benutzer. Schau später noch einmal vorbei!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            // Setze die Benutzeroberfläche zurück, um anzuzeigen, dass keine weiteren Benutzer verfügbar sind.
            TextBlockFirstName.Text = "Keine weiteren Benutzer verfügbar";
            AboutMeText.Text = "";
            HobbiesText.Text = "";
            SkillsText.Text = "";
            TextBlockDateOfBirth.Text = "";
            ProfilePhotoImage.Source = null; // Setze das Profilbild zurück.
        }

        /// <summary>
        /// Event-Handler für den "Gefällt mir"-Button. Erstellt einen Eintrag in der Matches-Tabelle in der Datenbank.
        /// </summary>
        private void OnLikeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Öffne die Datenbankverbindung.

                    // SQL-Befehl zum Einfügen eines neuen Matches in die Datenbank.
                    string query = @"
                    INSERT INTO matches (User_Id_matches_1, User_Id_matches_2) 
                    VALUES (@UserId1, @UserId2);";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Setze die Parameter für die SQL-Abfrage.
                        cmd.Parameters.AddWithValue("@UserId1", UserSync.CurrentUser.Instance.UserId); // ID des eingeloggten Benutzers.
                        cmd.Parameters.AddWithValue("@UserId2", currentDisplayedUserId); // ID des aktuell angezeigten Benutzers.

                        cmd.ExecuteNonQuery(); // Führe die Einfügeoperation in der Datenbank aus.
                    }

                    // Lade das nächste Benutzerprofil, nachdem ein Match erstellt wurde.
                    LoadUserProfile();
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung: Zeige eine Nachricht an, falls das Erstellen des Matches fehlschlägt.
                MessageBox.Show($"Fehler beim Erstellen eines Matches: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event-Handler für den "Ablehnen"-Button. Erstellt einen Eintrag in der Drops-Tabelle in der Datenbank.
        /// </summary>
        private void OnDislikeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Öffne die Datenbankverbindung.

                    // SQL-Befehl zum Einfügen eines neuen Drops in die Datenbank.
                    string query = @"
                    INSERT INTO drops (User_Id_drop_1, User_Id_drop_2) 
                    VALUES (@UserId1, @UserId2);";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Setze die Parameter für die SQL-Abfrage.
                        cmd.Parameters.AddWithValue("@UserId1", UserSync.CurrentUser.Instance.UserId); // ID des eingeloggten Benutzers.
                        cmd.Parameters.AddWithValue("@UserId2", currentDisplayedUserId); // ID des aktuell angezeigten Benutzers.

                        cmd.ExecuteNonQuery(); // Führe die Einfügeoperation in der Datenbank aus.
                    }

                    // Lade das nächste Benutzerprofil, nachdem ein Drop erstellt wurde.
                    LoadUserProfile();
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung: Zeige eine Nachricht an, falls das Droppen des Benutzers fehlschlägt.
                MessageBox.Show($"Fehler beim Droppen eines Benutzers: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bestimmt das Zielgeschlecht des nächsten Benutzers basierend auf den Präferenzen des aktuellen Benutzers.
        /// </summary>
        /// <returns>Das Zielgeschlecht als String ("männlich" oder "weiblich").</returns>
        private string DetermineTargetGender()
        {
            string targetAudience = UserSync.CurrentUser.Instance.TargetAudience; // Präferenzen des aktuellen Benutzers (männlich, weiblich oder beides).
            string userGender = UserSync.CurrentUser.Instance.Gender; // Geschlecht des eingeloggten Benutzers.

            if (targetAudience == "männlich")
                return "männlich"; // Benutzer präferiert männliche Benutzer.
            else if (targetAudience == "weiblich")
                return "weiblich"; // Benutzer präferiert weibliche Benutzer.
            else
                return userGender == "männlich" ? "weiblich" : "männlich"; // Wenn keine explizite Präferenz vorliegt, zeige das andere Geschlecht.
        }
    }
}
