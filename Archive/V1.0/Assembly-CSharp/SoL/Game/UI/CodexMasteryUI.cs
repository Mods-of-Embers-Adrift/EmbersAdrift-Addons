using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x02000861 RID: 2145
	public class CodexMasteryUI : MonoBehaviour, IContainerUI
	{
		// Token: 0x140000B8 RID: 184
		// (add) Token: 0x06003DEB RID: 15851 RVA: 0x00183F7C File Offset: 0x0018217C
		// (remove) Token: 0x06003DEC RID: 15852 RVA: 0x00183FB4 File Offset: 0x001821B4
		public event Action<ArchetypeInstance> InstanceAdded;

		// Token: 0x140000B9 RID: 185
		// (add) Token: 0x06003DED RID: 15853 RVA: 0x00183FEC File Offset: 0x001821EC
		// (remove) Token: 0x06003DEE RID: 15854 RVA: 0x00184024 File Offset: 0x00182224
		public event Action<ArchetypeInstance> InstanceRemoved;

		// Token: 0x17000E50 RID: 3664
		// (get) Token: 0x06003DEF RID: 15855 RVA: 0x00069E70 File Offset: 0x00068070
		// (set) Token: 0x06003DF0 RID: 15856 RVA: 0x00069E78 File Offset: 0x00068078
		public CodexAbilityUI CodexAbilityUI { get; private set; }

		// Token: 0x06003DF1 RID: 15857 RVA: 0x00069E81 File Offset: 0x00068081
		private void Awake()
		{
			this.CodexAbilityUI = base.gameObject.GetComponent<CodexAbilityUI>();
		}

		// Token: 0x06003DF2 RID: 15858 RVA: 0x00069E94 File Offset: 0x00068094
		public bool TryGetMasteryUI(UniqueId id, out MasteryAbilityContainerUI slot)
		{
			return this.m_masteryUIs.TryGetValue(id, out slot);
		}

		// Token: 0x06003DF3 RID: 15859 RVA: 0x0018405C File Offset: 0x0018225C
		public void RefreshAbilities()
		{
			for (int i = 0; i < this.m_masteryUIs.Count; i++)
			{
				this.m_masteryUIs[i].RefreshAbilities();
			}
		}

		// Token: 0x17000E51 RID: 3665
		// (get) Token: 0x06003DF4 RID: 15860 RVA: 0x00069EA3 File Offset: 0x000680A3
		public string Id
		{
			get
			{
				return this.m_container.Id;
			}
		}

		// Token: 0x17000E52 RID: 3666
		// (get) Token: 0x06003DF5 RID: 15861 RVA: 0x0004479C File Offset: 0x0004299C
		public bool Locked
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000E53 RID: 3667
		// (get) Token: 0x06003DF6 RID: 15862 RVA: 0x00069EB0 File Offset: 0x000680B0
		public bool Visible { get; }

		// Token: 0x06003DF7 RID: 15863 RVA: 0x0004475B File Offset: 0x0004295B
		void IContainerUI.CloseButtonPressed()
		{
		}

		// Token: 0x06003DF8 RID: 15864 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		bool IContainerUI.TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI)
		{
			slotUI = null;
			return false;
		}

		// Token: 0x06003DF9 RID: 15865 RVA: 0x00184090 File Offset: 0x00182290
		public void AddInstance(ArchetypeInstance instance)
		{
			AutoAttackAbility autoAttackAbility;
			if (instance.Archetype.TryGetAsType(out autoAttackAbility))
			{
				return;
			}
			MasteryAbilityContainerUI masteryAbilityContainerUI = null;
			if (!this.m_masteryUIs.TryGetValue(instance.ArchetypeId, out masteryAbilityContainerUI))
			{
				MasteryArchetype masteryArchetype;
				if (!instance.Archetype.TryGetAsType(out masteryArchetype))
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"INVALID MASTERY? ",
						instance.Archetype.DisplayName,
						" (",
						instance.ArchetypeId.ToString(),
						")"
					}));
					return;
				}
				MasteryAbilityContainerUI[] array = null;
				switch (masteryArchetype.Type)
				{
				case MasteryType.Combat:
					array = this.m_adventuringLeftDetails;
					break;
				case MasteryType.Utility:
					array = this.m_adventuringRightDetails;
					break;
				case MasteryType.Trade:
					array = this.m_craftingDetails;
					break;
				case MasteryType.Harvesting:
					array = this.m_harvestingDetails;
					break;
				}
				if (array == null)
				{
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Instance == null)
					{
						masteryAbilityContainerUI = array[i];
						break;
					}
				}
				if (masteryAbilityContainerUI == null)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"NO UI FOR MASTERY? ",
						instance.Archetype.DisplayName,
						" (",
						instance.ArchetypeId.ToString(),
						")"
					}));
					return;
				}
				this.m_masteryUIs.Add(instance.ArchetypeId, masteryAbilityContainerUI);
			}
			masteryAbilityContainerUI.InstanceAdded(instance);
			Action<ArchetypeInstance> instanceAdded = this.InstanceAdded;
			if (instanceAdded == null)
			{
				return;
			}
			instanceAdded(instance);
		}

		// Token: 0x06003DFA RID: 15866 RVA: 0x00184214 File Offset: 0x00182414
		public void RemoveInstance(ArchetypeInstance instance)
		{
			MasteryAbilityContainerUI masteryAbilityContainerUI;
			if (this.m_masteryUIs.TryGetValue(instance.ArchetypeId, out masteryAbilityContainerUI))
			{
				masteryAbilityContainerUI.InstanceRemoved(instance);
				this.m_masteryUIs.Remove(instance.ArchetypeId);
			}
			Action<ArchetypeInstance> instanceRemoved = this.InstanceRemoved;
			if (instanceRemoved == null)
			{
				return;
			}
			instanceRemoved(instance);
		}

		// Token: 0x06003DFB RID: 15867 RVA: 0x0004475B File Offset: 0x0004295B
		public void ItemsSwapped()
		{
		}

		// Token: 0x06003DFC RID: 15868 RVA: 0x00069EB8 File Offset: 0x000680B8
		public void Initialize(ContainerInstance containerInstance)
		{
			this.m_container = containerInstance;
		}

		// Token: 0x06003DFD RID: 15869 RVA: 0x0004475B File Offset: 0x0004295B
		public void PostInit()
		{
		}

		// Token: 0x06003DFE RID: 15870 RVA: 0x0004475B File Offset: 0x0004295B
		public void InstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06003DFF RID: 15871 RVA: 0x0004475B File Offset: 0x0004295B
		public void InteractWithInstance(ArchetypeInstance instance)
		{
		}

		// Token: 0x17000E54 RID: 3668
		// (get) Token: 0x06003E00 RID: 15872 RVA: 0x00069EC1 File Offset: 0x000680C1
		public ContainerInstance ContainerInstance
		{
			get
			{
				return this.m_container;
			}
		}

		// Token: 0x06003E01 RID: 15873 RVA: 0x0004475B File Offset: 0x0004295B
		public void Show()
		{
		}

		// Token: 0x06003E02 RID: 15874 RVA: 0x0004475B File Offset: 0x0004295B
		public void Hide()
		{
		}

		// Token: 0x06003E03 RID: 15875 RVA: 0x00069EC9 File Offset: 0x000680C9
		public bool IsLockedWithNotification()
		{
			return this.Locked;
		}

		// Token: 0x04003C5D RID: 15453
		[SerializeField]
		private MasteryAbilityContainerUI[] m_adventuringLeftDetails;

		// Token: 0x04003C5E RID: 15454
		[SerializeField]
		private MasteryAbilityContainerUI[] m_adventuringRightDetails;

		// Token: 0x04003C5F RID: 15455
		[SerializeField]
		private MasteryAbilityContainerUI[] m_harvestingDetails;

		// Token: 0x04003C60 RID: 15456
		[SerializeField]
		private MasteryAbilityContainerUI[] m_craftingDetails;

		// Token: 0x04003C61 RID: 15457
		private ContainerInstance m_container;

		// Token: 0x04003C62 RID: 15458
		private DictionaryList<UniqueId, MasteryAbilityContainerUI> m_masteryUIs = new DictionaryList<UniqueId, MasteryAbilityContainerUI>(default(UniqueIdComparer), true);
	}
}
