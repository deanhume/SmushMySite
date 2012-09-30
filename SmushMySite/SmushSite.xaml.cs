namespace SmushMySite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using Logic;
    using Logic.Entities;
    using Logic.Interfaces;
    using MahApps.Metro.Controls;
    using Extensions;
    using MessageBox = System.Windows.MessageBox;
    using System.Threading;
    using System.Xml;
    using System.Net;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SmushSite : MetroWindow
    {
        private readonly IUtils _utils;
        private readonly ICommonUtils _commonUtils;
        private readonly ISmushLogic _smushLogic;
        private readonly IProxyHelper _proxyHelper;
        IEnumerable<SquishedImage> _images = new List<SquishedImage>();

        #region ctor
        public SmushSite()
        {
            InitializeComponent();

            _utils = new Utils();
            _commonUtils = new CommonUtils();
            _smushLogic = new SmushLogic();
            _proxyHelper = new ProxyHelper();

            // Set the default output directory
            txtOutputUrl.Text = @"C:\Temp";

            lblComplete.ToggleLabel(false);
            imgTick.ToggleImage(false);
            btnInfo.ToggleButton(false);
        }

        #endregion

        /// <summary>
        /// Smush images for URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSmush_Click(object sender, RoutedEventArgs e)
        {
            HideControls();

            // Show progress bar
            progressRing.ToggleProgressRing(true);

            string siteMapUrl = txtSiteMapUrl.Text;
            string outputUrl = txtOutputUrl.Text;

            if (txtOutputUrl.Text == string.Empty)
            {
                lblError.Content = "Please choose an output directory";
                return;
            }

            // Check if the user has a proxy
            if (_proxyHelper.HasProxy())
            {
                // Open new window and prompt for user details
                ProxyAuthentication authentication = new ProxyAuthentication();
                authentication.Show();

                // Disable the progress ring
                progressRing.ToggleProgressRing(false);

                return;
            }

            // Start a separate thread and download the images
            Observable.Start(() => SmushAndDownloadImages(siteMapUrl, outputUrl));
        }

        /// <summary>
        /// Hides the controls
        /// </summary>
        private void HideControls()
        {
            lblError.Content = string.Empty;
            lblComplete.ToggleLabel(false);
            btnInfo.ToggleButton(false);
        }

        /// <summary>
        /// Checks whether or not to display the info button. This
        /// is decided on the error details of an image.
        /// </summary>
        private void ShouldDisplayInfoButton()
        {
            if (_images.Where(squishedImage => squishedImage != null).Any(squishedImage => squishedImage.error != null))
            {
                btnInfo.ToggleButton(true);
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
        /// Displays the info details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_smushLogic.GetErrorReasons(_images));
        }

        /// <summary>
        /// This is the main method that is called to download and perform
        /// the smushing logic against the sitemap.
        /// </summary>
        /// <param name="siteMapUrl"></param>
        /// <param name="outputUrl"></param>
        public void SmushAndDownloadImages(string siteMapUrl, string outputUrl)
        {
            string siteMapString;

            try
            {
                siteMapUrl = _commonUtils.AppendHttp(siteMapUrl);
                siteMapString = _commonUtils.DownloadFromUrl(siteMapUrl);


                // get all links from the sitemap
                XmlDocument document = new XmlDocument();
                document.LoadXml(siteMapString);

                XmlNodeList xmlNodeList = document.GetElementsByTagName("loc");

                // Process the images in the xml list
                List<SquishedImage> squishedCssImages = new List<SquishedImage>();
                IEnumerable<string> imagesInSiteMap = _smushLogic.ProcessImagesForXmlList(xmlNodeList, siteMapUrl, ref squishedCssImages);

                _images = _smushLogic.ProcessImages(imagesInSiteMap, siteMapUrl);

                // Join the two lists.
                _images = squishedCssImages.Union(_images);

                // Loop through the list and save
                _utils.DownloadAndSave(_images, outputUrl);

                // Add the complete images
                lblComplete.ToggleLabel(true, "Complete");
                imgTick.ToggleImage(true);

                // Decide Whether or not to display the Info button
                ShouldDisplayInfoButton();

                // Calculate the page and image stats
                if (_images.Count() != 0)
                {
                    double totalBytesSavings = Math.Round(_utils.CalculateStatistics(_images) / 1024d, 2);
                    MessageBox.Show("You saved: " + totalBytesSavings + " KB" + Environment.NewLine + "Across " + _images.Count() + " images");
                }

                progressRing.ToggleProgressRing(false);
            }
            catch (Exception e)
            {
                // Disable the progress ring
                progressRing.ToggleProgressRing(false);

                // Update the error label
                lblError.ToggleLabel(true, e.Message);

                // Get outta here
                return;
            }
        }
    }
}
