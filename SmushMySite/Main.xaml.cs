using MahApps.Metro.Controls;

namespace SmushMySite
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using Logic;
    using Logic.Interfaces;

    using System.Windows;

    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : MetroWindow
    {
        private readonly IProxyHelper _proxyHelper;
        public Main()
        {
            InitializeComponent();
            _proxyHelper = new ProxyHelper();
        }

        private void btnEntire_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new SmushSite();
            mainWindow.Show();
        }

        private void btnUrl_Click(object sender, RoutedEventArgs e)
        {
            var smushPage = new SmushMyPage();
            smushPage.Show();
        }


        private void About_Click(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            CheckForUpdates(true);
        }

        /// <summary>
        /// Used to check the codeplex site for any updates
        /// </summary>
        /// <param name="userRequested"></param>
        public void CheckForUpdates(bool userRequested)
        {
            // First check if the user is able to connect
            // Check if the user has a proxy
            if (_proxyHelper.HasProxy())
            {
                // Open new window and prompt for user details
                ProxyAuthentication authentication = new ProxyAuthentication();
                authentication.Show();

                return;
            }

            // Go ahead and download
            string url = Properties.Settings.Default.UpdateUrl;
            Thread thread = new Thread(delegate()
            {
                WebClient webClient = new WebClient();
                webClient.DownloadDataCompleted += this.OnDownloadDataCompleted;
                webClient.DownloadDataAsync(new Uri(url), userRequested);
            }) { IsBackground = true };

            thread.Start();
        }

        private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                bool userRequested = false;
                if (e.UserState is bool)
                    userRequested = (bool)e.UserState;

                Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                ReleaseInfoCollection releases = ReleaseInfoCollection.FromRss(e.Result);

                if (releases == null)
                {
                    if (userRequested)
                        MessageBox.Show("No updates were found.", AssemblyProduct);
                    return;
                }

                ReleaseInfo latest = releases.GetLatest(ReleaseStatus.Beta);

                if (latest == null)
                {
                    if (userRequested)
                        MessageBox.Show("No updates were found.", AssemblyProduct);

                    return;
                }

                if (latest.Version > currentVersion)
                {
                    // prompt user
                    string message = string.Format("{0} version {1} is available.\n\nWould you like to visit the release page?", AssemblyProduct, latest.Version.ToString());

                    MessageBoxResult messageBoxResult = MessageBox.Show(message, AssemblyProduct, MessageBoxButton.YesNo, MessageBoxImage.Question,MessageBoxResult.No);

                    if (!messageBoxResult.HasFlag(MessageBoxResult.Yes))
                    {
                        return;
                    }

                    Process.Start(latest.Url);
                    return;
                }
                MessageBox.Show(string.Format("{0} is up to date (v{1})", AssemblyProduct, currentVersion), AssemblyProduct);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
    }
}
