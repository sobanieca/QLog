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
    /// Interaction logic for CleanLogsWindow.xaml
    /// </summary>
    public partial class DeleteLogsWindow : Window
    {
        private const int DEFAULT_DAYS_VALUE = 7;

        private int _noDays = DEFAULT_DAYS_VALUE;

        public bool DoDelete { get; private set; }
        public int NoDays { get { return _noDays; } }

        public DeleteLogsWindow()
        {
            InitializeComponent();
            txtDays.Text = DEFAULT_DAYS_VALUE.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            SetNoDays();
            string confirmation = String.Format("Are You sure that You want to delete all logs older than {0} day(s)? This operation cannot be cancelled.", _noDays);
            if (_noDays == 0)
                confirmation = String.Format("Are You sure that You want to delete ALL logs? This operation cannot be cancelled.");
            if (MessageBox.Show(confirmation, "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                DoDelete = true;
                Close();
            }
        }

        private void txtDays_LostFocus(object sender, RoutedEventArgs e)
        {
            SetNoDays();
        }

        private void SetNoDays()
        {
            int noDays = 0;
            Int32.TryParse(txtDays.Text, out noDays);
            if (noDays == 0)
            {
                if (txtDays.Text != "0")
                {
                    txtDays.Text = DEFAULT_DAYS_VALUE.ToString();
                    _noDays = DEFAULT_DAYS_VALUE;
                }
                else
                    _noDays = 0;
            }
            else
            {
                _noDays = noDays;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            if (e.Key == Key.Enter)
                Delete();
        }
    }
}
