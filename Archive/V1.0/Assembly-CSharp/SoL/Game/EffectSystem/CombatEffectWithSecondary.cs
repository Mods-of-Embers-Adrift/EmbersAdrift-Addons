using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C04 RID: 3076
	[Serializable]
	public class CombatEffectWithSecondary : CombatEffect
	{
		// Token: 0x06005EC8 RID: 24264 RVA: 0x0007FC1D File Offset: 0x0007DE1D
		public override bool TryGetSecondary(EffectApplicationFlags flags, out TargetingParams targeting, out CombatEffect secondaryEffect)
		{
			targeting = null;
			secondaryEffect = null;
			if (this.m_hasSecondary && this.m_condition.Trigger(flags))
			{
				targeting = this.m_secondaryEffect.Targeting;
				secondaryEffect = this.m_secondaryEffect.Effect;
				return true;
			}
			return false;
		}

		// Token: 0x1700167A RID: 5754
		// (get) Token: 0x06005EC9 RID: 24265 RVA: 0x0007FC58 File Offset: 0x0007DE58
		public override bool HasSecondary
		{
			get
			{
				return this.m_hasSecondary;
			}
		}

		// Token: 0x1700167B RID: 5755
		// (get) Token: 0x06005ECA RID: 24266 RVA: 0x0007FC60 File Offset: 0x0007DE60
		public override CombatEffect SecondaryCombatEffect
		{
			get
			{
				SecondaryEffect secondaryEffect = this.m_secondaryEffect;
				if (secondaryEffect == null)
				{
					return null;
				}
				return secondaryEffect.Effect;
			}
		}

		// Token: 0x1700167C RID: 5756
		// (get) Token: 0x06005ECB RID: 24267 RVA: 0x0007FC73 File Offset: 0x0007DE73
		public CombatTriggerCondition SecondaryTriggerCondition
		{
			get
			{
				return this.m_condition;
			}
		}

		// Token: 0x1700167D RID: 5757
		// (get) Token: 0x06005ECC RID: 24268 RVA: 0x0007FC7B File Offset: 0x0007DE7B
		public override TargetingParams SecondaryTargetingParams
		{
			get
			{
				SecondaryEffect secondaryEffect = this.m_secondaryEffect;
				if (secondaryEffect == null)
				{
					return null;
				}
				return secondaryEffect.Targeting;
			}
		}

		// Token: 0x040051EC RID: 20972
		private const string kSecondaryGroupName = "Secondary";

		// Token: 0x040051ED RID: 20973
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x040051EE RID: 20974
		[SerializeField]
		private bool m_hasSecondary;

		// Token: 0x040051EF RID: 20975
		[SerializeField]
		private CombatTriggerCondition m_condition;

		// Token: 0x040051F0 RID: 20976
		[SerializeField]
		private SecondaryEffect m_secondaryEffect;
	}
}
