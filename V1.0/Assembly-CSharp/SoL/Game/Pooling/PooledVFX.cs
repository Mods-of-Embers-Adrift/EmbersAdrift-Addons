using System;
using SoL.Game.Audio;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007E2 RID: 2018
	public class PooledVFX : PooledObject
	{
		// Token: 0x17000D75 RID: 3445
		// (get) Token: 0x06003AC1 RID: 15041 RVA: 0x00067D54 File Offset: 0x00065F54
		public bool HasTimeoutOverride
		{
			get
			{
				return this.m_overrideTimeout;
			}
		}

		// Token: 0x06003AC2 RID: 15042 RVA: 0x00178C94 File Offset: 0x00176E94
		protected override void Update()
		{
			base.Update();
			if (this.m_audioEventTriggerTime != null && this.m_audioEventTriggerTime.Value <= Time.time)
			{
				this.TriggerAudioEvent(this.m_audioEvent);
				this.TriggerAudioEvent(this.m_secondaryAudioEvent);
				this.m_audioEventTriggerTime = null;
			}
		}

		// Token: 0x06003AC3 RID: 15043 RVA: 0x00067D5C File Offset: 0x00065F5C
		public void Initialize(GameEntity entity, float timeout, GameEntity sourceEntity = null)
		{
			this.m_targetEntity = entity;
			this.m_sourceEntity = sourceEntity;
			this.InitializeInternal(new float?(timeout));
		}

		// Token: 0x06003AC4 RID: 15044 RVA: 0x00178CEC File Offset: 0x00176EEC
		public void Initialize(GameEntity entity, GameEntity sourceEntity = null)
		{
			this.m_targetEntity = entity;
			this.m_sourceEntity = sourceEntity;
			this.InitializeInternal(null);
		}

		// Token: 0x06003AC5 RID: 15045 RVA: 0x00178D18 File Offset: 0x00176F18
		private void InitializeInternal(float? timeout)
		{
			Transform parent = null;
			Vector3 pos = this.m_targetEntity ? this.m_targetEntity.gameObject.transform.position : Vector3.zero;
			Quaternion identity = Quaternion.identity;
			if (this.m_overrideTimeout)
			{
				timeout = new float?(this.m_timeoutValue);
			}
			if (timeout != null)
			{
				base.Initialize(parent, pos, identity, timeout.Value);
			}
			else
			{
				base.Initialize(parent, pos, identity);
			}
			if (!this.m_targetEntity)
			{
				return;
			}
			if (this.m_targetEntity == LocalPlayer.GameEntity)
			{
				this.m_volumeFraction = 1f;
			}
			else if (this.m_sourceEntity && this.m_sourceEntity == LocalPlayer.GameEntity)
			{
				this.m_volumeFraction = 1f;
			}
			else
			{
				this.m_volumeFraction = 0.8f;
			}
			GameEntity targetEntity = this.m_targetEntity;
			for (int i = 0; i < this.m_vfxElements.Length; i++)
			{
				if (this.m_vfxElements[i].Vfx)
				{
					Transform transform = base.GetMountPoint(this.m_vfxElements[i].MountPoint, targetEntity, this.m_sourceEntity);
					if (!transform)
					{
						transform = targetEntity.gameObject.transform;
					}
					Vector3 b = Vector3.zero;
					if (this.m_vfxElements[i].MountPoint == VfxMountPoint.DamageTarget && this.m_sourceEntity && this.m_sourceEntity.CharacterData && this.m_sourceEntity.CharacterData.ReferencePoints != null && this.m_sourceEntity.CharacterData.ReferencePoints.Value.DamageTarget)
					{
						Vector3 normalized = (transform.position - this.m_sourceEntity.CharacterData.ReferencePoints.Value.DamageTarget.transform.position).normalized;
						float num = targetEntity.NpcReferencePoints ? targetEntity.NpcReferencePoints.DamageTargetToSourceOffset : GlobalSettings.Values.Uma.DamageTargetToSourceOffset;
						if (targetEntity.CharacterData && targetEntity.CharacterData.TransformScale != null)
						{
							num = num.PercentModification(targetEntity.CharacterData.TransformScale.Value);
						}
						b = normalized * num;
					}
					if (this.m_vfxElements[i].ChildToMountPoint && transform)
					{
						this.m_vfxElements[i].Vfx.transform.SetParent(transform, true);
					}
					Vector3 a = transform ? transform.position : targetEntity.gameObject.transform.position;
					this.m_vfxElements[i].Vfx.transform.position = a - b;
					if (this.m_vfxElements[i].MatchEntityRotation)
					{
						this.m_vfxElements[i].Vfx.transform.rotation = targetEntity.gameObject.transform.rotation;
					}
					else
					{
						this.m_vfxElements[i].Vfx.transform.localRotation = this.m_vfxElements[i].DefaultRotation;
					}
					this.m_vfxElements[i].AdjustDuration();
				}
			}
			if (this.m_audioEventDelay > 0f)
			{
				this.m_audioEventTriggerTime = new float?(Time.time + this.m_audioEventDelay);
				return;
			}
			this.m_audioEventTriggerTime = null;
			this.TriggerAudioEvent(this.m_audioEvent);
			this.TriggerAudioEvent(this.m_secondaryAudioEvent);
		}

		// Token: 0x06003AC6 RID: 15046 RVA: 0x001790CC File Offset: 0x001772CC
		public void InitializeSimple(Transform parent, Vector3 pos, Quaternion rot, float? timeout)
		{
			if (this.m_overrideTimeout)
			{
				timeout = new float?(this.m_timeoutValue);
			}
			if (timeout != null)
			{
				base.Initialize(parent, pos, rot, timeout.Value);
			}
			else
			{
				base.Initialize(parent, pos, rot);
			}
			for (int i = 0; i < this.m_vfxElements.Length; i++)
			{
				if (this.m_vfxElements[i].Vfx)
				{
					this.m_vfxElements[i].Vfx.transform.SetParent(base.gameObject.transform);
					this.m_vfxElements[i].Vfx.transform.localPosition = Vector3.zero;
					this.m_vfxElements[i].Vfx.transform.localRotation = this.m_vfxElements[i].DefaultRotation;
				}
			}
			if (this.m_audioEventDelay > 0f)
			{
				this.m_audioEventTriggerTime = new float?(Time.time + this.m_audioEventDelay);
				return;
			}
			this.m_audioEventTriggerTime = null;
			this.TriggerAudioEvent(this.m_audioEvent);
			this.TriggerAudioEvent(this.m_secondaryAudioEvent);
		}

		// Token: 0x06003AC7 RID: 15047 RVA: 0x001791E8 File Offset: 0x001773E8
		public override void ResetPooledObject()
		{
			bool flag = true;
			for (int i = 0; i < this.m_vfxElements.Length; i++)
			{
				if (!this.m_vfxElements[i].Vfx)
				{
					flag = false;
					break;
				}
				this.m_vfxElements[i].Vfx.transform.SetParent(base.gameObject.transform);
			}
			this.m_audioEventTriggerTime = null;
			this.m_targetEntity = null;
			this.m_sourceEntity = null;
			this.m_volumeFraction = 1f;
			if (flag)
			{
				base.ResetPooledObject();
				return;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06003AC8 RID: 15048 RVA: 0x00179280 File Offset: 0x00177480
		private void TriggerAudioEvent(AudioEvent audioEvent)
		{
			if (audioEvent == null)
			{
				return;
			}
			if (this.m_addMissingAudioSource && !audioEvent.Source)
			{
				if (!this.m_instantiatedAudioSource && GlobalSettings.Values.Audio.VfxAudioSource)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GlobalSettings.Values.Audio.VfxAudioSource, base.gameObject.transform);
					gameObject.transform.localPosition = Vector3.zero;
					this.m_instantiatedAudioSource = gameObject.GetComponent<AudioSource>();
				}
				audioEvent.Source = this.m_instantiatedAudioSource;
			}
			audioEvent.Play(this.m_volumeFraction);
		}

		// Token: 0x04003942 RID: 14658
		private const string kTimeoutGroup = "Timeout";

		// Token: 0x04003943 RID: 14659
		[SerializeField]
		private bool m_overrideTimeout;

		// Token: 0x04003944 RID: 14660
		[SerializeField]
		private float m_timeoutValue = 5f;

		// Token: 0x04003945 RID: 14661
		[SerializeField]
		private PooledVFX.VfxStuff[] m_vfxElements;

		// Token: 0x04003946 RID: 14662
		private const string kAudioGroup = "Audio";

		// Token: 0x04003947 RID: 14663
		private const string kAudioEvent1 = "Audio/Primary";

		// Token: 0x04003948 RID: 14664
		private const string kAudioEvent2 = "Audio/Secondary";

		// Token: 0x04003949 RID: 14665
		[SerializeField]
		private bool m_addMissingAudioSource;

		// Token: 0x0400394A RID: 14666
		[SerializeField]
		private float m_audioEventDelay;

		// Token: 0x0400394B RID: 14667
		[SerializeField]
		private AudioEvent m_audioEvent;

		// Token: 0x0400394C RID: 14668
		[SerializeField]
		private AudioEvent m_secondaryAudioEvent;

		// Token: 0x0400394D RID: 14669
		private float? m_audioEventTriggerTime;

		// Token: 0x0400394E RID: 14670
		private AudioSource m_instantiatedAudioSource;

		// Token: 0x0400394F RID: 14671
		private GameEntity m_targetEntity;

		// Token: 0x04003950 RID: 14672
		private GameEntity m_sourceEntity;

		// Token: 0x04003951 RID: 14673
		private float m_volumeFraction = 1f;

		// Token: 0x020007E3 RID: 2019
		[Serializable]
		private class VfxStuff
		{
			// Token: 0x17000D76 RID: 3446
			// (get) Token: 0x06003ACA RID: 15050 RVA: 0x00179320 File Offset: 0x00177520
			public GameObject Vfx
			{
				get
				{
					if (this.m_instantiatedVfx == null && this.m_prefab)
					{
						this.m_instantiatedVfx = UnityEngine.Object.Instantiate<GameObject>(this.m_prefab);
						if (this.m_adjustDuration)
						{
							this.m_durationAdjuster = this.m_instantiatedVfx.GetComponent<VfxDurationAdjuster>();
						}
						this.DefaultRotation = this.m_instantiatedVfx.gameObject.transform.rotation;
					}
					return this.m_instantiatedVfx;
				}
			}

			// Token: 0x17000D77 RID: 3447
			// (get) Token: 0x06003ACB RID: 15051 RVA: 0x00067D96 File Offset: 0x00065F96
			// (set) Token: 0x06003ACC RID: 15052 RVA: 0x00067D9E File Offset: 0x00065F9E
			public Quaternion DefaultRotation { get; private set; }

			// Token: 0x17000D78 RID: 3448
			// (get) Token: 0x06003ACD RID: 15053 RVA: 0x00067DA7 File Offset: 0x00065FA7
			public bool ChildToMountPoint
			{
				get
				{
					return this.m_childToMountPoint;
				}
			}

			// Token: 0x17000D79 RID: 3449
			// (get) Token: 0x06003ACE RID: 15054 RVA: 0x00067DAF File Offset: 0x00065FAF
			public bool MatchEntityRotation
			{
				get
				{
					return this.m_matchEntityRotation;
				}
			}

			// Token: 0x17000D7A RID: 3450
			// (get) Token: 0x06003ACF RID: 15055 RVA: 0x00067DB7 File Offset: 0x00065FB7
			public VfxMountPoint MountPoint
			{
				get
				{
					return this.m_mountPoint;
				}
			}

			// Token: 0x06003AD0 RID: 15056 RVA: 0x00067DBF File Offset: 0x00065FBF
			public void AdjustDuration()
			{
				if (this.m_adjustDuration && this.m_durationAdjuster)
				{
					this.m_durationAdjuster.Init(this.m_durationMultiplier);
				}
			}

			// Token: 0x04003952 RID: 14674
			[SerializeField]
			private bool m_childToMountPoint;

			// Token: 0x04003953 RID: 14675
			[SerializeField]
			private bool m_matchEntityRotation;

			// Token: 0x04003954 RID: 14676
			[SerializeField]
			private VfxMountPoint m_mountPoint;

			// Token: 0x04003955 RID: 14677
			[SerializeField]
			private GameObject m_prefab;

			// Token: 0x04003956 RID: 14678
			[SerializeField]
			private bool m_adjustDuration;

			// Token: 0x04003957 RID: 14679
			[SerializeField]
			private float m_durationMultiplier = 1f;

			// Token: 0x04003958 RID: 14680
			private VfxDurationAdjuster m_durationAdjuster;

			// Token: 0x04003959 RID: 14681
			private GameObject m_instantiatedVfx;
		}
	}
}
