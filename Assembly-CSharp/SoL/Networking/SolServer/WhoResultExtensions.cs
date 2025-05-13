using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Grouping;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003FA RID: 1018
	public static class WhoResultExtensions
	{
		// Token: 0x06001B12 RID: 6930 RVA: 0x0010A98C File Offset: 0x00108B8C
		public static void PrintResults(bool isTruncated, List<WhoResult> whoResults)
		{
			if (whoResults == null || whoResults.Count <= 0)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, "/who results:\n0 players found");
				return;
			}
			whoResults.Sort(new Comparison<WhoResult>(WhoResultExtensions.WhoResultComparison));
			string formattedZoneName = LocalPlayer.GetFormattedZoneName();
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendLine("/who results:");
				foreach (WhoResult whoResult in whoResults)
				{
					string text = "Unknown";
					string arg = whoResult.CharacterName[0].ToString().ToUpperInvariant();
					string text2 = ZString.Format<string, string>("{0}{1}", arg, whoResult.CharacterName.Substring(1));
					bool flag = SessionData.SelectedCharacter != null && text2.Equals(SessionData.SelectedCharacter.Name, StringComparison.InvariantCultureIgnoreCase);
					bool flag2 = !flag && ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsFriend(text2);
					GroupMember groupMember;
					bool flag3 = !flag && ClientGameManager.GroupManager && ClientGameManager.GroupManager.TryGetGroupMember(text2, out groupMember);
					bool flag4 = !flag && whoResult.Lfg;
					string text3 = SoL.Utilities.Extensions.TextMeshProExtensions.CreatePlayerLink(text2);
					if (flag || flag2)
					{
						Color color = flag ? Color.white : WhoResultExtensions.kSameColor;
						text3 = ZString.Format<string, string>("<color={0}><i>{1}</i></color>", color.ToHex(), text3);
					}
					if (flag3 || flag4)
					{
						string arg2 = flag3 ? "Group Member" : "Looking for Group";
						string arg3 = flag3 ? "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>" : "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
						text3 = ZString.Format<string, string, string, string, string>("<color={0}><link=\"{1}:{2}\">{3}</link></color> {4}", NameplateControllerUI.kGroupColor.ToHex(), "text", arg2, arg3, text3);
					}
					if (whoResult.ZoneId == -2)
					{
						utf16ValueStringBuilder.AppendFormat<string>("[Anonymous] {0}", text3);
					}
					else
					{
						ZoneId zoneId;
						if (whoResult.ZoneId >= 0 && ZoneIdExtensions.ZoneIdDict.TryGetValue(whoResult.ZoneId, out zoneId))
						{
							ZoneRecord zoneRecord = SessionData.GetZoneRecord(zoneId);
							if (zoneRecord != null)
							{
								text = LocalZoneManager.GetFormattedZoneName(zoneRecord.DisplayName, (SubZoneId)whoResult.SubZoneId);
							}
							if (text.Equals(formattedZoneName, StringComparison.CurrentCultureIgnoreCase))
							{
								text = ZString.Format<string, string>("<color={0}><i>{1}</i></color>", WhoResultExtensions.kSameColor.ToHex(), text);
							}
						}
						BaseArchetype roleArchetype = whoResult.RoleArchetype;
						string arg4 = roleArchetype ? roleArchetype.DisplayName : "Unknown";
						utf16ValueStringBuilder.AppendFormat<string, string>("[<link=\"{0}:{1}\">", "text", arg4);
						if (whoResult.Level <= 0)
						{
							utf16ValueStringBuilder.Append("??");
						}
						else if (whoResult.Level < 10)
						{
							utf16ValueStringBuilder.AppendFormat<byte>("<color=#00000000>0</color>{0}", whoResult.Level);
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<byte>("{0}", whoResult.Level);
						}
						if (roleArchetype)
						{
							utf16ValueStringBuilder.AppendFormat<string, string>(" <color={1}><sprite=\"RoleIcons\" name=\"{0}\" tint=1></color>", ((RolePacked)whoResult.Role).GetRolePackedSpriteName(), roleArchetype.IconTint.ToHex());
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string>(" <color=#00000000><sprite=\"RoleIcons\" name=\"{0}\" tint=1></color>", RolePacked.Striker.GetRolePackedSpriteName());
						}
						utf16ValueStringBuilder.Append("</link>]");
						utf16ValueStringBuilder.AppendFormat<string>(" {0}", text3);
						if (whoResult.GuildName != null)
						{
							if (ClientGameManager.SocialManager && ClientGameManager.SocialManager.Guild != null && ClientGameManager.SocialManager.Guild.Name.Equals(whoResult.GuildName, StringComparison.InvariantCultureIgnoreCase))
							{
								utf16ValueStringBuilder.AppendFormat<string, string>(" <color={0}><<i>{1}</i>></color>", WhoResultExtensions.kSameColor.ToHex(), whoResult.GuildName);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<string>(" <{0}>", whoResult.GuildName);
							}
						}
						utf16ValueStringBuilder.AppendFormat<string>(" ({0})", text);
					}
					utf16ValueStringBuilder.AppendLine();
				}
				if (!isTruncated)
				{
					utf16ValueStringBuilder.AppendFormat<string, string>("{0} {1} found", whoResults.Count.ToString(), (whoResults.Count == 1) ? "player" : "players");
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<string, string>("{0} {1} found (results have been truncated)", whoResults.Count.ToString(), (whoResults.Count == 1) ? "player" : "players");
				}
				MessageManager.ChatQueue.AddToQueue(MessageType.Social, utf16ValueStringBuilder.ToString());
			}
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x0005502E File Offset: 0x0005322E
		private static int WhoResultComparison(WhoResult x, WhoResult y)
		{
			if (x.ZoneId != y.ZoneId)
			{
				if (x.ZoneId == -2)
				{
					return -1;
				}
				if (y.ZoneId == -2)
				{
					return 1;
				}
			}
			return x.Level.CompareTo(y.Level);
		}

		// Token: 0x04002243 RID: 8771
		private const int kAnonymousZoneId = -2;

		// Token: 0x04002244 RID: 8772
		private const bool kTintRoleIcons = true;

		// Token: 0x04002245 RID: 8773
		public static Color kSameColor = GlobalSettings.Values.Nameplates.GroupColor;
	}
}
