using System;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E0 RID: 992
	public static class GuildHandler
	{
		// Token: 0x06001A84 RID: 6788 RVA: 0x001095DC File Offset: 0x001077DC
		public static void Handle(SolServerCommand cmd)
		{
			if (!cmd.State)
			{
				string content;
				if (cmd.TryGetArgValue("err", out content))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
				return;
			}
			CommandType command = cmd.Command;
			if (command != CommandType.delete)
			{
				if (command != CommandType.update)
				{
					if (command != CommandType.deletemember)
					{
						return;
					}
					string[] array = cmd.DeserializeKey("list");
					if (array != null && array.Length != 0)
					{
						ClientGameManager.SocialManager.EnqueueGuildRosterRemovals(array);
					}
				}
				else
				{
					Guild guild = null;
					GuildMember[] array2 = null;
					if (cmd.Args.ContainsKey("guild"))
					{
						guild = cmd.DeserializeKey("guild");
					}
					if (cmd.Args.ContainsKey("list"))
					{
						array2 = cmd.DeserializeKey("list");
					}
					if (guild != null)
					{
						ClientGameManager.SocialManager.UpdateGuild(guild);
					}
					if (array2 != null && array2.Length != 0)
					{
						ClientGameManager.SocialManager.EnqueueGuildRosterUpdates(array2);
						return;
					}
				}
				return;
			}
			ClientGameManager.SocialManager.UpdateGuild(null);
		}
	}
}
