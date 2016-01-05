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
        public void IsLoginOK_ValidUserAndPassword_LogsMessage()
        {
            var logger = new FakeLogger();
            var lm = new LoginManager1(logger);

            lm.AddUser("a","b");

            lm.IsLoginOK("a","b");

            Assert.AreEqual("login ok: user: a", logger.LastMessage());
        }
        
        [TestCase("a","b","c","d")]
        [TestCase("a","b","a","d")]
        //[TestCase("a","b","a","b")]
        public void IsLoginOK_InvalidUserOrPassword_LogsMessage(
            string addUserName, string addPassword, 
            string checkUserName, string checkPassword)
        {
            var logger = new FakeLogger();
            var lm = new LoginManager1(logger);

            lm.AddUser(addUserName, addPassword);

            lm.IsLoginOK(checkUserName, checkPassword);

            Assert.AreEqual(string.Format("bad login: {0},{1}", checkUserName, checkPassword), logger.LastMessage());
        }
    }

    internal class FakeLogger : ILogger
    {
        private string _message;

        public void Write(string text)
        {
            _message = text;
        }


        public string LastMessage()
        {
            return _message;
        }
    }
}
