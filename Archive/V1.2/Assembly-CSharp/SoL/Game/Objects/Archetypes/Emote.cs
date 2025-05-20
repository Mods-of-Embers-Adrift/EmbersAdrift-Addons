using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Animation;
using SoL.Game.Messages;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AB8 RID: 2744
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Emote")]
	public class Emote : LearnableArchetype
	{
		// Token: 0x060054D2 RID: 21714 RVA: 0x00078B89 File Offset: 0x00076D89
		private string GetPrepositionText(Emote.TargetPreposition preposition)
		{
			if (preposition == Emote.TargetPreposition.kFor)
			{
				return "for";
			}
			if (preposition != Emote.TargetPreposition.empty)
			{
				return preposition.ToString();
			}
			return "";
		}

		// Token: 0x060054D3 RID: 21715 RVA: 0x00078BAE File Offset: 0x00076DAE
		private string GetTitle()
		{
			if (!this.m_separateFemaleSequence)
			{
				return "Animation";
			}
			return "Male";
		}

		// Token: 0x17001382 RID: 4994
		// (get) Token: 0x060054D4 RID: 21716 RVA: 0x00078BC3 File Offset: 0x00076DC3
		public bool SubscriberOnly
		{
			get
			{
				return this.m_subscriberOnly;
			}
		}

		// Token: 0x17001383 RID: 4995
		// (get) Token: 0x060054D5 RID: 21717 RVA: 0x00078BCB File Offset: 0x00076DCB
		public bool DeferHandIk
		{
			get
			{
				return this.m_deferHandIk;
			}
		}

		// Token: 0x17001384 RID: 4996
		// (get) Token: 0x060054D6 RID: 21718 RVA: 0x00078BD3 File Offset: 0x00076DD3
		public float DeferHandIkDuration
		{
			get
			{
				return this.m_deferHandIkDuration;
			}
		}

		// Token: 0x060054D7 RID: 21719 RVA: 0x00078BDB File Offset: 0x00076DDB
		public AnimationSequenceWithOverride GetAnimationSequence(CharacterSex sex)
		{
			if (this.m_separateFemaleSequence && sex != CharacterSex.Male && !this.m_femaleAnimationSequence.IsEmpty)
			{
				return this.m_femaleAnimationSequence;
			}
			return this.m_animationSequence;
		}

		// Token: 0x060054D8 RID: 21720 RVA: 0x001DB7F4 File Offset: 0x001D99F4
		private bool MatchesCommand(string cmd)
		{
			for (int i = 0; i < this.m_chatCommands.Length; i++)
			{
				if (string.Equals(cmd, this.m_chatCommands[i], StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060054D9 RID: 21721 RVA: 0x001DB828 File Offset: 0x001D9A28
		public static bool AttemptEmote(GameEntity entity, string inputCommand)
		{
			if (entity && entity.NetworkEntity && entity.NetworkEntity.PlayerRpcHandler)
			{
				if (string.Equals(inputCommand, "emotes", StringComparison.InvariantCultureIgnoreCase) || string.Equals(inputCommand, "listemotes", StringComparison.InvariantCultureIgnoreCase))
				{
					Emote.PrintKnownEmotes(entity);
					return true;
				}
				for (int i = 0; i < GlobalSettings.Values.Animation.DefaultEmotes.Length; i++)
				{
					Emote emote = GlobalSettings.Values.Animation.DefaultEmotes[i];
					if (emote.MatchesCommand(inputCommand))
					{
						Emote.PlayMatchingEmote(entity, emote);
						return true;
					}
				}
				for (int j = 0; j < entity.CollectionController.Emotes.Count; j++)
				{
					Emote emote2;
					if (entity.CollectionController.Emotes.GetIndex(j).TryGetAsType(out emote2) && emote2.MatchesCommand(inputCommand))
					{
						Emote.PlayMatchingEmote(entity, emote2);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060054DA RID: 21722 RVA: 0x001DB914 File Offset: 0x001D9B14
		private static void PrintKnownEmotes(GameEntity entity)
		{
			if (!entity)
			{
				return;
			}
			List<Emote> list = new List<Emote>(20);
			List<Emote> list2 = new List<Emote>(20);
			foreach (Emote emote in GlobalSettings.Values.Animation.DefaultEmotes)
			{
				if (emote.m_subscriberOnly)
				{
					list2.Add(emote);
				}
				else
				{
					list.Add(emote);
				}
			}
			if (entity.CollectionController != null && entity.CollectionController.Emotes != null)
			{
				for (int j = 0; j < entity.CollectionController.Emotes.Count; j++)
				{
					Emote emote2;
					if (entity.CollectionController.Emotes.GetIndex(j).TryGetAsType(out emote2))
					{
						if (emote2.m_subscriberOnly)
						{
							if (!list2.Contains(emote2))
							{
								list2.Add(emote2);
							}
						}
						else if (!list.Contains(emote2))
						{
							list.Add(emote2);
						}
					}
				}
			}
			list.Sort((Emote a, Emote b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.InvariantCultureIgnoreCase));
			list2.Sort((Emote a, Emote b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.InvariantCultureIgnoreCase));
			List<string> list3 = new List<string>(list.Count + list2.Count);
			foreach (Emote emote3 in list)
			{
				if (emote3.m_chatCommands != null && emote3.m_chatCommands.Length != 0)
				{
					list3.Add(emote3.m_chatCommands[0].ToLowerInvariant());
				}
			}
			foreach (Emote emote4 in list2)
			{
				if (emote4.m_chatCommands != null && emote4.m_chatCommands.Length != 0)
				{
					string item = ZString.Format<string, string, string>("<color={0}><link=\"{1}:Subscribers Only\">{2}</link></color>", UIManager.SubscriberColor.ToHex(), "text", emote4.m_chatCommands[0].ToLowerInvariant());
					list3.Add(item);
				}
			}
			string content = (list3.Count > 0) ? ("Known emotes: " + string.Join(", ", list3)) : "Known emotes: none";
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
		}

		// Token: 0x060054DB RID: 21723 RVA: 0x001DBB70 File Offset: 0x001D9D70
		private static void PlayMatchingEmote(GameEntity entity, Emote emote)
		{
			if (!entity.Vitals.Stance.CanEmote())
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot emote in " + entity.Vitals.Stance.ToString() + " stance!");
				return;
			}
			if (emote.m_subscriberOnly && !entity.Subscriber)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "This emote is reserved for subscribers. <link=\"activateSub\"><u>Activate your subscription.</u></link>");
				return;
			}
			entity.NetworkEntity.PlayerRpcHandler.PlayEmoteRequest(emote.Id);
		}

		// Token: 0x060054DC RID: 21724 RVA: 0x001DBBFC File Offset: 0x001D9DFC
		public static void AddEmoteTextToChat(GameEntity entity, Emote emote)
		{
			string text = "Someone";
			string text2 = string.Empty;
			if (entity != null)
			{
				if (entity.CharacterData != null)
				{
					text = entity.CharacterData.Name.Value;
				}
				if (emote.m_targetPreposition != Emote.TargetPreposition.none && entity.TargetController != null)
				{
					if (entity.TargetController.OffensiveTarget != null && entity.TargetController.OffensiveTarget.CharacterData != null)
					{
						text2 = entity.TargetController.OffensiveTarget.CharacterData.Name.Value;
					}
					else if (entity.TargetController.DefensiveTarget != null && entity.TargetController.DefensiveTarget != entity && entity.TargetController.DefensiveTarget.CharacterData != null)
					{
						text2 = entity.TargetController.DefensiveTarget.CharacterData.Name.Value;
					}
				}
			}
			if (ClientGameManager.SocialManager.IsBlocked(text))
			{
				return;
			}
			string text3 = emote.m_emoteText;
			if (emote.m_targetPreposition != Emote.TargetPreposition.none && !string.IsNullOrEmpty(text2))
			{
				string str = string.IsNullOrEmpty(emote.GetPrepositionText(emote.m_targetPreposition)) ? (" " + text2) : (" " + emote.GetPrepositionText(emote.m_targetPreposition) + " " + text2);
				text3 += str;
			}
			SolServerCommand solServerCommand = CommandClass.chat.NewCommand(SoL.Networking.SolServer.CommandType.emote);
			solServerCommand.Args.Add("Message", text3);
			solServerCommand.Args.Add("Sender", text);
			solServerCommand.Args.Add("presence", PresenceFlags.Online.ToString());
			ChatHandler.Handle(solServerCommand);
			solServerCommand.Args.ReturnToPool();
		}

		// Token: 0x060054DD RID: 21725 RVA: 0x001DBDC4 File Offset: 0x001D9FC4
		public override bool EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			errorMessage = string.Empty;
			if (entity == null || entity.CollectionController == null)
			{
				return false;
			}
			LearnableContainerInstance learnableContainerInstance;
			LearnableArchetype learnableArchetype;
			if (entity.CollectionController.TryGetLearnableInstance(ContainerType.Emotes, out learnableContainerInstance) && learnableContainerInstance.TryGetLearnableForId(base.Id, out learnableArchetype))
			{
				errorMessage = "Emote already known!";
				return false;
			}
			return true;
		}

		// Token: 0x060054DE RID: 21726 RVA: 0x001DBE18 File Offset: 0x001DA018
		public override bool AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance resultingInstance)
		{
			resultingInstance = null;
			if (!GameManager.IsServer)
			{
				return false;
			}
			LearnableContainerInstance learnableContainerInstance;
			string text;
			if (entity.CollectionController.TryGetLearnableInstance(ContainerType.Emotes, out learnableContainerInstance) && this.EntityCanAcquire(entity, out text))
			{
				learnableContainerInstance.Add(this, true);
				this.m_tempIdArray[0] = base.Id;
				LearnablesAddedTransaction transaction = new LearnablesAddedTransaction
				{
					Op = OpCodes.Ok,
					LearnableIds = this.m_tempIdArray,
					TargetContainer = learnableContainerInstance.Id
				};
				entity.NetworkEntity.PlayerRpcHandler.LearnablesAdded(transaction);
				return true;
			}
			return false;
		}

		// Token: 0x04004B52 RID: 19282
		private const string kEmote = "Emote";

		// Token: 0x04004B53 RID: 19283
		private const string kListEmotes = "emotes";

		// Token: 0x04004B54 RID: 19284
		private const string kListEmotesAlt = "listemotes";

		// Token: 0x04004B55 RID: 19285
		[SerializeField]
		private bool m_subscriberOnly;

		// Token: 0x04004B56 RID: 19286
		[SerializeField]
		private AnimationSequenceWithOverride m_animationSequence;

		// Token: 0x04004B57 RID: 19287
		[SerializeField]
		private bool m_separateFemaleSequence;

		// Token: 0x04004B58 RID: 19288
		[SerializeField]
		private AnimationSequenceWithOverride m_femaleAnimationSequence;

		// Token: 0x04004B59 RID: 19289
		[SerializeField]
		private bool m_deferHandIk;

		// Token: 0x04004B5A RID: 19290
		[SerializeField]
		private float m_deferHandIkDuration;

		// Token: 0x04004B5B RID: 19291
		[SerializeField]
		private string[] m_chatCommands;

		// Token: 0x04004B5C RID: 19292
		[SerializeField]
		private string m_emoteText;

		// Token: 0x04004B5D RID: 19293
		[SerializeField]
		private Emote.TargetPreposition m_targetPreposition;

		// Token: 0x04004B5E RID: 19294
		private UniqueId[] m_tempIdArray = new UniqueId[1];

		// Token: 0x02000AB9 RID: 2745
		private enum TargetPreposition
		{
			// Token: 0x04004B60 RID: 19296
			none,
			// Token: 0x04004B61 RID: 19297
			with,
			// Token: 0x04004B62 RID: 19298
			at,
			// Token: 0x04004B63 RID: 19299
			kFor,
			// Token: 0x04004B64 RID: 19300
			empty,
			// Token: 0x04004B65 RID: 19301
			to
		}
	}
}
