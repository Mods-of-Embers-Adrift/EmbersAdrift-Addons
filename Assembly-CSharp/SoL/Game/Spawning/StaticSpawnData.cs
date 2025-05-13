using System;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning.Behavior;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006D9 RID: 1753
	[Serializable]
	public class StaticSpawnData : ISpawnController
	{
		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x06003518 RID: 13592 RVA: 0x000645E8 File Offset: 0x000627E8
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x06003519 RID: 13593 RVA: 0x000645F0 File Offset: 0x000627F0
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x0600351A RID: 13594 RVA: 0x000645F8 File Offset: 0x000627F8
		public int MaxHealth
		{
			get
			{
				return this.m_maxHealth;
			}
		}

		// Token: 0x17000BB0 RID: 2992
		// (get) Token: 0x0600351B RID: 13595 RVA: 0x00064600 File Offset: 0x00062800
		public int MaxArmorClass
		{
			get
			{
				return this.m_maxArmorClass;
			}
		}

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x0600351C RID: 13596 RVA: 0x00064608 File Offset: 0x00062808
		public int MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x0600351D RID: 13597 RVA: 0x00064610 File Offset: 0x00062810
		public PortraitConfig PortraitConfig
		{
			get
			{
				return this.m_portraitConfig;
			}
		}

		// Token: 0x0600351E RID: 13598 RVA: 0x0005897E File Offset: 0x00056B7E
		bool ISpawnController.TryGetBehaviorProfile(out BehaviorProfile profile)
		{
			profile = null;
			return false;
		}

		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x0600351F RID: 13599 RVA: 0x00049FFA File Offset: 0x000481FA
		BehaviorSubTreeCollection ISpawnController.BehaviorOverrides
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003520 RID: 13600 RVA: 0x00064618 File Offset: 0x00062818
		bool ISpawnController.TryGetLevel(out int level)
		{
			level = this.m_level;
			return true;
		}

		// Token: 0x06003521 RID: 13601 RVA: 0x00064623 File Offset: 0x00062823
		int ISpawnController.GetLevel()
		{
			return this.m_level;
		}

		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x06003522 RID: 13602 RVA: 0x0006462B File Offset: 0x0006282B
		MinMaxIntRange ISpawnController.LevelRange
		{
			get
			{
				return new MinMaxIntRange(this.m_level, this.m_level);
			}
		}

		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x06003523 RID: 13603 RVA: 0x00154B30 File Offset: 0x00152D30
		float? ISpawnController.LeashDistance
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x06003524 RID: 13604 RVA: 0x00154B30 File Offset: 0x00152D30
		float? ISpawnController.ResetDistance
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x06003525 RID: 13605 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.DespawnOnDeath
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x06003526 RID: 13606 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.CallForHelpRequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x06003527 RID: 13607 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.ForceIndoorProfiles
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x06003528 RID: 13608 RVA: 0x0006463E File Offset: 0x0006283E
		bool ISpawnController.MatchAttackerLevel
		{
			get
			{
				return this.m_matchAttackerLevel;
			}
		}

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x06003529 RID: 13609 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISpawnController.LogSpawns
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x0600352A RID: 13610 RVA: 0x00164B20 File Offset: 0x00162D20
		Vector3? ISpawnController.CurrentPosition
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x0600352B RID: 13611 RVA: 0x00049FFA File Offset: 0x000481FA
		DialogueSource ISpawnController.OverrideDialogue
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600352C RID: 13612 RVA: 0x00063E4C File Offset: 0x0006204C
		public bool OverrideInteractionFlags(out NpcInteractionFlags flags)
		{
			flags = NpcInteractionFlags.None;
			return false;
		}

		// Token: 0x0600352D RID: 13613 RVA: 0x0004475B File Offset: 0x0004295B
		void ISpawnController.NotifyOfDeath(GameEntity entity)
		{
		}

		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x0600352E RID: 13614 RVA: 0x00045BCA File Offset: 0x00043DCA
		int ISpawnController.XpAdjustment
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600352F RID: 13615 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		bool ISpawnController.TryGetOverrideData(SpawnProfile profile, out SpawnControllerOverrideData data)
		{
			data = null;
			return false;
		}

		// Token: 0x04003350 RID: 13136
		[SerializeField]
		private string m_name;

		// Token: 0x04003351 RID: 13137
		[SerializeField]
		private string m_title;

		// Token: 0x04003352 RID: 13138
		[SerializeField]
		private int m_maxHealth = 25;

		// Token: 0x04003353 RID: 13139
		[SerializeField]
		private int m_maxArmorClass = 30;

		// Token: 0x04003354 RID: 13140
		[SerializeField]
		private int m_maxDamageAbsorption = 100;

		// Token: 0x04003355 RID: 13141
		[SerializeField]
		private bool m_matchAttackerLevel;

		// Token: 0x04003356 RID: 13142
		[Range(1f, 50f)]
		[SerializeField]
		private int m_level = 1;

		// Token: 0x04003357 RID: 13143
		[SerializeField]
		private PortraitConfig m_portraitConfig;
	}
}
