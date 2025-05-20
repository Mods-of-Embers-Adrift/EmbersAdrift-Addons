using System;

namespace RESTfulHTTPServer.src.models
{
	// Token: 0x02000079 RID: 121
	public class BasicAuth
	{
		// Token: 0x06000512 RID: 1298 RVA: 0x000468C2 File Offset: 0x00044AC2
		public BasicAuth(string username, string password)
		{
			this._username = username;
			this._password = password;
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x000468D8 File Offset: 0x00044AD8
		public string GetUsername()
		{
			return this._username;
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x000468E0 File Offset: 0x00044AE0
		public string GetPassword()
		{
			return this._password;
		}

		// Token: 0x040005B7 RID: 1463
		private string _username;

		// Token: 0x040005B8 RID: 1464
		private string _password;
	}
}
