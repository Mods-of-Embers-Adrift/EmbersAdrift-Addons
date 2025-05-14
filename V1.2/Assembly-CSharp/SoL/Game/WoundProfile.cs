using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005FF RID: 1535
	[CreateAssetMenu(menuName = "SoL/Profiles/Wound")]
	public class WoundProfile : ScriptableObject, ISerializationCallbackReceiver
	{
		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06003102 RID: 12546 RVA: 0x00061C32 File Offset: 0x0005FE32
		public float OnUnconscious
		{
			get
			{
				return this.m_onUnconscious;
			}
		}

		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06003103 RID: 12547 RVA: 0x00061C3A File Offset: 0x0005FE3A
		public float OnRelease
		{
			get
			{
				return this.m_onRelease;
			}
		}

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06003104 RID: 12548 RVA: 0x00061C42 File Offset: 0x0005FE42
		public float Max
		{
			get
			{
				return this.m_max;
			}
		}

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06003105 RID: 12549 RVA: 0x00061C4A File Offset: 0x0005FE4A
		public float AccumulationRate
		{
			get
			{
				return this.m_accumulationRate;
			}
		}

		// Token: 0x06003106 RID: 12550 RVA: 0x0015C088 File Offset: 0x0015A288
		public bool TryGetWound(HitType hitType, float evalValue, out float wound)
		{
			wound = 0f;
			WoundHitProfile woundHitProfile;
			if (this.m_hitProfileDict.TryGetValue(hitType, out woundHitProfile))
			{
				float num = woundHitProfile.Chance.Evaluate(evalValue);
				if (UnityEngine.Random.Range(0f, 1f) < num)
				{
					wound = woundHitProfile.Wound.RandomWithinRange();
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x00061C52 File Offset: 0x0005FE52
		public float GetWoundForFallDamage(float percentOfHealth)
		{
			return this.m_fallDamageWoundCurve.Evaluate(percentOfHealth);
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x0004475B File Offset: 0x0004295B
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x0015C0DC File Offset: 0x0015A2DC
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_hitProfileDict == null)
			{
				this.m_hitProfileDict = new Dictionary<HitType, WoundHitProfile>(default(HitTypeComparer));
			}
			else
			{
				this.m_hitProfileDict.Clear();
			}
			for (int i = 0; i < this.m_hitProfiles.Length; i++)
			{
				this.m_hitProfileDict.Add(this.m_hitProfiles[i].HitType, this.m_hitProfiles[i]);
			}
		}

		// Token: 0x04002F4E RID: 12110
		[SerializeField]
		private float m_onUnconscious;

		// Token: 0x04002F4F RID: 12111
		[SerializeField]
		private float m_onRelease;

		// Token: 0x04002F50 RID: 12112
		[SerializeField]
		private float m_max = 0.95f;

		// Token: 0x04002F51 RID: 12113
		[SerializeField]
		private float m_accumulationRate;

		// Token: 0x04002F52 RID: 12114
		[SerializeField]
		private AnimationCurve m_fallDamageWoundCurve;

		// Token: 0x04002F53 RID: 12115
		[SerializeField]
		private WoundHitProfile[] m_hitProfiles;

		// Token: 0x04002F54 RID: 12116
		private Dictionary<HitType, WoundHitProfile> m_hitProfileDict;
	}
}
