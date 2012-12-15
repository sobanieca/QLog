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

namespace QLogBrowser
{
    /// <summary>
    /// Interaction logic for AdvancedFiltersWindow.xaml
    /// </summary>
    public partial class AdvancedFiltersWindow : Window
    {
        public string SessionId { get { return txtSesionId.Text; } }
        public string ThreadId { get { return txtThreadId.Text; } }
        public string UserAgent { get { return txtUserAgent.Text; } }
        public string UserHost { get { return txtUserHost.Text; } }
        public string Class { get { return txtClass.Text; } }
        public string Method { get { return txtMethod.Text; } }
        public string InstanceId { get { return txtInstanceId.Text; } }
        public string DeploymentId { get { return txtDeploymentId.Text; } }

        private bool _okPressed = false;
        public bool OkPressed { get { if (_okPressed) { _okPressed = false; return true; } else return false; } }

        public AdvancedFiltersWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            OnOkClick();
        }

        private void OnOkClick()
        {
            _okPressed = true;
            Hide();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OnOkClick();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

    }
}
