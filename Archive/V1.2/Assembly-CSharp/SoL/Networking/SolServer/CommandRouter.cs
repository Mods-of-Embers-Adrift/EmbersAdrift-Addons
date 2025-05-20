using System;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003DE RID: 990
	public static class CommandRouter
	{
		// Token: 0x06001A81 RID: 6785 RVA: 0x001091E4 File Offset: 0x001073E4
		public static void Route(SolServerCommand cmd)
		{
			switch (cmd.CommandClass)
			{
			case CommandClass.client:
				ClientHandler.Handle(cmd);
				return;
			case CommandClass.chat:
				ChatHandler.Handle(cmd);
				return;
			case CommandClass.group:
				GroupHandler.Handle(cmd);
				return;
			case CommandClass.guild:
				GuildHandler.Handle(cmd);
				break;
			case CommandClass.gm:
			case CommandClass.server:
			case CommandClass.local:
			case CommandClass.combat:
				break;
			case CommandClass.login:
				LoginHandler.Handle(cmd);
				return;
			case CommandClass.lookingfor:
				LookingForHandler.Handle(cmd);
				return;
			case CommandClass.mail:
				MailHandler.Handle(cmd);
				return;
			case CommandClass.relations:
				RelationsHandler.Handle(cmd);
				return;
			default:
				return;
			}
		}
	}
}
