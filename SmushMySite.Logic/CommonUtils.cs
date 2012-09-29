namespace SmushMySite.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using HtmlAgilityPack;
    using Interfaces;
    using SmushMySite.Logic.Entities;

    public class CommonUtils : ICommonUtils
    {
        /// <summary>
        /// Removes the "http" from a string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string RemoveHttp(string url)
        {
            if (url.Contains("http://www."))
            {
                return url.Replace("http://www.", "");
            }
            if (url.Contains("http://"))
            {
                return url.Replace("http://", "");
            }
            if (url.Contains("www."))
            {
                return url.Replace("www.", "");
            }
            return url;
        }

        /// <summary>
        /// A simple helper method to 
        /// determine if the URL contains a 
        /// leading "http://"
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string AppendHttp(string url)
        {
            if (url.Contains("http://"))
            {
                return url;
            }
            else
            {
                return "http://" + url;
            }
        }

        /// <summary>
        /// Perform an HTTP "GET" operation, and return the result.
        /// </summary>
        /// <param name="url">Fully qualified URL to the remote page</param>
        /// <returns>A string with the result from the GET operation.</returns>
        public string GetWebPage(string url)
        {
            // Create a request for the URL.         
            WebRequest request = WebRequest.Create(new Uri(url));

            request.Proxy = null;

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;

            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream containing content returned by the server.
            if (response != null)
            {
                Stream dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                if (dataStream != null)
                {
                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    // Cleanup the streams and the response.
                    reader.Close();
                    dataStream.Close();
                    response.Close();

                    return (responseFromServer);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks if the image is a valid image based on the extension.
        /// </summary>
        /// <param name="extractSrcFromImgTag"></param>
        /// <returns></returns>
        public bool IsValidImage(string extractSrcFromImgTag)
        {
            // First check if the file contains any illegal characters
            if (!string.IsNullOrEmpty(extractSrcFromImgTag) &&
                            extractSrcFromImgTag.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }

            string extension = Path.GetExtension(extractSrcFromImgTag);

            if (extension != null)
                switch (extension.ToLower())
                {
                    case ".gif":
                        return true;
                    case ".jpg":
                        return true;
                    case ".png":
                        return true;
                    case ".jpeg":
                        return true;
                    case ".bmp":
                        return true;
                    default:
                        return false;
                }

            return false;
        }

        /// <summary>
        /// Gets all images in an html page by their img tags
        /// </summary>
        /// <param name="htmlString">The string to process.</param>
        /// <returns>A list of all images.</returns>
        public List<string> GetImagesInHtmlString(string htmlString)
        {
            List<string> images = new List<string>();
            
            // Load the document
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlString);

            // Check for standard image tags
            HtmlNodeCollection htmlNodeCollection = document.DocumentNode.SelectNodes("//img");
            if (htmlNodeCollection != null)
                foreach (HtmlNode node in htmlNodeCollection)
                {
                    string src = node.GetAttributeValue("src", "");
                    images.Add(src);
                }

            // Check for asp.net image tags
            HtmlNodeCollection htmlNodeCollectionNet = document.DocumentNode.SelectNodes("//input");
            if (htmlNodeCollectionNet != null)
                foreach (HtmlNode node in htmlNodeCollectionNet)
                {
                    if (node.GetAttributeValue("type", "") == "image")
                    {
                        string src = node.GetAttributeValue("src", "");
                        images.Add(src);
                    }
                }

            return images;
        }

        /// <summary>
        /// Checks to see if a file extension is valid
        /// for sending through to Yahoo for smushing.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsValidFileExtension(string url)
        {
            // First check if the file contains any illegal characters
            if (!string.IsNullOrEmpty(url) && url.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }

            string extension = Path.GetExtension(url);
            if (extension != ".pdf")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Downloads a web page and returns a string
        /// Might want to consider making this more async, 
        /// but we are already in a new thread.
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public string DownloadWebPage(string pageUrl)
        {
            // Initialize the string
            string downloadString = string.Empty;

            using (WebClient client = new WebClient())
            {
                downloadString = client.DownloadString(pageUrl);
            }

            return downloadString;
        }

        /// <summary>
        /// Checks if the extension is a valid CSS file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsValidCssExtension(string fileName)
        {
            // First check if the file contains any illegal characters
            if (!string.IsNullOrEmpty(fileName) &&
                            fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }

            string extension = Path.GetExtension(fileName);

            if (extension != null)
                switch (extension.ToLower())
                {
                    case ".less":
                        return true;
                    case ".css":
                        return true;
                    default:
                        return false;
                }

            return false;
        }

        /// <summary>
        /// Checks to see if an image exists.
        /// We aren't downloading the entire image, just 
        /// checking if it's there first.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool DoesImageExist(string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "HEAD";
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    request.GetResponse();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
