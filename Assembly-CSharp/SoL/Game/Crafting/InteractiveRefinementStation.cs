using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CDF RID: 3295
	public class InteractiveRefinementStation : BaseNetworkedInteractive, ITooltip, IInteractiveBase, IRefinementStation
	{
		// Token: 0x170017EB RID: 6123
		// (get) Token: 0x060063D6 RID: 25558 RVA: 0x00083311 File Offset: 0x00081511
		public CraftingStationProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x170017EC RID: 6124
		// (get) Token: 0x060063D7 RID: 25559 RVA: 0x00083319 File Offset: 0x00081519
		string IRefinementStation.DisplayName
		{
			get
			{
				if (!(this.m_profile != null))
				{
					return "Crafting Station";
				}
				return this.m_profile.DisplayName;
			}
		}

		// Token: 0x060063D8 RID: 25560 RVA: 0x00207AC0 File Offset: 0x00205CC0
		public override void BeginInteraction(GameEntity interactionSource)
		{
			bool flag;
			if (GameManager.IsServer)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.Server_RequestInteraction(base.GameEntity.NetworkEntity);
				interactionSource.CharacterData.CharacterFlags.Value |= PlayerFlags.RemoteContainer;
				interactionSource.CollectionController.RefinementStation = this;
				flag = true;
			}
			else
			{
				if (this.m_hideCallback == null)
				{
					this.m_hideCallback = new Action(this.LocalClientHide);
				}
				interactionSource.CollectionController.RefinementStation = this;
				ClientGameManager.UIManager.CraftingUI.RefinementStation = this;
				ClientGameManager.UIManager.CraftingUI.HideCallback = this.m_hideCallback;
				ClientGameManager.UIManager.CraftingUI.Show(false);
				flag = true;
			}
			if (flag)
			{
				base.BeginInteraction(interactionSource);
			}
		}

		// Token: 0x060063D9 RID: 25561 RVA: 0x00207B84 File Offset: 0x00205D84
		public override void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			if (GameManager.IsServer)
			{
				interactionSource.CharacterData.CharacterFlags.Value &= ~PlayerFlags.RemoteContainer;
				interactionSource.CollectionController.RefinementStation = null;
				if (!clientIsEnding)
				{
					interactionSource.NetworkEntity.PlayerRpcHandler.Server_CancelInteraction(base.GameEntity.NetworkEntity);
				}
			}
			else
			{
				interactionSource.CollectionController.RefinementStation = null;
				ClientGameManager.UIManager.CraftingUI.RefinementStation = null;
				ClientGameManager.UIManager.CraftingUI.HideCallback = null;
				if (ClientGameManager.UIManager.CraftingUI.Visible)
				{
					ClientGameManager.UIManager.CraftingUI.Hide(false);
				}
				if (clientIsEnding)
				{
					interactionSource.NetworkEntity.PlayerRpcHandler.Client_CancelInteraction(base.GameEntity.NetworkEntity);
				}
			}
			base.EndInteraction(interactionSource, clientIsEnding);
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x0007CC1A File Offset: 0x0007AE1A
		private void LocalClientHide()
		{
			this.EndInteraction(LocalPlayer.GameEntity, true);
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x00207C54 File Offset: 0x00205E54
		protected virtual string GetTooltipText()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (this.m_profile != null)
				{
					utf16ValueStringBuilder.Append(this.m_profile.DisplayName);
				}
				else
				{
					utf16ValueStringBuilder.Append("Crafting Station");
				}
				if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CharacterData != null && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag))
				{
					utf16ValueStringBuilder.Append("\nYou must first recover your bag to interact with this object!");
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060063DC RID: 25564 RVA: 0x0008333A File Offset: 0x0008153A
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.GetTooltipText(), false);
		}

		// Token: 0x170017ED RID: 6125
		// (get) Token: 0x060063DD RID: 25565 RVA: 0x0008334E File Offset: 0x0008154E
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170017EE RID: 6126
		// (get) Token: 0x060063DE RID: 25566 RVA: 0x0008335C File Offset: 0x0008155C
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040056B8 RID: 22200
		private const string kDefaultDisplayName = "Crafting Station";

		// Token: 0x040056B9 RID: 22201
		[SerializeField]
		private CraftingStationProfile m_profile;

		// Token: 0x040056BA RID: 22202
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040056BB RID: 22203
		private Action m_hideCallback;
	}
}
