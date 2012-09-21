using System.Linq;

namespace SmushMySite.Logic
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Xml;
    using Entities;
    using Interfaces;

    public class SmushLogic : ISmushLogic
    {
        private readonly IUtils _utils;
        private readonly ICommonUtils _commonUtils;

        public SmushLogic()
        {
            _utils = new Utils();
            _commonUtils = new CommonUtils();
        }

        /// <summary>
        /// Process all the images for the given list of images
        /// retrieved from the sites HTML
        /// </summary>
        /// <param name="imagesInHtmlString"></param>
        /// <param name="pageUrl"></param>
        public List<SquishedImage> ProcessImages(IEnumerable<string> imagesInHtmlString, string pageUrl)
        {
            List<SquishedImage> images = new List<SquishedImage>();

            //Parallel.ForEach(imagesInHtmlString, item => images.Add(ProcessSingleImage(item, pageUrl, true)));

            foreach (string s in imagesInHtmlString)
            {
                images.Add(ProcessSingleImage(s, pageUrl, true));
            }

            return images;
        }

        /// <summary>
        /// Processes a single image using
        /// the Yahoo Smush.it service
        /// </summary>
        /// <param name="imageUrl">The image to process</param>
        /// <param name="pageUrl">The page Url.</param>
        /// <returns></returns>
        private SquishedImage ProcessSingleImage(string imageUrl, string pageUrl)
        {
            // Remove any images from the list that arent true images
            if (_commonUtils.IsValidImage(imageUrl))
            {
                // Go to Smush.it
                string jsonString = _utils.SmushImageFromUrl(imageUrl, pageUrl);

                // Build up a list of the images and their locations
                SquishedImage squishedImage = _utils.ReadToObject(jsonString);

                return squishedImage;
            }
            return null;
        }

        /// <summary>
        /// Processes an image that is not from between a tag
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="pageUrl"></param>
        /// <param name="removeTag">Removes the tag string in a image Url</param>
        /// <returns></returns>
        public SquishedImage ProcessSingleImage(string imageUrl, string pageUrl, bool removeTag)
        {
            if (removeTag)
            {
                //Get the img src from the tag
                imageUrl = _utils.ExtractSrcFromImgTag(imageUrl);
            }

            return ProcessSingleImage(imageUrl, pageUrl);
        }

        /// <summary>
        /// Processes and returns a list of images in the CSS file.
        /// </summary>
        /// <param name="downloadString"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="alreadyProcessed"></param>
        public List<SquishedImage> ProcessCss(string downloadString, string sourceUrl, ref List<string> alreadyProcessed)
        {
            IEnumerable<string> imagesInCssString = new List<string>();

            // Retrieve the Css files from the html string
            List<string> extractedCss = _utils.ExtractCssHrefFromHtmlString(downloadString);

            // Loop through the extracted Css and process
            foreach (string cssFile in extractedCss)
            {
                if (_commonUtils.IsValidCssExtension(cssFile))
                {
                    if (!alreadyProcessed.Contains(cssFile))
                    {
                        // Add to already processed list
                        alreadyProcessed.Add(cssFile);

                        // TODO: Fix this
                        List<string> extractImagesFromCssFile = _utils.ExtractImagesFromCssFile(cssFile, sourceUrl);

                        // Join the new list with the existing image list.
                        imagesInCssString = extractImagesFromCssFile.Union(imagesInCssString);
                    }
                }
            }

            // Send the images through to Smush.it
            List<SquishedImage> images = new List<SquishedImage>();
            foreach (var imagePath in imagesInCssString)
            {
                images.Add(ProcessSingleImage(imagePath, sourceUrl, false));
            }

            return images;
        }

        /// <summary>
        /// Builds a string of the error reasons to display in an info 
        /// textbox to the user.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public string GetErrorReasons(IEnumerable<SquishedImage> images)
        {
            StringBuilder errorReasons = new StringBuilder();
            if (images.Count() > 0)
            {
                foreach (SquishedImage squishedImage in images)
                {
                    // Add the error reasons
                    if (squishedImage != null)
                        if (squishedImage.error != null)
                        {
                            errorReasons.Append("Image: ");
                            errorReasons.Append(squishedImage.src);
                            errorReasons.Append(" Error reason: ");
                            errorReasons.Append(squishedImage.error);
                            errorReasons.AppendLine();
                        }
                }
            }

            return errorReasons.ToString();
        }

        /// <summary>
        /// Process all the images for the given list of images
        /// retrieved from the sites HTML - this processes all images
        /// in a given list of xml nodes for the sitemap.
        /// </summary>
        /// <param name="xmlNodeList"></param>
        /// <param name="siteMapUrl"></param>
        /// <param name="squishedCssImages"></param>
        /// <returns></returns>
        public IEnumerable<string> ProcessImagesForXmlList(XmlNodeList xmlNodeList, string siteMapUrl, ref List<SquishedImage> squishedCssImages)
        {
            List<SquishedImage> images = new List<SquishedImage>();
            List<string> imageList = new List<string>();
            List<string> alreadyProcessed = new List<string>();

            foreach (XmlNode node in xmlNodeList)
            {
                string imageUrl = node.InnerText;

                if (!_commonUtils.IsValidFileExtension(imageUrl))
                {
                    break;
                }
                
                WebClient client = new WebClient();
                string downloadString = client.DownloadString(imageUrl);

                imageList.AddRange(_commonUtils.GetImagesInHtmlString(downloadString));

                // Process images in CSS file.
                squishedCssImages.AddRange(ProcessCss(downloadString, imageUrl, ref alreadyProcessed));
            }

            return imageList.Distinct();
        }
    }
}
