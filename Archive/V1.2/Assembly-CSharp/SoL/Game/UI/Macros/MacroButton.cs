using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.UI.Chat;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.UI.Macros
{
	// Token: 0x0200097A RID: 2426
	public class MacroButton : MonoBehaviour, ITooltip, IInteractiveBase, IContextMenu
	{
		// Token: 0x17001002 RID: 4098
		// (get) Token: 0x0600483B RID: 18491 RVA: 0x0007093F File Offset: 0x0006EB3F
		// (set) Token: 0x0600483C RID: 18492 RVA: 0x00070947 File Offset: 0x0006EB47
		public MacroData Data { get; private set; }

		// Token: 0x17001003 RID: 4099
		// (get) Token: 0x0600483D RID: 18493 RVA: 0x00070950 File Offset: 0x0006EB50
		public int DisplayIndex
		{
			get
			{
				return this.m_index + 1;
			}
		}

		// Token: 0x17001004 RID: 4100
		// (get) Token: 0x0600483E RID: 18494 RVA: 0x0007095A File Offset: 0x0006EB5A
		private bool IsVertical
		{
			get
			{
				return this.m_controller && this.m_controller.IsVertical;
			}
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x00070976 File Offset: 0x0006EB76
		private void Awake()
		{
			if (this.m_button)
			{
				this.m_button.PointerClicked += this.OnPointerClicked;
			}
		}

		// Token: 0x06004840 RID: 18496 RVA: 0x0007099C File Offset: 0x0006EB9C
		private void OnDestroy()
		{
			if (this.m_button)
			{
				this.m_button.PointerClicked -= this.OnPointerClicked;
			}
		}

		// Token: 0x06004841 RID: 18497 RVA: 0x000709C2 File Offset: 0x0006EBC2
		private void OnPointerClicked(PointerEventData obj)
		{
			if (obj.button == PointerEventData.InputButton.Left)
			{
				this.ExecuteMacro();
			}
		}

		// Token: 0x06004842 RID: 18498 RVA: 0x001A92F8 File Offset: 0x001A74F8
		internal void ExecuteMacro()
		{
			if (this.Data == null || string.IsNullOrEmpty(this.Data.MacroText) || !LocalPlayer.GameEntity)
			{
				return;
			}
			ChatWindowUI chatWindowForMacro = UIManager.GetChatWindowForMacro();
			if (!chatWindowForMacro)
			{
				return;
			}
			string text = this.Data.MacroText;
			if (text.Contains("%ot"))
			{
				string newValue = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController && LocalPlayer.GameEntity.TargetController.OffensiveTarget && LocalPlayer.GameEntity.TargetController.OffensiveTarget.CharacterData) ? LocalPlayer.GameEntity.TargetController.OffensiveTarget.CharacterData.Name.Value : "offensive target";
				text = text.Replace("%ot", newValue);
			}
			if (text.Contains("%dt"))
			{
				string newValue2 = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController && LocalPlayer.GameEntity.TargetController.DefensiveTarget && LocalPlayer.GameEntity.TargetController.DefensiveTarget.CharacterData) ? LocalPlayer.GameEntity.TargetController.DefensiveTarget.CharacterData.Name.Value : "defensive target";
				text = text.Replace("%dt", newValue2);
			}
			chatWindowForMacro.SubmitMacroInput(text);
		}

		// Token: 0x06004843 RID: 18499 RVA: 0x000709D2 File Offset: 0x0006EBD2
		internal void Init(MacroBarUI controller, int index, MacroData data)
		{
			this.m_controller = controller;
			this.m_index = index;
			this.Data = data;
			this.Refresh();
		}

		// Token: 0x06004844 RID: 18500 RVA: 0x000709EF File Offset: 0x0006EBEF
		private void DeleteMacroRequest()
		{
			if (this.m_controller)
			{
				this.m_controller.DeleteMacro(this);
			}
		}

		// Token: 0x06004845 RID: 18501 RVA: 0x00070A0A File Offset: 0x0006EC0A
		private void EditMacroRequest()
		{
			if (this.m_controller)
			{
				this.m_controller.EditMacro(this);
			}
		}

		// Token: 0x06004846 RID: 18502 RVA: 0x00070A25 File Offset: 0x0006EC25
		private void MoveTowardsFrontRequest()
		{
			if (this.m_controller)
			{
				this.m_controller.MoveTowardsFront(this);
			}
		}

		// Token: 0x06004847 RID: 18503 RVA: 0x00070A40 File Offset: 0x0006EC40
		private void MoveTowardsBackRequest()
		{
			if (this.m_controller)
			{
				this.m_controller.MoveTowardsBack(this);
			}
		}

		// Token: 0x06004848 RID: 18504 RVA: 0x001A9470 File Offset: 0x001A7670
		private void Refresh()
		{
			string text = "EMPTY";
			bool active = false;
			if (this.Data != null)
			{
				if (!string.IsNullOrEmpty(this.Data.DisplayName))
				{
					text = this.Data.DisplayName;
				}
				active = true;
			}
			this.m_button.SetText(text);
			base.gameObject.SetActive(active);
		}

		// Token: 0x06004849 RID: 18505 RVA: 0x001A94C8 File Offset: 0x001A76C8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.Data != null)
			{
				string txt = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.AppendFormat<int>("Macro {0}:", this.DisplayIndex);
					utf16ValueStringBuilder.AppendLine();
					if (string.IsNullOrEmpty(this.Data.MacroText))
					{
						utf16ValueStringBuilder.AppendLine("<b>EMPTY</b>");
					}
					else
					{
						utf16ValueStringBuilder.AppendFormat<string>("<b>{0}</b>", this.Data.MacroText);
						utf16ValueStringBuilder.AppendLine();
					}
					utf16ValueStringBuilder.AppendLine();
					utf16ValueStringBuilder.Append("<size=80%>");
					if (this.m_binding)
					{
						IBindingLabel binding = this.m_binding;
						if (binding.Label && !string.IsNullOrEmpty(binding.Label.text))
						{
							utf16ValueStringBuilder.AppendFormat<string>("Keybind assigned to {0}", binding.Label.text);
						}
						else
						{
							utf16ValueStringBuilder.Append("Unbound. Can be assigned a keybind in the options");
						}
					}
					utf16ValueStringBuilder.AppendLine("Right click to EDIT or DELETE.");
					utf16ValueStringBuilder.Append("</size>");
					txt = utf16ValueStringBuilder.ToString();
				}
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17001005 RID: 4101
		// (get) Token: 0x0600484A RID: 18506 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001006 RID: 4102
		// (get) Token: 0x0600484B RID: 18507 RVA: 0x00070A5B File Offset: 0x0006EC5B
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001007 RID: 4103
		// (get) Token: 0x0600484C RID: 18508 RVA: 0x00070A69 File Offset: 0x0006EC69
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600484D RID: 18509 RVA: 0x001A9604 File Offset: 0x001A7804
		public string FillActionsGetTitle()
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.CharacterData)
			{
				return null;
			}
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction(ZString.Format<string>("{0} Edit", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>"), this.Data != null, new Action(this.EditMacroRequest), null, null);
			ContextMenuUI.AddContextAction(ZString.Format<string>("{0} Delete", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>"), this.Data != null, new Action(this.DeleteMacroRequest), null, null);
			if (this.m_index > 0)
			{
				ContextMenuUI.AddContextAction(this.IsVertical ? ZString.Format<string>("{0} Move Up", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>") : ZString.Format<string>("{0} Move Left", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>"), this.Data != null, new Action(this.MoveTowardsFrontRequest), null, null);
			}
			if (this.m_controller && this.m_index < this.m_controller.MacroDataCount - 1)
			{
				ContextMenuUI.AddContextAction(this.IsVertical ? ZString.Format<string>("{0} Move Down", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>") : ZString.Format<string>("{0} Move Right", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>"), this.Data != null, new Action(this.MoveTowardsBackRequest), null, null);
			}
			return ZString.Format<int>("Macro {0}", this.DisplayIndex);
		}

		// Token: 0x0600484F RID: 18511 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004398 RID: 17304
		private const string kOffensiveTarget = "%ot";

		// Token: 0x04004399 RID: 17305
		private const string kDefensiveTarget = "%dt";

		// Token: 0x0400439A RID: 17306
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400439B RID: 17307
		[SerializeField]
		private SolButton m_button;

		// Token: 0x0400439C RID: 17308
		[SerializeField]
		private BindingLabel m_binding;

		// Token: 0x0400439D RID: 17309
		private MacroBarUI m_controller;

		// Token: 0x0400439F RID: 17311
		private int m_index = -1;
	}
}
