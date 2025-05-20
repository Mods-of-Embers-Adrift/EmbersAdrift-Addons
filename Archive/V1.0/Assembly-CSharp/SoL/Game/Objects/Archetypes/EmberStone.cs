using System;
using SoL.Game.Pooling;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A37 RID: 2615
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Ember Stone")]
	public class EmberStone : BaseArchetype
	{
		// Token: 0x17001212 RID: 4626
		// (get) Token: 0x060050EE RID: 20718 RVA: 0x00076177 File Offset: 0x00074377
		public int MaxCapacity
		{
			get
			{
				return this.m_maxCapacity;
			}
		}

		// Token: 0x17001213 RID: 4627
		// (get) Token: 0x060050EF RID: 20719 RVA: 0x0007617F File Offset: 0x0007437F
		public override Color IconTint
		{
			get
			{
				return this.m_iconTint;
			}
		}

		// Token: 0x17001214 RID: 4628
		// (get) Token: 0x060050F0 RID: 20720 RVA: 0x00076187 File Offset: 0x00074387
		public PooledHandheldItem HandHeldItem
		{
			get
			{
				return this.m_item;
			}
		}

		// Token: 0x17001215 RID: 4629
		// (get) Token: 0x060050F1 RID: 20721 RVA: 0x0007618F File Offset: 0x0007438F
		public EmberStone NextEmberStone
		{
			get
			{
				return this.m_nextEmberStone;
			}
		}

		// Token: 0x04004870 RID: 18544
		private const string kVariantDirectory = "Assets/Prefabs/Pooling/Weapons/EmberStones";

		// Token: 0x04004871 RID: 18545
		[SerializeField]
		private Color m_iconTint = Color.white;

		// Token: 0x04004872 RID: 18546
		[SerializeField]
		private int m_maxCapacity = 100;

		// Token: 0x04004873 RID: 18547
		[SerializeField]
		private PooledHandheldItem m_item;

		// Token: 0x04004874 RID: 18548
		[SerializeField]
		private EmberStone m_nextEmberStone;
	}
}
