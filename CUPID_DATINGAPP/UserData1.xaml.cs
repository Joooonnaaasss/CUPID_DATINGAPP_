using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using static CUPID_DATINGAPP.UserSync;

namespace CUPID_DATINGAPP
{
    public partial class UserData1 : UserControl
    {
        public UserData1()
        {
            InitializeComponent();

            // Setze den Datenkontext auf das UserDataModel, initialisiert mit den aktuellen Daten
            this.DataContext = new UserDataModel
            {
                Vorname = CurrentUser.Instance.FirstName,
                Nachname = CurrentUser.Instance.LastName,
                Email = CurrentUser.Instance.Email,
                Geburtsdatum = CurrentUser.Instance.DateOfBirth.ToShortDateString(),
                Geschlecht = CurrentUser.Instance.Gender
            };
        }

        private void SaveUserDataButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (UserDataModel)this.DataContext;

            if (string.IsNullOrWhiteSpace(model.Vorname) ||
                string.IsNullOrWhiteSpace(model.Nachname) ||
                string.IsNullOrWhiteSpace(model.Email))
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
                            FirstName = @FirstName,
                            LastName = @LastName,
                            Email = @Email,
                            DateOfBirth = @DateOfBirth,
                            Gender = @Gender
                        WHERE UserID = @UserId";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", model.Vorname);
                        cmd.Parameters.AddWithValue("@LastName", model.Nachname);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(model.Geburtsdatum));
                        cmd.Parameters.AddWithValue("@Gender", model.Geschlecht);
                        cmd.Parameters.AddWithValue("@UserId", CurrentUser.Instance.UserId);

                        cmd.ExecuteNonQuery();

                        // Aktualisiere die globalen Daten
                        CurrentUser.Instance.FirstName = model.Vorname;
                        CurrentUser.Instance.LastName = model.Nachname;
                        CurrentUser.Instance.Email = model.Email;
                        CurrentUser.Instance.DateOfBirth = DateTime.Parse(model.Geburtsdatum);
                        CurrentUser.Instance.Gender = model.Geschlecht;

                        MessageBox.Show("Benutzerdaten wurden erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelUserDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Daten auf die ursprünglichen Werte zurücksetzen
            this.DataContext = new UserDataModel
            {
                Vorname = CurrentUser.Instance.FirstName,
                Nachname = CurrentUser.Instance.LastName,
                Email = CurrentUser.Instance.Email,
                Geburtsdatum = CurrentUser.Instance.DateOfBirth.ToShortDateString(),
                Geschlecht = CurrentUser.Instance.Gender
            };

            MessageBox.Show("Änderungen wurden verworfen. Die ursprünglichen Daten wurden wiederhergestellt.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public class UserDataModel
        {
            public string Vorname { get; set; }
            public string Nachname { get; set; }
            public string Email { get; set; }
            public string Geburtsdatum { get; set; }
            public string Geschlecht { get; set; }
        }
    }
}
