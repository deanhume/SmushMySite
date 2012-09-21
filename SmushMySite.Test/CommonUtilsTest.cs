using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SmushMySite.Logic;
using SmushMySite.Logic.Interfaces;

namespace SmushMySite.Test
{
    public class CommonUtilsTest
    {
        private readonly ICommonUtils _commonUtils;
        public CommonUtilsTest()
        {
            _commonUtils = new CommonUtils();
        }

        [Test]
        public void IsValidImage_ShouldReturnFalse()
        {
            const string url = "http://dotnetshoutout.com/image.axd?url=test";

            Assert.That(_commonUtils.IsValidImage(url), Is.False);
        }

        [Test]
        public void IsValidImage_ShouldReturnTrue()
        {
            const string url = "http://dotnetshoutout.com/image.gif";

            Assert.That(_commonUtils.IsValidImage(url), Is.True);
        }

        [Test]
        public void IsValidImage_WithInvalidChars_ShouldReturnFalse()
        {
            const string url = @"http://dotnetshoutout.com/image.gif\";

            Assert.That(_commonUtils.IsValidImage(url), Is.False);
        }

        [Test]
        public void RemoveHttp_ShouldReturnCorrectString()
        {
            // Arrange
            const string url1 = "http://www.deanhume.com";
            const string url2 = "www.deanhume.com";
            const string url3 = "http://deanhume.com";
            const string expectedResult = "deanhume.com";

            // Act
            string returnedValue1 = _commonUtils.RemoveHttp(url1);
            string returnedValue2 = _commonUtils.RemoveHttp(url2);
            string returnedValue3 = _commonUtils.RemoveHttp(url3);

            // Assert
            Assert.That(returnedValue1, Is.EqualTo(expectedResult));
            Assert.That(returnedValue2, Is.EqualTo(expectedResult));
            Assert.That(returnedValue3, Is.EqualTo(expectedResult));
        }

        [Test]
        public void IsValidFileExtension_ShouldReturnFalse()
        {
            // Arrange
            const string url = "http://www.test.com/index.pdf";

            // Act
            bool computed = _commonUtils.IsValidImage(url);

            // Assert
            Assert.That(computed, Is.False);
        }

        [Test]
        public void IsValidFileExtension_ShouldReturnTrue()
        {
            // Arrange
            const string url = "http://www.test.com/index.html";

            // Act
            bool computed = _commonUtils.IsValidImage(url);

            // Assert
            Assert.That(computed, Is.False);
        }

        [Test]
        public void GetImagesInHtmlString_ShouldReturnAllImages()
        {
            // Arrange
            const string url = "http://2beknown.co.uk/What-We-Do.html";
            WebClient client = new WebClient();
            string htmlString = client.DownloadString(url);

            // Act
            List<string> imagesInHtmlString = _commonUtils.GetImagesInHtmlString(htmlString);

            // Assert
            Assert.That(imagesInHtmlString.Count, Is.GreaterThan(0));
        }
    }
}
