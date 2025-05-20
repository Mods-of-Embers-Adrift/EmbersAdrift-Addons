using System;
using Cysharp.Text;

namespace SoL.Game.UI
{
	// Token: 0x02000888 RID: 2184
	public class GroupWindowGelUI : GroupWindowIndicatorUI
	{
		// Token: 0x17000EB5 RID: 3765
		// (get) Token: 0x06003F96 RID: 16278 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool m_alwaysShowTooltip
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x0006B009 File Offset: 0x00069209
		private bool IsPlayerLevelSameAsGEL()
		{
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.AdventuringLevel == base.Value;
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x0006B041 File Offset: 0x00069241
		protected override string GetLabelText()
		{
			if (base.Value > 0 && !this.IsPlayerLevelSameAsGEL())
			{
				return "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
			}
			return "<color=#696264><font=\"Font Awesome 5 Free-Solid-900 SDF\"></font></color>";
		}

		// Token: 0x06003F99 RID: 16281 RVA: 0x001892A8 File Offset: 0x001874A8
		protected override string GetTooltipText()
		{
			if (base.Value > 0 && !this.IsPlayerLevelSameAsGEL())
			{
				return ZString.Format<string, string, int>("{0} {1} <color=green>ACTIVE</color>\nYour effective combat level has been increased to level {2} thanks to a nearby group member.", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>", "G.E.L. (Group Elevated Level)", base.Value);
			}
			return ZString.Format<string, string>("{0} {1} INACTIVE\nGEL increases your effective combat level based on nearby group members", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>", "G.E.L. (Group Elevated Level)");
		}

		// Token: 0x04003D54 RID: 15700
		private const string kGelDescriptor = "G.E.L. (Group Elevated Level)";

		// Token: 0x04003D55 RID: 15701
		private const string kInactiveGelIcon = "<color=#696264><font=\"Font Awesome 5 Free-Solid-900 SDF\"></font></color>";
	}
}
