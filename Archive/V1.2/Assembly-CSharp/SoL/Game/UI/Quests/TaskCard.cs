using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using SoL.Game.Discovery;
using SoL.Game.Interactives;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200094E RID: 2382
	public class TaskCard : MonoBehaviour, IContextMenu, IInteractiveBase, ICursor, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x140000D7 RID: 215
		// (add) Token: 0x06004670 RID: 18032 RVA: 0x001A3FC4 File Offset: 0x001A21C4
		// (remove) Token: 0x06004671 RID: 18033 RVA: 0x001A3FFC File Offset: 0x001A21FC
		public event Action<BBTaskType> CoverClicked;

		// Token: 0x140000D8 RID: 216
		// (add) Token: 0x06004672 RID: 18034 RVA: 0x001A4034 File Offset: 0x001A2234
		// (remove) Token: 0x06004673 RID: 18035 RVA: 0x001A406C File Offset: 0x001A226C
		public event Action<BBTask> TurnInClicked;

		// Token: 0x06004674 RID: 18036 RVA: 0x0006F60D File Offset: 0x0006D80D
		private void Start()
		{
			this.m_cover.onClick.AddListener(new UnityAction(this.OnCoverClicked));
			this.m_turnInButton.onClick.AddListener(new UnityAction(this.OnTurnInClicked));
		}

		// Token: 0x06004675 RID: 18037 RVA: 0x0006F647 File Offset: 0x0006D847
		private void OnDestroy()
		{
			this.m_cover.onClick.RemoveListener(new UnityAction(this.OnCoverClicked));
			this.m_turnInButton.onClick.RemoveListener(new UnityAction(this.OnTurnInClicked));
		}

		// Token: 0x06004676 RID: 18038 RVA: 0x0006F681 File Offset: 0x0006D881
		public void Init(bool onBoard, bool open, BBTaskType type, BBTask task)
		{
			this.m_onBoard = onBoard;
			this.m_open = open;
			this.m_type = type;
			this.m_task = task;
			this.RefreshVisuals();
		}

		// Token: 0x06004677 RID: 18039 RVA: 0x001A40A4 File Offset: 0x001A22A4
		public void RefreshVisuals()
		{
			this.RefreshHighlight();
			this.m_cover.gameObject.SetActive(this.m_onBoard && !this.m_open);
			if (this.m_onBoard && !this.m_open)
			{
				this.m_turnInButton.gameObject.SetActive(false);
				Image coverSeal = this.m_coverSeal;
				Sprite sprite;
				switch (this.m_type)
				{
				case BBTaskType.Adventuring:
					sprite = this.m_adventuringSeal;
					break;
				case BBTaskType.Crafting:
					sprite = this.m_craftingSeal;
					break;
				case BBTaskType.Gathering:
					sprite = this.m_gatheringSeal;
					break;
				default:
					sprite = this.m_adventuringSeal;
					break;
				}
				coverSeal.sprite = sprite;
				return;
			}
			switch (this.m_type)
			{
			case BBTaskType.Adventuring:
				this.m_categoryIcon.sprite = this.m_adventuringIcon;
				this.m_categoryIconTooltip.Text = "Adventuring";
				break;
			case BBTaskType.Crafting:
				this.m_categoryIcon.sprite = this.m_craftingIcon;
				this.m_categoryIconTooltip.Text = "Crafting";
				break;
			case BBTaskType.Gathering:
				this.m_categoryIcon.sprite = this.m_gatheringIcon;
				this.m_categoryIconTooltip.Text = "Gathering";
				break;
			default:
				this.m_categoryIcon.sprite = this.m_comingSoonIcon;
				this.m_categoryIconTooltip.Text = "Coming Soon...";
				break;
			}
			TextMeshProUGUI text = this.m_title;
			if (this.m_task == null)
			{
				this.m_masteryIconRibbon.gameObject.SetActive(false);
				this.m_masteryIcon.gameObject.SetActive(false);
				this.m_title.gameObject.SetActive(true);
				this.m_titleAlternate.gameObject.SetActive(false);
				if (this.m_categoryIcon.sprite == this.m_comingSoonIcon)
				{
					text.ZStringSetText("Coming Soon...");
				}
				else
				{
					text.ZStringSetText("No Active Task");
				}
				this.m_levelRange.ZStringSetText(string.Empty);
				this.m_description.ZStringSetText(string.Empty);
				this.m_objective.ZStringSetText(string.Empty);
				return;
			}
			if (this.m_task.AssociatedMastery != null)
			{
				this.m_masteryIconRibbon.gameObject.SetActive(true);
				this.m_masteryIcon.gameObject.SetActive(true);
				this.m_title.gameObject.SetActive(false);
				this.m_titleAlternate.gameObject.SetActive(true);
				text = this.m_titleAlternate;
				this.m_masteryIcon.sprite = this.m_task.AssociatedMastery.Icon;
				this.m_masteryIcon.color = this.m_task.AssociatedMastery.IconTint;
				this.m_masteryIconTooltip.Text = this.m_task.AssociatedMastery.DisplayName;
			}
			else
			{
				this.m_masteryIconRibbon.gameObject.SetActive(false);
				this.m_masteryIcon.gameObject.SetActive(false);
				this.m_title.gameObject.SetActive(true);
				this.m_titleAlternate.gameObject.SetActive(false);
			}
			text.SetTextFormat("{0}: {1}", this.m_task.Label.ToStringWithSpaces(), this.m_task.Title);
			this.m_levelRange.SetTextFormat("<link=\"{0}:Level Range\">{1}-{2}</link>", "text", this.m_task.LevelRange.x, this.m_task.LevelRange.y);
			this.m_description.ZStringSetText(this.m_task.Description);
			bool flag = this.m_task.IsReadyForTurnIn(null);
			this.m_turnInButton.gameObject.SetActive(this.m_onBoard && flag);
			if (this.m_task.OverrideObjectiveText)
			{
				this.m_objective.SetTextFormat("{0} {1}", flag ? TaskCard.kCompleteCheckbox : TaskCard.kIncompleteCheckbox, this.m_task.ObjectiveOverrideText);
				return;
			}
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			foreach (QuestObjective questObjective in this.m_task.Objectives)
			{
				if (questObjective.IsVisible(LocalPlayer.GameEntity) && (!this.m_task.HideCompletedObjectives || !questObjective.IsComplete(LocalPlayer.GameEntity)))
				{
					DiscoveryObjective discoveryObjective = questObjective as DiscoveryObjective;
					if (discoveryObjective != null)
					{
						List<DiscoveryProfile> list = discoveryObjective.ListUndiscovered(LocalPlayer.GameEntity);
						fromPool.AppendLine(((list.Count == 0) ? TaskCard.kCompleteCheckbox : TaskCard.kIncompleteCheckbox) + " " + questObjective.Description);
						using (List<DiscoveryProfile>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								DiscoveryProfile discoveryProfile = enumerator2.Current;
								fromPool.AppendLine("<indent=5%><sprite=\"SolIcons\" name=\"Circle\" tint=1> Discover: " + discoveryProfile.DisplayName + "</indent>");
							}
							continue;
						}
					}
					LootObjective lootObjective = questObjective as LootObjective;
					if (lootObjective != null)
					{
						BBTaskProgressionData bbtaskProgressionData;
						this.m_task.TryGetProgress(out bbtaskProgressionData);
						ObjectiveProgressionData objectiveProgressionData = null;
						bool flag2 = bbtaskProgressionData != null && bbtaskProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData) && objectiveProgressionData.IterationsCompleted >= questObjective.IterationsRequired;
						int num = flag2 ? lootObjective.AmountRequired : lootObjective.GetAvailableItems(LocalPlayer.GameEntity, null);
						fromPool.AppendLine((flag2 ? TaskCard.kCompleteCheckbox : TaskCard.kIncompleteCheckbox) + " " + questObjective.Description);
						if (num >= lootObjective.AmountRequired)
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
							Math.Min(num, lootObjective.AmountRequired),
							lootObjective.AmountRequired
						}));
					}
					else
					{
						EmberEssenceObjective emberEssenceObjective = questObjective as EmberEssenceObjective;
						BBTaskProgressionData bbtaskProgressionData2;
						ObjectiveProgressionData objectiveProgressionData2;
						if (emberEssenceObjective != null)
						{
							int emberEssenceCount = LocalPlayer.GameEntity.CollectionController.GetEmberEssenceCount();
							fromPool.AppendLine(string.Format("{0} {1} ({2}/{3})", new object[]
							{
								(Math.Min(emberEssenceCount, emberEssenceObjective.RequiredEssence) >= emberEssenceObjective.RequiredEssence) ? TaskCard.kCompleteCheckbox : TaskCard.kIncompleteCheckbox,
								questObjective.Description,
								Math.Min(emberEssenceCount, emberEssenceObjective.RequiredEssence),
								emberEssenceObjective.RequiredEssence
							}));
						}
						else if (this.m_task.TryGetProgress(out bbtaskProgressionData2) && bbtaskProgressionData2.TryGetObjective(questObjective.Id, out objectiveProgressionData2))
						{
							if (questObjective.IterationsRequired > 1)
							{
								fromPool.AppendLine(string.Format("{0} {1} ({2}/{3})", new object[]
								{
									(objectiveProgressionData2.IterationsCompleted >= questObjective.IterationsRequired) ? TaskCard.kCompleteCheckbox : TaskCard.kIncompleteCheckbox,
									questObjective.Description,
									objectiveProgressionData2.IterationsCompleted,
									questObjective.IterationsRequired
								}));
							}
							else
							{
								fromPool.AppendLine(TaskCard.kCompleteCheckbox + " " + questObjective.Description + " (Complete)");
							}
						}
						else if (questObjective.IterationsRequired > 1)
						{
							fromPool.AppendLine(string.Format("{0} {1} (0/{2})", TaskCard.kIncompleteCheckbox, questObjective.Description, questObjective.IterationsRequired));
						}
						else
						{
							fromPool.AppendLine(TaskCard.kIncompleteCheckbox + " " + questObjective.Description);
						}
					}
				}
			}
			this.m_objective.ZStringSetText(fromPool.ToString_ReturnToPool());
		}

		// Token: 0x06004678 RID: 18040 RVA: 0x0006F6A6 File Offset: 0x0006D8A6
		private void RefreshHighlight()
		{
			if (this.m_backgroundHighlight)
			{
				this.m_backgroundHighlight.enabled = (this.m_cursorInside && this.m_onBoard && !this.m_open);
			}
		}

		// Token: 0x06004679 RID: 18041 RVA: 0x0006F6DC File Offset: 0x0006D8DC
		private void OnCoverClicked()
		{
			Action<BBTaskType> coverClicked = this.CoverClicked;
			if (coverClicked == null)
			{
				return;
			}
			coverClicked(this.m_type);
		}

		// Token: 0x0600467A RID: 18042 RVA: 0x0006F6F4 File Offset: 0x0006D8F4
		private void OnTurnInClicked()
		{
			Action<BBTask> turnInClicked = this.TurnInClicked;
			if (turnInClicked == null)
			{
				return;
			}
			turnInClicked(this.m_task);
		}

		// Token: 0x0600467B RID: 18043 RVA: 0x00049FFA File Offset: 0x000481FA
		public string FillActionsGetTitle()
		{
			return null;
		}

		// Token: 0x17000FB2 RID: 4018
		// (get) Token: 0x0600467C RID: 18044 RVA: 0x0006F70C File Offset: 0x0006D90C
		public InteractionSettings Settings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000FB3 RID: 4019
		// (get) Token: 0x0600467D RID: 18045 RVA: 0x0006F714 File Offset: 0x0006D914
		CursorType ICursor.Type
		{
			get
			{
				if (!this.m_onBoard || this.m_open)
				{
					return CursorType.MainCursor;
				}
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x0600467E RID: 18046 RVA: 0x0006F729 File Offset: 0x0006D929
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_cursorInside = true;
			this.RefreshHighlight();
		}

		// Token: 0x0600467F RID: 18047 RVA: 0x0006F738 File Offset: 0x0006D938
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_cursorInside = false;
			this.RefreshHighlight();
		}

		// Token: 0x06004682 RID: 18050 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004280 RID: 17024
		[SerializeField]
		private Image m_categoryIcon;

		// Token: 0x04004281 RID: 17025
		[SerializeField]
		private TextTooltipTrigger m_categoryIconTooltip;

		// Token: 0x04004282 RID: 17026
		[SerializeField]
		private Image m_masteryIconRibbon;

		// Token: 0x04004283 RID: 17027
		[SerializeField]
		private Image m_masteryIcon;

		// Token: 0x04004284 RID: 17028
		[SerializeField]
		private TextTooltipTrigger m_masteryIconTooltip;

		// Token: 0x04004285 RID: 17029
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04004286 RID: 17030
		[SerializeField]
		private TextMeshProUGUI m_titleAlternate;

		// Token: 0x04004287 RID: 17031
		[SerializeField]
		private TextMeshProUGUI m_levelRange;

		// Token: 0x04004288 RID: 17032
		[SerializeField]
		private TextMeshProUGUI m_description;

		// Token: 0x04004289 RID: 17033
		[SerializeField]
		private TextMeshProUGUI m_objective;

		// Token: 0x0400428A RID: 17034
		[SerializeField]
		private SolButton m_turnInButton;

		// Token: 0x0400428B RID: 17035
		[SerializeField]
		private SolButton m_cover;

		// Token: 0x0400428C RID: 17036
		[SerializeField]
		private Image m_coverSeal;

		// Token: 0x0400428D RID: 17037
		[SerializeField]
		private Image m_backgroundHighlight;

		// Token: 0x0400428E RID: 17038
		[SerializeField]
		private Sprite m_adventuringIcon;

		// Token: 0x0400428F RID: 17039
		[SerializeField]
		private Sprite m_craftingIcon;

		// Token: 0x04004290 RID: 17040
		[SerializeField]
		private Sprite m_gatheringIcon;

		// Token: 0x04004291 RID: 17041
		[SerializeField]
		private Sprite m_comingSoonIcon;

		// Token: 0x04004292 RID: 17042
		[SerializeField]
		private Sprite m_adventuringSeal;

		// Token: 0x04004293 RID: 17043
		[SerializeField]
		private Sprite m_craftingSeal;

		// Token: 0x04004294 RID: 17044
		[SerializeField]
		private Sprite m_gatheringSeal;

		// Token: 0x04004295 RID: 17045
		[SerializeField]
		private InteractionSettings m_settings;

		// Token: 0x04004296 RID: 17046
		private bool m_onBoard;

		// Token: 0x04004297 RID: 17047
		private bool m_open;

		// Token: 0x04004298 RID: 17048
		private BBTaskType m_type;

		// Token: 0x04004299 RID: 17049
		private BBTask m_task;

		// Token: 0x0400429A RID: 17050
		private bool m_cursorInside;

		// Token: 0x0400429D RID: 17053
		private static readonly string kIncompleteCheckbox = "<alpha=#FF><font=\"Font Awesome 5 Free-Regular-400 SDF\"></font>";

		// Token: 0x0400429E RID: 17054
		private static readonly string kCompleteCheckbox = "<alpha=#FF><font=\"Font Awesome 5 Free-Solid-900 SDF\"></font><alpha=#AA>";
	}
}
