namespace SmushMySite.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Entities;
    using Interfaces;
    using System.Text.RegularExpressions;

    public class Utils : IUtils
    {
        private ICommonUtils _commonUtils;
        
        public Utils()
        {
            _commonUtils = new CommonUtils();
        }

        public Utils(ICommonUtils commonUtils)
        {
            _commonUtils = commonUtils;
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        SquishedImage IUtils.ReadToObject(string json)
        {
            SquishedImage deserialized = new SquishedImage();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserialized.GetType());
            deserialized = ser.ReadObject(ms) as SquishedImage;
            ms.Close();
            return deserialized;
        }

        /// <summary>
        /// Extracts the image "src" from the image tag.
        /// </summary>
        /// <param name="imageString"></param>
        /// <returns></returns>
        public string ExtractSrcFromImgTag(string imageString)
        {
            const string imageHtmlCode = "<img";
            const string imageSrcCode = @"src=""";

            int index = imageString.IndexOf(imageHtmlCode);

            if (index == -1)
            {
                return imageString;
            }

            //Remove previous data
            string newString = imageString.Substring(index);

            //Find the location of the two quotes that mark the image's location
            int brackedEnd = newString.IndexOf('>'); //make sure data will be inside img tag
            int start = newString.IndexOf(imageSrcCode) + imageSrcCode.Length;
            int end = newString.IndexOf('"', start + 1);

            //Extract the line
            if (end > start && start < brackedEnd)
            {
                return newString.Substring(start, end - start);

            }

            return newString;
        }

        /// <summary>
        /// Smushes an image from the given URL.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        public string SmushImageFromUrl(string imageUrl, string sourceUrl)
        {
            // Check to see if the image exists, cos then we can just download it
            if (_commonUtils.DoesImageExist(imageUrl))
            {
                StringBuilder correctImage = new StringBuilder("http://www.smushit.com/ysmush.it/ws.php?img=");
                correctImage.Append(imageUrl);
                correctImage.Append("&task=84354117326373970&id=paste0");

                return _commonUtils.GetWebPage(correctImage.ToString());
            }

            // Else we do some string manipulation to get the url.
            Uri uri = new Uri(sourceUrl);
            string baseUrl = uri.Host;

            // Check if the imageurl contains http://
            StringBuilder builder = new StringBuilder("http://www.smushit.com/ysmush.it/ws.php?img=");

            // First we need to check if the url already contains the domain name
            imageUrl = _commonUtils.RemoveHttp(imageUrl.Replace(uri.Host, ""));
            builder.Append("http://");
            builder.Append(baseUrl);
            builder.Append("/");
            builder.Append(imageUrl);
            builder.Append("&task=84354117326373970&id=paste0");

            return _commonUtils.GetWebPage(builder.ToString());
        }

        /// <summary>
        /// Downloads and Saves the image from the given URL
        /// </summary>
        /// <param name="images">A list of the images to retrieve</param>
        /// <param name="outputDirectory">The output directory</param>
        public void DownloadAndSave(IEnumerable<SquishedImage> images, string outputDirectory)
        {
            // Download and save the images
            if (images != null)
                foreach (SquishedImage squishedImage in images)
                {
                    if (squishedImage != null)
                    {
                        if (squishedImage.dest != null)
                        {
                            try
                            {
                                string extension = Path.GetExtension(squishedImage.dest);
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(squishedImage.src);

                                WebClient webClient = new WebClient();

                                webClient.DownloadFile(squishedImage.dest,
                                                       outputDirectory + @"\" + fileNameWithoutExtension + extension);

                            }
                            catch (Exception)
                            {
                                // We dont want an error to be thrown
                                // rather skip on to the next item
                                continue;
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// Calculates the total statistics - savings and original sizes
        /// </summary>
        /// <param name="images"></param>
        public int CalculateStatistics(IEnumerable<SquishedImage> images)
        {
            return images.Where(squishedImage => squishedImage != null).Sum(squishedImage => Convert.ToInt32((string) squishedImage.dest_size));
        }

        /// <summary>
        /// Extracts a list of css files and their Href's from a HTML string.
        /// </summary>
        /// <param name="downloadString"></param>
        /// <returns></returns>
        public List<string> ExtractCssHrefFromHtmlString(string downloadString)
        {
            List<string> extractedCss = new List<string>();
            MatchCollection matchCollection = Regex.Matches(downloadString, @"href=\""(.*?)\""",
                                                            RegexOptions.Singleline);
            foreach (Match match in matchCollection)
            {
                if (match.Success)
                {
                    string value = match.Value;

                    // Remove the first href and quotes
                    value = value.Replace("href=\"", "");

                    // Remove the last quote
                    extractedCss.Add(value.Replace("\"", ""));
                }
            }

            return extractedCss;
        }

        /// <summary>
        /// Extracst all images from a CSS file.
        /// </summary>
        /// <param name="cssFile"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        public List<string> ExtractImagesFromCssFile(string cssFile, string sourceUrl)
        {
            // Check to see if the cssFile exists, cos then we can just download it
            string cssToDownload;
            if (_commonUtils.DoesImageExist(cssFile))
            {
                cssToDownload = cssFile;
            }
            else
            {
                // Get the base url
                Uri uri = new Uri(sourceUrl);
                StringBuilder builder = new StringBuilder("http://");
                builder.Append(uri.Host);
                builder.Append("/");

                // First we need to check if the url already contains the domain name
                string url = _commonUtils.RemoveHttp(cssFile.Replace(uri.Host, ""));
                builder.Append(url);

                cssToDownload = builder.ToString();
            }

            string downloadString = _commonUtils.DownloadFromUrl(cssToDownload);
            return GetImagesUrlFromCss(downloadString, cssToDownload);
        }

        /// <summary>
        /// Get an image url from css string
        /// </summary>
        /// <param name="cssString"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        public List<string> GetImagesUrlFromCss(string cssString, string sourceUrl)
        {
            List<string> images = new List<string>();
            const string pattern = @"url\((?<char>['""])?(?<url>.*?)\k<char>?\)";

            MatchCollection matchCollection = Regex.Matches(cssString.ToLower(), pattern, RegexOptions.Singleline);

            foreach (Match match in matchCollection)
            {
                string validateUrl = ValidateAndCorrectCssImageUrl(match.Value, sourceUrl);

                if (_commonUtils.IsValidImage(validateUrl))
                {
                    images.Add(validateUrl);
                }
            }

            return images;
        }

        /// <summary>
        /// Validates and corrects the Css image url
        /// that is passed in. Removes all double slashes
        /// and double ellipses.
        /// </summary>
        /// <returns></returns>
        public string ValidateAndCorrectCssImageUrl(string url, string sourceUrl)
        {
            // Get the match and replace the single quotes
            string value = url.Replace("'", "");

            // Remove all the dodgy characters from the URL string
            string cleanImageUrl = value.Replace("url(", "").Replace(")", "").Replace(@"\","").Replace("\"",""); //.Replace("//","");

            // Check if the image exists before we process it any further
            // This needs to be made neater
            if (_commonUtils.DoesImageExist(cleanImageUrl))
            {
                return cleanImageUrl;
            }

            // Work out the file path
            Uri uri = new Uri(sourceUrl);
            string fileName = Path.GetFileName(sourceUrl);
            string destination = !string.IsNullOrEmpty(fileName) ? uri.AbsoluteUri.Replace(fileName, "") : uri.AbsoluteUri;

            return new Uri(new Uri(destination), cleanImageUrl).ToString();
        }
    }
}
