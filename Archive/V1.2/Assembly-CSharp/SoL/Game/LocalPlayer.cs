using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Player;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game
{
	// Token: 0x020005AB RID: 1451
	public static class LocalPlayer
	{
		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06002D90 RID: 11664 RVA: 0x0014F198 File Offset: 0x0014D398
		// (remove) Token: 0x06002D91 RID: 11665 RVA: 0x0014F1CC File Offset: 0x0014D3CC
		public static event Action LocalPlayerInitialized;

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x06002D92 RID: 11666 RVA: 0x0014F200 File Offset: 0x0014D400
		// (remove) Token: 0x06002D93 RID: 11667 RVA: 0x0014F234 File Offset: 0x0014D434
		public static event Action FadeLoadingScreenShowUi;

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x06002D94 RID: 11668 RVA: 0x0014F268 File Offset: 0x0014D468
		// (remove) Token: 0x06002D95 RID: 11669 RVA: 0x0014F29C File Offset: 0x0014D49C
		public static event Action LocalPlayerDestroyed;

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x06002D96 RID: 11670 RVA: 0x0014F2D0 File Offset: 0x0014D4D0
		// (remove) Token: 0x06002D97 RID: 11671 RVA: 0x0014F304 File Offset: 0x0014D504
		public static event Action<HealthState> LocalPlayerHealthStateUpdated;

		// Token: 0x14000096 RID: 150
		// (add) Token: 0x06002D98 RID: 11672 RVA: 0x0014F338 File Offset: 0x0014D538
		// (remove) Token: 0x06002D99 RID: 11673 RVA: 0x0014F36C File Offset: 0x0014D56C
		public static event Action<GameEntity> LocalPlayerOffensiveTargetChanged;

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x06002D9A RID: 11674 RVA: 0x0014F3A0 File Offset: 0x0014D5A0
		// (remove) Token: 0x06002D9B RID: 11675 RVA: 0x0014F3D4 File Offset: 0x0014D5D4
		public static event Action<GameEntity> LocalPlayerDefensiveTargetChanged;

		// Token: 0x14000098 RID: 152
		// (add) Token: 0x06002D9C RID: 11676 RVA: 0x0014F408 File Offset: 0x0014D608
		// (remove) Token: 0x06002D9D RID: 11677 RVA: 0x0014F43C File Offset: 0x0014D63C
		public static event Action HighestMasteryLevelChanged;

		// Token: 0x14000099 RID: 153
		// (add) Token: 0x06002D9E RID: 11678 RVA: 0x0014F470 File Offset: 0x0014D670
		// (remove) Token: 0x06002D9F RID: 11679 RVA: 0x0014F4A4 File Offset: 0x0014D6A4
		public static event Action SpecializationMaxLevelChanged;

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x06002DA0 RID: 11680 RVA: 0x0014F4D8 File Offset: 0x0014D6D8
		// (remove) Token: 0x06002DA1 RID: 11681 RVA: 0x0014F50C File Offset: 0x0014D70C
		public static event Action AlchemyIIUnlocked;

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06002DA2 RID: 11682 RVA: 0x0005FA5E File Offset: 0x0005DC5E
		// (set) Token: 0x06002DA3 RID: 11683 RVA: 0x0005FA65 File Offset: 0x0005DC65
		public static float TimeOfLastInput { get; private set; }

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06002DA4 RID: 11684 RVA: 0x0005FA6D File Offset: 0x0005DC6D
		public static bool IsAlive
		{
			get
			{
				return LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.IsAlive;
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06002DA5 RID: 11685 RVA: 0x0005FA88 File Offset: 0x0005DC88
		public static bool IsStunned
		{
			get
			{
				return LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.IsStunned;
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x06002DA6 RID: 11686 RVA: 0x0005FAA3 File Offset: 0x0005DCA3
		public static bool IsInitialized
		{
			get
			{
				return LocalPlayer.m_timePlayed != null && LocalPlayer.GameEntity;
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x06002DA7 RID: 11687 RVA: 0x0005FAB8 File Offset: 0x0005DCB8
		public static bool IsPvp
		{
			get
			{
				return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Pvp);
			}
		}

		// Token: 0x1400009B RID: 155
		// (add) Token: 0x06002DA8 RID: 11688 RVA: 0x0014F540 File Offset: 0x0014D740
		// (remove) Token: 0x06002DA9 RID: 11689 RVA: 0x0014F574 File Offset: 0x0014D774
		public static event Action<UniqueId, UniqueId> ActiveAuraChanged;

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06002DAA RID: 11690 RVA: 0x0005FAF7 File Offset: 0x0005DCF7
		// (set) Token: 0x06002DAB RID: 11691 RVA: 0x0014F5A8 File Offset: 0x0014D7A8
		public static EffectSyncData? ActiveAura
		{
			get
			{
				return LocalPlayer.m_activeAura;
			}
			set
			{
				UniqueId arg = (LocalPlayer.m_activeAura != null) ? LocalPlayer.m_activeAura.Value.ArchetypeId : UniqueId.Empty;
				LocalPlayer.m_activeAura = value;
				UniqueId arg2 = (LocalPlayer.m_activeAura != null) ? LocalPlayer.m_activeAura.Value.ArchetypeId : UniqueId.Empty;
				Action<UniqueId, UniqueId> activeAuraChanged = LocalPlayer.ActiveAuraChanged;
				if (activeAuraChanged == null)
				{
					return;
				}
				activeAuraChanged(arg, arg2);
			}
		}

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x06002DAC RID: 11692 RVA: 0x0014F614 File Offset: 0x0014D814
		// (remove) Token: 0x06002DAD RID: 11693 RVA: 0x0014F648 File Offset: 0x0014D848
		public static event Action<GameEntity> FollowTargetChanged;

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06002DAE RID: 11694 RVA: 0x0005FAFE File Offset: 0x0005DCFE
		// (set) Token: 0x06002DAF RID: 11695 RVA: 0x0005FB05 File Offset: 0x0005DD05
		public static GameEntity FollowTarget
		{
			get
			{
				return LocalPlayer.m_followTarget;
			}
			private set
			{
				if (LocalPlayer.m_followTarget == value)
				{
					return;
				}
				LocalPlayer.m_followTarget = value;
				LocalPlayer.m_followTargetPresent = (LocalPlayer.m_followTarget != null);
				LocalPlayer.TriggerFollowTargetChanged();
			}
		}

		// Token: 0x06002DB0 RID: 11696 RVA: 0x0014F67C File Offset: 0x0014D87C
		private static void TriggerFollowTargetChanged()
		{
			Action<GameEntity> followTargetChanged = LocalPlayer.FollowTargetChanged;
			if (followTargetChanged != null)
			{
				followTargetChanged(LocalPlayer.m_followTarget);
			}
			string content = LocalPlayer.m_followTarget ? ZString.Format<string>("You are now following {0}.", (LocalPlayer.m_followTarget && LocalPlayer.m_followTarget.CharacterData) ? LocalPlayer.m_followTarget.CharacterData.Name.Value : "UNKNOWN") : "You are no longer following anyone.";
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
		}

		// Token: 0x06002DB1 RID: 11697 RVA: 0x0005FB30 File Offset: 0x0005DD30
		public static IEnumerator InvokeLocalPlayerInitialized()
		{
			LocalPlayer.LootInteractive = null;
			if (LocalPlayer.m_sessionStart == null)
			{
				LocalPlayer.m_sessionStart = new DateTime?(DateTime.UtcNow);
			}
			LocalPlayer.m_timePlayed = new TimePlayed(LocalPlayer.GameEntity.CollectionController.Record);
			LocalPlayer.UpdateTimeOfLastInput();
			LocalPlayer.SetProcessIsRunningKey();
			yield return LocalPlayer.LocalPlayerInitializedEvent();
			if (ClientGameManager.MainCamera)
			{
				ClientGameManager.MainCamera.gameObject.GetComponent<HDAdditionalCameraData>().volumeAnchorOverride = LocalPlayer.GameEntity.gameObject.transform;
			}
			SolDebug.LogWithTime("Refreshing VSP", true);
			LocalZoneManager.RefreshVegetationStudio();
			SolDebug.LogWithTime("Finished Refreshing VSP", true);
			yield break;
		}

		// Token: 0x06002DB2 RID: 11698 RVA: 0x0005FB38 File Offset: 0x0005DD38
		private static IEnumerator LocalPlayerInitializedEvent()
		{
			if (LocalPlayer.LocalPlayerInitialized != null)
			{
				int setsInvoked = 0;
				int num = 0;
				Delegate[] delegates = LocalPlayer.LocalPlayerInitialized.GetInvocationList();
				SolDebug.LogWithTime(ZString.Format<int>("Invoking LocalPlayerInitialized with {0} delegates", delegates.Length), true);
				int num2;
				for (int i = 0; i < delegates.Length; i = num2 + 1)
				{
					if (delegates[i] != null)
					{
						if (delegates[i].Method != null && delegates[i].Method.DeclaringType != null)
						{
							string text = delegates[i].Method.DeclaringType.ToString();
							string[] array = text.Split(".", StringSplitOptions.None);
							if (array != null && array.Length != 0)
							{
								text = array[array.Length - 1];
							}
							SolDebug.LogWithTime(ZString.Format<string>("...{0}", text), true);
						}
						delegates[i].DynamicInvoke(Array.Empty<object>());
						num++;
						if (num >= 8)
						{
							setsInvoked++;
							yield return null;
							num = 0;
						}
					}
					num2 = i;
				}
				SolDebug.LogWithTime("Finished Invoking LocalPlayerInitialized", true);
				delegates = null;
			}
			yield break;
		}

		// Token: 0x06002DB3 RID: 11699 RVA: 0x0005FB40 File Offset: 0x0005DD40
		public static void GetRemainingSubscribers()
		{
			Debug.Log(LocalPlayer.GetRemainingSubscriberDescription());
		}

		// Token: 0x06002DB4 RID: 11700 RVA: 0x0014F704 File Offset: 0x0014D904
		public static string GetRemainingSubscriberDescription()
		{
			if (LocalPlayer.LocalPlayerInitialized != null)
			{
				Delegate[] invocationList = LocalPlayer.LocalPlayerInitialized.GetInvocationList();
				List<string> fromPool = StaticListPool<string>.GetFromPool();
				fromPool.Add(invocationList.Length.ToString() + " delegates found!");
				for (int i = 0; i < invocationList.Length; i++)
				{
					if (invocationList[i].Method.DeclaringType != null)
					{
						string text = invocationList[i].Method.DeclaringType.ToString();
						string[] array = text.Split(".", StringSplitOptions.None);
						if (array != null && array.Length != 0)
						{
							text = array[array.Length - 1];
						}
						fromPool.Add(text);
					}
				}
				string result = string.Join(", ", fromPool);
				StaticListPool<string>.ReturnToPool(fromPool);
				return result;
			}
			return "No Subscribers!";
		}

		// Token: 0x06002DB5 RID: 11701 RVA: 0x0014F7C0 File Offset: 0x0014D9C0
		public static void ShowUiFadeLoading()
		{
			SolDebug.LogWithTime("Invoking FadeLoadingScreenShowUi", true);
			Action fadeLoadingScreenShowUi = LocalPlayer.FadeLoadingScreenShowUi;
			if (fadeLoadingScreenShowUi != null)
			{
				fadeLoadingScreenShowUi();
			}
			SolDebug.LogWithTime("Finished Invoking FadeLoadingScreenShowUi", true);
			if (LocalZoneManager.ZoneRecord != null)
			{
				CenterScreenAnnouncementOptions opts = new CenterScreenAnnouncementOptions
				{
					Title = LocalPlayer.GetFormattedZoneName(),
					TimeShown = 5f,
					ShowDelay = 2f
				};
				ClientGameManager.UIManager.InitCenterScreenAnnouncement(opts);
			}
		}

		// Token: 0x06002DB6 RID: 11702 RVA: 0x0014F834 File Offset: 0x0014DA34
		public static string GetFormattedZoneName()
		{
			string result = string.Empty;
			if (LocalZoneManager.ZoneRecord != null)
			{
				string displayName = LocalZoneManager.ZoneRecord.DisplayName;
				string empty = string.Empty;
				SubZoneId subZoneId = SubZoneId.None;
				SpawnVolumeOverride spawnVolumeOverride;
				if (LocalPlayer.GameEntity && LocalZoneManager.TryGetSpawnVolumeOverride(LocalPlayer.GameEntity.gameObject.transform.position, out spawnVolumeOverride))
				{
					string zoneNameSuffix = spawnVolumeOverride.ZoneNameSuffix;
					subZoneId = spawnVolumeOverride.SubZoneId;
				}
				result = LocalZoneManager.GetFormattedZoneName(displayName, subZoneId);
			}
			return result;
		}

		// Token: 0x06002DB7 RID: 11703 RVA: 0x0014F8A0 File Offset: 0x0014DAA0
		public static string GetZoneNameForMap(ZoneId zid)
		{
			ZoneRecord zoneRecord = SessionData.GetZoneRecord(zid);
			if (zoneRecord == null)
			{
				return string.Empty;
			}
			return zoneRecord.DisplayName;
		}

		// Token: 0x06002DB8 RID: 11704 RVA: 0x0005FB4C File Offset: 0x0005DD4C
		public static void InvokeHighestMasteryLevelChanged()
		{
			Action highestMasteryLevelChanged = LocalPlayer.HighestMasteryLevelChanged;
			if (highestMasteryLevelChanged == null)
			{
				return;
			}
			highestMasteryLevelChanged();
		}

		// Token: 0x06002DB9 RID: 11705 RVA: 0x0005FB5D File Offset: 0x0005DD5D
		public static void InvokeSpecializationMaxLevelChanged()
		{
			Action specializationMaxLevelChanged = LocalPlayer.SpecializationMaxLevelChanged;
			if (specializationMaxLevelChanged == null)
			{
				return;
			}
			specializationMaxLevelChanged();
		}

		// Token: 0x06002DBA RID: 11706 RVA: 0x0005FB6E File Offset: 0x0005DD6E
		public static void InvokeAlchemyIIUnlocked()
		{
			Action alchemyIIUnlocked = LocalPlayer.AlchemyIIUnlocked;
			if (alchemyIIUnlocked == null)
			{
				return;
			}
			alchemyIIUnlocked();
		}

		// Token: 0x06002DBB RID: 11707 RVA: 0x0014F8C4 File Offset: 0x0014DAC4
		public static void InvokeLocalPlayerDestroyed()
		{
			LocalPlayer.m_timePlayed = null;
			LocalPlayer.ActiveAura = null;
			LocalPlayer.FollowTarget = null;
			Action localPlayerDestroyed = LocalPlayer.LocalPlayerDestroyed;
			if (localPlayerDestroyed == null)
			{
				return;
			}
			localPlayerDestroyed();
		}

		// Token: 0x06002DBC RID: 11708 RVA: 0x0005FB7F File Offset: 0x0005DD7F
		public static void HealthStateUpdated(HealthState state)
		{
			Action<HealthState> localPlayerHealthStateUpdated = LocalPlayer.LocalPlayerHealthStateUpdated;
			if (localPlayerHealthStateUpdated == null)
			{
				return;
			}
			localPlayerHealthStateUpdated(state);
		}

		// Token: 0x06002DBD RID: 11709 RVA: 0x0014F8FC File Offset: 0x0014DAFC
		public static string GetDebugPositionString()
		{
			if (!(LocalPlayer.GameEntity != null))
			{
				return string.Empty;
			}
			return new DebugLocation(LocalZoneManager.ZoneRecord.ZoneId, LocalPlayer.GameEntity.gameObject).DebugString;
		}

		// Token: 0x06002DBE RID: 11710 RVA: 0x0005FB91 File Offset: 0x0005DD91
		public static void CopyTeleportStringToClipboard()
		{
			GUIUtility.systemCopyBuffer = LocalPlayer.GetDebugPositionString();
		}

		// Token: 0x06002DBF RID: 11711 RVA: 0x0014F940 File Offset: 0x0014DB40
		public static string GetTimePlayed()
		{
			if (!LocalPlayer.GameEntity || LocalPlayer.m_timePlayed == null)
			{
				return "Unknown";
			}
			DateTime dateTime = LocalPlayer.GameEntity.CollectionController.Record.Created.ToLocalTime();
			TimeSpan updateTimePlayed = LocalPlayer.m_timePlayed.GetUpdateTimePlayed();
			string result = null;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendFormat<string>("Time stats for {0}:", LocalPlayer.GameEntity.CharacterData.Name.Value);
				utf16ValueStringBuilder.AppendLine();
				utf16ValueStringBuilder.AppendFormat<string>("Created: {0}", dateTime.ToString("MMMM dd, yyyy @ hh:mm tt"));
				utf16ValueStringBuilder.AppendLine();
				utf16ValueStringBuilder.AppendFormat<string>("Played: {0}", updateTimePlayed.GetFormatted());
				if (LocalPlayer.m_sessionStart != null)
				{
					utf16ValueStringBuilder.AppendLine();
					TimeSpan timeSpan = DateTime.UtcNow - LocalPlayer.m_sessionStart.Value;
					utf16ValueStringBuilder.AppendFormat<string>("Session: {0}", timeSpan.GetFormatted());
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06002DC0 RID: 11712 RVA: 0x0005FB9D File Offset: 0x0005DD9D
		public static void ResetSessionStart()
		{
			LocalPlayer.m_sessionStart = null;
		}

		// Token: 0x06002DC1 RID: 11713 RVA: 0x0005FBAA File Offset: 0x0005DDAA
		public static void TriggerOffensiveTargetChanged(GameEntity target)
		{
			Action<GameEntity> localPlayerOffensiveTargetChanged = LocalPlayer.LocalPlayerOffensiveTargetChanged;
			if (localPlayerOffensiveTargetChanged == null)
			{
				return;
			}
			localPlayerOffensiveTargetChanged(target);
		}

		// Token: 0x06002DC2 RID: 11714 RVA: 0x0005FBBC File Offset: 0x0005DDBC
		public static void TriggerDefensiveTargetChanged(GameEntity target)
		{
			Action<GameEntity> localPlayerDefensiveTargetChanged = LocalPlayer.LocalPlayerDefensiveTargetChanged;
			if (localPlayerDefensiveTargetChanged == null)
			{
				return;
			}
			localPlayerDefensiveTargetChanged(target);
		}

		// Token: 0x06002DC3 RID: 11715 RVA: 0x0005FBCE File Offset: 0x0005DDCE
		public static void SetProcessIsRunningKey()
		{
			LocalPlayer.SetProcessIsRunningKey((LocalZoneManager.ZoneRecord != null) ? LocalZoneManager.ZoneRecord.DisplayName.Replace(' ', '_') : "UNKNOWN_ZONE");
		}

		// Token: 0x06002DC4 RID: 11716 RVA: 0x0005FBF6 File Offset: 0x0005DDF6
		public static void SetProcessIsRunningKey(string description)
		{
			PlayerPrefs.SetString("ProcessIsRunning", description);
		}

		// Token: 0x06002DC5 RID: 11717 RVA: 0x0005FC03 File Offset: 0x0005DE03
		public static void DeleteProcessIsRunningKey()
		{
			PlayerPrefs.DeleteKey("ProcessIsRunning");
		}

		// Token: 0x06002DC6 RID: 11718 RVA: 0x0005FC0F File Offset: 0x0005DE0F
		public static string GetProcessIsRunningValue()
		{
			return PlayerPrefs.GetString("ProcessIsRunning", string.Empty);
		}

		// Token: 0x06002DC7 RID: 11719 RVA: 0x0005FC20 File Offset: 0x0005DE20
		public static void UpdateTimeOfLastInput()
		{
			LocalPlayer.TimeOfLastInput = Time.time;
		}

		// Token: 0x06002DC8 RID: 11720 RVA: 0x0014FA58 File Offset: 0x0014DC58
		public static void SetFollowTarget(GameEntity entity)
		{
			if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Subscriber)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "The follow feature is reserved for subscribers.");
				return;
			}
			if (!entity || entity.Type != GameEntityType.Player || entity == LocalPlayer.GameEntity)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No one to follow! Please select a defensive target or pass their name (/follow PlayerName).");
				return;
			}
			if (!entity.IsAlive || entity.IsConfused || entity.IsStunned)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot follow in this state!");
				return;
			}
			if (!ClientGameManager.GroupManager || !ClientGameManager.GroupManager.IsMemberOfMyGroup(entity))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Can only follow members of your group!");
				return;
			}
			if (entity.GetCachedSqrDistanceFromLocalPlayer() > 144f)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "That player is too far away to follow!");
				return;
			}
			LocalPlayer.FollowTarget = entity;
		}

		// Token: 0x06002DC9 RID: 11721 RVA: 0x0005FC2C File Offset: 0x0005DE2C
		public static void ClearFollowTarget()
		{
			LocalPlayer.FollowTarget = null;
		}

		// Token: 0x06002DCA RID: 11722 RVA: 0x0014FB3C File Offset: 0x0014DD3C
		public static void ValidateFollowTarget()
		{
			if (LocalPlayer.FollowTarget)
			{
				if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.IsAlive || LocalPlayer.GameEntity.IsStunned || LocalPlayer.GameEntity.IsConfused || !ClientGameManager.GroupManager || !ClientGameManager.GroupManager.IsMemberOfMyGroup(LocalPlayer.FollowTarget))
				{
					LocalPlayer.ClearFollowTarget();
					return;
				}
			}
			else if (LocalPlayer.m_followTargetPresent)
			{
				LocalPlayer.m_followTargetPresent = false;
				LocalPlayer.TriggerFollowTargetChanged();
			}
		}

		// Token: 0x06002DCB RID: 11723 RVA: 0x0014FBBC File Offset: 0x0014DDBC
		public static bool IsActiveCharacter(string characterId)
		{
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Id == characterId;
		}

		// Token: 0x04002D23 RID: 11555
		public static GameEntity GameEntity;

		// Token: 0x04002D24 RID: 11556
		public static PlayerMotorController Motor;

		// Token: 0x04002D25 RID: 11557
		public static PlayerAnimancerController Animancer;

		// Token: 0x04002D26 RID: 11558
		public static NetworkEntity NetworkEntity;

		// Token: 0x04002D27 RID: 11559
		public static DetectionCollider DetectionCollider;

		// Token: 0x04002D28 RID: 11560
		public static Corpse Corpse;

		// Token: 0x04002D29 RID: 11561
		public static IInteractive LootInteractive;

		// Token: 0x04002D2A RID: 11562
		public static bool NameplateDebug;

		// Token: 0x04002D2C RID: 11564
		private static DateTime? m_sessionStart;

		// Token: 0x04002D2D RID: 11565
		private static TimePlayed m_timePlayed;

		// Token: 0x04002D2F RID: 11567
		private static EffectSyncData? m_activeAura;

		// Token: 0x04002D31 RID: 11569
		private static bool m_followTargetPresent;

		// Token: 0x04002D32 RID: 11570
		private static GameEntity m_followTarget;

		// Token: 0x04002D33 RID: 11571
		private const string kProcessIsRunningKey = "ProcessIsRunning";
	}
}
