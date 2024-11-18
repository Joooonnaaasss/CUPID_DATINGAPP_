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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySqlConnector;

namespace CUPID_DATINGAPP // Namespace für die Anwendung
{
    // Die Log-Klasse repräsentiert das Login-Steuerelement
    public partial class Log : UserControl
    {
        // Konstruktor: Initialisiert das Login-Steuerelement
        public Log()
        {
            InitializeComponent(); // Lädt die zugehörige XAML-Datei und initialisiert die UI-Komponenten
        }

        // Event-Handler für das Fokussieren der Benutzername-TextBox
        private void UserTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Entfernt den Platzhaltertext, wenn der Benutzer die TextBox fokussiert
            if (UserTextBox.Text == "Benutzername")
            {
                UserTextBox.Text = ""; // Leert die TextBox
                UserTextBox.Foreground = new SolidColorBrush(Colors.Black); // Setzt die Schriftfarbe auf Schwarz
            }
        }

        // Event-Handler für das Verlassen des Fokus in der Benutzername-TextBox
        private void UserTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Setzt den Platzhalter zurück, wenn die TextBox leer ist
            if (string.IsNullOrWhiteSpace(UserTextBox.Text))
            {
                UserTextBox.Text = "Benutzername"; // Setzt den Platzhaltertext
                UserTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)); // Setzt die Schriftfarbe auf Grau
            }
        }

        // Event-Handler für das Fokussieren der Passwort-TextBox
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Entfernt den Platzhaltertext, wenn der Benutzer die TextBox fokussiert
            if (PasswordBox.Text == "Password")
            {
                PasswordBox.Text = ""; // Leert die TextBox
                PasswordBox.Foreground = new SolidColorBrush(Colors.Black); // Setzt die Schriftfarbe auf Schwarz
            }
        }

        // Event-Handler für das Verlassen des Fokus in der Passwort-TextBox
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Setzt den Platzhalter zurück, wenn die TextBox leer ist
            if (string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                PasswordBox.Text = "Password"; // Setzt den Platzhaltertext
                PasswordBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)); // Setzt die Schriftfarbe auf Grau
            }
        }

        // Event-Handler für den Klick auf den Anmelden-Button
        private void AnmeldenButton_Click(object sender, RoutedEventArgs e)
        {
            var con = new MySqlConnection(
           "server=localhost;userid=root;password=Niviistcool123!;database=cupid");

            // Hier kannst du die Login-Logik hinzufügen
            string username = UserTextBox.Text;
            string password = PasswordBox.Text;


            // Beispielhafte Überprüfung der Anmeldedaten
            if (ValidateLogin(username, password))
            {
                // Navigiert zur Hauptseite, wenn die Anmeldung erfolgreich ist
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ShowFrame(mainWindow.MenuFrame);
            }
            else
            {
                // Zeigt eine Fehlermeldung bei ungültigen Anmeldedaten
                MessageBox.Show("Benutzername oder Passwort ist falsch. Bitte versuchen Sie es erneut.", "Login Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Methode zur Validierung der Anmeldedaten durch die Datenbank
        private bool ValidateLogin(string username, string password)
        {
            try
            {
                // Ruft die Verbindungszeichenfolge aus der App.config ab
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                // Erstellt eine Verbindung zur MySQL-Datenbank
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Öffnet die Verbindung

                    // SQL-Abfrage zur Überprüfung der Benutzerdaten
                    string query = "SELECT COUNT(*) FROM users WHERE Username = @Username AND Password = @Password";

                    // Erstellt einen SQL-Befehl mit der Abfrage
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Fügt Parameter hinzu, um SQL-Injection zu verhindern
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        // Führt die Abfrage aus und holt die Anzahl der passenden Datensätze
                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                        return userCount > 0; // Gibt true zurück, wenn ein Benutzer gefunden wurde
                    }
                }
            }
            catch (Exception ex)
            {
                // Zeigt eine Fehlermeldung an, wenn ein Fehler auftritt
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // Gibt false zurück, wenn ein Fehler auftritt
            }
        }

        // Event-Handler für den Registrierungs-Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Navigiert zur Registrierungsseite
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ShowFrame(mainWindow.RegistrierFrame);
        }

        // Event-Handler für den "Passwort vergessen"-Link
        private void PasswordForget(object sender, MouseButtonEventArgs e)
        {
            // Zeigt eine Info-Box für das Zurücksetzen des Passworts
            MessageBox.Show("Bitte kontaktieren Sie den Support, um Ihr Passwort zurückzusetzen.", "Passwort vergessen", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
