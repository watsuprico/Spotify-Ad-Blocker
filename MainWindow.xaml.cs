using System;
using System.Runtime.InteropServices;
using System.Windows;
using MaterialDesignThemes;

namespace SpotifyAdBlocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Console settings
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole(); // Create console window
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow(); // Get console window handle
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        private bool consoleVisable = false;
        #endregion

        #region Misc
        private static readonly ProxyController Controller = new ProxyController();
        private static readonly Settings settings = new Settings();
        private readonly MaterialDesignThemes.Wpf.PaletteHelper Theme = new MaterialDesignThemes.Wpf.PaletteHelper();

        private System.Windows.Forms.NotifyIcon NoteIcon = new System.Windows.Forms.NotifyIcon();
        private System.Windows.Forms.ContextMenu NoteMenu = new System.Windows.Forms.ContextMenu();
        private System.Windows.Forms.MenuItem NoteMenuShutdown = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.MenuItem NoteMenuRestart = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.MenuItem NoteMenuConsole = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.MenuItem NoteMenuExit = new System.Windows.Forms.MenuItem();
        
        private bool settingOpened = false;
        #endregion

        #region Window items
        public MainWindow()
        {
            //InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            try {
                NoteMenuShutdown.Text = "Shutdown proxy";
                NoteMenuShutdown.Click += NoteMenuShutdown_Click;
                NoteMenuRestart.Text = "Restart proxy";
                NoteMenuRestart.Click += NoteMenuRestart_Click;
                NoteMenuConsole.Text = "Show console";
                NoteMenuConsole.Click += NoteMenuConsole_Click;
                NoteMenuExit.Text = "Shutdown and exit";
                NoteMenuExit.Click += NoteMenuExit_Click;
                NoteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { NoteMenuShutdown });
                NoteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { NoteMenuRestart });
                NoteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { NoteMenuConsole });
                NoteMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { NoteMenuExit });
                if (System.IO.File.Exists(Environment.CurrentDirectory + "/icon.ico")) { NoteIcon.Icon = new System.Drawing.Icon(Environment.CurrentDirectory + "/icon.ico"); NoteIcon.Text = "Double click to show."; }
                else { NoteIcon.Icon = System.Drawing.SystemIcons.Application; NoteIcon.Text = "Missing icon.ico file! Double click to open."; }
                NoteIcon.DoubleClick += NoteIcon_DoubleClick;
                NoteIcon.Visible = true;
                NoteIcon.ContextMenu = NoteMenu;

                ShowConsole();
                if (!Convert.ToBoolean(Properties.Settings.Default["AutoShowConsole"])) HideConsole();
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
                if (Convert.ToBoolean(Properties.Settings.Default["AutoStartProxy"])) {
                    Controller.StartProxy();
                    updateProxyStatus();
                }
                else {
                    lblConnection.Content = "Server offline";
                    btnShutdown.Content = "Start";
                    NoteMenuShutdown.Text = "Start proxy";
                    btnShutdown.ToolTip = "Start the proxy.";
                    btnShutdownAndExit.Content = "Exit";
                    btnRestart.IsEnabled = NoteMenuRestart.Enabled = false;
                }
                Theme.SetLightDark(Convert.ToBoolean(Properties.Settings.Default["isDark"]));
                Theme.ReplacePrimaryColor(Convert.ToString(Properties.Settings.Default["themePrimary"]));
                Theme.ReplaceAccentColor(Convert.ToString(Properties.Settings.Default["themeAccent"]));
                if (Convert.ToBoolean(Properties.Settings.Default["AutoMinToTray"])) Hide();
            }
            catch (Exception c)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) MessageBox.Show(c.ToString(), "Alert!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Convert.ToBoolean(Properties.Settings.Default["MinToTray"]))
            {
                if (settings.IsVisible)
                {
                    settings.Hide();
                    settingOpened = true;
                }
                else
                    settingOpened = false;
                Hide();
                e.Cancel = true;
            }
            else
            {
                if (Controller.serverRunning && !Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) MessageBox.Show("Please note that if you do not disable connecting to a proxy in Spotify, it will not be able to connect until you start Spotify Ad Blocker back up.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Controller.Stop();
                Application.Current.Shutdown();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            base.OnStateChanged(e);
        }

        private void updateProxyStatus()
        {
            if (Controller.serverRunning)
            {
                lblConnection.Content = "Proxy type: HTTP | Host: " + Controller.connectorIP + " | Port: " + Controller.connectorPort;
                btnShutdown.Content = "Shutdown";
                NoteMenuShutdown.Text = "Shutdown proxy";
                btnShutdown.ToolTip = "Shutdown the proxy";
                btnShutdownAndExit.Content = "Shutdown and exit";
                btnRestart.IsEnabled = NoteMenuRestart.Enabled = true;
            }
            else
            {
                lblConnection.Content = "Server offline";
                btnShutdown.Content = "Start";
                NoteMenuShutdown.Text = "Start proxy";
                btnShutdown.ToolTip = "Start the proxy.";
                btnShutdownAndExit.Content = "Exit";
                btnRestart.IsEnabled = NoteMenuRestart.Enabled = false;
            }
        }
        #endregion


        #region Console
        private void ShowConsole()
        {
            consoleVisable = true;
            btnToggleConsole.Content = NoteMenuConsole.Text = "Hide console";
            var handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                AllocConsole();
            else
                ShowWindow(handle, SW_SHOW);
        }
        private void HideConsole()
        {
            consoleVisable = false;
            btnToggleConsole.Content = NoteMenuConsole.Text = "Show console";
            var handle = GetConsoleWindow();
            if (handle != null)
                ShowWindow(handle, SW_HIDE);
        }
        #endregion


        #region UI
        #region Window UI
        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (Controller.serverRunning)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) MessageBox.Show("Please note that if you do not disable connecting to a proxy in Spotify, it will not be able to connect until you start the proxy back up.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Controller.Stop();
                lblConnection.Content = "Server offline";
                btnShutdown.Content = "Start";
                NoteMenuShutdown.Text = "Start proxy";
                btnShutdown.ToolTip = "Start the proxy.";
                btnShutdownAndExit.Content = "Exit";
                btnRestart.IsEnabled = NoteMenuRestart.Enabled = false;
            }
            else
            {
                Controller.StartProxy();
                updateProxyStatus();
            }
        }
        private void btnShutdownAndExit_Click(object sender, RoutedEventArgs e)
        {
            if (Controller.serverRunning)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) MessageBox.Show("Please note that if you do not disable connecting to a proxy in Spotify, it will not be able to connect until you start Spotify Ad Blocker back up.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Controller.Stop();
            }
            Application.Current.Shutdown();
        }
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            lblConnection.Content = "Restarting...";
            NoteMenuShutdown.Enabled = NoteMenuRestart.Enabled = btnShutdown.IsEnabled = false;
            Controller.Restart();
            NoteMenuShutdown.Enabled = NoteMenuRestart.Enabled = btnShutdown.IsEnabled = true;
            updateProxyStatus();
        }

        private void btnToggleConsole_Click(object sender, RoutedEventArgs e)
        {
            if (consoleVisable) HideConsole();
            else ShowConsole();
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            settings.Show();
            settings.Focus();
        }
        #endregion

        #region Tray Icon UI
        private void NoteIcon_DoubleClick(object sender, EventArgs e)
        {
            if (IsVisible) Close();
            else
            {
                Show();
                Focus();
                if (settingOpened)
                {
                    settings.Show();
                    settings.Focus();
                }
                WindowState = WindowState.Normal;
            }
        }
        private void NoteMenuShutdown_Click(object sender, EventArgs e)
        {
            if (Controller.serverRunning)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) MessageBox.Show("Please note that if you do not disable connecting to a proxy in Spotify, it will not be able to connect until you start the proxy back up.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Controller.Stop();
                lblConnection.Content = "Server offline";
                btnShutdown.Content = "Start";
                NoteMenuShutdown.Text = "Start proxy";
                btnShutdown.ToolTip = "Start the proxy.";
                btnShutdownAndExit.Content = "Exit";
                btnRestart.IsEnabled = NoteMenuRestart.Enabled = false;
            }
            else
            {
                Controller.StartProxy();
                updateProxyStatus();
            }
        }
        private void NoteMenuRestart_Click(object sender, EventArgs e)
        {
            lblConnection.Content = "Restarting...";
            NoteMenuShutdown.Enabled = NoteMenuRestart.Enabled = btnShutdown.IsEnabled = false;
            Controller.Restart();
            NoteMenuShutdown.Enabled = NoteMenuRestart.Enabled = btnShutdown.IsEnabled = true;
            updateProxyStatus();
        }
        private void NoteMenuConsole_Click(object sender, EventArgs e)
        {
            if (consoleVisable) HideConsole();
            else ShowConsole();
        }
        private void NoteMenuExit_Click(object sender, EventArgs e)
        {
            if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) MessageBox.Show("Please note that if you do not disable connecting to a proxy in Spotify, it will not be able to connect until you start Spotify Ad Blocker back up.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Warning);
            Controller.Stop();
            Application.Current.Shutdown();
        }
        #endregion
        #endregion
    }
}