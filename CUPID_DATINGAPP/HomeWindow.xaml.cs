
using System; // Grundlegende Funktionen wie Exception Handling und Datum/Zeit.
using System.Windows; // WPF-Bibliothek für Fenster, Nachrichten und Benutzeroberfläche.
using System.Windows.Controls; // Stellt WPF-Steuerelemente wie UserControl und TextBlock bereit.
using MySql.Data.MySqlClient; // Bibliothek für den Zugriff auf MySQL-Datenbanken.

namespace CUPID_DATINGAPP
{
    public partial class HomeWindow : UserControl // HomeWindow ist ein Benutzersteuerelement (Teil der GUI).
    {
        // Verbindung zur Datenbank wird aus der Datei "app.config" geladen.
        // Die Verbindung nutzt den Namen "MySqlConnection", wie in app.config definiert.
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public HomeWindow()
        {
            InitializeComponent(); // Initialisiert die grafischen Komponenten (geladen aus XAML).
            LoadUserProfile(); // Lädt ein Benutzerprofil aus der Datenbank, wenn die Ansicht geöffnet wird.
        }

        // Methode zum Laden eines Benutzerprofils aus der Datenbank
        private void LoadUserProfile()
        {
            try
            {
                // Erstellen einer neuen Verbindung zur MySQL-Datenbank mit der Verbindungszeichenfolge.
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Öffnet die Verbindung zur Datenbank.

                    // SQL-Abfrage, die zufällig einen Benutzer aus der Tabelle "users" auswählt.
                    // Die Felder "FirstName", "Biography", "Hobbys", "Skills" und "DateOfBirth" werden abgerufen.
                    string query = @"
                        SELECT FirstName, Biography, Hobbys, Skills, DateOfBirth 
                        FROM users 
                        ORDER BY RAND() LIMIT 1;";

                    // MySQL-Befehl mit der Abfrage und der bestehenden Verbindung.
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Führt die Abfrage aus und speichert die Ergebnisse im MySqlDataReader.
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Prüft, ob die Abfrage ein Ergebnis liefert.
                            if (reader.Read())
                            {
                                // Setzt die Textblöcke in der Benutzeroberfläche mit den ausgelesenen Daten.
                                TextBlockFirstName.Text = reader["FirstName"].ToString(); // Vorname des Benutzers.
                                AboutMeText.Text = reader["Biography"].ToString(); // Kurzbeschreibung.
                                HobbiesText.Text = reader["Hobbys"].ToString(); // Hobbys.
                                SkillsText.Text = reader["Skills"].ToString(); // Fähigkeiten.

                                // Formatiert das Geburtsdatum und zeigt es an, falls es gültig ist.
                                if (DateTime.TryParse(reader["DateOfBirth"].ToString(), out DateTime dateOfBirth))
                                {
                                    TextBlockDateOfBirth.Text = $"Geburtsdatum: {dateOfBirth:dd.MM.yyyy}";
                                }
                                else
                                {
                                    // Falls das Geburtsdatum nicht gültig ist, wird "Unbekannt" angezeigt.
                                    TextBlockDateOfBirth.Text = "Geburtsdatum: Unbekannt";
                                }
                            }
                            else
                            {
                                // Zeigt eine Nachricht an, wenn keine Daten gefunden wurden.
                                MessageBox.Show("Keine Benutzerdaten gefunden.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Zeigt eine Fehlermeldung an, wenn ein Fehler beim Laden der Benutzerdaten auftritt.
                MessageBox.Show($"Fehler beim Laden der Benutzerdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event-Handler für den "Gefällt mir"-Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Nach dem Klicken wird ein neuer Benutzer geladen.
            LoadUserProfile();
        }

        // Event-Handler für den "Ablehnen"-Button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Nach dem Klicken wird ein neuer Benutzer geladen.
            LoadUserProfile();
        }
    }
}
