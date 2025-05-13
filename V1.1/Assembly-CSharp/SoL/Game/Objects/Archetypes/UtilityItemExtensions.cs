using System;
using SoL.Managers;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AAD RID: 2733
	public static class UtilityItemExtensions
	{
		// Token: 0x06005474 RID: 21620 RVA: 0x00078748 File Offset: 0x00076948
		public static void InitializeUtilityItemMode(ArchetypeInstance sourceInstance, IUtilityItem utilityItem)
		{
			if (sourceInstance == null || utilityItem == null)
			{
				UtilityItemExtensions.ResetUtilityItemMode();
				return;
			}
			UtilityItemExtensions.m_sourceInstance = sourceInstance;
			UtilityItemExtensions.m_sourceItem = utilityItem;
			IUtilityItem sourceItem = UtilityItemExtensions.m_sourceItem;
			if (sourceItem != null)
			{
				sourceItem.PlayAudioClip();
			}
			CursorManager.ToggleGameMode(CursorGameMode.UtilityItem);
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x00078778 File Offset: 0x00076978
		public static void ResetUtilityItemMode()
		{
			UtilityItemExtensions.m_sourceInstance = null;
			UtilityItemExtensions.m_sourceItem = null;
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x001DA9C8 File Offset: 0x001D8BC8
		public static void OnClientUtilityItemUsage(ArchetypeInstance targetInstance)
		{
			if (UtilityItemExtensions.m_sourceInstance == null)
			{
				throw new ArgumentNullException("m_sourceInstance");
			}
			if (UtilityItemExtensions.m_sourceItem == null)
			{
				throw new ArgumentNullException("m_sourceItem");
			}
			if (targetInstance == null)
			{
				throw new ArgumentNullException("targetInstance");
			}
			UtilityItemExtensions.m_sourceItem.ClientRequestExecuteUtility(LocalPlayer.GameEntity, UtilityItemExtensions.m_sourceInstance, targetInstance);
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x00078786 File Offset: 0x00076986
		public static CursorType GetCursorForUtilityItem()
		{
			if (UtilityItemExtensions.m_sourceItem == null)
			{
				return CursorType.MainCursor;
			}
			return UtilityItemExtensions.m_sourceItem.GetCursorType();
		}

		// Token: 0x04004B16 RID: 19222
		private static ArchetypeInstance m_sourceInstance;

		// Token: 0x04004B17 RID: 19223
		private static IUtilityItem m_sourceItem;
	}
}
