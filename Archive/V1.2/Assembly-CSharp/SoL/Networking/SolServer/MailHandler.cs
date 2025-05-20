using System;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E3 RID: 995
	public static class MailHandler
	{
		// Token: 0x06001A88 RID: 6792 RVA: 0x00109850 File Offset: 0x00107A50
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
			if (command != CommandType.notification)
			{
				if (command != CommandType.namecheck)
				{
					return;
				}
				if (ClientGameManager.SocialManager)
				{
					CharacterIdentification ident = null;
					string text = null;
					int attachmentsRemaining = -1;
					if (cmd.Args.ContainsKey("data") && cmd.Args["data"] != null)
					{
						cmd.TryDeserializeKey("data", out ident);
					}
					if (cmd.Args.ContainsKey("Message") && cmd.Args["Message"] != null)
					{
						text = (string)cmd.Args["Message"];
					}
					if (cmd.Args.ContainsKey("attachmentsremaining") && cmd.Args["attachmentsremaining"] != null)
					{
						cmd.TryDeserializeKey("attachmentsremaining", out attachmentsRemaining);
					}
					if (text != null)
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text);
					}
					ClientGameManager.SocialManager.NameCheckResponse(ident, attachmentsRemaining);
				}
			}
			else
			{
				Mail[] mail;
				if (cmd.Args.ContainsKey("list") && cmd.TryDeserializeKey("list", out mail))
				{
					ClientGameManager.SocialManager.EnqueueMail(mail);
				}
				string id;
				MailType type;
				object obj;
				if (cmd.TryGetArgValue("mailid", out id) && cmd.Args.ContainsKey("type") && cmd.TryDeserializeKey("type", out type) && cmd.Args.TryGetValue("shoulddelete", out obj) && (bool)obj)
				{
					ClientGameManager.SocialManager.DeleteMail(type, id);
				}
				string content2;
				if (cmd.TryGetArgValue("Message", out content2))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Social, content2);
					return;
				}
			}
		}
	}
}
