using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Influence;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Randomization;
using SoL.Game.Spawning;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CF0 RID: 3312
	[CreateAssetMenu(menuName = "SoL/Profiles/Resource Node 2")]
	public class ResourceSpawnProfileV2 : SpawnProfile
	{
		// Token: 0x17001810 RID: 6160
		// (get) Token: 0x0600645B RID: 25691 RVA: 0x00083803 File Offset: 0x00081A03
		private bool m_showSortOverrides
		{
			get
			{
				return this.m_overrides != null && this.m_overrides.Length != 0;
			}
		}

		// Token: 0x0600645C RID: 25692 RVA: 0x00063AFE File Offset: 0x00061CFE
		private IEnumerable GetInfluenceProfile()
		{
			return SolOdinUtilities.GetDropdownItems<InfluenceProfile>();
		}

		// Token: 0x17001811 RID: 6161
		// (get) Token: 0x0600645D RID: 25693 RVA: 0x00083819 File Offset: 0x00081A19
		private bool AllowCurrency
		{
			get
			{
				ResourceSpawnProfileV2.BaseResourceTier @base = this.m_base;
				return ((@base != null) ? @base.Loot : null) != null && this.m_base.Loot.RequiredTool == CraftingToolType.None;
			}
		}

		// Token: 0x17001812 RID: 6162
		// (get) Token: 0x0600645E RID: 25694 RVA: 0x00083844 File Offset: 0x00081A44
		private bool HasCurrency
		{
			get
			{
				return this.AllowCurrency && this.m_hasCurrency;
			}
		}

		// Token: 0x0600645F RID: 25695 RVA: 0x00209C70 File Offset: 0x00207E70
		private string GetDecadalCurrencyDescription()
		{
			if (this.m_currency.Min <= 0 && this.m_currency.Max <= 0)
			{
				return "No Currency";
			}
			CurrencyConverter arg = new CurrencyConverter((ulong)((long)this.m_currency.Min));
			CurrencyConverter arg2 = new CurrencyConverter((ulong)((long)this.m_currency.Max));
			return ZString.Format<CurrencyConverter, CurrencyConverter>("[{0}] to [{1}]", arg, arg2);
		}

		// Token: 0x06006460 RID: 25696 RVA: 0x00083856 File Offset: 0x00081A56
		private string GetDetails()
		{
			return "Currency Details";
		}

		// Token: 0x06006461 RID: 25697 RVA: 0x0008385D File Offset: 0x00081A5D
		private void SortOverrides()
		{
			Array.Sort<ResourceSpawnProfileV2.ResourceTierWithOverrides>(this.m_overrides, (ResourceSpawnProfileV2.ResourceTierWithOverrides a, ResourceSpawnProfileV2.ResourceTierWithOverrides b) => a.LevelThreshold.CompareTo(b.LevelThreshold));
		}

		// Token: 0x06006462 RID: 25698 RVA: 0x00209CD4 File Offset: 0x00207ED4
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			GlobalCounters.SpawnedNodes += 1U;
			base.SpawnInternal(controller, gameEntity);
			ResourceSpawnProfileV2.MetadataSettings metadata = this.m_base.Metadata;
			ResourceSpawnProfileV2.VisualSettings visuals = this.m_base.Visuals;
			ResourceLootSettings loot = this.m_base.Loot;
			if (this.m_influenceProfile != null && gameEntity.InfluenceSource != null)
			{
				gameEntity.InfluenceSource.InfluenceProfile = this.m_influenceProfile;
			}
			InteractiveGatheringNode interactiveGatheringNode;
			if (gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveGatheringNode) && loot.ResourceLootTable != null && loot.ResourceLootTable.Table != null)
			{
				interactiveGatheringNode.SpawnController = controller;
				interactiveGatheringNode.SpawnProfile = this;
				interactiveGatheringNode.ResourceTable = loot.ResourceLootTable;
				interactiveGatheringNode.LogLoot = loot.LogLoot;
				interactiveGatheringNode.Description = metadata.Name;
				if (this.HasCurrency)
				{
					interactiveGatheringNode.Currency = new MinMaxIntRange?(this.m_currency);
				}
				interactiveGatheringNode.GatheringParams = new GatheringParameters
				{
					RequiredTool = loot.RequiredTool,
					RequiredItemId = loot.GetRequiredItemIdForGathering(),
					AbilityId = loot.GetAbilityIdForGathering(),
					GatherTime = loot.GatherTime,
					Level = loot.ResourceLevel,
					RemoveRequiredItemOnUse = loot.RemoveRequiredItemOnUse
				};
			}
			int num;
			if (gameEntity.SeedReplicator != null && visuals.VisualIndex != null && visuals.VisualIndex.TryGetVisualIndex(this.m_prefabReference, out num))
			{
				gameEntity.SeedReplicator.VisualIndexOverride = new byte?((byte)num);
			}
		}

		// Token: 0x0400571F RID: 22303
		[SerializeField]
		private InfluenceProfile m_influenceProfile;

		// Token: 0x04005720 RID: 22304
		[SerializeField]
		private ResourceSpawnProfileV2.BaseResourceTier m_base;

		// Token: 0x04005721 RID: 22305
		[HideInInspector]
		[SerializeField]
		private ResourceSpawnProfileV2.ResourceTierWithOverrides[] m_overrides;

		// Token: 0x04005722 RID: 22306
		private const string kCurrencyGroup = "Currency";

		// Token: 0x04005723 RID: 22307
		private const string kActualCurrency = "Currency/Horizontal";

		// Token: 0x04005724 RID: 22308
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x04005725 RID: 22309
		[SerializeField]
		private bool m_hasCurrency;

		// Token: 0x04005726 RID: 22310
		[SerializeField]
		private MinMaxIntRange m_currency = new MinMaxIntRange(0, 0);

		// Token: 0x04005727 RID: 22311
		[SerializeField]
		private DummyClass m_currencyDummy;

		// Token: 0x02000CF1 RID: 3313
		[Serializable]
		private class MetadataSettings
		{
			// Token: 0x04005728 RID: 22312
			private const string kGroupName = "Metadata";

			// Token: 0x04005729 RID: 22313
			public string Name;
		}

		// Token: 0x02000CF2 RID: 3314
		[Serializable]
		private class VisualSettings
		{
			// Token: 0x0400572A RID: 22314
			private const string kGroupName = "Visuals";

			// Token: 0x0400572B RID: 22315
			public VisualIndex VisualIndex;
		}

		// Token: 0x02000CF3 RID: 3315
		[Serializable]
		private abstract class ResourceTier
		{
			// Token: 0x17001813 RID: 6163
			// (get) Token: 0x06006466 RID: 25702 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowMetadata
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17001814 RID: 6164
			// (get) Token: 0x06006467 RID: 25703 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowVisuals
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17001815 RID: 6165
			// (get) Token: 0x06006468 RID: 25704 RVA: 0x0004479C File Offset: 0x0004299C
			protected virtual bool ShowLoot
			{
				get
				{
					return true;
				}
			}

			// Token: 0x0400572C RID: 22316
			protected const int kMetadataOrder = 10;

			// Token: 0x0400572D RID: 22317
			protected const int kVisualsOrder = 20;

			// Token: 0x0400572E RID: 22318
			protected const int kLootOrder = 30;

			// Token: 0x0400572F RID: 22319
			public ResourceSpawnProfileV2.MetadataSettings Metadata;

			// Token: 0x04005730 RID: 22320
			public ResourceSpawnProfileV2.VisualSettings Visuals;

			// Token: 0x04005731 RID: 22321
			public ResourceLootSettings Loot;
		}

		// Token: 0x02000CF4 RID: 3316
		[Serializable]
		private class BaseResourceTier : ResourceSpawnProfileV2.ResourceTier
		{
		}

		// Token: 0x02000CF5 RID: 3317
		[Serializable]
		private class ResourceTierWithOverrides : ResourceSpawnProfileV2.BaseResourceTier
		{
			// Token: 0x17001816 RID: 6166
			// (get) Token: 0x0600646B RID: 25707 RVA: 0x000838A6 File Offset: 0x00081AA6
			public int LevelThreshold
			{
				get
				{
					return this.m_levelThreshold;
				}
			}

			// Token: 0x17001817 RID: 6167
			// (get) Token: 0x0600646C RID: 25708 RVA: 0x000838AE File Offset: 0x00081AAE
			public bool OverrideMetadata
			{
				get
				{
					return this.m_overrideMetadata;
				}
			}

			// Token: 0x17001818 RID: 6168
			// (get) Token: 0x0600646D RID: 25709 RVA: 0x000838B6 File Offset: 0x00081AB6
			public bool OverrideVisuals
			{
				get
				{
					return this.m_overrideVisuals;
				}
			}

			// Token: 0x17001819 RID: 6169
			// (get) Token: 0x0600646E RID: 25710 RVA: 0x000838BE File Offset: 0x00081ABE
			public bool OverrideLoot
			{
				get
				{
					return this.m_overrideLoot;
				}
			}

			// Token: 0x1700181A RID: 6170
			// (get) Token: 0x0600646F RID: 25711 RVA: 0x000838AE File Offset: 0x00081AAE
			protected override bool ShowMetadata
			{
				get
				{
					return this.m_overrideMetadata;
				}
			}

			// Token: 0x1700181B RID: 6171
			// (get) Token: 0x06006470 RID: 25712 RVA: 0x000838B6 File Offset: 0x00081AB6
			protected override bool ShowVisuals
			{
				get
				{
					return this.m_overrideVisuals;
				}
			}

			// Token: 0x1700181C RID: 6172
			// (get) Token: 0x06006471 RID: 25713 RVA: 0x000838BE File Offset: 0x00081ABE
			protected override bool ShowLoot
			{
				get
				{
					return this.m_overrideLoot;
				}
			}

			// Token: 0x04005732 RID: 22322
			[Range(1f, 50f)]
			[SerializeField]
			private int m_levelThreshold = 1;

			// Token: 0x04005733 RID: 22323
			[SerializeField]
			private bool m_overrideMetadata;

			// Token: 0x04005734 RID: 22324
			[SerializeField]
			private bool m_overrideVisuals;

			// Token: 0x04005735 RID: 22325
			[SerializeField]
			private bool m_overrideLoot;
		}
	}
}
