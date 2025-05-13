using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008CD RID: 2253
	public class PvpFlagTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060041DC RID: 16860 RVA: 0x00190B30 File Offset: 0x0018ED30
		private ITooltipParameter GetParameter()
		{
			string txt = "Player vs. Player.";
			if (GlobalSettings.Values && GlobalSettings.Values.Combat != null)
			{
				List<string> fromPool = StaticListPool<string>.GetFromPool();
				fromPool.Add(ZString.Format<string>("{0}", "Player vs. Player."));
				int num = GlobalSettings.Values.Combat.PvpResistControl;
				if (num > 0)
				{
					fromPool.Add(ZString.Format<int>("  +{0} Stun, Daze, and Movement effects.", num));
				}
				num = GlobalSettings.Values.Combat.PvpResistDamage;
				if (num > 0)
				{
					fromPool.Add(ZString.Format<int>("  +{0} Physical, Mental, Chemical, and Ember damage.", num));
				}
				num = GlobalSettings.Values.Combat.PvpResistDebuff;
				if (num > 0)
				{
					fromPool.Add(ZString.Format<int>("  +{0} Physical, Mental, Chemical, and Ember debuffs.", num));
				}
				if (fromPool.Count > 1)
				{
					fromPool.Insert(1, "Resists against other players:");
					fromPool.Insert(1, " ");
				}
				txt = string.Join("\n", fromPool);
			}
			return new ObjectTextTooltipParameter(this, txt, this.m_isOptionsMenu);
		}

		// Token: 0x17000F03 RID: 3843
		// (get) Token: 0x060041DD RID: 16861 RVA: 0x0006C845 File Offset: 0x0006AA45
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000F04 RID: 3844
		// (get) Token: 0x060041DE RID: 16862 RVA: 0x0006C853 File Offset: 0x0006AA53
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000F05 RID: 3845
		// (get) Token: 0x060041DF RID: 16863 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060041E1 RID: 16865 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F12 RID: 16146
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04003F13 RID: 16147
		[SerializeField]
		private bool m_isOptionsMenu;

		// Token: 0x04003F14 RID: 16148
		private const string kPvp = "Player vs. Player.";
	}
}
