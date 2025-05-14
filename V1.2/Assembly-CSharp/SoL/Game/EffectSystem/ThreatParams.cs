using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C29 RID: 3113
	[Serializable]
	public class ThreatParams
	{
		// Token: 0x1700170C RID: 5900
		// (get) Token: 0x06006015 RID: 24597 RVA: 0x00080A80 File Offset: 0x0007EC80
		public float Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x1700170D RID: 5901
		// (get) Token: 0x06006016 RID: 24598 RVA: 0x00080A88 File Offset: 0x0007EC88
		public bool Aoe
		{
			get
			{
				return this.m_aoe;
			}
		}

		// Token: 0x1700170E RID: 5902
		// (get) Token: 0x06006017 RID: 24599 RVA: 0x00080A90 File Offset: 0x0007EC90
		public float Radius
		{
			get
			{
				return this.m_radius;
			}
		}

		// Token: 0x06006018 RID: 24600 RVA: 0x00080A98 File Offset: 0x0007EC98
		private void ValidateValues()
		{
			this.m_radius = Mathf.Clamp(this.m_radius, 0f, float.MaxValue);
		}

		// Token: 0x06006019 RID: 24601 RVA: 0x00080AB5 File Offset: 0x0007ECB5
		public bool ShouldApply(EffectApplicationFlags flags)
		{
			return this.m_additionalThreat && this.m_condition.Trigger(flags);
		}

		// Token: 0x0600601A RID: 24602 RVA: 0x001FBE98 File Offset: 0x001FA098
		public void FillEffectData(ArchetypeTooltip tooltip)
		{
			if (!this.m_additionalThreat)
			{
				return;
			}
			string text = Mathf.FloorToInt(this.m_value).ToString() + " additional threat";
			string triggerDescription = this.m_condition.GetTriggerDescription();
			if (!string.IsNullOrEmpty(triggerDescription))
			{
				text = text + " if " + triggerDescription;
			}
			if (this.m_aoe)
			{
				text = text + " to all within " + Mathf.FloorToInt(this.m_radius).ToString() + "m";
			}
			tooltip.EffectsBlock.AppendLine(text, 0);
		}

		// Token: 0x040052CF RID: 21199
		private const string kThreatGroupName = "Threat";

		// Token: 0x040052D0 RID: 21200
		[SerializeField]
		private bool m_additionalThreat;

		// Token: 0x040052D1 RID: 21201
		[SerializeField]
		private CombatTriggerCondition m_condition;

		// Token: 0x040052D2 RID: 21202
		[SerializeField]
		private float m_value;

		// Token: 0x040052D3 RID: 21203
		[SerializeField]
		private bool m_aoe;

		// Token: 0x040052D4 RID: 21204
		[SerializeField]
		private float m_radius = 5f;
	}
}
