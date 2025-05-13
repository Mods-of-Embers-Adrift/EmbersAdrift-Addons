using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005DC RID: 1500
	public class DeathIconUI : MonoBehaviour, IContextMenu, IInteractiveBase, ITooltip
	{
		// Token: 0x06002F9A RID: 12186 RVA: 0x00157798 File Offset: 0x00155998
		private void Start()
		{
			if (this.m_isForLocal && LocalPlayer.GameEntity != null)
			{
				LocalPlayer.GameEntity.CharacterData.CharacterFlags.Changed += this.PlayerFlagsOnChanged;
				this.PlayerFlagsOnChanged(LocalPlayer.GameEntity.CharacterData.CharacterFlags);
			}
		}

		// Token: 0x06002F9B RID: 12187 RVA: 0x00060D83 File Offset: 0x0005EF83
		private void OnDestroy()
		{
			if (this.m_isForLocal && LocalPlayer.GameEntity != null)
			{
				LocalPlayer.GameEntity.CharacterData.CharacterFlags.Changed -= this.PlayerFlagsOnChanged;
			}
		}

		// Token: 0x06002F9C RID: 12188 RVA: 0x001577F4 File Offset: 0x001559F4
		private void PlayerFlagsOnChanged(PlayerFlags obj)
		{
			if (!this.m_isForLocal)
			{
				return;
			}
			if (this.m_canvasGroup != null)
			{
				bool flag = obj.HasBitFlag(PlayerFlags.MissingBag);
				this.m_canvasGroup.alpha = (flag ? 1f : 0f);
				this.m_canvasGroup.interactable = flag;
				this.m_canvasGroup.blocksRaycasts = flag;
			}
		}

		// Token: 0x06002F9D RID: 12189 RVA: 0x00060DBA File Offset: 0x0005EFBA
		private bool IsDead()
		{
			return this.m_isForLocal && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag);
		}

		// Token: 0x06002F9E RID: 12190 RVA: 0x00060DE0 File Offset: 0x0005EFE0
		public void SetIsLocal(bool isLocal)
		{
			this.m_isForLocal = isLocal;
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06002F9F RID: 12191 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002FA0 RID: 12192 RVA: 0x00157854 File Offset: 0x00155A54
		public string FillActionsGetTitle()
		{
			if (!this.m_isForLocal || LocalPlayer.GameEntity == null)
			{
				return null;
			}
			ContextMenuUI.AddContextAction("Forfeit Bag Contents", LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag), new Action(this.ForfeitConfirm), null, null);
			return "Death";
		}

		// Token: 0x06002FA1 RID: 12193 RVA: 0x001578B0 File Offset: 0x00155AB0
		private void ForfeitConfirm()
		{
			if (!this.m_isForLocal || !this.IsDead() || LocalPlayer.GameEntity == null)
			{
				return;
			}
			DialogOptions opts = new DialogOptions
			{
				Title = "Forfeit Bag",
				Text = "Are you sure you want to forfeit the contents of your bag?.  All will be lost!",
				Callback = new Action<bool, object>(this.ForfeitResponse),
				ConfirmationText = "Yes",
				CancelText = "NO"
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x06002FA2 RID: 12194 RVA: 0x00060DE9 File Offset: 0x0005EFE9
		private void ForfeitResponse(bool answer, object value)
		{
			if (this.m_isForLocal && this.IsDead() && answer)
			{
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.ForfeitInventoryRequest();
			}
		}

		// Token: 0x06002FA3 RID: 12195 RVA: 0x00157938 File Offset: 0x00155B38
		private ITooltipParameter GetTooltipParameter()
		{
			string text = this.m_isForLocal ? "Your inventory is unavailable until you find your bag..." : "This person is in search of their bag";
			if (this.m_isForLocal && LocalPlayer.GameEntity != null)
			{
				text += "\nRight click to forfeit.";
			}
			return new ObjectTextTooltipParameter(this, text, false);
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06002FA4 RID: 12196 RVA: 0x00060E14 File Offset: 0x0005F014
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06002FA5 RID: 12197 RVA: 0x00060E22 File Offset: 0x0005F022
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06002FA7 RID: 12199 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002E9F RID: 11935
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04002EA0 RID: 11936
		[SerializeField]
		private bool m_isForLocal;

		// Token: 0x04002EA1 RID: 11937
		[SerializeField]
		private CanvasGroup m_canvasGroup;
	}
}
