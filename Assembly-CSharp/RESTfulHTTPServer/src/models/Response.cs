using System;

namespace RESTfulHTTPServer.src.models
{
	// Token: 0x0200007B RID: 123
	public class Response
	{
		// Token: 0x06000522 RID: 1314 RVA: 0x000469D9 File Offset: 0x00044BD9
		public Response()
		{
			this._mimeType = "text/html";
			this._content = "";
			this._httpStatus = 204;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00046A02 File Offset: 0x00044C02
		public Response(string mimeType, string content, int httpStatus)
		{
			this._mimeType = mimeType;
			this._content = content;
			this._httpStatus = httpStatus;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00046A1F File Offset: 0x00044C1F
		public string GetMIMEType()
		{
			return this._mimeType;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00046A27 File Offset: 0x00044C27
		public void SetMimeType(string mimeType)
		{
			this._mimeType = mimeType;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00046A30 File Offset: 0x00044C30
		public void SetContent(string content)
		{
			this._content = content;
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00046A39 File Offset: 0x00044C39
		public string GetContent()
		{
			return this._content;
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00046A41 File Offset: 0x00044C41
		public int GetHTTPStatusCode()
		{
			return this._httpStatus;
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x00046A49 File Offset: 0x00044C49
		public void SetHTTPStatusCode(int httpStatus)
		{
			this._httpStatus = httpStatus;
		}

		// Token: 0x040005BD RID: 1469
		public const string MIME_CONTENT_TYPE_JSON = "application/json";

		// Token: 0x040005BE RID: 1470
		public const string MIME_CONTENT_TYPE_XML = "application/xml";

		// Token: 0x040005BF RID: 1471
		public const string MIME_CONTENT_TYPE_HTML = "text/html";

		// Token: 0x040005C0 RID: 1472
		public const string MIME_CONTENT_TYPE_TEXT = "text/plain";

		// Token: 0x040005C1 RID: 1473
		private string _mimeType;

		// Token: 0x040005C2 RID: 1474
		private string _content;

		// Token: 0x040005C3 RID: 1475
		private int _httpStatus;
	}
}
