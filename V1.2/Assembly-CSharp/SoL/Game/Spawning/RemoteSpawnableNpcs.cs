using System;
using SoL.Game.Objects;
using SoL.Game.Spawning.Behavior;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006B9 RID: 1721
	[CreateAssetMenu(menuName = "SoL/Spawning/Remote Npcs")]
	public class RemoteSpawnableNpcs : RemoteSpawnableBase
	{
		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x0600346B RID: 13419 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override RemoteSpawnProfile.RemoteSpawnProfileType Type
		{
			get
			{
				return RemoteSpawnProfile.RemoteSpawnProfileType.Npc;
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x0600346C RID: 13420 RVA: 0x00164B58 File Offset: 0x00162D58
		protected override BehaviorSubTreeCollection BehaviorOverrides
		{
			get
			{
				SpawnBehaviorType behaviorType = base.BehaviorType;
				if (behaviorType != SpawnBehaviorType.Wander)
				{
					if (behaviorType != SpawnBehaviorType.Hunt)
					{
						RemoteSpawnableNpcs.RemoteSpawnConfig @default = this.m_default;
						if (@default == null)
						{
							return null;
						}
						return @default.BehaviorOverrides;
					}
					else
					{
						RemoteSpawnableNpcs.RemoteSpawnConfig hunt = this.m_hunt;
						if (hunt == null)
						{
							return null;
						}
						return hunt.BehaviorOverrides;
					}
				}
				else
				{
					RemoteSpawnableNpcs.RemoteSpawnConfig wander = this.m_wander;
					if (wander == null)
					{
						return null;
					}
					return wander.BehaviorOverrides;
				}
			}
		}

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x0600346D RID: 13421 RVA: 0x00063E75 File Offset: 0x00062075
		protected override MinMaxIntRange LevelRange
		{
			get
			{
				return new MinMaxIntRange(base.Level, base.Level);
			}
		}

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x0600346E RID: 13422 RVA: 0x00164BAC File Offset: 0x00162DAC
		protected override float? LeashDistance
		{
			get
			{
				SpawnBehaviorType behaviorType = base.BehaviorType;
				if (behaviorType != SpawnBehaviorType.Wander)
				{
					if (behaviorType != SpawnBehaviorType.Hunt)
					{
						RemoteSpawnableNpcs.RemoteSpawnConfig @default = this.m_default;
						if (@default == null)
						{
							return null;
						}
						return @default.LeashDistance;
					}
					else
					{
						RemoteSpawnableNpcs.RemoteSpawnConfig hunt = this.m_hunt;
						if (hunt == null)
						{
							return null;
						}
						return hunt.LeashDistance;
					}
				}
				else
				{
					RemoteSpawnableNpcs.RemoteSpawnConfig wander = this.m_wander;
					if (wander == null)
					{
						return null;
					}
					return wander.LeashDistance;
				}
			}
		}

		// Token: 0x04003268 RID: 12904
		private const string kBehaviorGroup = "Behavior";

		// Token: 0x04003269 RID: 12905
		private const string kDefault = "Behavior/Default";

		// Token: 0x0400326A RID: 12906
		private const string kWander = "Behavior/Wander";

		// Token: 0x0400326B RID: 12907
		private const string kHunt = "Behavior/Hunt";

		// Token: 0x0400326C RID: 12908
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x0400326D RID: 12909
		[SerializeField]
		private RemoteSpawnableNpcs.RemoteSpawnConfig m_default;

		// Token: 0x0400326E RID: 12910
		[SerializeField]
		private RemoteSpawnableNpcs.RemoteSpawnConfig m_wander;

		// Token: 0x0400326F RID: 12911
		[SerializeField]
		private RemoteSpawnableNpcs.RemoteSpawnConfig m_hunt;

		// Token: 0x020006BA RID: 1722
		[Serializable]
		private class RemoteSpawnConfig
		{
			// Token: 0x17000B70 RID: 2928
			// (get) Token: 0x06003470 RID: 13424 RVA: 0x00063E88 File Offset: 0x00062088
			public BehaviorSubTreeCollection BehaviorOverrides
			{
				get
				{
					return this.m_behaviorOverrides;
				}
			}

			// Token: 0x17000B71 RID: 2929
			// (get) Token: 0x06003471 RID: 13425 RVA: 0x00164C18 File Offset: 0x00162E18
			public float? LeashDistance
			{
				get
				{
					if (!this.m_overrideLeashDistance)
					{
						return null;
					}
					return new float?(this.m_leashDistance);
				}
			}

			// Token: 0x04003270 RID: 12912
			[SerializeField]
			private bool m_overrideLeashDistance;

			// Token: 0x04003271 RID: 12913
			[SerializeField]
			private float m_leashDistance = 10f;

			// Token: 0x04003272 RID: 12914
			[SerializeField]
			private BehaviorSubTreeCollection m_behaviorOverrides;
		}
	}
}
