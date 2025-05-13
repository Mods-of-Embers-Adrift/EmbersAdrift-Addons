using System;
using System.Collections.Generic;
using Cysharp.Text;
using NetStack.Serialization;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;

namespace SoL.Game.Loot
{
	// Token: 0x02000B0C RID: 2828
	public class LootRollItem : IPoolable, INetworkSerializable
	{
		// Token: 0x06005733 RID: 22323 RVA: 0x001E2CB0 File Offset: 0x001E0EB0
		private static void InitHexColors()
		{
			if (LootRollItem.m_hexColorsInitialized)
			{
				return;
			}
			LootRollItem.m_needHexColor = GlobalSettings.Values.Group.LootRollColorNeed.ToHex();
			LootRollItem.m_greedHexColor = GlobalSettings.Values.Group.LootRollColorGreed.ToHex();
			LootRollItem.m_passHexColor = GlobalSettings.Values.Group.LootRollColorPass.ToHex();
			LootRollItem.m_hexColorsInitialized = true;
		}

		// Token: 0x17001478 RID: 5240
		// (get) Token: 0x06005734 RID: 22324 RVA: 0x0007A189 File Offset: 0x00078389
		public LootRollStatus Status
		{
			get
			{
				return this.m_status;
			}
		}

		// Token: 0x17001479 RID: 5241
		// (get) Token: 0x06005735 RID: 22325 RVA: 0x0007A191 File Offset: 0x00078391
		public UniqueId Id
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x1700147A RID: 5242
		// (get) Token: 0x06005736 RID: 22326 RVA: 0x0007A199 File Offset: 0x00078399
		public ArchetypeInstance Instance
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x1700147B RID: 5243
		// (get) Token: 0x06005737 RID: 22327 RVA: 0x0007A1A1 File Offset: 0x000783A1
		public float ExpirationMultiplier
		{
			get
			{
				return this.m_expirationMultiplier;
			}
		}

		// Token: 0x06005738 RID: 22328 RVA: 0x001E2D18 File Offset: 0x001E0F18
		private string GetDisplayName()
		{
			if (string.IsNullOrEmpty(this.m_displayName) && this.m_instance != null && this.m_instance.Archetype)
			{
				this.m_displayName = this.m_instance.Archetype.GetModifiedDisplayName(this.m_instance);
			}
			if (!string.IsNullOrEmpty(this.m_displayName))
			{
				return this.m_displayName;
			}
			return "UNKNOWN";
		}

		// Token: 0x06005739 RID: 22329 RVA: 0x001E2D84 File Offset: 0x001E0F84
		public void Reset()
		{
			this.m_status = LootRollStatus.Pending;
			this.m_id = UniqueId.Empty;
			this.m_timestamp = DateTime.MinValue;
			this.m_expirationMultiplier = 1f;
			this.m_lootRollTimeout = 120f;
			this.m_instance = null;
			this.m_members.Clear();
			this.m_displayName = null;
		}

