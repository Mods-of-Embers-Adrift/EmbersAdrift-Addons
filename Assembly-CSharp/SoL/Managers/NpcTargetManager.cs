using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.NPCs;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x0200053D RID: 1341
	public class NpcTargetManager : MonoBehaviour
	{
		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x060028AA RID: 10410 RVA: 0x0005C38C File Offset: 0x0005A58C
		// (set) Token: 0x060028AB RID: 10411 RVA: 0x0013D464 File Offset: 0x0013B664
		public int BucketSize
		{
			get
			{
				return this.m_bucketSize;
			}
			set
			{
				if (GameManager.IsServer)
				{
					int bucketSize = this.m_bucketSize;
					this.m_bucketSize = Mathf.Clamp(value, 16, 256);
					Debug.Log("Adjusted BucketSize from " + bucketSize.ToString() + " to " + this.m_bucketSize.ToString());
				}
			}
		}

		// Token: 0x060028AC RID: 10412 RVA: 0x0005C394 File Offset: 0x0005A594
		private void Start()
		{
			this.m_updateCo = this.UpdateCo();
			base.StartCoroutine(this.m_updateCo);
		}

		// Token: 0x060028AD RID: 10413 RVA: 0x0005C3AF File Offset: 0x0005A5AF
		private void OnDestroy()
		{
			base.StopCoroutine(this.m_updateCo);
		}

		// Token: 0x060028AE RID: 10414 RVA: 0x0005C3BD File Offset: 0x0005A5BD
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				if (BaseNetworkEntityManager.PlayerConnectedCount > 0)
				{
					DateTime now = DateTime.UtcNow;
					int num;
					for (int i = 0; i < this.m_buckets.Count; i = num + 1)
					{
						if (this.m_buckets[i].Count > 0 && now >= this.m_buckets[i].NextUpdateTime)
						{
							this.m_buckets[i].Update();
							yield return null;
						}
						num = i;
					}
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x060028AF RID: 10415 RVA: 0x0013D4B8 File Offset: 0x0013B6B8
		public void RegisterNpc(NpcTargetController controller)
		{
			bool flag = true;
			if (this.m_buckets.Count > 0)
			{
				for (int i = 0; i < this.m_buckets.Count; i++)
				{
					if (this.m_buckets[i].Count < this.m_bucketSize)
					{
						this.m_buckets[i].AddController(controller);
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				NpcTargetManager.NpcTargetControllerBucket item = new NpcTargetManager.NpcTargetControllerBucket(controller);
				this.m_buckets.Add(item);
			}
		}

		// Token: 0x040029DE RID: 10718
		private const int kMinBucketSize = 16;

		// Token: 0x040029DF RID: 10719
		private const int kMaxBucketSize = 256;

		// Token: 0x040029E0 RID: 10720
		private const float kTickRate = 1f;

		// Token: 0x040029E1 RID: 10721
		private int m_bucketSize = 16;

		// Token: 0x040029E2 RID: 10722
		private readonly List<NpcTargetManager.NpcTargetControllerBucket> m_buckets = new List<NpcTargetManager.NpcTargetControllerBucket>(128);

		// Token: 0x040029E3 RID: 10723
		private IEnumerator m_updateCo;

		// Token: 0x0200053E RID: 1342
		private class NpcTargetControllerBucket
		{
			// Token: 0x1700085C RID: 2140
			// (get) Token: 0x060028B1 RID: 10417 RVA: 0x0005C3EC File Offset: 0x0005A5EC
			public int Count
			{
				get
				{
					return this.m_controllers.Count;
				}
			}

			// Token: 0x1700085D RID: 2141
			// (get) Token: 0x060028B2 RID: 10418 RVA: 0x0005C3F9 File Offset: 0x0005A5F9
			public DateTime NextUpdateTime
			{
				get
				{
					return this.m_nextUpdateTime;
				}
			}

			// Token: 0x060028B3 RID: 10419 RVA: 0x0013D530 File Offset: 0x0013B730
			public NpcTargetControllerBucket(NpcTargetController controller)
			{
				this.m_controllers = new List<NpcTargetController>(256)
				{
					controller
				};
				this.m_nextUpdateTime = DateTime.UtcNow.AddSeconds(1.0);
			}

			// Token: 0x060028B4 RID: 10420 RVA: 0x0005C401 File Offset: 0x0005A601
			public void AddController(NpcTargetController controller)
			{
				this.m_controllers.Add(controller);
			}

			// Token: 0x060028B5 RID: 10421 RVA: 0x0013D584 File Offset: 0x0013B784
			public void Update()
			{
				for (int i = 0; i < this.m_controllers.Count; i++)
				{
					if (this.m_controllers[i] == null)
					{
						this.m_controllers.RemoveAt(i);
						i--;
					}
					else
					{
						this.m_controllers[i].UpdateExternal();
					}
				}
				this.m_nextUpdateTime = DateTime.UtcNow.AddSeconds(1.0);
			}

			// Token: 0x040029E4 RID: 10724
			private readonly List<NpcTargetController> m_controllers;

			// Token: 0x040029E5 RID: 10725
			private DateTime m_nextUpdateTime = DateTime.MinValue;
		}
	}
}
