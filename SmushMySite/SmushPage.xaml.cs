using MahApps.Metro.Controls;

namespace SmushMySite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using MessageBox = System.Windows.MessageBox;
    using Logic;
    using Logic.Entities;
    using Logic.Interfaces;

    /// <summary>
    /// Interaction logic for SmushPage.xaml
    /// </summary>
    public partial class SmushPage : MetroWindow
    {
        private readonly IUtils _utils;
        private readonly ICommonUtils _commonUtils;
        private readonly ISmushLogic _smushLogic;
        private readonly IProxyHelper _proxyHelper;
        IEnumerable<SquishedImage> _images = new List<SquishedImage>();

        public SmushPage()
        {
            
            InitializeComponent();

            _utils = new Utils();
            _commonUtils = new CommonUtils();
            _smushLogic = new SmushLogic();
            _proxyHelper = new ProxyHelper();
            
            // Set the default output directory
            txtOutputUrl.Text = @"C:\Temp";
            lblComplete.Visibility = Visibility.Hidden;
            imgTick.Visibility = Visibility.Hidden;
            pgBar.Visibility = Visibility.Hidden;
            btnInfo.Visibility = Visibility.Hidden;

        }

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
                UpdateProgressBar(10);
                string pageUrl = txtPageUrl.Text;

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

                    // Hide the progress bar.
                    HideProgressBar();

                    return;
                }

                // Download from the page
                string downloadString = _commonUtils.DownloadWebPage(pageUrl);

                // Process the images in the CSS file.
                List<string> alreadyProcessed = new List<string>();
                List<SquishedImage> squishedCssImages = _smushLogic.ProcessCss(downloadString, pageUrl,
                                                                               ref alreadyProcessed);
                UpdateProgressBar(40);

                IEnumerable<string> imagesInHtmlString = _commonUtils.GetImagesInHtmlString(downloadString);

                lblError.Content = (imagesInHtmlString.Count() + squishedCssImages.Count()) + " Image(s) Found";
                lblError.Refresh();

                // Process the image with Yahoo Smush.it
                UpdateProgressBar(50);
                _images = _smushLogic.ProcessImages(imagesInHtmlString, pageUrl);

                // First join the css and image lists
                _images = squishedCssImages.Union(_images);

                // Download and save the images
                _utils.DownloadAndSave(_images, txtOutputUrl.Text);

                // Add the complete images
                lblComplete.Visibility = Visibility.Visible;
                imgTick.Visibility = Visibility.Visible;

                // Show the error button
                UpdateProgressBar(90);

                // Decide Whether or not to display the Info button
                ShouldDisplayInfoButton();

                pgBar.Visibility = Visibility.Hidden;
                pgBar.Refresh();

                // Calculate the page and image stats
                if (_images.Count() != 0)
                {
                    double totalBytesSavings = Math.Round(_utils.CalculateStatistics(_images)/1024d, 2);
                    MessageBox.Show("You saved: " + totalBytesSavings + " KB" + Environment.NewLine + "Across " +
                                    _images.Count() + " images");
                }
                
            }
            catch (Exception exception)
            {
                HideProgressBar();
                lblError.Content = exception.Message;
            }
        }

        /// <summary>
        /// Hides the controls
        /// </summary>
        private void HideControls()
        {
            lblError.Content = string.Empty;
            pgBar.Visibility = Visibility.Visible;
            lblComplete.Visibility = Visibility.Hidden;
            imgTick.Visibility = Visibility.Hidden;
            btnInfo.Visibility = Visibility.Hidden;
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
        /// Displays the info details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_smushLogic.GetErrorReasons(_images));
        }

        /// <summary>
        /// Updates the progressbar on the page
        /// </summary>
        /// <param name="value"></param>
        private void UpdateProgressBar(int value)
        {
            pgBar.Value = value;
            pgBar.Refresh();
        }
    }
}
