using System;
using System.Collections;

namespace MyBillingProduct
{
	public class LoginManager1
	{
	    private readonly ILogger _logger;
	    private Hashtable m_users = new Hashtable();

	    public LoginManager1(ILogger logger)
	    {
	        _logger = logger;
	    }

	    public bool IsLoginOK(string user, string password)
	    {
            if (m_users[user] != null &&
	            (string) m_users[user] == password)
            {
                _logger.Write(string.Format("login ok: user: {0}", user));
	            return true;
	        }
            _logger.Write(string.Format("bad login: {0},{1}", user, password));
	        return false;
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
}
