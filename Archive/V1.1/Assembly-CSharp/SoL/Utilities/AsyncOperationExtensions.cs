using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200027E RID: 638
	public static class AsyncOperationExtensions
	{
		// Token: 0x060013E1 RID: 5089 RVA: 0x000F8354 File Offset: 0x000F6554
		public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
			asyncOp.completed += delegate(AsyncOperation obj)
			{
				tcs.SetResult(null);
			};
			return tcs.Task.GetAwaiter();
		}
	}
}
