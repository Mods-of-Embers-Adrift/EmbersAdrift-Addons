using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008D6 RID: 2262
	public class StanceSelectionBubble : MonoBehaviour, ITooltip, IInteractiveBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000F10 RID: 3856
		// (get) Token: 0x0600421F RID: 16927 RVA: 0x0006CA14 File Offset: 0x0006AC14
		public Stance Stance
		{
			get
			{
				return this.m_stance;
			}
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x001919FC File Offset: 0x0018FBFC
		private void Awake()
		{
			this.m_highlight.enabled = false;
			this.m_rectToAnimate = base.gameObject.transform.parent.GetComponent<RectTransform>();
			this.m_retractedHeight = this.m_rectToAnimate.sizeDelta.y;
			this.m_targetHeight = this.m_retractedHeight;
			this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x00191A70 File Offset: 0x0018FC70
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
			if (this.m_stance == Stance.Combat)
			{
				LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged -= this.RefreshMasteryIcon;
				LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged -= this.RefreshMasteryIcon;
			}
		}

		// Token: 0x06004222 RID: 16930 RVA: 0x00191AD8 File Offset: 0x0018FCD8
		private void Update()
		{
			float num = Mathf.MoveTowards(this.m_rectToAnimate.sizeDelta.y, this.m_targetHeight, Time.deltaTime * GlobalSettings.Values.Stance.StanceBubbleAnimationSpeed);
			if (this.m_rectToAnimate.sizeDelta.y != num)
			{
				this.m_rectToAnimate.sizeDelta = new Vector2(this.m_rectToAnimate.sizeDelta.x, num);
			}
		}

		// Token: 0x06004223 RID: 16931 RVA: 0x00191B4C File Offset: 0x0018FD4C
		public void Init(StanceSelectionBubbleController controller, Stance stance, Sprite icon)
		{
			this.m_controller = controller;
			this.m_stance = stance;
			this.m_button.interactable = true;
			this.m_backgroundIcon.sprite = icon;
			this.m_fullIcon.sprite = null;
			Color color = this.m_fullIcon.color;
			color.a = 0f;
			this.m_fullIcon.color = color;
			this.m_toggleController.Toggle(false);
			switch (this.m_stance)
			{
			default:
				if (this.m_stance == Stance.Combat)
				{
					LocalPlayer.NetworkEntity.OnStartLocalClient += this.NetworkEntityOnOnStartLocalClient;
				}
				return;
			}
		}

		// Token: 0x06004224 RID: 16932 RVA: 0x00191BF8 File Offset: 0x0018FDF8
		private void NetworkEntityOnOnStartLocalClient()
		{
			LocalPlayer.NetworkEntity.OnStartLocalClient -= this.NetworkEntityOnOnStartLocalClient;
			LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged += this.RefreshMasteryIcon;
			LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged += this.RefreshMasteryIcon;
			this.RefreshMasteryIcon();
		}

		// Token: 0x06004225 RID: 16933 RVA: 0x0006CA1C File Offset: 0x0006AC1C
		private void ButtonClicked()
		{
			this.m_controller.BubbleClicked(this);
		}

		// Token: 0x06004226 RID: 16934 RVA: 0x0006CA2A File Offset: 0x0006AC2A
		public void SetState(bool active)
		{
			this.m_targetHeight = (active ? this.m_extendedHeight : this.m_retractedHeight);
			this.m_toggleController.Toggle(active);
		}

		// Token: 0x06004227 RID: 16935 RVA: 0x0006CA4F File Offset: 0x0006AC4F
		public void ToggleInteractive(bool interactive)
		{
			this.m_button.interactable = interactive;
		}

		// Token: 0x06004228 RID: 16936 RVA: 0x00191C58 File Offset: 0x0018FE58
		private ITooltipParameter GetTooltipParameter()
		{
			int num = -1;
			switch (this.m_stance)
			{
			case Stance.Combat:
				num = 10;
				break;
			case Stance.Torch:
				num = 12;
				break;
			case Stance.Sit:
				num = 11;
				break;
			}
			string text = "<i>Not currently bound.</i>";
			if (num >= 0)
			{
				string primaryBindingNameForAction = SolInput.Mapper.GetPrimaryBindingNameForAction(num);
				if (!string.IsNullOrEmpty(primaryBindingNameForAction))
				{
					text = ZString.Format<string>("<i>{0} to toggle.</i>", primaryBindingNameForAction);
				}
			}
			if (this.m_combatMasteryInstance != null)
			{
				return new ArchetypeTooltipParameter
				{
					Instance = this.m_combatMasteryInstance,
					AdditionalText = text
				};
			}
			string txt = ZString.Format<Stance, string>("{0}\n{1}", this.m_stance, text);
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000F11 RID: 3857
		// (get) Token: 0x06004229 RID: 16937 RVA: 0x0006CA5D File Offset: 0x0006AC5D
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F12 RID: 3858
		// (get) Token: 0x0600422A RID: 16938 RVA: 0x0006CA6B File Offset: 0x0006AC6B
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000F13 RID: 3859
		// (get) Token: 0x0600422B RID: 16939 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600422C RID: 16940 RVA: 0x0006C197 File Offset: 0x0006A397
		private UniqueId GetActiveMastery()
		{
			return LocalPlayer.GameEntity.CharacterData.BaseRoleId;
		}

		// Token: 0x0600422D RID: 16941 RVA: 0x00191D0C File Offset: 0x0018FF0C
		private WeaponItem GetActiveWeapon()
		{
			EquipmentSlot index = LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_MainHand : EquipmentSlot.PrimaryWeapon_MainHand;
			ArchetypeInstance archetypeInstance;
			WeaponItem result;
			if (LocalPlayer.GameEntity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x00191D5C File Offset: 0x0018FF5C
		private void RefreshMasteryIcon()
		{
			UniqueId activeMastery = this.GetActiveMastery();
			Color color = this.m_fullIcon.color;
			CombatMasteryArchetype combatMasteryArchetype;
			if (InternalGameDatabase.Archetypes.TryGetAsType<CombatMasteryArchetype>(activeMastery, out combatMasteryArchetype))
			{
				this.m_fullIcon.overrideSprite = combatMasteryArchetype.Icon;
				color.a = 1f;
				LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(combatMasteryArchetype.Id, out this.m_combatMasteryInstance);
			}
			else
			{
				this.m_fullIcon.overrideSprite = null;
				color.a = 0f;
				this.m_combatMasteryInstance = null;
			}
			this.m_backgroundIcon.enabled = (this.m_fullIcon.overrideSprite == null);
			this.m_fullIcon.color = color;
		}

		// Token: 0x0600422F RID: 16943 RVA: 0x0006CA73 File Offset: 0x0006AC73
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = true;
			}
		}

		// Token: 0x06004230 RID: 16944 RVA: 0x0006CA8F File Offset: 0x0006AC8F
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = false;
			}
		}

		// Token: 0x06004232 RID: 16946 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F3A RID: 16186
		[SerializeField]
		private Image m_fullIcon;

		// Token: 0x04003F3B RID: 16187
		[SerializeField]
		private Image m_backgroundIcon;

		// Token: 0x04003F3C RID: 16188
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04003F3D RID: 16189
		[SerializeField]
		private float m_extendedHeight = 45f;

		// Token: 0x04003F3E RID: 16190
		[SerializeField]
		private ToggleController m_toggleController;

		// Token: 0x04003F3F RID: 16191
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003F40 RID: 16192
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04003F41 RID: 16193
		private StanceSelectionBubbleController m_controller;

		// Token: 0x04003F42 RID: 16194
		private Stance m_stance;

		// Token: 0x04003F43 RID: 16195
		private ArchetypeInstance m_combatMasteryInstance;

		// Token: 0x04003F44 RID: 16196
		private RectTransform m_rectToAnimate;

		// Token: 0x04003F45 RID: 16197
		private float m_retractedHeight;

		// Token: 0x04003F46 RID: 16198
		private float m_targetHeight;
	}
}
