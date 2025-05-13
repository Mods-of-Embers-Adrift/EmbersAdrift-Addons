using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CEA RID: 3306
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Crafting/Raw Component")]
	public class RawComponent : StackableItem
	{
		// Token: 0x06006414 RID: 25620 RVA: 0x0008360E File Offset: 0x0008180E
		private void CopyFromGlobal()
		{
			this.m_gatherFailureChance = new RawComponentFailureChance(GlobalSettings.Values.Gathering.FailureChance);
		}

		// Token: 0x170017FB RID: 6139
		// (get) Token: 0x06006415 RID: 25621 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanPlaceInGathering
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170017FC RID: 6140
		// (get) Token: 0x06006416 RID: 25622 RVA: 0x0008362A File Offset: 0x0008182A
		public RawComponent FailureResult
		{
			get
			{
				return this.m_failureResult;
			}
		}

		// Token: 0x170017FD RID: 6141
		// (get) Token: 0x06006417 RID: 25623 RVA: 0x00083632 File Offset: 0x00081832
		public int MinimumRawMaterialLevel
		{
			get
			{
				return this.m_minimumRawMaterialLevel;
			}
		}

		// Token: 0x170017FE RID: 6142
		// (get) Token: 0x06006418 RID: 25624 RVA: 0x0008363A File Offset: 0x0008183A
		public int MaximumRawMaterialLevel
		{
			get
			{
				return this.m_maximumRawMaterialLevel;
			}
		}

		// Token: 0x170017FF RID: 6143
		// (get) Token: 0x06006419 RID: 25625 RVA: 0x00083642 File Offset: 0x00081842
		public int GlobalAttributeClamp
		{
			get
			{
				return this.m_globalAttributeClamp;
			}
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x0008364A File Offset: 0x0008184A
		public int GetAttribute(ItemAttributes.Names name)
		{
			return this.m_attributes.GetAttribute(name);
		}

		// Token: 0x0600641B RID: 25627 RVA: 0x00083658 File Offset: 0x00081858
		public bool IsAttributeActive(ItemAttributes.Names name)
		{
			return this.m_attributes.IsActive(name);
		}

		// Token: 0x17001800 RID: 6144
		// (get) Token: 0x0600641C RID: 25628 RVA: 0x00083666 File Offset: 0x00081866
		private bool m_showFailureOverride
		{
			get
			{
				return this.m_failureResult != null;
			}
		}

		// Token: 0x17001801 RID: 6145
		// (get) Token: 0x0600641D RID: 25629 RVA: 0x00083674 File Offset: 0x00081874
		private bool m_showFailureOverrideCurve
		{
			get
			{
				return this.m_showFailureOverride && this.m_overrideGlobalGatherFailureChance;
			}
		}

		// Token: 0x0600641E RID: 25630 RVA: 0x002085A0 File Offset: 0x002067A0
		public bool CheckForGatherFailure(GameEntity source, IGatheringNode node, out ArchetypeInstance newInstance)
		{
			newInstance = null;
			MasteryArchetype masteryArchetype = (node != null) ? node.GetGatheringMastery() : null;
			ArchetypeInstance archetypeInstance;
			if (this.m_failureResult != null && masteryArchetype != null && source.CollectionController.Masteries.TryGetInstanceForArchetypeId(masteryArchetype.Id, out archetypeInstance))
			{
				float associatedLevel = archetypeInstance.GetAssociatedLevel(source);
				RawComponentFailureChance rawComponentFailureChance = this.m_overrideGlobalGatherFailureChance ? this.m_gatherFailureChance : GlobalSettings.Values.Gathering.FailureChance;
				bool flag;
				if (associatedLevel < (float)this.m_maximumRawMaterialLevel)
				{
					float num = associatedLevel - (float)this.m_minimumRawMaterialLevel;
					int num2 = this.m_maximumRawMaterialLevel - this.m_minimumRawMaterialLevel;
					flag = rawComponentFailureChance.CheckForFailure(false, num / (float)num2);
				}
				else
				{
					float num3 = associatedLevel - (float)this.m_maximumRawMaterialLevel;
					float num4 = 50f - (float)this.m_maximumRawMaterialLevel;
					flag = rawComponentFailureChance.CheckForFailure(true, num3 / num4);
				}
				if (flag)
				{
					newInstance = this.m_failureResult.CreateNewInstance();
				}
			}
			return newInstance != null;
		}

		// Token: 0x17001802 RID: 6146
		// (get) Token: 0x0600641F RID: 25631 RVA: 0x00083686 File Offset: 0x00081886
		private IEnumerable GetRawComponents
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<RawComponent>();
			}
		}

		// Token: 0x040056FA RID: 22266
		[SerializeField]
		private RawComponent m_failureResult;

		// Token: 0x040056FB RID: 22267
		private const string kFailureGroup = "Failure";

		// Token: 0x040056FC RID: 22268
		[SerializeField]
		private bool m_overrideGlobalGatherFailureChance;

		// Token: 0x040056FD RID: 22269
		[SerializeField]
		private RawComponentFailureChance m_gatherFailureChance;

		// Token: 0x040056FE RID: 22270
		[SerializeField]
		private DummyClass m_failureDummy;

		// Token: 0x040056FF RID: 22271
		[Obsolete]
		[SerializeField]
		private Color m_imageTintColor = Color.white;

		// Token: 0x04005700 RID: 22272
		[SerializeField]
		private int m_minimumRawMaterialLevel = 1;

		// Token: 0x04005701 RID: 22273
		[SerializeField]
		private int m_maximumRawMaterialLevel = 1;

		// Token: 0x04005702 RID: 22274
		[Range(0f, 1000f)]
		[SerializeField]
		private int m_globalAttributeClamp = 1000;

		// Token: 0x04005703 RID: 22275
		[SerializeField]
		private ItemAttributes m_attributes;
	}
}
