using System;
using SoL.Game.EffectSystem;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A7D RID: 2685
	public static class HandheldFlagConfigExtensions
	{
		// Token: 0x0600530F RID: 21263 RVA: 0x001D6E4C File Offset: 0x001D504C
		public static HandheldFlagConfig GetHandheldFlagConfig(this GameEntity entity)
		{
			HandheldItemFlags mainHand = HandheldItemFlags.Empty;
			HandheldItemFlags offHand = HandheldItemFlags.Empty;
			bool alternateAnimationSet = false;
			ArchetypeInstance archetypeInstance;
			IHandheldItem handheldItem;
			if (entity.TryGetHandheldItem_MainHandAsType(out archetypeInstance, out handheldItem))
			{
				mainHand = handheldItem.HandheldItemFlag;
				alternateAnimationSet = handheldItem.AlternateAnimationSet;
			}
			IHandheldItem handheldItem2;
			if (entity.TryGetHandheldItem_OffHandAsType(out archetypeInstance, out handheldItem2))
			{
				offHand = handheldItem2.HandheldItemFlag;
			}
			return new HandheldFlagConfig
			{
				MainHand = mainHand,
				OffHand = offHand,
				AlternateAnimationSet = alternateAnimationSet
			};
		}

		// Token: 0x06005310 RID: 21264 RVA: 0x001D6EB4 File Offset: 0x001D50B4
		public static HandheldFlagConfig GetHandheldFlagConfig(this IHandHeldItems handheldItems)
		{
			HandheldItemFlags mainHand = HandheldItemFlags.Empty;
			HandheldItemFlags offHand = HandheldItemFlags.Empty;
			if (handheldItems.MainHand != null && handheldItems.MainHand.HandHeldItem != null)
			{
				mainHand = handheldItems.MainHand.HandHeldItem.HandheldItemFlag;
			}
			if (handheldItems.OffHand != null && handheldItems.OffHand.HandHeldItem != null)
			{
				offHand = handheldItems.OffHand.HandHeldItem.HandheldItemFlag;
			}
			return new HandheldFlagConfig
			{
				MainHand = mainHand,
				OffHand = offHand
			};
		}
	}
}
