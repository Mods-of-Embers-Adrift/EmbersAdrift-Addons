using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BBA RID: 3002
	public abstract class DebugViewBase : MonoBehaviour
	{
		// Token: 0x170015F8 RID: 5624
		// (get) Token: 0x06005D0A RID: 23818 RVA: 0x0007E8F6 File Offset: 0x0007CAF6
		// (set) Token: 0x06005D0B RID: 23819 RVA: 0x0007E8FE File Offset: 0x0007CAFE
		public int CurrentInfluenceCellCount { get; set; }

		// Token: 0x06005D0C RID: 23820 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void CreateDebugData()
		{
		}

		// Token: 0x06005D0D RID: 23821 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void PostInfluenceMapUpdated()
		{
		}

		// Token: 0x06005D0E RID: 23822 RVA: 0x001F2898 File Offset: 0x001F0A98
		private void InfluenceMapUpdated()
		{
			if (!Application.isPlaying || this.m_views == null || this.m_views.Length == 0)
			{
				return;
			}
			SpatialManager spatialManager = this.m_spatialManager ? this.m_spatialManager : ServerGameManager.SpatialManager;
			if (!spatialManager)
			{
				return;
			}
			Vector3 position = base.gameObject.transform.position;
			float y = position.y;
			spatialManager.QueryRadius(position, (float)this.m_queryRadius, this.m_results);
			int num = 2 * this.m_queryRadius;
			this.m_intensitiesDict.Clear();
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)i / (float)num;
				float num3 = (float)(-(float)this.m_queryRadius) + (float)num * num2 + base.gameObject.transform.position.x;
				for (int j = 0; j < num; j++)
				{
					float num4 = (float)j / (float)num;
					float num5 = (float)(-(float)this.m_queryRadius) + (float)num * num4 + position.z;
					Vector3 samplePosition = new Vector3(num3, y, num5);
					Color color = new Color(0f, 0f, 0f, 0f);
					float num6 = 0f;
					for (int k = 0; k < this.m_results.Count; k++)
					{
						int num7 = 0;
						for (int l = 0; l < this.m_views.Length; l++)
						{
							if (this.m_results[k].GameEntity.InfluenceSource != null)
							{
								float influence = this.m_results[k].GameEntity.InfluenceSource.GetInfluence(samplePosition, this.m_views[l].Flags);
								if (influence > 0f)
								{
									num6 += influence;
									color += this.m_views[l].Color * this.m_views[l].Weight * influence;
									num7++;
								}
							}
						}
						if (num7 > 0)
						{
							color /= (float)num7;
						}
					}
					if (num6 > 0f)
					{
						int num8 = (int)num3;
						int num9 = (int)num5;
						int key = num8 + num9 * num;
						DebugViewBase.CellInfluence cellInfluence;
						if (this.m_intensitiesDict.TryGetValue(key, out cellInfluence))
						{
							cellInfluence.value += num6;
							cellInfluence.color = (cellInfluence.color + color) / 2f;
							this.m_intensitiesDict[key] = cellInfluence;
						}
						else
						{
							this.m_intensitiesDict.Add(key, new DebugViewBase.CellInfluence
							{
								x = num8,
								y = num9,
								value = num6,
								color = color
							});
						}
					}
				}
			}
			this.CurrentInfluenceCellCount = this.m_intensitiesDict.Count;
			this.PostInfluenceMapUpdated();
		}

		// Token: 0x04005069 RID: 20585
		[SerializeField]
		protected float m_quadSize = 0.75f;

		// Token: 0x0400506A RID: 20586
		[SerializeField]
		protected int m_queryRadius = 128;

		// Token: 0x0400506B RID: 20587
		[SerializeField]
		private DebugViewBase.DebugView[] m_views;

		// Token: 0x0400506C RID: 20588
		[SerializeField]
		private SpatialManager m_spatialManager;

		// Token: 0x0400506E RID: 20590
		private readonly List<NetworkEntity> m_results = new List<NetworkEntity>();

		// Token: 0x0400506F RID: 20591
		protected readonly Dictionary<int, DebugViewBase.CellInfluence> m_intensitiesDict = new Dictionary<int, DebugViewBase.CellInfluence>();

		// Token: 0x04005070 RID: 20592
		private DateTime m_timeOfNextUpdate = DateTime.MinValue;

		// Token: 0x04005071 RID: 20593
		[SerializeField]
		private float m_updateRate = 1f;

		// Token: 0x02000BBB RID: 3003
		[Serializable]
		protected class DebugView
		{
			// Token: 0x04005072 RID: 20594
			public InfluenceFlags Flags = InfluenceFlags.All;

			// Token: 0x04005073 RID: 20595
			public Color Color = Color.blue;

			// Token: 0x04005074 RID: 20596
			public float Weight = 1f;
		}

		// Token: 0x02000BBC RID: 3004
		protected struct CellInfluence : IEquatable<DebugViewBase.CellInfluence>
		{
			// Token: 0x06005D11 RID: 23825 RVA: 0x0007E92C File Offset: 0x0007CB2C
			public bool Equals(DebugViewBase.CellInfluence other)
			{
				return this.x == other.x && this.y == other.y;
			}

			// Token: 0x06005D12 RID: 23826 RVA: 0x0007E94C File Offset: 0x0007CB4C
			public override bool Equals(object obj)
			{
				return obj != null && obj is DebugViewBase.CellInfluence && this.Equals((DebugViewBase.CellInfluence)obj);
			}

			// Token: 0x06005D13 RID: 23827 RVA: 0x0007E969 File Offset: 0x0007CB69
			public override int GetHashCode()
			{
				return this.x * 397 ^ this.y;
			}

			// Token: 0x04005075 RID: 20597
			public int x;

			// Token: 0x04005076 RID: 20598
			public int y;

			// Token: 0x04005077 RID: 20599
			public float value;

			// Token: 0x04005078 RID: 20600
			public Color color;
		}
	}
}
