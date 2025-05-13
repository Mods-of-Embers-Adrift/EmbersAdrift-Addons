using System;
using Cysharp.Text;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA9 RID: 3241
	public class LeyLinkProfile : DiscoveryProfile
	{
		// Token: 0x17001773 RID: 6003
		// (get) Token: 0x06006239 RID: 25145 RVA: 0x000822A7 File Offset: 0x000804A7
		public int LevelRequirement
		{
			get
			{
				return this.m_levelRequirement;
			}
		}

		// Token: 0x17001774 RID: 6004
		// (get) Token: 0x0600623A RID: 25146 RVA: 0x000822AF File Offset: 0x000804AF
		public int EssenceCost
		{
			get
			{
				return GlobalSettings.Values.Ashen.GroupFromMonolithEssenceCost;
			}
		}

		// Token: 0x0600623B RID: 25147 RVA: 0x00203D00 File Offset: 0x00201F00
		public string GetRequiredLevelTooltipText()
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.CharacterData)
			{
				return this.LevelRequirement.ToString();
			}
			return this.GetRequiredLevelTooltipText(LocalPlayer.GameEntity.CharacterData.AdventuringLevel >= this.LevelRequirement);
		}

		// Token: 0x0600623C RID: 25148 RVA: 0x00203D5C File Offset: 0x00201F5C
		public string GetRequiredLevelTooltipText(bool meetsLevelRequirements)
		{
			Color color = meetsLevelRequirements ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
			return ZString.Format<string, int>("<color={0}>{1}</color>", color.ToHex(), this.LevelRequirement);
		}

		// Token: 0x040055C6 RID: 21958
		public const string kDescription = "Ley Link";

		// Token: 0x040055C7 RID: 21959
		[Range(1f, 50f)]
		[SerializeField]
		private int m_levelRequirement = 1;
	}
}
