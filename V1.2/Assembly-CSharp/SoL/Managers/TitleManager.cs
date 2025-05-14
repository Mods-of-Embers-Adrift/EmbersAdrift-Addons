using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SoL.Game;
using SoL.Game.HuntingLog;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;

namespace SoL.Managers
{
	// Token: 0x0200054A RID: 1354
	public static class TitleManager
	{
		// Token: 0x14000082 RID: 130
		// (add) Token: 0x0600292B RID: 10539 RVA: 0x001403E0 File Offset: 0x0013E5E0
		// (remove) Token: 0x0600292C RID: 10540 RVA: 0x00140414 File Offset: 0x0013E614
		public static event Action TitlesChanged;

		// Token: 0x0600292D RID: 10541 RVA: 0x00140448 File Offset: 0x0013E648
		public static List<string> GetAvailableTitles(GameEntity entity, AccessFlags userFlags, ClientReward[] rewards)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, AccessFlags> keyValuePair in TitleManager.m_flaggedTitles)
			{
				if (userFlags.HasBitFlag(keyValuePair.Value))
				{
					list.Add(keyValuePair.Key);
				}
			}
			if (rewards != null)
			{
				for (int i = 0; i < rewards.Length; i++)
				{
					if (rewards[i] != null && rewards[i].rewardType == "title")
					{
						list.Add(rewards[i].code);
					}
				}
			}
			list.AddRange(TitleManager.m_freeTitles);
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null)
			{
				LearnableContainerRecord learnableContainerRecord;
				if (entity.CollectionController.Record.Learnables != null && entity.CollectionController.Record.Learnables.TryGetValue(ContainerType.Titles, out learnableContainerRecord))
				{
					foreach (UniqueId id in learnableContainerRecord.LearnableIds)
					{
						LearnableTitle learnableTitle;
						if (InternalGameDatabase.Archetypes.TryGetAsType<LearnableTitle>(id, out learnableTitle))
						{
							list.Add(learnableTitle.GetFormattedTitle());
						}
					}
				}
				if (entity.CollectionController.Record.AvailableTitles != null)
				{
					for (int j = 0; j < entity.CollectionController.Record.AvailableTitles.Count; j++)
					{
						list.Add(entity.CollectionController.Record.AvailableTitles[j]);
					}
				}
			}
			string item;
			if (TitleManager.TryGetSpecializationTitle(entity, out item))
			{
				list.Add(item);
				string item2;
				if (TitleManager.TryGetHighSpecializationTitle(entity, out item2))
				{
					list.Add(item2);
				}
			}
			if (GlobalSettings.Values != null && GlobalSettings.Values.HuntingLog != null && GlobalSettings.Values.HuntingLog.Enabled && entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.HuntingLog != null)
			{
				foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair2 in entity.CollectionController.Record.HuntingLog)
				{
					HuntingLogProfile huntingLogProfile;
					if (InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(keyValuePair2.Key, out huntingLogProfile))
					{
						huntingLogProfile.AddAcquiredTitles(keyValuePair2.Value, list);
					}
				}
			}
			if (GlobalSettings.Values != null && GlobalSettings.Values.Titles != null && GlobalSettings.Values.Titles.UnlockableTitles != null)
			{
				foreach (UnlockableTitle unlockableTitle in GlobalSettings.Values.Titles.UnlockableTitles)
				{
					if (unlockableTitle != null)
					{
						bool flag = false;
						using (List<string>.Enumerator enumerator4 = list.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								if (enumerator4.Current == unlockableTitle.Title)
								{
									flag = true;
								}
							}
						}
						if (!flag && unlockableTitle.Requirement.MeetsAllRequirements(entity))
						{
							list.Add(unlockableTitle.Title);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x001407C0 File Offset: 0x0013E9C0
		public static void AssignTitleToPlayer(GameEntity entity, string title)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			AccessFlags b;
			string b2;
			string b3;
			ProgressionRequirement progressionRequirement;
			if (string.IsNullOrEmpty(title) || TitleManager.m_freeTitles.Contains(title) || (TitleManager.m_flaggedTitles.TryGetValue(title, out b) && entity.UserFlags.HasBitFlag(b)) || (entity.CollectionController.Record.AvailableTitles != null && entity.CollectionController.Record.AvailableTitles.Contains(title)) || (TitleManager.TryGetSpecializationTitle(entity, out b2) && title == b2) || (TitleManager.TryGetHighSpecializationTitle(entity, out b3) && title == b3) || TitleManager.UserRecordContainsRewardTitle(entity, title) || TitleManager.CharacterContainsHuntingTitle(entity, title) || TitleManager.CharacterHasTitleFromArchetype(entity, title) || (GlobalSettings.Values.Titles.TryGetTitleRequirement(title, out progressionRequirement) && progressionRequirement.MeetsAllRequirements(entity)))
			{
				entity.CharacterData.Title.Value = title;
				entity.CollectionController.Record.Title = title;
				TitleManager.UpdateMetaRecord(entity.CollectionController.Record);
			}
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x001408CC File Offset: 0x0013EACC
		private static bool TryGetSpecializationTitle(GameEntity entity, out string specTitle)
		{
			specTitle = string.Empty;
			SpecializedRole specializedRole;
			if (entity && entity.CharacterData && !entity.CharacterData.SpecializedRoleId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(entity.CharacterData.SpecializedRoleId, out specializedRole))
			{
				specTitle = specializedRole.DisplayName;
				return true;
			}
			return false;
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x00140930 File Offset: 0x0013EB30
		private static bool TryGetHighSpecializationTitle(GameEntity entity, out string highSpecTitle)
		{
			highSpecTitle = string.Empty;
			SpecializedRole specializedRole;
			ArchetypeInstance archetypeInstance;
			if (entity && entity.CharacterData && (float)entity.CharacterData.AdventuringLevel >= 50f && !entity.CharacterData.SpecializedRoleId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(entity.CharacterData.SpecializedRoleId, out specializedRole) && specializedRole.GeneralRole && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(specializedRole.GeneralRole.Id, out archetypeInstance) && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null && archetypeInstance.MasteryData.Specialization.Value == specializedRole.Id && archetypeInstance.MasteryData.SpecializationLevel >= 50f)
			{
				highSpecTitle = "High " + specializedRole.DisplayName;
				return true;
			}
			return false;
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x00140A58 File Offset: 0x0013EC58
		private static bool UserRecordContainsRewardTitle(GameEntity entity, string title)
		{
			if (!entity || entity.User == null || entity.User.Rewards == null)
			{
				return false;
			}
			for (int i = 0; i < entity.User.Rewards.Length; i++)
			{
				if (entity.User.Rewards[i].rewardType == "title" && title == entity.User.Rewards[i].code)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x00140AD8 File Offset: 0x0013ECD8
		private static bool CharacterContainsHuntingTitle(GameEntity entity, string title)
		{
			if (!entity || entity.CollectionController == null || entity.CollectionController.Record == null || entity.CollectionController.Record.HuntingLog == null)
			{
				return false;
			}
			if (GlobalSettings.Values.HuntingLog.Enabled)
			{
				foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair in entity.CollectionController.Record.HuntingLog)
				{
					HuntingLogProfile huntingLogProfile;
					if (InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(keyValuePair.Key, out huntingLogProfile) && huntingLogProfile.IsAcquiredTitle(keyValuePair.Value.PerkCount, title, keyValuePair.Value))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06002933 RID: 10547 RVA: 0x00140BA8 File Offset: 0x0013EDA8
		private static bool CharacterHasTitleFromArchetype(GameEntity entity, string title)
		{
			if (!entity || entity.CollectionController == null || entity.CollectionController.Record == null || entity.CollectionController.Record.Learnables == null)
			{
				return false;
			}
			LearnableContainerRecord learnableContainerRecord;
			if (entity.CollectionController.Record.Learnables.TryGetValue(ContainerType.Titles, out learnableContainerRecord))
			{
				foreach (UniqueId id in learnableContainerRecord.LearnableIds)
				{
					LearnableTitle learnableTitle;
					if (InternalGameDatabase.Archetypes.TryGetAsType<LearnableTitle>(id, out learnableTitle) && learnableTitle.GetFormattedTitle().Equals(title))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06002934 RID: 10548 RVA: 0x00140C68 File Offset: 0x0013EE68
		public static bool AlterTitles(GameEntity entity, string title)
		{
			if (string.IsNullOrEmpty(title) || TitleManager.m_freeTitles.ContainsIgnoreCase(title) || TitleManager.m_flaggedTitles.ContainsKey(title))
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			if (entity.CollectionController.Record.AvailableTitles == null)
			{
				entity.CollectionController.Record.AvailableTitles = new List<string>
				{
					title
				};
				flag2 = true;
				flag = true;
			}
			else
			{
				for (int i = 0; i < entity.CollectionController.Record.AvailableTitles.Count; i++)
				{
					if (entity.CollectionController.Record.AvailableTitles[i].Equals(title, StringComparison.InvariantCultureIgnoreCase))
					{
						entity.CollectionController.Record.AvailableTitles.RemoveAt(i);
						if (!string.IsNullOrEmpty(entity.CollectionController.Record.Title) && entity.CollectionController.Record.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
						{
							entity.CollectionController.Record.Title = "";
							if (GameManager.IsServer)
							{
								entity.CharacterData.Title.Value = "";
							}
						}
						if (entity.CollectionController.Record.AvailableTitles.Count <= 0)
						{
							entity.CollectionController.Record.AvailableTitles = null;
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					entity.CollectionController.Record.AvailableTitles.Add(title);
					flag2 = true;
					flag = true;
				}
			}
			if (flag)
			{
				TitleManager.InvokeTitlesChangedEvent();
				if (GameManager.IsServer)
				{
					entity.NetworkEntity.PlayerRpcHandler.TitleModifiedResponse(title);
					TitleManager.UpdateMetaRecord(entity.CollectionController.Record);
				}
				else if (entity == LocalPlayer.GameEntity)
				{
					string content = flag2 ? ("Title of " + title + " added!") : ("Title of " + title + " removed!");
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
			}
			return flag;
		}

		// Token: 0x06002935 RID: 10549 RVA: 0x0005C7B8 File Offset: 0x0005A9B8
		public static void InvokeTitlesChangedEvent()
		{
			Action titlesChanged = TitleManager.TitlesChanged;
			if (titlesChanged == null)
			{
				return;
			}
			titlesChanged();
		}

		// Token: 0x06002936 RID: 10550 RVA: 0x00140E50 File Offset: 0x0013F050
		private static void UpdateMetaRecord(CharacterRecord record)
		{
			TitleManager.<UpdateMetaRecord>d__16 <UpdateMetaRecord>d__;
			<UpdateMetaRecord>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateMetaRecord>d__.<>1__state = -1;
			<UpdateMetaRecord>d__.<>t__builder.Start<TitleManager.<UpdateMetaRecord>d__16>(ref <UpdateMetaRecord>d__);
		}

		// Token: 0x04002A27 RID: 10791
		private const string kTitleRewardType = "title";

		// Token: 0x04002A29 RID: 10793
		private static readonly List<string> m_freeTitles = new List<string>();

		// Token: 0x04002A2A RID: 10794
		private static readonly Dictionary<string, AccessFlags> m_flaggedTitles = new Dictionary<string, AccessFlags>(StringComparer.InvariantCultureIgnoreCase)
		{
			{
				"GM",
				AccessFlags.GM
			},
			{
				"Hero of Stormhaven",
				AccessFlags.Subscriber
			}
		};

		// Token: 0x04002A2B RID: 10795
		private static readonly Dictionary<string, int> m_tieredTitles = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
		{
			{
				"Collector",
				5
			},
			{
				"Patron",
				4
			},
			{
				"Founder",
				3
			}
		};
	}
}
