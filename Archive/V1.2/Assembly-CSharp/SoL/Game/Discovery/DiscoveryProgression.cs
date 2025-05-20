using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Networking.Database;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA7 RID: 3239
	public static class DiscoveryProgression
	{
		// Token: 0x14000122 RID: 290
		// (add) Token: 0x06006229 RID: 25129 RVA: 0x002039FC File Offset: 0x00201BFC
		// (remove) Token: 0x0600622A RID: 25130 RVA: 0x00203A30 File Offset: 0x00201C30
		public static event Action<DiscoveryProfile> DiscoveryFound;

		// Token: 0x14000123 RID: 291
		// (add) Token: 0x0600622B RID: 25131 RVA: 0x00203A64 File Offset: 0x00201C64
		// (remove) Token: 0x0600622C RID: 25132 RVA: 0x00203A98 File Offset: 0x00201C98
		public static event Action DiscoveryRefresh;

		// Token: 0x0600622D RID: 25133 RVA: 0x00203ACC File Offset: 0x00201CCC
		public static void DiscoverForEntity(GameEntity entity, UniqueId discoveryProfileId)
		{
			DiscoveryProfile profile;
			if (!discoveryProfileId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<DiscoveryProfile>(discoveryProfileId, out profile))
			{
				DiscoveryProgression.DiscoverForEntity(entity, profile);
			}
		}

		// Token: 0x0600622E RID: 25134 RVA: 0x00203AF8 File Offset: 0x00201CF8
		public static void DiscoverForEntity(GameEntity entity, DiscoveryProfile profile)
		{
			if (profile == null || profile.Id.IsEmpty || !entity || entity.CollectionController == null || entity.CollectionController.Record == null)
			{
				return;
			}
			ZoneId zoneId = (ZoneId)LocalZoneManager.ZoneRecord.ZoneId;
			CharacterRecord record = entity.CollectionController.Record;
			if (record.Discoveries == null)
			{
				record.Discoveries = new Dictionary<ZoneId, List<UniqueId>>(1, default(ZoneIdComparer));
			}
			bool flag = false;
			List<UniqueId> list;
			if (record.Discoveries.TryGetValue(zoneId, out list))
			{
				if (!list.Contains(profile.Id))
				{
					list.Add(profile.Id);
					flag = true;
				}
			}
			else
			{
				record.Discoveries.Add(zoneId, new List<UniqueId>
				{
					profile.Id
				});
				flag = true;
			}
			if (flag)
			{
				if (!GameManager.IsServer)
				{
					profile.ClientDiscovered();
				}
				Action<DiscoveryProfile> discoveryFound = DiscoveryProgression.DiscoveryFound;
				if (discoveryFound == null)
				{
					return;
				}
				discoveryFound(profile);
			}
		}

		// Token: 0x0600622F RID: 25135 RVA: 0x0004475B File Offset: 0x0004295B
		public static void ResetAllDiscoveries(GameEntity entity)
		{
		}

		// Token: 0x06006230 RID: 25136 RVA: 0x0004475B File Offset: 0x0004295B
		public static void ResetZoneDiscoveries(GameEntity entity)
		{
		}
	}
}
