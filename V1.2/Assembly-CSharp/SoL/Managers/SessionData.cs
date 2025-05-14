using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.GM;
using SoL.Networking.Database;
using SoL.Utilities;

namespace SoL.Managers
{
	// Token: 0x0200050E RID: 1294
	public static class SessionData
	{
		// Token: 0x14000048 RID: 72
		// (add) Token: 0x06002517 RID: 9495 RVA: 0x0012FC74 File Offset: 0x0012DE74
		// (remove) Token: 0x06002518 RID: 9496 RVA: 0x0012FCA8 File Offset: 0x0012DEA8
		public static event Action SelectedCharacterChanged;

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06002519 RID: 9497 RVA: 0x0012FCDC File Offset: 0x0012DEDC
		// (remove) Token: 0x0600251A RID: 9498 RVA: 0x0012FD10 File Offset: 0x0012DF10
		public static event Action SessionDataCleared;

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x0600251B RID: 9499 RVA: 0x0012FD44 File Offset: 0x0012DF44
		// (remove) Token: 0x0600251C RID: 9500 RVA: 0x0012FD78 File Offset: 0x0012DF78
		public static event Action UserRecordUpdated;

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x0600251D RID: 9501 RVA: 0x0005A838 File Offset: 0x00058A38
		// (set) Token: 0x0600251E RID: 9502 RVA: 0x0012FDAC File Offset: 0x0012DFAC
		public static UserRecord User
		{
			get
			{
				return SessionData.m_user;
			}
			set
			{
				bool flag = SessionData.m_user != value;
				if (flag && SessionData.m_user != null && value != null)
				{
					flag = (SessionData.m_user.Id != value.Id);
				}
				SessionData.m_user = value;
				if (flag)
				{
					if (SessionData.m_user != null)
					{
						POICommands.RegisterPOICommands();
					}
					else
					{
						POICommands.UnregisterPOICommands();
					}
				}
				Action userRecordUpdated = SessionData.UserRecordUpdated;
				if (userRecordUpdated == null)
				{
					return;
				}
				userRecordUpdated();
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x0600251F RID: 9503 RVA: 0x0005A83F File Offset: 0x00058A3F
		// (set) Token: 0x06002520 RID: 9504 RVA: 0x0005A846 File Offset: 0x00058A46
		public static string SubscriptionStatus { get; set; }

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06002521 RID: 9505 RVA: 0x0005A84E File Offset: 0x00058A4E
		// (set) Token: 0x06002522 RID: 9506 RVA: 0x0005A855 File Offset: 0x00058A55
		public static DateTime? SubscriptionExpires { get; set; }

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06002523 RID: 9507 RVA: 0x0005A85D File Offset: 0x00058A5D
		// (set) Token: 0x06002524 RID: 9508 RVA: 0x0005A864 File Offset: 0x00058A64
		public static CharacterRecord[] Characters
		{
			get
			{
				return SessionData.m_characters;
			}
			set
			{
				SessionData.m_characters = value;
				SessionData.SortCharacters();
			}
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x0012FE14 File Offset: 0x0012E014
		public static void SortCharacters()
		{
			if (SessionData.m_characters == null)
			{
				return;
			}
			List<CharacterRecord> fromPool = StaticListPool<CharacterRecord>.GetFromPool();
			List<CharacterRecord> fromPool2 = StaticListPool<CharacterRecord>.GetFromPool();
			List<CharacterRecord> fromPool3 = StaticListPool<CharacterRecord>.GetFromPool();
			for (int i = 0; i < SessionData.m_characters.Length; i++)
			{
				if (SessionData.m_characters[i] != null)
				{
					bool flag = SessionData.CharacterIsActive(SessionData.m_characters[i]);
					bool flag2 = SessionData.m_characters[i].SelectionPositionIndex >= 0;
					if (flag && flag2)
					{
						fromPool.Add(SessionData.m_characters[i]);
					}
					else if (flag)
					{
						fromPool2.Add(SessionData.m_characters[i]);
					}
					else
					{
						fromPool3.Add(SessionData.m_characters[i]);
					}
				}
			}
			fromPool.Sort((CharacterRecord a, CharacterRecord b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
			fromPool2.Sort((CharacterRecord a, CharacterRecord b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
			fromPool3.Sort((CharacterRecord a, CharacterRecord b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
			fromPool.AddRange(fromPool2);
			fromPool.AddRange(fromPool3);
			SessionData.m_characters = fromPool.ToArray();
			StaticListPool<CharacterRecord>.ReturnToPool(fromPool);
			StaticListPool<CharacterRecord>.ReturnToPool(fromPool2);
			StaticListPool<CharacterRecord>.ReturnToPool(fromPool3);
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06002526 RID: 9510 RVA: 0x0005A871 File Offset: 0x00058A71
		public static bool IsSubscriber
		{
			get
			{
				return SessionData.User != null && SessionData.User.IsSubscriber();
			}
		}

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06002527 RID: 9511 RVA: 0x0005A886 File Offset: 0x00058A86
		public static bool IsTrial
		{
			get
			{
				return SessionData.User == null || SessionData.User.IsTrial();
			}
		}

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06002528 RID: 9512 RVA: 0x0005A89B File Offset: 0x00058A9B
		// (set) Token: 0x06002529 RID: 9513 RVA: 0x0005A8A2 File Offset: 0x00058AA2
		public static CharacterRecord SelectedCharacter
		{
			get
			{
				return SessionData.m_selectedCharacter;
			}
			private set
			{
				if (SessionData.m_selectedCharacter != value)
				{
					SessionData.m_selectedCharacter = value;
					Action selectedCharacterChanged = SessionData.SelectedCharacterChanged;
					if (selectedCharacterChanged == null)
					{
						return;
					}
					selectedCharacterChanged();
				}
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x0600252A RID: 9514 RVA: 0x0005A8C1 File Offset: 0x00058AC1
		public static int MaxCharacters
		{
			get
			{
				return UserRecord.GetMaxCharacterCount(SessionData.User);
			}
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x0012FF48 File Offset: 0x0012E148
		public static bool CharacterCanBeSetAsActive(CharacterRecord record)
		{
			if (record == null || SessionData.User == null || SessionData.User.IsSubscriber())
			{
				return false;
			}
			if (SessionData.User.ActiveCharacters == null)
			{
				return true;
			}
			if (SessionData.User.ActiveCharacters.Length >= 2)
			{
				return false;
			}
			for (int i = 0; i < SessionData.User.ActiveCharacters.Length; i++)
			{
				if (SessionData.User.ActiveCharacters[i] == record.Id)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x0012FFC0 File Offset: 0x0012E1C0
		public static bool CharacterIsActive(CharacterRecord record)
		{
			if (record == null || SessionData.User == null)
			{
				return false;
			}
			if (SessionData.User.IsSubscriber())
			{
				return true;
			}
			if (SessionData.User.ActiveCharacters == null)
			{
				return false;
			}
			for (int i = 0; i < SessionData.User.ActiveCharacters.Length; i++)
			{
				if (SessionData.User.ActiveCharacters[i] == record.Id)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x00130028 File Offset: 0x0012E228
		public static void Clear()
		{
			Action sessionDataCleared = SessionData.SessionDataCleared;
			if (sessionDataCleared != null)
			{
				sessionDataCleared();
			}
			SessionData.SessionKey = null;
			SessionData.World = null;
			SessionData.User = null;
			SessionData.Characters = null;
			SessionData.SelectedCharacter = null;
			SessionData.ReturnToCharacterSelection = false;
			SessionData.SelectLastCharacter = false;
			SessionData.LastCreatedEditedCharacterId = null;
			SessionData.Zones = null;
			SessionData.m_zones = null;
			SessionData.SubscriptionStatus = null;
			SessionData.SubscriptionExpires = null;
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x00130098 File Offset: 0x0012E298
		public static void SetZones(ZoneRecord[] zones)
		{
			SessionData.Zones = zones;
			SessionData.m_zones = new Dictionary<ZoneId, ZoneRecord>();
			for (int i = 0; i < zones.Length; i++)
			{
				SessionData.m_zones.Add((ZoneId)zones[i].ZoneId, zones[i]);
			}
		}

		// Token: 0x0600252F RID: 9519 RVA: 0x001300D8 File Offset: 0x0012E2D8
		public static ZoneRecord GetZoneRecord(ZoneId zoneId)
		{
			ZoneRecord result = null;
			Dictionary<ZoneId, ZoneRecord> zones = SessionData.m_zones;
			if (zones != null)
			{
				zones.TryGetValue(zoneId, out result);
			}
			return result;
		}

		// Token: 0x06002530 RID: 9520 RVA: 0x0005A8CD File Offset: 0x00058ACD
		public static bool TryGetZoneRecord(ZoneId zoneId, out ZoneRecord record)
		{
			record = SessionData.GetZoneRecord(zoneId);
			return record != null;
		}

		// Token: 0x06002531 RID: 9521 RVA: 0x0005A8DC File Offset: 0x00058ADC
		public static string GetSelectedCharacterId()
		{
			if (SessionData.SelectedCharacter == null)
			{
				throw new ArgumentException("Invalid character selected!");
			}
			return SessionData.SelectedCharacter.Id;
		}

		// Token: 0x06002532 RID: 9522 RVA: 0x0005A8FA File Offset: 0x00058AFA
		public static void SelectCharacter(int index)
		{
			if (index == -1)
			{
				SessionData.SelectedCharacter = null;
				return;
			}
			if (index >= SessionData.Characters.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			SessionData.SelectedCharacter = SessionData.Characters[index];
		}

		// Token: 0x06002533 RID: 9523 RVA: 0x0005A928 File Offset: 0x00058B28
		public static void SelectCharacter(CharacterRecord record)
		{
			SessionData.SelectedCharacter = record;
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x001300FC File Offset: 0x0012E2FC
		public static bool IsMyCharacter(string characterId)
		{
			if (SessionData.Characters != null && !string.IsNullOrEmpty(characterId))
			{
				for (int i = 0; i < SessionData.Characters.Length; i++)
				{
					if (SessionData.Characters[i] != null && SessionData.Characters[i].Id == characterId)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x040027C4 RID: 10180
		public static string SessionKey;

		// Token: 0x040027C5 RID: 10181
		public static WorldRecord World;

		// Token: 0x040027C6 RID: 10182
		private static UserRecord m_user;

		// Token: 0x040027C9 RID: 10185
		private static CharacterRecord[] m_characters;

		// Token: 0x040027CA RID: 10186
		public static bool ReturnToCharacterSelection;

		// Token: 0x040027CB RID: 10187
		public static bool SelectLastCharacter;

		// Token: 0x040027CC RID: 10188
		public static string LastCreatedEditedCharacterId;

		// Token: 0x040027CD RID: 10189
		private static ZoneRecord[] Zones;

		// Token: 0x040027CE RID: 10190
		private static Dictionary<ZoneId, ZoneRecord> m_zones;

		// Token: 0x040027CF RID: 10191
		private static CharacterRecord m_selectedCharacter;
	}
}
