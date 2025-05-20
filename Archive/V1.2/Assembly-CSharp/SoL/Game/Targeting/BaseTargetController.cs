using System;
using SoL.Game.EffectSystem;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.Replication;

namespace SoL.Game.Targeting
{
	// Token: 0x0200064F RID: 1615
	public class BaseTargetController : SyncVarReplicator
	{
		// Token: 0x140000A9 RID: 169
		// (add) Token: 0x0600323E RID: 12862 RVA: 0x0015F7C8 File Offset: 0x0015D9C8
		// (remove) Token: 0x0600323F RID: 12863 RVA: 0x0015F800 File Offset: 0x0015DA00
		public event Action<GameEntity> OffensiveTargetChanged;

		// Token: 0x140000AA RID: 170
		// (add) Token: 0x06003240 RID: 12864 RVA: 0x0015F838 File Offset: 0x0015DA38
		// (remove) Token: 0x06003241 RID: 12865 RVA: 0x0015F870 File Offset: 0x0015DA70
		public event Action<GameEntity> DefensiveTargetChanged;

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06003242 RID: 12866 RVA: 0x00062BAB File Offset: 0x00060DAB
		// (set) Token: 0x06003243 RID: 12867 RVA: 0x00062BB3 File Offset: 0x00060DB3
		public int? NTargets { get; protected set; }

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06003244 RID: 12868 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsLulled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06003245 RID: 12869 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IgnoreGuards
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06003246 RID: 12870 RVA: 0x00062BBC File Offset: 0x00060DBC
		// (set) Token: 0x06003247 RID: 12871 RVA: 0x0015F8A8 File Offset: 0x0015DAA8
		public GameEntity OffensiveTarget
		{
			get
			{
				return this.m_offensiveTarget;
			}
			set
			{
				this.m_offensiveTarget = value;
				if (this.m_netEntity.IsLocal)
				{
					if (this.m_offensiveTarget == null)
					{
						this.m_offensiveTargetNetworkId.Value = 0U;
					}
					else
					{
						this.m_offensiveTargetNetworkId.Value = this.m_offensiveTarget.NetworkEntity.NetworkId.Value;
					}
				}
				Action<GameEntity> offensiveTargetChanged = this.OffensiveTargetChanged;
				if (offensiveTargetChanged != null)
				{
					offensiveTargetChanged(this.m_offensiveTarget);
				}
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity == base.GameEntity)
				{
					LocalPlayer.TriggerOffensiveTargetChanged(this.m_offensiveTarget);
				}
			}
		}

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06003248 RID: 12872 RVA: 0x00062BC4 File Offset: 0x00060DC4
		// (set) Token: 0x06003249 RID: 12873 RVA: 0x0015F948 File Offset: 0x0015DB48
		public GameEntity DefensiveTarget
		{
			get
			{
				return this.m_defensiveTarget;
			}
			set
			{
				this.m_defensiveTarget = value;
				if (this.m_netEntity.IsLocal)
				{
					if (this.m_defensiveTarget == null)
					{
						this.m_defensiveTargetNetworkId.Value = 0U;
					}
					else
					{
						this.m_defensiveTargetNetworkId.Value = this.m_defensiveTarget.NetworkEntity.NetworkId.Value;
					}
				}
				Action<GameEntity> defensiveTargetChanged = this.DefensiveTargetChanged;
				if (defensiveTargetChanged != null)
				{
					defensiveTargetChanged(this.m_defensiveTarget);
				}
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity == base.GameEntity)
				{
					LocalPlayer.TriggerDefensiveTargetChanged(this.m_defensiveTarget);
				}
			}
		}

		// Token: 0x0600324A RID: 12874 RVA: 0x00062BCC File Offset: 0x00060DCC
		private void Awake()
		{
			base.GameEntity.TargetController = this;
			this.m_offensiveTargetNetworkId.PermitClientToModify(this);
			this.m_defensiveTargetNetworkId.PermitClientToModify(this);
		}

		// Token: 0x0600324B RID: 12875 RVA: 0x0015F9E8 File Offset: 0x0015DBE8
		protected override void OnDestroy()
		{
			if (this.m_netEntity != null && !this.m_netEntity.IsLocal)
			{
				this.m_offensiveTargetNetworkId.Changed -= this.OffensiveTargetNetworkIdOnChanged;
				this.m_defensiveTargetNetworkId.Changed -= this.DefensiveTargetNetworkIdOnChanged;
			}
			base.OnDestroy();
		}

		// Token: 0x0600324C RID: 12876 RVA: 0x0015FA44 File Offset: 0x0015DC44
		protected override void PostInit()
		{
			if (!this.m_netEntity.IsLocal)
			{
				this.m_offensiveTargetNetworkId.Changed += this.OffensiveTargetNetworkIdOnChanged;
				this.m_defensiveTargetNetworkId.Changed += this.DefensiveTargetNetworkIdOnChanged;
				this.OffensiveTargetNetworkIdOnChanged(this.m_offensiveTargetNetworkId);
				this.DefensiveTargetNetworkIdOnChanged(this.m_defensiveTargetNetworkId);
			}
		}

		// Token: 0x0600324D RID: 12877 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Initialize()
		{
		}

		// Token: 0x0600324E RID: 12878 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AssignNameplate(TargetType type, NameplateControllerUI nameplate)
		{
		}

		// Token: 0x0600324F RID: 12879 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool EscapePressed()
		{
			return false;
		}

		// Token: 0x06003250 RID: 12880 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void SetTarget(TargetType type, ITargetable targetable)
		{
		}

		// Token: 0x06003251 RID: 12881 RVA: 0x0015FAB0 File Offset: 0x0015DCB0
		private void OffensiveTargetNetworkIdOnChanged(uint obj)
		{
			if (this.m_netEntity.IsLocal)
			{
				return;
			}
			NetworkEntity networkEntity;
			if (obj != 0U && NetworkManager.EntityManager.TryGetNetworkEntity(obj, out networkEntity))
			{
				this.OffensiveTarget = networkEntity.GameEntity;
				return;
			}
			this.OffensiveTarget = null;
		}

		// Token: 0x06003252 RID: 12882 RVA: 0x0015FAF4 File Offset: 0x0015DCF4
		private void DefensiveTargetNetworkIdOnChanged(uint obj)
		{
			if (this.m_netEntity.IsLocal)
			{
				return;
			}
			NetworkEntity networkEntity;
			if (obj != 0U && NetworkManager.EntityManager.TryGetNetworkEntity(obj, out networkEntity))
			{
				this.DefensiveTarget = networkEntity.GameEntity;
				return;
			}
			this.DefensiveTarget = null;
		}

		// Token: 0x140000AB RID: 171
		// (add) Token: 0x06003253 RID: 12883 RVA: 0x0015FB38 File Offset: 0x0015DD38
		// (remove) Token: 0x06003254 RID: 12884 RVA: 0x0015FB70 File Offset: 0x0015DD70
		public event Action<NetworkEntity, EffectProcessingResult> ThreatReceived;

		// Token: 0x06003255 RID: 12885 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EntityDied()
		{
		}

		// Token: 0x06003256 RID: 12886 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void InitializeAsAshen(BaseTargetController otherTargetController)
		{
		}

		// Token: 0x06003257 RID: 12887 RVA: 0x00062BF2 File Offset: 0x00060DF2
		public virtual float GetTopThreatValue(GameEntity source, out float sourceThreat)
		{
			sourceThreat = 0f;
			return 0f;
		}

		// Token: 0x06003258 RID: 12888 RVA: 0x00062C00 File Offset: 0x00060E00
		public virtual bool IsTopThreat(GameEntity source, out float sourceThreat, out float? nextLowerThreat)
		{
			sourceThreat = 0f;
			nextLowerThreat = null;
			return false;
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x0015FBA8 File Offset: 0x0015DDA8
		public virtual void AddThreat(GameEntity source, float threat, float dmg, bool addAsTagger)
		{
			EffectProcessingResult arg = new EffectProcessingResult
			{
				DamageDone = dmg,
				Threat = threat
			};
			Action<NetworkEntity, EffectProcessingResult> threatReceived = this.ThreatReceived;
			if (threatReceived == null)
			{
				return;
			}
			threatReceived(source.NetworkEntity, arg);
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsHostileTo(NetworkEntity sourceEntity, NetworkEntity alternateSourceEntity = null)
		{
			return false;
		}

		// Token: 0x0600325B RID: 12891 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool InTargetList(NetworkEntity sourceEntity)
		{
			return false;
		}

		// Token: 0x0600325C RID: 12892 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IsCurrentHostileTarget(NetworkEntity sourceEntity)
		{
			return false;
		}

		// Token: 0x0600325D RID: 12893 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void BehaviorFlagsUpdated(GameEntity source, DateTime expiration, BehaviorEffectTypeFlags flags, bool adding)
		{
		}

		// Token: 0x0600325E RID: 12894 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ReplacementNetworkEntitySpawned(uint isReplacingId, NetworkEntity incomingReplacement)
		{
		}

		// Token: 0x0600325F RID: 12895 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ClearBehaviorReferences()
		{
		}

		// Token: 0x06003260 RID: 12896 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ConsiderTarget()
		{
		}

		// Token: 0x06003261 RID: 12897 RVA: 0x0015FBE8 File Offset: 0x0015DDE8
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_defensiveTargetNetworkId);
			this.m_defensiveTargetNetworkId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_offensiveTargetNetworkId);
			this.m_offensiveTargetNetworkId.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x040030D2 RID: 12498
		private GameEntity m_offensiveTarget;

		// Token: 0x040030D3 RID: 12499
		private GameEntity m_defensiveTarget;

		// Token: 0x040030D4 RID: 12500
		protected SynchronizedUInt m_offensiveTargetNetworkId = new SynchronizedUInt(0U);

		// Token: 0x040030D5 RID: 12501
		protected SynchronizedUInt m_defensiveTargetNetworkId = new SynchronizedUInt(0U);
	}
}
