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

    /// <summary>
    /// Interaction logic for SmushPage.xaml
    /// </summary>
    public partial class SmushMyPage : MetroWindow
    {
        private readonly IUtils _utils;
        private readonly ICommonUtils _commonUtils;
        private readonly ISmushLogic _smushLogic;
        private readonly IProxyHelper _proxyHelper;
        IEnumerable<SquishedImage> _images = new List<SquishedImage>();

        #region ctor
        public SmushMyPage()
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
        /// Smush images for sitemap URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSmush_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideControls();

                // Show progress bar
                progressRing.ToggleProgressRing(true);

                string pageUrl = txtPageUrl.Text;
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

                    return;
                }

                // Start a separate thread and download the images
                Observable.Start(() => SmushAndDownloadImages(pageUrl, outputUrl));
            }
            catch (Exception exception)
            {
                lblError.Content = exception.Message;
            }
        }

        /// <summary>
        /// Smushes and downloads all the images on the web page.
        /// This method is a little longer than I would like
        /// and could be refactored.
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="outputUrl"></param>
        public void SmushAndDownloadImages(string pageUrl, string outputUrl)
        {
            // Download from the page
            string downloadString;
            try
            {
                pageUrl = _commonUtils.AppendHttp(pageUrl);
                downloadString = _commonUtils.DownloadFromUrl(pageUrl);
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

            // Process the images in the CSS file.
            List<string> alreadyProcessed = new List<string>();
            List<SquishedImage> squishedCssImages = _smushLogic.ProcessCss(downloadString, pageUrl, ref alreadyProcessed);

            IEnumerable<string> imagesInHtmlString = _commonUtils.GetImagesInHtmlString(downloadString);

            // Show an error message
            //lblError.ToggleLabel(true, (imagesInHtmlString.Count() + squishedCssImages.Count()) + " Image(s) Found");

            _images = _smushLogic.ProcessImages(imagesInHtmlString, pageUrl);

            // First join the css and image lists
            _images = squishedCssImages.Union(_images);

            // Download and save the images
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
    }
}
