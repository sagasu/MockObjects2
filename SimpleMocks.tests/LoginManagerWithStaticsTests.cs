using MyBillingProduct;
using NUnit.Framework;

namespace SimpleMocks.tests
{
    [TestFixture]
    class LoginManagerWithStaticsTests
    {

        internal class FakeLoginManager : LoginManagerWithStatics
        {
            private readonly string _machineName;

            public FakeLoginManager(string machineName) : base(new FakeDateTimeService())
            {
                _machineName = machineName;
            }

            public string Message { get; set; }
            protected override void CallStaticWebService(LoggerException e, string dateTime)
            {
                Message = dateTime + e.Message + _machineName;
            }

            protected override void CallStaticWs()
            {
                throw new LoggerException("My Exception ");
            }
        }

        [Test]
        public void IsLoginOK_CallStaticLogger_LogsCorrectMessage()
        {
            //var logger = new FakeLogger(new FakeWebService());

            var machineName = "machineName";
            var lm = new FakeLoginManager(machineName);

            lm.AddUser("a", "b");

            lm.IsLoginOK("a", "b");

            Assert.AreEqual(new FakeDateTimeService().GetTime() + "My Exception " + machineName, lm.Message);
        }
    }

    public class FakeDateTimeService : IDateTimeService
    {
        public string GetTime()
        {
            return "12.12.12 12:33:55";
        }
    }
}
