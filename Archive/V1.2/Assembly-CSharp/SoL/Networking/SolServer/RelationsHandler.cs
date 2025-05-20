using System;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003E5 RID: 997
	public static class RelationsHandler
	{
		// Token: 0x06001A8A RID: 6794 RVA: 0x00109A20 File Offset: 0x00107C20
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
			PlayerStatus[] updates;
			if (command <= CommandType.renamed)
			{
				if (command != CommandType.notification)
				{
					if (command != CommandType.renamed)
					{
						return;
					}
					string oldName;
					string newName;
					if (cmd.TryGetArgValue("oldname", out oldName) && cmd.TryGetArgValue("newname", out newName))
					{
						ClientGameManager.SocialManager.UpdateRelationName(oldName, newName);
						ClientGameManager.GroupManager.RenameGroupMember(oldName, newName);
					}
				}
				else
				{
					Relation[] relations;
					if (cmd.Args.ContainsKey("list") && cmd.TryDeserializeKey("list", out relations))
					{
						ClientGameManager.SocialManager.EnqueueRelations(relations);
					}
					string content2;
					if (cmd.TryGetArgValue("Message", out content2))
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Social, content2);
						return;
					}
				}
			}
			else if (command != CommandType.statusupdate)
			{
				if (command != CommandType.delete)
				{
					return;
				}
				string id;
				RelationType type;
				if (cmd.TryGetArgValue("relid", out id) && cmd.Args.ContainsKey("type") && cmd.TryDeserializeKey("type", out type))
				{
					ClientGameManager.SocialManager.DeleteRelation(type, id);
				}
				string content3;
				if (cmd.TryGetArgValue("Message", out content3))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Social, content3);
					return;
				}
			}
			else if (cmd.Args.ContainsKey("list") && cmd.TryDeserializeKey("list", out updates))
			{
				ClientGameManager.SocialManager.EnqueuePlayerStatusUpdates(updates);
				return;
			}
		}
	}
}
