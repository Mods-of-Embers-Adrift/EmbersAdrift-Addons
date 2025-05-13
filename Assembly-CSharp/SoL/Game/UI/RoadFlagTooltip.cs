using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008D0 RID: 2256
	public class RoadFlagTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060041FF RID: 16895 RVA: 0x001916B8 File Offset: 0x0018F8B8
		private ITooltipParameter GetParameter()
		{
			string asPercentage = GlobalSettings.Values.Player.OnRoadSpeedMod.GetAsPercentage();
			string txt = ZString.Format<string>(RoadFlagTooltip.kRoadFlagText, asPercentage);
			return new ObjectTextTooltipParameter(this, txt, this.m_isOptionsMenu);
		}

		// Token: 0x17000F07 RID: 3847
		// (get) Token: 0x06004200 RID: 16896 RVA: 0x0006C929 File Offset: 0x0006AB29
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000F08 RID: 3848
		// (get) Token: 0x06004201 RID: 16897 RVA: 0x0006C937 File Offset: 0x0006AB37
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000F09 RID: 3849
		// (get) Token: 0x06004202 RID: 16898 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F25 RID: 16165
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04003F26 RID: 16166
		[SerializeField]
		private bool m_isOptionsMenu;

		// Token: 0x04003F27 RID: 16167
		private static string kRoadFlagText = "On a well traveled road.\n+{0} " + StatType.Movement.ToString();
	}
}
