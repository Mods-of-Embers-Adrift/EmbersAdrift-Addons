using System;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Managers;
using SoL.Networking;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ADD RID: 2781
	[Serializable]
	public class MasteryInstanceData : INetworkSerializable
	{
		// Token: 0x14000112 RID: 274
		// (add) Token: 0x060055BB RID: 21947 RVA: 0x001DF7D4 File Offset: 0x001DD9D4
		// (remove) Token: 0x060055BC RID: 21948 RVA: 0x001DF80C File Offset: 0x001DDA0C
		public event Action MasteryDataChanged;

		// Token: 0x14000113 RID: 275
		// (add) Token: 0x060055BD RID: 21949 RVA: 0x001DF844 File Offset: 0x001DDA44
		// (remove) Token: 0x060055BE RID: 21950 RVA: 0x001DF87C File Offset: 0x001DDA7C
		public event Action<UniqueId> SpecializationUnlearned;

		// Token: 0x14000114 RID: 276
		// (add) Token: 0x060055BF RID: 21951 RVA: 0x001DF8B4 File Offset: 0x001DDAB4
		// (remove) Token: 0x060055C0 RID: 21952 RVA: 0x001DF8EC File Offset: 0x001DDAEC
		public event Action LevelDataChanged;

		// Token: 0x170013DC RID: 5084
		// (get) Token: 0x060055C1 RID: 21953 RVA: 0x00079338 File Offset: 0x00077538
		// (set) Token: 0x060055C2 RID: 21954 RVA: 0x001DF924 File Offset: 0x001DDB24
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public UniqueId? Specialization
		{
			get
			{
				return this.m_specialization;
			}
			set
			{
				if (this.m_specialization != null && value != this.m_specialization)
				{
					Action<UniqueId> specializationUnlearned = this.SpecializationUnlearned;
					if (specializationUnlearned != null)
					{
						specializationUnlearned(this.m_specialization.Value);
					}
				}
				this.m_specialization = value;
				Action masteryDataChanged = this.MasteryDataChanged;
				if (masteryDataChanged != null)
				{
					masteryDataChanged();
				}
				if (!GameManager.IsServer)
				{
					if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
					{
						LocalPlayer.GameEntity.CollectionController.Masteries.InvokeContentsChanged();
					}
					return;
				}
				if (this.m_specialization != null && !this.m_specialization.Value.IsEmpty)
				{
					this.SpecializationLevel = ((ServerGameManager.GameServerConfig != null && (float)ServerGameManager.GameServerConfig.StartingLevel > 6f) ? ((float)ServerGameManager.GameServerConfig.StartingLevel) : 6f);
					return;
				}
				this.SpecializationLevel = 0f;
			}
		}

		// Token: 0x170013DD RID: 5085
		// (get) Token: 0x060055C3 RID: 21955 RVA: 0x00079340 File Offset: 0x00077540
		// (set) Token: 0x060055C4 RID: 21956 RVA: 0x00079348 File Offset: 0x00077548
		public float BaseLevel
		{
			get
			{
				return this.m_baseLevel;
			}
			set
			{
				this.m_baseLevel = Mathf.Clamp(value, 1f, 50f);
				Action levelDataChanged = this.LevelDataChanged;
				if (levelDataChanged == null)
				{
					return;
				}
				levelDataChanged();
			}
		}

		// Token: 0x170013DE RID: 5086
		// (get) Token: 0x060055C5 RID: 21957 RVA: 0x00079370 File Offset: 0x00077570
		// (set) Token: 0x060055C6 RID: 21958 RVA: 0x00079378 File Offset: 0x00077578
		public float SpecializationLevel
		{
			get
			{
				return this.m_specializationLevel;
			}
			set
			{
				this.m_specializationLevel = Mathf.Clamp(value, 1f, 50f);
				Action levelDataChanged = this.LevelDataChanged;
				if (levelDataChanged == null)
				{
					return;
				}
				levelDataChanged();
			}
		}

		// Token: 0x060055C7 RID: 21959 RVA: 0x000793A0 File Offset: 0x000775A0
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddFloat(this.m_baseLevel);
			buffer.AddFloat(this.m_specializationLevel);
			buffer.AddNullableUniqueId(this.Specialization);
			return buffer;
		}

		// Token: 0x060055C8 RID: 21960 RVA: 0x000793CA File Offset: 0x000775CA
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.m_baseLevel = buffer.ReadFloat();
			this.m_specializationLevel = buffer.ReadFloat();
			this.Specialization = buffer.ReadNullableUniqueId();
			return buffer;
		}

		// Token: 0x060055C9 RID: 21961 RVA: 0x000793F1 File Offset: 0x000775F1
		public void CopyDataFrom(MasteryInstanceData other)
		{
			this.m_baseLevel = other.m_baseLevel;
			this.m_specializationLevel = other.m_specializationLevel;
			this.m_specialization = other.m_specialization;
		}

		// Token: 0x04004C18 RID: 19480
		private UniqueId? m_specialization;

		// Token: 0x04004C19 RID: 19481
		private float m_baseLevel = 1f;

		// Token: 0x04004C1A RID: 19482
		private float m_specializationLevel;
	}
}
