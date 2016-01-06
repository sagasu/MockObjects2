using System;
using MyBillingProduct;
using NUnit.Framework;

namespace SimpleMocks.tests
{
    [TestFixture]
    class LoginManagerWithFutureObjectTests
    {
        private const string ANY_PASSWORD = "Password1";


        [Test]
        public void IsLogin_WhenLoginOk_CallsLogger()
        {
            var lm = new TestableRealLogger_LoginManagerWithFutureObject();
            lm.AddUser("a",ANY_PASSWORD);
            
            lm.IsLoginOK("a",ANY_PASSWORD);

            var message = lm.Message;
            Assert.AreEqual("login ok",message);
        }

        [Test]
        public void IsLogin_WhenLoggerThrowsException_CallWebservice()
        {
            var lm = new TestableWebservice_LoginManagerWithFutureObject();

            lm.IsLoginOK("a",ANY_PASSWORD);

            var message = lm.Message;
            Assert.AreEqual("exception in logger MACHINENAME", message);
        }

        [Test]
        public void IsLogin_WhenLoginOk_FireEvent()
        {
            var lm = new TestableRaiseEvent_LoginManagerWithFutureObject();
            lm.AddUser("a", ANY_PASSWORD);

            lm.IsLoginOK("a", ANY_PASSWORD);

            var isEventFired = lm.IsEventFired;
            Assert.AreEqual(true, isEventFired);
        }


        [TestCase("a","12345678")]
        [TestCase("a","abcdefgh")]
        [TestCase("a","abcdefg")]
        [TestCase("a","abc")]
        [TestCase("a","")]
        public void AddUser_WhenPasswordNotMinChars_Throws(string username,string password)
        {
            var lm = new Fake_LoginManagerWithFutureObject();

            Assert.Throws<PasswordIsTooShortException>(() => lm.AddUser(username, password));
        }

        [TestCase("a", "abcdefgha")]
        [TestCase("a", "abcdefgha")]
        public void AddUser_WhenPasswordHasNoUpperCase_Throws(string username,string password)
        {
            var lm = new Fake_LoginManagerWithFutureObject();

            Assert.Throws<PasswordDoesNotHaveUppercaseException>(() => lm.AddUser(username, password));
        }

        [Test]
        public void AddUser_WhenPasswordNull_Throws()
        {
            var lm = new Fake_LoginManagerWithFutureObject();

            Assert.Throws<PasswordIsNullException>(() => lm.AddUser("a", null));
        }
        




    }


    public class Fake_LoginManagerWithFutureObject : LoginManagerWithFutureObject
    {
        protected override void WriteMessageToLogger(string msg)
        {
            return;
        }
    }
    
    public class TestableRaiseEvent_LoginManagerWithFutureObject : LoginManagerWithFutureObject
    {
        public bool IsEventFired { get; set; }

        protected override void WriteMessageToLogger(string msg)
        {
            return;
        }

        protected override void FireLoggedInEvent()
        {
            IsEventFired = true;
        }
    }

    public class TestableRealLogger_LoginManagerWithFutureObject:LoginManagerWithFutureObject
    {
        public string Message { get; set; }

        protected override void WriteMessageToLogger(string msg)
        {
            Message = msg;
        }
    }

    public class TestableWebservice_LoginManagerWithFutureObject:LoginManagerWithFutureObject
    {
        public string Message { get; set; }

        protected override void WriteMessageToLogger(string msg)
        {
            throw new LoggerException("exception in logger");
        }

        protected override void WriteMessageToWebservice(string msg)
        {
            Message = msg;
        }

        protected override string GetMachineName()
        {
            return "MACHINENAME";
        }

    }

}
