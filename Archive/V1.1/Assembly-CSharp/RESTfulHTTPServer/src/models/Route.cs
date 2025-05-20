using System;

namespace RESTfulHTTPServer.src.models
{
	// Token: 0x0200007C RID: 124
	public class Route
	{
		// Token: 0x0600052A RID: 1322 RVA: 0x00046A52 File Offset: 0x00044C52
		public Route(Route.Type type, string url, string invokerInfo)
		{
			this._className = "";
			this._methodName = "";
			this._type = type;
			this._url = url;
			this._invokerInfo = invokerInfo;
			this.InitInvoker();
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00046A8B File Offset: 0x00044C8B
		public Route.Type GetHTTPType()
		{
			return this._type;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00046A93 File Offset: 0x00044C93
		public string GetUrl()
		{
			return this._url;
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00046A9B File Offset: 0x00044C9B
		public string GetInvokerClass()
		{
			return this._className;
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00046AA3 File Offset: 0x00044CA3
		public string GetInvokerMethod()
		{
			return this._methodName;
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0009E9F4 File Offset: 0x0009CBF4
		private void InitInvoker()
		{
			string[] array = this._invokerInfo.Split('.', StringSplitOptions.None);
			if (array.Length == 2)
			{
				this._className = array[0];
				this._methodName = array[1];
				return;
			}
			throw new Exception("Invoker parameter missmatch");
		}

		// Token: 0x040005C4 RID: 1476
		private Route.Type _type;

		// Token: 0x040005C5 RID: 1477
		private string _url;

		// Token: 0x040005C6 RID: 1478
		private string _invokerInfo;

		// Token: 0x040005C7 RID: 1479
		private string _className;

		// Token: 0x040005C8 RID: 1480
		private string _methodName;

		// Token: 0x0200007D RID: 125
		public enum Type
		{
			// Token: 0x040005CA RID: 1482
			GET,
			// Token: 0x040005CB RID: 1483
			POST,
			// Token: 0x040005CC RID: 1484
			DELETE,
			// Token: 0x040005CD RID: 1485
			PUT
		}
	}
}
