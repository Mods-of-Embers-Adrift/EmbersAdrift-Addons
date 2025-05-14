using System;
using SoL.Networking.Replication;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Targeting
{
	// Token: 0x0200064D RID: 1613
	public abstract class BaseTargetable : GameEntityComponent, ITargetable
	{
		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x06003229 RID: 12841 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool IsPlayer
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x0600322A RID: 12842 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool IsNpc
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x0600322B RID: 12843 RVA: 0x00062AD6 File Offset: 0x00060CD6
		private Faction m_faction
		{
			get
			{
				return base.GameEntity.CharacterData.Faction;
			}
		}

		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x0600322C RID: 12844 RVA: 0x00062AE8 File Offset: 0x00060CE8
		private SynchronizedString m_name
		{
			get
			{
				return base.GameEntity.CharacterData.Name;
			}
		}

		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x0600322D RID: 12845 RVA: 0x00062AFA File Offset: 0x00060CFA
		private SynchronizedString m_title
		{
			get
			{
				return base.GameEntity.CharacterData.Title;
			}
		}

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x0600322E RID: 12846 RVA: 0x00062B0C File Offset: 0x00060D0C
		private SynchronizedString m_guild
		{
			get
			{
				return base.GameEntity.CharacterData.GuildName;
			}
		}

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x0600322F RID: 12847 RVA: 0x00062B1E File Offset: 0x00060D1E
		private SynchronizedEnum<PlayerFlags> m_playerFlags
		{
			get
			{
				return base.GameEntity.CharacterData.CharacterFlags;
			}
		}

		// Token: 0x06003230 RID: 12848 RVA: 0x00062B30 File Offset: 0x00060D30
		private void Awake()
		{
			base.GameEntity.Targetable = this;
		}

		// Token: 0x06003231 RID: 12849 RVA: 0x0015F714 File Offset: 0x0015D914
		private void Start()
		{
			float num = Mathf.Clamp(this.m_distanceBuffer, 0.2f, 10f);
			float value = (base.GameEntity.CharacterData & base.GameEntity.CharacterData.TransformScale != null) ? num.PercentModification(base.GameEntity.CharacterData.TransformScale.Value) : num;
			this.m_distanceBufferScaled = Mathf.Clamp(value, 0f, 10f);
		}

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x06003232 RID: 12850 RVA: 0x00062B3E File Offset: 0x00060D3E
		GameEntity ITargetable.Entity
		{
			get
			{
				return base.GameEntity;
			}
		}

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x06003233 RID: 12851 RVA: 0x00062B46 File Offset: 0x00060D46
		Faction ITargetable.Faction
		{
			get
			{
				return this.m_faction;
			}
		}

		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06003234 RID: 12852 RVA: 0x00062B4E File Offset: 0x00060D4E
		bool ITargetable.IsPlayer
		{
			get
			{
				return this.IsPlayer;
			}
		}

		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06003235 RID: 12853 RVA: 0x00062B56 File Offset: 0x00060D56
		bool ITargetable.IsNpc
		{
			get
			{
				return this.IsNpc;
			}
		}

		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x06003236 RID: 12854 RVA: 0x00062B5E File Offset: 0x00060D5E
		int ITargetable.Level
		{
			get
			{
				return base.GameEntity.CharacterData.AdventuringLevel;
			}
		}

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x06003237 RID: 12855 RVA: 0x00062B70 File Offset: 0x00060D70
		float ITargetable.DistanceBuffer
		{
			get
			{
				return this.m_distanceBufferScaled;
			}
		}

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x06003238 RID: 12856 RVA: 0x0015F79C File Offset: 0x0015D99C
		float? ITargetable.ReticleRadiusOverride
		{
			get
			{
				if (!this.m_overrideReticleRadius)
				{
					return null;
				}
				return new float?(this.m_reticleRadius);
			}
		}

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x06003239 RID: 12857 RVA: 0x00062B78 File Offset: 0x00060D78
		SynchronizedString ITargetable.Name
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x0600323A RID: 12858 RVA: 0x00062B80 File Offset: 0x00060D80
		SynchronizedString ITargetable.Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x0600323B RID: 12859 RVA: 0x00062B88 File Offset: 0x00060D88
		SynchronizedString ITargetable.Guild
		{
			get
			{
				return this.m_guild;
			}
		}

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x0600323C RID: 12860 RVA: 0x00062B90 File Offset: 0x00060D90
		SynchronizedEnum<PlayerFlags> ITargetable.PlayerFlags
		{
			get
			{
				return this.m_playerFlags;
			}
		}

		// Token: 0x040030C7 RID: 12487
		private const float kMinDistanceBuffer = 0.2f;

		// Token: 0x040030C8 RID: 12488
		private const float kMaxDistanceBuffer = 10f;

		// Token: 0x040030C9 RID: 12489
		[Range(0.2f, 10f)]
		[SerializeField]
		protected float m_distanceBuffer;

		// Token: 0x040030CA RID: 12490
		[NonSerialized]
		private float m_distanceBufferScaled;

		// Token: 0x040030CB RID: 12491
		[SerializeField]
		private bool m_overrideReticleRadius;

		// Token: 0x040030CC RID: 12492
		[Range(0.2f, 10f)]
		[SerializeField]
		private float m_reticleRadius = 1f;
	}
}
