namespace SmushMySite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Forms;
    using System.Xml;
    using Logic;
    using Logic.Entities;
    using Logic.Interfaces;
    using MahApps.Metro.Controls;
    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SmushSite : MetroWindow
    {
        private readonly IUtils _utils;
        private readonly ISmushLogic _smushLogic;
        private readonly IProxyHelper _proxyHelper;
        IEnumerable<SquishedImage> _images = new List<SquishedImage>();
        
        public SmushSite()
        {
            InitializeComponent();
            _utils = new Utils();
            _smushLogic = new SmushLogic();
            _proxyHelper = new ProxyHelper();

            // Set the default output directory
            txtOutputUrl.Text = @"C:\Temp";
            lblComplete.Visibility = Visibility.Hidden;
            pgBar.Visibility = Visibility.Hidden;
            imgTick.Visibility = Visibility.Hidden;
            btnInfo.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Smush images for URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSmush_Click(object sender, RoutedEventArgs e)
        {
            UpdateProgressBar(10);
            HideControls();

            try
            {
                string siteMapUrl = txtSiteMapUrl.Text;

                // Check if the user has a proxy
                if (_proxyHelper.HasProxy())
                {
                    // Open new window and prompt for user details
                    ProxyAuthentication authentication = new ProxyAuthentication();
                    authentication.Show();

                    // Hide the progress bar.
                    HideProgressBar();

                    return;
                }

                // go to sitemap
                WebClient siteMapClient = new WebClient();
                string siteMapString = siteMapClient.DownloadString(siteMapUrl);

                // get all links from the sitemap
                XmlDocument document = new XmlDocument();
                document.LoadXml(siteMapString);

                XmlNodeList xmlNodeList = document.GetElementsByTagName("loc");

                // Process the images in the xml list
                List<SquishedImage> squishedCssImages = new List<SquishedImage>();
                IEnumerable<string> imagesInSiteMap = _smushLogic.ProcessImagesForXmlList(xmlNodeList, siteMapUrl, ref squishedCssImages);
                
                UpdateProgressBar(50);

                _images = _smushLogic.ProcessImages(imagesInSiteMap, siteMapUrl);

                // Join the two lists.
                _images = squishedCssImages.Union(_images);

                // Loop through the list and save
                _utils.DownloadAndSave(_images, txtOutputUrl.Text);

                // Add the complete images
                lblComplete.Visibility = Visibility.Visible;
                imgTick.Visibility = Visibility.Visible;

                // Decide Whether or not to display the Info button
                ShouldDisplayInfoButton();

                // Hide the progress bar
                UpdateProgressBar(100);
                HideProgressBar();

                // Calculate the page and image stats
                if (_images.Count() != 0)
                {
                    double totalBytesSavings = Math.Round(_utils.CalculateStatistics(_images) / 1024d, 2);
                    MessageBox.Show("You saved: " + totalBytesSavings + " KB" + Environment.NewLine + "Across " + _images.Count() + " images");
                }

            }
            catch (Exception exception)
            {
                HideProgressBar();
                lblError.Content = exception.ToString();
            }
        }

        /// <summary>
        /// Hides the controls
        /// </summary>
        private void HideControls()
        {
            pgBar.Visibility = Visibility.Visible;
            lblError.Content = string.Empty;
            pgBar.Refresh();

            lblError.Content = string.Empty;
        }

        /// <summary>
        /// Hides the progress bar
        /// </summary>
        public void HideProgressBar()
        {
            pgBar.Visibility = Visibility.Hidden;
            pgBar.Refresh();
        }

        /// <summary>
        /// Checks whether or not to display the info button. This
        /// is decided on the error details of an image.
        /// </summary>
        private void ShouldDisplayInfoButton()
        {
            if (_images.Where(squishedImage => squishedImage != null).Any(squishedImage => squishedImage.error != null))
            {
                btnInfo.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Opens a file dialog to browse
        /// for output path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            lblError.Content = string.Empty;

            FolderBrowserDialog fileDialog = new FolderBrowserDialog { };

            DialogResult dialogResult = fileDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputUrl.Text = fileDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Updates the progressbar on the page
        /// </summary>
        /// <param name="value"></param>
        public void UpdateProgressBar(int value)
        {
            pgBar.Value = value;
            pgBar.Refresh();
        }

        /// <summary>
        /// Displays the info details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_smushLogic.GetErrorReasons(_images));
        }
    }
}
