using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RESTfulHTTPServer.src.models;
using UnityEngine.Profiling;

namespace RESTfulHTTPServer.src.controller
{
	// Token: 0x02000081 RID: 129
	internal class SimpleRESTServer
	{
		// Token: 0x06000542 RID: 1346 RVA: 0x00046BBB File Offset: 0x00044DBB
		public SimpleRESTServer(int port, RoutingManager routingManager)
		{
			this._routingManager = routingManager;
			this._basicAuth = null;
			this.Initialize(port);
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00046BD8 File Offset: 0x00044DD8
		public SimpleRESTServer(int port, RoutingManager routingManager, BasicAuth basicAuth)
		{
			this._routingManager = routingManager;
			this._basicAuth = basicAuth;
			this.Initialize(port);
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0009EE04 File Offset: 0x0009D004
		public SimpleRESTServer(RoutingManager routingManager)
		{
			this._routingManager = routingManager;
			this._basicAuth = null;
			TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
			tcpListener.Start();
			int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
			tcpListener.Stop();
			this.Initialize(port);
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00046BF5 File Offset: 0x00044DF5
		private void Initialize(int port)
		{
			this.m_threadActive = true;
			this._port = port;
			this._serverThread = new Thread(new ThreadStart(this.Listen));
			this._serverThread.Start();
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x00046C29 File Offset: 0x00044E29
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x0004475B File Offset: 0x0004295B
		public int Port
		{
			get
			{
				return this._port;
			}
			private set
			{
			}
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00046C31 File Offset: 0x00044E31
		public void Stop()
		{
			this.m_threadActive = false;
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0009EE54 File Offset: 0x0009D054
		private void Listen()
		{
			this._listener = new HttpListener();
			this._listener.Prefixes.Add("http://*:" + this._port.ToString() + "/");
			if (this._basicAuth != null)
			{
				this._listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
			}
			this._listener.Start();
			Logger.Log("Simple REST Server", "Server is up and running on port " + this._port.ToString());
			while (this.m_threadActive)
			{
				try
				{
					HttpListenerContext context = this._listener.GetContext();
					this.Process(context);
				}
				catch (Exception ex)
				{
					Logger.Log("Simple REST Server", ex.ToString());
				}
				Profiler.EndThreadProfiling();
			}
			this._listener.Stop();
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00046C3C File Offset: 0x00044E3C
		public static Stream GenerateStreamFromString(string s)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(s);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			return memoryStream;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0009EF2C File Offset: 0x0009D12C
		private Request ReceivePostData(Request request, HttpListenerRequest httpRequest)
		{
			using (Stream inputStream = httpRequest.InputStream)
			{
				string postdata;
				using (StreamReader streamReader = new StreamReader(inputStream, httpRequest.ContentEncoding))
				{
					postdata = streamReader.ReadToEnd();
				}
				request.SetPOSTData(postdata);
			}
			return request;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0009EF94 File Offset: 0x0009D194
		private void Process(HttpListenerContext context)
		{
			Response response = new Response();
			HttpListenerRequest request = context.Request;
			string absolutePath = context.Request.Url.AbsolutePath;
			Logger.Log("Simple REST Server", "Rquest Type: " + request.HttpMethod);
			Logger.Log("Simple REST Server", "Requested URL: " + absolutePath);
			bool flag = false;
			if (this._basicAuth == null)
			{
				flag = true;
			}
			else
			{
				HttpListenerBasicIdentity httpListenerBasicIdentity = (HttpListenerBasicIdentity)context.User.Identity;
				if (this._basicAuth.GetUsername().Equals(httpListenerBasicIdentity.Name) && this._basicAuth.GetPassword().Equals(httpListenerBasicIdentity.Password))
				{
					flag = true;
					Logger.Log("Simple REST Server", "Username: " + httpListenerBasicIdentity.Name);
					Logger.Log("Simple REST Server", "Password: " + httpListenerBasicIdentity.Password);
				}
			}
			string s;
			if (flag)
			{
				Request request2 = this._routingManager.DoesRouteExists(absolutePath, request.HttpMethod);
				if (request2.GetRoute() != null)
				{
					if (request.HttpMethod.Equals("POST") || (request.HttpMethod.Equals("PUT") && request.HasEntityBody))
					{
						request2 = this.ReceivePostData(request2, request);
					}
					request2 = this._routingManager.DetermineURLQuery(context, request2);
					response = this._routingManager.CallDelegater(request2);
					context.Response.StatusCode = response.GetHTTPStatusCode();
					s = response.GetContent();
				}
				else
				{
					context.Response.StatusCode = 404;
					s = "<html><head><title>Simple REST Server</title></head><body>404</body></html>";
				}
			}
			else
			{
				context.Response.StatusCode = 401;
				s = "<html><head><title>Simple REST Server</title></head><body>401</body></html>";
			}
			try
			{
				Stream stream = SimpleRESTServer.GenerateStreamFromString(s);
				context.Response.ContentType = response.GetMIMEType();
				context.Response.ContentLength64 = stream.Length;
				context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
				context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(absolutePath).ToString("r"));
				byte[] array = new byte[16384];
				int count;
				while ((count = stream.Read(array, 0, array.Length)) > 0)
				{
					context.Response.OutputStream.Write(array, 0, count);
				}
				stream.Close();
				context.Response.OutputStream.Flush();
			}
			catch (Exception ex)
			{
				Logger.Log("Simple REST Server", ex.ToString());
				context.Response.StatusCode = 500;
			}
			context.Response.OutputStream.Close();
		}

		// Token: 0x040005D7 RID: 1495
		private const string TAG = "Simple REST Server";

		// Token: 0x040005D8 RID: 1496
		private int _port;

		// Token: 0x040005D9 RID: 1497
		private Thread _serverThread;

		// Token: 0x040005DA RID: 1498
		private HttpListener _listener;

		// Token: 0x040005DB RID: 1499
		private RoutingManager _routingManager;

		// Token: 0x040005DC RID: 1500
		private BasicAuth _basicAuth;

		// Token: 0x040005DD RID: 1501
		private volatile bool m_threadActive;
	}
}
