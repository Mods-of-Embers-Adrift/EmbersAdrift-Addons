using System;
using System.Collections.Generic;
using Drawing;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.NPCs.Interactions
{
	// Token: 0x0200083B RID: 2107
	public class NpcPath : MonoBehaviourGizmos
	{
		// Token: 0x06003CEE RID: 15598 RVA: 0x001813FC File Offset: 0x0017F5FC
		public void RegisterOccupant(GameEntity entity, NpcWaypoint wp)
		{
			if (!entity || !entity.NetworkEntity)
			{
				return;
			}
			if (this.m_lastRemoval.Entity == entity)
			{
				this.m_occupants.Add(entity.NetworkEntity.NetworkId.Value, new NpcPath.NpcPathData
				{
					Entity = entity,
					Reversed = this.m_lastRemoval.Reversed
				});
				this.m_lastRemoval = default(NpcPath.NpcPathData);
				return;
			}
			bool reversed = false;
			NpcPath.InitialDirectionChoice directionChoice = this.m_directionChoice;
			if (directionChoice != NpcPath.InitialDirectionChoice.Random)
			{
				if (directionChoice - NpcPath.InitialDirectionChoice.MostWaypoints <= 1)
				{
					int num = this.GetWaypointIndex(wp) + 1;
					int num2 = this.m_waypoints.Length - num;
					int num3 = num - 1;
					if (this.m_directionChoice == NpcPath.InitialDirectionChoice.MostWaypoints)
					{
						reversed = (num2 < num3);
					}
					else
					{
						reversed = (num3 < num2);
					}
				}
			}
			else
			{
				reversed = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
			}
			this.m_occupants.Add(entity.NetworkEntity.NetworkId.Value, new NpcPath.NpcPathData
			{
				Entity = entity,
				Reversed = reversed
			});
		}

		// Token: 0x06003CEF RID: 15599 RVA: 0x0018151C File Offset: 0x0017F71C
		public void DeregisterOccupant(GameEntity entity)
		{
			if (!entity || !entity.NetworkEntity)
			{
				return;
			}
			if (this.m_occupants.TryGetValue(entity.NetworkEntity.NetworkId.Value, out this.m_lastRemoval))
			{
				this.m_occupants.Remove(entity.NetworkEntity.NetworkId.Value);
				if (this.m_useOccupancyCooldown)
				{
					this.m_activeOccupancyCooldowns.Add(Time.time + this.m_occupancyCooldownTime.RandomWithinRange());
				}
			}
		}

		// Token: 0x17000E09 RID: 3593
		// (get) Token: 0x06003CF0 RID: 15600 RVA: 0x000694DA File Offset: 0x000676DA
		internal bool UseLightItemAtNight
		{
			get
			{
				return this.m_useLightItemAtNight;
			}
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x001815A8 File Offset: 0x0017F7A8
		private void Awake()
		{
			if (this.m_useOccupancyCooldown)
			{
				this.m_activeOccupancyCooldowns = new List<float>(this.m_maxOccupancy);
			}
			if (this.m_waypoints != null)
			{
				for (int i = 0; i < this.m_waypoints.Length; i++)
				{
					if (this.m_waypoints[i])
					{
						this.m_waypoints[i].Init(this);
					}
				}
			}
		}

		// Token: 0x06003CF2 RID: 15602 RVA: 0x00181608 File Offset: 0x0017F808
		internal bool EntityCanInteract(GameEntity entity)
		{
			int num = 0;
			if (this.m_useOccupancyCooldown && this.m_activeOccupancyCooldowns.Count > 0)
			{
				float time = Time.time;
				for (int i = this.m_activeOccupancyCooldowns.Count - 1; i >= 0; i--)
				{
					if (this.m_activeOccupancyCooldowns[i] <= time)
					{
						this.m_activeOccupancyCooldowns.RemoveAt(i);
					}
				}
				num = this.m_activeOccupancyCooldowns.Count;
			}
			return this.m_occupants.Count + num < this.m_maxOccupancy && entity && entity.Type == GameEntityType.Npc && this.EntityFlagsMatch(entity);
		}

		// Token: 0x06003CF3 RID: 15603 RVA: 0x001816A4 File Offset: 0x0017F8A4
		private bool EntityFlagsMatch(GameEntity entity)
		{
			if (entity && entity.ServerNpcController && entity.CharacterData)
			{
				long b = (this.m_npcTagSet != null) ? this.m_npcTagSet.GetCombinedTags() : 0L;
				return NpcTagExtensions.HasAnyFlags((entity.CharacterData.NpcTagsSet != null) ? entity.CharacterData.NpcTagsSet.GetCombinedTags() : 0L, b) && entity.ServerNpcController.InteractionFlags.HasAnyFlags(this.m_interactionFlags);
			}
			return false;
		}

		// Token: 0x06003CF4 RID: 15604 RVA: 0x00181730 File Offset: 0x0017F930
		internal NpcWaypoint GetNextWaypoint(GameEntity entity, NpcWaypoint waypoint)
		{
			if (this.m_waypoints == null || !entity || !entity.NetworkEntity)
			{
				return null;
			}
			uint value = entity.NetworkEntity.NetworkId.Value;
			NpcPath.NpcPathData npcPathData;
			if (!this.m_occupants.TryGetValue(value, out npcPathData))
			{
				return null;
			}
			int waypointIndex = this.GetWaypointIndex(waypoint);
			int num = this.m_waypoints.Length - 1;
			if (waypointIndex == 0)
			{
				if (!npcPathData.Reversed)
				{
					return this.m_waypoints[1];
				}
				NpcPath.LoopType loopType = this.m_loopType;
				if (loopType == NpcPath.LoopType.Closed)
				{
					return this.m_waypoints[num];
				}
				if (loopType != NpcPath.LoopType.PingPong)
				{
					return null;
				}
				npcPathData.Reversed = false;
				this.m_occupants[value] = npcPathData;
				return this.m_waypoints[1];
			}
			else if (waypointIndex == num)
			{
				if (npcPathData.Reversed)
				{
					return this.m_waypoints[num - 1];
				}
				NpcPath.LoopType loopType = this.m_loopType;
				if (loopType == NpcPath.LoopType.Closed)
				{
					return this.m_waypoints[0];
				}
				if (loopType != NpcPath.LoopType.PingPong)
				{
					return null;
				}
				npcPathData.Reversed = true;
				this.m_occupants[value] = npcPathData;
				return this.m_waypoints[num - 1];
			}
			else
			{
				if (!npcPathData.Reversed)
				{
					return this.m_waypoints[waypointIndex + 1];
				}
				return this.m_waypoints[waypointIndex - 1];
			}
		}

		// Token: 0x06003CF5 RID: 15605 RVA: 0x00181864 File Offset: 0x0017FA64
		private int GetWaypointIndex(NpcWaypoint waypoint)
		{
			for (int i = 0; i < this.m_waypoints.Length; i++)
			{
				if (this.m_waypoints[i] && this.m_waypoints[i] == waypoint)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06003CF6 RID: 15606 RVA: 0x001818A8 File Offset: 0x0017FAA8
		private bool GetWaypointIndex(NpcWaypoint waypoint, out int index)
		{
			index = -1;
			if (this.m_waypoints != null)
			{
				for (int i = 0; i < this.m_waypoints.Length; i++)
				{
					if (this.m_waypoints[i] == waypoint)
					{
						index = i;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04003BB9 RID: 15289
		private const string kGizmoGroup = "Gizmos";

		// Token: 0x04003BBA RID: 15290
		internal const string kButtonGroup = "CreateWaypoints";

		// Token: 0x04003BBB RID: 15291
		internal const string kCreateAtPath = "Create WP @ Path";

		// Token: 0x04003BBC RID: 15292
		internal const string kCreateAtCamera = "Create WP @ Camera";

		// Token: 0x04003BBD RID: 15293
		private readonly Dictionary<uint, NpcPath.NpcPathData> m_occupants = new Dictionary<uint, NpcPath.NpcPathData>();

		// Token: 0x04003BBE RID: 15294
		private NpcPath.NpcPathData m_lastRemoval;

		// Token: 0x04003BBF RID: 15295
		[SerializeField]
		private bool m_showHandles = true;

		// Token: 0x04003BC0 RID: 15296
		[SerializeField]
		private Color m_gizmoColor = Color.yellow;

		// Token: 0x04003BC1 RID: 15297
		[SerializeField]
		private NpcPath.InitialDirectionChoice m_directionChoice;

		// Token: 0x04003BC2 RID: 15298
		[SerializeField]
		private NpcPath.LoopType m_loopType;

		// Token: 0x04003BC3 RID: 15299
		[SerializeField]
		private bool m_useLightItemAtNight;

		// Token: 0x04003BC4 RID: 15300
		[SerializeField]
		private bool m_useOccupancyCooldown;

		// Token: 0x04003BC5 RID: 15301
		[SerializeField]
		private MinMaxFloatRange m_occupancyCooldownTime = new MinMaxFloatRange(30f, 60f);

		// Token: 0x04003BC6 RID: 15302
		[Range(1f, 10f)]
		[SerializeField]
		private int m_maxOccupancy = 1;

		// Token: 0x04003BC7 RID: 15303
		[SerializeField]
		private NpcTags m_npcTags;

		// Token: 0x04003BC8 RID: 15304
		[SerializeField]
		private NpcTagSet m_npcTagSet;

		// Token: 0x04003BC9 RID: 15305
		[SerializeField]
		private NpcInteractionFlags m_interactionFlags = NpcInteractionFlags.Path;

		// Token: 0x04003BCA RID: 15306
		[SerializeField]
		private NpcWaypoint[] m_waypoints;

		// Token: 0x04003BCB RID: 15307
		private List<float> m_activeOccupancyCooldowns;

		// Token: 0x0200083C RID: 2108
		private struct NpcPathData
		{
			// Token: 0x04003BCC RID: 15308
			public GameEntity Entity;

			// Token: 0x04003BCD RID: 15309
			public bool Reversed;
		}

		// Token: 0x0200083D RID: 2109
		private enum LoopType
		{
			// Token: 0x04003BCF RID: 15311
			None,
			// Token: 0x04003BD0 RID: 15312
			Closed,
			// Token: 0x04003BD1 RID: 15313
			PingPong
		}

		// Token: 0x0200083E RID: 2110
		private enum InitialDirectionChoice
		{
			// Token: 0x04003BD3 RID: 15315
			Random,
			// Token: 0x04003BD4 RID: 15316
			MostWaypoints,
			// Token: 0x04003BD5 RID: 15317
			FewestWaypoints
		}
	}
}
