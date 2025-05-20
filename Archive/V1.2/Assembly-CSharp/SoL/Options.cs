using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL
{
	// Token: 0x02000223 RID: 547
	public static class Options
	{
		// Token: 0x02000224 RID: 548
		public abstract class Option<T>
		{
			// Token: 0x1400001C RID: 28
			// (add) Token: 0x0600125E RID: 4702 RVA: 0x000E6D00 File Offset: 0x000E4F00
			// (remove) Token: 0x0600125F RID: 4703 RVA: 0x000E6D38 File Offset: 0x000E4F38
			public event Action Changed;

			// Token: 0x170004D1 RID: 1233
			// (get) Token: 0x06001260 RID: 4704 RVA: 0x0004F147 File Offset: 0x0004D347
			public T DefaultValue
			{
				get
				{
					return this.m_defaultValue;
				}
			}

			// Token: 0x170004D2 RID: 1234
			// (get) Token: 0x06001261 RID: 4705 RVA: 0x0004F14F File Offset: 0x0004D34F
			// (set) Token: 0x06001262 RID: 4706 RVA: 0x000E6D70 File Offset: 0x000E4F70
			public virtual T Value
			{
				get
				{
					if (!this.m_hasCachedValue)
					{
						this.m_cachedValue = this.Load();
						this.m_hasCachedValue = true;
					}
					return this.m_cachedValue;
				}
				set
				{
					if (value != null && value.Equals(this.m_cachedValue))
					{
						return;
					}
					this.m_cachedValue = value;
					this.m_hasCachedValue = true;
					this.Save();
					Action changed = this.Changed;
					if (changed == null)
					{
						return;
					}
					changed();
				}
			}

			// Token: 0x06001263 RID: 4707 RVA: 0x0004F172 File Offset: 0x0004D372
			public Option(string key, T defaultValue)
			{
				this.m_key = key;
				this.m_defaultValue = defaultValue;
			}

			// Token: 0x06001264 RID: 4708
			protected abstract T Load();

			// Token: 0x06001265 RID: 4709
			protected abstract void Save();

			// Token: 0x06001266 RID: 4710 RVA: 0x0004F188 File Offset: 0x0004D388
			public static implicit operator T(Options.Option<T> option)
			{
				return option.Value;
			}

			// Token: 0x04000FFA RID: 4090
			protected readonly string m_key;

			// Token: 0x04000FFB RID: 4091
			protected readonly T m_defaultValue;

			// Token: 0x04000FFC RID: 4092
			private T m_cachedValue;

			// Token: 0x04000FFD RID: 4093
			private bool m_hasCachedValue;
		}

		// Token: 0x02000225 RID: 549
		public class Option_Boolean : Options.Option<bool>
		{
			// Token: 0x06001267 RID: 4711 RVA: 0x0004F190 File Offset: 0x0004D390
			public Option_Boolean(string key, bool defaultValue) : base(key, defaultValue)
			{
			}

			// Token: 0x06001268 RID: 4712 RVA: 0x0004F19A File Offset: 0x0004D39A
			protected override bool Load()
			{
				return PlayerPrefs.GetInt(this.m_key, this.m_defaultValue ? 1 : 0) == 1;
			}

			// Token: 0x06001269 RID: 4713 RVA: 0x0004F1B6 File Offset: 0x0004D3B6
			protected override void Save()
			{
				if (this.Value == this.m_defaultValue)
				{
					PlayerPrefs.DeleteKey(this.m_key);
					return;
				}
				PlayerPrefs.SetInt(this.m_key, this.Value ? 1 : 0);
			}

			// Token: 0x04000FFE RID: 4094
			private const int kFalse = 0;

			// Token: 0x04000FFF RID: 4095
			private const int kTrue = 1;
		}

		// Token: 0x02000226 RID: 550
		public class Option_Int : Options.Option<int>
		{
			// Token: 0x0600126A RID: 4714 RVA: 0x0004F1E9 File Offset: 0x0004D3E9
			public Option_Int(string key, int defaultValue) : base(key, defaultValue)
			{
			}

			// Token: 0x0600126B RID: 4715 RVA: 0x0004F1F3 File Offset: 0x0004D3F3
			protected override int Load()
			{
				return PlayerPrefs.GetInt(this.m_key, this.m_defaultValue);
			}

			// Token: 0x0600126C RID: 4716 RVA: 0x0004F206 File Offset: 0x0004D406
			protected override void Save()
			{
				if (this.Value == this.m_defaultValue)
				{
					PlayerPrefs.DeleteKey(this.m_key);
					return;
				}
				PlayerPrefs.SetInt(this.m_key, this.Value);
			}
		}

		// Token: 0x02000227 RID: 551
		public class Option_Float : Options.Option<float>
		{
			// Token: 0x0600126D RID: 4717 RVA: 0x0004F233 File Offset: 0x0004D433
			public Option_Float(string key, float defaultValue) : base(key, defaultValue)
			{
			}

			// Token: 0x0600126E RID: 4718 RVA: 0x0004F23D File Offset: 0x0004D43D
			protected override float Load()
			{
				return PlayerPrefs.GetFloat(this.m_key, this.m_defaultValue);
			}

			// Token: 0x0600126F RID: 4719 RVA: 0x0004F250 File Offset: 0x0004D450
			protected override void Save()
			{
				if (this.Value == this.m_defaultValue)
				{
					PlayerPrefs.DeleteKey(this.m_key);
					return;
				}
				PlayerPrefs.SetFloat(this.m_key, this.Value);
			}
		}

		// Token: 0x02000228 RID: 552
		public class Option_String : Options.Option<string>
		{
			// Token: 0x06001270 RID: 4720 RVA: 0x0004F27D File Offset: 0x0004D47D
			public Option_String(string key, string defaultValue) : base(key, defaultValue)
			{
			}

			// Token: 0x06001271 RID: 4721 RVA: 0x0004F287 File Offset: 0x0004D487
			protected override string Load()
			{
				return PlayerPrefs.GetString(this.m_key, this.m_defaultValue);
			}

			// Token: 0x06001272 RID: 4722 RVA: 0x0004F29A File Offset: 0x0004D49A
			protected override void Save()
			{
				if (this.Value == this.m_defaultValue)
				{
					PlayerPrefs.DeleteKey(this.m_key);
					return;
				}
				PlayerPrefs.SetString(this.m_key, this.Value);
			}
		}

		// Token: 0x02000229 RID: 553
		public static class GameOptions
		{
			// Token: 0x06001273 RID: 4723 RVA: 0x000E6DC4 File Offset: 0x000E4FC4
			static GameOptions()
			{
				for (int i = 0; i < Options.GameOptions.kOldKeys.Length; i++)
				{
					string key = Options.GameOptions.kOldKeys[i];
					if (PlayerPrefs.HasKey(key))
					{
						PlayerPrefs.DeleteKey(key);
					}
				}
			}

			// Token: 0x170004D3 RID: 1235
			// (get) Token: 0x06001274 RID: 4724 RVA: 0x0004F2CC File Offset: 0x0004D4CC
			internal static float GameUIScalePercentage
			{
				get
				{
					return (float)Options.GameOptions.GameUIScale.Value * 0.01f;
				}
			}

			// Token: 0x04001000 RID: 4096
			private static readonly string[] kOldKeys = new string[]
			{
				"ShowNameplates",
				"ShowNameplateStatBars"
			};

			// Token: 0x04001001 RID: 4097
			internal static Options.Option_Boolean SaveUsername = new Options.Option_Boolean("SaveUsername", false);

			// Token: 0x04001002 RID: 4098
			internal static Options.Option_String Username = new Options.Option_String("Username", "");

			// Token: 0x04001003 RID: 4099
			internal static Options.Option_Boolean InvertMouse = new Options.Option_Boolean("InvertMouse", false);

			// Token: 0x04001004 RID: 4100
			internal static Options.Option_Float XMouseSensitivity = new Options.Option_Float("XMouseSensitivity", 1f);

			// Token: 0x04001005 RID: 4101
			internal static Options.Option_Float YMouseSensitivity = new Options.Option_Float("YMouseSensitivity", 1f);

			// Token: 0x04001006 RID: 4102
			internal static Options.Option_Boolean EscapeTogglesMenu = new Options.Option_Boolean("EscapeTogglesMenu", true);

			// Token: 0x04001007 RID: 4103
			internal static Options.Option_Boolean AllowSelfTarget = new Options.Option_Boolean("AllowSelfTarget", false);

			// Token: 0x04001008 RID: 4104
			internal static Options.Option_Int GameUIScale = new Options.Option_Int("GameUIScale", 100);

			// Token: 0x04001009 RID: 4105
			internal static Options.Option_Float TooltipDelay = new Options.Option_Float("TooltipDelay", 0.1f);

			// Token: 0x0400100A RID: 4106
			internal static Options.Option_Boolean DisableTooltipForActionBarAbilities = new Options.Option_Boolean("DisableTooltipForActionBarAbilities", false);

			// Token: 0x0400100B RID: 4107
			internal static Options.Option_Boolean HideTutorialPopups = new Options.Option_Boolean("HideTutorialPopups", false);

			// Token: 0x0400100C RID: 4108
			internal static Options.Option_Boolean ShowOverheadNameplates = new Options.Option_Boolean("ShowOverheadNameplates", true);

			// Token: 0x0400100D RID: 4109
			internal static Options.Option_Boolean ShowOverheadStatBars = new Options.Option_Boolean("ShowOverheadStatBars", true);

			// Token: 0x0400100E RID: 4110
			internal static Options.Option_Boolean ShowOverheadCombatText = new Options.Option_Boolean("ShowOverheadCombatText", true);

			// Token: 0x0400100F RID: 4111
			internal static Options.Option_Boolean ShowOverheadSelfNameplate = new Options.Option_Boolean("ShowOverheadSelfNameplate", true);

			// Token: 0x04001010 RID: 4112
			internal static Options.Option_Boolean ShowOverheadPlayerNameplates = new Options.Option_Boolean("ShowOverheadPlayerNameplates", true);

			// Token: 0x04001011 RID: 4113
			internal static Options.Option_Boolean ShowOverheadPlayerNameplatesGroupOnly = new Options.Option_Boolean("ShowOverheadPlayerNameplatesGroupOnly", true);

			// Token: 0x04001012 RID: 4114
			internal static Options.Option_Boolean ShowOverheadNpcNameplates = new Options.Option_Boolean("ShowOverheadNpcNameplates", true);

			// Token: 0x04001013 RID: 4115
			internal static Options.Option_Boolean ShowOverheadChat = new Options.Option_Boolean("ShowOverheadChat", true);

			// Token: 0x04001014 RID: 4116
			internal static Options.Option_Float OverheadNameplateScale = new Options.Option_Float("OverheadNameplateScale", 3f);

			// Token: 0x04001015 RID: 4117
			internal static Options.Option_Boolean ShowOverheadNameplate_Self = new Options.Option_Boolean("ShowOverheadNameplate_Self", false);

			// Token: 0x04001016 RID: 4118
			internal static Options.Option_Boolean ShowOverheadStatBar_Self = new Options.Option_Boolean("ShowOverheadStatBar_Self", false);

			// Token: 0x04001017 RID: 4119
			internal static Options.Option_Boolean ShowOverheadNameplate_Group = new Options.Option_Boolean("ShowOverheadNameplate_Group", true);

			// Token: 0x04001018 RID: 4120
			internal static Options.Option_Boolean ShowOverheadStatBar_Group = new Options.Option_Boolean("ShowOverheadStatBar_Group", true);

			// Token: 0x04001019 RID: 4121
			internal static Options.Option_Boolean ShowOverheadNameplate_Guild = new Options.Option_Boolean("ShowOverheadNameplate_Guild", true);

			// Token: 0x0400101A RID: 4122
			internal static Options.Option_Boolean ShowOverheadStatBar_Guild = new Options.Option_Boolean("ShowOverheadStatBar_Guild", true);

			// Token: 0x0400101B RID: 4123
			internal static Options.Option_Boolean ShowOverheadNameplate_OtherPlayers = new Options.Option_Boolean("ShowOverheadNameplate_OtherPlayers", true);

			// Token: 0x0400101C RID: 4124
			internal static Options.Option_Boolean ShowOverheadStatBar_OtherPlayers = new Options.Option_Boolean("ShowOverheadStatBar_OtherPlayers", true);

			// Token: 0x0400101D RID: 4125
			internal static Options.Option_Boolean ShowOverheadNameplate_Npcs = new Options.Option_Boolean("ShowOverheadNameplate_Npcs", true);

			// Token: 0x0400101E RID: 4126
			internal static Options.Option_Boolean ShowOverheadStatBar_Npcs = new Options.Option_Boolean("ShowOverheadStatBar_Npcs", true);

			// Token: 0x0400101F RID: 4127
			internal static Options.Option_Boolean ShowOverheadNameplate_ResourceNode = new Options.Option_Boolean("ShowOverheadNameplate_ResourceNode", true);

			// Token: 0x04001020 RID: 4128
			internal static Options.Option_Int DialogSpeed = new Options.Option_Int("DialogSpeed", 120);

			// Token: 0x04001021 RID: 4129
			internal static Options.Option_Int SelectedCamera = new Options.Option_Int("SelectedCamera", 0);

			// Token: 0x04001022 RID: 4130
			internal static Options.Option_Boolean EnableZoomToFirstPerson = new Options.Option_Boolean("EnableZoomToFirstPerson", false);

			// Token: 0x04001023 RID: 4131
			internal static Options.Option_Boolean OverTheShoulderCameraActive = new Options.Option_Boolean("OverTheShoulderCameraActive", false);

			// Token: 0x04001024 RID: 4132
			internal static Options.Option_Boolean EnableCameraDamping = new Options.Option_Boolean("EnableCameraDampingV2", false);

			// Token: 0x04001025 RID: 4133
			internal static Options.Option_Boolean DisableCameraCollisionSmoothing = new Options.Option_Boolean("DisableCameraCollisionSmoothing", false);

			// Token: 0x04001026 RID: 4134
			internal static Options.Option_Boolean DisableAutoAttackOnTargetDeath = new Options.Option_Boolean("DisableAutoAttackOnTargetDeath", true);

			// Token: 0x04001027 RID: 4135
			internal static Options.Option_Boolean LeaveCombatAfterLooting = new Options.Option_Boolean("LeaveCombatAfterLooting", false);

			// Token: 0x04001028 RID: 4136
			internal static Options.Option_Boolean LeaveCombatAfterCombatFlagDrop = new Options.Option_Boolean("LeaveCombatAfterCombatFlagDrop", false);

			// Token: 0x04001029 RID: 4137
			internal static Options.Option_Boolean OpenBagAndGatheringTogether = new Options.Option_Boolean("OpenBagAndGatheringTogether", false);

			// Token: 0x0400102A RID: 4138
			internal static Options.Option_Boolean CrouchIsToggle = new Options.Option_Boolean("CrouchIsToggle", false);

			// Token: 0x0400102B RID: 4139
			internal static Options.Option_Boolean UseTargetAtExecutionComplete = new Options.Option_Boolean("UseTargetAtExecutionComplete", false);

			// Token: 0x0400102C RID: 4140
			internal static Options.Option_Int ChatFontSize = new Options.Option_Int("ChatFontSize", 12);

			// Token: 0x0400102D RID: 4141
			internal static Options.Option_Int DialogueFontSize = new Options.Option_Int("DialogueFontSize", 18);

			// Token: 0x0400102E RID: 4142
			internal static Options.Option_Boolean DisablePlayerWorldSpaceContextMenus = new Options.Option_Boolean("DisablePlayerWorldSpaceContextMenus", false);

			// Token: 0x0400102F RID: 4143
			internal static Options.Option_Boolean DisablePlayerWorldSpaceContextMenusWeaponsDrawn = new Options.Option_Boolean("DisablePlayerWorldSpaceContextMenusWeaponsDrawn", false);

			// Token: 0x04001030 RID: 4144
			internal static Options.Option_Boolean AlternateLearningAnimation = new Options.Option_Boolean("AlternateLearningAnimation", false);

			// Token: 0x04001031 RID: 4145
			internal static Options.Option_Boolean ShowGelIndicators = new Options.Option_Boolean("ShowGelIndicators", false);

			// Token: 0x04001032 RID: 4146
			internal static Options.Option_Boolean EnableClothSim = new Options.Option_Boolean("EnableClothSim", false);

			// Token: 0x04001033 RID: 4147
			internal static Options.Option_Boolean HideUncraftable = new Options.Option_Boolean("HideUncraftable", false);

			// Token: 0x04001034 RID: 4148
			internal static Options.Option_Boolean DeselectSingleTargetOnTab = new Options.Option_Boolean("DeselectSingleTargetOnTab", false);

			// Token: 0x04001035 RID: 4149
			internal static Options.Option_Boolean AutoPassKnownRecipes = new Options.Option_Boolean("AutoPassKnownRecipes", false);

			// Token: 0x04001036 RID: 4150
			internal static Options.Option_Boolean AutoPassNonUsableReagents = new Options.Option_Boolean("AutoPassNonUsableReagents", false);

			// Token: 0x04001037 RID: 4151
			internal static Options.Option_Boolean KeepAlchemyActiveOnCancel = new Options.Option_Boolean("KeepAlchemyDisabledOnCancel", false);

			// Token: 0x04001038 RID: 4152
			internal static Options.Option_Boolean HailOffensiveTargetFirst = new Options.Option_Boolean("HailOffensiveTargetFirst", false);

			// Token: 0x04001039 RID: 4153
			internal static Options.Option_Int AutoRollKnownRecipes = new Options.Option_Int("AutoRollKnownRecipes", 0);

			// Token: 0x0400103A RID: 4154
			internal static Options.Option_Int AutoRollNonUsableRecipes = new Options.Option_Int("AutoRollNonUsableRecipes", 0);

			// Token: 0x0400103B RID: 4155
			internal static Options.Option_Int AutoRollNonUsableReagents = new Options.Option_Int("AutoRollNonUsableReagents", 0);

			// Token: 0x0400103C RID: 4156
			internal static Options.Option_Int AutoRollBasicItems = new Options.Option_Int("AutoRollBasicItems", 0);

			// Token: 0x0400103D RID: 4157
			internal static Options.Option_Int ToggleAllWindows = new Options.Option_Int("ToggleAllWindows", 14);

			// Token: 0x0400103E RID: 4158
			internal static Options.Option_Int PreviousJobWorkerMaximumCount = new Options.Option_Int("PreviousJobWorkerMaximumCount", -1);

			// Token: 0x0400103F RID: 4159
			internal static Options.Option_Int SelectedJobWorkerMaximumCount = new Options.Option_Int("SelectedJobWorkerMaximumCount", -1);

			// Token: 0x04001040 RID: 4160
			internal static Options.Option_Boolean ShowMacroBar = new Options.Option_Boolean("ShowMacroBar", false);

			// Token: 0x04001041 RID: 4161
			internal static Options.Option_Boolean ShowUiCompass = new Options.Option_Boolean("ShowUiCompass", true);

			// Token: 0x04001042 RID: 4162
			internal static Options.Option_Boolean ShowZoneNameOnCompass = new Options.Option_Boolean("ShowZoneNameOnCompass", true);

			// Token: 0x04001043 RID: 4163
			internal static Options.Option_Boolean HoldKeybindToShowCompass = new Options.Option_Boolean("HoldKeybindToShowCompass", false);

			// Token: 0x04001044 RID: 4164
			internal static Options.Option_Boolean ShowBidConfirmation = new Options.Option_Boolean("ShowBidConfirmation", true);
		}

		// Token: 0x0200022A RID: 554
		public static class AudioOptions
		{
			// Token: 0x04001045 RID: 4165
			internal static Options.Option_Float MasterVolume = new Options.Option_Float("Volume_Master2", 1f);

			// Token: 0x04001046 RID: 4166
			internal static Options.Option_Float EffectsVolume = new Options.Option_Float("Volume_Effects2", 1f);

			// Token: 0x04001047 RID: 4167
			internal static Options.Option_Float EnvironmentVolume = new Options.Option_Float("Volume_Environment2", 1f);

			// Token: 0x04001048 RID: 4168
			internal static Options.Option_Float FootstepsVolume = new Options.Option_Float("Volume_Footsteps2", 0.8f);

			// Token: 0x04001049 RID: 4169
			internal static Options.Option_Float UIVolume = new Options.Option_Float("Volume_UI2", 1f);

			// Token: 0x0400104A RID: 4170
			internal static Options.Option_Float MusicVolume = new Options.Option_Float("Volume_Music2", 0.8f);

			// Token: 0x0400104B RID: 4171
			internal static Options.Option_Float AuraVolume = new Options.Option_Float("Volume_Aura2", 0.8f);

			// Token: 0x0400104C RID: 4172
			internal static Options.Option_Boolean AudioOnTell = new Options.Option_Boolean("AudioOnTell", true);

			// Token: 0x0400104D RID: 4173
			internal static Options.Option_Boolean AudioOnFriendStatusUpdate = new Options.Option_Boolean("AudioOnFriendStatusUpdate", true);

			// Token: 0x0400104E RID: 4174
			internal static Options.Option_Boolean AudioOnGuildStatusUpdate = new Options.Option_Boolean("AudioOnGuildStatusUpdate", true);

			// Token: 0x0400104F RID: 4175
			internal static Options.Option_Boolean AlternateJumpAudio = new Options.Option_Boolean("AlternateJumpAudio", false);

			// Token: 0x04001050 RID: 4176
			internal static Options.Option_Boolean AudioListenerAtPlayer = new Options.Option_Boolean("AudioListenerAtPlayer", false);

			// Token: 0x04001051 RID: 4177
			internal static Options.Option_Int AudioOnTellOption = new Options.Option_Int("AudioOnTellOption", 1);
		}

		// Token: 0x0200022B RID: 555
		public static class VideoOptions
		{
			// Token: 0x170004D4 RID: 1236
			// (get) Token: 0x06001276 RID: 4726 RVA: 0x000E736C File Offset: 0x000E556C
			// (set) Token: 0x06001277 RID: 4727 RVA: 0x0004F2DF File Offset: 0x0004D4DF
			internal static Resolution CustomResolution
			{
				get
				{
					string @string = PlayerPrefs.GetString("Resolution22", string.Empty);
					return Options.VideoOptions.CustomResolution = (string.IsNullOrEmpty(@string) ? Screen.currentResolution : JsonConvert.DeserializeObject<Resolution>(@string));
				}
				set
				{
					PlayerPrefs.SetString("Resolution22", JsonConvert.SerializeObject(value));
				}
			}

			// Token: 0x04001052 RID: 4178
			private const string kPlayerPrefsKeyCustomResolution = "Resolution22";

			// Token: 0x04001053 RID: 4179
			public const int kDefaultQualityLevel = 1;

			// Token: 0x04001054 RID: 4180
			public const int kDefaultVegetationDesnity = 75;

			// Token: 0x04001055 RID: 4181
			public const int kDefaultCloudQuality = 1;

			// Token: 0x04001056 RID: 4182
			public const int kDefaultFrameRateLimiter = 2;

			// Token: 0x04001057 RID: 4183
			internal static readonly Options.Option_Boolean FullScreen = new Options.Option_Boolean("FullScreen", true);

			// Token: 0x04001058 RID: 4184
			internal static readonly Options.Option_Boolean AlternateFullScreenMode = new Options.Option_Boolean("AlternateFullScreenMode", false);

			// Token: 0x04001059 RID: 4185
			internal static readonly Options.Option_Boolean VSync = new Options.Option_Boolean("VSync", true);

			// Token: 0x0400105A RID: 4186
			internal static readonly Options.Option_Boolean Bloom = new Options.Option_Boolean("Bloom", true);

			// Token: 0x0400105B RID: 4187
			internal static readonly Options.Option_Boolean GrassShadows = new Options.Option_Boolean("GrassShadows", false);

			// Token: 0x0400105C RID: 4188
			internal static readonly Options.Option_Boolean ContactShadows = new Options.Option_Boolean("ContactShadows", false);

			// Token: 0x0400105D RID: 4189
			internal static readonly Options.Option_Boolean Reflections = new Options.Option_Boolean("Reflections", true);

			// Token: 0x0400105E RID: 4190
			internal static readonly Options.Option_Boolean Volumetrics = new Options.Option_Boolean("Volumetrics", true);

			// Token: 0x0400105F RID: 4191
			internal static readonly Options.Option_Boolean Transmission = new Options.Option_Boolean("Transmission", true);

			// Token: 0x04001060 RID: 4192
			internal static readonly Options.Option_Boolean Subsurface = new Options.Option_Boolean("Subsurface", true);

			// Token: 0x04001061 RID: 4193
			internal static readonly Options.Option_Int QualitySetting = new Options.Option_Int("QualitySetting", 1);

			// Token: 0x04001062 RID: 4194
			internal static readonly Options.Option_Float ViewDistance = new Options.Option_Float("ViewDistance", 1f);

			// Token: 0x04001063 RID: 4195
			internal static readonly Options.Option_Float ShadowDistance = new Options.Option_Float("ShadowDistance", 0.5f);

			// Token: 0x04001064 RID: 4196
			internal static readonly Options.Option_Int AntiAliasingType = new Options.Option_Int("AntiAliasingOption", 3);

			// Token: 0x04001065 RID: 4197
			internal static readonly Options.Option_Int FrameRateLimiter = new Options.Option_Int("FrameRateLimiter", 2);

			// Token: 0x04001066 RID: 4198
			internal static readonly Options.Option_Int VegetationDensity = new Options.Option_Int("VegetationDensity", 75);

			// Token: 0x04001067 RID: 4199
			internal static readonly Options.Option_Float VegetationDistance = new Options.Option_Float("VegetationDistance", 1f);

			// Token: 0x04001068 RID: 4200
			internal static readonly Options.Option_Int TreeDistance = new Options.Option_Int("TreeDistance", 100);

			// Token: 0x04001069 RID: 4201
			internal static readonly Options.Option_Int TreeImposterDistance = new Options.Option_Int("TreeImposterDistance", 1);

			// Token: 0x0400106A RID: 4202
			internal static readonly Options.Option_Boolean UseImposterBillboards = new Options.Option_Boolean("UseImposterBillboards", true);

			// Token: 0x0400106B RID: 4203
			internal static readonly Options.Option_Int CloudQuality = new Options.Option_Int("CloudQuality", 1);

			// Token: 0x0400106C RID: 4204
			internal static readonly Options.Option_Float ExposureAdjustment = new Options.Option_Float("ExposureAdjustment", 0f);

			// Token: 0x0400106D RID: 4205
			internal static readonly Options.Option_Boolean AmortizeCloudReflections = new Options.Option_Boolean("AmortizeCloudReflections", true);

			// Token: 0x0400106E RID: 4206
			internal static readonly Options.Option_Float ResolutionScale = new Options.Option_Float("ResolutionScale", 1f);

			// Token: 0x0400106F RID: 4207
			internal static readonly Options.Option_Int UMAResolution = new Options.Option_Int("UMAResolution", 1);

			// Token: 0x04001070 RID: 4208
			internal static readonly Options.Option_Int VolumetricQuality = new Options.Option_Int("VolumetricQuality", 1);

			// Token: 0x04001071 RID: 4209
			internal static readonly Options.Option_Int CameraFieldOfView = new Options.Option_Int("CameraFieldOfView", 50);

			// Token: 0x04001072 RID: 4210
			internal static readonly Options.Option_Boolean CameraShake = new Options.Option_Boolean("CameraShake", true);

			// Token: 0x04001073 RID: 4211
			internal static readonly Options.Option_Boolean NvidiaDLSSEnable = new Options.Option_Boolean("NvidiaDLSSEnable", false);

			// Token: 0x04001074 RID: 4212
			internal static readonly Options.Option_Int NvidiaDLSSQuality = new Options.Option_Int("NvidiaDLSSQuality", 0);

			// Token: 0x04001075 RID: 4213
			internal static readonly Options.Option_Int MaxShadowCastingLights = new Options.Option_Int("MaxShadowCastingLights", 3);

			// Token: 0x04001076 RID: 4214
			internal static readonly Options.Option_Boolean HighResolutionSunShadows = new Options.Option_Boolean("HighResolutionSunShadows", false);
		}
	}
}
