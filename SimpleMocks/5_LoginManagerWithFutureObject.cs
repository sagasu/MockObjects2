using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace MyBillingProduct
{
	public class LoginManagerWithFutureObject
	{
	    private Hashtable m_users = new Hashtable();
	    private const int MINPASSLENGTH = 8;

	    public event Action<string> LoggedInOK;

	    public LoginManagerWithFutureObject()
	    {
	        LoggedInOK = s => { };
	    }

	    public bool IsLoginOK(string user, string password)
	    {
	        try
	        {
	            WriteMessageToLogger("login ok");
	        }
	        catch (LoggerException e)
	        {
                WriteMessageToWebservice(String.Format("{0} {1}", e.Message ,GetMachineName()));
	        }
	        if (m_users[user] != null &&
	            m_users[user] == password)
	        {
	            FireLoggedInEvent();
	            return true;
	        }
	        return false;
	    }

	    protected virtual void FireLoggedInEvent()
	    {
	        LoggedInOK.Invoke("");
	    }

	    protected virtual string GetMachineName()
	    {
	        return Environment.MachineName;
	    }



	    protected virtual void WriteMessageToLogger(string msg)
	    {
	        new RealLogger().Write(msg);
	    }

	    public void AddUser(string user, string password)
	    {
	        ValidatePasswordLength(password);
	        m_users[user] = password;
	    }

	    private void ValidatePasswordLength(string password)
	    {
            if (password==null)
                throw new PasswordIsNullException(string.Format("Password cannot be null"));

            if (password.Length <= MINPASSLENGTH)
	            throw new PasswordIsTooShortException(string.Format("Password must be grater than {0}",MINPASSLENGTH));

            var uppercaseRegex = new Regex(@"[A-Z]", RegexOptions.Compiled);
            if (!uppercaseRegex.IsMatch(password))
                throw new PasswordDoesNotHaveUppercaseException("Password must have at least one uppercase");
	    }

	    public void ChangePass(string user, string oldPass, string newPassword)
		{
			m_users[user]= newPassword;
		}

	    protected virtual void WriteMessageToWebservice(string msg)
	    {
            new WebService().Write(msg);
	    }
	}

    public class PasswordIsTooShortException : Exception
    {
        public PasswordIsTooShortException(string format){}
    }

    public class PasswordIsNullException : Exception
    {
        public PasswordIsNullException(string format){}
    }
    
    public class PasswordDoesNotHaveUppercaseException : Exception
    {
        public PasswordDoesNotHaveUppercaseException(string format) { }
    }
}
