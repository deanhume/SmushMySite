using System.Windows;
using MahApps.Metro.Controls;
using SmushMySite.Logic;

namespace SmushMySite
{
    /// <summary>
    /// Interaction logic for ProxyAuthentication.xaml
    /// </summary>
    public partial class ProxyAuthentication : MetroWindow
    {
        private ProxyHelper _proxyHelper;
        public ProxyAuthentication()
        {
            InitializeComponent();
            _proxyHelper = new ProxyHelper();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Add the credentials to the cache
            _proxyHelper.StoreCredentials(txtUserName.Text, txtPassword.Password, txtDomain.Text);

            // Close the window
            this.Close();
        }
    }
}
