using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using RESTfulHTTPServer.src.models;

namespace RESTfulHTTPServer.src.controller
{
	// Token: 0x02000080 RID: 128
	public class RoutingManager
	{
		// Token: 0x06000531 RID: 1329 RVA: 0x00046AAB File Offset: 0x00044CAB
		public RoutingManager()
		{
			this._getRoutes = new List<Route>();
			this._postRoutes = new List<Route>();
			this._putRoutes = new List<Route>();
			this._deleteRoutes = new List<Route>();
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0009EA7C File Offset: 0x0009CC7C
		public void AddRoute(Route route)
		{
			switch (route.GetHTTPType())
			{
			case Route.Type.GET:
				this._getRoutes.Add(route);
				return;
			case Route.Type.POST:
				this._postRoutes.Add(route);
				return;
			case Route.Type.DELETE:
				this._deleteRoutes.Add(route);
				return;
			case Route.Type.PUT:
				this._putRoutes.Add(route);
				return;
			default:
				throw new Exception("HTTP Type not matching " + route.GetHTTPType().ToString());
			}
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00046ADF File Offset: 0x00044CDF
		public void AddGETRoute(Route route)
		{
			if (route.GetHTTPType() == Route.Type.GET)
			{
				this._getRoutes.Add(route);
				return;
			}
			throw new Exception("HTTP Type not matching");
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00046B00 File Offset: 0x00044D00
		public void AddPOSTRoute(Route route)
		{
			if (route.GetHTTPType() == Route.Type.POST)
			{
				this._postRoutes.Add(route);
				return;
			}
			throw new Exception("HTTP Type not matching");
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x00046B22 File Offset: 0x00044D22
		public void AddPUTRoute(Route route)
		{
			if (route.GetHTTPType() == Route.Type.PUT)
			{
				this._putRoutes.Add(route);
				return;
			}
			throw new Exception("HTTP Type not matching");
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00046B44 File Offset: 0x00044D44
		public void AddDELETERoute(Route route)
		{
			if (route.GetHTTPType() == Route.Type.DELETE)
			{
				this._deleteRoutes.Add(route);
				return;
			}
			throw new Exception("HTTP Type not matching");
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00046B66 File Offset: 0x00044D66
		public List<Route> GetGETRoutes()
		{
			return this._getRoutes;
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x00046B6E File Offset: 0x00044D6E
		public List<Route> GetPOSTRoutes()
		{
			return this._postRoutes;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00046B76 File Offset: 0x00044D76
		public List<Route> GetPUTRoutes()
		{
			return this._putRoutes;
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x00046B7E File Offset: 0x00044D7E
		public List<Route> GetDELETERoutes()
		{
			return this._deleteRoutes;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0009EB00 File Offset: 0x0009CD00
		public Request DoesRouteExists(string calledUrl, string type)
		{
			Route.Type type2;
			if (!(type == "GET"))
			{
				if (!(type == "POST"))
				{
					if (!(type == "PUT"))
					{
						if (!(type == "DELETE"))
						{
							return null;
						}
						type2 = Route.Type.DELETE;
					}
					else
					{
						type2 = Route.Type.PUT;
					}
				}
				else
				{
					type2 = Route.Type.POST;
				}
			}
			else
			{
				type2 = Route.Type.GET;
			}
			return this.DoesRouteExists(calledUrl, type2);
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0009EB60 File Offset: 0x0009CD60
		public Request DoesRouteExists(string calledUrl, Route.Type type)
		{
			List<Route> routes;
			switch (type)
			{
			case Route.Type.GET:
				routes = this._getRoutes;
				break;
			case Route.Type.POST:
				routes = this._postRoutes;
				break;
			case Route.Type.DELETE:
				routes = this._deleteRoutes;
				break;
			case Route.Type.PUT:
				routes = this._putRoutes;
				break;
			default:
				if (this.m_emptyRouteList == null)
				{
					this.m_emptyRouteList = new List<Route>();
				}
				routes = this.m_emptyRouteList;
				break;
			}
			return RoutingManager.DoesRouteExists(routes, calledUrl);
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0009EBCC File Offset: 0x0009CDCC
		public Response CallDelegater(Request request)
		{
			Response response;
			try
			{
				response = (Response)Type.GetType("SoL.Networking.REST.Invokers." + request.GetRoute().GetInvokerClass()).GetMethod(request.GetRoute().GetInvokerMethod(), BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[]
				{
					request
				});
			}
			catch (Exception ex)
			{
				Logger.Log("Routing Manager", ex.ToString());
				response = new Response();
				response.SetContent("Error: Unable to call invoker.");
				response.SetHTTPStatusCode(404);
				response.SetMimeType("text/html");
			}
			return response;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0009EC68 File Offset: 0x0009CE68
		private static Request DoesRouteExists(List<Route> routes, string calledUrl)
		{
			Request request = new Request();
			bool flag = false;
			string[] array = RoutingManager.RemoveStartingSlash(calledUrl).Split('/', StringSplitOptions.None);
			int num = 0;
			while (num < routes.Count && !flag)
			{
				string[] array2 = RoutingManager.RemoveStartingSlash(routes[num].GetUrl()).Split('/', StringSplitOptions.None);
				if (array.Length == array2.Length)
				{
					bool flag2 = true;
					int num2 = 0;
					while (num2 < array2.Length && flag2)
					{
						string text = array2[num2];
						string text2 = array[num2];
						if (RoutingManager.IsUrlArgumentParameter(text))
						{
							if (text2.Length == 0)
							{
								flag2 = false;
							}
							else
							{
								request.AddParameter(text.Substring(1, text.Length - 2), text2);
								Logger.Log("Routing Manager", "Added parameter to route, key: " + text.Substring(1, text.Length - 2) + " value: " + text2);
							}
						}
						else if (!text2.Equals(text))
						{
							flag2 = false;
						}
						num2++;
					}
					if (flag2)
					{
						Route route = routes[num];
						request.SetRoute(route);
						flag = true;
					}
				}
				num++;
			}
			return request;
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00046B86 File Offset: 0x00044D86
		private static bool IsUrlArgumentParameter(string token)
		{
			return token.StartsWith("{") && token.EndsWith("}");
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00046BA2 File Offset: 0x00044DA2
		private static string RemoveStartingSlash(string url)
		{
			if (url.StartsWith("/"))
			{
				url = url.Substring(1);
			}
			return url;
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0009ED84 File Offset: 0x0009CF84
		public Request DetermineURLQuery(HttpListenerContext context, Request request)
		{
			for (int i = 0; i < context.Request.QueryString.Count; i++)
			{
				string key = context.Request.QueryString.GetKey(i);
				string text = context.Request.QueryString[i];
				if (key != null && (text != null & key.Length > 0) && text.Length > 0)
				{
					request.AddQuery(key, text);
					Logger.Log("Routing Manager", "Added query to route, key: " + key + " ,value: " + text);
				}
				else
				{
					Logger.Log("Routing Manager", "Invalid URL query");
				}
			}
			return request;
		}

		// Token: 0x040005D1 RID: 1489
		private const string TAG = "Routing Manager";

		// Token: 0x040005D2 RID: 1490
		private readonly List<Route> _getRoutes;

		// Token: 0x040005D3 RID: 1491
		private readonly List<Route> _postRoutes;

		// Token: 0x040005D4 RID: 1492
		private readonly List<Route> _putRoutes;

		// Token: 0x040005D5 RID: 1493
		private readonly List<Route> _deleteRoutes;

		// Token: 0x040005D6 RID: 1494
		private List<Route> m_emptyRouteList;
	}
}
