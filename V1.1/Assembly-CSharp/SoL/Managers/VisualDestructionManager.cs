using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000524 RID: 1316
	public class VisualDestructionManager : MonoBehaviour
	{
		// Token: 0x0600277A RID: 10106 RVA: 0x0005BA79 File Offset: 0x00059C79
		public static void SinkEntity(GameEntity entity, float delay)
		{
			if (!VisualDestructionManager.m_instance || !entity)
			{
				return;
			}
			VisualDestructionManager.m_instance.StartSink(entity, delay);
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x0005BA9C File Offset: 0x00059C9C
		private void Awake()
		{
			if (VisualDestructionManager.m_instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			VisualDestructionManager.m_instance = this;
		}

		// Token: 0x0600277C RID: 10108 RVA: 0x0013834C File Offset: 0x0013654C
		private void Update()
		{
			if (this.m_currentlyDestroying.Count <= 0)
			{
				return;
			}
			float time = Time.time;
			for (int i = 0; i < this.m_currentlyDestroying.Count; i++)
			{
				if (this.m_currentlyDestroying[i].RemoveFromList())
				{
					VisualDestructionManager.DestructionData destructionData = this.m_currentlyDestroying[i];
					destructionData.Finished();
					StaticPool<VisualDestructionManager.DestructionData>.ReturnToPool(destructionData);
					this.m_currentlyDestroying.RemoveAt(i);
					i--;
				}
				else if (time >= this.m_currentlyDestroying[i].TimeToStart)
				{
					this.m_currentlyDestroying[i].ProgressDestruction();
				}
			}
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x001383E8 File Offset: 0x001365E8
		private void StartSink(GameEntity entity, float delay)
		{
			VisualDestructionManager.DestructionData fromPool = StaticPool<VisualDestructionManager.DestructionData>.GetFromPool();
			fromPool.Init(entity, delay);
			if (this.m_currentlyDestroying.Contains(fromPool))
			{
				StaticPool<VisualDestructionManager.DestructionData>.ReturnToPool(fromPool);
				return;
			}
			this.m_currentlyDestroying.Add(fromPool);
		}

		// Token: 0x04002931 RID: 10545
		private const float kSinkDistance = 5f;

		// Token: 0x04002932 RID: 10546
		private const float kMaxSinkTime = 5f;

		// Token: 0x04002933 RID: 10547
		private static VisualDestructionManager m_instance;

		// Token: 0x04002934 RID: 10548
		private readonly List<VisualDestructionManager.DestructionData> m_currentlyDestroying = new List<VisualDestructionManager.DestructionData>(10);

		// Token: 0x02000525 RID: 1317
		private class DestructionData : IPoolable, IEquatable<VisualDestructionManager.DestructionData>
		{
			// Token: 0x1700082D RID: 2093
			// (get) Token: 0x0600277F RID: 10111 RVA: 0x0005BAD2 File Offset: 0x00059CD2
			public float TimeToStart
			{
				get
				{
					return this.m_timeToStart;
				}
			}

			// Token: 0x06002780 RID: 10112 RVA: 0x00138424 File Offset: 0x00136624
			public void Init(GameEntity entity, float delay)
			{
				if (!entity || !entity.gameObject || !entity.gameObject.transform)
				{
					return;
				}
				this.m_instanceId = entity.GetInstanceID();
				this.m_entity = entity;
				this.m_entity.IsDestroying = true;
				this.m_transform = this.m_entity.gameObject.transform;
				this.m_timeToStart = Time.time + delay;
				this.m_timeElapsed = 0f;
				this.m_targetPos = this.m_transform.position;
				this.m_targetPos.y = this.m_targetPos.y - 5f;
			}

			// Token: 0x06002781 RID: 10113 RVA: 0x0005BADA File Offset: 0x00059CDA
			public void Reset()
			{
				this.m_instanceId = -1;
				this.m_entity = null;
				this.m_transform = null;
				this.m_timeToStart = float.MaxValue;
				this.m_timeElapsed = float.MaxValue;
				this.m_targetPos = Vector3.zero;
			}

			// Token: 0x1700082E RID: 2094
			// (get) Token: 0x06002782 RID: 10114 RVA: 0x0005BB12 File Offset: 0x00059D12
			// (set) Token: 0x06002783 RID: 10115 RVA: 0x0005BB1A File Offset: 0x00059D1A
			bool IPoolable.InPool
			{
				get
				{
					return this.m_inPool;
				}
				set
				{
					this.m_inPool = value;
				}
			}

			// Token: 0x06002784 RID: 10116 RVA: 0x0005BB23 File Offset: 0x00059D23
			public bool RemoveFromList()
			{
				return !this.m_entity || !this.m_transform || this.m_timeElapsed > 5f;
			}

			// Token: 0x06002785 RID: 10117 RVA: 0x0005BB4E File Offset: 0x00059D4E
			public void ProgressDestruction()
			{
				this.m_timeElapsed += Time.deltaTime;
				this.m_transform.position = Vector3.Slerp(this.m_transform.position, this.m_targetPos, Time.deltaTime * 0.1f);
			}

			// Token: 0x06002786 RID: 10118 RVA: 0x0005BB8E File Offset: 0x00059D8E
			public void Finished()
			{
				if (this.m_entity && this.m_entity.NetworkEntity)
				{
					this.m_entity.NetworkEntity.DisableObjects();
				}
			}

			// Token: 0x06002787 RID: 10119 RVA: 0x0005BBBF File Offset: 0x00059DBF
			public bool Equals(VisualDestructionManager.DestructionData other)
			{
				return other != null && (this == other || this.m_instanceId == other.m_instanceId);
			}

			// Token: 0x06002788 RID: 10120 RVA: 0x0005BBDA File Offset: 0x00059DDA
			public override bool Equals(object obj)
			{
				return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((VisualDestructionManager.DestructionData)obj)));
			}

			// Token: 0x06002789 RID: 10121 RVA: 0x0005BC08 File Offset: 0x00059E08
			public override int GetHashCode()
			{
				return this.m_instanceId;
			}

			// Token: 0x04002935 RID: 10549
			private bool m_inPool;

			// Token: 0x04002936 RID: 10550
			private int m_instanceId = -1;

			// Token: 0x04002937 RID: 10551
			private GameEntity m_entity;

			// Token: 0x04002938 RID: 10552
			private Transform m_transform;

			// Token: 0x04002939 RID: 10553
			private float m_timeToStart = float.MaxValue;

			// Token: 0x0400293A RID: 10554
			private float m_timeElapsed = float.MaxValue;

			// Token: 0x0400293B RID: 10555
			private Vector3 m_targetPos = Vector3.zero;
		}
	}
}
