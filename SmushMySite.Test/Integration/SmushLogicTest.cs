using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SmushMySite.Logic;
using SmushMySite.Logic.Interfaces;

namespace SmushMySite.Test.Integration
{
    [TestFixture]
    public class SmushLogicTest
    {
        private readonly ISmushLogic _smushLogic;
        public SmushLogicTest()
        {
            _smushLogic = new SmushLogic();
        }

        [Test]
        public void ExtractImagesFromCssFile_ShouldReturnImageList()
        {
            string html = "<link rel=\"stylesheet\" type=\"text/css\" href=\"css/layout.css\" />";
            
            List<string> alreadyProcessed = new List<string>();
            List<string> imagesInHtmlFile = new List<string>();

            //string cssFile = "background: url(../image/container_bgrd_high_contrast.jpg) top center repeat-x;";
            string sourceUrl = "http://www.stevenfenwick.com/about.php";


            _smushLogic.ProcessCss(html, sourceUrl, ref alreadyProcessed);
        }
    }
}
