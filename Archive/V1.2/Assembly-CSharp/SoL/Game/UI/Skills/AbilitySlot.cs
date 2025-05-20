using System;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000922 RID: 2338
	public class AbilitySlot : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x17000F6B RID: 3947
		// (get) Token: 0x060044CC RID: 17612 RVA: 0x0006E7C4 File Offset: 0x0006C9C4
		public ContainerSlotUI ContainerSlotUi
		{
			get
			{
				return this.m_containerSlotUi;
			}
		}

		// Token: 0x17000F6C RID: 3948
		// (get) Token: 0x060044CD RID: 17613 RVA: 0x0006E7CC File Offset: 0x0006C9CC
		// (set) Token: 0x060044CE RID: 17614 RVA: 0x0006E7D4 File Offset: 0x0006C9D4
		public AbilityGrouping Column { get; set; }

		// Token: 0x17000F6D RID: 3949
		// (get) Token: 0x060044CF RID: 17615 RVA: 0x0006E7DD File Offset: 0x0006C9DD
		// (set) Token: 0x060044D0 RID: 17616 RVA: 0x0019DBC8 File Offset: 0x0019BDC8
		public AbilityArchetype Ability
		{
			get
			{
				return this.m_ability;
			}
			private set
			{
				if (this.m_archetypeInitialized && this.m_ability == value)
				{
					return;
				}
				this.m_ability = value;
				if (this.m_ability == null || this.AbilityInstance == null || this.AbilityInstance.Archetype == null || this.AbilityInstance.Archetype.Id != this.m_ability.Id)
				{
					this.AbilityInstance = null;
				}
				this.RefreshArchtype();
				this.m_archetypeInitialized = true;
			}
		}

		// Token: 0x17000F6E RID: 3950
		// (get) Token: 0x060044D1 RID: 17617 RVA: 0x0006E7E5 File Offset: 0x0006C9E5
		// (set) Token: 0x060044D2 RID: 17618 RVA: 0x0019DC54 File Offset: 0x0019BE54
		public ArchetypeInstance AbilityInstance
		{
			get
			{
				return this.m_abilityInstance;
			}
			set
			{
				if (this.m_instanceInitialized && this.m_abilityInstance == value)
				{
					this.RefreshOrnaments();
					return;
				}
				if (this.m_abilityInstance != null && this.m_abilityInstance.AbilityData != null)
				{
					this.m_abilityInstance.AbilityData.UsageCountChanged -= this.RefreshAlchemyDisplay;
				}
				this.m_abilityInstance = value;
				if (this.m_abilityInstance != null && this.m_abilityInstance.AbilityData != null)
				{
					this.m_abilityInstance.AbilityData.UsageCountChanged += this.RefreshAlchemyDisplay;
				}
				this.RefreshAlchemyDisplay();
				this.RefreshInstance();
				this.m_instanceInitialized = true;
			}
		}

		// Token: 0x060044D3 RID: 17619 RVA: 0x0019DCF8 File Offset: 0x0019BEF8
		private void RefreshArchtype()
		{
			if (this.Ability != null)
			{
				this.m_backgroundImage.overrideSprite = this.m_ability.Icon;
				this.m_backgroundImage.gameObject.SetActive(true);
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(true);
				}
			}
			else
			{
				this.m_backgroundImage.overrideSprite = null;
				this.m_backgroundImage.gameObject.SetActive(false);
				base.gameObject.SetActive(false);
			}
			this.RefreshLock();
		}

		// Token: 0x060044D4 RID: 17620 RVA: 0x0006E7ED File Offset: 0x0006C9ED
		private void AbilityLevelChanged()
		{
			this.RefreshInstance();
			this.Column.TriggerLockRefresh(this);
		}

		// Token: 0x060044D5 RID: 17621 RVA: 0x0006E801 File Offset: 0x0006CA01
		internal void RefreshAlchemyDisplay()
		{
			if (this.m_alchemyAbilitySlot)
			{
				this.m_alchemyAbilitySlot.RefreshProgressDisplay(this);
			}
		}

		// Token: 0x060044D6 RID: 17622 RVA: 0x0019DD84 File Offset: 0x0019BF84
		private void RefreshInstance()
		{
			bool active = false;
			this.m_progressParent.SetActive(active);
			this.RefreshLock();
			this.RefreshOrnaments();
		}

		// Token: 0x060044D7 RID: 17623 RVA: 0x0006E81C File Offset: 0x0006CA1C
		public void RefreshOrnaments()
		{
			this.m_ornamentController.Type = ((this.AbilityInstance != null && this.AbilityInstance.Index <= -1) ? OrnamentType.Metal2 : OrnamentType.None);
		}

		// Token: 0x060044D8 RID: 17624 RVA: 0x0006E843 File Offset: 0x0006CA43
		public void RefreshLock()
		{
			this.m_lock.RefreshLock();
		}

		// Token: 0x060044D9 RID: 17625 RVA: 0x0006E850 File Offset: 0x0006CA50
		public void AssignAbility(AbilityArchetype ability)
		{
			this.Ability = ability;
		}

		// Token: 0x060044DA RID: 17626 RVA: 0x0019DDAC File Offset: 0x0019BFAC
		internal ITooltipParameter GetAlchemyTooltipParameter(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (this.AbilityInstance != null)
			{
				return new ArchetypeTooltipParameter
				{
					Instance = this.AbilityInstance,
					Archetype = this.AbilityInstance.Archetype,
					AlchemyPowerLevel = new AlchemyPowerLevel?(alchemyPowerLevel)
				};
			}
			if (this.Ability != null)
			{
				return new ArchetypeTooltipParameter
				{
					Archetype = this.Ability,
					AlchemyPowerLevel = new AlchemyPowerLevel?(alchemyPowerLevel)
				};
			}
			return null;
		}

		// Token: 0x060044DB RID: 17627 RVA: 0x0006E859 File Offset: 0x0006CA59
		private ITooltipParameter GetTooltipParameters()
		{
			return this.GetAlchemyTooltipParameter(AlchemyPowerLevel.None);
		}

		// Token: 0x17000F6F RID: 3951
		// (get) Token: 0x060044DC RID: 17628 RVA: 0x0006E862 File Offset: 0x0006CA62
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameters);
			}
		}

		// Token: 0x17000F70 RID: 3952
		// (get) Token: 0x060044DD RID: 17629 RVA: 0x0006E870 File Offset: 0x0006CA70
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000F71 RID: 3953
		// (get) Token: 0x060044DE RID: 17630 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060044DF RID: 17631 RVA: 0x00049FFA File Offset: 0x000481FA
		public string FillActionsGetTitle()
		{
			return null;
		}

		// Token: 0x060044E0 RID: 17632 RVA: 0x0006E878 File Offset: 0x0006CA78
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocused = UIManager.IsChatActive;
		}

		// Token: 0x060044E1 RID: 17633 RVA: 0x0019DE34 File Offset: 0x0019C034
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			bool chatWasFocused = this.m_chatWasFocused;
			this.m_chatWasFocused = false;
			if (eventData.button == PointerEventData.InputButton.Left && ClientGameManager.InputManager.HoldingShift && chatWasFocused)
			{
				UIManager.ActiveChatInput.AddArchetypeLink(this.m_ability);
			}
		}

		// Token: 0x060044E3 RID: 17635 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004170 RID: 16752
		[SerializeField]
		private AbilitySlotLock m_lock;

		// Token: 0x04004171 RID: 16753
		[SerializeField]
		private WindowOrnamentController m_ornamentController;

		// Token: 0x04004172 RID: 16754
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004173 RID: 16755
		[SerializeField]
		private Image m_backgroundImage;

		// Token: 0x04004174 RID: 16756
		[SerializeField]
		private GameObject m_progressParent;

		// Token: 0x04004175 RID: 16757
		[SerializeField]
		private ContainerSlotUI m_containerSlotUi;

		// Token: 0x04004176 RID: 16758
		[SerializeField]
		private AlchemyAbilitySlot m_alchemyAbilitySlot;

		// Token: 0x04004177 RID: 16759
		private bool m_archetypeInitialized;

		// Token: 0x04004178 RID: 16760
		private bool m_instanceInitialized;

		// Token: 0x0400417A RID: 16762
		private AbilityArchetype m_ability;

		// Token: 0x0400417B RID: 16763
		private ArchetypeInstance m_abilityInstance;

		// Token: 0x0400417C RID: 16764
		private bool m_chatWasFocused;
	}
}
