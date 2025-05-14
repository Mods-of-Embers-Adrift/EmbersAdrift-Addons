using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008B2 RID: 2226
	public class MasterySelectionBubblesUI : MonoBehaviour
	{
		// Token: 0x0600413C RID: 16700 RVA: 0x0018E9A0 File Offset: 0x0018CBA0
		private void Awake()
		{
			for (int i = 0; i < this.m_bubbles.Length; i++)
			{
				this.m_bubbles[i].Bubble.BubbleController = this;
			}
			this.m_boolHash = Animator.StringToHash("Retracted");
			this.m_animator.SetBool(this.m_boolHash, false);
			LocalPlayer.NetworkEntity.OnStartLocalClient += this.InitInternal;
		}

		// Token: 0x0600413D RID: 16701 RVA: 0x0018EA0C File Offset: 0x0018CC0C
		private void OnDestroy()
		{
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged -= this.OnSettingsChanged;
					LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged -= this.OnSettingsChanged;
				}
				if (LocalPlayer.GameEntity.CollectionController != null)
				{
					LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
					LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
				}
				if (LocalPlayer.GameEntity.VitalsReplicator)
				{
					LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
			}
		}

		// Token: 0x0600413E RID: 16702 RVA: 0x0018EAEC File Offset: 0x0018CCEC
		private void InitInternal()
		{
			this.RefreshMasteries();
			LocalPlayer.NetworkEntity.OnStartLocalClient -= this.InitInternal;
			LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged += this.OnSettingsChanged;
			LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged += this.OnSettingsChanged;
			LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
			LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
		}

		// Token: 0x0600413F RID: 16703 RVA: 0x0006C18F File Offset: 0x0006A38F
		private void MasteriesOnContentsChanged()
		{
			this.RefreshMasteries();
		}

		// Token: 0x06004140 RID: 16704 RVA: 0x0006C18F File Offset: 0x0006A38F
		private void EquipmentOnContentsChanged()
		{
			this.RefreshMasteries();
		}

		// Token: 0x06004141 RID: 16705 RVA: 0x0006C18F File Offset: 0x0006A38F
		private void OnSettingsChanged()
		{
			this.RefreshMasteries();
		}

		// Token: 0x06004142 RID: 16706 RVA: 0x0018EBAC File Offset: 0x0018CDAC
		private void CurrentStanceOnChanged(Stance obj)
		{
			bool flag = obj == Stance.Combat;
			this.m_animator.SetBool(this.m_boolHash, flag);
			this.m_currentStance = obj;
			for (int i = 0; i < this.m_bubbles.Length; i++)
			{
				this.m_bubbles[i].Bubble.ToggleLock(flag);
			}
		}

		// Token: 0x06004143 RID: 16707 RVA: 0x0006C197 File Offset: 0x0006A397
		private UniqueId GetActiveMastery()
		{
			return LocalPlayer.GameEntity.CharacterData.BaseRoleId;
		}

		// Token: 0x06004144 RID: 16708 RVA: 0x0018EC00 File Offset: 0x0018CE00
		private WeaponItem GetActiveWeapon()
		{
			ArchetypeInstance archetypeInstance;
			WeaponItem result;
			if (LocalPlayer.GameEntity.TryGetHandheldItem_MainHandAsType(out archetypeInstance, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06004145 RID: 16709 RVA: 0x0018EC20 File Offset: 0x0018CE20
		private void RefreshMasteries()
		{
			UniqueId activeMastery = this.GetActiveMastery();
			ContainerInstance masteries = LocalPlayer.GameEntity.CollectionController.Masteries;
			this.GetActiveWeapon();
			int num = 0;
			bool flag = this.m_currentStance == Stance.Combat;
			foreach (ArchetypeInstance archetypeInstance in masteries.Instances)
			{
				CombatMasteryArchetype combatMasteryArchetype;
				if (archetypeInstance.Archetype.TryGetAsType(out combatMasteryArchetype))
				{
					bool flag2 = combatMasteryArchetype.CanSelectMastery(LocalPlayer.GameEntity, !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive);
					bool locked = flag || !flag2;
					bool selected = combatMasteryArchetype.Id == activeMastery;
					this.m_bubbles[num].Arm.gameObject.SetActive(true);
					this.m_bubbles[num].Bubble.Init(archetypeInstance, combatMasteryArchetype, locked, selected);
					num++;
				}
			}
			for (int i = num; i < this.m_bubbles.Length; i++)
			{
				this.m_bubbles[i].Arm.gameObject.SetActive(false);
			}
		}

		// Token: 0x04003EA9 RID: 16041
		private const string kBoolName = "Retracted";

		// Token: 0x04003EAA RID: 16042
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04003EAB RID: 16043
		[SerializeField]
		private MasterySelectionBubblesUI.BubbleArm[] m_bubbles;

		// Token: 0x04003EAC RID: 16044
		private int m_boolHash;

		// Token: 0x04003EAD RID: 16045
		private Stance m_currentStance;

		// Token: 0x020008B3 RID: 2227
		[Serializable]
		private class BubbleArm
		{
			// Token: 0x04003EAE RID: 16046
			public Image Arm;

			// Token: 0x04003EAF RID: 16047
			public MasterySelectionBubble Bubble;
		}
	}
}
