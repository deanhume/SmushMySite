using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmushMySite.Logic.Entities
{
    public class DownloadedWebPage
    {
        public string WebPageString { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
