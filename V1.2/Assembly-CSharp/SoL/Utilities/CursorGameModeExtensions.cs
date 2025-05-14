using System;
using SoL.Game.Audio;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;

namespace SoL.Utilities
{
	// Token: 0x02000263 RID: 611
	public static class CursorGameModeExtensions
	{
		// Token: 0x06001372 RID: 4978 RVA: 0x0004FB48 File Offset: 0x0004DD48
		public static CursorType GetCursorTypeForMode(this CursorGameMode mode)
		{
			switch (mode)
			{
			case CursorGameMode.Sell:
				return CursorType.MerchantCursor;
			case CursorGameMode.Repair:
				return CursorType.AnvilCursor;
			case CursorGameMode.Deconstruct:
				return CursorType.HammerCursor;
			case CursorGameMode.UtilityItem:
				return UtilityItemExtensions.GetCursorForUtilityItem();
			default:
				return CursorType.MainCursor;
			}
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x000F6E34 File Offset: 0x000F5034
		public static void PlayAudioForMode(this CursorGameMode mode)
		{
			AudioClipCollection audioClipCollection = null;
			if (mode != CursorGameMode.Sell)
			{
				if (mode - CursorGameMode.Repair <= 1)
				{
					audioClipCollection = GlobalSettings.Values.Audio.RepairModeClipCollection;
				}
			}
			else
			{
				audioClipCollection = GlobalSettings.Values.Audio.MoneyClipCollection;
			}
			if (audioClipCollection != null)
			{
				ClientGameManager.UIManager.PlayRandomClip(audioClipCollection, null);
			}
		}
	}
}
