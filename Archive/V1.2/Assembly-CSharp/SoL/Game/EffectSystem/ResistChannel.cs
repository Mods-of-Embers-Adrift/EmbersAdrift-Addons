using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C2D RID: 3117
	[Serializable]
	public class ResistChannel
	{
		// Token: 0x17001710 RID: 5904
		// (get) Token: 0x06006021 RID: 24609 RVA: 0x00080AFD File Offset: 0x0007ECFD
		private StatusEffectSubType[] m_validSubTypes
		{
			get
			{
				return this.m_channel.GetValidSubTypes();
			}
		}

		// Token: 0x06006022 RID: 24610 RVA: 0x00080B0A File Offset: 0x0007ED0A
		private void ValidateSubType()
		{
			this.m_subChannel = this.m_channel.ValidateSubType(this.m_subChannel);
		}

		// Token: 0x17001711 RID: 5905
		// (get) Token: 0x06006023 RID: 24611 RVA: 0x00080B23 File Offset: 0x0007ED23
		private bool m_hasSubChannel
		{
			get
			{
				return this.m_channel.HasSubTypes();
			}
		}

		// Token: 0x17001712 RID: 5906
		// (get) Token: 0x06006024 RID: 24612 RVA: 0x00080B30 File Offset: 0x0007ED30
		private bool m_hasDmgChannel
		{
			get
			{
				return this.m_hasSubChannel && this.m_channel.HasDamageTypes();
			}
		}

		// Token: 0x17001713 RID: 5907
		// (get) Token: 0x06006025 RID: 24613 RVA: 0x00080B47 File Offset: 0x0007ED47
		private bool m_showDmgChannel
		{
			get
			{
				return this.m_hasDmgChannel && this.m_useDamageChannel;
			}
		}

		// Token: 0x06006026 RID: 24614 RVA: 0x001FC07C File Offset: 0x001FA27C
		public int GetResistValue(EffectValueContainer container)
		{
			if (container == null)
			{
				return 0;
			}
			if (!this.m_channel.HasSubTypes())
			{
				return container.GetValue();
			}
			if (!this.m_channel.HasDamageTypes())
			{
				return container.GetValue(this.m_subChannel);
			}
			return container.GetValue(this.m_subChannel, this.m_damageType);
		}

		// Token: 0x17001714 RID: 5908
		// (get) Token: 0x06006027 RID: 24615 RVA: 0x00080B59 File Offset: 0x0007ED59
		public StatusEffectType PrimaryType
		{
			get
			{
				return this.m_channel;
			}
		}

		// Token: 0x040052E4 RID: 21220
		[SerializeField]
		private StatusEffectType m_channel;

		// Token: 0x040052E5 RID: 21221
		[SerializeField]
		private StatusEffectSubType m_subChannel;

		// Token: 0x040052E6 RID: 21222
		[SerializeField]
		private bool m_useDamageChannel;

		// Token: 0x040052E7 RID: 21223
		[SerializeField]
		private DamageType m_damageType;
	}
}
