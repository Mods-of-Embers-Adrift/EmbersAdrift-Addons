using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Objects;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000948 RID: 2376
	public class QuestLogUI : MonoBehaviour
	{
		// Token: 0x17000FAC RID: 4012
		// (get) Token: 0x06004634 RID: 17972 RVA: 0x0006F3A5 File Offset: 0x0006D5A5
		public Quest SelectedQuest
		{
			get
			{
				return this.m_questCategoriesList.SelectedItem;
			}
		}

		// Token: 0x06004635 RID: 17973 RVA: 0x001A2958 File Offset: 0x001A0B58
		protected void Start()
		{
			if (this.m_initialDetailContentHeight == null)
			{
				this.m_initialDetailContentHeight = new float?(this.m_detailContentRect.rect.height);
			}
			this.m_questCategoriesList.SelectionChanged += this.OnSelectionChanged;
			LootObjective.LootAmountChanged += this.OnLootAmountChanged;
			DiscoveryProgression.DiscoveryFound += this.OnDiscoveriesChanged;
			GameManager.QuestManager.QuestsUpdated += this.OnQuestsUpdated;
			this.m_muteButton.onClick.AddListener(new UnityAction(this.OnMuteClicked));
			this.m_questCategoriesList.PlayerPrefsKey = this.PlayerPrefsKey + "_Category";
			if (LocalPlayer.IsInitialized)
			{
				this.OnLocalPlayerInitialized();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x06004636 RID: 17974 RVA: 0x001A2A38 File Offset: 0x001A0C38
		protected void OnDestroy()
		{
			this.m_questCategoriesList.SelectionChanged -= this.OnSelectionChanged;
			LootObjective.LootAmountChanged -= this.OnLootAmountChanged;
			DiscoveryProgression.DiscoveryFound -= this.OnDiscoveriesChanged;
			GameManager.QuestManager.QuestsUpdated -= this.OnQuestsUpdated;
			this.m_muteButton.onClick.RemoveListener(new UnityAction(this.OnMuteClicked));
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.OnEmberStoneChanged;
			}
			this.m_questCategoriesList.FullyInitialized -= this.UpdateList;
		}

		// Token: 0x06004637 RID: 17975 RVA: 0x0006F3B2 File Offset: 0x0006D5B2
		public void Show()
		{
			this.UpdateListWhenReady();
			this.RefreshVisuals();
		}

		// Token: 0x06004638 RID: 17976 RVA: 0x0006F3C0 File Offset: 0x0006D5C0
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.OnEmberStoneChanged;
		}

		// Token: 0x06004639 RID: 17977 RVA: 0x0006F3EE File Offset: 0x0006D5EE
		private void OnSelectionChanged(Category<Quest> category, Quest quest)
		{
			this.RefreshVisuals();
		}

		// Token: 0x0600463A RID: 17978 RVA: 0x0006F3EE File Offset: 0x0006D5EE
		private void OnLootAmountChanged(LootObjective objective)
		{
			this.RefreshVisuals();
		}

		// Token: 0x0600463B RID: 17979 RVA: 0x0006F3EE File Offset: 0x0006D5EE
		private void OnDiscoveriesChanged(DiscoveryProfile discovery)
		{
			this.RefreshVisuals();
		}

		// Token: 0x0600463C RID: 17980 RVA: 0x0006F3EE File Offset: 0x0006D5EE
		private void OnEmberStoneChanged()
		{
			this.RefreshVisuals();
		}

		// Token: 0x0600463D RID: 17981 RVA: 0x0006F3B2 File Offset: 0x0006D5B2
		private void OnQuestsUpdated()
		{
			this.UpdateListWhenReady();
			this.RefreshVisuals();
		}

		// Token: 0x0600463E RID: 17982 RVA: 0x001A2AF0 File Offset: 0x001A0CF0
		public void RefreshVisuals()
		{
			if (this.SelectedQuest == null)
			{
				this.m_noSelection.gameObject.SetActive(true);
				this.m_questDetail.SetActive(false);
				return;
			}
			this.m_noSelection.gameObject.SetActive(false);
			this.m_questDetail.SetActive(true);
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			if (LocalPlayer.GameEntity && this.SelectedQuest.TryGetProgress(out questProgressionData) && this.SelectedQuest.TryGetStep(questProgressionData.CurrentNodeId, out questStep))
			{
				this.m_title.text = this.SelectedQuest.Title;
				this.m_logEntry.text = questStep.LogEntry;
				bool flag = questStep.Next.Count == 0;
				this.m_objectiveSection.gameObject.SetActive(!flag);
				if (!flag && questStep.NextSteps != null)
				{
					StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
					for (int i = 0; i < questStep.NextSteps.Count; i++)
					{
						if (questStep.NextSteps[i].HasVisibleObjective(LocalPlayer.GameEntity))
						{
							if (fromPool.Length > 0 && !questStep.CombineBranchObjectiveLists)
							{
								fromPool.AppendLine("\nOR\n");
							}
							List<QuestObjective> fromPool2 = StaticListPool<QuestObjective>.GetFromPool();
							foreach (QuestObjective questObjective in questStep.NextSteps[i].Objectives)
							{
								if (!fromPool2.Contains(questObjective))
								{
									int num = 1;
									int num2 = 0;
									if (questStep.NextSteps[i].CombineLikeObjectives)
									{
										foreach (QuestObjective questObjective2 in questStep.NextSteps[i].Objectives)
										{
											if (questObjective2.Id != questObjective.Id && questObjective2.IterationsRequired == 1 && questObjective2.Description == questObjective.Description)
											{
												fromPool2.Add(questObjective2);
												num++;
												ObjectiveProgressionData objectiveProgressionData;
												if (questProgressionData.TryGetObjective(questObjective2.Id, out objectiveProgressionData) && objectiveProgressionData.IterationsCompleted > 0)
												{
													num2++;
												}
											}
										}
									}
									if (questObjective.IsVisible(LocalPlayer.GameEntity) && ((!this.SelectedQuest.HideCompletedObjectives && !questStep.HideCompletedObjectives) || !questObjective.IsComplete(LocalPlayer.GameEntity)))
									{
										DiscoveryObjective discoveryObjective = questObjective as DiscoveryObjective;
										if (discoveryObjective != null)
										{
											List<DiscoveryProfile> list = discoveryObjective.ListUndiscovered(LocalPlayer.GameEntity);
											fromPool.AppendLine(((list.Count == 0) ? QuestLogUI.kCompleteCheckbox : QuestLogUI.kIncompleteCheckbox) + " " + questObjective.Description);
											using (List<DiscoveryProfile>.Enumerator enumerator3 = list.GetEnumerator())
											{
												while (enumerator3.MoveNext())
												{
													DiscoveryProfile discoveryProfile = enumerator3.Current;
													fromPool.AppendLine("<indent=5%><sprite=\"SolIcons\" name=\"Circle\" tint=1> Discover: " + discoveryProfile.DisplayName + "</indent>");
												}
												continue;
											}
										}
										LootObjective lootObjective = questObjective as LootObjective;
										if (lootObjective != null)
										{
											if (lootObjective.DropLocations.Count > 0)
											{
												ObjectiveProgressionData objectiveProgressionData2 = null;
												bool flag2 = questProgressionData != null && questProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData2) && objectiveProgressionData2.IterationsCompleted >= questObjective.IterationsRequired;
												int num3 = flag2 ? lootObjective.AmountRequired : lootObjective.GetAvailableItems(LocalPlayer.GameEntity, null);
												fromPool.AppendLine((flag2 ? QuestLogUI.kCompleteCheckbox : QuestLogUI.kIncompleteCheckbox) + " " + questObjective.Description);
												if (num3 >= lootObjective.AmountRequired)
												{
													fromPool.Append("<alpha=#AA>");
												}
												else
												{
													fromPool.Append("<alpha=#FF>");
												}
												fromPool.AppendLine(string.Format("<indent=5%>{0} {1} ({2}/{3})</indent>", new object[]
												{
													"<sprite=\"SolIcons\" name=\"Circle\" tint=1>",
													lootObjective.BuildObjectiveDescription(),
													Math.Min(num3, lootObjective.AmountRequired),
													lootObjective.AmountRequired
												}));
											}
											else
											{
												int availableItems = lootObjective.GetAvailableItems(LocalPlayer.GameEntity, null);
												fromPool.AppendLine(((Math.Min(availableItems, lootObjective.AmountRequired) >= lootObjective.AmountRequired) ? QuestLogUI.kCompleteCheckbox : QuestLogUI.kIncompleteCheckbox) + " " + questObjective.Description);
												fromPool.AppendLine(string.Format("<indent=5%>{0} {1} ({2}/{3})</indent>", new object[]
												{
													"<sprite=\"SolIcons\" name=\"Circle\" tint=1>",
													lootObjective.BuildObjectiveDescription(),
													Math.Min(availableItems, lootObjective.AmountRequired),
													lootObjective.AmountRequired
												}));
											}
										}
										else
										{
											EmberEssenceObjective emberEssenceObjective = questObjective as EmberEssenceObjective;
											ObjectiveProgressionData objectiveProgressionData3;
											if (emberEssenceObjective != null)
											{
												int emberEssenceCount = LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount();
												fromPool.AppendLine(string.Format("{0} {1} ({2}/{3})", new object[]
												{
													(Math.Min(emberEssenceCount, emberEssenceObjective.RequiredEssence) >= emberEssenceObjective.RequiredEssence) ? QuestLogUI.kCompleteCheckbox : QuestLogUI.kIncompleteCheckbox,
													questObjective.Description,
													Math.Min(emberEssenceCount, emberEssenceObjective.RequiredEssence),
													emberEssenceObjective.RequiredEssence
												}));
											}
											else if (questProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData3))
											{
												if (questObjective.IterationsRequired > 1)
												{
													fromPool.AppendLine(string.Format("{0} {1} ({2}/{3})", new object[]
													{
														(objectiveProgressionData3.IterationsCompleted >= questObjective.IterationsRequired) ? QuestLogUI.kCompleteCheckbox : QuestLogUI.kIncompleteCheckbox,
														questObjective.Description,
														objectiveProgressionData3.IterationsCompleted,
														questObjective.IterationsRequired
													}));
												}
												else if (questStep.NextSteps[i].CombineLikeObjectives && num > 0)
												{
													int num4 = num2 + objectiveProgressionData3.IterationsCompleted;
													fromPool.AppendLine(string.Format("{0} {1} ({2}/{3})", new object[]
													{
														(num4 >= num) ? QuestLogUI.kCompleteCheckbox : QuestLogUI.kIncompleteCheckbox,
														questObjective.Description,
														num4,
														num
													}));
												}
												else
												{
													fromPool.AppendLine(QuestLogUI.kCompleteCheckbox + " " + questObjective.Description + " (Complete)");
												}
											}
											else if (questObjective.IterationsRequired > 1)
											{
												fromPool.AppendLine(string.Format("{0} {1} (0/{2})", QuestLogUI.kIncompleteCheckbox, questObjective.Description, questObjective.IterationsRequired));
											}
											else if (questStep.NextSteps[i].CombineLikeObjectives && num > 0)
											{
												fromPool.AppendLine(string.Format("{0} {1} ({2}/{3})", new object[]
												{
													QuestLogUI.kIncompleteCheckbox,
													questObjective.Description,
													num2,
													num
												}));
											}
											else
											{
												fromPool.AppendLine(QuestLogUI.kIncompleteCheckbox + " " + questObjective.Description);
											}
										}
									}
								}
							}
							StaticListPool<QuestObjective>.ReturnToPool(fromPool2);
						}
					}
					this.m_objectives.ZStringSetText(fromPool.ToString_ReturnToPool());
					this.m_objectives.ForceMeshUpdate(false, false);
					this.m_detailContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Math.Max(this.m_initialDetailContentHeight.Value, this.m_objectives.preferredHeight + 270f));
					this.m_muteButton.image.sprite = (questProgressionData.Muted ? this.m_unmuteSprite : this.m_muteSprite);
					return;
				}
				this.m_detailContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.m_initialDetailContentHeight.Value);
			}
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x0006F3F6 File Offset: 0x0006D5F6
		private void UpdateListWhenReady()
		{
			if (this.m_questCategoriesList.IsInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_questCategoriesList.Initialized += this.UpdateList;
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x001A3300 File Offset: 0x001A1500
		private void UpdateList()
		{
			List<Category<Quest>> fromPool = StaticListPool<Category<Quest>>.GetFromPool();
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.Count > 0)
			{
				List<Quest> list = null;
				List<Quest> list2 = null;
				List<Quest> list3 = null;
				foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests)
				{
					Quest quest;
					if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.Enabled && (quest.StartHints & ObjectiveBehaviorHint.Hidden) == ObjectiveBehaviorHint.None)
					{
						if (quest.IsComplete(null))
						{
							if (list3 == null)
							{
								list3 = StaticListPool<Quest>.GetFromPool();
							}
							list3.Add(quest);
						}
						else if (keyValuePair.Value.Muted)
						{
							if (list2 == null)
							{
								list2 = StaticListPool<Quest>.GetFromPool();
							}
							list2.Add(quest);
						}
						else
						{
							if (list == null)
							{
								list = StaticListPool<Quest>.GetFromPool();
							}
							list.Add(quest);
						}
					}
				}
				if (list != null)
				{
					fromPool.Add(new Category<Quest>
					{
						Name = "Active",
						Data = list
					});
				}
				if (list2 != null)
				{
					fromPool.Add(new Category<Quest>
					{
						Name = "Muted",
						Data = list2
					});
				}
				if (list3 != null)
				{
					fromPool.Add(new Category<Quest>
					{
						Name = "Completed",
						Data = list3
					});
				}
				this.m_questCategoriesList.UpdateCategories(fromPool);
				this.m_questCategoriesList.ReindexItems(this.SelectedQuest);
				if (list != null)
				{
					StaticListPool<Quest>.ReturnToPool(list);
				}
				if (list2 != null)
				{
					StaticListPool<Quest>.ReturnToPool(list2);
				}
				if (list3 != null)
				{
					StaticListPool<Quest>.ReturnToPool(list3);
				}
			}
			else
			{
				this.m_questCategoriesList.UpdateCategories(fromPool);
			}
			StaticListPool<Category<Quest>>.ReturnToPool(fromPool);
		}

		// Token: 0x06004641 RID: 17985 RVA: 0x001A353C File Offset: 0x001A173C
		private void OnMuteClicked()
		{
			QuestProgressionData questProgressionData;
			if (this.SelectedQuest.TryGetProgress(out questProgressionData))
			{
				GameManager.QuestManager.MuteQuest(LocalPlayer.GameEntity, this.SelectedQuest.Id, !questProgressionData.Muted);
				this.m_muteButton.image.sprite = (questProgressionData.Muted ? this.m_unmuteSprite : this.m_muteSprite);
				this.UpdateListWhenReady();
			}
		}

		// Token: 0x04004253 RID: 16979
		[SerializeField]
		private QuestCategoriesList m_questCategoriesList;

		// Token: 0x04004254 RID: 16980
		[SerializeField]
		private TextMeshProUGUI m_noSelection;

		// Token: 0x04004255 RID: 16981
		[SerializeField]
		private GameObject m_questDetail;

		// Token: 0x04004256 RID: 16982
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04004257 RID: 16983
		[SerializeField]
		private TextMeshProUGUI m_logEntry;

		// Token: 0x04004258 RID: 16984
		[SerializeField]
		private RectTransform m_detailContentRect;

		// Token: 0x04004259 RID: 16985
		[SerializeField]
		private RectTransform m_objectiveSection;

		// Token: 0x0400425A RID: 16986
		[SerializeField]
		private TextMeshProUGUI m_objectives;

		// Token: 0x0400425B RID: 16987
		[SerializeField]
		private SolButton m_muteButton;

		// Token: 0x0400425C RID: 16988
		[SerializeField]
		private Sprite m_muteSprite;

		// Token: 0x0400425D RID: 16989
		[SerializeField]
		private Sprite m_unmuteSprite;

		// Token: 0x0400425E RID: 16990
		private float? m_initialDetailContentHeight;

		// Token: 0x0400425F RID: 16991
		public string PlayerPrefsKey = string.Empty;

		// Token: 0x04004260 RID: 16992
		private static string kIncompleteCheckbox = "<alpha=#FF><font=\"Font Awesome 5 Free-Regular-400 SDF\"></font>";

		// Token: 0x04004261 RID: 16993
		private static string kCompleteCheckbox = "<alpha=#FF><font=\"Font Awesome 5 Free-Solid-900 SDF\"></font><alpha=#AA>";
	}
}
