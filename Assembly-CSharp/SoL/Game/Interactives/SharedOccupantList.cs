using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BB8 RID: 3000
	public class SharedOccupantList : SyncVarReplicator
	{
		// Token: 0x170015F7 RID: 5623
		// (get) Token: 0x06005CFF RID: 23807 RVA: 0x0007E7CA File Offset: 0x0007C9CA
		internal bool IsFull
		{
			get
			{
				if (!GameManager.IsServer)
				{
					return this.m_occupantNames.Count >= this.m_populationCap;
				}
				return this.m_occupants.Count >= this.m_populationCap;
			}
		}

		// Token: 0x06005D00 RID: 23808 RVA: 0x0007E800 File Offset: 0x0007CA00
		internal string GetOccupantNames()
		{
			if (this.m_occupantNames.Count > 0)
			{
				return string.Join(", ", this.m_occupantNames);
			}
			return string.Empty;
		}

		// Token: 0x06005D01 RID: 23809 RVA: 0x001F276C File Offset: 0x001F096C
		internal bool IsInList(GameEntity entity)
		{
			if (!entity || !entity.CharacterData)
			{
				return false;
			}
			if (GameManager.IsServer)
			{
				SharedOccupantList.OccupantData item = new SharedOccupantList.OccupantData
				{
					Entity = entity
				};
				return this.m_occupants.Contains(item);
			}
			return this.m_occupantNames.Contains(entity.CharacterData.Name.Value);
		}

		// Token: 0x06005D02 RID: 23810 RVA: 0x0007E826 File Offset: 0x0007CA26
		internal bool IsInList(ref SharedOccupantList.OccupantData occupant)
		{
			return GameManager.IsServer && this.m_occupants.Contains(occupant);
		}

		// Token: 0x06005D03 RID: 23811 RVA: 0x0007E842 File Offset: 0x0007CA42
		internal void AddToList(ref SharedOccupantList.OccupantData occupant)
		{
			if (!this.m_occupants.Contains(occupant))
			{
				this.m_occupants.Add(occupant);
				this.m_occupantNames.Add(occupant.Name);
			}
		}

		// Token: 0x06005D04 RID: 23812 RVA: 0x0007E879 File Offset: 0x0007CA79
		internal void RemoveFromList(ref SharedOccupantList.OccupantData occupant)
		{
			this.m_occupants.Remove(occupant);
			this.m_occupantNames.Remove(occupant.Name);
		}

		// Token: 0x06005D05 RID: 23813 RVA: 0x001F27D4 File Offset: 0x001F09D4
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_occupantNames);
			this.m_occupantNames.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04005064 RID: 20580
		[SerializeField]
		private int m_populationCap = 2;

		// Token: 0x04005065 RID: 20581
		private readonly List<SharedOccupantList.OccupantData> m_occupants = new List<SharedOccupantList.OccupantData>(10);

		// Token: 0x04005066 RID: 20582
		private readonly SynchronizedListString m_occupantNames = new SynchronizedListString();

		// Token: 0x02000BB9 RID: 3001
		internal struct OccupantData : IEquatable<SharedOccupantList.OccupantData>
		{
			// Token: 0x06005D07 RID: 23815 RVA: 0x0007E8C6 File Offset: 0x0007CAC6
			public bool Equals(SharedOccupantList.OccupantData other)
			{
				return object.Equals(this.Entity, other.Entity);
			}

			// Token: 0x06005D08 RID: 23816 RVA: 0x001F2810 File Offset: 0x001F0A10
			public override bool Equals(object obj)
			{
				if (obj is SharedOccupantList.OccupantData)
				{
					SharedOccupantList.OccupantData other = (SharedOccupantList.OccupantData)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06005D09 RID: 23817 RVA: 0x0007E8D9 File Offset: 0x0007CAD9
			public override int GetHashCode()
			{
				if (!(this.Entity != null))
				{
					return 0;
				}
				return this.Entity.GetHashCode();
			}

			// Token: 0x04005067 RID: 20583
			public GameEntity Entity;

			// Token: 0x04005068 RID: 20584
			public string Name;
		}
	}
}
