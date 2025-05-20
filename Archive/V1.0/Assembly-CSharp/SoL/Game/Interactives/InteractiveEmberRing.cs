using System;
using SoL.Game.Culling;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B88 RID: 2952
	public class InteractiveEmberRing : SyncVarReplicator
	{
		// Token: 0x06005B03 RID: 23299 RVA: 0x0007D265 File Offset: 0x0007B465
		private void Awake()
		{
			if (this.m_applicator)
			{
				this.m_applicator.Interactive = this;
			}
		}

		// Token: 0x06005B04 RID: 23300 RVA: 0x001EDD84 File Offset: 0x001EBF84
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				this.Data.Changed += this.DataOnChanged;
				this.RefreshEnhancementVisuals();
				if (this.m_subscriberParticleSystem)
				{
					this.m_subscriberCulledParticleSystem = this.m_subscriberParticleSystem.GetComponent<CulledParticleSystem>();
					Color subscriberColor = GlobalSettings.Values.Subscribers.SubscriberColor;
					ParticleSystem[] componentsInChildren = this.m_subscriberParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						ParticleSystem.MainModule main = componentsInChildren[i].main;
						subscriberColor.a = main.startColor.color.a;
						main.startColor = subscriberColor;
					}
				}
				this.SubscriberPresent.Changed += this.SubscriberPresentOnChanged;
				this.RefreshSubscriberVisuals();
			}
		}

		// Token: 0x06005B05 RID: 23301 RVA: 0x0007D280 File Offset: 0x0007B480
		protected override void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.Data.Changed -= this.DataOnChanged;
				this.SubscriberPresent.Changed -= this.SubscriberPresentOnChanged;
			}
			base.OnDestroy();
		}

		// Token: 0x06005B06 RID: 23302 RVA: 0x001EDE54 File Offset: 0x001EC054
		public bool CanEnhanceWithItem(GameEntity source, ConsumableItemEmberRingEnhancment enhancementItem, out string failureMessage)
		{
			failureMessage = "Unknown";
			if (source == null || enhancementItem == null)
			{
				return false;
			}
			if (this.Data.Value.Item != null && this.Data.Value.Item.Level > enhancementItem.Level)
			{
				failureMessage = "More powerful enhancement already applied!";
				return false;
			}
			return true;
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x0007D2BD File Offset: 0x0007B4BD
		public void EnhanceEmberRing(GameEntity source, ConsumableItemEmberRingEnhancment enhancementItem)
		{
			this.Data.Value = new EmberRingEnhancementData(source, enhancementItem);
			this.m_applicator.RefreshInteractiveState();
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x0007D2DC File Offset: 0x0007B4DC
		private void DataOnChanged(EmberRingEnhancementData obj)
		{
			this.RefreshEnhancementVisuals();
		}

		// Token: 0x06005B09 RID: 23305 RVA: 0x001EDEC4 File Offset: 0x001EC0C4
		private void RefreshEnhancementVisuals()
		{
			if (!GameManager.IsServer && this.m_enhancedParticleSystem)
			{
				if (this.Data.Value.Item != null)
				{
					this.m_enhancedParticleSystem.main.startColor = this.Data.Value.Item.Color;
					this.m_enhancedParticleSystem.gameObject.SetActive(true);
					return;
				}
				this.m_enhancedParticleSystem.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x0007D2E4 File Offset: 0x0007B4E4
		private void SubscriberPresentOnChanged(bool obj)
		{
			this.RefreshSubscriberVisuals();
			this.RefreshEventVisuals();
		}

		// Token: 0x06005B0B RID: 23307 RVA: 0x001EDF54 File Offset: 0x001EC154
		private void RefreshSubscriberVisuals()
		{
			if (!GameManager.IsServer && this.m_subscriberParticleSystem)
			{
				if (!this.m_subscriberParticleSystem.gameObject.activeSelf)
				{
					this.m_subscriberParticleSystem.Stop();
					this.m_subscriberParticleSystem.gameObject.SetActive(true);
				}
				if (this.SubscriberPresent.Value)
				{
					if (this.m_subscriberCulledParticleSystem)
					{
						this.m_subscriberCulledParticleSystem.ExternallyStopped = false;
						return;
					}
					this.m_subscriberParticleSystem.Play();
					return;
				}
				else
				{
					if (this.m_subscriberCulledParticleSystem)
					{
						this.m_subscriberCulledParticleSystem.ExternallyStopped = true;
						return;
					}
					this.m_subscriberParticleSystem.Stop();
				}
			}
		}

		// Token: 0x06005B0C RID: 23308 RVA: 0x0007D2F2 File Offset: 0x0007B4F2
		private void RefreshEventVisuals()
		{
			if (!GameManager.IsServer && this.m_eventVfx && this.SubscriberPresent.Value)
			{
				this.m_eventVfx.GetPooledInstance<PooledVFX>().Initialize(base.GameEntity, 10f, null);
			}
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x001EE004 File Offset: 0x001EC204
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.Data);
			this.Data.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.SubscriberPresent);
			this.SubscriberPresent.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04004FA5 RID: 20389
		[SerializeField]
		private CampfireEffectApplicator m_applicator;

		// Token: 0x04004FA6 RID: 20390
		[SerializeField]
		private ParticleSystem m_enhancedParticleSystem;

		// Token: 0x04004FA7 RID: 20391
		[SerializeField]
		private ParticleSystem m_subscriberParticleSystem;

		// Token: 0x04004FA8 RID: 20392
		[SerializeField]
		private PooledVFX m_eventVfx;

		// Token: 0x04004FA9 RID: 20393
		public readonly SynchronizedStruct<EmberRingEnhancementData> Data = new SynchronizedStruct<EmberRingEnhancementData>();

		// Token: 0x04004FAA RID: 20394
		public readonly SynchronizedBool SubscriberPresent = new SynchronizedBool();

		// Token: 0x04004FAB RID: 20395
		private CulledParticleSystem m_subscriberCulledParticleSystem;
	}
}
