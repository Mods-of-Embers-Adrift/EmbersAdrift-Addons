using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DC2 RID: 3522
	public class TestSyncVarAlloc : MonoBehaviour
	{
		// Token: 0x06006931 RID: 26929 RVA: 0x00216EF4 File Offset: 0x002150F4
		private void Start()
		{
			if (this.randomize)
			{
				this.m_bool.Value = true;
				this.m_byte.Value = 1;
				this.m_nullableByte.Value = new byte?((byte)2);
				this.m_int.Value = UnityEngine.Random.Range(0, int.MaxValue);
				this.m_nullableInt.Value = new int?(UnityEngine.Random.Range(0, int.MaxValue));
				this.m_string.Value = "hi there how are you";
				this.m_id.Value = new UniqueId("test test test");
				this.m_color.Value = Color.cyan;
				this.m_dateTime.Value = DateTime.UtcNow;
				this.m_charVisuals.Value = new CharacterVisuals
				{
					Sex = CharacterSex.Female,
					Dna = new Dictionary<string, float>(100)
					{
						{
							"test1",
							0.8f
						},
						{
							"test2",
							0.8f
						},
						{
							"test3",
							0.8f
						}
					},
					BuildType = CharacterBuildType.Rotund,
					CustomizedSlots = new List<UniqueId>
					{
						new UniqueId("AAAA"),
						new UniqueId("BBBB"),
						new UniqueId("CCC")
					}
				};
				this.m_playerFlags.Value = (PlayerFlags.InCampfire | PlayerFlags.Invisible);
				this.m_corpseData.Value = new CorpseData
				{
					CharacterId = "yayayayhA",
					CharacterName = "weeE",
					TimeCreated = DateTime.UtcNow
				};
				this.m_location.Value = new CharacterLocation
				{
					h = 1f,
					x = 2f,
					y = 3f,
					z = 4f,
					ZoneId = 2
				};
			}
		}

		// Token: 0x06006932 RID: 26930 RVA: 0x002170D0 File Offset: 0x002152D0
		private void Update()
		{
			for (int i = 0; i < this.cycles; i++)
			{
				bool isDefault = this.m_location.IsDefault;
			}
		}

		// Token: 0x04005B89 RID: 23433
		public bool randomize;

		// Token: 0x04005B8A RID: 23434
		public int cycles = 100;

		// Token: 0x04005B8B RID: 23435
		public readonly SynchronizedBool m_bool = new SynchronizedBool();

		// Token: 0x04005B8C RID: 23436
		public readonly SynchronizedByte m_byte = new SynchronizedByte();

		// Token: 0x04005B8D RID: 23437
		public readonly SynchronizedNullableByte m_nullableByte = new SynchronizedNullableByte();

		// Token: 0x04005B8E RID: 23438
		public readonly SynchronizedInt m_int = new SynchronizedInt();

		// Token: 0x04005B8F RID: 23439
		public readonly SynchronizedNullableInt m_nullableInt = new SynchronizedNullableInt();

		// Token: 0x04005B90 RID: 23440
		public readonly SynchronizedString m_string = new SynchronizedString();

		// Token: 0x04005B91 RID: 23441
		public readonly SynchronizedUniqueId m_id = new SynchronizedUniqueId();

		// Token: 0x04005B92 RID: 23442
		public readonly SynchronizedColor m_color = new SynchronizedColor();

		// Token: 0x04005B93 RID: 23443
		public readonly SynchronizedDateTime m_dateTime = new SynchronizedDateTime();

		// Token: 0x04005B94 RID: 23444
		public readonly SynchronizedCharacterVisuals m_charVisuals = new SynchronizedCharacterVisuals();

		// Token: 0x04005B95 RID: 23445
		public readonly SynchronizedEnum<PlayerFlags> m_playerFlags = new SynchronizedEnum<PlayerFlags>();

		// Token: 0x04005B96 RID: 23446
		public readonly SynchronizedStruct<CorpseData> m_corpseData = new SynchronizedStruct<CorpseData>();

		// Token: 0x04005B97 RID: 23447
		public readonly SynchronizedLocation m_location = new SynchronizedLocation();
	}
}
