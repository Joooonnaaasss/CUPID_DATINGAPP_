using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

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

        public Reg3(Dictionary<string, string> data) : this() // Aufruf des parameterlosen Konstruktors
        {
            registrationData = data;
        }

        // Registrierung abschließen
        private void FinishRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
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
                        cmd.Parameters.AddWithValue("@Hobbys", registrationData.ContainsKey("Hobbys") ? registrationData["Hobbys"] : ""); // Korrektur hier
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
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Registrierung: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            // Lade Reg2 in den RegistrierFrame
            mainWindow.RegistrierFrame.Content = new Reg2(registrationData); // Reg2 als Inhalt setzen

            // Zeige den RegistrierFrame an
            mainWindow.ShowFrame(mainWindow.RegistrierFrame);
        }
    }
}
