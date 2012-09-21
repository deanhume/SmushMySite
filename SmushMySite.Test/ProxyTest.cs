using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SmushMySite.Logic;
using SmushMySite.Logic.Entities;

namespace SmushMySite.Test
{
    [TestFixture]
    public class ProxyTest
    {
        private Mock<ProxyHelper> _mock;
        private UserCredentials _userCredentials;
        private ProxyDetails _trueProxyDetails;
        private ProxyDetails _falseProxyDetails;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<ProxyHelper>();
            _userCredentials = new UserCredentials {Domain = "", Password = "Password", UserName = "UserName"};
            _trueProxyDetails = new ProxyDetails {Result = "true", Successful = true, WebClient = null};
            _falseProxyDetails = new ProxyDetails { Result = "false", Successful = false, WebClient = null };
        }

        [Test]
        public void HasProxy_ShouldReturnTrue_WhenTested()
        {
            // Arrange
            _mock.Setup(x => x.RequiresAuthentication()).Returns(true);
            _mock.CallBase = true;

            // Act
            bool hasProxy = _mock.Object.HasProxy();

            //Assert
            Assert.That(hasProxy, Is.True);
        }

        [Test]
        public void HasProxy_ShouldReturnFalse_IfNoAuthenticationRequired()
        {
            // Arrange
            _mock.Setup(x => x.RequiresAuthentication()).Returns(true);
            
            // Set the users credentials
            _mock.Setup(x => x.GetCredentials()).Returns(_userCredentials);

            // Set the authentication as successful
            _mock.Setup(x => x.TryAuthenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(
                _trueProxyDetails);

            _mock.CallBase = true;

            // Act
            bool hasProxy = _mock.Object.HasProxy();

            //Assert
            Assert.That(hasProxy, Is.False);
        }

        [Test]
        public void HasProxy_ShouldReturnTrue_IfAuthenticateUnsuccessful()
        {
            // Arrange
            _mock.Setup(x => x.RequiresAuthentication()).Returns(true);

            // Set the users credentials
            _mock.Setup(x => x.GetCredentials()).Returns(_userCredentials);

            // Set the authentication as successful
            _mock.Setup(x => x.TryAuthenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(
                _falseProxyDetails);

            _mock.CallBase = true;

            // Act
            bool hasProxy = _mock.Object.HasProxy();

            //Assert
            Assert.That(hasProxy, Is.True);
        }

        [Test]
        public void HasProxy_ShouldReturnTrue_IfNoUserCredentials()
        {
            // Arrange
            _mock.Setup(x => x.RequiresAuthentication()).Returns(true);

            // Set the users credentials
            _mock.Setup(x => x.GetCredentials());

            _mock.CallBase = true;

            // Act
            bool hasProxy = _mock.Object.HasProxy();

            //Assert
            Assert.That(hasProxy, Is.True);
        }
    }
}
