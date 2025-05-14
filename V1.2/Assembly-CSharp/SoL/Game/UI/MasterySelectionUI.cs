using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008B4 RID: 2228
	public class MasterySelectionUI : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu
	{
		// Token: 0x06004148 RID: 16712 RVA: 0x0006C1A8 File Offset: 0x0006A3A8
		private void Awake()
		{
			LocalPlayer.NetworkEntity.OnStartLocalClient += this.InitInternal;
		}

		// Token: 0x06004149 RID: 16713 RVA: 0x0018ED48 File Offset: 0x0018CF48
		private void OnDestroy()
		{
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged -= this.OnSettingsChanged;
				}
				if (LocalPlayer.GameEntity.CollectionController != null)
				{
					LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
					LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.OnEquipmentChanged;
				}
				if (LocalPlayer.GameEntity.VitalsReplicator)
				{
					LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
			}
		}

		// Token: 0x0600414A RID: 16714 RVA: 0x0018EE10 File Offset: 0x0018D010
		private void InitInternal()
		{
			LocalPlayer.NetworkEntity.OnStartLocalClient -= this.InitInternal;
			LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged += this.OnSettingsChanged;
			LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
			LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.OnEquipmentChanged;
			this.m_disabledOverlay.enabled = false;
			this.RefreshMastery(true);
		}

		// Token: 0x0600414B RID: 16715 RVA: 0x0018EEC4 File Offset: 0x0018D0C4
		private void MasteriesOnContentsChanged()
		{
			MasterySelectionUI.MasterySelectorType type = this.m_type;
			if (type <= MasterySelectionUI.MasterySelectorType.Secondary)
			{
				UniqueId baseRoleId = LocalPlayer.GameEntity.CharacterData.BaseRoleId;
				if (baseRoleId.IsEmpty)
				{
					if (LocalPlayer.GameEntity.CollectionController.Masteries.Count <= 0)
					{
						return;
					}
					using (IEnumerator<ArchetypeInstance> enumerator = LocalPlayer.GameEntity.CollectionController.Masteries.Instances.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CombatMasteryArchetype mastery;
							if (enumerator.Current.Archetype.TryGetAsType(out mastery))
							{
								this.SelectMastery(mastery, false);
								break;
							}
						}
						return;
					}
				}
				ArchetypeInstance archetypeInstance;
				if (!LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(baseRoleId, out archetypeInstance))
				{
					this.NullifyMastery();
				}
			}
		}

		// Token: 0x0600414C RID: 16716 RVA: 0x0006C1C0 File Offset: 0x0006A3C0
		private void OnEquipmentChanged()
		{
			this.RefreshMastery(false);
		}

		// Token: 0x0600414D RID: 16717 RVA: 0x0006C1C0 File Offset: 0x0006A3C0
		private void OnSettingsChanged()
		{
			this.RefreshMastery(false);
		}

		// Token: 0x0600414E RID: 16718 RVA: 0x0018EF8C File Offset: 0x0018D18C
		private void CurrentStanceOnChanged(Stance obj)
		{
			bool flag = obj == Stance.Combat;
			this.m_locked = flag;
			this.m_disabledOverlay.enabled = flag;
		}

		// Token: 0x0600414F RID: 16719 RVA: 0x0018EFB4 File Offset: 0x0018D1B4
		private void RefreshMastery(bool initial)
		{
			UniqueId activeMastery = this.GetActiveMastery();
			CombatMasteryArchetype combatMasteryArchetype = null;
			ArchetypeInstance masteryInstance = null;
			if (!activeMastery.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<CombatMasteryArchetype>(activeMastery, out combatMasteryArchetype) && combatMasteryArchetype.CanSelectMastery(LocalPlayer.GameEntity, this.IsPrimary()) && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(activeMastery, out masteryInstance))
			{
				this.m_masteryInstance = masteryInstance;
				this.SelectMastery(combatMasteryArchetype, initial);
				return;
			}
			using (IEnumerator<ArchetypeInstance> enumerator = LocalPlayer.GameEntity.CollectionController.Masteries.Instances.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Archetype.TryGetAsType(out combatMasteryArchetype) && combatMasteryArchetype.CanSelectMastery(LocalPlayer.GameEntity, this.IsPrimary()))
					{
						this.SelectMastery(combatMasteryArchetype, false);
						break;
					}
				}
			}
		}

		// Token: 0x06004150 RID: 16720 RVA: 0x0018F094 File Offset: 0x0018D294
		private bool IsPrimary()
		{
			switch (this.m_type)
			{
			case MasterySelectionUI.MasterySelectorType.Primary:
				return true;
			case MasterySelectionUI.MasterySelectorType.Secondary:
				return false;
			case MasterySelectionUI.MasterySelectorType.ActionBar:
				return !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			default:
				throw new ArgumentException("m_type");
			}
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x0006C197 File Offset: 0x0006A397
		private UniqueId GetActiveMastery()
		{
			return LocalPlayer.GameEntity.CharacterData.BaseRoleId;
		}

		// Token: 0x06004152 RID: 16722 RVA: 0x0018F0E0 File Offset: 0x0018D2E0
		private WeaponItem GetActiveWeapon()
		{
			EquipmentSlot index = EquipmentSlot.None;
			switch (this.m_type)
			{
			case MasterySelectionUI.MasterySelectorType.Primary:
				index = EquipmentSlot.PrimaryWeapon_MainHand;
				break;
			case MasterySelectionUI.MasterySelectorType.Secondary:
				index = EquipmentSlot.SecondaryWeapon_MainHand;
				break;
			case MasterySelectionUI.MasterySelectorType.ActionBar:
				index = (LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_MainHand : EquipmentSlot.PrimaryWeapon_MainHand);
				break;
			}
			ArchetypeInstance archetypeInstance;
			WeaponItem result;
			if (LocalPlayer.GameEntity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06004153 RID: 16723 RVA: 0x0018F154 File Offset: 0x0018D354
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_masteryInstance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_masteryInstance
			};
		}

		// Token: 0x17000EEF RID: 3823
		// (get) Token: 0x06004154 RID: 16724 RVA: 0x0006C1C9 File Offset: 0x0006A3C9
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EF0 RID: 3824
		// (get) Token: 0x06004155 RID: 16725 RVA: 0x0006C1D7 File Offset: 0x0006A3D7
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000EF1 RID: 3825
		// (get) Token: 0x06004156 RID: 16726 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x0018F188 File Offset: 0x0018D388
		public string FillActionsGetTitle()
		{
			if (this.m_locked)
			{
				return null;
			}
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Masteries, out containerInstance))
			{
				UniqueId activeMastery = this.GetActiveMastery();
				foreach (ArchetypeInstance archetypeInstance in containerInstance.Instances)
				{
					CombatMasteryArchetype mastery;
					if (archetypeInstance.Archetype.TryGetAsType(out mastery) && mastery.Type != MasteryType.Trade && mastery.Type != MasteryType.Harvesting)
					{
						int associatedLevelInteger = archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity);
						ContextMenuUI.AddContextAction((activeMastery == mastery.Id) ? (mastery.DisplayName + " [" + associatedLevelInteger.ToString() + "] <size=50%>(SELECTED)</size>") : (mastery.DisplayName + " [" + associatedLevelInteger.ToString() + "]"), activeMastery != mastery.Id && mastery.CanSelectMastery(LocalPlayer.GameEntity, this.IsPrimary()), delegate()
						{
							this.SelectMastery(mastery, false);
						}, null, null);
					}
				}
			}
			return "Select Mastery";
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x0006C1DF File Offset: 0x0006A3DF
		private void SelectMastery(CombatMasteryArchetype mastery, bool initial)
		{
			this.m_currentMastery = mastery;
			this.m_icon.sprite = mastery.Icon;
		}

		// Token: 0x06004159 RID: 16729 RVA: 0x0006C1F9 File Offset: 0x0006A3F9
		private void NullifyMastery()
		{
			this.m_currentMastery = null;
			this.m_icon.sprite = null;
		}

		// Token: 0x0600415B RID: 16731 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003EB0 RID: 16048
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003EB1 RID: 16049
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003EB2 RID: 16050
		[SerializeField]
		private Image m_disabledOverlay;

		// Token: 0x04003EB3 RID: 16051
		[SerializeField]
		private MasterySelectionUI.MasterySelectorType m_type;

		// Token: 0x04003EB4 RID: 16052
		private ArchetypeInstance m_masteryInstance;

		// Token: 0x04003EB5 RID: 16053
		private MasteryArchetype m_currentMastery;

		// Token: 0x04003EB6 RID: 16054
		private bool m_locked;

		// Token: 0x020008B5 RID: 2229
		private enum MasterySelectorType
		{
			// Token: 0x04003EB8 RID: 16056
			Primary,
			// Token: 0x04003EB9 RID: 16057
			Secondary,
			// Token: 0x04003EBA RID: 16058
			ActionBar
		}
	}
}
