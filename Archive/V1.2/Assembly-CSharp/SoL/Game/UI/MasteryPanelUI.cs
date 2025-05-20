using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI
{
	// Token: 0x020008AD RID: 2221
	[Obsolete]
	public class MasteryPanelUI : MonoBehaviour, IContainerUI
	{
		// Token: 0x17000EE0 RID: 3808
		// (get) Token: 0x060040F4 RID: 16628 RVA: 0x0006BFC0 File Offset: 0x0006A1C0
		// (set) Token: 0x060040F5 RID: 16629 RVA: 0x0006BFC8 File Offset: 0x0006A1C8
		public AbilityPanelUI AbilityPanelUI { get; private set; }

		// Token: 0x060040F6 RID: 16630 RVA: 0x0006BFD1 File Offset: 0x0006A1D1
		private void Awake()
		{
			this.AbilityPanelUI = base.gameObject.GetComponent<AbilityPanelUI>();
		}

		// Token: 0x060040F7 RID: 16631 RVA: 0x0006BFE4 File Offset: 0x0006A1E4
		public bool TryGetMasteryUI(UniqueId id, out MasteryPanelSlotUI slot)
		{
			return this.m_masteryUIs.TryGetValue(id, out slot);
		}

		// Token: 0x060040F8 RID: 16632 RVA: 0x0018DEC0 File Offset: 0x0018C0C0
		public void RefreshAbilities()
		{
			for (int i = 0; i < this.m_masteryUIs.Count; i++)
			{
				this.m_masteryUIs[i].RefreshAbilities();
			}
		}

		// Token: 0x17000EE1 RID: 3809
		// (get) Token: 0x060040F9 RID: 16633 RVA: 0x0006BFF3 File Offset: 0x0006A1F3
		public string Id
		{
			get
			{
				return this.m_container.Id;
			}
		}

		// Token: 0x17000EE2 RID: 3810
		// (get) Token: 0x060040FA RID: 16634 RVA: 0x0004479C File Offset: 0x0004299C
		public bool Locked
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000EE3 RID: 3811
		// (get) Token: 0x060040FB RID: 16635 RVA: 0x0006C000 File Offset: 0x0006A200
		public bool Visible { get; }

		// Token: 0x060040FC RID: 16636 RVA: 0x0004475B File Offset: 0x0004295B
		void IContainerUI.CloseButtonPressed()
		{
		}

		// Token: 0x060040FD RID: 16637 RVA: 0x0018DEF4 File Offset: 0x0018C0F4
		public void AddInstance(ArchetypeInstance instance)
		{
			MasteryPanelSlotUI masteryPanelSlotUI = null;
			if (!this.m_masteryUIs.TryGetValue(instance.ArchetypeId, out masteryPanelSlotUI))
			{
				masteryPanelSlotUI = UnityEngine.Object.Instantiate<GameObject>(this.m_masteryPrefab, this.m_content).GetComponent<MasteryPanelSlotUI>();
				this.m_masteryUIs.Add(instance.ArchetypeId, masteryPanelSlotUI);
			}
			masteryPanelSlotUI.InstanceAdded(instance);
		}

		// Token: 0x060040FE RID: 16638 RVA: 0x0006C008 File Offset: 0x0006A208
		public void RemoveInstance(ArchetypeInstance instance)
		{
			Debug.Log("MASTERY INSTANCE REMOVED");
		}

		// Token: 0x060040FF RID: 16639 RVA: 0x0004475B File Offset: 0x0004295B
		public void ItemsSwapped()
		{
		}

		// Token: 0x06004100 RID: 16640 RVA: 0x0006C014 File Offset: 0x0006A214
		public void Initialize(ContainerInstance containerInstance)
		{
			this.m_container = containerInstance;
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x0004475B File Offset: 0x0004295B
		public void PostInit()
		{
		}

		// Token: 0x06004102 RID: 16642 RVA: 0x0004475B File Offset: 0x0004295B
		public void InstanceClicked(PointerEventData eventData, ArchetypeInstance instance)
		{
		}

		// Token: 0x06004103 RID: 16643 RVA: 0x0004475B File Offset: 0x0004295B
		public void InteractWithInstance(ArchetypeInstance instance)
		{
		}

		// Token: 0x17000EE4 RID: 3812
		// (get) Token: 0x06004104 RID: 16644 RVA: 0x0006C01D File Offset: 0x0006A21D
		public ContainerInstance ContainerInstance
		{
			get
			{
				return this.m_container;
			}
		}

		// Token: 0x06004105 RID: 16645 RVA: 0x0004475B File Offset: 0x0004295B
		public void Show()
		{
		}

		// Token: 0x06004106 RID: 16646 RVA: 0x0004475B File Offset: 0x0004295B
		public void Hide()
		{
		}

		// Token: 0x06004107 RID: 16647 RVA: 0x0006C025 File Offset: 0x0006A225
		public bool IsLockedWithNotification()
		{
			return this.Locked;
		}

		// Token: 0x06004108 RID: 16648 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		bool IContainerUI.TryGetContainerSlotUI(int index, out ContainerSlotUI slotUI)
		{
			slotUI = null;
			return false;
		}

		// Token: 0x04003E87 RID: 16007
		[SerializeField]
		private GameObject m_masteryPrefab;

		// Token: 0x04003E88 RID: 16008
		[SerializeField]
		private RectTransform m_content;

		// Token: 0x04003E8A RID: 16010
		private ContainerInstance m_container;

		// Token: 0x04003E8B RID: 16011
		private DictionaryList<UniqueId, MasteryPanelSlotUI> m_masteryUIs = new DictionaryList<UniqueId, MasteryPanelSlotUI>(default(UniqueIdComparer), true);
	}
}
