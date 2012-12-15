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
using QLogBrowser.Models;
using QLogBrowser.Helpers;

namespace QLogBrowser
{
    /// <summary>
    /// Interaction logic for LogDetailsWindow.xaml
    /// </summary>
    public partial class LogDetailsWindow : Window
    {
        private QLog _log;

        public LogDetailsWindow(QLog log)
        {
            InitializeComponent();
            lblArea.Foreground = new SolidColorBrush(AreaColorHelper.GetColor(log.AreaColor));
            lblArea.Content = log.Area;
            txtMessage.Text = log.Message;
            txtId.Text = log.Id.ToString();
            txtDate.Text = log.CreatedOn.ToString("dd-MM-yyyy HH:mm:ss,fff");
            txtClass.Text = log.Class;
            txtMethod.Text = log.Method;
            txtSessionId.Text = log.SessionId;
            txtThreadId.Text = log.ThreadId;
            txtUserAgent.Text = log.UserAgent;
            txtUserHost.Text = log.UserHost;
            txtInstanceId.Text = log.InstanceId;
            txtDeploymentId.Text = log.DeploymentId;
            _log = log;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            if (e.Key == Key.Tab)
            {
                SwitchTab();
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                ShowMessageTab();
            else
                ShowDetailsTab();
        }

        private void ShowDetailsTab()
        {
            tbcPages.SelectedIndex = 1;
        }

        private void ShowMessageTab()
        {
            tbcPages.SelectedIndex = 0;
        }

        private bool IsMessageTabSelected()
        {
            return tbcPages.SelectedIndex == 0;
        }

        private void SwitchTab()
        {
            if (IsMessageTabSelected())
                ShowDetailsTab();
            else
                ShowMessageTab();
        }

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("QLog #{0}:\n", _log.Id);
            sb.AppendFormat("Area: {0}\n", _log.Area);
            sb.AppendFormat("Created on: {0}\n", _log.CreatedOn.ToString("dd-MM-yyyy HH:mm"));
            if(!String.IsNullOrWhiteSpace(_log.Class))
                sb.AppendFormat("Class: {0}\n", _log.Class);
            if(!String.IsNullOrWhiteSpace(_log.Method))
                sb.AppendFormat("Method: {0}\n", _log.Method);
            if(!String.IsNullOrWhiteSpace(_log.SessionId))
                sb.AppendFormat("SessionId: {0}\n", _log.SessionId);
            if(!String.IsNullOrWhiteSpace(_log.ThreadId))
                sb.AppendFormat("ThreadId: {0}\n", _log.ThreadId);
            if(!String.IsNullOrWhiteSpace(_log.UserAgent))
                sb.AppendFormat("User agent: {0}\n", _log.UserAgent);
            if(!String.IsNullOrWhiteSpace(_log.UserHost))
                sb.AppendFormat("User host: {0}\n\n", _log.UserHost);

            sb.AppendFormat("\nLog message:\n\n{0}", _log.Message);

            Clipboard.SetText(sb.ToString());
        }
    }
}
