using System;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game;
using SoL.Game.Spawning;
using SoL.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Networking.Database
{
	// Token: 0x02000436 RID: 1078
	[Serializable]
	public class CharacterLocation : INetworkSerializable, ISpawnLocation
	{
		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06001EC6 RID: 7878 RVA: 0x00056CC7 File Offset: 0x00054EC7
		[JsonIgnore]
		[BsonIgnore]
		public ZoneRecord Zone
		{
			get
			{
				if (this.m_zone == null)
				{
					this.m_zone = SessionData.GetZoneRecord((ZoneId)this.ZoneId);
				}
				return this.m_zone;
			}
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x0011D498 File Offset: 0x0011B698
		public void UpdateFromTransform(Transform trans)
		{
			this.x = trans.position.x;
			this.y = trans.position.y;
			this.z = trans.position.z;
			this.h = trans.rotation.eulerAngles.y;
		}

		// Token: 0x06001EC8 RID: 7880 RVA: 0x0011D4F4 File Offset: 0x0011B6F4
		public CharacterLocation Clone()
		{
			return new CharacterLocation
			{
				ZoneId = this.ZoneId,
				x = this.x,
				y = this.y,
				z = this.z,
				h = this.h
			};
		}

		// Token: 0x06001EC9 RID: 7881 RVA: 0x0011D544 File Offset: 0x0011B744
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddInt(this.ZoneId);
			buffer.AddFloat(this.x);
			buffer.AddFloat(this.y);
			buffer.AddFloat(this.z);
			buffer.AddFloat(this.h);
			return buffer;
		}

		// Token: 0x06001ECA RID: 7882 RVA: 0x00056CE8 File Offset: 0x00054EE8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ZoneId = buffer.ReadInt();
			this.x = buffer.ReadFloat();
			this.y = buffer.ReadFloat();
			this.z = buffer.ReadFloat();
			this.h = buffer.ReadFloat();
			return buffer;
		}

		// Token: 0x06001ECB RID: 7883 RVA: 0x00056D27 File Offset: 0x00054F27
		Vector3 ISpawnLocation.GetPosition()
		{
			return this.GetPosition();
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x00056D2F File Offset: 0x00054F2F
		Quaternion ISpawnLocation.GetRotation()
		{
			return this.GetRotation();
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001ECD RID: 7885 RVA: 0x00056D37 File Offset: 0x00054F37
		// (set) Token: 0x06001ECE RID: 7886 RVA: 0x00056D3F File Offset: 0x00054F3F
		bool ISpawnLocation.Occupied { get; set; }

		// Token: 0x06001ECF RID: 7887 RVA: 0x0004475B File Offset: 0x0004295B
		void ISpawnLocation.SetPosition(NavMeshHit hit)
		{
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x0011D594 File Offset: 0x0011B794
		public override string ToString()
		{
			return string.Format("pos: ({0:F2}, {1:F2}, {2:F2}), rot: {3:F2}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.h
			});
		}

		// Token: 0x04002432 RID: 9266
		public int ZoneId;

		// Token: 0x04002433 RID: 9267
		public float x;

		// Token: 0x04002434 RID: 9268
		public float y;

		// Token: 0x04002435 RID: 9269
		public float z;

		// Token: 0x04002436 RID: 9270
		public float h;

		// Token: 0x04002437 RID: 9271
		[JsonIgnore]
		[BsonIgnore]
		private ZoneRecord m_zone;
	}
}
