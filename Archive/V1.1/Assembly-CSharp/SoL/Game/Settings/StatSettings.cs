using System;
using SoL.Game.EffectSystem;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000743 RID: 1859
	[Serializable]
	public class StatSettings
	{
		// Token: 0x0600377C RID: 14204 RVA: 0x00049FFA File Offset: 0x000481FA
		public StatSettings.ClampSettings GetSettingForStat(StatType type)
		{
			return null;
		}

		// Token: 0x0600377D RID: 14205 RVA: 0x0016BE8C File Offset: 0x0016A08C
		private void Init()
		{
			if (!this.m_initialized || this.m_indexedMaxValues == null)
			{
				StatType[] array = (StatType[])Enum.GetValues(typeof(StatType));
				int num = int.MinValue;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] > (StatType)num)
					{
						num = (int)array[i];
					}
				}
				this.m_indexedMaxValues = new StatSettings.ClampSettings[num + 1];
				for (int j = 0; j < this.m_maxValues.Length; j++)
				{
					this.m_indexedMaxValues[(int)this.m_maxValues[j].Type] = this.m_maxValues[j];
				}
				this.m_initialized = true;
			}
		}

		// Token: 0x0600377E RID: 14206 RVA: 0x00065F2C File Offset: 0x0006412C
		private void Reinitialize()
		{
			this.m_indexedMaxValues = null;
			this.m_initialized = false;
			this.Init();
		}

		// Token: 0x17000C7E RID: 3198
		// (get) Token: 0x0600377F RID: 14207 RVA: 0x00065F42 File Offset: 0x00064142
		public StatSettings.DiminishingCurveCollection DiminishingCurves
		{
			get
			{
				return this.m_diminishingCurves;
			}
		}

		// Token: 0x04003662 RID: 13922
		[SerializeField]
		private StatSettings.ClampSettings[] m_maxValues;

		// Token: 0x04003663 RID: 13923
		[NonSerialized]
		private StatSettings.ClampSettings[] m_indexedMaxValues;

		// Token: 0x04003664 RID: 13924
		[NonSerialized]
		private bool m_initialized;

		// Token: 0x04003665 RID: 13925
		[SerializeField]
		private StatSettings.DiminishingCurveCollection m_diminishingCurves;

		// Token: 0x02000744 RID: 1860
		[Serializable]
		public class ClampSettings
		{
			// Token: 0x17000C7F RID: 3199
			// (get) Token: 0x06003781 RID: 14209 RVA: 0x00065F4A File Offset: 0x0006414A
			public StatType Type
			{
				get
				{
					return this.m_stat;
				}
			}

			// Token: 0x17000C80 RID: 3200
			// (get) Token: 0x06003782 RID: 14210 RVA: 0x00065F52 File Offset: 0x00064152
			public int MaxEquipped
			{
				get
				{
					return this.m_maxEquipped;
				}
			}

			// Token: 0x17000C81 RID: 3201
			// (get) Token: 0x06003783 RID: 14211 RVA: 0x00065F5A File Offset: 0x0006415A
			public int MaxTotal
			{
				get
				{
					return this.m_maxTotal;
				}
			}

			// Token: 0x04003666 RID: 13926
			[SerializeField]
			private StatType m_stat;

			// Token: 0x04003667 RID: 13927
			[SerializeField]
			private int m_maxEquipped = 1000;

			// Token: 0x04003668 RID: 13928
			[SerializeField]
			private int m_maxTotal = 1000;
		}

		// Token: 0x02000745 RID: 1861
		[Serializable]
		public class DiminishingCurve
		{
			// Token: 0x17000C82 RID: 3202
			// (get) Token: 0x06003785 RID: 14213 RVA: 0x00065F80 File Offset: 0x00064180
			private bool m_hideCurve
			{
				get
				{
					return this.m_curveOverride != null;
				}
			}

			// Token: 0x06003786 RID: 14214 RVA: 0x00065F8E File Offset: 0x0006418E
			public int GetDiminishedValue(int incoming)
			{
				if (this.m_enabled)
				{
					return Mathf.FloorToInt(this.m_curveOverride ? this.m_curveOverride.Evaluate((float)incoming) : this.m_curve.Evaluate((float)incoming));
				}
				return incoming;
			}

			// Token: 0x04003669 RID: 13929
			[SerializeField]
			private bool m_enabled;

			// Token: 0x0400366A RID: 13930
			[SerializeField]
			private ScriptableCurve m_curveOverride;

			// Token: 0x0400366B RID: 13931
			[Tooltip("x-axis is incoming value, y-axis is outgoing value")]
			[SerializeField]
			private AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 100f, 100f);
		}

		// Token: 0x02000746 RID: 1862
		[Serializable]
		public class DiminishingCurveCollection
		{
			// Token: 0x17000C83 RID: 3203
			// (get) Token: 0x06003788 RID: 14216 RVA: 0x00065FEF File Offset: 0x000641EF
			public StatSettings.DiminishingCurve DamageResists
			{
				get
				{
					return this.m_damageResists;
				}
			}

			// Token: 0x17000C84 RID: 3204
			// (get) Token: 0x06003789 RID: 14217 RVA: 0x00065FF7 File Offset: 0x000641F7
			public StatSettings.DiminishingCurve LastingResists
			{
				get
				{
					return this.m_lastingResists;
				}
			}

			// Token: 0x17000C85 RID: 3205
			// (get) Token: 0x0600378A RID: 14218 RVA: 0x00065FFF File Offset: 0x000641FF
			public StatSettings.DiminishingCurve Avoid
			{
				get
				{
					return this.m_avoid;
				}
			}

			// Token: 0x17000C86 RID: 3206
			// (get) Token: 0x0600378B RID: 14219 RVA: 0x00066007 File Offset: 0x00064207
			public StatSettings.DiminishingCurve Resilience
			{
				get
				{
					return this.m_resilience;
				}
			}

			// Token: 0x17000C87 RID: 3207
			// (get) Token: 0x0600378C RID: 14220 RVA: 0x0006600F File Offset: 0x0006420F
			public StatSettings.DiminishingCurve OutgoingDamage
			{
				get
				{
					return this.m_outgoingDamage;
				}
			}

			// Token: 0x17000C88 RID: 3208
			// (get) Token: 0x0600378D RID: 14221 RVA: 0x00066017 File Offset: 0x00064217
			public StatSettings.DiminishingCurve Penetration
			{
				get
				{
					return this.m_penetration;
				}
			}

			// Token: 0x17000C89 RID: 3209
			// (get) Token: 0x0600378E RID: 14222 RVA: 0x0006601F File Offset: 0x0006421F
			public StatSettings.DiminishingCurve Hit
			{
				get
				{
					return this.m_hit;
				}
			}

			// Token: 0x0400366C RID: 13932
			private const string kDefensive = "Defensive";

			// Token: 0x0400366D RID: 13933
			private const string kOffensive = "Offensive";

			// Token: 0x0400366E RID: 13934
			[SerializeField]
			private StatSettings.DiminishingCurve m_damageResists;

			// Token: 0x0400366F RID: 13935
			[SerializeField]
			private StatSettings.DiminishingCurve m_lastingResists;

			// Token: 0x04003670 RID: 13936
			[SerializeField]
			private StatSettings.DiminishingCurve m_avoid;

			// Token: 0x04003671 RID: 13937
			[SerializeField]
			private StatSettings.DiminishingCurve m_resilience;

			// Token: 0x04003672 RID: 13938
			[SerializeField]
			private StatSettings.DiminishingCurve m_outgoingDamage;

			// Token: 0x04003673 RID: 13939
			[SerializeField]
			private StatSettings.DiminishingCurve m_penetration;

			// Token: 0x04003674 RID: 13940
			[SerializeField]
			private StatSettings.DiminishingCurve m_hit;
		}
	}
}
