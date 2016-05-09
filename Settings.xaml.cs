using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SpotifyAdBlocker
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private static readonly ProxyController Controller = new ProxyController();
        private readonly MaterialDesignThemes.Wpf.PaletteHelper Theme = new MaterialDesignThemes.Wpf.PaletteHelper();

        private System.Windows.Threading.DispatcherTimer checkTimer = new System.Windows.Threading.DispatcherTimer();

        private string lastTextIP;

        public Settings()
        {
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            InitializeComponent();
            loadSettings();
            checkTimer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            loadSettings();
        }

        private void btnExitAndSave_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            loadSettings();
            Close();
        }

        #region Settings
        private void loadSettings()
        {
            chkEnableWhitelist.IsChecked = Convert.ToBoolean(Properties.Settings.Default["EnableWhitelist"]);
            chkEnableBlacklist.IsChecked = Convert.ToBoolean(Properties.Settings.Default["EnableBlacklist"]);
            chkCustomBlock.IsChecked = Convert.ToBoolean(Properties.Settings.Default["CustomBlock"]);
            chkMinToTray.IsChecked = Convert.ToBoolean(Properties.Settings.Default["MinToTray"]);
            chkDarkTheme.IsChecked = Convert.ToBoolean(Properties.Settings.Default["isDark"]);
            chkAutoStartProxy.IsChecked = Convert.ToBoolean(Properties.Settings.Default["AutoStartProxy"]);
            chkDismissWarnings.IsChecked = Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"]);
            chkAutoMinToTray.IsChecked = Convert.ToBoolean(Properties.Settings.Default["AutoMinToTray"]);
            chkAutoShowConsole.IsChecked = Convert.ToBoolean(Properties.Settings.Default["AutoShowConsole"]);
            txtPort.Text = Properties.Settings.Default["proxyIP"].ToString();
            txtPort.Text = Properties.Settings.Default["proxyPort"].ToString();
            Theme.SetLightDark(Convert.ToBoolean(Properties.Settings.Default["isDark"]));
            Theme.ReplacePrimaryColor(Convert.ToString(Properties.Settings.Default["themePrimary"]));
            Theme.ReplaceAccentColor(Convert.ToString(Properties.Settings.Default["themeAccent"]));
            btnSaveSettings.IsEnabled = btnExitAndSave.IsEnabled = false;
            lblStatus.Content = "Settings up-to-date.";
        }
        private void saveSettings()
        {
            btnSaveSettings.IsEnabled = false;
            Properties.Settings.Default["EnableBlackList"] = chkEnableBlacklist.IsChecked;
            Properties.Settings.Default["EnableWhiteList"] = chkEnableWhitelist.IsChecked;
            Properties.Settings.Default["CustomBlock"] = chkCustomBlock.IsChecked;
            Properties.Settings.Default["MinToTray"] = chkMinToTray.IsChecked;
            Properties.Settings.Default["isDark"] = chkDarkTheme.IsChecked;
            Properties.Settings.Default["AutoStartProxy"] = chkAutoStartProxy.IsChecked;
            Properties.Settings.Default["DismissWarnings"] = chkDismissWarnings.IsChecked;
            Properties.Settings.Default["AutoMinToTray"] = chkAutoMinToTray.IsChecked;
            Properties.Settings.Default["autoShowConsole"] = chkAutoShowConsole.IsChecked;
            Properties.Settings.Default["proxyIP"] = txtIP.Text.ToString();
            Properties.Settings.Default["proxyPort"] = int.Parse(txtPort.Text);
            Properties.Settings.Default.Save();
            Theme.SetLightDark(Convert.ToBoolean(Properties.Settings.Default["isDark"]));
            Theme.ReplacePrimaryColor(Convert.ToString(Properties.Settings.Default["themePrimary"]));
            Theme.ReplaceAccentColor(Convert.ToString(Properties.Settings.Default["themeAccent"]));
            lblStatus.Content = "Settings up-to-date.";
            Controller.loadSettings(false);
        }
        private void checkForChanges()
        {
            if (Properties.Settings.Default["proxyIP"].ToString() == txtIP.Text &&
                Properties.Settings.Default["proxyPort"].ToString() == txtPort.Text &&
                Convert.ToBoolean(Properties.Settings.Default["AutoMinToTray"]) == chkAutoMinToTray.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["AutoShowConsole"]) == chkAutoShowConsole.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"]) == chkDismissWarnings.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["MinToTray"]) == chkMinToTray.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["AutoStartProxy"]) == chkAutoStartProxy.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["isDark"]) == chkDarkTheme.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["EnableBlackList"].ToString()) == chkEnableBlacklist.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["EnableWhiteList"].ToString()) == chkEnableWhitelist.IsChecked &&
                Convert.ToBoolean(Properties.Settings.Default["CustomBlock"].ToString()) == chkCustomBlock.IsChecked)
            {
                btnSaveSettings.IsEnabled = btnExitAndSave.IsEnabled = false;
                lblStatus.Content = "Settings up-to-date.";
            }
            else
            {
                // Not saved
                btnSaveSettings.IsEnabled = btnExitAndSave.IsEnabled = true;
                lblStatus.Content = "Not saved.";
            }
        }

        private bool IsTextAllowed(string text)
        {
            if (text == " ") return false;
            if (text == "." && lastTextIP == ".") return false;
            lastTextIP = text;
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
        private bool IsTextAllowed2(string text)
        {
            if (txtPort.Text.Length >= 5) return false;
            if (text == " ") return false;
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
        private void txtIP_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void txtPort_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed2(e.Text);
        }
        private void chkMinToTray_Click(object sender, RoutedEventArgs e)
        {
            checkForChanges();
        }
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            checkForChanges();
            if ((txtIP.Text.EndsWith(".") || txtIP.Text.EndsWith(" ")) && !txtIP.IsFocused) txtIP.Text = txtIP.Text.Remove(txtIP.Text.Length - 1, 1);
            if ((txtIP.Text.StartsWith(".") || txtIP.Text.StartsWith(" ")) && !txtIP.IsFocused) txtIP.Text = txtIP.Text.Remove(0, 1);
        }
        #endregion
    }
}
