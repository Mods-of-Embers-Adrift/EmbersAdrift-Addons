using System;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Utilities;

namespace SoL.Game.GM
{
	// Token: 0x02000BF2 RID: 3058
	public static class QACommands
	{
		// Token: 0x06005E67 RID: 24167 RVA: 0x0007F6F3 File Offset: 0x0007D8F3
		private static bool LocalNetworkEntityExists()
		{
			return LocalPlayer.NetworkEntity;
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x0007F6FF File Offset: 0x0007D8FF
		private static bool IsNearEmberRing()
		{
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire);
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x0007F761 File Offset: 0x0007D961
		public static void RegisterQACommands()
		{
			if (!DeploymentBranchFlagsExtensions.IsQA())
			{
				return;
			}
			CommandRegistry.Register("qa", delegate(string[] args)
			{
				if (!QACommands.LocalNetworkEntityExists())
				{
					return;
				}
				if (!QACommands.IsNearEmberRing())
				{
					QACommands.Log("Must be near an ember ring to use /qa!");
					return;
				}
				if (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == 50)
				{
					QACommands.Log(string.Format("You are already in {0}!", ZoneId.QuartermastersAlcove));
					return;
				}
				QACommands.Log(string.Format("Requesting zone to {0}", ZoneId.QuartermastersAlcove));
				LoginApiManager.PerformZoneCheck(ZoneId.QuartermastersAlcove, delegate(bool response)
				{
					if (response && QACommands.LocalNetworkEntityExists())
					{
						LocalPlayer.NetworkEntity.PlayerRpcHandler.QA_RequestZoneToQA();
						return;
					}
					QACommands.Log(string.Format("Failed to zone to {0}!", ZoneId.QuartermastersAlcove));
				});
			}, "Teleport to the QA zone", "Teleport to the QA zone while you are near an Ember Ring. (Usage: /qa)", null);
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x0007F746 File Offset: 0x0007D946
		private static void Log(string txt)
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, txt);
		}

		// Token: 0x040051A6 RID: 20902
		public const ZoneId kQAZoneId = ZoneId.QuartermastersAlcove;
	}
}
