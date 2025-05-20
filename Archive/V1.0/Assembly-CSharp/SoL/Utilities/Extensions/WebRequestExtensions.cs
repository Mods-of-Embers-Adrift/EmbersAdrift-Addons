using System;
using UnityEngine.Networking;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000342 RID: 834
	public static class WebRequestExtensions
	{
		// Token: 0x060016D6 RID: 5846 RVA: 0x00051FC3 File Offset: 0x000501C3
		public static bool IsWebError(this UnityWebRequest webRequest)
		{
			return webRequest.result != UnityWebRequest.Result.Success;
		}
	}
}
