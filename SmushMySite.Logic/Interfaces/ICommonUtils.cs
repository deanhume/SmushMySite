namespace SmushMySite.Logic.Interfaces
{
    using System.Collections.Generic;

    public interface ICommonUtils
    {
        /// <summary>
        /// Removes any http details from the URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string RemoveHttp(string url);

        /// <summary>
        /// Gets the contents of a web page
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetWebPage(string url);

        /// <summary>
        /// Determines if the image is a vlid image based on the extension
        /// This will need to be updated
        /// </summary>
        /// <param name="extractSrcFromImgTag"></param>
        /// <returns></returns>
        bool IsValidImage(string extractSrcFromImgTag);

        /// <summary>
        /// Gets all the images in an HTML string.
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        List<string> GetImagesInHtmlString(string htmlString);

        /// <summary>
        /// Checks if the file extension is valid.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool IsValidFileExtension(string url);

        /// <summary>
        /// Downloads a web page contents for a given Url
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        string DownloadWebPage(string pageUrl);

        /// <summary>
        /// Checks if the extension is a valid CSS file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool IsValidCssExtension(string fileName);

        /// <summary>
        /// Checks to see if an image exists.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool DoesImageExist(string url);
    }
}
