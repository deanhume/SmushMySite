using SmushMySite.Logic.Entities;

namespace SmushMySite.Logic.Interfaces
{
    public interface IProxyHelper
    {
        bool RequiresAuthentication();

        /// <summary>
        /// Once we have established that we need to authenticate
        /// we try a username and password / domain to 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        ProxyDetails TryAuthenticate(string userName, string password, string domain = null);

        /// <summary>
        /// Check if we can connect by simply acessing a webpage.
        /// We are using a HEAD request because we dont want to download the page,
        /// we just need to get a reponse and this saves bandwidth.
        /// </summary>
        void CheckExists();

        /// <summary>
        /// Used to store the user's proxy credentials.
        /// If this is an web application, a safer form of storing the user's
        /// credentials might be considered.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        void StoreCredentials(string userName, string password, string domain = null);

        /// <summary>
        /// Retrieves the user's credentials.
        /// </summary>
        /// <returns></returns>
        UserCredentials GetCredentials();

        /// <summary>
        /// Removes the user's credentials from the cache
        /// </summary>
        void ClearCredentials();

        /// <summary>
        /// Checks the proxy details anmd determines whether or not to authenticate.
        /// </summary>
        bool HasProxy();
    }
}
