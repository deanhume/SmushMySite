using System;
using System.Net;
using SmushMySite.Logic.Entities;
using SmushMySite.Logic.Interfaces;

namespace SmushMySite.Logic
{
    public class ProxyHelper : IProxyHelper
    {
        public virtual bool RequiresAuthentication()
        {
            // See if the default credentials work.
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            bool authRequired = false;

            try
            {
                CheckExists();
            }
            catch (WebException webException)
            {
                if (((HttpWebResponse)webException.Response).StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
                    authRequired = true;
            }

            return authRequired;
        }

        /// <summary>
        /// Once we have established that we need to authenticate
        /// we try a username and password / domain to 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public virtual ProxyDetails TryAuthenticate(string userName, string password, string domain = null)
        {
            ProxyDetails proxyDetails = new ProxyDetails();
            WebClient webClient = new WebClient();

            // Set the credentials
            WebRequest.DefaultWebProxy.Credentials = new NetworkCredential(userName, password, domain);

            // Try and connect again
            try
            {
                CheckExists();
                proxyDetails.Result = "Sucessful";
                proxyDetails.WebClient = webClient;
                proxyDetails.Successful = true;
            }
            catch (Exception exception)
            {
                proxyDetails.Result = exception.Message;
                proxyDetails.WebClient = null;
                proxyDetails.Successful = false;
            }

            return proxyDetails;
        }

        /// <summary>
        /// Check if we can connect by simply acessing a webpage.
        /// We are using a HEAD request because we dont want to download the page,
        /// we just need to get a reponse and this saves bandwidth.
        /// </summary>
        public void CheckExists()
        {
            // We need to check this by checking a site - this could be changed to anything.
            var request = (HttpWebRequest) WebRequest.Create("http://www.google.com/");
            request.Method = "HEAD";
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                request.GetResponse();
            }
        }

        /// <summary>
        /// Used to store the user's proxy credentials.
        /// If this is an web application, a safer form of storing the user's
        /// credentials might be considered.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        public void StoreCredentials(string userName, string password, string domain = null)
        {
            UserCredentials credentials = new UserCredentials
                                              {
                                                  Domain = domain,
                                                  Password = password,
                                                  UserName = userName
                                              };


            CacheLayer.Add(credentials, "Credentials");
        }

        /// <summary>
        /// Retrieves the user's credentials.
        /// </summary>
        /// <returns></returns>
        public virtual UserCredentials GetCredentials()
        {
            return CacheLayer.Get<UserCredentials>("Credentials");
        }

        /// <summary>
        /// Removes the user's credentials from the cache
        /// </summary>
        public void ClearCredentials()
        {
            CacheLayer.Clear("Credentials");
        }

        /// <summary>
        /// Checks the proxy details anmd determines whether or not to authenticate.
        /// </summary>
        public bool HasProxy()
        {
            if (RequiresAuthentication())
            {
                // First check if we have the user's details stored
                UserCredentials userCredentials = GetCredentials();

                if (userCredentials != null)
                {
                    // Try and authenticate
                    ProxyDetails authenticate = TryAuthenticate(userCredentials.UserName, userCredentials.Password,
                                                                                userCredentials.Domain);

                    if (!authenticate.Successful)
                    {
                        // Clear the cache and request that the user enters a new password
                        ClearCredentials();

                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
