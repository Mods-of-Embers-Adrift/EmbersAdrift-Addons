using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SoL.Game;

namespace SoL.Networking.Database
{
	// Token: 0x02000467 RID: 1127
	public class Relation
	{
		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06001FAB RID: 8107 RVA: 0x000573F5 File Offset: 0x000555F5
		// (set) Token: 0x06001FAC RID: 8108 RVA: 0x000573FD File Offset: 0x000555FD
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06001FAD RID: 8109 RVA: 0x00057406 File Offset: 0x00055606
		// (set) Token: 0x06001FAE RID: 8110 RVA: 0x0005740E File Offset: 0x0005560E
		public string Source { get; set; }

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06001FAF RID: 8111 RVA: 0x00057417 File Offset: 0x00055617
		// (set) Token: 0x06001FB0 RID: 8112 RVA: 0x0005741F File Offset: 0x0005561F
		public string Target { get; set; }

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06001FB1 RID: 8113 RVA: 0x00057428 File Offset: 0x00055628
		// (set) Token: 0x06001FB2 RID: 8114 RVA: 0x00057430 File Offset: 0x00055630
		public RelationType Type { get; set; }

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06001FB3 RID: 8115 RVA: 0x00057439 File Offset: 0x00055639
		// (set) Token: 0x06001FB4 RID: 8116 RVA: 0x00057441 File Offset: 0x00055641
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime Created { get; set; }

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06001FB5 RID: 8117 RVA: 0x001203E4 File Offset: 0x0011E5E4
		// (set) Token: 0x06001FB6 RID: 8118 RVA: 0x00120440 File Offset: 0x0011E640
		[BsonIgnore]
		[JsonIgnore]
		public string OtherName
		{
			get
			{
				if (LocalPlayer.GameEntity && this.m_otherName == null)
				{
					this.m_otherName = ((this.Source == LocalPlayer.GameEntity.CharacterData.Name) ? this.Target : this.Source);
				}
				return this.m_otherName;
			}
			set
			{
				if (LocalPlayer.GameEntity)
				{
					if (this.Source == LocalPlayer.GameEntity.CharacterData.Name)
					{
						this.Target = value;
					}
					else
					{
						this.Source = value;
					}
				}
				this.m_otherName = value;
			}
		}

		// Token: 0x0400251A RID: 9498
		private string m_otherName;
	}
}