		// Token: 0x1700147C RID: 5244
		// (get) Token: 0x0600573A RID: 22330 RVA: 0x0007A1A9 File Offset: 0x000783A9
		// (set) Token: 0x0600573B RID: 22331 RVA: 0x0007A1B1 File Offset: 0x000783B1
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x0600573C RID: 22332 RVA: 0x001E2DE0 File Offset: 0x001E0FE0
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.m_status);
			buffer.AddUniqueId(this.m_id);
			buffer.AddFloat(this.m_expirationMultiplier);
			this.m_instance.PackData(buffer);
			buffer.AddInt(this.m_members.Count);
			for (int i = 0; i < this.m_members.Count; i++)
			{
				this.m_members[i].PackData(buffer);
			}
			return buffer;
		}

		// Token: 0x0600573D RID: 22333 RVA: 0x001E2E60 File Offset: 0x001E1060
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.m_status = buffer.ReadEnum<LootRollStatus>();
			this.m_id = buffer.ReadUniqueId();
			this.m_expirationMultiplier = buffer.ReadFloat();
			this.m_instance = StaticPool<ArchetypeInstance>.GetFromPool();
			this.m_instance.ReadData(buffer);
			int num = buffer.ReadInt();
			if (this.m_members == null)
			{
				this.m_members = new List<LootRollMember>(num);
			}
			else
			{
				this.m_members.Clear();
			}
			for (int i = 0; i < num; i++)
			{
				LootRollMember item = default(LootRollMember);
				item.ReadData(buffer);
				this.m_members.Add(item);
			}
			return buffer;
		}

		// Token: 0x0600573E RID: 22334 RVA: 0x001E2EFC File Offset: 0x001E10FC
		public void PrintResults()
		{
			if (LootRollItem.m_needRolls == null)
			{
				LootRollItem.m_needRolls = new List<LootRollMember>(6);
				LootRollItem.m_greedRolls = new List<LootRollMember>(6);
				LootRollItem.m_passRolls = new List<LootRollMember>(6);
			}
			LootRollItem.m_needRolls.Clear();
			LootRollItem.m_greedRolls.Clear();
			LootRollItem.m_passRolls.Clear();
			LootRollItem.InitHexColors();
			LootRollMember? lootRollMember = null;
			LootRollMember? lootRollMember2 = null;
			for (int i = 0; i < this.m_members.Count; i++)
			{
				LootRollMember lootRollMember3 = this.m_members[i];
				LootRollChoice choice = lootRollMember3.Choice;
				if (choice != LootRollChoice.Need)
				{
					if (choice != LootRollChoice.Greed)
					{
						LootRollItem.m_passRolls.Add(lootRollMember3);
					}
					else
					{
						if (lootRollMember2 == null)
						{
							lootRollMember2 = new LootRollMember?(lootRollMember3);
						}
						else if (lootRollMember3.Roll != null)
						{
							LootRollMember value = lootRollMember2.Value;
							if (value.Roll != null)
							{
								int value2 = lootRollMember3.Roll.Value;
								value = lootRollMember2.Value;
								if (value2 > value.Roll.Value)
								{
									lootRollMember2 = new LootRollMember?(lootRollMember3);
								}
							}
						}
						LootRollItem.m_greedRolls.Add(lootRollMember3);
					}
				}
				else
				{
					if (lootRollMember == null)
					{
						lootRollMember = new LootRollMember?(lootRollMember3);
					}
					else if (lootRollMember3.Roll != null)
					{
						LootRollMember value = lootRollMember.Value;
						if (value.Roll != null)
						{
							int value3 = lootRollMember3.Roll.Value;
							value = lootRollMember.Value;
							if (value3 > value.Roll.Value)
							{
								lootRollMember = new LootRollMember?(lootRollMember3);
							}
						}
					}
					LootRollItem.m_needRolls.Add(lootRollMember3);
				}
			}
			LootRollItem.m_needRolls.Sort(new Comparison<LootRollMember>(this.LootRollComparison));
			LootRollItem.m_greedRolls.Sort(new Comparison<LootRollMember>(this.LootRollComparison));
			LootRollItem.m_passRolls.Sort(new Comparison<LootRollMember>(this.PlayerNameComparison));
			LootRollMember? winner = null;
			if (lootRollMember != null)
			{
				winner = new LootRollMember?(lootRollMember.Value);
			}
			else if (lootRollMember2 != null)
			{
				winner = new LootRollMember?(lootRollMember2.Value);
			}
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				string text = SoL.Utilities.Extensions.TextMeshProExtensions.CreateInstanceLink(this.m_instance);
				utf16ValueStringBuilder.AppendFormat<string>("Loot Roll Results for: {0}\n", text);
				MessageManager.AddLinkedInstance(this.m_instance, false);
				if (LootRollItem.m_needRolls.Count > 0)
				{
					utf16ValueStringBuilder.Append(this.GetSortedLootResultsForList(LootRollItem.m_needRolls, winner));
				}
				if (LootRollItem.m_greedRolls.Count > 0)
				{
					if (LootRollItem.m_needRolls.Count > 0)
					{
						utf16ValueStringBuilder.AppendLine();
					}
					utf16ValueStringBuilder.Append(this.GetSortedLootResultsForList(LootRollItem.m_greedRolls, winner));
				}
				if (LootRollItem.m_passRolls.Count > 0)
				{
					if (LootRollItem.m_needRolls.Count > 0 || LootRollItem.m_greedRolls.Count > 0)
					{
						utf16ValueStringBuilder.AppendLine();
					}
					utf16ValueStringBuilder.Append(this.GetSortedLootResultsForList(LootRollItem.m_passRolls, winner));
				}
				string content = (LootRollItem.m_passRolls.Count >= this.m_members.Count) ? ("Everyone passed on " + text) : utf16ValueStringBuilder.ToString();
				MessageManager.ChatQueue.AddToQueue(MessageType.Loot, content);
			}
		}

		// Token: 0x0600573F RID: 22335 RVA: 0x001E3248 File Offset: 0x001E1448
		private string GetSortedLootResultsForList(List<LootRollMember> members, LootRollMember? winner)
		{
			string result = string.Empty;
			if (members != null && members.Count > 0)
			{
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					for (int i = 0; i < members.Count; i++)
					{
						LootRollMember lootRollMember = members[i];
						bool flag = winner != null && winner.Value == lootRollMember;
						switch (lootRollMember.Choice)
						{
						case LootRollChoice.Pass:
							utf16ValueStringBuilder.AppendFormat<string, string>("     <color={0}>{1}</color>   selected by:", LootRollItem.m_passHexColor, "<sprite=\"SolIcons\" name=\"PassIcon\" tint=1>");
							break;
						case LootRollChoice.Need:
							utf16ValueStringBuilder.AppendFormat<string, string, int>("     <color={0}>{1}</color> {2} rolled by:", LootRollItem.m_needHexColor, "<sprite=\"SolIcons\" name=\"NeedIcon\" tint=1>", lootRollMember.Roll.GetValueOrDefault());
							break;
						case LootRollChoice.Greed:
							utf16ValueStringBuilder.AppendFormat<string, string, int>("     <color={0}>{1}</color> {2} rolled by:", LootRollItem.m_greedHexColor, "<sprite=\"SolIcons\" name=\"GreedIcon\" tint=1>", lootRollMember.Roll.GetValueOrDefault());
							break;
						}
						utf16ValueStringBuilder.Append(" <i>" + SoL.Utilities.Extensions.TextMeshProExtensions.CreatePlayerLink(lootRollMember.PlayerName) + "</i>");
						if (flag)
						{
							utf16ValueStringBuilder.Append(" <b>[WINNER]</b>");
						}
						if (i < members.Count - 1)
						{
							utf16ValueStringBuilder.AppendLine();
						}
					}
					result = utf16ValueStringBuilder.ToString();
				}
			}
			return result;
		}

		// Token: 0x06005740 RID: 22336 RVA: 0x001E33A8 File Offset: 0x001E15A8
		private int LootRollComparison(LootRollMember x, LootRollMember y)
		{
			if (x.Roll != null && y.Roll != null)
			{
				int num = y.Roll.Value.CompareTo(x.Roll.Value);
				if (num != 0)
				{
					return num;
				}
				return this.PlayerNameComparison(x, y);
			}
			else
			{
				if (x.Roll != null)
				{
					return -1;
				}
				if (y.Roll != null)
				{
					return 1;
				}
				return this.PlayerNameComparison(x, y);
			}
		}

		// Token: 0x06005741 RID: 22337 RVA: 0x0007A1BA File Offset: 0x000783BA
		private int PlayerNameComparison(LootRollMember x, LootRollMember y)
		{
			return string.Compare(x.PlayerName, y.PlayerName, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x04004CF4 RID: 19700
		private const float kDefaultLootRollTimeout = 120f;

		// Token: 0x04004CF5 RID: 19701
		private const float kServerLootRollForceTime = 60f;

		// Token: 0x04004CF6 RID: 19702
		private static bool m_hexColorsInitialized = false;

		// Token: 0x04004CF7 RID: 19703
		private static string m_needHexColor = string.Empty;

		// Token: 0x04004CF8 RID: 19704
		private static string m_greedHexColor = string.Empty;

		// Token: 0x04004CF9 RID: 19705
		private static string m_passHexColor = string.Empty;

		// Token: 0x04004CFA RID: 19706
		private bool m_inPool;

		// Token: 0x04004CFB RID: 19707
		private LootRollStatus m_status;

		// Token: 0x04004CFC RID: 19708
		private UniqueId m_id = UniqueId.Empty;

		// Token: 0x04004CFD RID: 19709
		private DateTime m_timestamp = DateTime.MinValue;

		// Token: 0x04004CFE RID: 19710
		private float m_expirationMultiplier = 1f;

		// Token: 0x04004CFF RID: 19711
		private float m_lootRollTimeout = 120f;

		// Token: 0x04004D00 RID: 19712
		private ArchetypeInstance m_instance;

		// Token: 0x04004D01 RID: 19713
		private List<LootRollMember> m_members;

		// Token: 0x04004D02 RID: 19714
		private ILootRollSource m_lootRollSource;

		// Token: 0x04004D03 RID: 19715
		private string m_displayName;

		// Token: 0x04004D04 RID: 19716
		private static List<LootRollMember> m_needRolls = null;

		// Token: 0x04004D05 RID: 19717
		private static List<LootRollMember> m_greedRolls = null;

		// Token: 0x04004D06 RID: 19718
		private static List<LootRollMember> m_passRolls = null;
	}
}
