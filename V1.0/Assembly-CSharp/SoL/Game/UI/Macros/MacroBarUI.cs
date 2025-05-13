using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Newtonsoft.Json;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Macros
{
	// Token: 0x02000976 RID: 2422
	public class MacroBarUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000FF9 RID: 4089
		// (get) Token: 0x06004810 RID: 18448 RVA: 0x00070861 File Offset: 0x0006EA61
		internal int MacroDataCount
		{
			get
			{
				if (this.m_macroData == null)
				{
					return 0;
				}
				return this.m_macroData.Count;
			}
		}

		// Token: 0x17000FFA RID: 4090
		// (get) Token: 0x06004811 RID: 18449 RVA: 0x00070878 File Offset: 0x0006EA78
		internal bool IsVertical
		{
			get
			{
				return this.m_gridLayout && this.m_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount;
			}
		}

		// Token: 0x06004812 RID: 18450 RVA: 0x001A8964 File Offset: 0x001A6B64
		private void Awake()
		{
			if (this.m_window)
			{
				this.m_savePosAndSize = this.m_window.SaveWindowPositionSizeValue;
				this.m_window.SaveWindowPositionSizeValue = false;
			}
			InputManager.MacroActionPressed += this.InputManagerOnMacroActionPressed;
			this.m_addButton.onClick.AddListener(new UnityAction(this.AddButtonClicked));
			this.m_rotateButton.onClick.AddListener(new UnityAction(this.OnRotateClicked));
			Options.GameOptions.ShowMacroBar.Changed += this.ShowMacroBarOnChanged;
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			if (LocalPlayer.GameEntity)
			{
				this.LocalPlayerOnLocalPlayerInitialized();
			}
		}

		// Token: 0x06004813 RID: 18451 RVA: 0x00070897 File Offset: 0x0006EA97
		private void Start()
		{
			this.ShowMacroBarOnChanged();
		}

		// Token: 0x06004814 RID: 18452 RVA: 0x001A8A20 File Offset: 0x001A6C20
		private void OnDestroy()
		{
			InputManager.MacroActionPressed -= this.InputManagerOnMacroActionPressed;
			this.m_addButton.onClick.RemoveListener(new UnityAction(this.AddButtonClicked));
			this.m_rotateButton.onClick.RemoveListener(new UnityAction(this.OnRotateClicked));
			Options.GameOptions.ShowMacroBar.Changed -= this.ShowMacroBarOnChanged;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06004815 RID: 18453 RVA: 0x001A8AA0 File Offset: 0x001A6CA0
		private static bool TryGetDataKey(out string key)
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				string value = LocalPlayer.GameEntity.CharacterData.Name.Value;
				key = "MacroBarData_" + value;
				return true;
			}
			key = string.Empty;
			return false;
		}

		// Token: 0x06004816 RID: 18454 RVA: 0x001A8AF8 File Offset: 0x001A6CF8
		private static bool TryGetRotationKey(out string key)
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				string value = LocalPlayer.GameEntity.CharacterData.Name.Value;
				key = "MacroBarRotation_" + value;
				return true;
			}
			key = string.Empty;
			return false;
		}

		// Token: 0x06004817 RID: 18455 RVA: 0x001A8B50 File Offset: 0x001A6D50
		private void LoadData()
		{
			string key;
			if (this.m_gridLayout && MacroBarUI.TryGetRotationKey(out key))
			{
				int @int = PlayerPrefs.GetInt(key, 1);
				if (Enum.IsDefined(typeof(GridLayoutGroup.Constraint), @int))
				{
					this.m_gridLayout.constraint = (GridLayoutGroup.Constraint)@int;
				}
				this.UpdateRotationIcon();
			}
			string key2;
			if (MacroBarUI.TryGetDataKey(out key2))
			{
				string @string = PlayerPrefs.GetString(key2, string.Empty);
				if (!string.IsNullOrEmpty(@string))
				{
					try
					{
						this.m_macroData = JsonConvert.DeserializeObject<List<MacroData>>(@string);
					}
					catch (Exception arg)
					{
						Debug.LogWarning(string.Format("Cannot load data for Macros! {0}", arg));
					}
				}
				if (this.m_macroData == null)
				{
					this.m_macroData = new List<MacroData>(this.m_macroButtons.Length);
				}
				this.RefreshMacroButtons();
			}
			if (this.m_window)
			{
				base.StartCoroutine("DelayedRestoreWindowPositionAndSize");
			}
		}

		// Token: 0x06004818 RID: 18456 RVA: 0x0007089F File Offset: 0x0006EA9F
		private IEnumerator DelayedRestoreWindowPositionAndSize()
		{
			yield return null;
			if (this.m_window)
			{
				this.m_window.SaveWindowPositionSizeValue = this.m_savePosAndSize;
				this.m_window.RestoreWindowPositionSize();
			}
			yield break;
		}

		// Token: 0x06004819 RID: 18457 RVA: 0x000708AE File Offset: 0x0006EAAE
		private IEnumerator DelayedClampToScreen()
		{
			yield return null;
			if (this.m_window)
			{
				this.m_window.ClampToScreen(true);
			}
			yield break;
		}

		// Token: 0x0600481A RID: 18458 RVA: 0x001A8C30 File Offset: 0x001A6E30
		private void SaveData()
		{
			string key;
			if (MacroBarUI.TryGetDataKey(out key))
			{
				if (this.m_macroData == null || this.m_macroData.Count <= 0)
				{
					PlayerPrefs.DeleteKey(key);
					return;
				}
				try
				{
					string value = JsonConvert.SerializeObject(this.m_macroData);
					PlayerPrefs.SetString(key, value);
				}
				catch (Exception arg)
				{
					Debug.LogWarning(string.Format("Cannot save data for Macros! {0}", arg));
				}
			}
		}

		// Token: 0x0600481B RID: 18459 RVA: 0x001A8C9C File Offset: 0x001A6E9C
		private void InputManagerOnMacroActionPressed(int obj)
		{
			if (!this.m_window || !this.m_window.Visible)
			{
				return;
			}
			if (obj < this.m_macroButtons.Length)
			{
				MacroButton macroButton = this.m_macroButtons[obj];
				if (macroButton)
				{
					macroButton.ExecuteMacro();
				}
			}
		}

		// Token: 0x0600481C RID: 18460 RVA: 0x001A8CE8 File Offset: 0x001A6EE8
		private void ShowMacroBarOnChanged()
		{
			if (!this.m_window)
			{
				return;
			}
			if (Options.GameOptions.ShowMacroBar.Value)
			{
				if (!this.m_window.Visible)
				{
					this.m_window.Show(true);
					return;
				}
			}
			else if (this.m_window.Visible)
			{
				this.m_window.Hide(true);
			}
		}

		// Token: 0x0600481D RID: 18461 RVA: 0x000708BD File Offset: 0x0006EABD
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.LoadData();
		}

		// Token: 0x0600481E RID: 18462 RVA: 0x001A8D44 File Offset: 0x001A6F44
		private void OnRotateClicked()
		{
			if (this.m_gridLayout)
			{
				GridLayoutGroup.Constraint constraint = (this.m_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount) ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
				this.m_gridLayout.constraint = constraint;
				string key;
				if (MacroBarUI.TryGetRotationKey(out key))
				{
					PlayerPrefs.SetInt(key, (int)constraint);
				}
				this.UpdateRotationIcon();
				if (this.m_window)
				{
					base.StartCoroutine("DelayedClampToScreen");
				}
			}
		}

		// Token: 0x0600481F RID: 18463 RVA: 0x001A8DAC File Offset: 0x001A6FAC
		private void UpdateRotationIcon()
		{
			if (this.m_gridLayout && this.m_rotateButtonImage)
			{
				this.m_rotateButtonImage.sprite = ((this.m_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount) ? this.m_arrowRightSprite : this.m_arrowDownSprite);
			}
		}

		// Token: 0x06004820 RID: 18464 RVA: 0x001A8DFC File Offset: 0x001A6FFC
		private void AddButtonClicked()
		{
			if (this.m_macroData == null)
			{
				this.m_macroData = new List<MacroData>(this.m_macroButtons.Length)
				{
					new MacroData()
				};
				this.RefreshMacroButtons();
				this.SaveData();
				return;
			}
			if (this.m_macroData.Count < this.m_macroButtons.Length)
			{
				this.m_macroData.Add(new MacroData());
				this.RefreshMacroButtons();
				this.SaveData();
			}
		}

		// Token: 0x06004821 RID: 18465 RVA: 0x001A8E70 File Offset: 0x001A7070
		internal void DeleteMacro(MacroButton button)
		{
			if (button && button.Data != null && this.m_macroData.Remove(button.Data))
			{
				if (this.m_dataCurrentlyEditing != null && this.m_dataCurrentlyEditing == button.Data)
				{
					this.m_dataCurrentlyEditing = null;
				}
				this.RefreshMacroButtons();
				this.SaveData();
			}
		}

		// Token: 0x06004822 RID: 18466 RVA: 0x001A8ECC File Offset: 0x001A70CC
		internal void MoveTowardsFront(MacroButton button)
		{
			if (!button || this.m_macroData == null)
			{
				return;
			}
			for (int i = 0; i < this.m_macroData.Count; i++)
			{
				if (i > 0 && this.m_macroData[i] != null && this.m_macroData[i] == button.Data)
				{
					MacroData item = this.m_macroData[i];
					int index = i - 1;
					this.m_macroData.RemoveAt(i);
					this.m_macroData.Insert(index, item);
					this.RefreshMacroButtons();
					this.SaveData();
					return;
				}
			}
		}

		// Token: 0x06004823 RID: 18467 RVA: 0x001A8F60 File Offset: 0x001A7160
		internal void MoveTowardsBack(MacroButton button)
		{
			if (!button || this.m_macroData == null)
			{
				return;
			}
			for (int i = 0; i < this.m_macroData.Count; i++)
			{
				if (i < this.m_macroData.Count - 1 && this.m_macroData[i] != null && this.m_macroData[i] == button.Data)
				{
					MacroData item = this.m_macroData[i];
					int index = i + 1;
					this.m_macroData.RemoveAt(i);
					this.m_macroData.Insert(index, item);
					this.RefreshMacroButtons();
					this.SaveData();
					return;
				}
			}
		}

		// Token: 0x06004824 RID: 18468 RVA: 0x001A9000 File Offset: 0x001A7200
		internal void EditMacro(MacroButton button)
		{
			if (button && button.Data != null && ClientGameManager.UIManager && ClientGameManager.UIManager.MacroEditDialog)
			{
				this.m_dataCurrentlyEditing = button.Data;
				MacroEditDialogOptions opts = new MacroEditDialogOptions
				{
					Title = ZString.Format<int>("Edit Macro {0}", button.DisplayIndex),
					ConfirmationText = "Save",
					CancelText = "Cancel",
					Callback = delegate(bool result, object obj)
					{
						this.m_dataCurrentlyEditing = null;
						if (result)
						{
							ValueTuple<string, string> valueTuple = (ValueTuple<string, string>)obj;
							button.Data.DisplayName = valueTuple.Item1;
							button.Data.MacroText = valueTuple.Item2;
							this.SaveData();
							this.RefreshMacroButtons();
						}
					},
					AutoCancel = new Func<bool>(this.AutoCancel),
					Data = button.Data
				};
				ClientGameManager.UIManager.MacroEditDialog.Init(opts);
			}
		}

		// Token: 0x06004825 RID: 18469 RVA: 0x000708C5 File Offset: 0x0006EAC5
		private bool AutoCancel()
		{
			return this.m_dataCurrentlyEditing == null;
		}

		// Token: 0x06004826 RID: 18470 RVA: 0x001A90FC File Offset: 0x001A72FC
		private void RefreshMacroButtons()
		{
			for (int i = 0; i < this.m_macroButtons.Length; i++)
			{
				MacroData data = null;
				if (this.m_macroData != null && this.m_macroData.Count > i)
				{
					data = this.m_macroData[i];
				}
				this.m_macroButtons[i].Init(this, i, data);
			}
			if (this.m_window)
			{
				this.m_window.ClampToScreen(true);
			}
		}

		// Token: 0x06004827 RID: 18471 RVA: 0x000708D0 File Offset: 0x0006EAD0
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_macroData != null && this.m_macroData.Count > 0)
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, "Empty Macro Bar\nClick the + button to get started!", false);
		}

		// Token: 0x17000FFB RID: 4091
		// (get) Token: 0x06004828 RID: 18472 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000FFC RID: 4092
		// (get) Token: 0x06004829 RID: 18473 RVA: 0x000708FB File Offset: 0x0006EAFB
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000FFD RID: 4093
		// (get) Token: 0x0600482A RID: 18474 RVA: 0x00070909 File Offset: 0x0006EB09
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600482C RID: 18476 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004382 RID: 17282
		private const string kMacroDataKeyPrefix = "MacroBarData";

		// Token: 0x04004383 RID: 17283
		private const string kMacroRotationKeyPrefix = "MacroBarRotation";

		// Token: 0x04004384 RID: 17284
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004385 RID: 17285
		[SerializeField]
		private DraggableUIWindow m_window;

		// Token: 0x04004386 RID: 17286
		[SerializeField]
		private GridLayoutGroup m_gridLayout;

		// Token: 0x04004387 RID: 17287
		[SerializeField]
		private SolButton m_addButton;

		// Token: 0x04004388 RID: 17288
		[SerializeField]
		private SolButton m_rotateButton;

		// Token: 0x04004389 RID: 17289
		[SerializeField]
		private Image m_rotateButtonImage;

		// Token: 0x0400438A RID: 17290
		[SerializeField]
		private MacroButton[] m_macroButtons;

		// Token: 0x0400438B RID: 17291
		[SerializeField]
		private Sprite m_arrowRightSprite;

		// Token: 0x0400438C RID: 17292
		[SerializeField]
		private Sprite m_arrowDownSprite;

		// Token: 0x0400438D RID: 17293
		private List<MacroData> m_macroData;

		// Token: 0x0400438E RID: 17294
		private bool m_savePosAndSize;

		// Token: 0x0400438F RID: 17295
		private MacroData m_dataCurrentlyEditing;
	}
}
