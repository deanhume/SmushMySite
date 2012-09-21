namespace SmushMySite.Logic.Interfaces
{
    using System.Collections.Generic;
    using Entities;

    public interface IUtils
    {
        /// <summary>
        /// Deserializes to our SquishedImage object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        SquishedImage ReadToObject(string json);

        /// <summary>
        /// Extracts image src from img tag
        /// </summary>
        /// <param name="imageString"></param>
        /// <returns></returns>
        string ExtractSrcFromImgTag(string imageString);

        /// <summary>
        /// Smushes the image from a URL
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        string SmushImageFromUrl(string imageUrl, string sourceUrl);

        /// <summary>
        /// Downloads and saves the image to an output directory
        /// </summary>
        /// <param name="images"></param>
        /// <param name="outputDirectory"></param>
        void DownloadAndSave(IEnumerable<SquishedImage> images, string outputDirectory);

        /// <summary>
        /// Calculate the statistics for our smushing results.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        int CalculateStatistics(IEnumerable<SquishedImage> images);

        /// <summary>
        /// Extracts Css Href from Html String.
        /// </summary>
        /// <param name="downloadString"></param>
        /// <returns></returns>
        List<string> ExtractCssHrefFromHtmlString(string downloadString);

        /// <summary>
        /// Extracts Images from Css file.
        /// </summary>
        /// <param name="cssFile"></param>
        /// <param name="sourceUrl"></param>
        /// <returns></returns>
        List<string> ExtractImagesFromCssFile(string cssFile, string sourceUrl);

        /// <summary>
        /// Validates and corrects the CSS image url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        string ValidateAndCorrectCssImageUrl(string url, string baseUrl);
    }
}
