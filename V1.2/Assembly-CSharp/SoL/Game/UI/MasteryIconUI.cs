using System;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Skills;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000869 RID: 2153
	public class MasteryIconUI : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		// Token: 0x06003E37 RID: 15927 RVA: 0x0006A15F File Offset: 0x0006835F
		private void OnEnable()
		{
			this.ToggleHighlight(false);
		}

		// Token: 0x06003E38 RID: 15928 RVA: 0x0006A168 File Offset: 0x00068368
		public void AssignInstance(ArchetypeInstance instance)
		{
			this.m_instance = instance;
		}

		// Token: 0x06003E39 RID: 15929 RVA: 0x0006A171 File Offset: 0x00068371
		private bool InternalCanForget()
		{
			return this.m_instance != null && this.m_instance.Mastery != null && !this.m_instance.Mastery.DynamicallyLoaded;
		}

		// Token: 0x06003E3A RID: 15930 RVA: 0x00184B14 File Offset: 0x00182D14
		private void ForgetBaseRoleConfirm()
		{
			if (!ForgetHelper.CanForgetSomething() || !this.InternalCanForget())
			{
				return;
			}
			BaseRole baseRole;
			string str = (this.m_instance != null && this.m_instance.Archetype && this.m_instance.Archetype.TryGetAsType(out baseRole) && baseRole.Type != MasteryType.Combat) ? "You will lose <b><i>all progress</i></b> you have made! However, all current recipes will be retained if you take up this profession again." : "You will lose <b><i>all progress</i></b> you have made and <b><i>all associated abilities</i></b>!";
			DialogOptions opts = new DialogOptions
			{
				Title = this.GetForgetTitle(),
				Text = "Are you sure you want to forget " + this.m_instance.Archetype.DisplayName + "? " + str,
				Callback = new Action<bool, object>(this.ForgetBaseRoleResponse),
				ConfirmationText = "Yes",
				CancelText = "NO"
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06003E3B RID: 15931 RVA: 0x0006A1A3 File Offset: 0x000683A3
		private void ForgetBaseRoleResponse(bool answer, object value)
		{
			if (!answer)
			{
				return;
			}
			if (ForgetHelper.CanForgetWithError() && this.InternalCanForget())
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.ForgetMastery(this.m_instance.InstanceId);
			}
		}

		// Token: 0x06003E3C RID: 15932 RVA: 0x00184BF0 File Offset: 0x00182DF0
		private void ForgetSpecializationConfirm()
		{
			if (!ForgetHelper.CanForgetWithError() || !this.InternalCanForget() || !ForgetHelper.CanForgetSpecialization(this.m_instance))
			{
				return;
			}
			SpecializedRole specializedRole;
			if (this.m_instance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(this.m_instance.MasteryData.Specialization.Value, out specializedRole))
			{
				ForgetHelper.ForgetSpecializationConfirmation(specializedRole, new Action<bool, object>(this.ForgetSpecializationResponse));
			}
		}

		// Token: 0x06003E3D RID: 15933 RVA: 0x00184C6C File Offset: 0x00182E6C
		private void ForgetSpecializationResponse(bool answer, object value)
		{
			if (!answer)
			{
				return;
			}
			if (ForgetHelper.CanForgetWithError() && this.InternalCanForget() && ForgetHelper.CanForgetSpecialization(this.m_instance) && this.m_instance.MasteryData != null && this.m_instance.MasteryData.Specialization != null)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.ForgetSpecializationRequest(this.m_instance.InstanceId, this.m_instance.MasteryData.Specialization.Value);
			}
		}

		// Token: 0x06003E3E RID: 15934 RVA: 0x0006A1D2 File Offset: 0x000683D2
		private void ToggleHighlight(bool isEnabled)
		{
			if (this.m_highlight != null)
			{
				this.m_highlight.enabled = isEnabled;
			}
		}

		// Token: 0x06003E3F RID: 15935 RVA: 0x00184CF4 File Offset: 0x00182EF4
		private string GetForgetTitle()
		{
			BaseRole baseRole;
			if (this.m_instance != null && this.m_instance.Archetype && this.m_instance.Archetype.TryGetAsType(out baseRole))
			{
				MasteryType type = baseRole.Type;
				if (type == MasteryType.Combat)
				{
					return "Forget BASE Role";
				}
				if (type - MasteryType.Trade <= 1)
				{
					return "Forget Profession";
				}
			}
			return string.Empty;
		}

		// Token: 0x06003E40 RID: 15936 RVA: 0x00184D54 File Offset: 0x00182F54
		string IContextMenu.FillActionsGetTitle()
		{
			if (!ForgetHelper.CanForgetSomething())
			{
				return null;
			}
			if (!this.InternalCanForget())
			{
				return null;
			}
			SpecializedRole specializedRole;
			if (this.m_instance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(this.m_instance.MasteryData.Specialization.Value, out specializedRole))
			{
				ContextMenuUI.AddContextAction("Forget SPECIALIZATION", true, new Action(this.ForgetSpecializationConfirm), null, null);
				return specializedRole.DisplayName;
			}
			ContextMenuUI.AddContextAction(this.GetForgetTitle(), true, new Action(this.ForgetBaseRoleConfirm), null, null);
			return this.m_instance.Mastery.DisplayName;
		}

		// Token: 0x06003E41 RID: 15937 RVA: 0x00184E00 File Offset: 0x00183000
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_instance
			};
		}

		// Token: 0x17000E5D RID: 3677
		// (get) Token: 0x06003E42 RID: 15938 RVA: 0x0006A1EE File Offset: 0x000683EE
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E5E RID: 3678
		// (get) Token: 0x06003E43 RID: 15939 RVA: 0x0006A1FC File Offset: 0x000683FC
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E5F RID: 3679
		// (get) Token: 0x06003E44 RID: 15940 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003E45 RID: 15941 RVA: 0x0006A204 File Offset: 0x00068404
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.ToggleHighlight(true);
		}

		// Token: 0x06003E46 RID: 15942 RVA: 0x0006A15F File Offset: 0x0006835F
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.ToggleHighlight(false);
		}

		// Token: 0x06003E47 RID: 15943 RVA: 0x0006A20D File Offset: 0x0006840D
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocused = UIManager.IsChatActive;
		}

		// Token: 0x06003E48 RID: 15944 RVA: 0x00184E34 File Offset: 0x00183034
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			bool chatWasFocused = this.m_chatWasFocused;
			this.m_chatWasFocused = false;
			if (eventData.button == PointerEventData.InputButton.Left && ClientGameManager.InputManager.HoldingShift && chatWasFocused)
			{
				UIManager.ActiveChatInput.AddArchetypeLink(this.m_instance.Archetype);
			}
		}

		// Token: 0x06003E4A RID: 15946 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C86 RID: 15494
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C87 RID: 15495
		[SerializeField]
		private Image m_highlight;

		// Token: 0x04003C88 RID: 15496
		private ArchetypeInstance m_instance;

		// Token: 0x04003C89 RID: 15497
		private bool m_chatWasFocused;
	}
}
