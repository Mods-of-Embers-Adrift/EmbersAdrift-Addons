using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008D3 RID: 2259
	public class ShowHideToggle : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004210 RID: 16912 RVA: 0x0006C971 File Offset: 0x0006AB71
		private void Start()
		{
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06004211 RID: 16913 RVA: 0x0006C992 File Offset: 0x0006AB92
		private void OnDestroy()
		{
			if (this.m_initialized)
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			}
		}

		// Token: 0x06004212 RID: 16914 RVA: 0x0006C9B8 File Offset: 0x0006ABB8
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x06004213 RID: 16915 RVA: 0x001918BC File Offset: 0x0018FABC
		public void Init()
		{
			if (this.m_initialized || LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null)
			{
				return;
			}
			ShowHideToggle.ToggleType type = this.m_type;
			if (type != ShowHideToggle.ToggleType.Helm)
			{
				if (type == ShowHideToggle.ToggleType.EmberStone)
				{
					this.m_toggle.isOn = !LocalPlayer.GameEntity.CharacterData.HideEmberStone;
				}
			}
			else
			{
				this.m_toggle.isOn = !LocalPlayer.GameEntity.CharacterData.HideHelm;
			}
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			this.m_initialized = true;
		}

		// Token: 0x06004214 RID: 16916 RVA: 0x00191960 File Offset: 0x0018FB60
		private void ToggleChanged(bool arg0)
		{
			ShowHideToggle.ToggleType type = this.m_type;
			if (type == ShowHideToggle.ToggleType.Helm)
			{
				LocalPlayer.GameEntity.CharacterData.HideHelm = !arg0;
				return;
			}
			if (type != ShowHideToggle.ToggleType.EmberStone)
			{
				return;
			}
			LocalPlayer.GameEntity.CharacterData.HideEmberStone = !arg0;
		}

		// Token: 0x06004215 RID: 16917 RVA: 0x001919A4 File Offset: 0x0018FBA4
		private string GetTitleForType()
		{
			ShowHideToggle.ToggleType type = this.m_type;
			if (type == ShowHideToggle.ToggleType.Helm)
			{
				return "Show/Hide Helm";
			}
			if (type != ShowHideToggle.ToggleType.EmberStone)
			{
				return string.Empty;
			}
			return "Show/Hide Ember Stone";
		}

		// Token: 0x06004216 RID: 16918 RVA: 0x0006C9D1 File Offset: 0x0006ABD1
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.GetTitleForType(), false);
		}

		// Token: 0x17000F0D RID: 3853
		// (get) Token: 0x06004217 RID: 16919 RVA: 0x0006C9E5 File Offset: 0x0006ABE5
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x17000F0E RID: 3854
		// (get) Token: 0x06004218 RID: 16920 RVA: 0x0006C9ED File Offset: 0x0006ABED
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000F0F RID: 3855
		// (get) Token: 0x06004219 RID: 16921 RVA: 0x0006C9F5 File Offset: 0x0006ABF5
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F30 RID: 16176
		private const string kHelmTitle = "Show/Hide Helm";

		// Token: 0x04003F31 RID: 16177
		private const string kStoneTitle = "Show/Hide Ember Stone";

		// Token: 0x04003F32 RID: 16178
		[SerializeField]
		private ShowHideToggle.ToggleType m_type;

		// Token: 0x04003F33 RID: 16179
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04003F34 RID: 16180
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04003F35 RID: 16181
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003F36 RID: 16182
		private bool m_initialized;

		// Token: 0x020008D4 RID: 2260
		private enum ToggleType
		{
			// Token: 0x04003F38 RID: 16184
			Helm,
			// Token: 0x04003F39 RID: 16185
			EmberStone
		}
	}
}
