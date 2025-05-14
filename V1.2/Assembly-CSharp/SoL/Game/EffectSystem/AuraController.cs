using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C32 RID: 3122
	public class AuraController : IPoolable
	{
		// Token: 0x17001720 RID: 5920
		// (get) Token: 0x06006048 RID: 24648 RVA: 0x00080CCC File Offset: 0x0007EECC
		// (set) Token: 0x06006049 RID: 24649 RVA: 0x00080CD4 File Offset: 0x0007EED4
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x0600604A RID: 24650 RVA: 0x00080CDD File Offset: 0x0007EEDD
		void IPoolable.Reset()
		{
			this.ResetForPool();
		}

		// Token: 0x0600604B RID: 24651 RVA: 0x00080CE5 File Offset: 0x0007EEE5
		private void ResetForPool()
		{
			this.RemoveFromAllEntities();
			List<GameEntity> currentEntities = this.m_currentEntities;
			if (currentEntities != null)
			{
				currentEntities.Clear();
			}
			this.m_sourceEntity = null;
			this.m_auraAbility = null;
			this.m_syncData = null;
			this.m_paused = false;
		}

		// Token: 0x17001721 RID: 5921
		// (get) Token: 0x0600604C RID: 24652 RVA: 0x00080D1F File Offset: 0x0007EF1F
		private List<GameEntity> CurrentEntities
		{
			get
			{
				if (this.m_currentEntities == null)
				{
					this.m_currentEntities = new List<GameEntity>(1);
				}
				return this.m_currentEntities;
			}
		}

		// Token: 0x17001722 RID: 5922
		// (get) Token: 0x0600604D RID: 24653 RVA: 0x00080D3B File Offset: 0x0007EF3B
		public GameEntity SourceEntity
		{
			get
			{
				return this.m_sourceEntity;
			}
		}

		// Token: 0x17001723 RID: 5923
		// (get) Token: 0x0600604E RID: 24654 RVA: 0x00080D43 File Offset: 0x0007EF43
		public AuraAbility AuraAbility
		{
			get
			{
				return this.m_auraAbility;
			}
		}

		// Token: 0x17001724 RID: 5924
		// (get) Token: 0x0600604F RID: 24655 RVA: 0x00080D4B File Offset: 0x0007EF4B
		public UniqueId AuraInstanceId
		{
			get
			{
				if (this.m_syncData == null)
				{
					return UniqueId.Empty;
				}
				return this.m_syncData.Value.InstanceId;
			}
		}

		// Token: 0x17001725 RID: 5925
		// (get) Token: 0x06006050 RID: 24656 RVA: 0x00080D70 File Offset: 0x0007EF70
		public EffectSyncData? SyncData
		{
			get
			{
				return this.m_syncData;
			}
		}

		// Token: 0x17001726 RID: 5926
		// (get) Token: 0x06006051 RID: 24657 RVA: 0x001FC418 File Offset: 0x001FA618
		public MinMaxIntRange LevelRange
		{
			get
			{
				if (!(this.m_auraAbility != null))
				{
					return default(MinMaxIntRange);
				}
				return this.m_auraAbility.LevelRange;
			}
		}

		// Token: 0x17001727 RID: 5927
		// (get) Token: 0x06006052 RID: 24658 RVA: 0x00080D78 File Offset: 0x0007EF78
		// (set) Token: 0x06006053 RID: 24659 RVA: 0x00080D80 File Offset: 0x0007EF80
		public bool Paused
		{
			get
			{
				return this.m_paused;
			}
			set
			{
				if (this.m_paused == value)
				{
					return;
				}
				this.m_paused = value;
				if (this.m_paused)
				{
					this.RemoveFromAllEntities();
					return;
				}
				this.AddToEntity(this.m_sourceEntity);
				this.ExternalUpdate();
			}
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x001FC448 File Offset: 0x001FA648
		public void InitAura(GameEntity source, AuraAbility auraAbility, int masteryLevel)
		{
			this.m_sourceEntity = source;
			this.m_auraAbility = auraAbility;
			this.m_syncData = new EffectSyncData?(new EffectSyncData
			{
				InstanceId = UniqueId.GenerateFromGuid(),
				ArchetypeId = this.m_auraAbility.Id,
				ApplicatorName = this.m_sourceEntity.CharacterData.Name.Value,
				SourceNetworkId = this.m_sourceEntity.NetworkEntity.NetworkId.Value,
				Level = (byte)masteryLevel,
				Duration = int.MaxValue,
				ExpirationTime = DateTime.MaxValue,
				Dismissible = false,
				IsSecondary = false
			});
			this.AddToEntity(this.m_sourceEntity);
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x00080DB4 File Offset: 0x0007EFB4
		public void CancelAura()
		{
			this.RemoveFromAllEntities();
			this.ResetForPool();
		}

		// Token: 0x06006056 RID: 24662 RVA: 0x00080DC2 File Offset: 0x0007EFC2
		private void AddToEntity(GameEntity entity)
		{
			if (entity && entity.EffectController)
			{
				entity.EffectController.AddAppliedAura(this);
				this.CurrentEntities.Add(entity);
			}
		}

		// Token: 0x06006057 RID: 24663 RVA: 0x001FC510 File Offset: 0x001FA710
		private void RemoveFromAllEntities()
		{
			for (int i = 0; i < this.CurrentEntities.Count; i++)
			{
				if (this.CurrentEntities[i] && this.CurrentEntities[i].EffectController)
				{
					this.CurrentEntities[i].EffectController.RemoveAppliedAura(this);
					this.CurrentEntities.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06006058 RID: 24664 RVA: 0x001FC588 File Offset: 0x001FA788
		public void ExternalUpdate()
		{
			if (!this.m_sourceEntity || !this.m_sourceEntity.CharacterData || this.m_currentEntities == null)
			{
				return;
			}
			bool flag = !this.m_sourceEntity.CharacterData.GroupId.IsEmpty;
			for (int i = 0; i < this.CurrentEntities.Count; i++)
			{
				if (!(this.CurrentEntities[i] == this.m_sourceEntity) && (!flag || !this.m_sourceEntity.CharacterData.NearbyGroupMembers.Contains(this.CurrentEntities[i])))
				{
					if (this.CurrentEntities[i] && this.CurrentEntities[i].EffectController)
					{
						this.CurrentEntities[i].EffectController.RemoveAppliedAura(this);
					}
					this.CurrentEntities.RemoveAt(i);
					i--;
				}
			}
			this.CurrentEntities.Clear();
			this.CurrentEntities.Add(this.m_sourceEntity);
			if (flag)
			{
				for (int j = 0; j < this.m_sourceEntity.CharacterData.NearbyGroupMembers.Count; j++)
				{
					this.AddToEntity(this.m_sourceEntity.CharacterData.NearbyGroupMembers[j]);
				}
			}
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x00080DF1 File Offset: 0x0007EFF1
		public bool ShouldChangeStateForStance(Stance obj)
		{
			if (obj == Stance.Looting)
			{
				return false;
			}
			if (!this.m_auraAbility)
			{
				return false;
			}
			if (!this.m_auraAbility.IsCombatAura)
			{
				return obj == Stance.Combat;
			}
			return obj != Stance.Combat;
		}

		// Token: 0x0600605A RID: 24666 RVA: 0x00080E21 File Offset: 0x0007F021
		public bool IsValidForStance(Stance obj)
		{
			if (obj == Stance.Looting)
			{
				return true;
			}
			if (!this.m_auraAbility)
			{
				return false;
			}
			if (!this.m_auraAbility.IsCombatAura)
			{
				return obj != Stance.Combat;
			}
			return obj == Stance.Combat;
		}

		// Token: 0x0600605B RID: 24667 RVA: 0x00080E51 File Offset: 0x0007F051
		public bool ShouldOutrightCancel(Stance obj)
		{
			return obj == Stance.Unconscious;
		}

		// Token: 0x0600605C RID: 24668 RVA: 0x00080E57 File Offset: 0x0007F057
		public bool CanPause(GameEntity entity)
		{
			return this.m_auraAbility && this.m_auraAbility.IsCombatAura && entity && entity == this.m_sourceEntity;
		}

		// Token: 0x0600605D RID: 24669 RVA: 0x00080E89 File Offset: 0x0007F089
		public bool CanResume(GameEntity entity)
		{
			return this.CanPause(entity) && this.IsMemorized(entity);
		}

		// Token: 0x0600605E RID: 24670 RVA: 0x001FC6E4 File Offset: 0x001FA8E4
		private bool IsMemorized(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			return entity && entity.CollectionController != null && entity.CollectionController.Abilities != null && entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_auraAbility.Id, out archetypeInstance) && archetypeInstance.AbilityData != null && archetypeInstance.AbilityData.MemorizationTimestamp != null;
		}

		// Token: 0x040052EF RID: 21231
		private List<GameEntity> m_currentEntities;

		// Token: 0x040052F0 RID: 21232
		private bool m_inPool;

		// Token: 0x040052F1 RID: 21233
		private GameEntity m_sourceEntity;

		// Token: 0x040052F2 RID: 21234
		private AuraAbility m_auraAbility;

		// Token: 0x040052F3 RID: 21235
		private EffectSyncData? m_syncData;

		// Token: 0x040052F4 RID: 21236
		private bool m_paused;
	}
}
