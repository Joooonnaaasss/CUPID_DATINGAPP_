using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using static CUPID_DATINGAPP.UserSync;

namespace CUPID_DATINGAPP
{
    public partial class UserData3 : UserControl
    {
        public UserData3()
        {
            InitializeComponent();

            // Setze den Datenkontext auf das UserDataModel, initialisiert mit den aktuellen Daten
            this.DataContext = new UserDataModel
            {
                Hobbies = CurrentUser.Instance.Hobbys,
                Skills = CurrentUser.Instance.Skills
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var model = (UserDataModel)this.DataContext;

            if (string.IsNullOrWhiteSpace(model.Hobbies) ||
                string.IsNullOrWhiteSpace(model.Skills))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Connection string not found. Check App.config.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE users 
                        SET 
                            Hobbies = @Hobbies,
                            Skills = @Skills
                        WHERE UserID = @UserId";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Hobbies", model.Hobbies);
                        cmd.Parameters.AddWithValue("@Skills", model.Skills);
                        cmd.Parameters.AddWithValue("@UserId", CurrentUser.Instance.UserId);

                        cmd.ExecuteNonQuery();

                        // Aktualisiere die globalen Daten
                        CurrentUser.Instance.Hobbys = model.Hobbies;
                        CurrentUser.Instance.Skills = model.Skills;

                        MessageBox.Show("User data successfully saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Daten auf die ursprünglichen Werte zurücksetzen
            this.DataContext = new UserDataModel
            {
                Hobbies = CurrentUser.Instance.Hobbys,
                Skills = CurrentUser.Instance.Skills
            };

            MessageBox.Show("Changes discarded. Original data restored.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public class UserDataModel
        {
            public string Hobbies { get; set; }
            public string Skills { get; set; }
        }
    }
}
