using System;
using System.Collections.Generic;
using System.Text;
using Ink;
using Ink.Runtime;
using SoL.Game.Interactives;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests.Objectives;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using SoL.Game.UI.Dialog;
using SoL.GameCamera;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000781 RID: 1921
	public static class DialogueManager
	{
		// Token: 0x140000B3 RID: 179
		// (add) Token: 0x06003894 RID: 14484 RVA: 0x0016E204 File Offset: 0x0016C404
		// (remove) Token: 0x06003895 RID: 14485 RVA: 0x0016E238 File Offset: 0x0016C438
		public static event Action DialogueUpdated;

		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06003896 RID: 14486 RVA: 0x0016E26C File Offset: 0x0016C46C
		// (remove) Token: 0x06003897 RID: 14487 RVA: 0x0016E2A0 File Offset: 0x0016C4A0
		public static event Action<Reward, bool> RewardAvailable;

		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x06003898 RID: 14488 RVA: 0x0016E2D4 File Offset: 0x0016C4D4
		// (remove) Token: 0x06003899 RID: 14489 RVA: 0x0016E308 File Offset: 0x0016C508
		public static event Action DialogueTerminated;

		// Token: 0x17000CF2 RID: 3314
		// (get) Token: 0x0600389A RID: 14490 RVA: 0x000667A3 File Offset: 0x000649A3
		public static Story Story
		{
			get
			{
				return DialogueManager.m_story;
			}
		}

		// Token: 0x17000CF3 RID: 3315
		// (get) Token: 0x0600389B RID: 14491 RVA: 0x000667AA File Offset: 0x000649AA
		public static List<Choice> Choices
		{
			get
			{
				return DialogueManager.m_choices;
			}
		}

		// Token: 0x17000CF4 RID: 3316
		// (get) Token: 0x0600389C RID: 14492 RVA: 0x000667B1 File Offset: 0x000649B1
		public static bool IsChoice
		{
			get
			{
				return DialogueManager.m_isChoice;
			}
		}

		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x0600389D RID: 14493 RVA: 0x000667B8 File Offset: 0x000649B8
		public static bool StoryActive
		{
			get
			{
				return DialogueManager.m_storyActive;
			}
		}

		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x0600389E RID: 14494 RVA: 0x000667BF File Offset: 0x000649BF
		public static BaseNetworkedInteractive CurrentInteractive
		{
			get
			{
				return DialogueManager.m_currentInteractive;
			}
		}

		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x0600389F RID: 14495 RVA: 0x000667C6 File Offset: 0x000649C6
		public static WorldObject CurrentWorldObject
		{
			get
			{
				return DialogueManager.m_currentNpcWorldObj;
			}
		}

		// Token: 0x17000CF8 RID: 3320
		// (get) Token: 0x060038A0 RID: 14496 RVA: 0x000667CD File Offset: 0x000649CD
		public static DialogSourceType CurrentSourceType
		{
			get
			{
				return DialogueManager.m_currentSourceType;
			}
		}

		// Token: 0x060038A1 RID: 14497 RVA: 0x0016E33C File Offset: 0x0016C53C
		public static void InitiateDialogue(DialogueSource source, BaseNetworkedInteractive interactive = null, WorldObject worldObj = null, DialogSourceType sourceType = DialogSourceType.NPC)
		{
			if (GameManager.IsServer || DialogueManager.m_storyActive)
			{
				return;
			}
			bool flag = GameManager.QuestManager.TryGetDialogueState(source.Id, out DialogueManager.m_currentQuestDialogue) && DialogueManager.m_currentQuestDialogue != null && DialogueManager.m_currentQuestDialogue.Count > 0;
			DialogueManager.m_currentSource = source;
			DialogueManager.m_currentProfile = (source as NpcProfile);
			DialogueManager.m_currentInteractive = interactive;
			DialogueManager.m_currentNpcWorldObj = worldObj;
			DialogueManager.m_currentSourceType = sourceType;
			DialogueManager.m_story = null;
			DialogueManager.m_lines.Clear();
			DialogueManager.m_choices.Clear();
			DialogueManager.m_isChoice = false;
			DialogueManager.m_isChoiceText = false;
			DialogueManager.m_shouldStartNewAfterRewardChoice = false;
			DialogueManager.m_shouldRestartAfterRewardChoice = false;
			DialogueManager.m_rewardChoiceObjectiveHash = 0;
			DialogueManager.m_rewardIsReissue = false;
			TextAsset textAsset = null;
			string text = null;
			if (flag && DialogueManager.m_currentQuestDialogue.Count == 1 && DialogueManager.m_currentQuestDialogue[0].OverrideNonQuestDialogue)
			{
				textAsset = DialogueManager.m_currentQuestDialogue[0].InkStory;
				text = DialogueManager.m_currentQuestDialogue[0].TargetPath;
				DialogueManager.m_currentQuest = DialogueManager.m_currentQuestDialogue[0].Quest;
			}
			else if (DialogueManager.m_currentSource != null && DialogueManager.m_currentSource.DefaultDialogue.InkStory != null && !string.IsNullOrEmpty(DialogueManager.m_currentSource.DefaultDialogue.TargetPath))
			{
				textAsset = DialogueManager.m_currentSource.DefaultDialogue.InkStory;
				text = DialogueManager.m_currentSource.DefaultDialogue.TargetPath;
				DialogueManager.m_currentQuest = null;
			}
			if (textAsset == null || string.IsNullOrEmpty(text))
			{
				Debug.LogError("Unable to compose dialogue! Either there is no dialogue defined for this NPC, or it's unclear what dialogue should be used.");
				return;
			}
			if (DialogueManager.m_story != null)
			{
				DialogueManager.m_story.onError -= DialogueManager.OnError;
			}
			DialogueManager.m_story = new Story(textAsset.text);
			DialogueManager.m_story.onError += DialogueManager.OnError;
			DialogueManager.InjectGlobals();
			try
			{
				DialogueManager.m_story.ChoosePathString(text, true, Array.Empty<object>());
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Exception trying to access {0} for story {1}.\n{2}", text, textAsset.name, arg));
			}
			DialogueManager.PumpStory();
			DialogueManager.m_storyActive = true;
			Action dialogueUpdated = DialogueManager.DialogueUpdated;
			if (dialogueUpdated == null)
			{
				return;
			}
			dialogueUpdated();
		}

		// Token: 0x060038A2 RID: 14498 RVA: 0x0016E568 File Offset: 0x0016C768
		public static void TerminateDialogue()
		{
			DialogueManager.m_storyActive = false;
			DialogueManager.m_shouldStartNewAfterRewardChoice = false;
			DialogueManager.m_shouldRestartAfterRewardChoice = false;
			DialogueManager.m_rewardChoiceObjectiveHash = 0;
			DialogueManager.m_rewardIsReissue = false;
			GameManager.QuestManager.QuestUpdated -= DialogueManager.OnQuestUpdated;
			Action dialogueTerminated = DialogueManager.DialogueTerminated;
			if (dialogueTerminated != null)
			{
				dialogueTerminated();
			}
			if (DialogueManager.m_effectActive_EmberRing || DialogueManager.m_effectActive_Hallow)
			{
				MainCameraSettings.DisableEffectOverrides();
			}
		}

		// Token: 0x060038A3 RID: 14499 RVA: 0x0016E5CC File Offset: 0x0016C7CC
		public static void Choose(int index)
		{
			if (index < 0 || index >= DialogueManager.m_choices.Count)
			{
				Debug.LogError(string.Format("Choice index out of range?! Index: {0}, Quest: {1}, Source: {2}, Last line: {3}", new object[]
				{
					index,
					DialogueManager.m_currentQuest.name,
					DialogueManager.m_currentSource.name,
					DialogueManager.m_lines[DialogueManager.m_lines.Count - 1].Text
				}));
				return;
			}
			DialogueManager.m_story.ChooseChoiceIndex(index);
			DialogueManager.m_isChoice = true;
			DialogueManager.m_isChoiceText = true;
			DialogueManager.PumpStory();
			if (DialogueManager.m_storyActive)
			{
				Action dialogueUpdated = DialogueManager.DialogueUpdated;
				if (dialogueUpdated != null)
				{
					dialogueUpdated();
				}
			}
			DialogueManager.m_isChoice = false;
		}

		// Token: 0x060038A4 RID: 14500 RVA: 0x0016E67C File Offset: 0x0016C87C
		public static string BuildDialogText()
		{
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			for (int i = 0; i < DialogueManager.m_lines.Count; i++)
			{
				DialogLine dialogLine = DialogueManager.m_lines[i];
				if (!string.IsNullOrWhiteSpace(dialogLine.Text))
				{
					string str = (i == 0) ? "" : "\n";
					if (i > 0 && dialogLine.Flags.RemainOnSameLine(DialogueManager.m_lines[i - 1].Flags))
					{
						str = " ";
					}
					fromPool.Append(str + dialogLine.Text.Trim());
				}
			}
			return fromPool.ToString_ReturnToPool();
		}

		// Token: 0x060038A5 RID: 14501 RVA: 0x0016E714 File Offset: 0x0016C914
		public static void OnRewardChosen(UniqueId rewardChoiceId)
		{
			if (DialogueManager.m_shouldRestartAfterRewardChoice)
			{
				GameManager.QuestManager.QuestUpdated += DialogueManager.OnQuestUpdated;
			}
			ObjectiveIterationCache cache = new ObjectiveIterationCache
			{
				QuestId = DialogueManager.m_currentQuest.Id,
				ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(DialogueManager.m_rewardChoiceObjectiveHash),
				RewardChoiceId = rewardChoiceId,
				WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty),
				NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null),
				StartQuestIfNotPresent = DialogueManager.m_shouldStartNewAfterRewardChoice
			};
			if (DialogueManager.m_rewardIsReissue)
			{
				GameManager.QuestManager.ReissueReward(cache, LocalPlayer.GameEntity);
				DialogueManager.m_currentQuest = null;
			}
			else
			{
				GameManager.QuestManager.Progress(cache, null, false);
			}
			DialogueManager.m_shouldStartNewAfterRewardChoice = false;
			DialogueManager.m_shouldRestartAfterRewardChoice = false;
			DialogueManager.m_rewardChoiceObjectiveHash = 0;
			DialogueManager.m_rewardIsReissue = false;
		}

		// Token: 0x060038A6 RID: 14502 RVA: 0x0016E81C File Offset: 0x0016CA1C
		private static void RestartStory(InkEntry ink)
		{
			DialogueManager.m_story.onError -= DialogueManager.OnError;
			DialogueManager.m_story = new Story(ink.InkStory.text);
			DialogueManager.m_story.onError += DialogueManager.OnError;
			DialogueManager.InjectGlobals();
			DialogueManager.m_isChoice = true;
			try
			{
				DialogueManager.m_story.ChoosePathString(ink.TargetPath, true, Array.Empty<object>());
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Exception trying to access {0} for story {1}.\n{2}", ink.TargetPath, ink.InkStory.name, arg));
			}
		}

		// Token: 0x060038A7 RID: 14503 RVA: 0x0016E8C4 File Offset: 0x0016CAC4
		private static void PumpStory()
		{
			while (DialogueManager.m_story.canContinue)
			{
				DialogLine item;
				if (DialogueManager.TryGetLine(out item))
				{
					DialogueManager.m_lines.Add(item);
				}
			}
			DialogueManager.m_choices.Clear();
			List<Choice> generatedChoices = DialogueManager.m_story.state.generatedChoices;
			if (generatedChoices.Count > 0)
			{
				for (int i = 0; i < generatedChoices.Count; i++)
				{
					if (!generatedChoices[i].isInvisibleDefault)
					{
						generatedChoices[i].index = i;
						DialogueManager.m_choices.Add(generatedChoices[i]);
					}
				}
			}
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			RewardChoiceObjective rewardChoiceObjective;
			if (DialogueManager.m_currentQuest != null && DialogueManager.m_currentProfile != null && DialogueManager.m_currentQuest.TryGetProgress(out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByType<RewardChoiceObjective>(out questStep2, out rewardChoiceObjective) && !rewardChoiceObjective.WaitForDialogueTag)
			{
				DialogueManager.m_rewardChoiceObjectiveHash = rewardChoiceObjective.CombinedId(questStep2.Id);
				Action<Reward, bool> rewardAvailable = DialogueManager.RewardAvailable;
				if (rewardAvailable == null)
				{
					return;
				}
				rewardAvailable(rewardChoiceObjective.Reward, false);
			}
		}

		// Token: 0x060038A8 RID: 14504 RVA: 0x0016E9D8 File Offset: 0x0016CBD8
		private static bool TryGetLine(out DialogLine line)
		{
			line = default(DialogLine);
			if (!DialogueManager.m_story.canContinue)
			{
				return false;
			}
			string text = DialogueManager.m_story.Continue();
			DialogFlags dialogFlags = DialogueManager.m_isChoiceText ? DialogFlags.Player : DialogFlags.None;
			DialogueManager.m_isChoiceText = false;
			string text2 = null;
			string text3 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			foreach (string text4 in DialogueManager.m_story.currentTags)
			{
				if (string.IsNullOrEmpty(text2) && text4.Contains("QUEST="))
				{
					text2 = text4.After("QUEST=").Trim();
				}
				if (string.IsNullOrEmpty(text3) && text4.Contains("OBJECTIVE="))
				{
					text3 = text4.After("OBJECTIVE=").Trim();
				}
				if (string.IsNullOrEmpty(text3) && text4.Contains("OUTPUT="))
				{
					text3 = text4.After("OUTPUT=").Trim();
				}
				if (text4.Contains("MERCHANT"))
				{
					flag = true;
				}
				if (text4.Contains("ESSENCE"))
				{
					flag2 = true;
				}
				if (text4.Contains("RESTART"))
				{
					flag3 = true;
				}
				if (text4.Contains("DONT_RESTART"))
				{
					flag4 = true;
				}
				if (text4.Contains("EFFECT_EMBERRING"))
				{
					DialogueManager.m_effectActive_EmberRing = !DialogueManager.m_effectActive_EmberRing;
					MainCameraSettings.ToggleEffectOverride(CameraEffectTypes.EmberRing, DialogueManager.m_effectActive_EmberRing);
				}
				if (text4.Contains("EFFECT_HALLOW"))
				{
					DialogueManager.m_effectActive_Hallow = !DialogueManager.m_effectActive_Hallow;
					MainCameraSettings.ToggleEffectOverride(CameraEffectTypes.Hallow, DialogueManager.m_effectActive_Hallow);
				}
				if (!dialogFlags.HasBitFlag(DialogFlags.Emotive) && text4.Contains("EMOTIVE"))
				{
					dialogFlags = dialogFlags.AddBitFlag(DialogFlags.Emotive);
				}
				if (!dialogFlags.HasBitFlag(DialogFlags.Warning) && text4.Contains("WARNING"))
				{
					dialogFlags = dialogFlags.AddBitFlag(DialogFlags.Warning);
				}
				if (!dialogFlags.HasBitFlag(DialogFlags.StageDirection) && text4.Contains("STAGE"))
				{
					dialogFlags = dialogFlags.AddBitFlag(DialogFlags.StageDirection);
				}
				if (!dialogFlags.HasBitFlag(DialogFlags.ForceNewline) && text4.Contains("NEWLINE"))
				{
					dialogFlags = dialogFlags.AddBitFlag(DialogFlags.ForceNewline);
				}
			}
			Quest quest = null;
			if (!string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3) && GameManager.QuestManager.TryGetQuestByTag(text2, out quest))
			{
				if (quest.IsOnQuest(null))
				{
					DialogueManager.m_currentQuest = quest;
					if (DialogueManager.m_currentSource != null)
					{
						GameManager.QuestManager.TryGetDialogueState(DialogueManager.m_currentSource.Id, out DialogueManager.m_currentQuestDialogue);
					}
					if (DialogueManager.m_currentQuestDialogue == null)
					{
						goto IL_3A5;
					}
					using (List<InkEntry>.Enumerator enumerator2 = DialogueManager.m_currentQuestDialogue.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							InkEntry inkEntry = enumerator2.Current;
							if (inkEntry.Quest.Id == quest.Id)
							{
								if (!flag4)
								{
									DialogueManager.RestartStory(inkEntry);
								}
								line = new DialogLine(text, dialogFlags, DialogueManager.m_currentSourceType);
								return true;
							}
						}
						goto IL_3A5;
					}
				}
				if (quest.CanStartQuest(null))
				{
					DialogueManager.m_currentQuest = quest;
					ObjectiveIterationCache objectiveIterationCache = new ObjectiveIterationCache
					{
						QuestId = quest.Id,
						ObjectiveHashes = null,
						WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty),
						NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null),
						StartQuestIfNotPresent = true
					};
					ObjectiveIterationCache cache = objectiveIterationCache;
					if (!flag4)
					{
						GameManager.QuestManager.QuestUpdated += DialogueManager.OnQuestUpdated;
					}
					GameManager.QuestManager.Progress(cache, null, false);
				}
			}
			IL_3A5:
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			if (DialogueManager.m_currentQuest != null && !string.IsNullOrEmpty(text3) && string.IsNullOrEmpty(text2) && DialogueManager.m_currentQuest.TryGetProgress(out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep))
			{
				QuestStep questStep2 = null;
				QuestObjective questObjective = null;
				if (text3.Contains(','))
				{
					string[] array = text3.Split(',', StringSplitOptions.RemoveEmptyEntries);
					int[] array2 = new int[array.Length];
					Reward reward = null;
					int i = 0;
					while (i < array.Length)
					{
						if (questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(array[i], out questStep2, out questObjective))
						{
							RewardChoiceObjective rewardChoiceObjective = questObjective as RewardChoiceObjective;
							if (rewardChoiceObjective == null)
							{
								goto IL_483;
							}
							if (!(reward != null))
							{
								if (flag3)
								{
									DialogueManager.m_shouldRestartAfterRewardChoice = true;
								}
								DialogueManager.m_rewardChoiceObjectiveHash = rewardChoiceObjective.CombinedId(questStep2.Id);
								reward = rewardChoiceObjective.Reward;
								goto IL_483;
							}
							Debug.LogError("Multiple reward objective tags found, this we cannot do.");
							IL_4B3:
							i++;
							continue;
							IL_483:
							array2[i] = questObjective.CombinedId(questStep2.Id);
							goto IL_4B3;
						}
						Debug.LogError("Unable to process OBJECTIVE tag, failed objective lookup for tag \"" + array[i] + "\"!");
						return false;
					}
					if (reward != null)
					{
						Action<Reward, bool> rewardAvailable = DialogueManager.RewardAvailable;
						if (rewardAvailable != null)
						{
							rewardAvailable(reward, false);
						}
					}
					else
					{
						ObjectiveIterationCache objectiveIterationCache = new ObjectiveIterationCache
						{
							QuestId = DialogueManager.m_currentQuest.Id,
							ObjectiveHashes = array2,
							WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty),
							NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null)
						};
						ObjectiveIterationCache cache2 = objectiveIterationCache;
						if (flag3)
						{
							GameManager.QuestManager.QuestUpdated += DialogueManager.OnQuestUpdated;
						}
						GameManager.QuestManager.Progress(cache2, null, false);
					}
				}
				else if (questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(text3, out questStep2, out questObjective))
				{
					RewardChoiceObjective rewardChoiceObjective2 = questObjective as RewardChoiceObjective;
					if (rewardChoiceObjective2 != null)
					{
						if (flag3)
						{
							DialogueManager.m_shouldRestartAfterRewardChoice = true;
						}
						DialogueManager.m_rewardChoiceObjectiveHash = rewardChoiceObjective2.CombinedId(questStep2.Id);
						Action<Reward, bool> rewardAvailable2 = DialogueManager.RewardAvailable;
						if (rewardAvailable2 != null)
						{
							rewardAvailable2(rewardChoiceObjective2.Reward, false);
						}
					}
					else
					{
						ObjectiveIterationCache objectiveIterationCache = new ObjectiveIterationCache
						{
							QuestId = DialogueManager.m_currentQuest.Id,
							ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(questObjective.CombinedId(questStep2.Id)),
							WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty),
							NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null)
						};
						ObjectiveIterationCache cache3 = objectiveIterationCache;
						if (flag3)
						{
							GameManager.QuestManager.QuestUpdated += DialogueManager.OnQuestUpdated;
						}
						GameManager.QuestManager.Progress(cache3, null, false);
					}
				}
			}
			QuestStep questStep3;
			QuestObjective questObjective2;
			if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && GameManager.QuestManager.TryGetQuestByTag(text2, out quest) && quest.TryGetStartByObjectiveTag(text3, out questStep3, out questObjective2))
			{
				DialogueManager.m_currentQuest = quest;
				RewardChoiceObjective rewardChoiceObjective3 = questObjective2 as RewardChoiceObjective;
				if (rewardChoiceObjective3 != null)
				{
					if (flag3)
					{
						DialogueManager.m_shouldRestartAfterRewardChoice = true;
					}
					DialogueManager.m_shouldStartNewAfterRewardChoice = true;
					DialogueManager.m_rewardChoiceObjectiveHash = rewardChoiceObjective3.CombinedId(questStep3.Id);
					Action<Reward, bool> rewardAvailable3 = DialogueManager.RewardAvailable;
					if (rewardAvailable3 != null)
					{
						rewardAvailable3(rewardChoiceObjective3.Reward, false);
					}
				}
				else
				{
					ObjectiveIterationCache objectiveIterationCache = new ObjectiveIterationCache
					{
						QuestId = quest.Id,
						ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(questObjective2.CombinedId(questStep3.Id)),
						WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty),
						NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null),
						StartQuestIfNotPresent = true
					};
					ObjectiveIterationCache cache4 = objectiveIterationCache;
					if (flag3)
					{
						GameManager.QuestManager.QuestUpdated += DialogueManager.OnQuestUpdated;
					}
					GameManager.QuestManager.Progress(cache4, null, false);
				}
			}
			if (flag && DialogueManager.m_currentInteractive != null)
			{
				InteractiveMerchantNpc interactiveMerchantNpc = DialogueManager.m_currentInteractive as InteractiveMerchantNpc;
				if (interactiveMerchantNpc != null)
				{
					interactiveMerchantNpc.OpenMerchant();
				}
			}
			if (flag2 && DialogueManager.m_currentInteractive != null)
			{
				InteractiveEssenceConverterNpc interactiveEssenceConverterNpc = DialogueManager.m_currentInteractive as InteractiveEssenceConverterNpc;
				if (interactiveEssenceConverterNpc != null)
				{
					interactiveEssenceConverterNpc.OpenEssenceConverter();
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return false;
			}
			line = new DialogLine(text, dialogFlags, DialogueManager.m_currentSourceType);
			return true;
		}

		// Token: 0x060038A9 RID: 14505 RVA: 0x0016F284 File Offset: 0x0016D484
		private static object IsQuestEnabled(string tag)
		{
			Quest quest;
			if (GameManager.QuestManager.TryGetQuestByTag(tag, out quest))
			{
				return quest.Enabled;
			}
			return false;
		}

		// Token: 0x060038AA RID: 14506 RVA: 0x0016F2B4 File Offset: 0x0016D4B4
		private static object CanStartQuest(string tag)
		{
			Quest quest;
			if (GameManager.QuestManager.TryGetQuestByTag(tag, out quest))
			{
				return quest.CanStartQuest(null);
			}
			return false;
		}

		// Token: 0x060038AB RID: 14507 RVA: 0x0016F2E4 File Offset: 0x0016D4E4
		private static object IsOnQuest(string tag)
		{
			Quest quest;
			if (GameManager.QuestManager.TryGetQuestByTag(tag, out quest))
			{
				return quest.IsOnQuest(null);
			}
			return false;
		}

		// Token: 0x060038AC RID: 14508 RVA: 0x0016F314 File Offset: 0x0016D514
		private static object IsQuestComplete(string tag)
		{
			Quest quest;
			if (GameManager.QuestManager.TryGetQuestByTag(tag, out quest))
			{
				return quest.IsComplete(null);
			}
			return false;
		}

		// Token: 0x060038AD RID: 14509 RVA: 0x0016F344 File Offset: 0x0016D544
		private static object IsQuestObjectiveActive(string questTag, string objectiveTag)
		{
			Quest quest;
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (GameManager.QuestManager.TryGetQuestByTag(questTag, out quest) && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(quest.Id, out questProgressionData) && quest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(objectiveTag, out questStep2, out questObjective))
			{
				return true;
			}
			return false;
		}

		// Token: 0x060038AE RID: 14510 RVA: 0x0016F418 File Offset: 0x0016D618
		private static object IsObjectiveActive(string tag)
		{
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (DialogueManager.m_currentQuest != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(DialogueManager.m_currentQuest.Id, out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(tag, out questStep2, out questObjective))
			{
				return true;
			}
			return false;
		}

		// Token: 0x060038AF RID: 14511 RVA: 0x0016F4F4 File Offset: 0x0016D6F4
		private static object IsObjectiveComplete(string tag)
		{
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			ObjectiveProgressionData objectiveProgressionData;
			if (DialogueManager.m_currentQuest != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(DialogueManager.m_currentQuest.Id, out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(tag, out questStep2, out questObjective) && questProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData) && objectiveProgressionData.IterationsCompleted >= questObjective.IterationsRequired)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060038B0 RID: 14512 RVA: 0x0016F5F0 File Offset: 0x0016D7F0
		private static object HasLoot(string tag)
		{
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (DialogueManager.m_currentQuest != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(DialogueManager.m_currentQuest.Id, out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(tag, out questStep2, out questObjective))
			{
				LootObjective lootObjective = questObjective as LootObjective;
				if (lootObjective != null)
				{
					return lootObjective.HasLoot(LocalPlayer.GameEntity);
				}
			}
			return false;
		}

		// Token: 0x060038B1 RID: 14513 RVA: 0x0016F6E4 File Offset: 0x0016D8E4
		private static object HasLootForQuest(string questTag, string tag)
		{
			Quest quest;
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (GameManager.QuestManager.TryGetQuestByTag(questTag, out quest) && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(quest.Id, out questProgressionData) && quest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(tag, out questStep2, out questObjective))
			{
				LootObjective lootObjective = questObjective as LootObjective;
				if (lootObjective != null)
				{
					return lootObjective.HasLoot(LocalPlayer.GameEntity);
				}
			}
			Quest quest2;
			QuestObjective questObjective2;
			if (LocalPlayer.GameEntity && GameManager.QuestManager.TryGetQuestByTag(questTag, out quest2) && quest2.CanStartQuest(LocalPlayer.GameEntity) && quest2.TryGetStartByObjectiveTag(tag, out questStep2, out questObjective2))
			{
				LootObjective lootObjective2 = questObjective2 as LootObjective;
				if (lootObjective2 != null)
				{
					return lootObjective2.HasLoot(LocalPlayer.GameEntity);
				}
			}
			return false;
		}

		// Token: 0x060038B2 RID: 14514 RVA: 0x0016F828 File Offset: 0x0016DA28
		private static object HasCurrency(int required)
		{
			CurrencySources currencySources;
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.GetAvailableCurrency(out currencySources, CurrencySources.Inventory | CurrencySources.PersonalBank) >= (ulong)((long)required);
		}

		// Token: 0x060038B3 RID: 14515 RVA: 0x0016F860 File Offset: 0x0016DA60
		private static object HasCurrencyFor(string tag)
		{
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (DialogueManager.m_currentQuest != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(DialogueManager.m_currentQuest.Id, out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(tag, out questStep2, out questObjective))
			{
				PurchaseObjective purchaseObjective = questObjective as PurchaseObjective;
				if (purchaseObjective != null)
				{
					return purchaseObjective.HasEnoughMoney(LocalPlayer.GameEntity);
				}
			}
			return false;
		}

		// Token: 0x060038B4 RID: 14516 RVA: 0x000667D4 File Offset: 0x000649D4
		private static object HasEmberstone()
		{
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.CurrentEmberStone != null;
		}

		// Token: 0x060038B5 RID: 14517 RVA: 0x0006680B File Offset: 0x00064A0B
		private static object HasEssence(int required)
		{
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount() >= required;
		}

		// Token: 0x060038B6 RID: 14518 RVA: 0x0016F954 File Offset: 0x0016DB54
		private static object HasEssenceFor(string tag)
		{
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (DialogueManager.m_currentQuest != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(DialogueManager.m_currentQuest.Id, out questProgressionData) && DialogueManager.m_currentQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextObjectiveByDialogueTag<QuestObjective>(tag, out questStep2, out questObjective))
			{
				EmberEssenceObjective emberEssenceObjective = questObjective as EmberEssenceObjective;
				if (emberEssenceObjective != null)
				{
					return emberEssenceObjective.HasEnoughEssence(LocalPlayer.GameEntity);
				}
			}
			return false;
		}

		// Token: 0x060038B7 RID: 14519 RVA: 0x00066842 File Offset: 0x00064A42
		private static object Knows(string label)
		{
			if (LocalPlayer.GameEntity)
			{
				return LocalPlayer.GameEntity.CharacterData.Knows(label);
			}
			return false;
		}

		// Token: 0x060038B8 RID: 14520 RVA: 0x0016FA48 File Offset: 0x0016DC48
		private static void Learn(string label)
		{
			NpcProfile npcProfile;
			if (DialogueManager.m_currentNpcWorldObj != null)
			{
				IKnowledgeCapable knowledgeCapable = DialogueManager.m_currentNpcWorldObj as IKnowledgeCapable;
				if (knowledgeCapable != null && knowledgeCapable.KnowledgeHolder != null)
				{
					npcProfile = knowledgeCapable.KnowledgeHolder;
					goto IL_36;
				}
			}
			npcProfile = DialogueManager.m_currentProfile;
			IL_36:
			NpcProfile npcProfile2 = npcProfile;
			int knowledgeIndex;
			if (npcProfile2 != null && npcProfile2.TryGetKnowledgeIndexByLabel(label, out knowledgeIndex))
			{
				NpcLearningCache cache = new NpcLearningCache
				{
					NpcProfile = npcProfile2,
					KnowledgeIndex = knowledgeIndex,
					NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null),
					WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty)
				};
				GameManager.QuestManager.Learn(cache, null);
			}
		}

		// Token: 0x060038B9 RID: 14521 RVA: 0x0016FB1C File Offset: 0x0016DD1C
		private static object HasMastery(string name)
		{
			GameEntity gameEntity = LocalPlayer.GameEntity;
			bool flag;
			if (gameEntity == null)
			{
				flag = (null != null);
			}
			else
			{
				ContainerInstance masteries = gameEntity.CollectionController.Masteries;
				flag = (((masteries != null) ? masteries.Instances : null) != null);
			}
			if (flag)
			{
				using (IEnumerator<ArchetypeInstance> enumerator = LocalPlayer.GameEntity.CollectionController.Masteries.Instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Mastery.DisplayName == name)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060038BA RID: 14522 RVA: 0x0016FBB8 File Offset: 0x0016DDB8
		private static object HasAnyHarvestingMastery()
		{
			GameEntity gameEntity = LocalPlayer.GameEntity;
			bool flag;
			if (gameEntity == null)
			{
				flag = (null != null);
			}
			else
			{
				ContainerInstance masteries = gameEntity.CollectionController.Masteries;
				flag = (((masteries != null) ? masteries.Instances : null) != null);
			}
			if (flag)
			{
				using (IEnumerator<ArchetypeInstance> enumerator = LocalPlayer.GameEntity.CollectionController.Masteries.Instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Mastery.Type == MasteryType.Harvesting)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060038BB RID: 14523 RVA: 0x0016FC50 File Offset: 0x0016DE50
		private static object HasAnyTradeMastery()
		{
			GameEntity gameEntity = LocalPlayer.GameEntity;
			bool flag;
			if (gameEntity == null)
			{
				flag = (null != null);
			}
			else
			{
				ContainerInstance masteries = gameEntity.CollectionController.Masteries;
				flag = (((masteries != null) ? masteries.Instances : null) != null);
			}
			if (flag)
			{
				using (IEnumerator<ArchetypeInstance> enumerator = LocalPlayer.GameEntity.CollectionController.Masteries.Instances.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Mastery.Type == MasteryType.Trade)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060038BC RID: 14524 RVA: 0x0006686C File Offset: 0x00064A6C
		private static object HasAnyQuestDialogue()
		{
			return GameManager.QuestManager.TryGetDialogueState(DialogueManager.m_currentSource.Id, out DialogueManager.m_currentQuestDialogue);
		}

		// Token: 0x060038BD RID: 14525 RVA: 0x0016FCE8 File Offset: 0x0016DEE8
		private static object HasDialogueForQuest(string tag)
		{
			if (GameManager.QuestManager.TryGetDialogueState(DialogueManager.m_currentSource.Id, out DialogueManager.m_currentQuestDialogue))
			{
				using (List<InkEntry>.Enumerator enumerator = DialogueManager.m_currentQuestDialogue.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Quest.DialogueTag == tag)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060038BE RID: 14526 RVA: 0x0016FD70 File Offset: 0x0016DF70
		private static void LearnSpecialization(string specializationName)
		{
			RolePacked packed;
			if (LocalPlayer.GameEntity && Enum.TryParse<RolePacked>(specializationName, true, out packed))
			{
				SpecializedRole specializedRole = GlobalSettings.Values.Roles.GetRoleFromPacked(packed) as SpecializedRole;
				ArchetypeInstance archetypeInstance;
				if (specializedRole != null && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(specializedRole.GeneralRole.Id, out archetypeInstance))
				{
					LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.TrainSpecializationRequest(archetypeInstance.InstanceId, specializedRole.Id);
				}
			}
		}

		// Token: 0x060038BF RID: 14527 RVA: 0x0016FDF0 File Offset: 0x0016DFF0
		private static object HasReissuableReward(string questTag)
		{
			Quest quest;
			if (LocalPlayer.GameEntity && GameManager.QuestManager.TryGetQuestByTag(questTag, out quest))
			{
				return GameManager.QuestManager.HasReissuableReward(quest.Id, LocalPlayer.GameEntity);
			}
			return false;
		}

		// Token: 0x060038C0 RID: 14528 RVA: 0x0016FE3C File Offset: 0x0016E03C
		private static void ReissueReward(string questTag)
		{
			Quest quest;
			QuestStep questStep;
			RewardChoiceObjective rewardChoiceObjective;
			if (LocalPlayer.GameEntity && GameManager.QuestManager.TryGetQuestByTag(questTag, out quest) && quest.TryGetMostRecentReward(LocalPlayer.GameEntity, out questStep, out rewardChoiceObjective, null))
			{
				DialogueManager.m_currentQuest = quest;
				DialogueManager.m_rewardIsReissue = true;
				Action<Reward, bool> rewardAvailable = DialogueManager.RewardAvailable;
				if (rewardAvailable == null)
				{
					return;
				}
				rewardAvailable(rewardChoiceObjective.Reward, true);
			}
		}

		// Token: 0x060038C1 RID: 14529 RVA: 0x0016FE98 File Offset: 0x0016E098
		private static object IsOnQuestStep(string questId, string stepId)
		{
			QuestProgressionData questProgressionData;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.TryGetValue(new UniqueId(questId), out questProgressionData) && questProgressionData.CurrentNodeId == stepId)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060038C2 RID: 14530 RVA: 0x0016FF50 File Offset: 0x0016E150
		private static void StartQuest(string questTag)
		{
			Quest quest;
			if (LocalPlayer.GameEntity && GameManager.QuestManager.TryGetQuestByTag(questTag, out quest) && quest.CanStartQuest(null))
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = quest.Id,
					ObjectiveHashes = null,
					WorldId = (DialogueManager.m_currentNpcWorldObj ? DialogueManager.m_currentNpcWorldObj.WorldId : UniqueId.Empty),
					NpcEntity = ((DialogueManager.m_currentInteractive && DialogueManager.m_currentInteractive.GameEntity) ? DialogueManager.m_currentInteractive.GameEntity.NetworkEntity : null),
					StartQuestIfNotPresent = true
				}, LocalPlayer.GameEntity, false);
			}
		}

		// Token: 0x060038C3 RID: 14531 RVA: 0x0017001C File Offset: 0x0016E21C
		private static void Teleport()
		{
			GameObject gameObject = (DialogueManager.m_currentInteractive != null) ? DialogueManager.m_currentInteractive.gameObject : DialogueManager.m_currentNpcWorldObj.gameObject;
			InteractiveTeleporter interactiveTeleporter;
			if (gameObject != null && gameObject.TryGetComponent<InteractiveTeleporter>(out interactiveTeleporter))
			{
				interactiveTeleporter.Interact();
			}
		}

		// Token: 0x060038C4 RID: 14532 RVA: 0x00170068 File Offset: 0x0016E268
		private static void OnQuestUpdated(ObjectiveIterationCache cache)
		{
			GameManager.QuestManager.QuestUpdated -= DialogueManager.OnQuestUpdated;
			if (DialogueManager.m_currentSource != null)
			{
				GameManager.QuestManager.TryGetDialogueState(DialogueManager.m_currentSource.Id, out DialogueManager.m_currentQuestDialogue);
			}
			if (DialogueManager.m_currentQuestDialogue != null)
			{
				foreach (InkEntry inkEntry in DialogueManager.m_currentQuestDialogue)
				{
					if (inkEntry.Quest.Id == cache.QuestId)
					{
						DialogueManager.RestartStory(inkEntry);
						DialogueManager.PumpStory();
						Action dialogueUpdated = DialogueManager.DialogueUpdated;
						if (dialogueUpdated == null)
						{
							break;
						}
						dialogueUpdated();
						break;
					}
				}
			}
		}

		// Token: 0x060038C5 RID: 14533 RVA: 0x0017012C File Offset: 0x0016E32C
		private static void InjectGlobals()
		{
			if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("PLAYER_NAME"))
			{
				DialogueManager.m_story.variablesState["PLAYER_NAME"] = LocalPlayer.GameEntity.CharacterData.Name.Value;
			}
			BaseRole baseRole;
			if (InternalGameDatabase.Archetypes.TryGetAsType<BaseRole>(LocalPlayer.GameEntity.CharacterData.BaseRoleId, out baseRole))
			{
				if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("BASE_ROLE"))
				{
					DialogueManager.m_story.variablesState["BASE_ROLE"] = baseRole.DisplayName;
				}
				ArchetypeInstance archetypeInstance;
				bool flag = LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(LocalPlayer.GameEntity.CharacterData.BaseRoleId, out archetypeInstance);
				if (flag && archetypeInstance.MasteryData != null && DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("BASE_ROLE_LEVEL"))
				{
					DialogueManager.m_story.variablesState["BASE_ROLE_LEVEL"] = (int)archetypeInstance.MasteryData.BaseLevel;
				}
				SpecializedRole specializedRole;
				if (InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(LocalPlayer.GameEntity.CharacterData.SpecializedRoleId, out specializedRole))
				{
					if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("SPECIALIZATION"))
					{
						DialogueManager.m_story.variablesState["SPECIALIZATION"] = specializedRole.DisplayName;
					}
					if (flag && archetypeInstance.MasteryData != null && DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("SPECIALIZATION_LEVEL"))
					{
						DialogueManager.m_story.variablesState["SPECIALIZATION_LEVEL"] = (int)archetypeInstance.MasteryData.SpecializationLevel;
					}
				}
			}
			if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("REQUIRED_LEVEL_SPECIALIZATION"))
			{
				DialogueManager.m_story.variablesState["REQUIRED_LEVEL_SPECIALIZATION"] = 6f;
			}
			if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("MAX_LEVEL"))
			{
				DialogueManager.m_story.variablesState["MAX_LEVEL"] = 50f;
			}
			AbilityRecipeItem abilityRecipeItem;
			if (InternalGameDatabase.Archetypes.TryGetAsType<AbilityRecipeItem>(DialogueManager.m_sentinelOverprovisionRecipeId, out abilityRecipeItem))
			{
				if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("SENTINEL_OVERPROVISION_LEVEL"))
				{
					DialogueManager.m_story.variablesState["SENTINEL_OVERPROVISION_LEVEL"] = abilityRecipeItem.LevelRequirement;
				}
			}
			else
			{
				Debug.LogError("Unable to retrieve the AbilityRecipeItem for the Sentinel's Overprovision ability, has the ID changed?");
			}
			if (DialogueManager.m_story.variablesState.GlobalVariableExistsWithName("IS_DAY"))
			{
				DialogueManager.m_story.variablesState["IS_DAY"] = SkyDomeManager.IsDay();
			}
			DialogueManager.m_story.BindExternalFunction<string>("IS_QUEST_ENABLED", new Func<string, object>(DialogueManager.IsQuestEnabled), false);
			DialogueManager.m_story.BindExternalFunction<string>("CAN_START_QUEST", new Func<string, object>(DialogueManager.CanStartQuest), false);
			DialogueManager.m_story.BindExternalFunction<string>("IS_ON_QUEST", new Func<string, object>(DialogueManager.IsOnQuest), false);
			DialogueManager.m_story.BindExternalFunction<string>("IS_QUEST_COMPLETE", new Func<string, object>(DialogueManager.IsQuestComplete), false);
			DialogueManager.m_story.BindExternalFunction<string, string>("IS_QUEST_OBJECTIVE_ACTIVE", new Func<string, string, object>(DialogueManager.IsQuestObjectiveActive), false);
			DialogueManager.m_story.BindExternalFunction<string>("IS_OBJECTIVE_ACTIVE", new Func<string, object>(DialogueManager.IsObjectiveActive), false);
			DialogueManager.m_story.BindExternalFunction<string>("IS_OBJECTIVE_COMPLETE", new Func<string, object>(DialogueManager.IsObjectiveComplete), false);
			DialogueManager.m_story.BindExternalFunction<string>("HAS_LOOT", new Func<string, object>(DialogueManager.HasLoot), false);
			DialogueManager.m_story.BindExternalFunction<string, string>("HAS_LOOT_FOR_QUEST", new Func<string, string, object>(DialogueManager.HasLootForQuest), false);
			DialogueManager.m_story.BindExternalFunction<int>("HAS_CURRENCY", new Func<int, object>(DialogueManager.HasCurrency), false);
			DialogueManager.m_story.BindExternalFunction<string>("HAS_CURRENCY_FOR", new Func<string, object>(DialogueManager.HasCurrencyFor), false);
			DialogueManager.m_story.BindExternalFunction("HAS_EMBERSTONE", new Func<object>(DialogueManager.HasEmberstone), false);
			DialogueManager.m_story.BindExternalFunction<int>("HAS_ESSENCE", new Func<int, object>(DialogueManager.HasEssence), false);
			DialogueManager.m_story.BindExternalFunction<string>("HAS_ESSENCE_FOR", new Func<string, object>(DialogueManager.HasEssenceFor), false);
			DialogueManager.m_story.BindExternalFunction<string>("KNOWS", new Func<string, object>(DialogueManager.Knows), false);
			DialogueManager.m_story.BindExternalFunction<string>("LEARN", new Action<string>(DialogueManager.Learn), false);
			DialogueManager.m_story.BindExternalFunction<string>("HAS_MASTERY", new Func<string, object>(DialogueManager.HasMastery), false);
			DialogueManager.m_story.BindExternalFunction("HAS_ANY_HARVESTING_MASTERY", new Func<object>(DialogueManager.HasAnyHarvestingMastery), false);
			DialogueManager.m_story.BindExternalFunction("HAS_ANY_TRADE_MASTERY", new Func<object>(DialogueManager.HasAnyTradeMastery), false);
			DialogueManager.m_story.BindExternalFunction("HAS_ANY_QUEST_DIALOGUE", new Func<object>(DialogueManager.HasAnyQuestDialogue), false);
			DialogueManager.m_story.BindExternalFunction<string>("HAS_DIALOGUE_FOR_QUEST", new Func<string, object>(DialogueManager.HasDialogueForQuest), false);
			DialogueManager.m_story.BindExternalFunction<string>("LEARN_SPECIALIZATION", new Action<string>(DialogueManager.LearnSpecialization), false);
			DialogueManager.m_story.BindExternalFunction<string>("HAS_REISSUABLE_REWARD", new Func<string, object>(DialogueManager.HasReissuableReward), false);
			DialogueManager.m_story.BindExternalFunction<string>("REISSUE_REWARD", new Action<string>(DialogueManager.ReissueReward), false);
			DialogueManager.m_story.BindExternalFunction<string, string>("_IS_ON_QUEST_STEP", new Func<string, string, object>(DialogueManager.IsOnQuestStep), false);
			DialogueManager.m_story.BindExternalFunction<string>("START_QUEST", new Action<string>(DialogueManager.StartQuest), false);
			DialogueManager.m_story.BindExternalFunction("TELEPORT", new Action(DialogueManager.Teleport), false);
		}

		// Token: 0x060038C6 RID: 14534 RVA: 0x0006688C File Offset: 0x00064A8C
		private static void OnError(string message, ErrorType type)
		{
			Debug.Log("Ink runtime error: " + message);
		}

		// Token: 0x04003773 RID: 14195
		private static readonly UniqueId m_sentinelOverprovisionRecipeId = new UniqueId("a2c54f0510a01bd4ba15f0545d064475");

		// Token: 0x04003774 RID: 14196
		private const string kInkVar_PlayerName = "PLAYER_NAME";

		// Token: 0x04003775 RID: 14197
		private const string kInkVar_BaseRole = "BASE_ROLE";

		// Token: 0x04003776 RID: 14198
		private const string kInkVar_Specialization = "SPECIALIZATION";

		// Token: 0x04003777 RID: 14199
		private const string kInkVar_BaseRoleLevel = "BASE_ROLE_LEVEL";

		// Token: 0x04003778 RID: 14200
		private const string kInkVar_SpecializationLevel = "SPECIALIZATION_LEVEL";

		// Token: 0x04003779 RID: 14201
		private const string kInkVar_RequiredLevelSpecialization = "REQUIRED_LEVEL_SPECIALIZATION";

		// Token: 0x0400377A RID: 14202
		private const string kInkVar_MaxLevel = "MAX_LEVEL";

		// Token: 0x0400377B RID: 14203
		private const string kInkVar_SentinelOverprovisionLevel = "SENTINEL_OVERPROVISION_LEVEL";

		// Token: 0x0400377C RID: 14204
		private const string kInkVar_IsDay = "IS_DAY";

		// Token: 0x0400377D RID: 14205
		private const string kInkFunc_IsQuestEnabled = "IS_QUEST_ENABLED";

		// Token: 0x0400377E RID: 14206
		private const string kInkFunc_CanStartQuest = "CAN_START_QUEST";

		// Token: 0x0400377F RID: 14207
		private const string kInkFunc_IsOnQuest = "IS_ON_QUEST";

		// Token: 0x04003780 RID: 14208
		private const string kInkFunc_IsQuestComplete = "IS_QUEST_COMPLETE";

		// Token: 0x04003781 RID: 14209
		private const string kInkFunc_IsQuestObjectiveActive = "IS_QUEST_OBJECTIVE_ACTIVE";

		// Token: 0x04003782 RID: 14210
		private const string kInkFunc_IsObjectiveActive = "IS_OBJECTIVE_ACTIVE";

		// Token: 0x04003783 RID: 14211
		private const string kInkFunc_IsObjectiveComplete = "IS_OBJECTIVE_COMPLETE";

		// Token: 0x04003784 RID: 14212
		private const string kInkFunc_HasLoot = "HAS_LOOT";

		// Token: 0x04003785 RID: 14213
		private const string kInkFunc_HasLootForQuest = "HAS_LOOT_FOR_QUEST";

		// Token: 0x04003786 RID: 14214
		private const string kInkFunc_HasCurrency = "HAS_CURRENCY";

		// Token: 0x04003787 RID: 14215
		private const string kInkFunc_HasCurrencyFor = "HAS_CURRENCY_FOR";

		// Token: 0x04003788 RID: 14216
		private const string kInkFunc_HasEmberstone = "HAS_EMBERSTONE";

		// Token: 0x04003789 RID: 14217
		private const string kInkFunc_HasEssence = "HAS_ESSENCE";

		// Token: 0x0400378A RID: 14218
		private const string kInkFunc_HasEssenceFor = "HAS_ESSENCE_FOR";

		// Token: 0x0400378B RID: 14219
		private const string kInkFunc_Knows = "KNOWS";

		// Token: 0x0400378C RID: 14220
		private const string kInkFunc_Learn = "LEARN";

		// Token: 0x0400378D RID: 14221
		private const string kInkFunc_HasMastery = "HAS_MASTERY";

		// Token: 0x0400378E RID: 14222
		private const string kInkFunc_HasAnyHarvestingMastery = "HAS_ANY_HARVESTING_MASTERY";

		// Token: 0x0400378F RID: 14223
		private const string kInkFunc_HasAnyTradeMastery = "HAS_ANY_TRADE_MASTERY";

		// Token: 0x04003790 RID: 14224
		private const string kInkFunc_HasAnyQuestDialogue = "HAS_ANY_QUEST_DIALOGUE";

		// Token: 0x04003791 RID: 14225
		private const string kInkFunc_HasDialogueForQuest = "HAS_DIALOGUE_FOR_QUEST";

		// Token: 0x04003792 RID: 14226
		private const string kInkFunc_LearnSpecialization = "LEARN_SPECIALIZATION";

		// Token: 0x04003793 RID: 14227
		private const string kInkFunc_HasReissuableReward = "HAS_REISSUABLE_REWARD";

		// Token: 0x04003794 RID: 14228
		private const string kInkFunc_ReissueReward = "REISSUE_REWARD";

		// Token: 0x04003795 RID: 14229
		private const string kInkFunc_StartQuest = "START_QUEST";

		// Token: 0x04003796 RID: 14230
		private const string kInkFunc_Teleport = "TELEPORT";

		// Token: 0x04003797 RID: 14231
		private const string kInkFunc_IsOnQuestStep = "_IS_ON_QUEST_STEP";

		// Token: 0x0400379B RID: 14235
		private static Story m_story = null;

		// Token: 0x0400379C RID: 14236
		private static List<DialogLine> m_lines = new List<DialogLine>();

		// Token: 0x0400379D RID: 14237
		private static List<Choice> m_choices = new List<Choice>();

		// Token: 0x0400379E RID: 14238
		private static bool m_isChoice = false;

		// Token: 0x0400379F RID: 14239
		private static bool m_isChoiceText = false;

		// Token: 0x040037A0 RID: 14240
		private static bool m_storyActive = false;

		// Token: 0x040037A1 RID: 14241
		private static List<InkEntry> m_currentQuestDialogue = null;

		// Token: 0x040037A2 RID: 14242
		private static DialogueSource m_currentSource = null;

		// Token: 0x040037A3 RID: 14243
		private static NpcProfile m_currentProfile = null;

		// Token: 0x040037A4 RID: 14244
		private static BaseNetworkedInteractive m_currentInteractive = null;

		// Token: 0x040037A5 RID: 14245
		private static WorldObject m_currentNpcWorldObj = null;

		// Token: 0x040037A6 RID: 14246
		private static DialogSourceType m_currentSourceType = DialogSourceType.None;

		// Token: 0x040037A7 RID: 14247
		private static Quest m_currentQuest = null;

		// Token: 0x040037A8 RID: 14248
		private static bool m_shouldStartNewAfterRewardChoice = false;

		// Token: 0x040037A9 RID: 14249
		private static bool m_shouldRestartAfterRewardChoice = false;

		// Token: 0x040037AA RID: 14250
		private static int m_rewardChoiceObjectiveHash = 0;

		// Token: 0x040037AB RID: 14251
		private static bool m_rewardIsReissue = false;

		// Token: 0x040037AC RID: 14252
		private static bool m_effectActive_EmberRing = false;

		// Token: 0x040037AD RID: 14253
		private static bool m_effectActive_Hallow = false;
	}
}
