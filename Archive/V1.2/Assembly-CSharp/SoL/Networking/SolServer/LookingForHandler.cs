using System;
using SoL.Game.Grouping;
using SoL.Game.Messages;
using SoL.Managers;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E2 RID: 994
	public static class LookingForHandler
	{
		// Token: 0x06001A87 RID: 6791 RVA: 0x001097B8 File Offset: 0x001079B8
		public static void Handle(SolServerCommand cmd)
		{
			if (!cmd.State)
			{
				ClientGameManager.SocialManager.StopLooking(false);
				string content;
				if (cmd.TryGetArgValue("err", out content))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
				return;
			}
			object obj;
			if (cmd.Command == CommandType.listall && cmd.Args.TryGetValue("modified", out obj) && obj is bool)
			{
				bool flag = (bool)obj;
				LookingFor[] entries;
				if (flag && cmd.TryDeserializeKey("list", out entries))
				{
					ClientGameManager.SocialManager.EnqueueLookingEntries(entries);
					return;
				}
				if (!flag)
				{
					ClientGameManager.SocialManager.EnqueueLookingEntries(null);
				}
			}
		}
	}
}
