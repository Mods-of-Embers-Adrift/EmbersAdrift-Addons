using System;
using SoL.Game.Messages;
using SoL.Networking.Objects;

namespace SoL.Game.Dueling
{
	// Token: 0x02000C9F RID: 3231
	public static class DuelExtensions
	{
		// Token: 0x06006208 RID: 25096 RVA: 0x000820E7 File Offset: 0x000802E7
		public static DuelStatus GetForfeitStatus(this DuelStatus currentStatus)
		{
			switch (currentStatus)
			{
			case DuelStatus.None:
			case DuelStatus.Requested:
			case DuelStatus.Declined:
			case DuelStatus.Expired:
				return DuelStatus.Cancelled;
			}
			return DuelStatus.Forfeited;
		}

		// Token: 0x06006209 RID: 25097 RVA: 0x0020343C File Offset: 0x0020163C
		private static bool TryGetDuelTarget(GameEntity source, out NetworkEntity target, out string msg)
		{
			target = null;
			msg = string.Empty;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.NetworkEntity && LocalPlayer.GameEntity.TargetController)
			{
				GameEntity defensiveTarget = LocalPlayer.GameEntity.TargetController.DefensiveTarget;
				if (defensiveTarget && defensiveTarget.NetworkEntity)
				{
					if (defensiveTarget == LocalPlayer.GameEntity)
					{
						msg = "You cannot duel yourself!";
					}
					else if (!DuelExtensions.IsWithinDuelDistance(source, defensiveTarget))
					{
						msg = "Out of range!";
					}
					else
					{
						target = defensiveTarget.NetworkEntity;
					}
				}
				else
				{
					msg = "Invalid duel target!";
				}
			}
			return target != null;
		}

		// Token: 0x0600620A RID: 25098 RVA: 0x002034EC File Offset: 0x002016EC
		public static bool IsWithinDuelDistance(GameEntity source, GameEntity target)
		{
			return source && target && (source.gameObject.transform.position - target.gameObject.transform.position).sqrMagnitude <= 100f;
		}

		// Token: 0x0600620B RID: 25099 RVA: 0x00203544 File Offset: 0x00201744
		public static void ClientRequestDuelAttempt()
		{
			string empty = string.Empty;
			NetworkEntity opponent;
			if (LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler && DuelExtensions.TryGetDuelTarget(LocalPlayer.GameEntity, out opponent, out empty))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.DuelRequest(opponent);
				return;
			}
			if (!string.IsNullOrEmpty(empty))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, empty);
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unknown duel error.");
		}
	}
}
