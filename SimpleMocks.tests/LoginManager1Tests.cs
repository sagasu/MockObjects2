using Moq;
using MyBillingProduct;
using NUnit.Framework;

namespace SimpleMocks.tests
{
    internal class FakeLoginManager : LoginManager1
    {
        public string Message { get; set; }

        public FakeLoginManager(ILogger logger) : base(logger)
        {
        }

        protected override void LogStaticMessage(string user)
        {
            Message = string.Format("user {0} logged in ok", user);
        }
    }

    [TestFixture]
    class LoginManager1Tests
    {

        [Test]
        public void IsLoginOK_CallStaticLogger_LogsCorrectMessage()
        {
            var logger = FakeLogerFactory();
            var lm = new FakeLoginManager(logger);

            lm.AddUser("a", "b");

            lm.IsLoginOK("a", "b");

            Assert.AreEqual("user a logged in ok", lm.Message);
        }


        [Test]
        public void IsLoginOK_WhenLoggerThrowsException_LogsCorrectMessage()
        {
            var mockWs = new Mock<IWebService>();
            var logger = FakeLogerFactory(mockWs.Object, willThrowException: true);
            var lm = new FakeLoginManager(logger);

            lm.AddUser("a","b");

            lm.IsLoginOK("a","b");

            mockWs.Verify(_ => _.Write(It.IsAny<string>()), Times.Once);
        }

        private static FakeLogger FakeLogerFactory(bool willThrowException = false)
        {
            return FakeLogerFactory(new FakeWebService(), willThrowException);
        }

        private static FakeLogger FakeLogerFactory(IWebService ws, bool willThrowException = false)
        {
            return new FakeLogger(ws){ WillThrowException = willThrowException };
        }

        [Test]
        public void IsLoginOK_UsingMoq_ValidUserAndPassword_LogsMessage()
        {
            var loggerMock = new Mock<ILogger>();

            var lm = new FakeLoginManager(loggerMock.Object);

            lm.AddUser("a","b");

            lm.IsLoginOK("a","b");

            loggerMock.Verify(_ => _.Write("login ok: user: a"), Times.Once);
        }
        
        [Test]
        public void IsLoginOK_ValidUserAndPassword_LogsMessage()
        {
            var logger = FakeLogerFactory();
            var lm = new FakeLoginManager(logger);

            lm.AddUser("a","b");

            lm.IsLoginOK("a","b");

            Assert.AreEqual("login ok: user: a", logger.LastMessage());
        }

        // In IsLoginOK implementation there is 'm_users[user] == password' object compartment. 
        // We are comparing by value not by reference, that's why we cast to string.
        // If there is no cast to string and compartment is by reference this test will fail.
        [Test]
        public void IsLoginOK_ValidUserAndPassword_FailsToCompareHashtable()
        {
            var logger = FakeLogerFactory();
            var lm = new FakeLoginManager(logger);

            var addUserName = new string(new []{'a'});
            var addPassword = new string(new[] { 'b' });
            var checkUserName = new string(new[] { 'a' });
            var checkPassword = new string(new[] { 'b' });

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            Assert.AreEqual("login ok: user: a", logger.LastMessage());
        }
        
        [TestCase("a","b","c","d")]
        [TestCase("a","b","a","d")]
        public void IsLoginOK_InvalidUserOrPassword_LogsMessage(
            string addUserName, string addPassword, 
            string checkUserName, string checkPassword)
        {
            var logger = FakeLogerFactory();
            var lm = new FakeLoginManager(logger);

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            Assert.AreEqual(string.Format("bad login: {0},{1}", checkUserName, checkPassword), logger.LastMessage());

        }
        
        [TestCase("a","b","c","d")]
        [TestCase("a","b","a","d")]
        public void IsLoginOK_UsingMock_InvalidUserOrPassword_LogsMessage(
            string addUserName, string addPassword, 
            string checkUserName, string checkPassword)
        {
            var logger = new Mock<ILogger>();
            var lm = new FakeLoginManager(logger.Object);

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            logger.Verify(_ => _.Write(string.Format("bad login: {0},{1}", checkUserName, checkPassword)), Times.Once);
        }

        [TestCase("a","b","a","b")]
        public void IsLoginOK_ValidUserAndPassword_FailsToCompareHashtable(
            string addUserName, string addPassword, 
            string checkUserName, string checkPassword)
        {
            var logger = FakeLogerFactory();
            var lm = new FakeLoginManager(logger);

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            Assert.AreEqual(string.Format("login ok: user: {0}", checkUserName), logger.LastMessage());
        }
    }

    internal class FakeWebService : IWebService
    {
        private string _message;

        public void Write(string message)
        {
            _message = message;
        }

    }

    public class FakeLogger : ILogger
    {
        private readonly IWebService _fakeWebService;
        private string _message;

        public FakeLogger(IWebService fakeWebService)
        {
            _fakeWebService = fakeWebService;
        }

        public void Write(string text)
        {
            if (WillThrowException)
            {
                _fakeWebService.Write("got exception - " + text);
            }
            _message = text;
        }

        public bool WillThrowException { get; set; }


        public string LastMessage()
        {
            return _message;
        }
    }
}
