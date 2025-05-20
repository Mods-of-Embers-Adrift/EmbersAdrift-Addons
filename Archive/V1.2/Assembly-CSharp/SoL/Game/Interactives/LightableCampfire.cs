using System;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BB5 RID: 2997
	public class LightableCampfire : SyncVarReplicator
	{
		// Token: 0x170015F2 RID: 5618
		// (get) Token: 0x06005CE9 RID: 23785 RVA: 0x0007E611 File Offset: 0x0007C811
		public bool IsLit
		{
			get
			{
				return this.CampfireData.Value.IsLit;
			}
		}

		// Token: 0x170015F3 RID: 5619
		// (get) Token: 0x06005CEA RID: 23786 RVA: 0x0007E623 File Offset: 0x0007C823
		public DateTime ExpirationTimestamp
		{
			get
			{
				return this.m_expirationTimestamp;
			}
		}

		// Token: 0x06005CEB RID: 23787 RVA: 0x0007E62B File Offset: 0x0007C82B
		private void Awake()
		{
			if (this.m_applicator == null)
			{
				base.enabled = false;
				return;
			}
			this.m_applicator.Lightable = this;
		}

		// Token: 0x06005CEC RID: 23788 RVA: 0x0007E64F File Offset: 0x0007C84F
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				this.CampfireDataOnChanged(this.CampfireData);
				this.CampfireData.Changed += this.CampfireDataOnChanged;
				base.enabled = false;
			}
		}

		// Token: 0x06005CED RID: 23789 RVA: 0x0007E687 File Offset: 0x0007C887
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!GameManager.IsServer)
			{
				this.CampfireData.Changed -= this.CampfireDataOnChanged;
			}
		}

		// Token: 0x06005CEE RID: 23790 RVA: 0x001F2460 File Offset: 0x001F0660
		private void Update()
		{
			if (!GameManager.IsServer || !this.CampfireData.Value.IsLit)
			{
				return;
			}
			if (DateTime.UtcNow >= this.m_expirationTimestamp)
			{
				this.CampfireData.Value = new LightableCampfireData
				{
					IsLit = false,
					MasteryLevel = 0,
					Timestamp = DateTime.MinValue,
					LighterName = ""
				};
				this.m_applicator.ExtinguishFire();
			}
		}

		// Token: 0x06005CEF RID: 23791 RVA: 0x001F24E0 File Offset: 0x001F06E0
		private void CampfireDataOnChanged(LightableCampfireData obj)
		{
			if (obj.IsLit)
			{
				float num = this.m_applicator.LightFire(obj.MasteryLevel);
				this.m_expirationTimestamp = obj.Timestamp.AddSeconds((double)num);
				return;
			}
			this.m_applicator.ExtinguishFire();
		}

		// Token: 0x06005CF0 RID: 23792 RVA: 0x001F2528 File Offset: 0x001F0728
		public float LightFire(GameEntity starter, int masteryLevel)
		{
			SynchronizedVariable<LightableCampfireData> campfireData = this.CampfireData;
			LightableCampfireData value = new LightableCampfireData
			{
				IsLit = true,
				MasteryLevel = masteryLevel,
				Timestamp = DateTime.UtcNow,
				LighterName = starter.CharacterData.Name.Value
			};
			campfireData.Value = value;
			float num = this.m_applicator.LightFire(masteryLevel);
			value = this.CampfireData.Value;
			this.m_expirationTimestamp = value.Timestamp.AddSeconds((double)num);
			return num;
		}

		// Token: 0x06005CF1 RID: 23793 RVA: 0x001F25AC File Offset: 0x001F07AC
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.CampfireData);
			this.CampfireData.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04005058 RID: 20568
		[SerializeField]
		private CampfireEffectApplicator m_applicator;

		// Token: 0x04005059 RID: 20569
		private DateTime m_expirationTimestamp = DateTime.MinValue;

		// Token: 0x0400505A RID: 20570
		public readonly SynchronizedStruct<LightableCampfireData> CampfireData = new SynchronizedStruct<LightableCampfireData>();
	}
}
