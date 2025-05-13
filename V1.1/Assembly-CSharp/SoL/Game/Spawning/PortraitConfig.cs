using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000688 RID: 1672
	[Serializable]
	public class PortraitConfig
	{
		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x060033A3 RID: 13219 RVA: 0x000637E6 File Offset: 0x000619E6
		private bool m_showPortraits
		{
			get
			{
				return this.m_portraitOverride == null;
			}
		}

		// Token: 0x060033A4 RID: 13220 RVA: 0x001626D8 File Offset: 0x001608D8
		public UniqueId GetPortraitId()
		{
			UniqueId result = UniqueId.Empty;
			if (this.m_portraitOverride != null)
			{
				result = this.m_portraitOverride.Id;
			}
			else
			{
				IdentifiableSpriteProbabilityCollection portraits = this.m_portraits;
				IdentifiableSpritePropabilityEntry identifiableSpritePropabilityEntry = (portraits != null) ? portraits.GetEntry(null, false) : null;
				if (identifiableSpritePropabilityEntry != null && !identifiableSpritePropabilityEntry.Obj.Id.IsEmpty)
				{
					result = identifiableSpritePropabilityEntry.Obj.Id;
				}
			}
			return result;
		}

		// Token: 0x060033A5 RID: 13221 RVA: 0x000637F4 File Offset: 0x000619F4
		private IEnumerable GetMaleFemaleSpriteCollection()
		{
			return SolOdinUtilities.GetDropdownItems<MaleFemaleSpriteCollection>();
		}

		// Token: 0x060033A6 RID: 13222 RVA: 0x000637FB File Offset: 0x000619FB
		public void Normalize()
		{
			IdentifiableSpriteProbabilityCollection portraits = this.m_portraits;
			if (portraits == null)
			{
				return;
			}
			portraits.Normalize();
		}

		// Token: 0x040031A2 RID: 12706
		[SerializeField]
		private MaleFemaleSpriteCollection m_portraitOverride;

		// Token: 0x040031A3 RID: 12707
		[SerializeField]
		private IdentifiableSpriteProbabilityCollection m_portraits;
	}
}
