using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000526 RID: 1318
	public class WaterTrailManager : MonoBehaviour
	{
		// Token: 0x0600278B RID: 10123 RVA: 0x001384CC File Offset: 0x001366CC
		private void Awake()
		{
			this.m_poolParent = new GameObject("POOL_WaterTrail").transform;
			this.m_activeParent = new GameObject("ACTIVE_WaterTrail").transform;
			this.m_waterTrailPool = new Stack<WaterTrailManager.WaterTrailController>(8);
			this.m_activeWaterTrails = new List<WaterTrailManager.WaterTrailController>(8);
		}

		// Token: 0x0600278C RID: 10124 RVA: 0x0013851C File Offset: 0x0013671C
		private void Start()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_waterTrailPool.Push(WaterTrailManager.GetNewWaterTrail(this.m_poolParent));
			}
		}

		// Token: 0x0600278D RID: 10125 RVA: 0x0013854C File Offset: 0x0013674C
		private void Update()
		{
			for (int i = 0; i < this.m_activeWaterTrails.Count; i++)
			{
				WaterTrailManager.WaterTrailController waterTrailController = this.m_activeWaterTrails[i];
				if (waterTrailController.Expired())
				{
					waterTrailController.Obj.SetActive(false);
					waterTrailController.Obj.transform.position = Vector3.zero;
					waterTrailController.Obj.transform.SetParent(this.m_poolParent);
					this.m_waterTrailPool.Push(waterTrailController);
					this.m_activeWaterTrails.RemoveAt(i);
					i--;
				}
				else
				{
					waterTrailController.UpdatePosition();
				}
			}
		}

		// Token: 0x0600278E RID: 10126 RVA: 0x001385E4 File Offset: 0x001367E4
		private static WaterTrailManager.WaterTrailController GetNewWaterTrail(Transform parent)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GlobalSettings.Values.Animation.WaterTrailPrefab, parent);
			gameObject.SetActive(false);
			return new WaterTrailManager.WaterTrailController
			{
				Height = 0f,
				Obj = gameObject,
				Entity = null
			};
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x0013862C File Offset: 0x0013682C
		public WaterTrailManager.WaterTrailController RequestWaterTrail(WaterTrail trail, Collider obj)
		{
			if (GameManager.IsServer || !trail || !obj)
			{
				return null;
			}
			WaterTrailManager.WaterTrailController waterTrailController = (this.m_waterTrailPool.Count > 0) ? this.m_waterTrailPool.Pop() : WaterTrailManager.GetNewWaterTrail(this.m_activeParent);
			if (waterTrailController != null)
			{
				this.m_activeWaterTrails.Add(waterTrailController);
				waterTrailController.Obj.transform.SetParent(this.m_activeParent);
				waterTrailController.Entity = trail.GameEntity;
				waterTrailController.Height = obj.transform.position.y;
				waterTrailController.UpdatePosition();
				waterTrailController.Obj.SetActive(true);
			}
			return waterTrailController;
		}

		// Token: 0x0400293C RID: 10556
		private const float kDisableDelay = 5f;

		// Token: 0x0400293D RID: 10557
		private const int kInitialPool = 4;

		// Token: 0x0400293E RID: 10558
		private const int kInitialCapacity = 8;

		// Token: 0x0400293F RID: 10559
		private Transform m_poolParent;

		// Token: 0x04002940 RID: 10560
		private Transform m_activeParent;

		// Token: 0x04002941 RID: 10561
		private Stack<WaterTrailManager.WaterTrailController> m_waterTrailPool;

		// Token: 0x04002942 RID: 10562
		private List<WaterTrailManager.WaterTrailController> m_activeWaterTrails;

		// Token: 0x02000527 RID: 1319
		public class WaterTrailController
		{
			// Token: 0x1700082F RID: 2095
			// (get) Token: 0x06002791 RID: 10129 RVA: 0x0005BC40 File Offset: 0x00059E40
			// (set) Token: 0x06002792 RID: 10130 RVA: 0x001386D4 File Offset: 0x001368D4
			public GameEntity Entity
			{
				get
				{
					return this.m_entity;
				}
				set
				{
					if (this.m_entity && !value)
					{
						this.m_expirationTime = new float?(Time.time + 5f);
					}
					else
					{
						this.m_expirationTime = null;
					}
					this.m_entity = value;
				}
			}

			// Token: 0x06002793 RID: 10131 RVA: 0x0005BC48 File Offset: 0x00059E48
			public bool Expired()
			{
				return !this.Entity && (this.m_expirationTime == null || this.m_expirationTime.Value < Time.time);
			}

			// Token: 0x06002794 RID: 10132 RVA: 0x00138724 File Offset: 0x00136924
			public void UpdatePosition()
			{
				if (this.Entity)
				{
					Vector3 position = this.Entity.gameObject.transform.position;
					position.y = this.Height;
					this.Obj.transform.position = position;
				}
			}

			// Token: 0x04002943 RID: 10563
			public float Height;

			// Token: 0x04002944 RID: 10564
			public GameObject Obj;

			// Token: 0x04002945 RID: 10565
			private float? m_expirationTime;

			// Token: 0x04002946 RID: 10566
			private GameEntity m_entity;
		}
	}
}
