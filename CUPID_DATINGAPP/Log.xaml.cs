using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySqlConnector;
using System.Configuration;
using static CUPID_DATINGAPP.UserSync;

namespace CUPID_DATINGAPP
{
    public partial class Log : UserControl
    {
        private int loggedInUserId = -1; // Globale Variable für die eingeloggte Benutzer-ID

        public Log()
        {
            InitializeComponent();
        }

        private void UserTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserTextBox.Text == "Benutzername")
            {
                UserTextBox.Text = "";
                UserTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void UserTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserTextBox.Text))
            {
                UserTextBox.Text = "Benutzername";
                UserTextBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Text == "Password")
            {
                PasswordBox.Text = "";
                PasswordBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                PasswordBox.Text = "Password";
                PasswordBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153));
            }
        }

        private void AnmeldenButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextBox.Text.Trim();
            string password = PasswordBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || username == "Benutzername" ||
                string.IsNullOrEmpty(password) || password == "Password")
            {
                MessageBox.Show("Bitte geben Sie Benutzername und Passwort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ValidateLogin(username, password))
            {
                LoadUserData(); // Passende Benutzerdaten laden
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.ShowFrame(mainWindow.MenuFrame);
            }
            else
            {
                MessageBox.Show("Benutzername oder Passwort ist falsch. Bitte versuchen Sie es erneut.", "Login Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUserData()
        {
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
                        SELECT 
                            FirstName, LastName, Email, DateOfBirth, Gender, 
                            Username, Biography, TargetAudience, profile_photo, 
                            Hobbys, Skills 
                        FROM users 
                        WHERE UserID = @UserId";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", loggedInUserId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CurrentUser.Instance.UserId = loggedInUserId;
                                CurrentUser.Instance.FirstName = reader.GetString("FirstName");
                                CurrentUser.Instance.LastName = reader.GetString("LastName");
                                CurrentUser.Instance.Email = reader.GetString("Email");
                                CurrentUser.Instance.DateOfBirth = reader.GetDateTime("DateOfBirth");
                                CurrentUser.Instance.Gender = reader.GetString("Gender");
                                CurrentUser.Instance.Username = reader.GetString("Username");
                                CurrentUser.Instance.Biography = reader.IsDBNull(reader.GetOrdinal("Biography")) ? null : reader.GetString("Biography");
                                CurrentUser.Instance.TargetAudience = reader.IsDBNull(reader.GetOrdinal("TargetAudience")) ? null : reader.GetString("TargetAudience");
                                CurrentUser.Instance.ProfilePhoto = reader.IsDBNull(reader.GetOrdinal("profile_photo")) ? null : (byte[])reader["profile_photo"];
                                CurrentUser.Instance.Hobbys = reader.IsDBNull(reader.GetOrdinal("Hobbys")) ? string.Empty : reader.GetString("Hobbys");
                                CurrentUser.Instance.Skills = reader.IsDBNull(reader.GetOrdinal("Skills")) ? string.Empty : reader.GetString("Skills");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}\nDetails: {ex.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Verbindungszeichenfolge nicht gefunden. Überprüfen Sie die App.config.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UserID FROM users WHERE Username = @Username AND Password = @Password";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            loggedInUserId = Convert.ToInt32(result);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}\nDetails: {ex.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ShowFrame(mainWindow.RegistrierFrame);
        }

        private void PasswordForget(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Bitte kontaktieren Sie den Support, um Ihr Passwort zurückzusetzen.", "Passwort vergessen", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
