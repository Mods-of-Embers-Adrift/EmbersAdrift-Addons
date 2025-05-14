using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Messages;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Player
{
	// Token: 0x020007E8 RID: 2024
	public class CampManager : MonoBehaviour
	{
		// Token: 0x17000D7C RID: 3452
		// (get) Token: 0x06003ADF RID: 15071 RVA: 0x00067E0B File Offset: 0x0006600B
		// (set) Token: 0x06003AE0 RID: 15072 RVA: 0x00067E12 File Offset: 0x00066012
		public static bool Camping { get; private set; }

		// Token: 0x17000D7D RID: 3453
		// (get) Token: 0x06003AE1 RID: 15073 RVA: 0x00067E1A File Offset: 0x0006601A
		// (set) Token: 0x06003AE2 RID: 15074 RVA: 0x00067E21 File Offset: 0x00066021
		public static int TimeRemaining { get; private set; }

		// Token: 0x06003AE3 RID: 15075 RVA: 0x00179804 File Offset: 0x00177A04
		private void Awake()
		{
			if (CampManager.m_campTimeNotificationStatus == null)
			{
				CampManager.m_campTimeNotificationStatus = new Dictionary<int, bool>(CampManager.kCampTimeNotifications.Length);
				for (int i = 0; i < CampManager.kCampTimeNotifications.Length; i++)
				{
					CampManager.m_campTimeNotificationStatus.Add(CampManager.kCampTimeNotifications[i], false);
				}
			}
		}

		// Token: 0x06003AE4 RID: 15076 RVA: 0x00067E29 File Offset: 0x00066029
		public void CancelCamp()
		{
			if (this.m_campDelayCo != null)
			{
				base.StopCoroutine(this.m_campDelayCo);
				this.m_campDelayCo = null;
			}
			this.AddNotificationToChat("Camp preparations abandoned.");
			CampManager.Camping = false;
			CampManager.TimeRemaining = 0;
		}

		// Token: 0x06003AE5 RID: 15077 RVA: 0x00179850 File Offset: 0x00177A50
		public void InitiateCamp(bool returnToCharacterSelection)
		{
			this.DelayedCampInternal(delegate
			{
				SessionData.ReturnToCharacterSelection = returnToCharacterSelection;
				CampManager.ReloadStartup();
			});
		}

		// Token: 0x06003AE6 RID: 15078 RVA: 0x00067E5D File Offset: 0x0006605D
		public void Quit()
		{
			this.DelayedCampInternal(delegate
			{
				Application.Quit();
			});
		}

		// Token: 0x06003AE7 RID: 15079 RVA: 0x0017987C File Offset: 0x00177A7C
		private void DelayedCampInternal(Action callback)
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null)
			{
				CampManager.Camping = false;
				if (callback != null)
				{
					callback();
				}
				return;
			}
			if (LocalZoneManager.ZoneRecord != null && Enum.IsDefined(typeof(ZoneId), LocalZoneManager.ZoneRecord.ZoneId) && ((ZoneId)LocalZoneManager.ZoneRecord.ZoneId).AvoidCampDelay())
			{
				CampManager.Camping = false;
				if (callback != null)
				{
					callback();
				}
				return;
			}
			int num = LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat) ? 1 : 0;
			bool flag = LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire);
			if (num == 0 && flag)
			{
				CampManager.Camping = false;
				if (callback != null)
				{
					callback();
				}
				return;
			}
			if (this.m_campDelayCo != null)
			{
				base.StopCoroutine(this.m_campDelayCo);
				CampManager.Camping = false;
			}
			float delay = flag ? 10f : 20f;
			this.m_campDelayCo = this.CampCo(callback, delay);
			base.StartCoroutine(this.m_campDelayCo);
		}

		// Token: 0x06003AE8 RID: 15080 RVA: 0x00067E84 File Offset: 0x00066084
		private IEnumerator CampCo(Action callback, float delay)
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.Animancer == null)
			{
				CampManager.Camping = false;
				yield break;
			}
			CampManager.Camping = true;
			if (LocalPlayer.Animancer.Stance != Stance.Sit)
			{
				LocalPlayer.Animancer.SetStance(Stance.Sit);
			}
			int num = Mathf.CeilToInt(delay);
			this.AddNotificationToChat(string.Format("{0}s to prepare camp...", num));
			for (int i = 0; i < CampManager.kCampTimeNotifications.Length; i++)
			{
				CampManager.m_campTimeNotificationStatus[CampManager.kCampTimeNotifications[i]] = (CampManager.kCampTimeNotifications[i] == num);
			}
			float time = 0f;
			bool currentlyWaiting = true;
			while (currentlyWaiting)
			{
				time += Time.deltaTime;
				CampManager.TimeRemaining = Mathf.CeilToInt(delay - time);
				currentlyWaiting = (time < delay && LocalPlayer.Animancer && LocalPlayer.Animancer.Stance == Stance.Sit);
				bool flag;
				if (CampManager.m_campTimeNotificationStatus.TryGetValue(CampManager.TimeRemaining, out flag) && !flag)
				{
					this.AddNotificationToChat(CampManager.TimeRemaining.ToString() + "s remaining for camp...");
					CampManager.m_campTimeNotificationStatus[CampManager.TimeRemaining] = true;
				}
				yield return null;
			}
			if (LocalPlayer.Animancer == null || LocalPlayer.Animancer.Stance != Stance.Sit)
			{
				CampManager.Camping = false;
				this.AddNotificationToChat("Camp preparations abandoned.");
				yield break;
			}
			CampManager.Camping = false;
			this.m_campDelayCo = null;
			if (callback != null)
			{
				callback();
			}
			yield return null;
			yield break;
		}

		// Token: 0x06003AE9 RID: 15081 RVA: 0x00067EA1 File Offset: 0x000660A1
		public static void ReloadStartup()
		{
			if (GameManager.NetworkManager.ConnectionIsActive)
			{
				GameManager.NetworkManager.Disconnect();
				return;
			}
			GameManager.SceneCompositionManager.ReloadStartupScene();
		}

		// Token: 0x06003AEA RID: 15082 RVA: 0x00067EC4 File Offset: 0x000660C4
		private void AddNotificationToChat(string msg)
		{
			if (LocalPlayer.GameEntity && MessageManager.ChatQueue != null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, msg);
			}
		}

		// Token: 0x0400396C RID: 14700
		private const float kCampDelayAtEmberRing = 10f;

		// Token: 0x0400396D RID: 14701
		private const float kCampDelayNotAtEmberRing = 20f;

		// Token: 0x0400396E RID: 14702
		private IEnumerator m_campDelayCo;

		// Token: 0x0400396F RID: 14703
		private const string kCancelledNotification = "Camp preparations abandoned.";

		// Token: 0x04003970 RID: 14704
		private static readonly int[] kCampTimeNotifications = new int[]
		{
			30,
			25,
			20,
			15,
			10,
			5,
			4,
			3,
			2,
			1
		};

		// Token: 0x04003971 RID: 14705
		private static Dictionary<int, bool> m_campTimeNotificationStatus = null;
	}
}
