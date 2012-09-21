using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SmushMySite.Logic;
using System.Collections.Generic;
using SmushMySite.Logic.Entities;
using SmushMySite.Logic.Interfaces;
using Moq;

namespace SmushMySite.Test
{
    [TestFixture]
    public class UtilsTest
    {
        private IUtils _utils;
        private Mock<ICommonUtils> _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<ICommonUtils>();
            _utils = new Utils(_mock.Object);
        }

        [Test]
        public void ReadToObject_ShouldReturnObject()
        {
            const string jsonObject = "{\"src\":\"http:\\/\\/www.deanhume.com\\/Zip\\/Image?Path=\\/Content\\/PostImages\\/SQLCompact\\/Create-Sql-Compact-DB.png\",\"src_size\":8475,\"dest\":\"http:\\/\\/smushit.zenfs.com\\/results\\/f06a7ee6\\/smush\\/%2FZip%2FImage_19ca3d7fbaeab1816ff5f2c02f42001e.png\",\"dest_size\":5838,\"percent\":\"31.12\",\"id\":\"paste0\"}";

            SquishedImage returnedObject = _utils.ReadToObject(jsonObject);

            Assert.That(returnedObject.dest_size, Is.EqualTo("5838"));
        }

        [Test]
        public void CalculateStatistics_ShouldReturnCorrectValues()
        {
            // Arrange
            List<SquishedImage> images = new List<SquishedImage>();
            images.Add(new SquishedImage { dest_size = "13", src_size = "24" });
            images.Add(new SquishedImage { dest_size = "13", src_size = "24" });

            // Act
            int calculateStatistics = _utils.CalculateStatistics(images);

            // Assert
            Assert.That(calculateStatistics, Is.EqualTo(26));

        }

        [Test]
        public void SmushImageFromUrl_WithHttpShouldUseCorrectUrlString()
        {
            // Arrange
            const string url = "http://www.deanhume.com/images/image.jpg";
            const string hostUrl = "http://www.deanhume.com";

            // Act
            string returnedValue = _utils.SmushImageFromUrl(url, hostUrl);

            // Assert
            //Assert.That(returnedValue, Is.Null);

        }

        [Test]
        public void SmushImageFromUrl_WithoutHttpShouldUseCorrectUrlString()
        {
            // Arrange
            const string url = "/images/image.jpg";
            const string hostUrl = "http://www.deanhume.com";

            // Act
            string returnedValue = _utils.SmushImageFromUrl(url, hostUrl);

            // Assert
            //Assert.That(returnedValue, Is.Null);
        }

        [Test]
        public void SmushImageFromUrl_WithFullUrl_ShouldUseCorrectUrlString()
        {
            // Arrange
            const string url = "http://i3.codeplex.com/Images/v17184/editicon.gif";
            const string hostUrl = "http://smushmysite.codeplex.com";

            // Act
            string returnedValue = _utils.SmushImageFromUrl(url, hostUrl);

            // Assert
            //Assert.That(returnedValue, Is.Null);
        }

        [Test]
        public void ExtractCssHrefFromHtmlString_ShouldReturnCorrectLinks()
        {
            // Arrange
            const string htmlString = "<link rel=\"stylesheet\" type=\"text/css\" href=\"css/layout.css\" /> ";

            // Act
            List<string> results = _utils.ExtractCssHrefFromHtmlString(htmlString);

            // Assert
            foreach (string result in results)
            {
                Assert.That(result, Is.EqualTo("css/layout.css"));
            }
        }

        [Test]
        public void ValidateAndCorrectCssImageUrl_ShouldReturnStringInCorrectFormat()
        {
            // Arrange
            const string url = "url(xenophone.eot)";
            const string hostUrl = "http://www.deanhume.com/";
            string expectedValue = "http://www.deanhume.com/xenophone.eot";

            _mock.Setup(x => x.DoesImageExist(It.IsAny<string>())).Returns(false);

            // Act
            string returnedValue = _utils.ValidateAndCorrectCssImageUrl(url, hostUrl);

            // Assert
            Assert.That(returnedValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ValidateAndCorrectCssImageUrl_WithEllipses_ShouldReturnStringInCorrectFormat()
        {
            // Arrange
            const string url = "url(../images/top_nav_bgrd.jpg)";
            const string hostUrl = "http://www.deanhume.com/";
            string expectedValue = "http://www.deanhume.com/images/top_nav_bgrd.jpg";

            _mock.Setup(x => x.DoesImageExist(It.IsAny<string>())).Returns(false);

            // Act
            string returnedValue = _utils.ValidateAndCorrectCssImageUrl(url, hostUrl);

            // Assert
            Assert.That(returnedValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ValidateAndCorrectCssImageUrl_WithoutEllipses_ShouldReturnStringInCorrectFormat()
        {
            // Arrange
            const string url = "url(/images/top_nav_bgrd.jpg)";
            const string hostUrl = "http://www.deanhume.com/";
            string expectedValue = "http://www.deanhume.com/images/top_nav_bgrd.jpg";

            _mock.Setup(x => x.DoesImageExist(It.IsAny<string>())).Returns(false);

            // Act
            string returnedValue = _utils.ValidateAndCorrectCssImageUrl(url, hostUrl);

            // Assert
            Assert.That(returnedValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ValidateAndCorrectCssImageUrl_WithoutFirstSlash_ShouldReturnStringInCorrectFormat()
        {
            // Arrange
            const string url = "url(graphics/bgMain.jpg)";
            const string hostUrl = "http://www.deanhume.com/";
            string expectedValue = "http://www.deanhume.com/graphics/bgMain.jpg";

            _mock.Setup(x => x.DoesImageExist(It.IsAny<string>())).Returns(false);

            // Act
            string returnedValue = _utils.ValidateAndCorrectCssImageUrl(url, hostUrl);

            // Assert
            Assert.That(returnedValue, Is.EqualTo(expectedValue));
        }
    }
}
