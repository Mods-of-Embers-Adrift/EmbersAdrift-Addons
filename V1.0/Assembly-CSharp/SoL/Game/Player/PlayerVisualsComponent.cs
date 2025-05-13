using System;

namespace SoL.Game.Player
{
	// Token: 0x020007EE RID: 2030
	public class PlayerVisualsComponent : GameEntityComponent
	{
		// Token: 0x06003B09 RID: 15113 RVA: 0x0017A480 File Offset: 0x00178680
		private void Start()
		{
			if (base.GameEntity != null && base.GameEntity.Type == GameEntityType.Player && base.GameEntity.NetworkEntity != null && !base.GameEntity.NetworkEntity.IsLocal && base.GameEntity.CharacterData != null)
			{
				base.GameEntity.CharacterData.CharacterFlags.Changed += this.CharacterFlagsOnChanged;
				this.CharacterFlagsOnChanged(base.GameEntity.CharacterData.CharacterFlags.Value);
			}
		}

		// Token: 0x06003B0A RID: 15114 RVA: 0x0017A520 File Offset: 0x00178720
		private void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.Type == GameEntityType.Player && base.GameEntity.NetworkEntity != null && !base.GameEntity.NetworkEntity.IsLocal && base.GameEntity.CharacterData != null)
			{
				base.GameEntity.CharacterData.CharacterFlags.Changed -= this.CharacterFlagsOnChanged;
			}
		}

		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06003B0B RID: 15115 RVA: 0x00067FAF File Offset: 0x000661AF
		// (set) Token: 0x06003B0C RID: 15116 RVA: 0x00067FB7 File Offset: 0x000661B7
		public bool IsInvisibleLocally
		{
			get
			{
				return this.m_isInvisibleLocally;
			}
			set
			{
				this.m_isInvisibleLocally = value;
				this.RefreshVisibility();
			}
		}

		// Token: 0x06003B0D RID: 15117 RVA: 0x00067FC6 File Offset: 0x000661C6
		private void CharacterFlagsOnChanged(PlayerFlags obj)
		{
			this.m_currentPlayerFlags = obj;
			this.RefreshVisibility();
		}

		// Token: 0x06003B0E RID: 15118 RVA: 0x0017A5A4 File Offset: 0x001787A4
		private void RefreshVisibility()
		{
			bool flag = !base.gameObject.activeInHierarchy;
			bool flag2 = this.IsInvisibleLocally || this.m_currentPlayerFlags.HasBitFlag(PlayerFlags.Invisible);
			if (flag != flag2)
			{
				base.gameObject.SetActive(!flag2);
			}
		}

		// Token: 0x04003983 RID: 14723
		private bool m_isInvisibleLocally;

		// Token: 0x04003984 RID: 14724
		private PlayerFlags m_currentPlayerFlags;
	}
}
