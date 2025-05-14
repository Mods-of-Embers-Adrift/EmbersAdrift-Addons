using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Cysharp.Text;
using SoL.Game;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x0200053B RID: 1339
	public class NpcBehaviorManager : MonoBehaviour
	{
		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x0600289B RID: 10395 RVA: 0x0005C30E File Offset: 0x0005A50E
		// (set) Token: 0x0600289C RID: 10396 RVA: 0x0013D068 File Offset: 0x0013B268
		public int TicksPerFrame
		{
			get
			{
				return this.m_ticksPerFrame;
			}
			set
			{
				if (GameManager.IsServer)
				{
					int ticksPerFrame = this.m_ticksPerFrame;
					this.m_ticksPerFrame = Mathf.Clamp(value, 16, 256);
					ServerGameManager.GameServerConfig.MaxNpcTicksPerFrame = this.m_ticksPerFrame;
					Debug.Log("Adjusted TicksPerFrame from " + ticksPerFrame.ToString() + " to " + this.m_ticksPerFrame.ToString());
				}
			}
		}

		// Token: 0x0600289D RID: 10397 RVA: 0x0013D0CC File Offset: 0x0013B2CC
		private void Start()
		{
			if (BehaviorManager.instance == null)
			{
				this.m_manager = base.gameObject.AddComponent<BehaviorManager>();
				this.m_manager.UpdateInterval = UpdateIntervalType.Manual;
			}
			if (ServerGameManager.GameServerConfig.MaxNpcTicksPerFrame > 0)
			{
				this.m_ticksPerFrame = Mathf.Clamp(ServerGameManager.GameServerConfig.MaxNpcTicksPerFrame, 16, 256);
			}
			Debug.Log("MaxNpcTicksPerFrame: " + this.m_ticksPerFrame.ToString());
			this.m_updateCo = this.UpdateCo();
			base.StartCoroutine(this.m_updateCo);
		}

		// Token: 0x0600289E RID: 10398 RVA: 0x0005C316 File Offset: 0x0005A516
		private void OnDestroy()
		{
			if (this.m_updateCo != null)
			{
				base.StopCoroutine(this.m_updateCo);
			}
		}

		// Token: 0x0600289F RID: 10399 RVA: 0x0005C32C File Offset: 0x0005A52C
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				int currentPlayerCount = BaseNetworkEntityManager.PlayerConnectedCount;
				if (currentPlayerCount > 0 || Time.time - this.m_timeOfLastPlayerPresent < 300f)
				{
					int ticksCycled = 0;
					int num;
					for (int i = 0; i < this.m_npcs.Count; i = num + 1)
					{
						if (this.m_npcs[i] == null || this.m_npcs[i].Tree == null)
						{
							this.m_npcs.RemoveAt(i);
							i--;
						}
						else
						{
							bool flag = false;
							try
							{
								flag = this.m_npcs[i].BehaviorTick();
							}
							catch (Exception arg)
							{
								flag = false;
								Debug.LogError(string.Format("Caught NPC tick exception!  {0}", arg));
							}
							if (flag)
							{
								ticksCycled++;
								if (ticksCycled >= this.m_ticksPerFrame)
								{
									ticksCycled = 0;
									yield return null;
								}
							}
						}
						num = i;
					}
				}
				if (currentPlayerCount > 0)
				{
					this.m_timeOfLastPlayerPresent = Time.time;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x060028A0 RID: 10400 RVA: 0x0013D160 File Offset: 0x0013B360
		public BehaviorManager RegisterNpc(ServerNpcController controller)
		{
			if (controller.Tree == null)
			{
				throw new ArgumentException("Controller on " + controller.GameEntity.gameObject.name + " has no behavior tree!");
			}
			this.m_npcs.Add(controller);
			return this.m_manager;
		}

		// Token: 0x060028A1 RID: 10401 RVA: 0x0005C33B File Offset: 0x0005A53B
		public void UnregisterNpc(ServerNpcController controller)
		{
			this.m_npcs.Remove(controller);
		}

		// Token: 0x060028A2 RID: 10402 RVA: 0x0013D1B4 File Offset: 0x0013B3B4
		public string GetNpcStatSummary()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (ServerNpcController serverNpcController in this.m_npcs)
			{
				if (serverNpcController && serverNpcController.GameEntity && serverNpcController.GameEntity.NetworkEntity)
				{
					num++;
					if (serverNpcController.Motor && serverNpcController.Motor.UnityNavAgent && serverNpcController.Motor.UnityNavAgent.enabled && serverNpcController.Motor.UnityNavAgent.pathPending)
					{
						num2++;
					}
					if (serverNpcController.GameEntity.NetworkEntity.NObservers > 0)
					{
						num3++;
					}
					if (serverNpcController.GameEntity.ServerNpcController && serverNpcController.GameEntity.ServerNpcController.EnableBehavior)
					{
						num4++;
					}
				}
			}
			return ZString.Format<int, int, int, int>("Total={0}, HasObservers={1}, BehaviorEnabled={2}, PathPending={3}", num, num3, num4, num2);
		}

		// Token: 0x040029D1 RID: 10705
		private const int kMinTicksPerFrame = 16;

		// Token: 0x040029D2 RID: 10706
		private const int kMaxTicksPerFrame = 256;

		// Token: 0x040029D3 RID: 10707
		private readonly List<ServerNpcController> m_npcs = new List<ServerNpcController>(1024);

		// Token: 0x040029D4 RID: 10708
		private IEnumerator m_updateCo;

		// Token: 0x040029D5 RID: 10709
		private BehaviorManager m_manager;

		// Token: 0x040029D6 RID: 10710
		private int m_ticksPerFrame = 16;

		// Token: 0x040029D7 RID: 10711
		private float m_timeOfLastPlayerPresent = -300f;
	}
}
