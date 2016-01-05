using System;
using System.Collections;

namespace MyBillingProduct
{
	public class LoginManager2
	{
	    private readonly IWebService service;
	    private readonly ILogger log;
	    private Hashtable m_users = new Hashtable();


	    public LoginManager2(ILogger logger,IWebService service)
	    {
	        this.service = service;
	        log = logger;
	    }

	    public bool IsLoginOK(string user, string password)
	    {
	        if (m_users[user] != null &&
	            m_users[user] == password)
	        {
	            return true;
	        }
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
