using System;
using System.Collections.Generic;

namespace RESTfulHTTPServer.src.models
{
	// Token: 0x0200007A RID: 122
	public class Request
	{
		// Token: 0x06000515 RID: 1301 RVA: 0x000468E8 File Offset: 0x00044AE8
		public Request(Route route)
		{
			this._route = route;
			this._querys = new Dictionary<string, string>();
			this._parameters = new Dictionary<string, string>();
			this._postData = "";
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x00046918 File Offset: 0x00044B18
		public Request()
		{
			this._querys = new Dictionary<string, string>();
			this._parameters = new Dictionary<string, string>();
			this._postData = "";
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x00046941 File Offset: 0x00044B41
		public void SetRoute(Route route)
		{
			this._route = route;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0004694A File Offset: 0x00044B4A
		public Route GetRoute()
		{
			return this._route;
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00046952 File Offset: 0x00044B52
		public void SetPOSTData(string postData)
		{
			this._postData = postData;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0004695B File Offset: 0x00044B5B
		public string GetPOSTData()
		{
			return this._postData;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00046963 File Offset: 0x00044B63
		public bool HasPOSTData()
		{
			return this._postData.Length > 0;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00046973 File Offset: 0x00044B73
		public void AddQuery(string key, string value)
		{
			if (!this._querys.ContainsKey(key))
			{
				this._querys.Add(key, value);
				return;
			}
			this._querys[key] = value;
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0004699E File Offset: 0x00044B9E
		public void AddParameter(string key, string value)
		{
			if (!this._parameters.ContainsKey(key))
			{
				this._parameters.Add(key, value);
				return;
			}
			this._parameters[key] = value;
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x000469C9 File Offset: 0x00044BC9
		public Dictionary<string, string> GetQuerys()
		{
			return this._querys;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x000469D1 File Offset: 0x00044BD1
		public Dictionary<string, string> GetParameters()
		{
			return this._parameters;
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0009E9B4 File Offset: 0x0009CBB4
		public string GetQuery(string key)
		{
			string result = "";
			if (this._querys.ContainsKey(key))
			{
				result = this._querys[key];
			}
			return result;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0009E9E4 File Offset: 0x0009CBE4
		public string GetParameter(string key)
		{
			string result = "";
			if (this._parameters.ContainsKey(key))
			{
				result = this._parameters[key];
			}
			return result;
		}

		// Token: 0x040005B9 RID: 1465
		private Route _route;

		// Token: 0x040005BA RID: 1466
		private Dictionary<string, string> _querys;

		// Token: 0x040005BB RID: 1467
		private Dictionary<string, string> _parameters;

		// Token: 0x040005BC RID: 1468
		private string _postData;
	}
}
