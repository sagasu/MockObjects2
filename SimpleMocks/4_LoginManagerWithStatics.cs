using System;
using System.Collections;
using Step1Mocks;

namespace MyBillingProduct
{
	public class LoginManagerWithStatics
	{
	    private readonly IDateTimeService _dateTimeService;
	    private Hashtable m_users = new Hashtable();

	    public LoginManagerWithStatics(IDateTimeService dateTimeService)
	    {
	        _dateTimeService = dateTimeService;
	    }

	    public bool IsLoginOK(string user, string password)
	    {
	        try
	        {
	           CallStaticWs();
	        }
	        catch (LoggerException e)
	        {
	            var message = string.Format("{0}{1}{2}", _dateTimeService.GetTime(), e.Message, GetMachineName());
                CallStaticWebService(message);
	        }
	        if (m_users[user] != null &&
	            (string) m_users[user] == password)
	        {
	            return true;
	        }
	        return false;
	    }

	    protected virtual string GetMachineName()
	    {
	        return Environment.MachineName;
	    }

	    protected virtual void CallStaticWs()
	    {
	        StaticLogger.Write("blah");
	    }

	    protected virtual void CallStaticWebService(string message)
	    {
            StaticWebService.Write(message);
	    }


	    public void AddUser(string user, string password)
	    {
	        m_users[user] = password;
	    }

	    public void ChangePass(string user, string oldPass, string newPassword)
		{
			m_users[user]= newPassword;
		}
	}

    public interface IDateTimeService
    {
        string GetTime();
    }
}
