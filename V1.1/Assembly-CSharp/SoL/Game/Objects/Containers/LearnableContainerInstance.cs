using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities.Extensions;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A15 RID: 2581
	public class LearnableContainerInstance
	{
		// Token: 0x14000100 RID: 256
		// (add) Token: 0x06004EF7 RID: 20215 RVA: 0x001C4590 File Offset: 0x001C2790
		// (remove) Token: 0x06004EF8 RID: 20216 RVA: 0x001C45C8 File Offset: 0x001C27C8
		public event Action<LearnableArchetype> LearnableAdded;

		// Token: 0x14000101 RID: 257
		// (add) Token: 0x06004EF9 RID: 20217 RVA: 0x001C4600 File Offset: 0x001C2800
		// (remove) Token: 0x06004EFA RID: 20218 RVA: 0x001C4638 File Offset: 0x001C2838
		public event Action<LearnableArchetype> LearnableRemoved;

		// Token: 0x14000102 RID: 258
		// (add) Token: 0x06004EFB RID: 20219 RVA: 0x001C4670 File Offset: 0x001C2870
		// (remove) Token: 0x06004EFC RID: 20220 RVA: 0x001C46A8 File Offset: 0x001C28A8
		public event Action ContentsChanged;

		// Token: 0x1700116E RID: 4462
		// (get) Token: 0x06004EFD RID: 20221 RVA: 0x0007535B File Offset: 0x0007355B
		public ContainerType ContainerType
		{
			get
			{
				return this.m_record.Type;
			}
		}

		// Token: 0x1700116F RID: 4463
		// (get) Token: 0x06004EFE RID: 20222 RVA: 0x00075368 File Offset: 0x00073568
		public int Count
		{
			get
			{
				return this.m_learnables.Count;
			}
		}

		// Token: 0x06004EFF RID: 20223 RVA: 0x001C46E0 File Offset: 0x001C28E0
		public LearnableContainerInstance(ICollectionController controller, LearnableContainerRecord record)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			this.m_controller = controller;
			this.m_record = record;
			this.Id = record.GetId();
			this.m_learnables = new DictionaryList<UniqueId, LearnableArchetype>(default(UniqueIdComparer), true);
			this.Initialize();
		}

		// Token: 0x06004F00 RID: 20224 RVA: 0x001C474C File Offset: 0x001C294C
		private void Initialize()
		{
			for (int i = 0; i < this.m_record.LearnableIds.Count; i++)
			{
				BaseArchetype archetype;
				LearnableArchetype learnable;
				if (InternalGameDatabase.Archetypes.TryGetItem(this.m_record.LearnableIds[i], out archetype) && archetype.TryGetAsType(out learnable))
				{
					this.Add(learnable, false);
				}
			}
		}

		// Token: 0x06004F01 RID: 20225 RVA: 0x001C47A8 File Offset: 0x001C29A8
		public void Add(LearnableArchetype learnable, bool update)
		{
			this.m_learnables.Add(learnable.Id, learnable);
			if (update)
			{
				if (!this.m_record.LearnableIds.Contains(learnable.Id))
				{
					this.m_record.LearnableIds.Add(learnable.Id);
				}
				Action<LearnableArchetype> learnableAdded = this.LearnableAdded;
				if (learnableAdded != null)
				{
					learnableAdded(learnable);
				}
				Action contentsChanged = this.ContentsChanged;
				if (contentsChanged == null)
				{
					return;
				}
				contentsChanged();
			}
		}

		// Token: 0x06004F02 RID: 20226 RVA: 0x00075375 File Offset: 0x00073575
		public LearnableArchetype Remove(UniqueId id)
		{
			return this.GetRemove(id, this.m_record.Type.AllowRemoval(), true);
		}

		// Token: 0x06004F03 RID: 20227 RVA: 0x001C481C File Offset: 0x001C2A1C
		private LearnableArchetype GetRemove(UniqueId id, bool remove, bool update)
		{
			LearnableArchetype learnableArchetype;
			if (this.m_learnables.TryGetValue(id, out learnableArchetype) && remove)
			{
				this.m_learnables.Remove(id);
				if (update)
				{
					this.m_record.LearnableIds.Remove(learnableArchetype.Id);
					Action<LearnableArchetype> learnableRemoved = this.LearnableRemoved;
					if (learnableRemoved != null)
					{
						learnableRemoved(learnableArchetype);
					}
					Action contentsChanged = this.ContentsChanged;
					if (contentsChanged != null)
					{
						contentsChanged();
					}
				}
				return learnableArchetype;
			}
			return null;
		}

		// Token: 0x06004F04 RID: 20228 RVA: 0x0007538F File Offset: 0x0007358F
		private IEnumerable<LearnableArchetype> GetLearnables()
		{
			int num;
			for (int i = 0; i < this.m_learnables.Count; i = num + 1)
			{
				yield return this.m_learnables[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x17001170 RID: 4464
		// (get) Token: 0x06004F05 RID: 20229 RVA: 0x0007539F File Offset: 0x0007359F
		public IEnumerable<LearnableArchetype> Learnables
		{
			get
			{
				if (this.m_learnableEnumerable == null)
				{
					this.m_learnableEnumerable = this.GetLearnables();
				}
				return this.m_learnableEnumerable;
			}
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x000753BB File Offset: 0x000735BB
		public LearnableArchetype GetIndex(int index)
		{
			if (index < this.m_learnables.Count)
			{
				return this.m_learnables[index];
			}
			return null;
		}

		// Token: 0x06004F07 RID: 20231 RVA: 0x000753D9 File Offset: 0x000735D9
		public bool Contains(UniqueId id)
		{
			return this.m_learnables.ContainsKey(id);
		}

		// Token: 0x06004F08 RID: 20232 RVA: 0x000753E7 File Offset: 0x000735E7
		public bool TryGetLearnableForId(UniqueId learnableId, out LearnableArchetype learnable)
		{
			return this.m_learnables.TryGetValue(learnableId, out learnable);
		}

		// Token: 0x06004F09 RID: 20233 RVA: 0x000753F6 File Offset: 0x000735F6
		public void Clear()
		{
			this.m_learnables.Clear();
			this.m_record.ClearContents();
			Action contentsChanged = this.ContentsChanged;
			if (contentsChanged == null)
			{
				return;
			}
			contentsChanged();
		}

		// Token: 0x040047D1 RID: 18385
		private readonly DictionaryList<UniqueId, LearnableArchetype> m_learnables;

		// Token: 0x040047D2 RID: 18386
		private readonly ICollectionController m_controller;

		// Token: 0x040047D3 RID: 18387
		private readonly LearnableContainerRecord m_record;

		// Token: 0x040047D4 RID: 18388
		public readonly string Id;

		// Token: 0x040047D5 RID: 18389
		private IEnumerable<LearnableArchetype> m_learnableEnumerable;
	}
}
