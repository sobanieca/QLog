using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using QLogBrowser.Libs;
using QLogBrowser.Models;
using QLogBrowser.Services;

namespace QLogBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int STANDARD_LOGS_LIMIT = 100;
        private const string CANCEL = "Cancel";
        private const string LOAD_LOGS = "Load logs";
        private const string SCAN = "Scan";
        private const string CONNECTION_ERROR = "Unable to connect to Azure Storage. Please check Your connection settings or firewall settings and try again.";
        private const int DATES_HOUR_SPAN = 3;

        private QLogBrowserSettings _settings;
        private CheckBox _selectAllCheckbox;
        private AdvancedFiltersWindow _advancedFiltersWindow;
        private LogService _logService;
        private List<WorkerTask> _bgWorkers;
        private static Guid _currentTaskId;
        private bool _locked = false;

        public static Guid CurrentTaskId
        {
            get { return _currentTaskId; }
        }

        public MainWindow()
        {
            InitializeComponent();
            _advancedFiltersWindow = new AdvancedFiltersWindow();
            UpgradeSettings();
            SetCultureInfo();
            InitializeAreasCheckbox();
            LoadSettings();
            _logService = new LogService();
            _bgWorkers = new List<WorkerTask>();
            _currentTaskId = Guid.NewGuid();
            PresentAreas();
            ResetDates();
            if (!_settings.Connections.Any(x => x.IsSelected))
                OpenStorageConnectionWindow();
            else
                LoadLogs();
        }

        private void UpgradeSettings()
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }

        private void SetCultureInfo()
        {
            CultureInfo ci = new CultureInfo(1033);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            ci.DateTimeFormat.DateSeparator = "-";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private void ResetDates()
        {
            DateTime now = DateTime.Now.AddMinutes(1);
            DateTime dateFrom = now.AddHours(-1 * DATES_HOUR_SPAN);
            DateTime dateTo = now.AddHours(DATES_HOUR_SPAN);
            dtDateFrom.SelectedDate = dateFrom;
            dtDateTo.SelectedDate = dateTo;
            txtHourFrom.Text = dateFrom.ToString("HH:mm");
            txtHourTo.Text = dateTo.ToString("HH:mm");
        }

        private void OpenStorageConnectionWindow()
        {
            UpdateSettings();
            StorageConnectionWindow StorageConnection = new StorageConnectionWindow(CloneHelper.Clone<QLogBrowserSettings>(_settings));
            StorageConnection.ShowDialog();
            if (StorageConnection.SettingsChanged)
            {
                _settings = StorageConnection.Settings;
                SaveSettings();
                PresentAreas();
                LoadLogs();
            }
        }

        private void SetTitle()
        {
            StorageConnection currentConnection = _settings.Connections.FirstOrDefault(x => x.IsSelected);
            if (currentConnection != null)
            {
                if (!String.IsNullOrWhiteSpace(currentConnection.SourceDataPostfix))
                    Title = String.Format("QLogBrowser [{0} | {1}]", currentConnection.AccountName, currentConnection.SourceDataPostfix);
                else
                    Title = String.Format("QLogBrowser [{0}]", currentConnection.AccountName);
            }
            else
            {
                Title = "QLogBrowser";
            }
        }

        private void LockUi()
        {
            _locked = true;
            dtDateFrom.IsEnabled = false;
            dtDateTo.IsEnabled = false;
            txtHourFrom.IsEnabled = false;
            txtHourTo.IsEnabled = false;
            txtContainingText.IsEnabled = false;
            foreach (var item in pnlAreas.Children)
            {
                var cbx = item as CheckBox;
                cbx.IsEnabled = false;
            }
            txtLimit.IsEnabled = false;
            btnReset.IsEnabled = false;
            btnAdd.IsEnabled = false;
        }

        private void UnlockUi()
        {
            _locked = false;
            dtDateFrom.IsEnabled = true;
            dtDateTo.IsEnabled = true;
            txtHourFrom.IsEnabled = true;
            txtHourTo.IsEnabled = true;
            txtContainingText.IsEnabled = true;
            foreach (var item in pnlAreas.Children)
            {
                var cbx = item as CheckBox;
                cbx.IsEnabled = true;
            }
            txtLimit.IsEnabled = true;
            btnLoadLogs.IsEnabled = true;
            btnReset.IsEnabled = true;
            btnScan.IsEnabled = true;
            btnLoadLogs.Content = LOAD_LOGS;
            btnScan.Content = SCAN;
            btnAdd.IsEnabled = true;
            txtLimit.Text = STANDARD_LOGS_LIMIT.ToString();
        }

        private void LoadLogs()
        {
            UpdateSettings();
            if (ValidateFilters())
            {
                SetStatus("Loading logs...");
                WorkerTask task = new WorkerTask();
                task.TaskId = Guid.NewGuid();
                _currentTaskId = task.TaskId;
                task.Worker = new BackgroundWorker();
                task.Worker.DoWork += StartLoadingLogs;
                task.Worker.RunWorkerCompleted += FinishLoadingLogs;
                _settings.TaskId = task.TaskId;
                LockUi();
                btnScan.IsEnabled = false;
                btnLoadLogs.Content = CANCEL;
                task.Worker.RunWorkerAsync(CloneHelper.Clone<QLogBrowserSettings>(_settings));
                _bgWorkers.Add(task);
            }
            SetTitle();
        }

        private bool ValidateFilters()
        {
            if (!ValidateLimit())
                return false;
            if (!ValidateDates())
                return false;
            return true;
        }

        private bool ValidateDates()
        {
            DateTime dateFrom = _settings.DateFrom;
            DateTime dateTo = _settings.DateTo;
            int hoursDiff = (int)(dateTo - dateFrom).TotalHours;
            if (hoursDiff > DATES_HOUR_SPAN)
            {
                string msg = String.Format("Difference between date filters exceeds {0} hours. Querying data may take significant amount of time. Do You want to continue?", DATES_HOUR_SPAN);
                MessageBoxResult result = MessageBox.Show(
                    msg,
                    "Information",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information
                    );
                if (result == MessageBoxResult.OK)
                    return true;
                else
                    return false;
            }
            return true;
        }

        private bool ValidateLimit()
        {
            int limit = _settings.Limit;
            if (limit > 1000)
            {
                string msg = "Setting up too big limit may lead to performance problems. Do You want to continue?";
                MessageBoxResult result = MessageBox.Show(
                    msg,
                    "Information",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information
                    );
                if (result == MessageBoxResult.OK)
                    return true;
                else
                    return false;
            }
            return true;
        }

        void FinishLoadingLogs(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadLogsResult result = e.Result as LoadLogsResult;
            if (result.TaskId == _currentTaskId)
            {
                pnlLogs.Children.Clear();
                if (result.ConnectionError)
                {
                    SetStatus("Error.");
                    Label lblError = new Label();
                    lblError.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                    lblError.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                    lblError.Padding = new Thickness(0, 250, 0, 0);
                    lblError.Content = CONNECTION_ERROR;
                    pnlLogs.Children.Add(lblError);
                }
                else
                {
                    if (result.Logs.Count > 0)
                    {
                        string msg = String.Format("Logs loaded: {0}. {1}", result.TotalLogsFound, GetAreasCount(result.AreasCount));
                        SetStatus(msg);
                        int index = 0;
                        foreach (var log in result.Logs)
                        {
                            pnlLogs.Children.Add(new LogRow(index++, log));
                        }
                    }
                    else
                    {
                        SetStatus("No logs found.");
                        Label lblNoLogs = new Label();
                        lblNoLogs.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        lblNoLogs.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        lblNoLogs.Padding = new Thickness(0, 250, 0, 0);
                        lblNoLogs.Content = String.Format("No records matching following filters found: \n\n{0}", GetFilters());
                        pnlLogs.Children.Add(lblNoLogs);
                    }
                }
                scvLogs.ScrollToHome();
                UnlockUi();
            }
            WorkerTask task = _bgWorkers.FirstOrDefault(x => x.TaskId == result.TaskId);
            _bgWorkers.Remove(task);
        }

        private string GetFilters()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("Created \"from\": {0}\n", _settings.DateFrom.ToString("dd-MM-yyyy HH:mm"));
            result.AppendFormat("Created \"to\": {0}\n", _settings.DateTo.ToString("dd-MM-yyyy HH:mm"));
            result.AppendFormat("Areas: as selected\n");
            if (!String.IsNullOrWhiteSpace(_settings.ContainingText))
                result.AppendFormat("Containing text: {0}\n", _settings.ContainingText);
            if (!String.IsNullOrWhiteSpace(_settings.ClassName))
                result.AppendFormat("Class name: {0}\n", _settings.ClassName);
            if (!String.IsNullOrWhiteSpace(_settings.MethodName))
                result.AppendFormat("Method name: {0}\n", _settings.MethodName);
            if (!String.IsNullOrWhiteSpace(_settings.SessionId))
                result.AppendFormat("Session id: {0}\n", _settings.SessionId);
            if (!String.IsNullOrWhiteSpace(_settings.ThreadId))
                result.AppendFormat("Thread id: {0}\n", _settings.ThreadId);
            if (!String.IsNullOrWhiteSpace(_settings.UserAgent))
                result.AppendFormat("User agent: {0}\n", _settings.UserAgent);
            if (!String.IsNullOrWhiteSpace(_settings.UserHost))
                result.AppendFormat("User host {0}\n", _settings.UserHost);

            return result.ToString();
        }

        private string GetAreasCount(Dictionary<string, int> areasCount)
        {
            StringBuilder result = new StringBuilder();

            foreach (string area in areasCount.Keys)
            {
                int areaCount = areasCount[area];
                result.AppendFormat("{0}: {1}. ", area, areaCount);
            }

            return result.ToString();
        }

        private void SetStatus(string msg)
        {
            sbiStatus.Content = msg;
        }

        void StartLoadingLogs(object sender, DoWorkEventArgs e)
        {
            QLogBrowserSettings settings = e.Argument as QLogBrowserSettings;
            LoadLogsResult result = _logService.LoadLogs(_settings);
            e.Result = result;
        }

        private void UpdateSettings()
        {
            _settings.ClassName = _advancedFiltersWindow.Class;
            _settings.ContainingText = txtContainingText.Text;
            _settings.DateFrom = GetDate(true);
            _settings.DateTo = GetDate(false);
            if (_settings.DateTo > DateTime.Now)
                _settings.DateTo = DateTime.Now;
            int limit = 0;
            Int32.TryParse(txtLimit.Text, out limit);
            _settings.Limit = limit;
            _settings.MethodName = _advancedFiltersWindow.Method;
            _settings.SessionId = _advancedFiltersWindow.SessionId;
            _settings.ThreadId = _advancedFiltersWindow.ThreadId;
            _settings.UserAgent = _advancedFiltersWindow.UserAgent;
            _settings.UserHost = _advancedFiltersWindow.UserHost;
            _settings.InstanceId = _advancedFiltersWindow.InstanceId;
            _settings.DeploymentId = _advancedFiltersWindow.DeploymentId;
            UpdateSettingsAreas();
        }

        private DateTime GetDate(bool lower)
        {
            try
            {
                DateTime date1Picker = dtDateFrom.SelectedDate.Value;
                DateTime date1HoursText = DateTime.Parse("2000-01-01 " + txtHourFrom.Text);
                DateTime date1 = new DateTime(date1Picker.Year, date1Picker.Month, date1Picker.Day, date1HoursText.Hour, date1HoursText.Minute, 0, DateTimeKind.Local);

                DateTime date2Picker = dtDateTo.SelectedDate.Value;
                DateTime date2HoursText = DateTime.Parse("2000-01-01 " + txtHourTo.Text);
                DateTime date2 = new DateTime(date2Picker.Year, date2Picker.Month, date2Picker.Day, date2HoursText.Hour, date2HoursText.Minute, 0, DateTimeKind.Local);

                if (lower)
                    return (date1 > date2) ? date2 : date1;
                else
                    return (date1 > date2) ? date1 : date2;
            }
            catch (Exception)
            {
                if (lower)
                    return DateTime.Now.AddDays(-1);
                else
                    return DateTime.Now;
            }
        }

        private void txtHourFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            DateTime result = DateTime.MinValue;
            if (!DateTime.TryParse("2000-01-01 " + txtHourFrom.Text, out result))
            {
                txtHourFrom.Text = DateTime.Now.ToString("HH:mm");
            }
        }

        private void txtHourTo_LostFocus(object sender, RoutedEventArgs e)
        {
            DateTime result = DateTime.MinValue;
            if (!DateTime.TryParse("2000-01-01 " + txtHourTo.Text, out result))
            {
                txtHourTo.Text = DateTime.Now.ToString("HH:mm");
            }
        }

        private void txtLimit_LostFocus(object sender, RoutedEventArgs e)
        {
            int limit = STANDARD_LOGS_LIMIT;
            if (Int32.TryParse(txtLimit.Text, out limit))
                if (limit > 0)
                    return;
            txtLimit.Text = STANDARD_LOGS_LIMIT.ToString();
        }

        private void UpdateSettingsAreas()
        {
            _settings.Areas.Clear();
            foreach (var item in pnlAreas.Children)
            {
                CheckBox cbx = item as CheckBox;
                if (cbx != _selectAllCheckbox)
                {
                    Color color = (cbx.Foreground as SolidColorBrush).Color;
                    _settings.Areas.Add(new QArea() { Name = cbx.Content.ToString(), ColorR = color.R, ColorG = color.G, ColorB = color.B, IsSelected = cbx.IsChecked ?? false });
                }
            }
        }

        private void SaveSettings()
        {
            _settings.Connections.ForEach(x => x.AccountKey = SecurityHelper.Encrypt(x.AccountKey));
            Properties.Settings.Default.QLogBrowserSettings = SerializationHelper.Serialize(_settings);
            Properties.Settings.Default.Save();
            _settings.Connections.ForEach(x => x.AccountKey = SecurityHelper.Decrypt(x.AccountKey));
        }

        private void PresentAreas()
        {
            pnlAreas.Children.Clear();
            pnlAreas.Children.Add(_selectAllCheckbox);
            AreasList areasList = new AreasList(_settings.Areas);
            foreach (var area in areasList.GetOrderedAreas())
            {
                pnlAreas.Children.Add(new CheckBox() { Content = area.Name, Foreground = new SolidColorBrush(Color.FromArgb(255, area.ColorR, area.ColorG, area.ColorB)), IsChecked = area.IsSelected });
            }
        }

        private void InitializeAreasCheckbox()
        {
            _selectAllCheckbox = cbxSelectAll;
        }

        private void LoadSettings()
        {
            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.QLogBrowserSettings))
            {
                _settings = GetDefaultSettings();
            }
            else
            {
                _settings = SerializationHelper.Deserialize(Properties.Settings.Default.QLogBrowserSettings);
                _settings.Connections.ForEach(x => x.AccountKey = SecurityHelper.Decrypt(x.AccountKey));
            }
            _settings.Areas = GetDefaultAreas();
        }

        private QLogBrowserSettings GetDefaultSettings()
        {
            QLogBrowserSettings defaultSettings = new QLogBrowserSettings();
            defaultSettings.ClassName = "";
            defaultSettings.Connections = new List<StorageConnection>();
            defaultSettings.ContainingText = "";
            defaultSettings.DateFrom = DateTime.Now.AddDays(-1);
            defaultSettings.DateTo = DateTime.Now.AddDays(1);
            defaultSettings.Limit = 100;
            defaultSettings.MethodName = "";
            defaultSettings.SessionId = "";
            defaultSettings.ThreadId = "";
            defaultSettings.UserAgent = "";
            defaultSettings.UserHost = "";
            defaultSettings.TaskId = Guid.Empty;
            
            return defaultSettings;
        }

        private List<QArea> GetDefaultAreas()
        {
            List<QArea> result = new List<QArea>();
            result.Add(new QArea() { Name = "QTrace", IsSelected = true, ColorR = 147, ColorG = 188, ColorB = 255 });
            result.Add(new QArea() { Name = "QDebug", IsSelected = true, ColorR = 77, ColorG = 82, ColorB = 255 });
            result.Add(new QArea() { Name = "QInfo", IsSelected = true, ColorR = 10, ColorG = 152, ColorB = 0 });
            result.Add(new QArea() { Name = "QWarn", IsSelected = true, ColorR = 206, ColorG = 129, ColorB = 0 });
            result.Add(new QArea() { Name = "QError", IsSelected = true, ColorR = 219, ColorG = 36, ColorB = 36 });
            result.Add(new QArea() { Name = "QCritical", IsSelected = true, ColorR = 255, ColorG = 0, ColorB = 0 });
            return result;
        }

        private void SelectAllAreas(object sender, RoutedEventArgs e)
        {
            foreach (UIElement checkbox in pnlAreas.Children)
            {
                CheckBox cbx = checkbox as CheckBox;
                cbx.IsChecked = true;
            }
        }

        private void UnselectAllAreas(object sender, RoutedEventArgs e)
        {
            foreach (UIElement checkbox in pnlAreas.Children)
            {
                CheckBox cbx = checkbox as CheckBox;
                cbx.IsChecked = false;
            }
        }

        private void OpenStorageConnectionWindow(object sender, RoutedEventArgs e)
        {
            if (!_locked)
            {
                OpenStorageConnectionWindow();
            }
        }

        private void OpenAdvancedFiltersWindow(object sender, RoutedEventArgs e)
        {
            if (!_locked)
            {
                _advancedFiltersWindow.ShowDialog();
                if (_advancedFiltersWindow.OkPressed)
                    LoadLogs();
            }
        }

        private void OpenAboutWindow(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void OpenDeleteLogsWindow(object sender, RoutedEventArgs e)
        {
            if (!_locked)
            {
                DeleteLogsWindow deleteLogsWindow = new DeleteLogsWindow();
                deleteLogsWindow.ShowDialog();
                if (deleteLogsWindow.DoDelete)
                {
                    SetStatus("Deleting logs...");
                    WorkerTask task = new WorkerTask();
                    task.TaskId = Guid.NewGuid();
                    _currentTaskId = task.TaskId;
                    task.Worker = new BackgroundWorker();
                    task.Worker.DoWork += StartDeletingLogs;
                    task.Worker.RunWorkerCompleted += FinishDeletingLogs;
                    _settings.TaskId = task.TaskId;
                    _settings.DeleteLogsNoDays = deleteLogsWindow.NoDays;
                    LockUi();
                    btnScan.IsEnabled = false;
                    btnLoadLogs.IsEnabled = false;
                    task.Worker.RunWorkerAsync(CloneHelper.Clone<QLogBrowserSettings>(_settings));
                    _bgWorkers.Add(task);
                }
            }
        }

        private void FinishDeletingLogs(object sender, RunWorkerCompletedEventArgs e)
        {
            DeleteLogsResult result = e.Result as DeleteLogsResult;
            if (result.TaskId == _currentTaskId)
            {
                if (!result.ConnectionError)
                {
                    SetStatus("Finished logs deletion.");
                }
                else
                {
                    SetStatus(CONNECTION_ERROR);
                }
                UnlockUi();
            }
            WorkerTask task = _bgWorkers.FirstOrDefault(x => x.TaskId == result.TaskId);
            _bgWorkers.Remove(task);
        }

        private void StartDeletingLogs(object sender, DoWorkEventArgs e)
        {
            QLogBrowserSettings settings = e.Argument as QLogBrowserSettings;
            DeleteLogsResult result = _logService.DeleteLogsOlderThan(_settings);
            e.Result = result;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetAreas();
        }

        private void ResetAreas()
        {
            _settings.Areas = GetDefaultAreas();
            PresentAreas();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DoApplicationCleanup();
            Application.Current.Shutdown();
        }

        private void DoApplicationCleanup()
        {
            UpdateSettings();
            SaveSettings();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DoApplicationCleanup();
            Application.Current.Shutdown();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            if (btnScan.Content.ToString() == CANCEL)
            {
                _currentTaskId = Guid.NewGuid();
                UnlockUi();
                SetStatus("Cancelled.");
            }
            else
            {
                ResetAreas();
                Scan();
            }
        }

        private void Scan()
        {
            UpdateSettings();
            if (ValidateFilters())
            {
                SetStatus("Scanning areas...");
                WorkerTask task = new WorkerTask();
                task.TaskId = Guid.NewGuid();
                _currentTaskId = task.TaskId;
                task.Worker = new BackgroundWorker();
                task.Worker.DoWork += StartScanningAreas;
                task.Worker.RunWorkerCompleted += FinishScanningAreas;
                _settings.TaskId = task.TaskId;
                LockUi();
                btnScan.Content = CANCEL;
                btnLoadLogs.IsEnabled = false;
                task.Worker.RunWorkerAsync(CloneHelper.Clone<QLogBrowserSettings>(_settings));
                _bgWorkers.Add(task);
            }
        }

        private void FinishScanningAreas(object sender, RunWorkerCompletedEventArgs e)
        {
            ScanAreasResult result = e.Result as ScanAreasResult;
            if (result.TaskId == _currentTaskId)
            {
                if (!result.ConnectionError)
                {
                    _settings.Areas = result.Areas;
                    PresentAreas();
                    SetStatus("Finished scanning areas.");
                }
                else
                {
                    SetStatus(CONNECTION_ERROR);
                }
                UnlockUi();
            }
            WorkerTask task = _bgWorkers.FirstOrDefault(x => x.TaskId == result.TaskId);
            _bgWorkers.Remove(task);
        }

        private void StartScanningAreas(object sender, DoWorkEventArgs e)
        {
            QLogBrowserSettings settings = e.Argument as QLogBrowserSettings;
            ScanAreasResult result = _logService.ScanAreas(settings);
            e.Result = result;
        }

        private void btnLoadLogs_Click(object sender, RoutedEventArgs e)
        {
            if (btnLoadLogs.Content.ToString() == CANCEL)
            {
                _currentTaskId = Guid.NewGuid();
                UnlockUi();
                SetStatus("Cancelled.");
            }
            else
            {
                LoadLogs();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddAreaWindow addAreaWindow = new AddAreaWindow();
            addAreaWindow.ShowDialog();
            if (addAreaWindow.AddClicked)
            {
                if (!String.IsNullOrWhiteSpace(addAreaWindow.AreaName))
                {
                    CheckBox cbx = new CheckBox();
                    cbx.Content = addAreaWindow.AreaName;
                    cbx.IsChecked = true;
                    pnlAreas.Children.Add(cbx);
                }
            }
        }
    }
}
