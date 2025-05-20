using System;

namespace SoL.Networking.Database
{
	// Token: 0x02000466 RID: 1126
	public static class PresenceExtensions
	{
		// Token: 0x06001FA6 RID: 8102 RVA: 0x00057378 File Offset: 0x00055578
		public static PresenceFlags ToFlags(this Presence value)
		{
			switch (value)
			{
			case Presence.Online:
				return PresenceFlags.Online;
			case Presence.Away:
				return PresenceFlags.AwayUserSet;
			case Presence.DoNotDisturb:
				return PresenceFlags.DoNotDisturb;
			case Presence.Anonymous:
				return PresenceFlags.Anonymous;
			case Presence.Invisible:
				return PresenceFlags.Invisible;
			default:
				return PresenceFlags.Invalid;
			}
		}

		// Token: 0x06001FA7 RID: 8103 RVA: 0x000573A3 File Offset: 0x000555A3
		public static Presence ExplicitPresenceFromFlags(this PresenceFlags value)
		{
			if ((value & PresenceFlags.Invisible) != PresenceFlags.Invalid)
			{
				return Presence.Invisible;
			}
			if ((value & PresenceFlags.Anonymous) != PresenceFlags.Invalid)
			{
				return Presence.Anonymous;
			}
			if ((value & PresenceFlags.DoNotDisturb) != PresenceFlags.Invalid)
			{
				return Presence.DoNotDisturb;
			}
			if ((value & PresenceFlags.AwayUserSet) != PresenceFlags.Invalid)
			{
				return Presence.Away;
			}
			return Presence.Online;
		}

		// Token: 0x06001FA8 RID: 8104 RVA: 0x000573C4 File Offset: 0x000555C4
		public static string ToStringAbbreviation(this PresenceFlags value)
		{
			if ((value & (PresenceFlags.AwayUserSet | PresenceFlags.AwayAutomatic)) != PresenceFlags.Invalid)
			{
				return "AFK";
			}
			if ((value & PresenceFlags.DoNotDisturb) != PresenceFlags.Invalid)
			{
				return "DnD";
			}
			if ((value & PresenceFlags.Anonymous) != PresenceFlags.Invalid)
			{
				return "Anon";
			}
			return string.Empty;
		}

		// Token: 0x06001FA9 RID: 8105 RVA: 0x000573ED File Offset: 0x000555ED
		public static bool IsAFK(this PresenceFlags value)
		{
			return (value & (PresenceFlags.AwayUserSet | PresenceFlags.AwayAutomatic)) > PresenceFlags.Invalid;
		}

		// Token: 0x06001FAA RID: 8106 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this PresenceFlags a, PresenceFlags b)
		{
			return (a & b) == b;
		}
	}
}
