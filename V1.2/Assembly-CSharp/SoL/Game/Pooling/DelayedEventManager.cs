using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.Audio;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007CE RID: 1998
	public class DelayedEventManager : MonoBehaviour
	{
		// Token: 0x06003A5C RID: 14940 RVA: 0x000678D1 File Offset: 0x00065AD1
		private void Start()
		{
			this.m_nextCleanup = Time.time + 10f;
		}

		// Token: 0x06003A5D RID: 14941 RVA: 0x0017779C File Offset: 0x0017599C
		private void Update()
		{
			float time = Time.time;
			if (time > this.m_nextCleanup)
			{
				this.CleanupDelayedEvents(time, this.m_delayedEvents);
				this.CleanupDelayedEvents(time, this.m_delayedDefendEvents);
				this.m_nextCleanup = time + GlobalSettings.Values.Combat.GlobalCooldown;
			}
		}

		// Token: 0x06003A5E RID: 14942 RVA: 0x001777EC File Offset: 0x001759EC
		private void CleanupDelayedEvents(float now, Dictionary<uint, DelayedEventManager.DelayedEvent> data)
		{
			if (data == null)
			{
				return;
			}
			foreach (KeyValuePair<uint, DelayedEventManager.DelayedEvent> keyValuePair in data)
			{
				if (keyValuePair.Value == null)
				{
					this.m_delayedEventKeysToRemove.Add(keyValuePair.Key);
				}
				else if (now - keyValuePair.Value.Timestamp > GlobalSettings.Values.Combat.GlobalCooldown)
				{
					StaticPool<DelayedEventManager.DelayedEvent>.ReturnToPool(keyValuePair.Value);
					this.m_delayedEventKeysToRemove.Add(keyValuePair.Key);
				}
			}
			for (int i = 0; i < this.m_delayedEventKeysToRemove.Count; i++)
			{
				data.Remove(this.m_delayedEventKeysToRemove[i]);
			}
			this.m_delayedEventKeysToRemove.Clear();
		}

		// Token: 0x06003A5F RID: 14943 RVA: 0x001778C8 File Offset: 0x00175AC8
		public void RegisterDelayedVFX(PooledVFX sourceApplication, GameEntity sourceEntity, PooledVFX targetApplication, GameEntity targetEntity)
		{
			if ((sourceApplication || targetApplication) && sourceEntity && sourceEntity.NetworkEntity)
			{
				uint value = sourceEntity.NetworkEntity.NetworkId.Value;
				DelayedEventManager.DelayedAction fromPool = StaticPool<DelayedEventManager.DelayedAction>.GetFromPool();
				fromPool.Type = DelayedEventManager.DelayedActionType.Vfx;
				fromPool.VfxSource = sourceEntity;
				fromPool.VfxSourceApplication = sourceApplication;
				fromPool.VfxTarget = targetEntity;
				fromPool.VfxTargetApplication = targetApplication;
				DelayedEventManager.DelayedEvent fromPool2;
				if (this.m_delayedEvents.TryGetValue(value, out fromPool2))
				{
					fromPool2.RefreshTimestamp();
				}
				else
				{
					fromPool2 = StaticPool<DelayedEventManager.DelayedEvent>.GetFromPool();
					fromPool2.Key = value;
					fromPool2.RefreshTimestamp();
					this.m_delayedEvents.Add(value, fromPool2);
				}
				fromPool2.AddAction(fromPool);
			}
		}

		// Token: 0x06003A60 RID: 14944 RVA: 0x00177980 File Offset: 0x00175B80
		public void RegisterDelayedEvent(uint key, ref CombatTextManager.AudioEvent? weaponAudioEvent, ref CombatTextManager.AudioEvent? hitAudioEvent, ref CombatTextManager.AnimEvent? hitAnimEvent, ref CombatTextManager.CombatTextEvent? combatTextEvent)
		{
			if (weaponAudioEvent == null && hitAudioEvent == null && hitAnimEvent == null)
			{
				return;
			}
			DelayedEventManager.DelayedEvent fromPool;
			if (!this.m_delayedEvents.TryGetValue(key, out fromPool))
			{
				fromPool = StaticPool<DelayedEventManager.DelayedEvent>.GetFromPool();
				fromPool.Key = key;
				this.m_delayedEvents.Add(key, fromPool);
			}
			DelayedEventManager.DelayedAction delayedAction = null;
			DelayedEventManager.DelayedAction delayedAction2 = null;
			fromPool.RefreshTimestamp();
			DelayedEventManager.DelayedAction delayedAudio = this.GetDelayedAudio(ref weaponAudioEvent);
			DelayedEventManager.DelayedAction delayedAudio2 = this.GetDelayedAudio(ref hitAudioEvent);
			if (delayedAudio2 != null)
			{
				delayedAudio2.IsHitAudio = true;
			}
			if (hitAnimEvent != null && hitAnimEvent.Value.Entity && hitAnimEvent.Value.Entity.AnimancerController != null)
			{
				delayedAction2 = StaticPool<DelayedEventManager.DelayedAction>.GetFromPool();
				delayedAction2.Type = DelayedEventManager.DelayedActionType.AnimEvent;
				delayedAction2.AnimController = hitAnimEvent.Value.Entity.AnimancerController;
				delayedAction2.AnimEvent = hitAnimEvent.Value.TriggerType;
				delayedAction2.LinkedHitAudio = delayedAudio2;
			}
			if (combatTextEvent != null && combatTextEvent.Value.Entity && !string.IsNullOrEmpty(combatTextEvent.Value.Text))
			{
				delayedAction = StaticPool<DelayedEventManager.DelayedAction>.GetFromPool();
				delayedAction.Type = DelayedEventManager.DelayedActionType.CombatText;
				delayedAction.CombatText = combatTextEvent.Value.Text;
				delayedAction.CombatTextEntity = combatTextEvent.Value.Entity;
				delayedAction.CombatTextColor = combatTextEvent.Value.Color;
				delayedAction.CombatTextIcon = combatTextEvent.Value.Icon;
			}
			fromPool.AddAction(delayedAction);
			fromPool.AddAction(delayedAudio);
			fromPool.AddAction(delayedAction2);
			fromPool.AddAction(delayedAudio2);
		}

		// Token: 0x06003A61 RID: 14945 RVA: 0x00177B0C File Offset: 0x00175D0C
		public void RegisterDelayedDefendEvent(uint key, ref CombatTextManager.AudioEvent? parryRiposteAudioEvent, ref CombatTextManager.AudioEvent? blockAudioEvent)
		{
			if (parryRiposteAudioEvent == null && blockAudioEvent == null)
			{
				return;
			}
			DelayedEventManager.DelayedEvent fromPool;
			if (!this.m_delayedDefendEvents.TryGetValue(key, out fromPool))
			{
				fromPool = StaticPool<DelayedEventManager.DelayedEvent>.GetFromPool();
				fromPool.Key = key;
				this.m_delayedDefendEvents.Add(key, fromPool);
			}
			fromPool.RefreshTimestamp();
			DelayedEventManager.DelayedAction delayedAudio = this.GetDelayedAudio(ref parryRiposteAudioEvent);
			DelayedEventManager.DelayedAction delayedAudio2 = this.GetDelayedAudio(ref blockAudioEvent);
			fromPool.AddAction(delayedAudio);
			fromPool.AddAction(delayedAudio2);
		}

		// Token: 0x06003A62 RID: 14946 RVA: 0x00177B7C File Offset: 0x00175D7C
		private DelayedEventManager.DelayedAction GetDelayedAudio(ref CombatTextManager.AudioEvent? incomingEvent)
		{
			if (incomingEvent != null && incomingEvent.Value.Entity && incomingEvent.Value.Entity.AudioEventController)
			{
				DelayedEventManager.DelayedAction fromPool = StaticPool<DelayedEventManager.DelayedAction>.GetFromPool();
				fromPool.Type = DelayedEventManager.DelayedActionType.Audio;
				fromPool.AudioController = incomingEvent.Value.Entity.AudioEventController;
				fromPool.AudioEventName = incomingEvent.Value.EventName;
				fromPool.VolumeFraction = incomingEvent.Value.VolumeFraction;
				return fromPool;
			}
			return null;
		}

		// Token: 0x06003A63 RID: 14947 RVA: 0x00177C00 File Offset: 0x00175E00
		public void ExecuteAbility(uint networkId)
		{
			DelayedEventManager.DelayedEvent delayedEvent;
			if (this.m_delayedEvents.TryGetValue(networkId, out delayedEvent))
			{
				delayedEvent.ExecuteActions();
				this.m_delayedEvents.Remove(delayedEvent.Key);
				StaticPool<DelayedEventManager.DelayedEvent>.ReturnToPool(delayedEvent);
			}
		}

		// Token: 0x06003A64 RID: 14948 RVA: 0x00177C3C File Offset: 0x00175E3C
		public bool Defend(uint networkId)
		{
			DelayedEventManager.DelayedEvent delayedEvent;
			if (this.m_delayedDefendEvents.TryGetValue(networkId, out delayedEvent))
			{
				delayedEvent.ExecuteActions();
				this.m_delayedDefendEvents.Remove(delayedEvent.Key);
				StaticPool<DelayedEventManager.DelayedEvent>.ReturnToPool(delayedEvent);
				return true;
			}
			return false;
		}

		// Token: 0x040038CA RID: 14538
		private const int kInitialCapacity = 32;

		// Token: 0x040038CB RID: 14539
		private readonly Dictionary<uint, DelayedEventManager.DelayedEvent> m_delayedEvents = new Dictionary<uint, DelayedEventManager.DelayedEvent>(32);

		// Token: 0x040038CC RID: 14540
		private readonly Dictionary<uint, DelayedEventManager.DelayedEvent> m_delayedDefendEvents = new Dictionary<uint, DelayedEventManager.DelayedEvent>(32);

		// Token: 0x040038CD RID: 14541
		private readonly List<uint> m_delayedEventKeysToRemove = new List<uint>(32);

		// Token: 0x040038CE RID: 14542
		private float m_nextCleanup = float.MinValue;

		// Token: 0x020007CF RID: 1999
		private class DelayedEvent : IPoolable
		{
			// Token: 0x17000D5C RID: 3420
			// (get) Token: 0x06003A66 RID: 14950 RVA: 0x0006791E File Offset: 0x00065B1E
			// (set) Token: 0x06003A67 RID: 14951 RVA: 0x00067926 File Offset: 0x00065B26
			public float Timestamp { get; private set; }

			// Token: 0x06003A68 RID: 14952 RVA: 0x0006792F File Offset: 0x00065B2F
			public void RefreshTimestamp()
			{
				this.Timestamp = Time.time;
			}

			// Token: 0x06003A69 RID: 14953 RVA: 0x0006793C File Offset: 0x00065B3C
			public void AddAction(DelayedEventManager.DelayedAction action)
			{
				if (action != null)
				{
					this.m_actions.Add(action);
				}
			}

			// Token: 0x06003A6A RID: 14954 RVA: 0x00177C7C File Offset: 0x00175E7C
			public void ExecuteActions()
			{
				for (int i = 0; i < this.m_actions.Count; i++)
				{
					DelayedEventManager.DelayedAction delayedAction = this.m_actions[i];
					if (delayedAction != null)
					{
						delayedAction.ExecuteDelayedEvent();
					}
				}
			}

			// Token: 0x17000D5D RID: 3421
			// (get) Token: 0x06003A6B RID: 14955 RVA: 0x0006794D File Offset: 0x00065B4D
			// (set) Token: 0x06003A6C RID: 14956 RVA: 0x00067955 File Offset: 0x00065B55
			public bool InPool { get; set; }

			// Token: 0x06003A6D RID: 14957 RVA: 0x00177CB8 File Offset: 0x00175EB8
			void IPoolable.Reset()
			{
				this.Key = 0U;
				this.Timestamp = float.MinValue;
				for (int i = 0; i < this.m_actions.Count; i++)
				{
					StaticPool<DelayedEventManager.DelayedAction>.ReturnToPool(this.m_actions[i]);
				}
				this.m_actions.Clear();
			}

			// Token: 0x06003A6E RID: 14958 RVA: 0x0004475B File Offset: 0x0004295B
			public void LogStatus(string status)
			{
			}

			// Token: 0x040038CF RID: 14543
			public uint Key;

			// Token: 0x040038D1 RID: 14545
			private readonly List<DelayedEventManager.DelayedAction> m_actions = new List<DelayedEventManager.DelayedAction>(10);
		}

		// Token: 0x020007D0 RID: 2000
		private enum DelayedActionType
		{
			// Token: 0x040038D4 RID: 14548
			None,
			// Token: 0x040038D5 RID: 14549
			CombatText,
			// Token: 0x040038D6 RID: 14550
			Vfx,
			// Token: 0x040038D7 RID: 14551
			Audio,
			// Token: 0x040038D8 RID: 14552
			AnimEvent
		}

		// Token: 0x020007D1 RID: 2001
		private class DelayedAction : IPoolable
		{
			// Token: 0x06003A70 RID: 14960 RVA: 0x00177D0C File Offset: 0x00175F0C
			public void ExecuteDelayedEvent()
			{
				switch (this.Type)
				{
				case DelayedEventManager.DelayedActionType.CombatText:
					if (this.CombatTextEntity)
					{
						ClientGameManager.CombatTextManager.InitializeOverheadCombatText(this.CombatText, this.CombatTextEntity, this.CombatTextColor, this.CombatTextIcon);
						return;
					}
					break;
				case DelayedEventManager.DelayedActionType.Vfx:
					if (this.VfxSourceApplication != null && this.VfxSource)
					{
						this.VfxSourceApplication.GetPooledInstance<PooledVFX>().Initialize(this.VfxSource, 5f, null);
					}
					if (this.VfxTargetApplication != null && this.VfxTarget)
					{
						this.VfxTargetApplication.GetPooledInstance<PooledVFX>().Initialize(this.VfxTarget, 5f, this.VfxSource);
						return;
					}
					break;
				case DelayedEventManager.DelayedActionType.Audio:
					if (this.AudioController)
					{
						if (!this.IsHitAudio)
						{
							this.AudioController.PlayAudioEvent(this.AudioEventName, this.VolumeFraction);
							return;
						}
						if (this.PlayAnimHitAudio || UnityEngine.Random.Range(0f, 1f) <= GlobalSettings.Values.Audio.TriggerHitAudioEventOnHitChance)
						{
							this.AudioController.PlayAudioEvent(this.AudioEventName, this.VolumeFraction);
							return;
						}
					}
					break;
				case DelayedEventManager.DelayedActionType.AnimEvent:
				{
					bool playAnimHitAudio = false;
					if (this.AnimController != null)
					{
						bool flag = this.AnimController.TriggerEvent(this.AnimEvent);
						if (this.AnimEvent == AnimationEventTriggerType.Hit)
						{
							playAnimHitAudio = flag;
						}
					}
					if (this.LinkedHitAudio != null)
					{
						this.LinkedHitAudio.PlayAnimHitAudio = playAnimHitAudio;
					}
					break;
				}
				default:
					return;
				}
			}

			// Token: 0x17000D5E RID: 3422
			// (get) Token: 0x06003A71 RID: 14961 RVA: 0x00067973 File Offset: 0x00065B73
			// (set) Token: 0x06003A72 RID: 14962 RVA: 0x0006797B File Offset: 0x00065B7B
			public bool InPool { get; set; }

			// Token: 0x06003A73 RID: 14963 RVA: 0x00177E94 File Offset: 0x00176094
			public void Reset()
			{
				this.CombatText = string.Empty;
				this.CombatTextEntity = null;
				this.CombatTextColor = default(Color);
				this.CombatTextIcon = null;
				this.VfxSource = null;
				this.VfxSourceApplication = null;
				this.VfxTarget = null;
				this.VfxTargetApplication = null;
				this.AudioController = null;
				this.AudioEventName = string.Empty;
				this.VolumeFraction = 1f;
				this.IsHitAudio = false;
				this.PlayAnimHitAudio = false;
				this.AnimController = null;
				this.AnimEvent = AnimationEventTriggerType.None;
				this.LinkedHitAudio = null;
			}

			// Token: 0x040038D9 RID: 14553
			public DelayedEventManager.DelayedActionType Type;

			// Token: 0x040038DA RID: 14554
			public string CombatText;

			// Token: 0x040038DB RID: 14555
			public GameEntity CombatTextEntity;

			// Token: 0x040038DC RID: 14556
			public Color CombatTextColor;

			// Token: 0x040038DD RID: 14557
			public Sprite CombatTextIcon;

			// Token: 0x040038DE RID: 14558
			public GameEntity VfxSource;

			// Token: 0x040038DF RID: 14559
			public PooledVFX VfxSourceApplication;

			// Token: 0x040038E0 RID: 14560
			public GameEntity VfxTarget;

			// Token: 0x040038E1 RID: 14561
			public PooledVFX VfxTargetApplication;

			// Token: 0x040038E2 RID: 14562
			public AudioEventController AudioController;

			// Token: 0x040038E3 RID: 14563
			public string AudioEventName;

			// Token: 0x040038E4 RID: 14564
			public float VolumeFraction;

			// Token: 0x040038E5 RID: 14565
			public bool IsHitAudio;

			// Token: 0x040038E6 RID: 14566
			private bool PlayAnimHitAudio;

			// Token: 0x040038E7 RID: 14567
			public IAnimationController AnimController;

			// Token: 0x040038E8 RID: 14568
			public AnimationEventTriggerType AnimEvent;

			// Token: 0x040038E9 RID: 14569
			public DelayedEventManager.DelayedAction LinkedHitAudio;
		}
	}
}
