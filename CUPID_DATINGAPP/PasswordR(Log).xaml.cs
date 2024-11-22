using System;
using System.Configuration;
using MySqlConnector;
using System.Windows;
using System.Windows.Controls;

namespace CUPID_DATINGAPP
{
    public partial class PasswordR_Log_ : UserControl
    {
        public PasswordR_Log_()
        {
            InitializeComponent();
        }

        // Event-Handler für den Schließen-Button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.ShowFrame(mainWindow.LogFrame);
        }

        // Platzhalter-Logik für die E-Mail-Textbox
        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Platzhalter-Logik für das Geburtsdatum-Textbox
        private void DateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatePlaceholder.Visibility = string.IsNullOrEmpty(DateTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Platzhalter-Logik für das Passwortfeld
        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordTextBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Event-Handler für den Passwort-Zurücksetzen-Button
        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            // Werte aus den Eingabefeldern
            string email = EmailTextBox.Text.Trim();
            string dateOfBirth = DateTextBox.Text.Trim(); // Originale Eingabe des Benutzers
            string newPassword = PasswordTextBox.Password.Trim();

            // Validierung der Eingaben
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(dateOfBirth) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Datum formatieren, falls erforderlich
            try
            {
                DateTime parsedDate = DateTime.Parse(dateOfBirth); // Eingabe prüfen
                dateOfBirth = parsedDate.ToString("yyyy-MM-dd"); // Annahme: DB verwendet YYYY-MM-DD
            }
            catch (FormatException)
            {
                MessageBox.Show("Bitte geben Sie ein gültiges Datum ein (z. B. TT/MM/JJJJ).", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Passwort zurücksetzen
            bool success = ResetUserPassword(email, dateOfBirth, newPassword);
            if (success)
            {
                MessageBox.Show("Das Passwort wurde erfolgreich zurückgesetzt.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                // Felder leeren
                EmailTextBox.Text = string.Empty;
                DateTextBox.Text = string.Empty;
                PasswordTextBox.Password = string.Empty;
            }
            else
            {
                MessageBox.Show("Benutzer konnte nicht gefunden werden oder die Eingaben sind falsch.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Passwort zurücksetzen in der Datenbank
        private bool ResetUserPassword(string email, string dateOfBirth, string newPassword)
        {
            try
            {
                // Verbindung zur Datenbank über die app.config
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Debugging: Gebe SQL-Abfrage und Parameter aus
                    string debugQuery = $"SELECT * FROM Users WHERE Email = '{email}' AND DateOfBirth = '{dateOfBirth}'";
                    MessageBox.Show($"Debug SQL Query: {debugQuery}", "Debugging");

                    // Überprüfen, ob der Benutzer existiert und die Daten zurückgeben
                    string query = "SELECT * FROM Users WHERE Email = @Email AND DateOfBirth = @DateOfBirth";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Benutzer gefunden
                            {
                                // Debugging: Werte des gefundenen Benutzers ausgeben
                                string foundEmail = reader["Email"].ToString();
                                string foundDateOfBirth = reader["DateOfBirth"].ToString();
                                MessageBox.Show($"Benutzer gefunden: Email={foundEmail}, DateOfBirth={foundDateOfBirth}", "Debugging");

                                // Passwort aktualisieren
                                reader.Close(); // Schließe den Reader vor einer neuen SQL-Anweisung
                                query = "UPDATE Users SET Password = @Password WHERE Email = @Email AND DateOfBirth = @DateOfBirth";
                                using (MySqlCommand updateCommand = new MySqlCommand(query, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@Password", newPassword); // Klartext-Passwort
                                    updateCommand.Parameters.AddWithValue("@Email", email);
                                    updateCommand.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                                    int rowsAffected = updateCommand.ExecuteNonQuery();
                                    return rowsAffected > 0; // Erfolg, wenn mindestens eine Zeile aktualisiert wurde
                                }
                            }
                            else
                            {
                                // Benutzer nicht gefunden
                                MessageBox.Show("Benutzer konnte nicht gefunden werden.", "Fehler");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Datenbankoperation: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
