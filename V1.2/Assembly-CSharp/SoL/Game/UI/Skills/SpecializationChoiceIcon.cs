using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000934 RID: 2356
	public class SpecializationChoiceIcon : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x17000F8E RID: 3982
		// (get) Token: 0x0600456C RID: 17772 RVA: 0x0006EBD5 File Offset: 0x0006CDD5
		// (set) Token: 0x0600456D RID: 17773 RVA: 0x0006EBDD File Offset: 0x0006CDDD
		private ArchetypeInstance MasteryInstance
		{
			get
			{
				return this.m_masteryInstance;
			}
			set
			{
				if (this.m_masteryInstance == value)
				{
					return;
				}
				this.Unsubscribe();
				this.m_masteryInstance = value;
				this.Subscribe();
			}
		}

		// Token: 0x0600456E RID: 17774 RVA: 0x0006EBFC File Offset: 0x0006CDFC
		private void Awake()
		{
			if (this.m_tickMark != null)
			{
				this.m_defaultTickMarkColor = this.m_tickMark.color;
			}
			this.m_stylizer = base.gameObject.GetComponent<WindowComponentStylizer>();
		}

		// Token: 0x0600456F RID: 17775 RVA: 0x0006EC2E File Offset: 0x0006CE2E
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x06004570 RID: 17776 RVA: 0x0019FD00 File Offset: 0x0019DF00
		private void Subscribe()
		{
			if (this.m_masteryInstance != null)
			{
				if (this.m_masteryInstance.MasteryData != null)
				{
					this.m_masteryInstance.MasteryData.LevelDataChanged += this.MasteryDataChanged;
					this.m_masteryInstance.MasteryData.MasteryDataChanged += this.MasteryDataChanged;
				}
				this.m_subscribed = true;
			}
		}

		// Token: 0x06004571 RID: 17777 RVA: 0x0019FD64 File Offset: 0x0019DF64
		private void Unsubscribe()
		{
			if (this.m_subscribed && this.m_masteryInstance != null)
			{
				if (this.m_masteryInstance.MasteryData != null)
				{
					this.m_masteryInstance.MasteryData.LevelDataChanged -= this.MasteryDataChanged;
					this.m_masteryInstance.MasteryData.MasteryDataChanged -= this.MasteryDataChanged;
				}
				this.m_subscribed = false;
			}
		}

		// Token: 0x06004572 RID: 17778 RVA: 0x0006EC36 File Offset: 0x0006CE36
		private void MasteryDataChanged()
		{
			this.RefreshStatus();
		}

		// Token: 0x06004573 RID: 17779 RVA: 0x0006EC3E File Offset: 0x0006CE3E
		public void SetArchetype(ArchetypeInstance masteryInstance, BaseRole baseRole, SpecializedRole specializedRole)
		{
			this.MasteryInstance = masteryInstance;
			this.m_baseRole = baseRole;
			this.m_specializedRole = specializedRole;
			this.RefreshStatus();
		}

		// Token: 0x06004574 RID: 17780 RVA: 0x0019FDD0 File Offset: 0x0019DFD0
		private void RefreshStatus()
		{
			if (this.MasteryInstance == null || this.m_baseRole == null || this.m_specializedRole == null)
			{
				this.m_isLearned = false;
				this.m_canLearn = false;
				this.m_icon.overrideSprite = null;
				this.m_disabledOverlay.enabled = true;
				if (this.m_tickMark)
				{
					this.m_tickMark.color = this.m_defaultTickMarkColor;
				}
				if (this.m_stylizer)
				{
					this.m_stylizer.ResetFrameColor();
				}
				base.gameObject.SetActive(false);
				return;
			}
			this.m_icon.overrideSprite = this.m_specializedRole.Icon;
			this.m_icon.color = this.m_specializedRole.IconTint;
			this.m_isLearned = (this.MasteryInstance.MasteryData.Specialization != null && this.MasteryInstance.MasteryData.Specialization.Value == this.m_specializedRole.Id);
			this.m_canLearn = (this.MasteryInstance.MasteryData.Specialization == null && this.MasteryInstance.GetAssociatedLevel(LocalPlayer.GameEntity) >= 6f && this.m_specializedRole != null && !this.m_specializedRole.DisallowContextMenuLearning);
			this.m_disabledOverlay.enabled = (!this.m_isLearned && !this.m_canLearn);
			if (this.m_tickMark)
			{
				Color color = this.m_isLearned ? UIManager.RequirementsMetColor : this.m_defaultTickMarkColor;
				color.a = this.m_defaultTickMarkColor.a;
				this.m_tickMark.color = color;
			}
			if (this.m_stylizer)
			{
				if (this.m_isLearned)
				{
					this.m_stylizer.SetFrameColor(UIManager.RequirementsMetColor);
				}
				else
				{
					this.m_stylizer.ResetFrameColor();
				}
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x17000F8F RID: 3983
		// (get) Token: 0x06004575 RID: 17781 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004576 RID: 17782 RVA: 0x0019FFD8 File Offset: 0x0019E1D8
		string IContextMenu.FillActionsGetTitle()
		{
			if (this.m_masteryInstance == null)
			{
				return null;
			}
			if (this.m_isLearned && !this.m_disabledOverlay.enabled)
			{
				ContextMenuUI.AddContextAction("Forget", true, new Action(this.ForgetConfirm), null, null);
				return this.m_specializedRole.DisplayName;
			}
			if (this.m_canLearn && !this.m_disabledOverlay.enabled)
			{
				ContextMenuUI.AddContextAction("Learn", true, new Action(this.SelectCallback), null, null);
				return this.m_specializedRole.DisplayName;
			}
			return null;
		}

		// Token: 0x06004577 RID: 17783 RVA: 0x001A0064 File Offset: 0x0019E264
		private void SelectCallback()
		{
			if (this.m_masteryInstance == null || this.m_specializedRole == null || LocalPlayer.NetworkEntity == null || LocalPlayer.NetworkEntity.PlayerRpcHandler == null)
			{
				return;
			}
			LocalPlayer.NetworkEntity.PlayerRpcHandler.TrainSpecializationRequest(this.m_masteryInstance.InstanceId, this.m_specializedRole.Id);
		}

		// Token: 0x06004578 RID: 17784 RVA: 0x0006EC5B File Offset: 0x0006CE5B
		private bool InternalCanForget()
		{
			return this.m_masteryInstance != null && this.m_specializedRole != null;
		}

		// Token: 0x06004579 RID: 17785 RVA: 0x0006EC73 File Offset: 0x0006CE73
		private void ForgetConfirm()
		{
			if (!ForgetHelper.CanForgetWithError() || !this.InternalCanForget() || !ForgetHelper.CanForgetSpecialization(this.m_masteryInstance))
			{
				return;
			}
			ForgetHelper.ForgetSpecializationConfirmation(this.m_specializedRole, new Action<bool, object>(this.ForgetResponse));
		}

		// Token: 0x0600457A RID: 17786 RVA: 0x001A00CC File Offset: 0x0019E2CC
		private void ForgetResponse(bool answer, object value)
		{
			if (!answer)
			{
				return;
			}
			if (ForgetHelper.CanForgetWithError() && this.InternalCanForget() && ForgetHelper.CanForgetSpecialization(this.m_masteryInstance))
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.ForgetSpecializationRequest(this.m_masteryInstance.InstanceId, this.m_specializedRole.Id);
			}
		}

		// Token: 0x0600457B RID: 17787 RVA: 0x001A0120 File Offset: 0x0019E320
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_specializedRole)
			{
				return new ArchetypeTooltipParameter
				{
					Archetype = this.m_specializedRole
				};
			}
			return null;
		}

		// Token: 0x17000F90 RID: 3984
		// (get) Token: 0x0600457C RID: 17788 RVA: 0x0006ECA9 File Offset: 0x0006CEA9
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F91 RID: 3985
		// (get) Token: 0x0600457D RID: 17789 RVA: 0x0006ECB7 File Offset: 0x0006CEB7
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600457E RID: 17790 RVA: 0x0006ECBF File Offset: 0x0006CEBF
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocused = UIManager.IsChatActive;
		}

		// Token: 0x0600457F RID: 17791 RVA: 0x001A0158 File Offset: 0x0019E358
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			bool chatWasFocused = this.m_chatWasFocused;
			this.m_chatWasFocused = false;
			if (eventData.button == PointerEventData.InputButton.Left && ClientGameManager.InputManager.HoldingShift && chatWasFocused)
			{
				UIManager.ActiveChatInput.AddArchetypeLink(this.m_specializedRole);
			}
		}

		// Token: 0x06004581 RID: 17793 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040041D1 RID: 16849
		[SerializeField]
		private Image m_icon;

		// Token: 0x040041D2 RID: 16850
		[SerializeField]
		private Image m_disabledOverlay;

		// Token: 0x040041D3 RID: 16851
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040041D4 RID: 16852
		[SerializeField]
		private Image m_tickMark;

		// Token: 0x040041D5 RID: 16853
		private ArchetypeInstance m_masteryInstance;

		// Token: 0x040041D6 RID: 16854
		private bool m_subscribed;

		// Token: 0x040041D7 RID: 16855
		private bool m_isLearned;

		// Token: 0x040041D8 RID: 16856
		private bool m_canLearn;

		// Token: 0x040041D9 RID: 16857
		private BaseRole m_baseRole;

		// Token: 0x040041DA RID: 16858
		private SpecializedRole m_specializedRole;

		// Token: 0x040041DB RID: 16859
		private Color m_defaultTickMarkColor = Color.white;

		// Token: 0x040041DC RID: 16860
		private WindowComponentStylizer m_stylizer;

		// Token: 0x040041DD RID: 16861
		private bool m_chatWasFocused;
	}
}
