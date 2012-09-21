using System.Net;

namespace SmushMySite.Logic.Entities
{
    public class ProxyDetails
    {
        /// <summary>
        /// The webclient that we are going to use for any further web requests
        /// </summary>
        public WebClient WebClient { get; set; }

        /// <summary>
        /// Was the authentication successful
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// The resulting error / success message
        /// </summary>
        public string Result { get; set; }
    }
}
