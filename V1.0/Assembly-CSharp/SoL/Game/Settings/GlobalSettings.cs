using System;
using System.Collections.Generic;
using SoL.Game.NPCs.Senses;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Settings
{
	// Token: 0x02000728 RID: 1832
	[CreateAssetMenu(menuName = "SoL/Global Settings")]
	public class GlobalSettings : ScriptableObject
	{
		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x06003701 RID: 14081 RVA: 0x0016AC00 File Offset: 0x00168E00
		public static GlobalSettings Values
		{
			get
			{
				if (!Application.isPlaying || GlobalSettings.m_globalSettings == null)
				{
					GlobalSettings.m_globalSettings = Resources.Load<GlobalSettings>("GlobalSettings");
					if (GlobalSettings.m_globalSettings == null)
					{
						Debug.LogError("Could not load GlobalSettings!");
					}
				}
				return GlobalSettings.m_globalSettings;
			}
		}

		// Token: 0x06003702 RID: 14082 RVA: 0x0016AC4C File Offset: 0x00168E4C
		public bool TryGetBatteryForExchange(RuneSourceType requestedType, out RunicBattery battery)
		{
			if (this.m_exchangeBatteriesDict == null)
			{
				this.m_exchangeBatteriesDict = new Dictionary<RuneSourceType, RunicBattery>(default(RuneSourceTypeComparer));
				for (int i = 0; i < this.m_exchangeBatteries.Length; i++)
				{
					if (this.m_exchangeBatteries[i] != null && this.m_exchangeBatteries[i].Mastery != null && this.m_exchangeBatteries[i].Mastery.RuneSource != RuneSourceType.None)
					{
						this.m_exchangeBatteriesDict.Add(this.m_exchangeBatteries[i].Mastery.RuneSource, this.m_exchangeBatteries[i]);
					}
				}
			}
			return this.m_exchangeBatteriesDict.TryGetValue(requestedType, out battery);
		}

		// Token: 0x04003536 RID: 13622
		public const string kDefaultIndexName = "IndexName";

		// Token: 0x04003537 RID: 13623
		public const float kDefaultAnimationFadeDuration = 0.3f;

		// Token: 0x04003538 RID: 13624
		public const int kHitLevelNormalThreshold = 1;

		// Token: 0x04003539 RID: 13625
		public const int kHitLevelNormalRange = 2;

		// Token: 0x0400353A RID: 13626
		public const float kHitLevelRange = 5f;

		// Token: 0x0400353B RID: 13627
		public const float kAdventuringMinimumXpFraction = 0.3f;

		// Token: 0x0400353C RID: 13628
		public const int kHitLevelUpperLimit = 2;

		// Token: 0x0400353D RID: 13629
		public const int kHitLevelLowerLimit = 10;

		// Token: 0x0400353E RID: 13630
		public const int kHitLevelXpDiminish = 2;

		// Token: 0x0400353F RID: 13631
		public const int kLowLevelRewardLevelSubtract = 3;

		// Token: 0x04003540 RID: 13632
		public const int kHitLevelMiddleLimit = 5;

		// Token: 0x04003541 RID: 13633
		public const float kTradeProfessionLevelRange = 15f;

		// Token: 0x04003542 RID: 13634
		public const float kTradeProfessionMinimumXpFraction = 0.1f;

		// Token: 0x04003543 RID: 13635
		public const float kMaxStamina = 100f;

		// Token: 0x04003544 RID: 13636
		public const float kMaxWound = 100f;

		// Token: 0x04003545 RID: 13637
		public const float kItemsAttachedValidateDelay = 1.1f;

		// Token: 0x04003546 RID: 13638
		public const float kBaseRoleLevelCap = 6f;

		// Token: 0x04003547 RID: 13639
		public const float kMaxLevel = 50f;

		// Token: 0x04003548 RID: 13640
		public const float kMaxNpcLevel = 54f;

		// Token: 0x04003549 RID: 13641
		public const int kMaxArmorClass = 2000;

		// Token: 0x0400354A RID: 13642
		public const float kMaxPlayerAbsorbDamageReduction = 0.5f;

		// Token: 0x0400354B RID: 13643
		public const float kMaxNpcAbsorbDamageReduction = 0.8f;

		// Token: 0x0400354C RID: 13644
		public const int kFirstLevelThresholdForTrade = 3;

		// Token: 0x0400354D RID: 13645
		public const int kSecondLevelThresholdForTrade = 6;

		// Token: 0x0400354E RID: 13646
		public const int kDefaultAutoAttackAnimationIndex = -1;

		// Token: 0x0400354F RID: 13647
		public const int kDefaultItemCharges = 100;

		// Token: 0x04003550 RID: 13648
		public const int kMatchAttackerLevelValue = 0;

		// Token: 0x04003551 RID: 13649
		public const float kDefaultEmoteDistance = 30f;

		// Token: 0x04003552 RID: 13650
		public const float kDefaultEmoteDistanceSquared = 900f;

		// Token: 0x04003553 RID: 13651
		public const float kAutoAfkTime = 300f;

		// Token: 0x04003554 RID: 13652
		public const float kAutoAfkBootTime = 1200f;

		// Token: 0x04003555 RID: 13653
		public const float kLoginAfkBootTime = 1200f;

		// Token: 0x04003556 RID: 13654
		public const int kNewhavenValleyHuntingLogLevelExclusion = 10;

		// Token: 0x04003557 RID: 13655
		private const string kDebugGroup = "Debug";

		// Token: 0x04003558 RID: 13656
		public const string kWaterTag = "Water";

		// Token: 0x04003559 RID: 13657
		public const int kUnlearnedAbilityIndex = -1;

		// Token: 0x0400355A RID: 13658
		public const float kDefaultVfxExpirationTime = 5f;

		// Token: 0x0400355B RID: 13659
		public const int kBuybackExpirationTimeInSeconds = 3600;

		// Token: 0x0400355C RID: 13660
		public const int kBagBuybackExpirationTimeInSeconds = 7200;

		// Token: 0x0400355D RID: 13661
		public const float kLootHighlightFadeTime = 1.5f;

		// Token: 0x0400355E RID: 13662
		public const int kMaxAugmentStackSize = 5;

		// Token: 0x0400355F RID: 13663
		public const bool kEnableCosmeticSlot = true;

		// Token: 0x04003560 RID: 13664
		public const string kEventCurrencyName = "Bloops";

		// Token: 0x04003561 RID: 13665
		public const int kMaxDamageReductionValue = 50;

		// Token: 0x04003562 RID: 13666
		public const float kMaxDamageReductionFraction = 0.5f;

		// Token: 0x04003563 RID: 13667
		public const float kMinimumNegativeHaste = -0.8f;

		// Token: 0x04003564 RID: 13668
		public static bool kAudioListenerAlwaysAtPlayer = true;

		// Token: 0x04003565 RID: 13669
		public static int[] kDecades = new int[]
		{
			1,
			10,
			20,
			30,
			40,
			50
		};

		// Token: 0x04003566 RID: 13670
		public static int[] kEveryFiveLevels = new int[]
		{
			1,
			5,
			10,
			15,
			20,
			25,
			30,
			35,
			40,
			45,
			50
		};

		// Token: 0x04003567 RID: 13671
		private static GlobalSettings m_globalSettings = null;

		// Token: 0x04003568 RID: 13672
		[FormerlySerializedAs("SceneConfig")]
		public ConfigSettings Configs;

		// Token: 0x04003569 RID: 13673
		public StanceSettings Stance;

		// Token: 0x0400356A RID: 13674
		public NpcSettings Npcs;

		// Token: 0x0400356B RID: 13675
		public GroupSettings Group;

		// Token: 0x0400356C RID: 13676
		public IkSettings Ik;

		// Token: 0x0400356D RID: 13677
		public UmaSettings Uma;

		// Token: 0x0400356E RID: 13678
		public AnimationSettings Animation;

		// Token: 0x0400356F RID: 13679
		public AudioSettings Audio;

		// Token: 0x04003570 RID: 13680
		public CombatSettings Combat;

		// Token: 0x04003571 RID: 13681
		public PlayerSettings Player;

		// Token: 0x04003572 RID: 13682
		public ArmorWeightSettings Armor;

		// Token: 0x04003573 RID: 13683
		public GeneralSettings General;

		// Token: 0x04003574 RID: 13684
		public ProgressionSettings Progression;

		// Token: 0x04003575 RID: 13685
		public ChatSettings Chat;

		// Token: 0x04003576 RID: 13686
		public GatheringSettings Gathering;

		// Token: 0x04003577 RID: 13687
		public CraftingSettings Crafting;

		// Token: 0x04003578 RID: 13688
		public RoleSettings Roles;

		// Token: 0x04003579 RID: 13689
		public MapSettings Maps;

		// Token: 0x0400357A RID: 13690
		public CullingSettings Culling;

		// Token: 0x0400357B RID: 13691
		public CameraSettingsG Camera;

		// Token: 0x0400357C RID: 13692
		public RenderSettings Rendering;

		// Token: 0x0400357D RID: 13693
		public NotificationSettings Notifications;

		// Token: 0x0400357E RID: 13694
		public UISettings UI;

		// Token: 0x0400357F RID: 13695
		public AshenSettings Ashen;

		// Token: 0x04003580 RID: 13696
		public HuntingLogSettings HuntingLog;

		// Token: 0x04003581 RID: 13697
		public StatSettings Stats;

		// Token: 0x04003582 RID: 13698
		public SubscriberSettings Subscribers;

		// Token: 0x04003583 RID: 13699
		public PortraitSettings Portraits;

		// Token: 0x04003584 RID: 13700
		public NameplateSettings Nameplates;

		// Token: 0x04003585 RID: 13701
		public TitleSettings Titles;

		// Token: 0x04003586 RID: 13702
		public TaskSettings Tasks;

		// Token: 0x04003587 RID: 13703
		public SocialSettings Social;

		// Token: 0x04003588 RID: 13704
		public DummyClass m_dummy;

		// Token: 0x04003589 RID: 13705
		public SensorDebugSettings[] SensorDebugSettings;

		// Token: 0x0400358A RID: 13706
		public GameObject GroundGizmosPrefab;

		// Token: 0x0400358B RID: 13707
		public int RuneExchangeCost = 10;

		// Token: 0x0400358C RID: 13708
		[SerializeField]
		private RunicBattery[] m_exchangeBatteries;

		// Token: 0x0400358D RID: 13709
		private Dictionary<RuneSourceType, RunicBattery> m_exchangeBatteriesDict;
	}
}
