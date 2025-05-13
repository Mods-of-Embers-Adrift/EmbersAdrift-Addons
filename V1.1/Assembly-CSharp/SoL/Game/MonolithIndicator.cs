using System;
using SoL.Game.Audio;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000554 RID: 1364
	public class MonolithIndicator : SyncVarReplicator
	{
		// Token: 0x06002974 RID: 10612 RVA: 0x001418AC File Offset: 0x0013FAAC
		private void Start()
		{
			if (!GameManager.IsServer && base.GameEntity && base.GameEntity.NetworkEntity)
			{
				if (base.GameEntity.NetworkEntity.IsInitialized)
				{
					this.Initialize();
					return;
				}
				base.GameEntity.NetworkEntity.OnStartClient += this.NetworkEntityOnOnStartClient;
			}
		}

		// Token: 0x06002975 RID: 10613 RVA: 0x0005CA85 File Offset: 0x0005AC85
		protected override void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.m_location.Changed -= this.LocationOnChanged;
			}
			base.OnDestroy();
		}

		// Token: 0x06002976 RID: 10614 RVA: 0x0005CAAB File Offset: 0x0005ACAB
		private void NetworkEntityOnOnStartClient()
		{
			base.GameEntity.NetworkEntity.OnStartClient -= this.NetworkEntityOnOnStartClient;
			this.Initialize();
		}

		// Token: 0x06002977 RID: 10615 RVA: 0x0005CACF File Offset: 0x0005ACCF
		private void Initialize()
		{
			this.m_location.Changed += this.LocationOnChanged;
			this.LocationOnChanged(this.m_location.Value);
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x0005CAF9 File Offset: 0x0005ACF9
		private void StateOnChanged(byte obj)
		{
			bool isServer = GameManager.IsServer;
		}

		// Token: 0x06002979 RID: 10617 RVA: 0x00141914 File Offset: 0x0013FB14
		private void LocationOnChanged(CharacterLocation obj)
		{
			if (!GameManager.IsServer && this.m_location.Value != null)
			{
				if (this.m_useLocalRotation)
				{
					base.GameEntity.transform.position = this.m_location.Value.GetPosition();
					Vector3 eulerAngles = base.GameEntity.transform.localRotation.eulerAngles;
					base.GameEntity.transform.localRotation = Quaternion.Euler(new Vector3(eulerAngles.x, this.m_location.Value.h, eulerAngles.z));
				}
				else
				{
					base.gameObject.transform.SetPositionAndRotation(this.m_location.Value.GetPosition(), this.m_location.Value.GetRotation());
				}
				AudioEvent onChangeAudioEvent = this.m_onChangeAudioEvent;
				if (onChangeAudioEvent == null)
				{
					return;
				}
				onChangeAudioEvent.Play(1f);
			}
		}

		// Token: 0x0600297A RID: 10618 RVA: 0x001419FC File Offset: 0x0013FBFC
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_location);
			this.m_location.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002A66 RID: 10854
		private readonly SynchronizedLocation m_location = new SynchronizedLocation();

		// Token: 0x04002A67 RID: 10855
		[SerializeField]
		private bool m_useLocalRotation;

		// Token: 0x04002A68 RID: 10856
		[SerializeField]
		private GameObject[] m_points;

		// Token: 0x04002A69 RID: 10857
		[SerializeField]
		private ZoneSettingsProfile m_profileOverride;

		// Token: 0x04002A6A RID: 10858
		[SerializeField]
		private AudioEvent m_onChangeAudioEvent;
	}
}
