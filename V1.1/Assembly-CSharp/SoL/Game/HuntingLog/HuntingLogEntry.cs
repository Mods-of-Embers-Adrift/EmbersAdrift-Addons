using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Networking;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BC7 RID: 3015
	[Serializable]
	public class HuntingLogEntry : INetworkSerializable
	{
		// Token: 0x06005D2C RID: 23852 RVA: 0x001F2D5C File Offset: 0x001F0F5C
		public HuntingLogProfile GetProfile()
		{
			HuntingLogProfile profile;
			if (this.m_profile == null && !this.ProfileId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(this.ProfileId, out profile))
			{
				this.m_profile = profile;
			}
			return this.m_profile;
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x0007EAB3 File Offset: 0x0007CCB3
		public bool TryGetProfile(out HuntingLogProfile profile)
		{
			profile = this.GetProfile();
			return profile != null;
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001F2DA8 File Offset: 0x001F0FA8
		public void IncrementCount()
		{
			this.TotalCount += 1UL;
			HuntingLogProfile huntingLogProfile;
			if (this.TryGetProfile(out huntingLogProfile) && huntingLogProfile.Settings && this.PerkCount < huntingLogProfile.Settings.MaxPerkCount)
			{
				this.PerkCount++;
			}
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x001F2DFC File Offset: 0x001F0FFC
		public int GetTallyForPerk(HuntingLogPerkType perkType)
		{
			if (perkType.IsTitle())
			{
				return 0;
			}
			if (this.m_tallyCache == null)
			{
				this.CacheCombatPerks();
			}
			if (this.m_tallyCache == null || (int)perkType >= this.m_tallyCache.Length)
			{
				return 0;
			}
			return this.m_tallyCache[(int)perkType];
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x001F2E40 File Offset: 0x001F1040
		public void CacheCombatPerks()
		{
			if (this.m_tallyCache == null)
			{
				this.m_tallyCache = new int[43];
			}
			for (int i = 0; i < this.m_tallyCache.Length; i++)
			{
				this.m_tallyCache[i] = 0;
			}
			if (this.ActivePerks != null)
			{
				foreach (KeyValuePair<int, HuntingLogPerkType> keyValuePair in this.ActivePerks)
				{
					int value = (int)keyValuePair.Value;
					this.m_tallyCache[value]++;
				}
			}
		}

		// Token: 0x06005D31 RID: 23857 RVA: 0x001F2EE0 File Offset: 0x001F10E0
		public bool TryGetPerksForStat(StatType statType, out int perkValue)
		{
			perkValue = 0;
			HuntingLogPerkType? huntingLogPerkType = statType.GetHuntingLogPerkType();
			int num;
			if (huntingLogPerkType != null && this.TryGetPerkValue(huntingLogPerkType.Value, out num))
			{
				perkValue = num;
			}
			return perkValue != 0;
		}

		// Token: 0x06005D32 RID: 23858 RVA: 0x001F2F1C File Offset: 0x001F111C
		public bool TryGetPerkValue(HuntingLogPerkType perkType, out int perkValue)
		{
			perkValue = 0;
			if (this.m_tallyCache == null)
			{
				this.CacheCombatPerks();
			}
			HuntingLogProfile huntingLogProfile;
			if (this.m_tallyCache != null && this.TryGetProfile(out huntingLogProfile) && huntingLogProfile.Settings)
			{
				int num = this.m_tallyCache[(int)perkType];
				perkValue = num * huntingLogProfile.Settings.GetPerkMultiplier(perkType);
			}
			return perkValue != 0;
		}

		// Token: 0x06005D33 RID: 23859 RVA: 0x001F2F78 File Offset: 0x001F1178
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ProfileId);
			buffer.AddULong(this.TotalCount);
			buffer.AddInt(this.PerkCount);
			if (this.ActivePerks == null)
			{
				buffer.AddInt(0);
			}
			else
			{
				int num = (this.ActivePerks != null) ? this.ActivePerks.Count : 0;
				buffer.AddInt(num);
				if (num > 0)
				{
					foreach (KeyValuePair<int, HuntingLogPerkType> keyValuePair in this.ActivePerks)
					{
						buffer.AddInt(keyValuePair.Key);
						buffer.AddEnum(keyValuePair.Value);
					}
				}
			}
			return buffer;
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x001F3040 File Offset: 0x001F1240
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ProfileId = buffer.ReadUniqueId();
			this.TotalCount = buffer.ReadULong();
			this.PerkCount = buffer.ReadInt();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.ActivePerks = new Dictionary<int, HuntingLogPerkType>(num);
				for (int i = 0; i < num; i++)
				{
					int key = buffer.ReadInt();
					HuntingLogPerkType value = buffer.ReadEnum<HuntingLogPerkType>();
					this.ActivePerks.Add(key, value);
				}
			}
			return buffer;
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x0007EAC5 File Offset: 0x0007CCC5
		public void ResetReferences()
		{
			this.m_profile = null;
		}

		// Token: 0x04005094 RID: 20628
		public UniqueId ProfileId;

		// Token: 0x04005095 RID: 20629
		[BsonIgnore]
		[JsonIgnore]
		[NonSerialized]
		private HuntingLogProfile m_profile;

		// Token: 0x04005096 RID: 20630
		public int Version;

		// Token: 0x04005097 RID: 20631
		public int SettingsVersion;

		// Token: 0x04005098 RID: 20632
		public ulong TotalCount;

		// Token: 0x04005099 RID: 20633
		public int PerkCount;

		// Token: 0x0400509A RID: 20634
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<int, HuntingLogPerkType> ActivePerks;

		// Token: 0x0400509B RID: 20635
		[BsonIgnore]
		[JsonIgnore]
		[NonSerialized]
		private int[] m_tallyCache;
	}
}
