using System;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Spawning
{
	// Token: 0x0200066E RID: 1646
	[Serializable]
	public class CallForHelpData
	{
		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x06003314 RID: 13076 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showChance
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x06003315 RID: 13077 RVA: 0x00063298 File Offset: 0x00061498
		private bool m_showMaxHostiles
		{
			get
			{
				return this.m_hostileChance > 0f;
			}
		}

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x06003316 RID: 13078 RVA: 0x000632A7 File Offset: 0x000614A7
		public NpcTagMatch CallToTags
		{
			get
			{
				return this.m_callToTags;
			}
		}

		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x06003317 RID: 13079 RVA: 0x000632AF File Offset: 0x000614AF
		public int MaxAlertCount
		{
			get
			{
				return this.m_maxAlertCount;
			}
		}

		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x06003318 RID: 13080 RVA: 0x000632B7 File Offset: 0x000614B7
		public int MaxHostileCount
		{
			get
			{
				return this.m_maxHostileCount;
			}
		}

		// Token: 0x06003319 RID: 13081 RVA: 0x000632BF File Offset: 0x000614BF
		protected virtual float GetChance(float healthPercent)
		{
			return this.m_chance;
		}

		// Token: 0x0600331A RID: 13082 RVA: 0x00161F70 File Offset: 0x00160170
		public bool CallsForHelp(float healthPercent)
		{
			float chance = this.GetChance(healthPercent);
			return UnityEngine.Random.Range(0f, 1f) < chance;
		}

		// Token: 0x0600331B RID: 13083 RVA: 0x000632C7 File Offset: 0x000614C7
		public float GetRange()
		{
			return this.m_range.RandomWithinRange();
		}

		// Token: 0x0600331C RID: 13084 RVA: 0x000632D4 File Offset: 0x000614D4
		public void EmoteCallForHelp(GameEntity source, float range)
		{
			EmotiveCalls emotiveCalls = this.m_emotiveCalls;
			if (emotiveCalls == null)
			{
				return;
			}
			emotiveCalls.EmoteToNearbyPlayers(source, new float?(range));
		}

		// Token: 0x0600331D RID: 13085 RVA: 0x000632ED File Offset: 0x000614ED
		public void NormalizeProbabilities()
		{
			this.m_emotiveCalls.Normalize();
		}

		// Token: 0x0600331E RID: 13086 RVA: 0x00161F98 File Offset: 0x00160198
		public bool ChanceHostileForEntity(GameEntity entity)
		{
			float time = entity.SqrDistanceFromLastNpcCallForHelp / (this.m_range.Max * this.m_range.Max);
			return this.m_hostileChanceCurve.Evaluate(time) >= UnityEngine.Random.Range(0f, 1f);
		}

		// Token: 0x04003150 RID: 12624
		private const string kCountGroup = "Counts";

		// Token: 0x04003151 RID: 12625
		protected const int kChanceOrder = 0;

		// Token: 0x04003152 RID: 12626
		protected const int kRangeOrder = 10;

		// Token: 0x04003153 RID: 12627
		protected const int kDispositionOrder = 20;

		// Token: 0x04003154 RID: 12628
		protected const int kEmoteOrder = 30;

		// Token: 0x04003155 RID: 12629
		protected const int kCountOrder = 40;

		// Token: 0x04003156 RID: 12630
		protected const float kDefaultChanceValue = 0.2f;

		// Token: 0x04003157 RID: 12631
		[Range(0f, 1f)]
		[SerializeField]
		private float m_chance = 0.2f;

		// Token: 0x04003158 RID: 12632
		[SerializeField]
		private MinMaxFloatRange m_range = new MinMaxFloatRange(5f, 25f);

		// Token: 0x04003159 RID: 12633
		[SerializeField]
		private NpcTagMatch m_callToTags;

		// Token: 0x0400315A RID: 12634
		[FormerlySerializedAs("m_emotiveCallClass")]
		[SerializeField]
		private EmotiveCalls m_emotiveCalls;

		// Token: 0x0400315B RID: 12635
		[Range(1f, 100f)]
		[SerializeField]
		private int m_maxAlertCount = 4;

		// Token: 0x0400315C RID: 12636
		[Range(0f, 1f)]
		[SerializeField]
		private float m_hostileChance = 0.25f;

		// Token: 0x0400315D RID: 12637
		[Tooltip("x=0 is closest to the NPC (distance=0m), while x=1 is the farthest (distance=Range.Max)")]
		[SerializeField]
		private AnimationCurve m_hostileChanceCurve = AnimationCurve.Linear(0f, 0.25f, 0f, 0.75f);

		// Token: 0x0400315E RID: 12638
		[Range(0f, 100f)]
		[SerializeField]
		private int m_maxHostileCount = 1;
	}
}
