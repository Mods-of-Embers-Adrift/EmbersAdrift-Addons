using System;
using Cysharp.Text;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C2B RID: 3115
	[Serializable]
	public class TriggeredParams
	{
		// Token: 0x1700170F RID: 5903
		// (get) Token: 0x0600601C RID: 24604 RVA: 0x00080AE0 File Offset: 0x0007ECE0
		public bool TargetSelf
		{
			get
			{
				return this.m_targetSelf;
			}
		}

		// Token: 0x0600601D RID: 24605 RVA: 0x001FBEC8 File Offset: 0x001FA0C8
		public bool ValidTrigger(bool isOnGetHit, DamageCategoriesForTriggerType dmgCategory, EffectApplicationFlags flags)
		{
			bool flag = (isOnGetHit && this.m_triggerType == TriggeredParams.InternalTriggerType.OnGetHit) || (!isOnGetHit && this.m_triggerType == TriggeredParams.InternalTriggerType.OnPerformHit);
			bool flag2 = (dmgCategory & this.m_categories) > DamageCategoriesForTriggerType.None;
			return flag && flag2 && this.m_chance.Trigger(flags);
		}

		// Token: 0x0600601E RID: 24606 RVA: 0x00080AE8 File Offset: 0x0007ECE8
		public string GetTriggerDescription()
		{
			if (this.m_triggerType != TriggeredParams.InternalTriggerType.OnGetHit)
			{
				return "<i>On <b>Perform</b> Hit</i>";
			}
			return "<i>On <b>Get</b> Hit</i>";
		}

		// Token: 0x0600601F RID: 24607 RVA: 0x001FBF10 File Offset: 0x001FA110
		public bool TryGetTriggerConditionDescription(out string conditionDescription)
		{
			conditionDescription = string.Empty;
			bool flag = false;
			TooltipExtensions.ToCombine.Clear();
			string chanceDescription = this.m_chance.GetChanceDescription();
			string hitTypeDescriptionIfNotValid = this.m_chance.GetHitTypeDescriptionIfNotValid();
			if (!string.IsNullOrEmpty(chanceDescription))
			{
				TooltipExtensions.ToCombine.Add(chanceDescription);
				flag = true;
			}
			TooltipExtensions.ToCombine.Add("on");
			if (!string.IsNullOrEmpty(hitTypeDescriptionIfNotValid))
			{
				string text = hitTypeDescriptionIfNotValid.ToLowerInvariant();
				string arg = text.StartsWith("a") ? "an" : "a";
				TooltipExtensions.ToCombine.Add(ZString.Format<string, string>("{0} {1}", arg, text));
				flag = true;
			}
			DamageCategoriesForTriggerType categories = this.m_categories;
			if (categories != DamageCategoriesForTriggerType.None && categories != (DamageCategoriesForTriggerType.Melee | DamageCategoriesForTriggerType.Ranged | DamageCategoriesForTriggerType.Ember | DamageCategoriesForTriggerType.Chemical) && categories != DamageCategoriesForTriggerType.All)
			{
				TooltipExtensions.ToCombine.Add(this.m_categories.ToStringWithSpaces().Replace(", ", "/").ToLowerInvariant());
				flag = true;
			}
			TooltipExtensions.ToCombine.Add("hit");
			if (flag)
			{
				conditionDescription = string.Join(" ", TooltipExtensions.ToCombine);
			}
			return flag;
		}

		// Token: 0x040052DD RID: 21213
		[SerializeField]
		private TriggeredParams.InternalTriggerType m_triggerType;

		// Token: 0x040052DE RID: 21214
		[SerializeField]
		private DamageCategoriesForTriggerType m_categories;

		// Token: 0x040052DF RID: 21215
		[SerializeField]
		private bool m_targetSelf;

		// Token: 0x040052E0 RID: 21216
		[SerializeField]
		private CombatTriggerCondition m_chance;

		// Token: 0x02000C2C RID: 3116
		private enum InternalTriggerType
		{
			// Token: 0x040052E2 RID: 21218
			OnGetHit,
			// Token: 0x040052E3 RID: 21219
			OnPerformHit
		}
	}
}
