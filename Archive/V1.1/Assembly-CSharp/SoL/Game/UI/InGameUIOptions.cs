using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using Expanse;
using SoL.Game.Audio;
using SoL.Game.Culling;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000895 RID: 2197
	public class InGameUIOptions : UIWindow
	{
		// Token: 0x06003FFA RID: 16378 RVA: 0x0018A228 File Offset: 0x00188428
		protected override void Awake()
		{
			base.Awake();
			if (PlayerPrefs.GetInt("ResetMouseInvert", 0) == 0)
			{
				Options.GameOptions.InvertMouse.Value = false;
				PlayerPrefs.SetInt("ResetMouseInvert", 1);
			}
			this.m_toggleHelpers = new List<InGameUIOptions.BoolToggleHelper>
			{
				new InGameUIOptions.BoolToggleHelper(this.m_mouseInvertToggle, Options.GameOptions.InvertMouse),
				new InGameUIOptions.BoolToggleHelper(this.m_escapeTogglesMenu, Options.GameOptions.EscapeTogglesMenu),
				new InGameUIOptions.BoolToggleHelper(this.m_allowSelfTargetToggle, Options.GameOptions.AllowSelfTarget),
				new InGameUIOptions.BoolToggleHelper(this.m_enableCameraDampingToggle, Options.GameOptions.EnableCameraDamping),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadCombatTextToggle, Options.GameOptions.ShowOverheadCombatText),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadChatToggle, Options.GameOptions.ShowOverheadChat),
				new InGameUIOptions.BoolToggleHelper(this.m_disableAutoAttackOnTargetDeathToggle, Options.GameOptions.DisableAutoAttackOnTargetDeath),
				new InGameUIOptions.BoolToggleHelper(this.m_leaveCombatAfterLootingToggle, Options.GameOptions.LeaveCombatAfterLooting),
				new InGameUIOptions.BoolToggleHelper(this.m_leaveCombatAfterCombatFlagDropToggle, Options.GameOptions.LeaveCombatAfterCombatFlagDrop),
				new InGameUIOptions.BoolToggleHelper(this.m_disableTooltipForActionBarToggle, Options.GameOptions.DisableTooltipForActionBarAbilities),
				new InGameUIOptions.BoolToggleHelper(this.m_openBagAndGatheringTogetherToggle, Options.GameOptions.OpenBagAndGatheringTogether),
				new InGameUIOptions.BoolToggleHelper(this.m_hideTutorialPopups, Options.GameOptions.HideTutorialPopups),
				new InGameUIOptions.BoolToggleHelper(this.m_disablePlayerWorldSpaceContextMenus, Options.GameOptions.DisablePlayerWorldSpaceContextMenus),
				new InGameUIOptions.BoolToggleHelper(this.m_disablePlayerWorldSpaceContextMenusWeaponsDrawn, Options.GameOptions.DisablePlayerWorldSpaceContextMenusWeaponsDrawn),
				new InGameUIOptions.BoolToggleHelper(this.m_crouchIsToggleToggle, Options.GameOptions.CrouchIsToggle),
				new InGameUIOptions.BoolToggleHelper(this.m_alternateLearningAnimationsToggle, Options.GameOptions.AlternateLearningAnimation),
				new InGameUIOptions.BoolToggleHelper(this.m_enableZoomToFirstPersonToggle, Options.GameOptions.EnableZoomToFirstPerson),
				new InGameUIOptions.BoolToggleHelper(this.m_showGelIndicatorsToggle, Options.GameOptions.ShowGelIndicators),
				new InGameUIOptions.BoolToggleHelper(this.m_enableClothSim, Options.GameOptions.EnableClothSim),
				new InGameUIOptions.BoolToggleHelper(this.m_deselectSingleTargetOnTabToggle, Options.GameOptions.DeselectSingleTargetOnTab),
				new InGameUIOptions.BoolToggleHelper(this.m_grassShadowToggle, Options.VideoOptions.GrassShadows),
				new InGameUIOptions.BoolToggleHelper(this.m_highResSunShadowToggle, Options.VideoOptions.HighResolutionSunShadows),
				new InGameUIOptions.BoolToggleHelper(this.m_contactShadowToggle, Options.VideoOptions.ContactShadows),
				new InGameUIOptions.BoolToggleHelper(this.m_reflectionToggle, Options.VideoOptions.Reflections),
				new InGameUIOptions.BoolToggleHelper(this.m_cameraShakeToggle, Options.VideoOptions.CameraShake),
				new InGameUIOptions.BoolToggleHelper(this.m_tellAudioToggle, Options.AudioOptions.AudioOnTell),
				new InGameUIOptions.BoolToggleHelper(this.m_friendAudioToggle, Options.AudioOptions.AudioOnFriendStatusUpdate),
				new InGameUIOptions.BoolToggleHelper(this.m_guildAudioToggle, Options.AudioOptions.AudioOnGuildStatusUpdate),
				new InGameUIOptions.BoolToggleHelper(this.m_alternateJumpAudioToggle, Options.AudioOptions.AlternateJumpAudio),
				new InGameUIOptions.BoolToggleHelper(this.m_audioListenerAtPlayerToggle, Options.AudioOptions.AudioListenerAtPlayer),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadNameSelf, Options.GameOptions.ShowOverheadNameplate_Self),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadNameGroup, Options.GameOptions.ShowOverheadNameplate_Group),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadNameGuild, Options.GameOptions.ShowOverheadNameplate_Guild),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadNameOtherPlayers, Options.GameOptions.ShowOverheadNameplate_OtherPlayers),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadNameNpcs, Options.GameOptions.ShowOverheadNameplate_Npcs),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadNameResourceNode, Options.GameOptions.ShowOverheadNameplate_ResourceNode),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadStatSelf, Options.GameOptions.ShowOverheadStatBar_Self),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadStatGroup, Options.GameOptions.ShowOverheadStatBar_Group),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadStatGuild, Options.GameOptions.ShowOverheadStatBar_Guild),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadStatOtherPlayers, Options.GameOptions.ShowOverheadStatBar_OtherPlayers),
				new InGameUIOptions.BoolToggleHelper(this.m_showOverheadStatNpcs, Options.GameOptions.ShowOverheadStatBar_Npcs),
				new InGameUIOptions.BoolToggleHelper(this.m_autoPassKnownRecipesToggle, Options.GameOptions.AutoPassKnownRecipes),
				new InGameUIOptions.BoolToggleHelper(this.m_autoPassNonUsableReagentToggle, Options.GameOptions.AutoPassNonUsableReagents),
				new InGameUIOptions.BoolToggleHelper(this.m_useImposterBillboards, Options.VideoOptions.UseImposterBillboards),
				new InGameUIOptions.BoolToggleHelper(this.m_useTargetAtExecutionCompleteToggle, Options.GameOptions.UseTargetAtExecutionComplete),
				new InGameUIOptions.BoolToggleHelper(this.m_keepAlchemyActiveOnCancelToggle, Options.GameOptions.KeepAlchemyActiveOnCancel),
				new InGameUIOptions.BoolToggleHelper(this.m_hailOffensiveTargetFirstToggle, Options.GameOptions.HailOffensiveTargetFirst),
				new InGameUIOptions.BoolToggleHelper(this.m_showUiCompassToggle, Options.GameOptions.ShowUiCompass),
				new InGameUIOptions.BoolToggleHelper(this.m_holdKeybindToShowCompassToggle, Options.GameOptions.HoldKeybindToShowCompass),
				new InGameUIOptions.BoolToggleHelper(this.m_showZoneNameOnCompassToggle, Options.GameOptions.ShowZoneNameOnCompass),
				new InGameUIOptions.BoolToggleHelper(this.m_showMacroBarToggle, Options.GameOptions.ShowMacroBar),
				new InGameUIOptions.BoolToggleHelper(this.m_showBidConfirmationToggle, Options.GameOptions.ShowBidConfirmation)
			};
			if (SoL.Game.Settings.GlobalSettings.kAudioListenerAlwaysAtPlayer)
			{
				this.m_audioListenerAtPlayerToggle.gameObject.SetActive(false);
			}
			this.m_mouseSensitivityX.Init(Options.GameOptions.XMouseSensitivity);
			this.m_mouseSensitivityY.Init(Options.GameOptions.YMouseSensitivity);
			this.m_jobWorkerCountSlider.Init(Options.GameOptions.SelectedJobWorkerMaximumCount);
			this.m_dialogSpeedSlider.Init(Options.GameOptions.DialogSpeed);
			this.m_gameUiScaleSlider.GameUICanvas = this.m_gameUICanvas;
			this.m_gameUiScaleSlider.Init(Options.GameOptions.GameUIScale);
			this.m_tooltipDelaySlider.Init(Options.GameOptions.TooltipDelay);
			this.m_chatFontSizeSlider.Init(Options.GameOptions.ChatFontSize);
			this.m_overheadNameplateScaleSlider.Init(Options.GameOptions.OverheadNameplateScale);
			this.m_fovSlider.Init(Options.VideoOptions.CameraFieldOfView);
			Options.GameOptions.ShowOverheadNameplates.Changed += this.ShowOverheadOverheadNameplatesChanged;
			this.m_showOverheadNameplatesToggle.isOn = Options.GameOptions.ShowOverheadNameplates.Value;
			this.m_showOverheadNameplatesToggle.onValueChanged.AddListener(new UnityAction<bool>(this.ShowOverheadNameplatesToggleChanged));
			List<string> list = new List<string>
			{
				"None",
				ZString.Format<string, string>("<color={0}>{1}</color> Greed", SoL.Game.Settings.GlobalSettings.Values.Group.LootRollColorGreed.ToHex(), "<sprite=\"SolIcons\" name=\"GreedIcon\" tint=1>"),
				ZString.Format<string, string>("<color={0}>{1}</color> Pass", SoL.Game.Settings.GlobalSettings.Values.Group.LootRollColorPass.ToHex(), "<sprite=\"SolIcons\" name=\"PassIcon\" tint=1>")
			};
			this.m_autoRollKnownRecipesDropdown.options.Clear();
			this.m_autoRollKnownRecipesDropdown.AddOptions(list);
			this.m_autoRollKnownRecipesDropdown.value = Options.GameOptions.AutoRollKnownRecipes.Value;
			this.m_autoRollKnownRecipesDropdown.onValueChanged.AddListener(new UnityAction<int>(this.AutoRollKnownRecipesChanged));
			this.m_autoRollNonUsableRecipesDropdown.options.Clear();
			this.m_autoRollNonUsableRecipesDropdown.AddOptions(list);
			this.m_autoRollNonUsableRecipesDropdown.value = Options.GameOptions.AutoRollNonUsableRecipes.Value;
			this.m_autoRollNonUsableRecipesDropdown.onValueChanged.AddListener(new UnityAction<int>(this.AutoRollNonUsableRecipesChanged));
			this.m_autoRollNonUsableReagentsDropdown.options.Clear();
			this.m_autoRollNonUsableReagentsDropdown.AddOptions(list);
			this.m_autoRollNonUsableReagentsDropdown.value = Options.GameOptions.AutoRollNonUsableReagents.Value;
			this.m_autoRollNonUsableReagentsDropdown.onValueChanged.AddListener(new UnityAction<int>(this.AutoRollNonUsableReagentsChanged));
			this.m_autoRollBasicItemsDropdown.options.Clear();
			this.m_autoRollBasicItemsDropdown.AddOptions(list);
			this.m_autoRollBasicItemsDropdown.value = Options.GameOptions.AutoRollBasicItems.Value;
			this.m_autoRollBasicItemsDropdown.onValueChanged.AddListener(new UnityAction<int>(this.AutoRollBasicItemsChanged));
			list.Clear();
			int value = Options.AudioOptions.AudioOnTellOption.DefaultValue;
			for (int i = 0; i < TellAudio.TellAudioTypes.Length; i++)
			{
				list.Add(TellAudio.TellAudioTypes[i].ToString());
				if (Options.AudioOptions.AudioOnTellOption.Value == (int)TellAudio.TellAudioTypes[i])
				{
					value = i;
				}
			}
			this.m_tellAudioDropdown.options.Clear();
			this.m_tellAudioDropdown.AddOptions(list);
			this.m_tellAudioDropdown.value = value;
			this.m_tellAudioDropdown.onValueChanged.AddListener(new UnityAction<int>(this.TellAudioDropdownChanged));
			this.m_showOverheadNameplatesToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnOverheadToggleChanged));
			this.RefreshOverheadToggleInteractivity();
		}

		// Token: 0x06003FFB RID: 16379 RVA: 0x0018AA34 File Offset: 0x00188C34
		protected override void Start()
		{
			this.SetupVideoOptions();
			this.SetupResolutionDropdown();
			for (int i = 0; i < this.m_volumeSliders.Length; i++)
			{
				this.m_volumeSliders[i].VolumeSliderInit();
			}
			if (this.m_toggleAllWindowsSettings != null)
			{
				for (int j = 0; j < this.m_toggleAllWindowsSettings.Length; j++)
				{
					ToggleAllWindowsSetting toggleAllWindowsSetting = this.m_toggleAllWindowsSettings[j];
					if (toggleAllWindowsSetting != null)
					{
						toggleAllWindowsSetting.Init();
					}
				}
			}
		}

		// Token: 0x06003FFC RID: 16380 RVA: 0x0018AA9C File Offset: 0x00188C9C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_qualityDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnQualityChanged));
			this.m_fullScreenToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnFullScreenChanged));
			this.m_alternateFullScreenToggleWindows.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnAlternateFullSCreenToggleChanged));
			this.m_resolutionDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnResolutionChanged));
			this.m_antialiasingDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.AntiAliasingDropdownChanged));
			this.m_frameRateLimiterDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.FrameRateLimiterDropdownChanged));
			this.m_vegetationDensityDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.VegetationDensityDropdownChanged));
			this.m_cloudQualityDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.CloudQualityDropdownChanged));
			this.m_showOverheadNameplatesToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ShowOverheadNameplatesToggleChanged));
			this.m_umaResolutionDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.UmaResolutionChanged));
			this.m_volumetricsQualityDropddown.onValueChanged.RemoveListener(new UnityAction<int>(this.VolumetricQualityChanged));
			this.m_treeImposterDistance.onValueChanged.RemoveListener(new UnityAction<int>(this.TreeImposterDistanceChanged));
			Options.GameOptions.ShowOverheadNameplates.Changed -= this.ShowOverheadOverheadNameplatesChanged;
			this.m_mouseSensitivityX.OnDestroy();
			this.m_mouseSensitivityY.OnDestroy();
			this.m_dialogSpeedSlider.OnDestroy();
			this.m_gameUiScaleSlider.OnDestroy();
			this.m_jobWorkerCountSlider.OnDestroy();
			this.m_tooltipDelaySlider.OnDestroy();
			this.m_maxShadowCastingLights.OnDestroy();
			this.m_shadowDistance.OnDestroy();
			this.m_viewDistance.OnDestroy();
			this.m_vegetationDistance.OnDestroy();
			this.m_treeDistance.OnDestroy();
			if (NvidiaDLSS.SupportsNvidiaDLSS)
			{
				this.m_dlssToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.NvidiaDLSSEnabledChanged));
				this.m_dlssDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.NvidiaDLSSQualityChanged));
			}
			else
			{
				this.m_resolutionScale.OnDestroy();
			}
			this.m_exposureAdjustment.OnDestroy();
			for (int i = 0; i < this.m_volumeSliders.Length; i++)
			{
				this.m_volumeSliders[i].OnDestroy();
			}
			for (int j = 0; j < this.m_toggleHelpers.Count; j++)
			{
				this.m_toggleHelpers[j].OnDestroy();
			}
			this.m_showOverheadNameplatesToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnOverheadToggleChanged));
			this.m_autoRollKnownRecipesDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.AutoRollKnownRecipesChanged));
			this.m_autoRollNonUsableRecipesDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.AutoRollNonUsableRecipesChanged));
			this.m_autoRollNonUsableReagentsDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.AutoRollNonUsableReagentsChanged));
			this.m_autoRollBasicItemsDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.AutoRollBasicItemsChanged));
			this.m_tellAudioDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.TellAudioDropdownChanged));
			if (this.m_toggleAllWindowsSettings != null)
			{
				for (int k = 0; k < this.m_toggleAllWindowsSettings.Length; k++)
				{
					ToggleAllWindowsSetting toggleAllWindowsSetting = this.m_toggleAllWindowsSettings[k];
					if (toggleAllWindowsSetting != null)
					{
						toggleAllWindowsSetting.Destroy();
					}
				}
			}
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x0018AE04 File Offset: 0x00189004
		private void Update()
		{
			if (this.m_fullScreenToggle.isOn != Screen.fullScreen)
			{
				this.m_preventFullscreenToggleAction = true;
				this.m_fullScreenToggle.isOn = Screen.fullScreen;
				this.m_preventFullscreenToggleAction = false;
			}
			if (Screen.width != this.m_currentResolution.width || Screen.height != this.m_currentResolution.height || !Screen.currentResolution.refreshRateRatio.Equals(this.m_currentResolution.refreshRateRatio))
			{
				this.m_currentResolution = new Resolution
				{
					width = Screen.width,
					height = Screen.height,
					refreshRateRatio = Screen.currentResolution.refreshRateRatio
				};
				Options.VideoOptions.CustomResolution = this.m_currentResolution;
				this.m_setResolutionFrame = new int?(Time.frameCount + 2);
				return;
			}
			if (this.m_setResolutionFrame != null && Time.frameCount >= this.m_setResolutionFrame.Value)
			{
				this.SetResolution();
				this.RefreshResolutionDropdownOptions();
				this.m_setResolutionFrame = null;
			}
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x0006B429 File Offset: 0x00069629
		public override void Show(bool skipTransition = false)
		{
			IInputManager inputManager = ClientGameManager.InputManager;
			if (inputManager != null)
			{
				inputManager.SetInputPreventionFlag(InputPreventionFlags.Options);
			}
			this.m_optionsShown = true;
			base.Show(skipTransition);
		}

		// Token: 0x06003FFF RID: 16383 RVA: 0x0006B44B File Offset: 0x0006964B
		public override void Hide(bool skipTransition = false)
		{
			IInputManager inputManager = ClientGameManager.InputManager;
			if (inputManager != null)
			{
				inputManager.UnsetInputPreventionFlag(InputPreventionFlags.Options);
			}
			this.m_optionsShown = false;
			this.m_keybindsUI.StopBinding();
			base.Hide(skipTransition);
		}

		// Token: 0x06004000 RID: 16384 RVA: 0x0018AF18 File Offset: 0x00189118
		private void SetupVideoOptions()
		{
			this.m_maxShadowCastingLights.Init(Options.VideoOptions.MaxShadowCastingLights);
			this.m_shadowDistance.Init(Options.VideoOptions.ShadowDistance);
			this.m_viewDistance.Init(Options.VideoOptions.ViewDistance);
			this.m_vegetationDistance.Init(Options.VideoOptions.VegetationDistance);
			this.m_treeDistance.Init(Options.VideoOptions.TreeDistance);
			this.m_resolutionScale.Init(Options.VideoOptions.ResolutionScale);
			if (NvidiaDLSS.SupportsNvidiaDLSS)
			{
				this.m_dlssPanel.SetActive(true);
				this.m_dlssToggle.isOn = Options.VideoOptions.NvidiaDLSSEnable.Value;
				this.m_dlssToggle.onValueChanged.AddListener(new UnityAction<bool>(this.NvidiaDLSSEnabledChanged));
				this.m_dlssDropdown.ClearOptions();
				this.m_dlssDropdown.AddOptions(NvidiaDLSS.QualityNames.ToList<string>());
				this.m_dlssDropdown.value = NvidiaDLSS.GetOptionValue();
				this.m_dlssDropdown.onValueChanged.AddListener(new UnityAction<int>(this.NvidiaDLSSQualityChanged));
				this.m_dlssDropdown.interactable = this.m_dlssToggle.isOn;
				this.m_resolutionScale.Slider.SetInteractable(!this.m_dlssToggle.isOn);
			}
			else
			{
				this.m_dlssPanel.SetActive(false);
				this.m_resolutionScale.Slider.SetInteractable(true);
			}
			this.m_exposureAdjustment.Init(Options.VideoOptions.ExposureAdjustment);
			this.m_qualityDropdown.ClearOptions();
			List<string> list = QualitySettings.names.ToList<string>();
			if (Options.VideoOptions.QualitySetting.Value >= list.Count || Options.VideoOptions.QualitySetting.Value < 0)
			{
				Options.VideoOptions.QualitySetting.Value = 1;
			}
			this.m_qualityDropdown.AddOptions(list);
			this.m_qualityDropdown.value = Options.VideoOptions.QualitySetting.Value;
			this.m_qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnQualityChanged));
			this.OnQualityChanged(this.m_qualityDropdown.value);
			this.m_fullScreenToggle.isOn = Options.VideoOptions.FullScreen.Value;
			this.m_fullScreenToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnFullScreenChanged));
			this.m_alternateFullScreenToggleMacOS.gameObject.SetActive(false);
			this.m_alternateFullScreenToggleWindows.gameObject.SetActive(true);
			this.m_alternateFullScreenToggleWindows.isOn = Options.VideoOptions.AlternateFullScreenMode.Value;
			this.m_alternateFullScreenToggleWindows.interactable = this.m_fullScreenToggle.isOn;
			this.m_alternateFullScreenToggleWindows.onValueChanged.AddListener(new UnityAction<bool>(this.OnAlternateFullSCreenToggleChanged));
			this.m_bloomToggle.gameObject.SetActive(false);
			this.SetResolution();
			AntiAliasingTypeInternalExtensions.ValidateAntiAliasingSelection();
			this.m_antialiasingDropdown.ClearOptions();
			this.m_antialiasingDropdown.AddOptions(AntiAliasingTypeInternalExtensions.GetTypeStrings());
			this.m_antialiasingDropdown.value = Options.VideoOptions.AntiAliasingType.Value;
			this.m_antialiasingDropdown.onValueChanged.AddListener(new UnityAction<int>(this.AntiAliasingDropdownChanged));
			UMAGlibManager.ValidateUmaResolution();
			this.m_umaResolutionDropdown.ClearOptions();
			this.m_umaResolutionDropdown.AddOptions(UMAGlibManager.UmaResolutionOptions);
			this.m_umaResolutionDropdown.value = Options.VideoOptions.UMAResolution.Value;
			this.m_umaResolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.UmaResolutionChanged));
			this.m_volumetricsQualityDropddown.ClearOptions();
			this.m_volumetricsQualityDropddown.AddOptions(new List<string>
			{
				"Low",
				"Medium",
				"High"
			});
			this.m_volumetricsQualityDropddown.value = ((Options.VideoOptions.VolumetricQuality.Value < 3) ? Options.VideoOptions.VolumetricQuality.Value : 1);
			this.m_volumetricsQualityDropddown.onValueChanged.AddListener(new UnityAction<int>(this.VolumetricQualityChanged));
			this.m_treeImposterDistance.ClearOptions();
			this.m_treeImposterDistance.AddOptions(new List<string>
			{
				ZString.Format<string, int>("{0} ({1}m)", TreeImposterCullingDistance.Low.ToString(), Mathf.FloorToInt(TreeImposterCullingDistance.Low.GetDistance())),
				ZString.Format<string, int>("{0} ({1}m)", TreeImposterCullingDistance.Medium.ToString(), Mathf.FloorToInt(TreeImposterCullingDistance.Medium.GetDistance())),
				ZString.Format<string, int>("{0} ({1}m)", TreeImposterCullingDistance.High.ToString(), Mathf.FloorToInt(TreeImposterCullingDistance.High.GetDistance())),
				ZString.Format<string, int>("{0} ({1}m)", TreeImposterCullingDistance.Ultra.ToString(), Mathf.FloorToInt(TreeImposterCullingDistance.Ultra.GetDistance()))
			});
			this.m_treeImposterDistance.value = ((Options.VideoOptions.TreeImposterDistance.Value < 4) ? Options.VideoOptions.TreeImposterDistance.Value : 1);
			this.m_treeImposterDistance.onValueChanged.AddListener(new UnityAction<int>(this.TreeImposterDistanceChanged));
			this.SetupFrameRateLimiterDropdown();
			this.SetupVegetationDensityDropdown();
			this.SetupCloudSettingsDropdown();
		}

		// Token: 0x06004001 RID: 16385 RVA: 0x0006B478 File Offset: 0x00069678
		private string GetDisplayForVegetationDensity(InGameUIOptions.VegetationDensityValue setting)
		{
			if (setting == InGameUIOptions.VegetationDensityValue.VeryLow)
			{
				return "Very Low";
			}
			return setting.ToString();
		}

		// Token: 0x06004002 RID: 16386 RVA: 0x0018B3F0 File Offset: 0x001895F0
		private void SetupVegetationDensityDropdown()
		{
			this.m_vegetationDensityDropdown.ClearOptions();
			InGameUIOptions.VegetationDensityValue[] array = (InGameUIOptions.VegetationDensityValue[])Enum.GetValues(typeof(InGameUIOptions.VegetationDensityValue));
			this.m_vegetationDensityValues = new Dictionary<int, InGameUIOptions.VegetationDensityValue>(array.Length);
			int? num = null;
			int num2 = 0;
			List<string> list = new List<string>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				this.m_vegetationDensityValues.Add(i, array[i]);
				list.Add(this.GetDisplayForVegetationDensity(array[i]));
				if (array[i] == InGameUIOptions.VegetationDensityValue.Medium)
				{
					num2 = i;
				}
				if (array[i] == (InGameUIOptions.VegetationDensityValue)Options.VideoOptions.VegetationDensity.Value)
				{
					num = new int?(i);
				}
			}
			if (num == null && (Options.VideoOptions.VegetationDensity.Value > (int)array[array.Length - 1] || Options.VideoOptions.VegetationDensity.Value < (int)array[0]))
			{
				Options.VideoOptions.VegetationDensity.Value = 75;
			}
			this.m_vegetationDensityDropdown.AddOptions(list);
			this.m_vegetationDensityDropdown.value = ((num != null) ? num.Value : num2);
			this.m_vegetationDensityDropdown.onValueChanged.AddListener(new UnityAction<int>(this.VegetationDensityDropdownChanged));
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x0018B514 File Offset: 0x00189714
		private void VegetationDensityDropdownChanged(int arg0)
		{
			int value = 100;
			InGameUIOptions.VegetationDensityValue vegetationDensityValue;
			if (this.m_vegetationDensityValues.TryGetValue(arg0, out vegetationDensityValue))
			{
				value = (int)vegetationDensityValue;
			}
			Options.VideoOptions.VegetationDensity.Value = value;
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x0018B544 File Offset: 0x00189744
		private void SetupCloudSettingsDropdown()
		{
			this.m_cloudQualityDropdown.ClearOptions();
			Datatypes.Quality[] availableQualities = ExpanseExtensions.AvailableQualities;
			List<string> list = new List<string>(availableQualities.Length);
			int value = 2;
			if (Options.VideoOptions.CloudQuality.Value < 0 || Options.VideoOptions.CloudQuality.Value >= availableQualities.Length)
			{
				Options.VideoOptions.CloudQuality.Value = Mathf.Clamp(Options.VideoOptions.CloudQuality.Value, 0, availableQualities.Length - 1);
			}
			for (int i = 0; i < availableQualities.Length; i++)
			{
				if (availableQualities[i] == (Datatypes.Quality)Options.VideoOptions.CloudQuality.Value)
				{
					value = (int)availableQualities[i];
				}
				list.Add(availableQualities[i].GetDescription());
			}
			this.m_cloudQualityDropdown.AddOptions(list);
			this.m_cloudQualityDropdown.value = value;
			this.m_cloudQualityDropdown.onValueChanged.AddListener(new UnityAction<int>(this.CloudQualityDropdownChanged));
		}

		// Token: 0x06004005 RID: 16389 RVA: 0x0006B492 File Offset: 0x00069692
		private void CloudQualityDropdownChanged(int value)
		{
			Options.VideoOptions.CloudQuality.Value = value;
		}

		// Token: 0x06004006 RID: 16390 RVA: 0x0006B49F File Offset: 0x0006969F
		private void AntiAliasingDropdownChanged(int value)
		{
			Options.VideoOptions.AntiAliasingType.Value = value;
		}

		// Token: 0x06004007 RID: 16391 RVA: 0x0006B4AC File Offset: 0x000696AC
		private void UmaResolutionChanged(int arg0)
		{
			Options.VideoOptions.UMAResolution.Value = arg0;
		}

		// Token: 0x06004008 RID: 16392 RVA: 0x0006B4B9 File Offset: 0x000696B9
		private void VolumetricQualityChanged(int arg0)
		{
			Options.VideoOptions.VolumetricQuality.Value = arg0;
		}

		// Token: 0x06004009 RID: 16393 RVA: 0x0006B4C6 File Offset: 0x000696C6
		private void TreeImposterDistanceChanged(int arg0)
		{
			Options.VideoOptions.TreeImposterDistance.Value = arg0;
		}

		// Token: 0x0600400A RID: 16394 RVA: 0x0006B4D3 File Offset: 0x000696D3
		private void NvidiaDLSSQualityChanged(int arg0)
		{
			Options.VideoOptions.NvidiaDLSSQuality.Value = arg0;
		}

		// Token: 0x0600400B RID: 16395 RVA: 0x0006B4E0 File Offset: 0x000696E0
		private void NvidiaDLSSEnabledChanged(bool isEnabled)
		{
			Options.VideoOptions.NvidiaDLSSEnable.Value = isEnabled;
			this.m_dlssDropdown.interactable = isEnabled;
			this.m_resolutionScale.Slider.SetInteractable(!isEnabled);
		}

		// Token: 0x0600400C RID: 16396 RVA: 0x0006B50D File Offset: 0x0006970D
		public void ShowHideClicked()
		{
			this.m_optionsShown = !this.m_optionsShown;
			if (this.m_optionsShown)
			{
				this.Show(false);
				return;
			}
			this.Hide(false);
		}

		// Token: 0x0600400D RID: 16397 RVA: 0x0006B535 File Offset: 0x00069735
		private void ShowOverheadNameplatesToggleChanged(bool arg0)
		{
			this.m_preventShowNameplateTrigger = true;
			Options.GameOptions.ShowOverheadNameplates.Value = arg0;
			this.m_preventShowNameplateTrigger = false;
		}

		// Token: 0x0600400E RID: 16398 RVA: 0x0006B550 File Offset: 0x00069750
		private void ShowOverheadOverheadNameplatesChanged()
		{
			if (this.m_preventShowNameplateTrigger)
			{
				return;
			}
			this.m_showOverheadNameplatesToggle.isOn = Options.GameOptions.ShowOverheadNameplates.Value;
		}

		// Token: 0x0600400F RID: 16399 RVA: 0x0018B60C File Offset: 0x0018980C
		public bool EscapePressedFirstPass()
		{
			if (!this.m_optionsShown)
			{
				return false;
			}
			if (this.m_keybindsUI && this.m_keybindsUI.IsBinding())
			{
				this.m_keybindsUI.StopBinding();
				return true;
			}
			if (this.m_optionsShown)
			{
				this.ShowHideClicked();
				return true;
			}
			return false;
		}

		// Token: 0x06004010 RID: 16400 RVA: 0x0006B570 File Offset: 0x00069770
		private void OnQualityChanged(int arg0)
		{
			Options.VideoOptions.QualitySetting.Value = arg0;
			if (arg0 != QualitySettings.GetQualityLevel())
			{
				QualitySettings.SetQualityLevel(arg0);
				this.m_shadowDistance.TriggerAdjustShadowsForRange();
			}
		}

		// Token: 0x06004011 RID: 16401 RVA: 0x0018B65C File Offset: 0x0018985C
		private void OnResolutionChanged(int arg0)
		{
			Resolution customResolution;
			if (!this.m_preventResolutionChangedAction && (this.m_customResolutionIndex == null || this.m_customResolutionIndex.Value != arg0) && this.m_resolutionDictionary != null && this.m_resolutionDictionary.TryGetValue(arg0, out customResolution))
			{
				Options.VideoOptions.CustomResolution = customResolution;
				this.SetResolution();
			}
		}

		// Token: 0x06004012 RID: 16402 RVA: 0x0006B596 File Offset: 0x00069796
		private void OnFullScreenChanged(bool arg0)
		{
			if (!this.m_preventFullscreenToggleAction)
			{
				Options.VideoOptions.FullScreen.Value = arg0;
				this.SetResolution();
			}
			this.m_alternateFullScreenToggleWindows.interactable = arg0;
		}

		// Token: 0x06004013 RID: 16403 RVA: 0x0006B5BD File Offset: 0x000697BD
		private void OnAlternateFullSCreenToggleChanged(bool arg0)
		{
			Options.VideoOptions.AlternateFullScreenMode.Value = arg0;
			this.SetResolution();
		}

		// Token: 0x06004014 RID: 16404 RVA: 0x0006B5D0 File Offset: 0x000697D0
		private void OnVSyncChanged(bool arg0)
		{
			Options.VideoOptions.VSync.Value = arg0;
			QualitySettings.vSyncCount = (arg0 ? 1 : 0);
		}

		// Token: 0x06004015 RID: 16405 RVA: 0x0006B5E9 File Offset: 0x000697E9
		private void SetupResolutionDropdown()
		{
			this.RefreshResolutionDropdownOptions();
			this.m_resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionChanged));
		}

		// Token: 0x06004016 RID: 16406 RVA: 0x0018B6B0 File Offset: 0x001898B0
		private void RefreshResolutionDropdownOptions()
		{
			if (this.m_resolutionDictionary == null)
			{
				this.m_resolutionDictionary = new Dictionary<int, Resolution>(100);
				this.m_resolutionStrings = new List<string>(100);
			}
			this.m_customResolutionIndex = null;
			this.m_resolutionDictionary.Clear();
			this.m_resolutionStrings.Clear();
			this.m_preventResolutionChangedAction = true;
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				this.m_resolutionStrings.Add(ZString.Format<int, int, float>("{0} x {1} @ {2:0.##}Hz", resolutions[i].width, resolutions[i].height, (float)resolutions[i].refreshRateRatio.value));
				this.m_resolutionDictionary.Add(i, resolutions[i]);
			}
			int? num = null;
			Resolution customResolution = Options.VideoOptions.CustomResolution;
			for (int j = 0; j < resolutions.Length; j++)
			{
				if (customResolution.width == resolutions[j].width && customResolution.height == resolutions[j].height && customResolution.refreshRateRatio.Equals(resolutions[j].refreshRateRatio))
				{
					num = new int?(j);
					break;
				}
			}
			if (num == null)
			{
				this.m_resolutionStrings.Add(customResolution.ToString());
				num = new int?(this.m_resolutionStrings.Count - 1);
				this.m_customResolutionIndex = new int?(num.Value);
				this.m_resolutionDictionary.Add(num.Value, customResolution);
			}
			this.m_resolutionDropdown.ClearOptions();
			this.m_resolutionDropdown.AddOptions(this.m_resolutionStrings);
			this.m_resolutionDropdown.value = num.Value;
			this.m_preventResolutionChangedAction = false;
		}

		// Token: 0x06004017 RID: 16407 RVA: 0x0018B878 File Offset: 0x00189A78
		private void SetResolution()
		{
			FullScreenMode fullscreenMode = FullScreenMode.Windowed;
			if (Options.VideoOptions.FullScreen.Value)
			{
				fullscreenMode = FullScreenMode.FullScreenWindow;
				if (Options.VideoOptions.AlternateFullScreenMode.Value)
				{
					fullscreenMode = FullScreenMode.ExclusiveFullScreen;
				}
			}
			Resolution customResolution = Options.VideoOptions.CustomResolution;
			Screen.SetResolution(customResolution.width, customResolution.height, fullscreenMode, customResolution.refreshRateRatio);
		}

		// Token: 0x06004018 RID: 16408 RVA: 0x0006B60D File Offset: 0x0006980D
		private void OnOverheadToggleChanged(bool arg0)
		{
			this.RefreshOverheadToggleInteractivity();
		}

		// Token: 0x06004019 RID: 16409 RVA: 0x0006B615 File Offset: 0x00069815
		private void AutoRollKnownRecipesChanged(int arg0)
		{
			Options.GameOptions.AutoRollKnownRecipes.Value = arg0;
		}

		// Token: 0x0600401A RID: 16410 RVA: 0x0006B622 File Offset: 0x00069822
		private void AutoRollNonUsableRecipesChanged(int arg0)
		{
			Options.GameOptions.AutoRollNonUsableRecipes.Value = arg0;
		}

		// Token: 0x0600401B RID: 16411 RVA: 0x0006B62F File Offset: 0x0006982F
		private void AutoRollNonUsableReagentsChanged(int arg0)
		{
			Options.GameOptions.AutoRollNonUsableReagents.Value = arg0;
		}

		// Token: 0x0600401C RID: 16412 RVA: 0x0006B63C File Offset: 0x0006983C
		private void AutoRollBasicItemsChanged(int arg0)
		{
			Options.GameOptions.AutoRollBasicItems.Value = arg0;
		}

		// Token: 0x0600401D RID: 16413 RVA: 0x0006B649 File Offset: 0x00069849
		private void TellAudioDropdownChanged(int arg0)
		{
			Options.AudioOptions.AudioOnTellOption.Value = arg0;
			TellAudio.PlayTellAudioClip();
		}

		// Token: 0x0600401E RID: 16414 RVA: 0x0018B8C4 File Offset: 0x00189AC4
		private void RefreshOverheadToggleInteractivity()
		{
			bool value = Options.GameOptions.ShowOverheadNameplates.Value;
			this.m_showOverheadNameSelf.interactable = value;
			this.m_showOverheadNameGroup.interactable = value;
			this.m_showOverheadNameGuild.interactable = value;
			this.m_showOverheadNameOtherPlayers.interactable = value;
			this.m_showOverheadNameNpcs.interactable = value;
			this.m_showOverheadStatSelf.interactable = value;
			this.m_showOverheadStatGroup.interactable = value;
			this.m_showOverheadStatGuild.interactable = value;
			this.m_showOverheadStatOtherPlayers.interactable = value;
			this.m_showOverheadStatNpcs.interactable = value;
		}

		// Token: 0x0600401F RID: 16415 RVA: 0x0018B954 File Offset: 0x00189B54
		private string GetDisplayForFrameRateLimiterOption(InGameUIOptions.FrameRateLimiterOptions opts)
		{
			switch (opts)
			{
			case InGameUIOptions.FrameRateLimiterOptions.None:
				return "None";
			case InGameUIOptions.FrameRateLimiterOptions.VSync:
				return "V-Sync";
			case InGameUIOptions.FrameRateLimiterOptions.k30:
				return "30 FPS";
			case InGameUIOptions.FrameRateLimiterOptions.k60:
				return "60 FPS";
			case InGameUIOptions.FrameRateLimiterOptions.k40:
				return "40 FPS";
			case InGameUIOptions.FrameRateLimiterOptions.k50:
				return "50 FPS";
			default:
				return "Unknown";
			}
		}

		// Token: 0x06004020 RID: 16416 RVA: 0x0018B9AC File Offset: 0x00189BAC
		private void SetupFrameRateLimiterDropdown()
		{
			this.m_frameRateLimiterDropdown.ClearOptions();
			InGameUIOptions.FrameRateLimiterOptions[] frameRateLimiterArray = this.m_frameRateLimiterArray;
			this.m_frameRateLimiterValues = new Dictionary<int, InGameUIOptions.FrameRateLimiterOptions>(frameRateLimiterArray.Length);
			int num = 1;
			List<string> list = new List<string>(frameRateLimiterArray.Length);
			for (int i = 0; i < frameRateLimiterArray.Length; i++)
			{
				this.m_frameRateLimiterValues.Add(i, frameRateLimiterArray[i]);
				list.Add(this.GetDisplayForFrameRateLimiterOption(frameRateLimiterArray[i]));
				if (frameRateLimiterArray[i] == (InGameUIOptions.FrameRateLimiterOptions)Options.VideoOptions.FrameRateLimiter.Value)
				{
					num = i;
				}
			}
			this.m_frameRateLimiterDropdown.AddOptions(list);
			this.m_frameRateLimiterDropdown.value = num;
			this.FrameRateLimiterDropdownChanged(num);
			this.m_frameRateLimiterDropdown.onValueChanged.AddListener(new UnityAction<int>(this.FrameRateLimiterDropdownChanged));
		}

		// Token: 0x06004021 RID: 16417 RVA: 0x0018BA5C File Offset: 0x00189C5C
		private void FrameRateLimiterDropdownChanged(int arg0)
		{
			InGameUIOptions.FrameRateLimiterOptions frameRateLimiterOptions2;
			InGameUIOptions.FrameRateLimiterOptions frameRateLimiterOptions = this.m_frameRateLimiterValues.TryGetValue(arg0, out frameRateLimiterOptions2) ? frameRateLimiterOptions2 : InGameUIOptions.FrameRateLimiterOptions.VSync;
			int value = (int)frameRateLimiterOptions;
			Options.VideoOptions.FrameRateLimiter.Value = value;
			int targetFrameRate = -1;
			bool flag = false;
			switch (frameRateLimiterOptions)
			{
			case InGameUIOptions.FrameRateLimiterOptions.VSync:
				flag = true;
				break;
			case InGameUIOptions.FrameRateLimiterOptions.k30:
				targetFrameRate = 30;
				break;
			case InGameUIOptions.FrameRateLimiterOptions.k60:
				targetFrameRate = 60;
				break;
			case InGameUIOptions.FrameRateLimiterOptions.k40:
				targetFrameRate = 40;
				break;
			case InGameUIOptions.FrameRateLimiterOptions.k50:
				targetFrameRate = 50;
				break;
			}
			QualitySettings.vSyncCount = (flag ? 1 : 0);
			Application.targetFrameRate = targetFrameRate;
			Debug.Log("Adjusting Frame Rate Limit.  VSyncCount=" + QualitySettings.vSyncCount.ToString() + ", TargetFrameRate=" + targetFrameRate.ToString());
		}

		// Token: 0x04003D87 RID: 15751
		private List<InGameUIOptions.BoolToggleHelper> m_toggleHelpers;

		// Token: 0x04003D88 RID: 15752
		private const string kMouseGroup = "Mouse";

		// Token: 0x04003D89 RID: 15753
		[SerializeField]
		private Toggle m_mouseInvertToggle;

		// Token: 0x04003D8A RID: 15754
		[SerializeField]
		private FloatSlider m_mouseSensitivityX;

		// Token: 0x04003D8B RID: 15755
		[SerializeField]
		private FloatSlider m_mouseSensitivityY;

		// Token: 0x04003D8C RID: 15756
		private const string kNameplateGroup = "Nameplates";

		// Token: 0x04003D8D RID: 15757
		[SerializeField]
		private Toggle m_showOverheadNameSelf;

		// Token: 0x04003D8E RID: 15758
		[SerializeField]
		private Toggle m_showOverheadNameGroup;

		// Token: 0x04003D8F RID: 15759
		[SerializeField]
		private Toggle m_showOverheadNameGuild;

		// Token: 0x04003D90 RID: 15760
		[SerializeField]
		private Toggle m_showOverheadNameOtherPlayers;

		// Token: 0x04003D91 RID: 15761
		[SerializeField]
		private Toggle m_showOverheadNameNpcs;

		// Token: 0x04003D92 RID: 15762
		[SerializeField]
		private Toggle m_showOverheadNameResourceNode;

		// Token: 0x04003D93 RID: 15763
		[SerializeField]
		private Toggle m_showOverheadStatSelf;

		// Token: 0x04003D94 RID: 15764
		[SerializeField]
		private Toggle m_showOverheadStatGroup;

		// Token: 0x04003D95 RID: 15765
		[SerializeField]
		private Toggle m_showOverheadStatGuild;

		// Token: 0x04003D96 RID: 15766
		[SerializeField]
		private Toggle m_showOverheadStatOtherPlayers;

		// Token: 0x04003D97 RID: 15767
		[SerializeField]
		private Toggle m_showOverheadStatNpcs;

		// Token: 0x04003D98 RID: 15768
		private const string kMiscGroup = "Misc";

		// Token: 0x04003D99 RID: 15769
		[SerializeField]
		private Toggle m_escapeTogglesMenu;

		// Token: 0x04003D9A RID: 15770
		[SerializeField]
		private Toggle m_showOverheadNameplatesToggle;

		// Token: 0x04003D9B RID: 15771
		[SerializeField]
		private Toggle m_showOverheadStatBarsToggle;

		// Token: 0x04003D9C RID: 15772
		[SerializeField]
		private Toggle m_showOverheadSelfToggle;

		// Token: 0x04003D9D RID: 15773
		[SerializeField]
		private Toggle m_showOverheadPlayersToggle;

		// Token: 0x04003D9E RID: 15774
		[SerializeField]
		private Toggle m_showOverheadPlayersGroupOnlyToggle;

		// Token: 0x04003D9F RID: 15775
		[SerializeField]
		private Toggle m_showOverheadNpcsToggle;

		// Token: 0x04003DA0 RID: 15776
		[SerializeField]
		private Toggle m_showOverheadCombatTextToggle;

		// Token: 0x04003DA1 RID: 15777
		[SerializeField]
		private Toggle m_showOverheadChatToggle;

		// Token: 0x04003DA2 RID: 15778
		[SerializeField]
		private Toggle m_allowSelfTargetToggle;

		// Token: 0x04003DA3 RID: 15779
		[SerializeField]
		private Toggle m_enableCameraDampingToggle;

		// Token: 0x04003DA4 RID: 15780
		[SerializeField]
		private Toggle m_disableCameraCollisionSmoothingToggle;

		// Token: 0x04003DA5 RID: 15781
		[SerializeField]
		private Toggle m_disableAutoAttackOnTargetDeathToggle;

		// Token: 0x04003DA6 RID: 15782
		[SerializeField]
		private Toggle m_leaveCombatAfterLootingToggle;

		// Token: 0x04003DA7 RID: 15783
		[SerializeField]
		private Toggle m_leaveCombatAfterCombatFlagDropToggle;

		// Token: 0x04003DA8 RID: 15784
		[SerializeField]
		private Toggle m_disableTooltipForActionBarToggle;

		// Token: 0x04003DA9 RID: 15785
		[SerializeField]
		private Toggle m_openBagAndGatheringTogetherToggle;

		// Token: 0x04003DAA RID: 15786
		[SerializeField]
		private Toggle m_crouchIsToggleToggle;

		// Token: 0x04003DAB RID: 15787
		[SerializeField]
		private Toggle m_hideTutorialPopups;

		// Token: 0x04003DAC RID: 15788
		[SerializeField]
		private Toggle m_disablePlayerWorldSpaceContextMenus;

		// Token: 0x04003DAD RID: 15789
		[SerializeField]
		private Toggle m_disablePlayerWorldSpaceContextMenusWeaponsDrawn;

		// Token: 0x04003DAE RID: 15790
		[SerializeField]
		private Toggle m_alternateLearningAnimationsToggle;

		// Token: 0x04003DAF RID: 15791
		[SerializeField]
		private Toggle m_enableZoomToFirstPersonToggle;

		// Token: 0x04003DB0 RID: 15792
		[SerializeField]
		private Toggle m_showGelIndicatorsToggle;

		// Token: 0x04003DB1 RID: 15793
		[SerializeField]
		private Toggle m_enableClothSim;

		// Token: 0x04003DB2 RID: 15794
		[SerializeField]
		private Toggle m_deselectSingleTargetOnTabToggle;

		// Token: 0x04003DB3 RID: 15795
		[SerializeField]
		private Toggle m_autoPassKnownRecipesToggle;

		// Token: 0x04003DB4 RID: 15796
		[SerializeField]
		private Toggle m_autoPassNonUsableReagentToggle;

		// Token: 0x04003DB5 RID: 15797
		[FormerlySerializedAs("m_stickyTargetingToggle")]
		[SerializeField]
		private SolToggle m_useTargetAtExecutionCompleteToggle;

		// Token: 0x04003DB6 RID: 15798
		[SerializeField]
		private Toggle m_keepAlchemyActiveOnCancelToggle;

		// Token: 0x04003DB7 RID: 15799
		[SerializeField]
		private Toggle m_hailOffensiveTargetFirstToggle;

		// Token: 0x04003DB8 RID: 15800
		[SerializeField]
		private TMP_Dropdown m_autoRollKnownRecipesDropdown;

		// Token: 0x04003DB9 RID: 15801
		[SerializeField]
		private TMP_Dropdown m_autoRollNonUsableRecipesDropdown;

		// Token: 0x04003DBA RID: 15802
		[SerializeField]
		private TMP_Dropdown m_autoRollNonUsableReagentsDropdown;

		// Token: 0x04003DBB RID: 15803
		[SerializeField]
		private TMP_Dropdown m_autoRollBasicItemsDropdown;

		// Token: 0x04003DBC RID: 15804
		[SerializeField]
		private Canvas m_gameUICanvas;

		// Token: 0x04003DBD RID: 15805
		[SerializeField]
		private IntSlider m_dialogSpeedSlider;

		// Token: 0x04003DBE RID: 15806
		[SerializeField]
		private UIScaleSlider m_gameUiScaleSlider;

		// Token: 0x04003DBF RID: 15807
		[SerializeField]
		private JobWorkerCountSlider m_jobWorkerCountSlider;

		// Token: 0x04003DC0 RID: 15808
		[SerializeField]
		private FloatSlider m_tooltipDelaySlider;

		// Token: 0x04003DC1 RID: 15809
		[SerializeField]
		private IntSlider m_chatFontSizeSlider;

		// Token: 0x04003DC2 RID: 15810
		[SerializeField]
		private FloatSlider m_overheadNameplateScaleSlider;

		// Token: 0x04003DC3 RID: 15811
		[SerializeField]
		private IntSlider m_fovSlider;

		// Token: 0x04003DC4 RID: 15812
		[SerializeField]
		private ToggleAllWindowsSetting[] m_toggleAllWindowsSettings;

		// Token: 0x04003DC5 RID: 15813
		[FormerlySerializedAs("m_hideUiCompassToggle")]
		[SerializeField]
		private Toggle m_showUiCompassToggle;

		// Token: 0x04003DC6 RID: 15814
		[SerializeField]
		private Toggle m_holdKeybindToShowCompassToggle;

		// Token: 0x04003DC7 RID: 15815
		[SerializeField]
		private Toggle m_showZoneNameOnCompassToggle;

		// Token: 0x04003DC8 RID: 15816
		[SerializeField]
		private Toggle m_showMacroBarToggle;

		// Token: 0x04003DC9 RID: 15817
		[SerializeField]
		private Toggle m_showBidConfirmationToggle;

		// Token: 0x04003DCA RID: 15818
		private const string kVideoGroup = "Video";

		// Token: 0x04003DCB RID: 15819
		[SerializeField]
		private Toggle m_fullScreenToggle;

		// Token: 0x04003DCC RID: 15820
		[SerializeField]
		private Toggle m_alternateFullScreenToggleWindows;

		// Token: 0x04003DCD RID: 15821
		[SerializeField]
		private Toggle m_alternateFullScreenToggleMacOS;

		// Token: 0x04003DCE RID: 15822
		[SerializeField]
		private Toggle m_bloomToggle;

		// Token: 0x04003DCF RID: 15823
		[SerializeField]
		private Toggle m_grassShadowToggle;

		// Token: 0x04003DD0 RID: 15824
		[SerializeField]
		private Toggle m_highResSunShadowToggle;

		// Token: 0x04003DD1 RID: 15825
		[SerializeField]
		private Toggle m_contactShadowToggle;

		// Token: 0x04003DD2 RID: 15826
		[SerializeField]
		private Toggle m_reflectionToggle;

		// Token: 0x04003DD3 RID: 15827
		[SerializeField]
		private Toggle m_cameraShakeToggle;

		// Token: 0x04003DD4 RID: 15828
		[SerializeField]
		private Toggle m_amortizeCloudReflectionsToggle;

		// Token: 0x04003DD5 RID: 15829
		[SerializeField]
		private Toggle m_volumetricsToggle;

		// Token: 0x04003DD6 RID: 15830
		[SerializeField]
		private Toggle m_subsurfaceToggle;

		// Token: 0x04003DD7 RID: 15831
		[SerializeField]
		private Toggle m_transmissionToggle;

		// Token: 0x04003DD8 RID: 15832
		[SerializeField]
		private TMP_Dropdown m_resolutionDropdown;

		// Token: 0x04003DD9 RID: 15833
		[SerializeField]
		private TMP_Dropdown m_qualityDropdown;

		// Token: 0x04003DDA RID: 15834
		[SerializeField]
		private TMP_Dropdown m_antialiasingDropdown;

		// Token: 0x04003DDB RID: 15835
		[SerializeField]
		private TMP_Dropdown m_frameRateLimiterDropdown;

		// Token: 0x04003DDC RID: 15836
		[SerializeField]
		private TMP_Dropdown m_vegetationDensityDropdown;

		// Token: 0x04003DDD RID: 15837
		[SerializeField]
		private TMP_Dropdown m_cloudQualityDropdown;

		// Token: 0x04003DDE RID: 15838
		[SerializeField]
		private TMP_Dropdown m_volumetricsQualityDropddown;

		// Token: 0x04003DDF RID: 15839
		[SerializeField]
		private TMP_Dropdown m_umaResolutionDropdown;

		// Token: 0x04003DE0 RID: 15840
		[SerializeField]
		private ShadowCastingLightsSlider m_maxShadowCastingLights;

		// Token: 0x04003DE1 RID: 15841
		[SerializeField]
		private ShadowDistanceSlider m_shadowDistance;

		// Token: 0x04003DE2 RID: 15842
		[SerializeField]
		private FloatSlider m_viewDistance;

		// Token: 0x04003DE3 RID: 15843
		[SerializeField]
		private FloatSlider m_vegetationDistance;

		// Token: 0x04003DE4 RID: 15844
		[SerializeField]
		private IntSlider m_treeDistance;

		// Token: 0x04003DE5 RID: 15845
		[SerializeField]
		private TMP_Dropdown m_treeImposterDistance;

		// Token: 0x04003DE6 RID: 15846
		[FormerlySerializedAs("m_useVspBillboards")]
		[SerializeField]
		private SolToggle m_useImposterBillboards;

		// Token: 0x04003DE7 RID: 15847
		[SerializeField]
		private ExposureAdjustmentSlider m_exposureAdjustment;

		// Token: 0x04003DE8 RID: 15848
		[SerializeField]
		private ResolutionScaleSlider m_resolutionScale;

		// Token: 0x04003DE9 RID: 15849
		[SerializeField]
		private GameObject m_dlssPanel;

		// Token: 0x04003DEA RID: 15850
		[SerializeField]
		private Toggle m_dlssToggle;

		// Token: 0x04003DEB RID: 15851
		[SerializeField]
		private TMP_Dropdown m_dlssDropdown;

		// Token: 0x04003DEC RID: 15852
		private const string kAudioGroup = "Audio";

		// Token: 0x04003DED RID: 15853
		[SerializeField]
		private VolumeSlider[] m_volumeSliders;

		// Token: 0x04003DEE RID: 15854
		[SerializeField]
		private Toggle m_tellAudioToggle;

		// Token: 0x04003DEF RID: 15855
		[SerializeField]
		private TMP_Dropdown m_tellAudioDropdown;

		// Token: 0x04003DF0 RID: 15856
		[SerializeField]
		private Toggle m_friendAudioToggle;

		// Token: 0x04003DF1 RID: 15857
		[SerializeField]
		private Toggle m_guildAudioToggle;

		// Token: 0x04003DF2 RID: 15858
		[SerializeField]
		private Toggle m_alternateJumpAudioToggle;

		// Token: 0x04003DF3 RID: 15859
		[SerializeField]
		private Toggle m_audioListenerAtPlayerToggle;

		// Token: 0x04003DF4 RID: 15860
		private bool m_optionsShown;

		// Token: 0x04003DF5 RID: 15861
		private bool m_preventFullscreenToggleAction;

		// Token: 0x04003DF6 RID: 15862
		private bool m_preventResolutionChangedAction;

		// Token: 0x04003DF7 RID: 15863
		private int? m_setResolutionFrame;

		// Token: 0x04003DF8 RID: 15864
		private int? m_customResolutionIndex;

		// Token: 0x04003DF9 RID: 15865
		private Resolution m_currentResolution;

		// Token: 0x04003DFA RID: 15866
		private Dictionary<int, Resolution> m_resolutionDictionary;

		// Token: 0x04003DFB RID: 15867
		private List<string> m_resolutionStrings;

		// Token: 0x04003DFC RID: 15868
		[SerializeField]
		private KeybindsUI m_keybindsUI;

		// Token: 0x04003DFD RID: 15869
		private Dictionary<int, InGameUIOptions.VegetationDensityValue> m_vegetationDensityValues;

		// Token: 0x04003DFE RID: 15870
		private bool m_preventShowNameplateTrigger;

		// Token: 0x04003DFF RID: 15871
		private Dictionary<int, InGameUIOptions.FrameRateLimiterOptions> m_frameRateLimiterValues;

		// Token: 0x04003E00 RID: 15872
		private readonly InGameUIOptions.FrameRateLimiterOptions[] m_frameRateLimiterArray = new InGameUIOptions.FrameRateLimiterOptions[]
		{
			InGameUIOptions.FrameRateLimiterOptions.None,
			InGameUIOptions.FrameRateLimiterOptions.VSync,
			InGameUIOptions.FrameRateLimiterOptions.k30,
			InGameUIOptions.FrameRateLimiterOptions.k40,
			InGameUIOptions.FrameRateLimiterOptions.k50,
			InGameUIOptions.FrameRateLimiterOptions.k60
		};

		// Token: 0x02000896 RID: 2198
		private class BoolToggleHelper
		{
			// Token: 0x06004023 RID: 16419 RVA: 0x0018BB04 File Offset: 0x00189D04
			public BoolToggleHelper(Toggle toggle, Options.Option_Boolean option)
			{
				if (toggle == null)
				{
					throw new ArgumentNullException("toggle");
				}
				if (option == null)
				{
					throw new ArgumentNullException("option");
				}
				this.m_toggle = toggle;
				this.m_option = option;
				this.m_toggle.isOn = this.m_option.Value;
				this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleOnChanged));
				this.m_option.Changed += this.OptionOnChanged;
			}

			// Token: 0x06004024 RID: 16420 RVA: 0x0006B67A File Offset: 0x0006987A
			public void OnDestroy()
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleOnChanged));
				this.m_option.Changed -= this.OptionOnChanged;
			}

			// Token: 0x06004025 RID: 16421 RVA: 0x0006B6AF File Offset: 0x000698AF
			private void ToggleOnChanged(bool arg0)
			{
				if (this.m_blockToggleChangedEvent)
				{
					return;
				}
				this.m_blockOptionChangedEvent = true;
				this.m_option.Value = arg0;
				this.m_blockOptionChangedEvent = false;
			}

			// Token: 0x06004026 RID: 16422 RVA: 0x0006B6D4 File Offset: 0x000698D4
			private void OptionOnChanged()
			{
				if (this.m_blockOptionChangedEvent)
				{
					return;
				}
				this.m_blockToggleChangedEvent = true;
				this.m_toggle.isOn = this.m_option.Value;
				this.m_blockToggleChangedEvent = false;
			}

			// Token: 0x04003E01 RID: 15873
			private readonly Toggle m_toggle;

			// Token: 0x04003E02 RID: 15874
			private readonly Options.Option_Boolean m_option;

			// Token: 0x04003E03 RID: 15875
			private bool m_blockToggleChangedEvent;

			// Token: 0x04003E04 RID: 15876
			private bool m_blockOptionChangedEvent;
		}

		// Token: 0x02000897 RID: 2199
		public enum VegetationDensityValue
		{
			// Token: 0x04003E06 RID: 15878
			VeryLow = 25,
			// Token: 0x04003E07 RID: 15879
			Low = 50,
			// Token: 0x04003E08 RID: 15880
			Medium = 75,
			// Token: 0x04003E09 RID: 15881
			High = 100
		}

		// Token: 0x02000898 RID: 2200
		public enum FrameRateLimiterOptions
		{
			// Token: 0x04003E0B RID: 15883
			None,
			// Token: 0x04003E0C RID: 15884
			VSync,
			// Token: 0x04003E0D RID: 15885
			k30,
			// Token: 0x04003E0E RID: 15886
			k60,
			// Token: 0x04003E0F RID: 15887
			k40,
			// Token: 0x04003E10 RID: 15888
			k50
		}
	}
}
