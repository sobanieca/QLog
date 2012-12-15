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
using System.Reflection;

namespace QLogBrowser
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AddAreaWindow : Window
    {
        public bool AddClicked { get; private set; }
        public string AreaName { get; private set; }

        public AddAreaWindow()
        {
            InitializeComponent();
            txtArea.Focus();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddAndClose();
        }

        private void AddAndClose()
        {
            AddClicked = true;
            AreaName = txtArea.Text;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void txtArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddAndClose();
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
