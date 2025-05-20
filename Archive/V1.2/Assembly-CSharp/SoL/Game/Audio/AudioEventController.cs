using System;
using System.Collections.Generic;
using SoL.Game.Culling;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000CFF RID: 3327
	public class AudioEventController : GameEntityComponent
	{
		// Token: 0x17001829 RID: 6185
		// (get) Token: 0x06006496 RID: 25750 RVA: 0x00083A3D File Offset: 0x00081C3D
		internal bool AddAshenFilters
		{
			get
			{
				return this.m_addAshenFilters;
			}
		}

		// Token: 0x1700182A RID: 6186
		// (get) Token: 0x06006497 RID: 25751 RVA: 0x00083A45 File Offset: 0x00081C45
		private bool m_showImpulseForce
		{
			get
			{
				return this.m_impulseFlags > AudioImpulseFlags.None;
			}
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x00083A50 File Offset: 0x00081C50
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.AudioEventController = this;
			}
		}

		// Token: 0x06006499 RID: 25753 RVA: 0x0020A368 File Offset: 0x00208568
		private void Start()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
				this.CurrentHealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState.Value);
			}
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x0020A3D4 File Offset: 0x002085D4
		private void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
			if (this.m_events != null)
			{
				StaticDictionaryPool<string, IAudioEvent>.ReturnToPool(this.m_events);
			}
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x00083A6C File Offset: 0x00081C6C
		public void RegisterEvent(IAudioEvent audioEvent)
		{
			if (audioEvent != null && !string.IsNullOrEmpty(audioEvent.EventName))
			{
				if (this.m_events == null)
				{
					this.m_events = StaticDictionaryPool<string, IAudioEvent, StringExtensions.StringComparerInvariantCultureIgnoreCase>.GetFromPool();
				}
				this.m_events.AddOrReplace(audioEvent.EventName, audioEvent);
			}
		}

		// Token: 0x0600649C RID: 25756 RVA: 0x00083AA3 File Offset: 0x00081CA3
		public void UnregisterEvent(IAudioEvent audioEvent)
		{
			if (this.m_events != null && audioEvent != null && !string.IsNullOrEmpty(audioEvent.EventName))
			{
				this.m_events.Remove(audioEvent.EventName);
			}
		}

		// Token: 0x0600649D RID: 25757 RVA: 0x00083ACF File Offset: 0x00081CCF
		public void RegisterAudioSource(AudioSource source)
		{
			this.m_audioSources.Add(source);
		}

		// Token: 0x0600649E RID: 25758 RVA: 0x00083ADE File Offset: 0x00081CDE
		public void PlayAudioEvent(string eventName)
		{
			this.PlayAudioEvent(eventName, 1f);
		}

		// Token: 0x0600649F RID: 25759 RVA: 0x0020A438 File Offset: 0x00208638
		public void PlayAudioEvent(string eventName, float volumeFraction)
		{
			if (this.m_events == null)
			{
				return;
			}
			if (this.m_isDead && !AudioEventController.m_permittedDeathAudio.Contains(eventName))
			{
				return;
			}
			IAudioEvent audioEvent;
			if (this.m_events.TryGetValue(eventName, out audioEvent))
			{
				IAudioEvent audioEvent2;
				if (eventName.Equals("Block", StringComparison.InvariantCultureIgnoreCase) && this.m_events.TryGetValue("ShieldBlock", out audioEvent2))
				{
					audioEvent = audioEvent2;
				}
				if (audioEvent != null)
				{
					audioEvent.Play(volumeFraction);
					if (audioEvent.ImpulseFlags != AudioImpulseFlags.None && this.m_impulseFlags.HasBitFlag(audioEvent.ImpulseFlags) && AudioImpulseSources.Instance)
					{
						if (this.m_impulseForceInternal == null)
						{
							this.m_impulseForceInternal = new float?((base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.TransformScale != null) ? this.m_impulseForce.PercentModification(base.GameEntity.CharacterData.TransformScale.Value) : this.m_impulseForce);
						}
						AudioImpulseSources.Instance.TriggerImpulseAtPosition(audioEvent.ImpulseFlags, audioEvent.ImpulseForce * this.m_impulseForceInternal.Value, base.gameObject.transform.position);
					}
				}
			}
		}

		// Token: 0x060064A0 RID: 25760 RVA: 0x00083AEC File Offset: 0x00081CEC
		public void PlaySound(string eventName)
		{
			this.PlayAudioEvent(eventName);
		}

		// Token: 0x060064A1 RID: 25761 RVA: 0x00083AF5 File Offset: 0x00081CF5
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious || obj == HealthState.Dead)
			{
				this.m_isDead = true;
				return;
			}
			this.m_isDead = false;
		}

		// Token: 0x060064A2 RID: 25762 RVA: 0x0020A588 File Offset: 0x00208788
		public void SetCullingDistance(CullingDistance cullingDistance)
		{
			this.m_cullingFlags = (((base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal) || cullingDistance.GetDistance() <= this.m_cullingDistance.GetDistance()) ? this.m_cullingFlags.UnsetBitFlag(CullingFlags.Distance) : this.m_cullingFlags.SetBitFlag(CullingFlags.Distance));
			this.RefreshAudioSources();
		}

		// Token: 0x060064A3 RID: 25763 RVA: 0x0020A60C File Offset: 0x0020880C
		private void RefreshAudioSources()
		{
			bool enabled = true;
			foreach (AudioSource audioSource in this.m_audioSources)
			{
				if (audioSource != null)
				{
					audioSource.enabled = enabled;
				}
			}
		}

		// Token: 0x04005758 RID: 22360
		private static readonly HashSet<string> m_permittedDeathAudio = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
		{
			"Death",
			"BodyFall"
		};

		// Token: 0x04005759 RID: 22361
		[SerializeField]
		private CullingDistance m_cullingDistance = CullingDistance.Near;

		// Token: 0x0400575A RID: 22362
		[SerializeField]
		private AudioImpulseFlags m_impulseFlags;

		// Token: 0x0400575B RID: 22363
		[SerializeField]
		private float m_impulseForce = 1f;

		// Token: 0x0400575C RID: 22364
		[SerializeField]
		private bool m_addAshenFilters;

		// Token: 0x0400575D RID: 22365
		private float? m_impulseForceInternal;

		// Token: 0x0400575E RID: 22366
		private readonly HashSet<AudioSource> m_audioSources = new HashSet<AudioSource>();

		// Token: 0x0400575F RID: 22367
		private CullingFlags m_cullingFlags;

		// Token: 0x04005760 RID: 22368
		private Dictionary<string, IAudioEvent> m_events;

		// Token: 0x04005761 RID: 22369
		private bool m_isDead;
	}
}
