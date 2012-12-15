using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QLogBrowser.Helpers;
using QLogBrowser.Models;
using System.Diagnostics;

namespace QLogBrowser
{
    /// <summary>
    /// Interaction logic for StorageConnectionWindow.xaml
    /// </summary>
    public partial class StorageConnectionWindow : Window
    {
        public bool SettingsChanged { get; private set; }

        public QLogBrowserSettings Settings { get; private set; }

        public StorageConnectionWindow(QLogBrowserSettings settings)
        {
            Settings = settings;
            InitializeComponent();
            PresentSettings();
        }

        private string GetCmbConnectionItem(string accountName, string postfix)
        {
            if (!String.IsNullOrWhiteSpace(postfix))
                return String.Format("{0} | {1}", accountName, postfix);
            else
                return String.Format("{0}", accountName);
        }

        private void PresentSettings()
        {
            cmbConnections.Items.Clear();
            int index = 0;
            int selectedConnectionIndex = 0;
            StorageConnection selectedConnection = null;
            foreach (StorageConnection conn in Settings.Connections)
            {
                if (conn.IsSelected)
                {
                    selectedConnection = conn;
                    selectedConnectionIndex = index;
                }
                cmbConnections.Items.Add(GetCmbConnectionItem(conn.AccountName, conn.SourceDataPostfix));
                index++;
            }
            if (selectedConnection != null)
            {
                cmbConnections.SelectedIndex = selectedConnectionIndex;
                PresentConnection(selectedConnection);
            }
            else
            {
                ClearFields();
                HideSelectionCombobox();
            }
        }

        private void ClearFields()
        {
            txtAccountName.Text = "";
            txtAccountKey.Text = "";
            txtPostfix.Text = "";
            cbxDevelopmentStorage.IsChecked = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Enter)
            {
                Save();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (ValidateConnection())
            {
                AddOrUpdateConnection();
                SettingsChanged = true;
                Close();
            }
        }

        private bool ValidateConnection()
        {
            if (String.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                MessageBox.Show("Please provide account name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (String.IsNullOrWhiteSpace(txtAccountKey.Text) && !cbxDevelopmentStorage.IsChecked.Value)
            {
                MessageBox.Show("Please provide account key!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void AddOrUpdateConnection()
        {
            StorageConnection newConnection = new StorageConnection();
            newConnection.AccountName = txtAccountName.Text;
            newConnection.AccountKey = txtAccountKey.Text;
            newConnection.SourceDataPostfix = txtPostfix.Text;
            newConnection.IsDevelopmentStorage = cbxDevelopmentStorage.IsChecked.Value;
            newConnection.IsHttps = cbxHttps.IsChecked.Value;
            AddOrUpdateConnection(newConnection);
        }

        private void AddOrUpdateConnection(StorageConnection editedConnection)
        {
            StorageConnection conn = FindConnection(editedConnection);
            Settings.Connections.ForEach(x => x.IsSelected = false);
            if (conn == null)
            {
                editedConnection.IsSelected = true;
                Settings.Connections.Add(editedConnection);
            }
            else
            {
                conn.IsDevelopmentStorage = editedConnection.IsDevelopmentStorage;
                conn.IsHttps = editedConnection.IsHttps;
                conn.AccountKey = editedConnection.AccountKey;
                conn.IsSelected = true;
            }
        }

        private StorageConnection FindConnection(StorageConnection newConnection)
        {
            return FindConnection(newConnection.AccountName, newConnection.SourceDataPostfix);
        }

        private StorageConnection FindConnection(string accountName, string postfix)
        {
            foreach (StorageConnection conn in Settings.Connections)
            {
                if (conn.AccountName.ToLower() == accountName.ToLower() && conn.SourceDataPostfix.ToLower() == postfix.ToLower())
                    return conn;
            }
            return null;
        }

        private void cmbConnections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = cmbConnections.SelectedIndex;
            if (index >= 0 && index < Settings.Connections.Count)
            {
                StorageConnection conn = Settings.Connections[index];
                Settings.Connections.ForEach(x => x.IsSelected = false);
                conn.IsSelected = true;
                PresentConnection(conn);
            }
        }

        private void PresentConnection(StorageConnection conn)
        {
            txtAccountName.Text = conn.AccountName;
            txtAccountKey.Text = conn.AccountKey;
            txtPostfix.Text = conn.SourceDataPostfix;
            cbxDevelopmentStorage.IsChecked = conn.IsDevelopmentStorage;
            cbxHttps.IsChecked = conn.IsHttps;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string accountName = txtAccountName.Text;
            string postfix = txtPostfix.Text;
            DeleteConnection(accountName, postfix);
            PresentSettings();
        }

        private void DeleteConnection(string accountName, string postfix)
        {
            StorageConnection conn = FindConnection(accountName, postfix);
            if (conn != null)
            {
                Settings.Connections.Remove(conn);
                if (conn.IsSelected)
                {
                    if (Settings.Connections.Count > 0)
                        Settings.Connections[0].IsSelected = true;
                }
            }
        }

        private void HideSelectionCombobox()
        {
            cmbConnections.Visibility = System.Windows.Visibility.Hidden;
            btnDelete.Visibility = System.Windows.Visibility.Hidden;
        }

        private void UpdateConnectionCombobox(object sender, KeyEventArgs e)
        {
            if (!FindMatchingConnection())
                HideSelectionCombobox();
            else
                ShowSelectionCombobox();
        }

        private void ShowSelectionCombobox()
        {
            string currentConnItem = GetCmbConnectionItem(txtAccountName.Text, txtPostfix.Text);
            int index = 0;
            for (int i = 0; i < cmbConnections.Items.Count; i++)
            {
                if (cmbConnections.Items[i].ToString().ToLower() == currentConnItem.ToLower())
                {
                    index = i;
                    break;
                }
            }
            cmbConnections.SelectedIndex = index;
            cmbConnections.Visibility = System.Windows.Visibility.Visible;
            btnDelete.Visibility = System.Windows.Visibility.Visible;
        }

        private bool FindMatchingConnection()
        {
            foreach (var item in cmbConnections.Items)
            {
                if (item.ToString().ToLower() == GetCmbConnectionItem(txtAccountName.Text, txtPostfix.Text).ToLower())
                    return true;
            }
            return false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisableAccountKey(object sender, RoutedEventArgs e)
        {
            txtAccountKey.IsEnabled = false;
        }

        private void EnableAccountKey(object sender, RoutedEventArgs e)
        {
            txtAccountKey.IsEnabled = true;
        }
    }
}
