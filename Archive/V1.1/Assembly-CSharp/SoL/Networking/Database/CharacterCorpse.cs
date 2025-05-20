using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoL.Networking.Database
{
	// Token: 0x02000438 RID: 1080
	[Serializable]
	public class CharacterCorpse
	{
		// Token: 0x04002441 RID: 9281
		[BsonDateTimeOptions(Representation = BsonType.String)]
		public DateTime TimeCreated;

		// Token: 0x04002442 RID: 9282
		public CharacterLocation Location;
	}
}
