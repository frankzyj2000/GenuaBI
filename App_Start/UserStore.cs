using Microsoft.AspNet.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using GenuinaBI.Models;
namespace GenuinaBI
{
	/// <summary>
	/// Works with CFG_Users objects stored in an XML file
	/// </summary>
	public class XmlUserStore : IUserStore<CFG_Users>, IUserPasswordStore<CFG_Users>, IUserLockoutStore<CFG_Users, object>
	{
		protected XmlDocument m_doc;

		public XmlUserStore(string credentialsXmlFile)
		{
			m_doc = new XmlDocument();
			m_doc.Load(credentialsXmlFile);
		}

		#region IUserStore implementation

		public Task<CFG_Users> FindByIdAsync(string userId)
		{
			Logger.Log("XmlUserStore:FindByIdAsync (userId = {0})", userId);			
			
			if (string.IsNullOrEmpty(userId)) return Task.FromResult<CFG_Users>(null);
			
			XmlNode n = m_doc.SelectSingleNode(string.Format("/users/user[@id='{0}']", userId));
			CFG_Users u = null;

			if (n != null)			
			{
				u = new CFG_Users { Id = userId, UserName = userId, PasswordHash = n.Attributes["password"].Value };
			}
			
			return Task.FromResult<CFG_Users>(u);
		}

		public Task<CFG_Users> FindByNameAsync(string userName)
		{
			Logger.Log("XmlUserStore:FindByNameAsync (userName = {0})", userName);
			string s = "pop";
			string s1 = new PasswordHasher().HashPassword(s);
			return FindByIdAsync(userName);
			
		}

		public Task CreateAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserPasswordStore implementation

		public Task<string> GetPasswordHashAsync(CFG_Users user)
		{
			Logger.Log("XmlUserStore:GetPasswordHashAsync (user = {0})", user);
			string hash = user.PasswordHash;
			return Task.FromResult<string>(hash);
		}

		public Task<bool> HasPasswordAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task SetPasswordHashAsync(CFG_Users user, string passwordHash)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserLockoutStore implementation

		public Task<int> GetAccessFailedCountAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task<bool> GetLockoutEnabledAsync(CFG_Users user)
		{
			Logger.Log("XmlUserStore:GetLockoutEnabledAsync (user = {0})", user);
			return Task.FromResult<bool>(false);
		}

		public Task<DateTimeOffset> GetLockoutEndDateAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task<int> IncrementAccessFailedCountAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task ResetAccessFailedCountAsync(CFG_Users user)
		{
			throw new NotImplementedException();
		}

		public Task SetLockoutEnabledAsync(CFG_Users user, bool enabled)
		{
			throw new NotImplementedException();
		}

		public Task SetLockoutEndDateAsync(CFG_Users user, DateTimeOffset lockoutEnd)
		{
			throw new NotImplementedException();
		}

		public Task<CFG_Users> FindByIdAsync(object userId)
		{
			throw new NotImplementedException();
		}

		#endregion

		public void Dispose()
		{
		}
	}
}