using System;
using DisruptorUnity3d;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000298 RID: 664
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x0600141D RID: 5149 RVA: 0x000F9418 File Offset: 0x000F7618
		public void Update()
		{
			if (MainThreadDispatcher.ExecuteOnMainThread.Count <= 0)
			{
				return;
			}
			Action action;
			while (MainThreadDispatcher.ExecuteOnMainThread.TryDequeue(out action))
			{
				action();
			}
		}

		// Token: 0x04001C76 RID: 7286
		private const int kRingBufferCapacity = 16;

		// Token: 0x04001C77 RID: 7287
		public static readonly RingBuffer<Action> ExecuteOnMainThread = new RingBuffer<Action>(16);
	}
}
