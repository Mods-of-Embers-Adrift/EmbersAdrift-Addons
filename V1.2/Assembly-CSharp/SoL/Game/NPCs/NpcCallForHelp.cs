using System;
using System.Collections.Generic;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000800 RID: 2048
	public class NpcCallForHelp : GameEntityComponent
	{
		// Token: 0x17000D9A RID: 3482
		// (get) Token: 0x06003B65 RID: 15205 RVA: 0x00068308 File Offset: 0x00066508
		// (set) Token: 0x06003B66 RID: 15206 RVA: 0x00068310 File Offset: 0x00066510
		public ICallForHelpSettings CallForHelpSettings { get; set; }

		// Token: 0x17000D9B RID: 3483
		// (get) Token: 0x06003B67 RID: 15207 RVA: 0x00068319 File Offset: 0x00066519
		// (set) Token: 0x06003B68 RID: 15208 RVA: 0x00068321 File Offset: 0x00066521
		public bool CallForHelpRequiresLos { get; set; }

		// Token: 0x06003B69 RID: 15209 RVA: 0x0017B188 File Offset: 0x00179388
		private void Awake()
		{
			if (!base.GameEntity)
			{
				base.enabled = false;
				return;
			}
			base.GameEntity.CallForHelp = this;
			if (NpcCallForHelp.m_players == null)
			{
				NpcCallForHelp.m_queryList = new List<NetworkEntity>(64);
				NpcCallForHelp.m_players = new List<GameEntity>(64);
				NpcCallForHelp.m_npcs = new List<GameEntity>(64);
			}
		}

		// Token: 0x06003B6A RID: 15210 RVA: 0x0006832A File Offset: 0x0006652A
		private void Start()
		{
			if (this.CallForHelpSettings == null || this.CallForHelpSettings.Periodic == null)
			{
				base.enabled = false;
				return;
			}
			this.m_vitals = base.GameEntity.Vitals;
		}

		// Token: 0x06003B6B RID: 15211 RVA: 0x0017B1E4 File Offset: 0x001793E4
		private void Update()
		{
			if (BaseNetworkEntityManager.PlayerConnectedCount <= 0)
			{
				return;
			}
			if (this.m_vitals && this.m_vitals.Health < (float)this.m_vitals.MaxHealth)
			{
				if (this.m_nextPeriodicCallForHelp == null)
				{
					this.RefreshNextCallForHelp();
					return;
				}
				if (Time.time >= this.m_nextPeriodicCallForHelp.Value)
				{
					this.RefreshNextCallForHelp();
					this.ChanceAlertNearbyNpcs();
				}
			}
		}

		// Token: 0x06003B6C RID: 15212 RVA: 0x0017B254 File Offset: 0x00179454
		private void RefreshNextCallForHelp()
		{
			this.m_nextPeriodicCallForHelp = new float?(Time.time + (float)this.CallForHelpSettings.Periodic.Frequency.RandomWithinRange());
		}

		// Token: 0x06003B6D RID: 15213 RVA: 0x0017B28C File Offset: 0x0017948C
		private void ChanceAlertNearbyNpcs()
		{
			if (this.CallForHelpSettings.Periodic.CallsForHelp(this.m_vitals.HealthPercent))
			{
				float range = this.CallForHelpSettings.Periodic.GetRange();
				if (this.AlertNearbyNpcs(range))
				{
					this.CallForHelpSettings.Periodic.EmoteCallForHelp(base.GameEntity, range);
				}
			}
		}

		// Token: 0x06003B6E RID: 15214 RVA: 0x0017B2E8 File Offset: 0x001794E8
		private void ManualAlertNearbyNpcs()
		{
			float range = this.CallForHelpSettings.Periodic.GetRange();
			if (this.AlertNearbyNpcs(range))
			{
				this.CallForHelpSettings.Periodic.EmoteCallForHelp(base.GameEntity, range);
			}
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x0017B328 File Offset: 0x00179528
		private bool AlertNearbyNpcs(float range)
		{
			NpcCallForHelp.m_players.Clear();
			NpcCallForHelp.m_npcs.Clear();
			float num = range * range;
			float radius = Mathf.Max(30.5f, range);
			ServerGameManager.SpatialManager.PhysicsQueryRadius(base.gameObject.transform.position, radius, NpcCallForHelp.m_queryList, false, null);
			List<NetworkEntity> queryList = NpcCallForHelp.m_queryList;
			for (int i = 0; i < queryList.Count; i++)
			{
				if (queryList[i] && queryList[i].GameEntity && queryList[i].GameEntity.Vitals && queryList[i].GameEntity.Vitals.GetCurrentHealthState() == HealthState.Alive)
				{
					GameEntityType type = queryList[i].GameEntity.Type;
					if (type != GameEntityType.Player)
					{
						if (type == GameEntityType.Npc)
						{
							NpcCallForHelp.m_npcs.Add(queryList[i].GameEntity);
						}
					}
					else
					{
						NpcCallForHelp.m_players.Add(queryList[i].GameEntity);
					}
				}
			}
			CallForHelpExtensions.SortListBySqrMagnitudeDistance(base.GameEntity, NpcCallForHelp.m_players);
			CallForHelpExtensions.SortListBySqrMagnitudeDistance(base.GameEntity, NpcCallForHelp.m_npcs);
			int num2 = 0;
			int num3 = 0;
			if (NpcCallForHelp.m_players.Count > 0 && NpcCallForHelp.m_npcs.Count > 0)
			{
				CallForHelpPeriodicData periodic = this.CallForHelpSettings.Periodic;
				foreach (GameEntity gameEntity in NpcCallForHelp.m_npcs)
				{
					if (gameEntity.SqrDistanceFromLastNpcCallForHelp <= num)
					{
						if (num2 >= periodic.MaxAlertCount)
						{
							break;
						}
						NpcTargetController npcTargetController = gameEntity.TargetController as NpcTargetController;
						if (npcTargetController && this.CallForHelpSettings.Periodic.CallToTags.Matches(npcTargetController.NpcTagsSet))
						{
							bool flag = false;
							bool flag2 = periodic.MaxHostileCount > 0 && periodic.MaxHostileCount > num3 && periodic.ChanceHostileForEntity(gameEntity);
							foreach (GameEntity gameEntity2 in NpcCallForHelp.m_players)
							{
								if (!this.CallForHelpRequiresLos || LineOfSight.NpcHasLineOfSight(gameEntity, gameEntity2))
								{
									if (flag2)
									{
										npcTargetController.AddSocialThreat(gameEntity2, 1f);
										flag = true;
									}
									else
									{
										flag = (npcTargetController.ApplyAlert(gameEntity2.NetworkEntity, true) || flag);
									}
								}
							}
							if (flag)
							{
								num2++;
								if (flag2)
								{
									num3++;
								}
							}
						}
					}
				}
			}
			NpcCallForHelp.m_players.Clear();
			NpcCallForHelp.m_npcs.Clear();
			return num2 > 0;
		}

		// Token: 0x040039CA RID: 14794
		private Vitals m_vitals;

		// Token: 0x040039CB RID: 14795
		private float? m_nextPeriodicCallForHelp;

		// Token: 0x040039CC RID: 14796
		private const int kInitSize = 64;

		// Token: 0x040039CD RID: 14797
		private static List<NetworkEntity> m_queryList;

		// Token: 0x040039CE RID: 14798
		private static List<GameEntity> m_players;

		// Token: 0x040039CF RID: 14799
		private static List<GameEntity> m_npcs;
	}
}
