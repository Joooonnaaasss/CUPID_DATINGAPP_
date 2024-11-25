using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Windows.Media;

namespace CUPID_DATINGAPP
{
    public partial class Reg3 : UserControl
    {
        private Dictionary<string, string> registrationData;

        public Reg3()
        {
            InitializeComponent();
            registrationData = new Dictionary<string, string>();
        }

        public Reg3(Dictionary<string, string> data) : this() // Konstruktor mit Parameter
        {
            registrationData = data;
        }

        // Registrierung abschließen
        private void FinishRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            // Prüfe, ob die Eingaben gültig sind
            if (ValidateInputs())
            {
                // Daten aus TextBoxen in das Dictionary übernehmen
                registrationData["Biography"] = BiographyTextBox.Text.Trim();
                registrationData["Hobbys"] = HobbysTextBox.Text.Trim();
                registrationData["Skills"] = SkillsTextBox.Text.Trim();

                try
                {
                    // Verbindung zur Datenbank herstellen
                    string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"INSERT INTO users 
                                         (FirstName, LastName, Email, DateOfBirth, Gender, Username, Biography, TargetAudience, profile_photo, Hobbys, Skills, Password) 
                                         VALUES 
                                         (@FirstName, @LastName, @Email, @DateOfBirth, @Gender, @Username, @Biography, @TargetAudience, @profile_photo, @Hobbys, @Skills, @Password)";

                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            // Parameter setzen
                            cmd.Parameters.AddWithValue("@FirstName", registrationData.ContainsKey("FirstName") ? registrationData["FirstName"] : "");
                            cmd.Parameters.AddWithValue("@LastName", registrationData.ContainsKey("LastName") ? registrationData["LastName"] : "");
                            cmd.Parameters.AddWithValue("@Email", registrationData.ContainsKey("Email") ? registrationData["Email"] : "");
                            cmd.Parameters.AddWithValue("@DateOfBirth", registrationData.ContainsKey("DateOfBirth") ? registrationData["DateOfBirth"] : "");
                            cmd.Parameters.AddWithValue("@Gender", registrationData.ContainsKey("Gender") ? registrationData["Gender"] : "");
                            cmd.Parameters.AddWithValue("@Username", registrationData.ContainsKey("Username") ? registrationData["Username"] : "");
                            cmd.Parameters.AddWithValue("@Biography", registrationData.ContainsKey("Biography") ? registrationData["Biography"] : "");
                            cmd.Parameters.AddWithValue("@TargetAudience", registrationData.ContainsKey("TargetAudience") ? registrationData["TargetAudience"] : "");
                            cmd.Parameters.AddWithValue("@profile_photo", registrationData.ContainsKey("profile_photo") ? registrationData["profile_photo"] : "");
                            cmd.Parameters.AddWithValue("@Password", registrationData.ContainsKey("Password") ? registrationData["Password"] : "");
                            cmd.Parameters.AddWithValue("@Hobbys", registrationData.ContainsKey("Hobbys") ? registrationData["Hobbys"] : "");
                            cmd.Parameters.AddWithValue("@Skills", registrationData.ContainsKey("Skills") ? registrationData["Skills"] : "");

                            // SQL-Befehl ausführen
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Registrierung erfolgreich abgeschlossen!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Zurück zur Login-Seite
                            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                            mainWindow.LogFrame.Content = new Log(); // Lade Login in den LogFrame
                            mainWindow.ShowFrame(mainWindow.LogFrame); // Zeige LogFrame an
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Fehler bei der Verbindung zur Datenbank: {ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ein unerwarteter Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Zurück-Button
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            // Lade Reg2 in den RegistrierFrame
            mainWindow.RegistrierFrame.Content = new Reg2(registrationData); // Reg2 als Inhalt setzen

            // Zeige den RegistrierFrame an
            mainWindow.ShowFrame(mainWindow.RegistrierFrame);
        }

        // Eingabevalidierung
        private bool ValidateInputs()
        {
            // Überprüfe, ob Biografie länger als 200 Zeichen ist
            if (BiographyTextBox.Text.Trim().Length > 200)
            {
                MessageBox.Show("Die Biografie darf maximal 200 Zeichen lang sein.", "Eingabefehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Überprüfe, ob Pflichtfelder ausgefüllt sind
            if (string.IsNullOrWhiteSpace(HobbysTextBox.Text) || HobbysTextBox.Text.Trim() == "Hobbys")
            {
                MessageBox.Show("Bitte geben Sie Ihre Hobbys ein.", "Eingabefehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SkillsTextBox.Text) || SkillsTextBox.Text.Trim() == "Fähigkeiten (Skills)")
            {
                MessageBox.Show("Bitte geben Sie Ihre Fähigkeiten ein.", "Eingabefehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(BiographyTextBox.Text) || BiographyTextBox.Text.Trim() == "Biografie")
            {
                MessageBox.Show("Bitte geben Sie Ihre Biografie ein.", "Eingabefehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void HobbysTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (HobbysTextBox.Text == "Hobbys")
            {
                HobbysTextBox.Text = "";
                HobbysTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void HobbysTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HobbysTextBox.Text))
            {
                HobbysTextBox.Text = "Hobbys";
                HobbysTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void SkillsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SkillsTextBox.Text == "Fähigkeiten (Skills)")
            {
                SkillsTextBox.Text = "";
                SkillsTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void SkillsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SkillsTextBox.Text))
            {
                SkillsTextBox.Text = "Fähigkeiten (Skills)";
                SkillsTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void BiographyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (BiographyTextBox.Text == "Biografie")
            {
                BiographyTextBox.Text = "";
                BiographyTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void BiographyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BiographyTextBox.Text))
            {
                BiographyTextBox.Text = "Biografie";
                BiographyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }
    }
}
