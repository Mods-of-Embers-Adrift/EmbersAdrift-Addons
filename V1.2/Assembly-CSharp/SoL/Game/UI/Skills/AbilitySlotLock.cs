using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000923 RID: 2339
	public class AbilitySlotLock : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu
	{
		// Token: 0x060044E4 RID: 17636 RVA: 0x0006E885 File Offset: 0x0006CA85
		private void LearnAbility()
		{
			ArchetypeInstance abilityInstance = this.m_slot.AbilityInstance;
		}

		// Token: 0x060044E5 RID: 17637 RVA: 0x0006E893 File Offset: 0x0006CA93
		private void TrainAbility()
		{
			bool trained = this.m_slot.AbilityInstance.AbilityData.Trained;
		}

		// Token: 0x060044E6 RID: 17638 RVA: 0x0019DE78 File Offset: 0x0019C078
		public void RefreshLock()
		{
			if (this.m_slot.Ability == null)
			{
				base.gameObject.SetActive(true);
				this.m_learnOverlayParent.SetActive(false);
				this.m_trainOverlayParent.SetActive(false);
				return;
			}
			bool active = false;
			if (this.m_slot.AbilityInstance == null)
			{
				this.m_learnIcon.color = ((LocalPlayer.GameEntity && LocalPlayer.GameEntity.IsAtTrainer() && this.m_slot.Ability.CanLearn(LocalPlayer.GameEntity)) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				this.m_learnOverlayParent.SetActive(true);
				this.m_trainOverlayParent.SetActive(false);
				active = true;
			}
			else if (!this.m_slot.AbilityInstance.AbilityData.Trained)
			{
				this.m_trainIcon.color = ((LocalPlayer.GameEntity && this.m_slot.Ability.CanTrain(LocalPlayer.GameEntity)) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				this.m_learnOverlayParent.SetActive(false);
				this.m_trainOverlayParent.SetActive(true);
				active = true;
			}
			base.gameObject.SetActive(active);
		}

		// Token: 0x060044E7 RID: 17639 RVA: 0x0019DFA8 File Offset: 0x0019C1A8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_slot.AbilityInstance == null)
			{
				string text = string.Empty;
				bool flag = false;
				bool flag2 = false;
				string empty = string.Empty;
				if (LocalPlayer.GameEntity)
				{
					flag = LocalPlayer.GameEntity.IsAtTrainer();
					flag2 = this.m_slot.Ability.CanLearnWithDetails(LocalPlayer.GameEntity, out empty, flag);
				}
				if (flag && flag2)
				{
					ArchetypeTooltipParameter archetypeTooltipParameter = new ArchetypeTooltipParameter
					{
						AdditionalText = "<size=120%>Unknown Ability</size>\nRight Click and select LEARN".Color(UIManager.RequirementsMetColor),
						Archetype = this.m_slot.Ability
					};
					return archetypeTooltipParameter;
				}
				if (flag && !flag2)
				{
					string text2 = string.IsNullOrEmpty(empty) ? "<size=120%>Unknown Ability</size>" : ("<size=120%>Unknown Ability</size>\n" + empty);
					ArchetypeTooltipParameter archetypeTooltipParameter = new ArchetypeTooltipParameter
					{
						AdditionalText = text2.Color(UIManager.RequirementsNotMetColor),
						Archetype = this.m_slot.Ability
					};
					return archetypeTooltipParameter;
				}
				if (flag2)
				{
					text += "Unknown Ability\nMust be at trainer!".Color(UIManager.RequirementsNotMetColor);
				}
				else if (!string.IsNullOrEmpty(empty))
				{
					text += empty.Color(UIManager.RequirementsNotMetColor);
				}
				return new ObjectTextTooltipParameter(this, text, false);
			}
			else
			{
				if (!this.m_slot.AbilityInstance.AbilityData.Trained)
				{
					string text3 = "<size=120%>Untrained Ability</size>";
					string text4;
					bool flag3 = this.m_slot.Ability.CanTrainWithDetails(LocalPlayer.GameEntity, out text4);
					if (flag3)
					{
						text3 += "\nRight click and select TRAIN";
					}
					else if (!string.IsNullOrEmpty(text4))
					{
						text3 = text3 + "\n" + text4;
					}
					ArchetypeTooltipParameter archetypeTooltipParameter = new ArchetypeTooltipParameter
					{
						AdditionalText = text3.Color(flag3 ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor),
						Instance = this.m_slot.AbilityInstance
					};
					return archetypeTooltipParameter;
				}
				return null;
			}
		}

		// Token: 0x17000F72 RID: 3954
		// (get) Token: 0x060044E8 RID: 17640 RVA: 0x0006E8AB File Offset: 0x0006CAAB
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F73 RID: 3955
		// (get) Token: 0x060044E9 RID: 17641 RVA: 0x0006E8B9 File Offset: 0x0006CAB9
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060044EA RID: 17642 RVA: 0x0019E184 File Offset: 0x0019C384
		public string FillActionsGetTitle()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Masteries == null || this.m_slot == null || this.m_slot.Ability == null)
			{
				return null;
			}
			if (this.m_slot.AbilityInstance == null)
			{
				if (LocalPlayer.GameEntity.IsAtTrainer())
				{
					bool enabled = this.m_slot.Ability.CanLearn(LocalPlayer.GameEntity);
					ContextMenuUI.AddContextAction("Learn Ability", enabled, new Action(this.LearnAbility), null, null);
					return this.m_slot.Ability.DisplayName;
				}
				return null;
			}
			else
			{
				if (!this.m_slot.AbilityInstance.AbilityData.Trained)
				{
					bool enabled2 = this.m_slot.Ability.CanTrain(LocalPlayer.GameEntity);
					ContextMenuUI.AddContextAction("Train Ability", enabled2, new Action(this.TrainAbility), null, null);
					return this.m_slot.Ability.DisplayName;
				}
				return null;
			}
		}

		// Token: 0x17000F74 RID: 3956
		// (get) Token: 0x060044EB RID: 17643 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060044ED RID: 17645 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400417D RID: 16765
		[SerializeField]
		private AbilitySlot m_slot;

		// Token: 0x0400417E RID: 16766
		[SerializeField]
		private GameObject m_learnOverlayParent;

		// Token: 0x0400417F RID: 16767
		[SerializeField]
		private Image m_learnIcon;

		// Token: 0x04004180 RID: 16768
		[SerializeField]
		private GameObject m_trainOverlayParent;

		// Token: 0x04004181 RID: 16769
		[SerializeField]
		private Image m_trainIcon;

		// Token: 0x04004182 RID: 16770
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
