﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CUPID_DATINGAPP
{
    public partial class Reg2 : UserControl
    {
        private Dictionary<string, string> registrationData;
        private string photoPath = null;


        public Reg2()
        {
            InitializeComponent();
            registrationData = new Dictionary<string, string>();
        }

        public Reg2(Dictionary<string, string> data) : this() // Aufruf des parameterlosen Konstruktors
        {
            registrationData = data; //Data
        }

        // Weiterleitung zu Reg3
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                // Lade Reg3 in den RegistrierFrame
                mainWindow.RegistrierFrame.Content = new Reg3(registrationData); // Reg3 als Inhalt setzen

                // Zeige den RegistrierFrame an
                mainWindow.ShowFrame(mainWindow.RegistrierFrame);
            }
        }


        // Zurück zu Reg
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var reg = new Reg(registrationData); // registrationData muss vorher definiert sein.

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            // Lade die Login-Seite in den LogFrame
            mainWindow.RegistrierFrame.Content = new Reg(); // Log ist die Login-Oberfläche

            // Zeige den LogFrame an
            mainWindow.ShowFrame(mainWindow.RegistrierFrame);

        }

        // Foto hochladen
        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // OpenFileDialog to select a photo
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Bilder (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Wählen Sie ein Profilbild aus"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string photoPath = openFileDialog.FileName; // Speichere den Pfad
                MessageBox.Show($"Foto erfolgreich hochgeladen: {photoPath}", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Kein Foto ausgewählt.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Eingabevalidierung
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie einen Benutzernamen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (TargetAudienceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Bitte wählen Sie eine Zielgruppe aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(BiographyTextBox.Text))
            {
                MessageBox.Show("Bitte geben Sie eine Biografie ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
        
    }
}
