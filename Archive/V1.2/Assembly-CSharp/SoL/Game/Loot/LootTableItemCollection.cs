using System;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Loot
{
	// Token: 0x02000B24 RID: 2852
	[Serializable]
	public class LootTableItemCollection
	{
		// Token: 0x1700149B RID: 5275
		// (get) Token: 0x0600579A RID: 22426 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool HideLocalCollection
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700149C RID: 5276
		// (get) Token: 0x0600579B RID: 22427 RVA: 0x0007A6B3 File Offset: 0x000788B3
		private bool m_allowDrawBagMethod
		{
			get
			{
				return this.m_collection != null && this.m_collection.Count > 1;
			}
		}

		// Token: 0x1700149D RID: 5277
		// (get) Token: 0x0600579C RID: 22428 RVA: 0x0007A6CD File Offset: 0x000788CD
		private bool m_showDrawBagParams
		{
			get
			{
				return this.m_allowDrawBagMethod && this.m_useDrawBagMethod;
			}
		}

		// Token: 0x0600579D RID: 22429 RVA: 0x001E4120 File Offset: 0x001E2320
		public virtual LootTableItem GetLootTableItem()
		{
			if (this.m_useDrawBagMethod && this.m_allowDrawBagMethod)
			{
				if (this.m_drawBag == null)
				{
					this.m_drawBag = new LootTableItemDrawBag(this.m_collection, this.m_sampleSize, this.m_reshuffleThreshold);
				}
				LootTableItemDrawBag drawBag = this.m_drawBag;
				if (drawBag == null)
				{
					return null;
				}
				return drawBag.GetItem();
			}
			else
			{
				LootTableItemProbabilityCollection collection = this.m_collection;
				if (collection == null)
				{
					return null;
				}
				LootTableItemProbabilityEntry entry = collection.GetEntry(null, false);
				if (entry == null)
				{
					return null;
				}
				return entry.Obj;
			}
		}

		// Token: 0x0600579E RID: 22430 RVA: 0x0007A6DF File Offset: 0x000788DF
		public void OnValidate()
		{
			LootTableItemProbabilityCollection collection = this.m_collection;
			if (collection == null)
			{
				return;
			}
			collection.Normalize();
		}

		// Token: 0x1700149E RID: 5278
		// (get) Token: 0x0600579F RID: 22431 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showLoadInternal
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700149F RID: 5279
		// (get) Token: 0x060057A0 RID: 22432 RVA: 0x0007A6F1 File Offset: 0x000788F1
		private bool m_showLoad
		{
			get
			{
				return this.m_showLoadInternal && this.m_toLoad != null && this.m_toLoad.Length != 0;
			}
		}

		// Token: 0x04004D4E RID: 19790
		protected const string kCollectionGroup = "Collection";

		// Token: 0x04004D4F RID: 19791
		[SerializeField]
		private bool m_useDrawBagMethod;

		// Token: 0x04004D50 RID: 19792
		[Range(100f, 1000f)]
		[SerializeField]
		private int m_sampleSize = 100;

		// Token: 0x04004D51 RID: 19793
		[Range(0.1f, 1f)]
		[SerializeField]
		private float m_reshuffleThreshold = 0.5f;

		// Token: 0x04004D52 RID: 19794
		[SerializeField]
		private LootTableItemProbabilityCollection m_collection;

		// Token: 0x04004D53 RID: 19795
		private LootTableItemDrawBag m_drawBag;

		// Token: 0x04004D54 RID: 19796
		[SerializeField]
		private ItemArchetype[] m_toLoad;
	}
}
