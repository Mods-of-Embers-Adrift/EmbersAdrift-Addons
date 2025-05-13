using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CAD RID: 3245
	public class MapDiscoveryTeleportTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06006264 RID: 25188 RVA: 0x000823EF File Offset: 0x000805EF
		internal void Initialize(MapTeleportProfile profile)
		{
			this.m_profile = profile;
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x0020493C File Offset: 0x00202B3C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_profile && !this.m_button.interactable)
			{
				string txt = ZString.Format<int>("Requires {0} Ember Essence to teleport!", this.m_profile.EssenceCost);
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17001781 RID: 6017
		// (get) Token: 0x06006266 RID: 25190 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001782 RID: 6018
		// (get) Token: 0x06006267 RID: 25191 RVA: 0x000823F8 File Offset: 0x000805F8
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001783 RID: 6019
		// (get) Token: 0x06006268 RID: 25192 RVA: 0x00082406 File Offset: 0x00080606
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040055E7 RID: 21991
		[SerializeField]
		private SolButton m_button;

		// Token: 0x040055E8 RID: 21992
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x040055E9 RID: 21993
		private MapTeleportProfile m_profile;
	}
}
