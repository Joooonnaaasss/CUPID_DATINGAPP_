using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using static CUPID_DATINGAPP.UserSync;

namespace CUPID_DATINGAPP
{
    public partial class UserData2 : UserControl
    {
        public UserData2()
        {
            InitializeComponent();

            // Setze den Datenkontext auf das UserDataModel, initialisiert mit den aktuellen Daten
            this.DataContext = new UserDataModel
            {
                Benutzername = CurrentUser.Instance.Username,
                Zielgruppe = CurrentUser.Instance.TargetAudience,
                Biografie = CurrentUser.Instance.Biography
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (UserDataModel)this.DataContext;

            if (string.IsNullOrWhiteSpace(model.Benutzername) ||
                string.IsNullOrWhiteSpace(model.Zielgruppe) ||
                string.IsNullOrWhiteSpace(model.Biografie))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Verbindungszeichenfolge nicht gefunden. Überprüfen Sie die App.config.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE users 
                        SET 
                            Username = @Username,
                            TargetAudience = @TargetAudience,
                            Biography = @Biography
                        WHERE UserID = @UserId";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", model.Benutzername);
                        cmd.Parameters.AddWithValue("@TargetAudience", model.Zielgruppe);
                        cmd.Parameters.AddWithValue("@Biography", model.Biografie);
                        cmd.Parameters.AddWithValue("@UserId", CurrentUser.Instance.UserId);

                        cmd.ExecuteNonQuery();

                        // Aktualisiere die globalen Daten
                        CurrentUser.Instance.Username = model.Benutzername;
                        CurrentUser.Instance.TargetAudience = model.Zielgruppe;
                        CurrentUser.Instance.Biography = model.Biografie;

                        MessageBox.Show("Benutzerdaten wurden erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Daten auf die ursprünglichen Werte zurücksetzen
            this.DataContext = new UserDataModel
            {
                Benutzername = CurrentUser.Instance.Username,
                Zielgruppe = CurrentUser.Instance.TargetAudience,
                Biografie = CurrentUser.Instance.Biography
            };

            MessageBox.Show("Änderungen wurden verworfen. Die ursprünglichen Daten wurden wiederhergestellt.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public class UserDataModel
        {
            public string Benutzername { get; set; }
            public string Zielgruppe { get; set; }
            public string Biografie { get; set; }
        }
    }
}
