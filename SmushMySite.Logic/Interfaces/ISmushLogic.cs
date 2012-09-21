namespace SmushMySite.Logic.Interfaces
{
    using System.Collections.Generic;
    using System.Xml;
    using Entities;

    public interface ISmushLogic
    {
        /// <summary>
        /// Process all the images for the given list of images
        /// retrieved from the sites HTML
        /// </summary>
        /// <param name="imagesInHtmlString"></param>
        /// <param name="pageUrl"></param>
        List<SquishedImage> ProcessImages(IEnumerable<string> imagesInHtmlString, string pageUrl);

        /// <summary>
        /// Builds a string of the error reasons to display in an info 
        /// textbox to the user.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        string GetErrorReasons(IEnumerable<SquishedImage> images);

        /// <summary>
        /// Process all the images for the given list of images
        /// retrieved from the sites HTML - this processes all images
        /// in a given list of xml nodes for the sitemap.
        /// </summary>
        /// <param name="xmlNodeList"></param>
        /// <param name="siteMapUrl"></param>
        /// <param name="squishedCssImages"></param>
        /// <returns></returns>
        IEnumerable<string> ProcessImagesForXmlList(XmlNodeList xmlNodeList, string siteMapUrl, ref List<SquishedImage> squishedCssImages);

        /// <summary>
        /// Processes a single image using
        /// the Yahoo Smush.it service
        /// </summary>
        /// <param name="imageUrl">The image to process</param>
        /// <param name="pageUrl">The page Url.</param>
        /// <param name="removeTag"></param>
        /// <returns></returns>
        SquishedImage ProcessSingleImage(string imageUrl, string pageUrl, bool removeTag);

        /// <summary>
        /// Processes the images in a Css file.
        /// </summary>
        /// <param name="downloadString"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="alreadyProcessed"></param>
        List<SquishedImage> ProcessCss(string downloadString, string sourceUrl, ref List<string> alreadyProcessed);
    }
}