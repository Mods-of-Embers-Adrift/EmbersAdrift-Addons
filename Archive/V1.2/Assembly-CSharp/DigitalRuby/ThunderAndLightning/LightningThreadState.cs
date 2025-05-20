using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000BD RID: 189
	public class LightningThreadState
	{
		// Token: 0x060006F6 RID: 1782 RVA: 0x000AB430 File Offset: 0x000A9630
		private bool UpdateMainThreadActionsOnce(bool inDestroy)
		{
			Queue<KeyValuePair<Action<bool>, ManualResetEvent>> obj = this.actionsForMainThread;
			KeyValuePair<Action<bool>, ManualResetEvent> keyValuePair;
			lock (obj)
			{
				if (this.actionsForMainThread.Count == 0)
				{
					return false;
				}
				keyValuePair = this.actionsForMainThread.Dequeue();
			}
			try
			{
				keyValuePair.Key(inDestroy);
			}
			catch
			{
			}
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.Set();
			}
			return true;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x000AB4C0 File Offset: 0x000A96C0
		private void BackgroundThreadMethod()
		{
			Action action = null;
			while (this.Running)
			{
				try
				{
					if (this.lightningThreadEvent.WaitOne(500))
					{
						for (;;)
						{
							Queue<Action> obj = this.actionsForBackgroundThread;
							lock (obj)
							{
								if (this.actionsForBackgroundThread.Count == 0)
								{
									break;
								}
								action = this.actionsForBackgroundThread.Dequeue();
							}
							action();
						}
					}
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Lightning thread exception: {0}", new object[]
					{
						ex
					});
				}
			}
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x000AB570 File Offset: 0x000A9770
		public LightningThreadState()
		{
			this.lightningThread = new Thread(new ThreadStart(this.BackgroundThreadMethod))
			{
				IsBackground = true,
				Name = "LightningBoltScriptThread"
			};
			this.lightningThread.Start();
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x000AB5E0 File Offset: 0x000A97E0
		public void TerminateAndWaitForEnd(bool inDestroy)
		{
			this.isTerminating = true;
			for (;;)
			{
				if (!this.UpdateMainThreadActionsOnce(inDestroy))
				{
					Queue<Action> obj = this.actionsForBackgroundThread;
					lock (obj)
					{
						if (this.actionsForBackgroundThread.Count != 0)
						{
							continue;
						}
					}
					break;
				}
			}
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00047BE5 File Offset: 0x00045DE5
		public void UpdateMainThreadActions()
		{
			while (this.UpdateMainThreadActionsOnce(false))
			{
			}
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x000AB63C File Offset: 0x000A983C
		public bool AddActionForMainThread(Action<bool> action, bool waitForAction = false)
		{
			if (this.isTerminating)
			{
				return false;
			}
			ManualResetEvent manualResetEvent = waitForAction ? new ManualResetEvent(false) : null;
			Queue<KeyValuePair<Action<bool>, ManualResetEvent>> obj = this.actionsForMainThread;
			lock (obj)
			{
				this.actionsForMainThread.Enqueue(new KeyValuePair<Action<bool>, ManualResetEvent>(action, manualResetEvent));
			}
			if (manualResetEvent != null)
			{
				manualResetEvent.WaitOne(10000);
			}
			return true;
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x000AB6B0 File Offset: 0x000A98B0
		public bool AddActionForBackgroundThread(Action action)
		{
			if (this.isTerminating)
			{
				return false;
			}
			Queue<Action> obj = this.actionsForBackgroundThread;
			lock (obj)
			{
				this.actionsForBackgroundThread.Enqueue(action);
			}
			this.lightningThreadEvent.Set();
			return true;
		}

		// Token: 0x04000840 RID: 2112
		private Thread lightningThread;

		// Token: 0x04000841 RID: 2113
		private AutoResetEvent lightningThreadEvent = new AutoResetEvent(false);

		// Token: 0x04000842 RID: 2114
		private readonly Queue<Action> actionsForBackgroundThread = new Queue<Action>();

		// Token: 0x04000843 RID: 2115
		private readonly Queue<KeyValuePair<Action<bool>, ManualResetEvent>> actionsForMainThread = new Queue<KeyValuePair<Action<bool>, ManualResetEvent>>();

		// Token: 0x04000844 RID: 2116
		public bool Running = true;

		// Token: 0x04000845 RID: 2117
		private bool isTerminating;
	}
}
