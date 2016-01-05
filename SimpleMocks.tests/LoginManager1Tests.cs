using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyBillingProduct;
using NUnit.Framework;

namespace SimpleMocks.tests
{
    [TestFixture]
    class LoginManager1Tests
    {
        [Test]
        public void IsLoginOK_WhenLoggerThrowsException_LogsCorrectMessage()
        {
            var fakeWebService = new FakeWebService();
            var logger = FakeLogerFactory();
            var lm = new LoginManager1(logger);

            lm.AddUser("a","b");

            lm.IsLoginOK("a","b");

            Assert.AreEqual("login ok: user: a", logger.LastMessage());
        }

        private static FakeLogger FakeLogerFactory()
        {
            return new FakeLogger(new FakeWebService());
        }

        [Test]
        public void IsLoginOK_ValidUserAndPassword_LogsMessage()
        {
            var logger = FakeLogerFactory();
            var lm = new LoginManager1(logger);

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
            var lm = new LoginManager1(logger);

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
            var lm = new LoginManager1(logger);

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            Assert.AreEqual(string.Format("bad login: {0},{1}", checkUserName, checkPassword), logger.LastMessage());
        }

        [TestCase("a","b","a","b")]
        public void IsLoginOK_ValidUserAndPassword_FailsToCompareHashtable(
            string addUserName, string addPassword, 
            string checkUserName, string checkPassword)
        {
            var logger = FakeLogerFactory();
            var lm = new LoginManager1(logger);

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            Assert.AreEqual(string.Format("login ok: user: {0}", checkUserName), logger.LastMessage());
        }
    }

    internal class FakeWebService : IWebService
    {
        public void Write(string message)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeLogger : ILogger
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
