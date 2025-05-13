using System;
using SoL.Game.Messages;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AD8 RID: 2776
	public static class MasteryTypeExtensions
	{
		// Token: 0x170013D0 RID: 5072
		// (get) Token: 0x0600559D RID: 21917 RVA: 0x000791EA File Offset: 0x000773EA
		public static MasteryType[] MasteryTypes
		{
			get
			{
				if (MasteryTypeExtensions.m_masteryTypes == null)
				{
					MasteryTypeExtensions.m_masteryTypes = (MasteryType[])Enum.GetValues(typeof(MasteryType));
				}
				return MasteryTypeExtensions.m_masteryTypes;
			}
		}

		// Token: 0x0600559E RID: 21918 RVA: 0x00079211 File Offset: 0x00077411
		public static int GetMaximumForType(this MasteryType type)
		{
			switch (type)
			{
			case MasteryType.Combat:
			case MasteryType.Utility:
			case MasteryType.Trade:
			case MasteryType.Harvesting:
				return 5;
			case MasteryType.Supplemental:
				return 2;
			default:
				throw new ArgumentException("type");
			}
		}

		// Token: 0x0600559F RID: 21919 RVA: 0x0007923F File Offset: 0x0007743F
		public static int GetMaximumForSphere(this MasterySphere sphere)
		{
			if (sphere == MasterySphere.Adventuring)
			{
				return 1;
			}
			if (sphere != MasterySphere.Crafting)
			{
				throw new ArgumentException("sphere");
			}
			return 3;
		}

		// Token: 0x060055A0 RID: 21920 RVA: 0x00045BCA File Offset: 0x00043DCA
		public static bool ContributeStats(this MasteryType type)
		{
			return false;
		}

		// Token: 0x060055A1 RID: 21921 RVA: 0x00079258 File Offset: 0x00077458
		public static bool RequiresPool(this MasteryType type)
		{
			return type - MasteryType.Supplemental > 2;
		}

		// Token: 0x060055A2 RID: 21922 RVA: 0x00079263 File Offset: 0x00077463
		public static MasterySphere GetMasterySphere(this MasteryType type)
		{
			if (type - MasteryType.Combat <= 1)
			{
				return MasterySphere.Adventuring;
			}
			if (type - MasteryType.Trade > 1)
			{
				throw new ArgumentException("type");
			}
			return MasterySphere.Crafting;
		}

		// Token: 0x060055A3 RID: 21923 RVA: 0x001DF35C File Offset: 0x001DD55C
		public static Color GetMasteryColor(this MasteryType type)
		{
			switch (type)
			{
			case MasteryType.Combat:
				return Colors.Crimson;
			case MasteryType.Utility:
				return Colors.CornflowerBlue;
			case MasteryType.Trade:
				return Colors.AztecGold;
			case MasteryType.Harvesting:
				return Colors.RoseGold;
			}
			return new Color(0.4627451f, 0.4745098f, 0.4862745f);
		}

		// Token: 0x060055A4 RID: 21924 RVA: 0x00079281 File Offset: 0x00077481
		public static MessageEventType GetMessageEventType(this MasteryType masteryType)
		{
			if (masteryType == MasteryType.Combat)
			{
				return MessageEventType.LevelUpAdventuring;
			}
			if (masteryType - MasteryType.Trade > 1)
			{
				return MessageEventType.None;
			}
			return MessageEventType.LevelUpGatheringCrafting;
		}

		// Token: 0x04004C01 RID: 19457
		private static MasteryType[] m_masteryTypes;
	}
}
