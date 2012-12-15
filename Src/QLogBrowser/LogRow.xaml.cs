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
using System.Windows.Navigation;
using System.Windows.Shapes;
using QLogBrowser.Helpers;
using QLogBrowser.Models;

namespace QLogBrowser
{
    /// <summary>
    /// Interaction logic for LogRow.xaml
    /// </summary>
    public partial class LogRow : UserControl
    {
        private readonly Brush _hoverBrush = new SolidColorBrush(Color.FromArgb(255, 185, 185, 185));
        private readonly Brush _evenBrush = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));

        private Brush _backgroundBrush;
        private QLog _log;

        public LogRow(int index, QLog log)
        {
            InitializeComponent();
            _log = log;
            lblArea.Content = log.Area;
            lblArea.Foreground = new SolidColorBrush(AreaColorHelper.GetColor(log.AreaColor));
            lblMessage.Content = log.Message;
            lblDate.Content = log.CreatedOn.ToString("dd-MM-yyyy HH:mm:ss,fff");
            if (index % 2 != 0)
                _backgroundBrush = Brushes.White;
            else
                _backgroundBrush = _evenBrush;
            Background = _backgroundBrush;
        }

        private void Highlight(object sender, MouseEventArgs e)
        {
            this.Background = _hoverBrush;
        }

        private void RemoveHighlight(object sender, MouseEventArgs e)
        {
            this.Background = _backgroundBrush;
        }

        private void UserControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            LogDetailsWindow logDetailsWindow = new LogDetailsWindow(_log);
            logDetailsWindow.ShowDialog();
        }
    }
}
