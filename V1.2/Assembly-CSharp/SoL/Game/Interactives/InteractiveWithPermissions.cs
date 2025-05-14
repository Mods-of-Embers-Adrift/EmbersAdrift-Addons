using System;
using SoL.Managers;
using SoL.Networking.Replication;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA8 RID: 2984
	public abstract class InteractiveWithPermissions : InteractiveRemoteContainerSingleLooter
	{
		// Token: 0x170015D8 RID: 5592
		// (get) Token: 0x06005C83 RID: 23683 RVA: 0x0007E0C8 File Offset: 0x0007C2C8
		// (set) Token: 0x06005C84 RID: 23684 RVA: 0x001F1598 File Offset: 0x001EF798
		private protected GameEntity TaggerEntity
		{
			protected get
			{
				return this.m_taggerEntity;
			}
			private set
			{
				if (value != null && this.m_taggerEntity == value)
				{
					return;
				}
				this.m_taggerEntity = value;
				this.m_frameTagged = Time.frameCount;
				if (this.m_taggerEntity == null)
				{
					this.m_tagger.Value = "";
					this.m_groupId.Value = UniqueId.Empty;
					this.m_raidId.Value = UniqueId.Empty;
					return;
				}
				if (this.m_taggerEntity.CharacterData)
				{
					this.m_tagger.Value = this.m_taggerEntity.CharacterData.Name.Value;
					if (!this.m_taggerEntity.CharacterData.GroupId.IsEmpty)
					{
						this.m_groupId.Value = this.m_taggerEntity.CharacterData.GroupId;
					}
					if (!this.m_taggerEntity.CharacterData.RaidId.IsEmpty)
					{
						this.m_raidId.Value = this.m_taggerEntity.CharacterData.RaidId;
					}
				}
			}
		}

		// Token: 0x170015D9 RID: 5593
		// (get) Token: 0x06005C85 RID: 23685 RVA: 0x001F16B0 File Offset: 0x001EF8B0
		public bool IsTagged
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_tagger.Value) || !this.m_groupId.Value.IsEmpty || !this.m_raidId.Value.IsEmpty;
			}
		}

		// Token: 0x170015DA RID: 5594
		// (get) Token: 0x06005C86 RID: 23686 RVA: 0x0007E0D0 File Offset: 0x0007C2D0
		public bool IsInteractive
		{
			get
			{
				return this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.Interactive);
			}
		}

		// Token: 0x170015DB RID: 5595
		// (get) Token: 0x06005C87 RID: 23687
		// (set) Token: 0x06005C88 RID: 23688
		protected abstract float m_permissionTimeoutTime { get; set; }

		// Token: 0x06005C89 RID: 23689 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void PermissionsCleared()
		{
		}

		// Token: 0x06005C8A RID: 23690 RVA: 0x0007E0E3 File Offset: 0x0007C2E3
		public void AddAsTagger(GameEntity entity)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			if (this.m_permissionsInitializedTime == null && this.TaggerEntity == null)
			{
				this.TaggerEntity = entity;
			}
		}

		// Token: 0x06005C8B RID: 23691 RVA: 0x0007E10F File Offset: 0x0007C30F
		public void ClearTaggers()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			if (this.m_permissionsInitializedTime == null)
			{
				this.TaggerEntity = null;
				this.PermissionsCleared();
			}
		}

		// Token: 0x06005C8C RID: 23692 RVA: 0x0007E133 File Offset: 0x0007C333
		protected override void OnDestroy()
		{
			this.m_taggerEntity = null;
			base.OnDestroy();
		}

		// Token: 0x06005C8D RID: 23693 RVA: 0x0007E142 File Offset: 0x0007C342
		protected override void Update()
		{
			base.Update();
			this.UpdateTaggers();
			this.UpdatePermissions();
		}

		// Token: 0x06005C8E RID: 23694 RVA: 0x0007E156 File Offset: 0x0007C356
		protected override bool CanInteractInternal()
		{
			return this.m_permissionsInitializedTime != null && base.CanInteractInternal();
		}

		// Token: 0x06005C8F RID: 23695 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateTaggers()
		{
		}

		// Token: 0x06005C90 RID: 23696 RVA: 0x001F16FC File Offset: 0x001EF8FC
		private void UpdatePermissions()
		{
			if (!this.m_permissionsExpire)
			{
				return;
			}
			if (GameManager.IsServer && this.m_permissionsInitializedTime != null && !this.m_permissionsTimedOut && (DateTime.UtcNow - this.m_permissionsInitializedTime.Value).TotalSeconds > (double)this.m_permissionTimeoutTime)
			{
				this.m_tagger.Value = "";
				this.m_groupId.Value = UniqueId.Empty;
				this.m_raidId.Value = UniqueId.Empty;
				this.m_permissionsTimedOut = true;
			}
		}

		// Token: 0x06005C91 RID: 23697 RVA: 0x001F178C File Offset: 0x001EF98C
		public override bool CanInteract(GameEntity entity)
		{
			return base.CanInteract(entity) && (string.IsNullOrEmpty(this.m_tagger.Value) || (entity.CharacterData && (this.m_tagger.Value.Equals(entity.CharacterData.Name.Value, StringComparison.InvariantCultureIgnoreCase) || (!this.m_groupId.Value.IsEmpty && this.m_groupId.Value == entity.CharacterData.GroupId) || (!this.m_raidId.Value.IsEmpty && this.m_raidId.Value == entity.CharacterData.RaidId))));
		}

		// Token: 0x06005C92 RID: 23698 RVA: 0x001F1854 File Offset: 0x001EFA54
		protected bool HasValidPermissions(GameEntity interactionSource)
		{
			if (this.m_interactiveFlags.Value.HasBitFlag(InteractiveFlags.NpcContributed))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(this.m_tagger.Value))
			{
				return interactionSource.CharacterData.Name.Value.Equals(this.m_tagger.Value, StringComparison.InvariantCultureIgnoreCase) || (!this.m_groupId.Value.IsEmpty && this.m_groupId.Value == interactionSource.CharacterData.GroupId) || (!this.m_raidId.Value.IsEmpty && this.m_raidId.Value == interactionSource.CharacterData.RaidId);
			}
			return this.m_groupId.Value.IsEmpty;
		}

		// Token: 0x06005C93 RID: 23699 RVA: 0x001F1930 File Offset: 0x001EFB30
		public bool IsValidTagger(GameEntity entity)
		{
			if (this.IsTagged && entity && entity.CharacterData && entity.CharacterData.Name != null)
			{
				if (entity.CharacterData.Name.Value.Equals(this.m_tagger.Value, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
				if (!this.m_groupId.Value.IsEmpty && this.m_groupId.Value == entity.CharacterData.GroupId)
				{
					return true;
				}
				if (!this.m_raidId.Value.IsEmpty && this.m_raidId.Value == entity.CharacterData.RaidId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005C94 RID: 23700 RVA: 0x0007E16D File Offset: 0x0007C36D
		protected override void AddContextualTooltipText()
		{
			base.AddContextualTooltipText();
			if (!this.HasValidPermissions(LocalPlayer.GameEntity))
			{
				BaseTooltip.Sb.AppendLine("Invalid Permissions");
			}
		}

		// Token: 0x06005C95 RID: 23701 RVA: 0x001F1A04 File Offset: 0x001EFC04
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_groupId);
			this.m_groupId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_raidId);
			this.m_raidId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_tagger);
			this.m_tagger.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04005022 RID: 20514
		protected int m_frameTagged;

		// Token: 0x04005023 RID: 20515
		private GameEntity m_taggerEntity;

		// Token: 0x04005024 RID: 20516
		protected readonly SynchronizedString m_tagger = new SynchronizedString();

		// Token: 0x04005025 RID: 20517
		protected readonly SynchronizedUniqueId m_groupId = new SynchronizedUniqueId();

		// Token: 0x04005026 RID: 20518
		protected readonly SynchronizedUniqueId m_raidId = new SynchronizedUniqueId();

		// Token: 0x04005027 RID: 20519
		protected DateTime? m_permissionsInitializedTime;

		// Token: 0x04005028 RID: 20520
		private bool m_permissionsTimedOut;

		// Token: 0x04005029 RID: 20521
		[SerializeField]
		protected bool m_permissionsExpire = true;
	}
}
