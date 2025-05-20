using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000557 RID: 1367
	[CreateAssetMenu(menuName = "SoL/Collections/Archetypes", order = 5)]
	public class ArchetypeCollection : IdentifiableScriptableObjectCollection<BaseArchetype>
	{
		// Token: 0x0600297E RID: 10622 RVA: 0x00141A58 File Offset: 0x0013FC58
		protected override void Initialize()
		{
			base.Initialize();
			if (this.m_categories != null)
			{
				return;
			}
			this.m_categories = new Dictionary<ArchetypeCategory, DictionaryList<UniqueId, BaseArchetype>>(default(ArchetypeCategoryComparer));
			for (int i = 0; i < this.m_items.Length; i++)
			{
				if (this.m_items[i] == null)
				{
					Debug.LogError(string.Format("index {0} is null!", i));
				}
				else if (this.m_items[i].Id.IsEmpty)
				{
					Debug.LogError(string.Format("{0} in index [{1}] has an empty ID!", this.m_items[i].name, i));
				}
				else
				{
					DictionaryList<UniqueId, BaseArchetype> dictionaryList;
					if (this.m_categories.TryGetValue(this.m_items[i].Category, out dictionaryList))
					{
						dictionaryList.Add(this.m_items[i].Id, this.m_items[i]);
					}
					else
					{
						DictionaryList<UniqueId, BaseArchetype> dictionaryList2 = new DictionaryList<UniqueId, BaseArchetype>(default(UniqueIdComparer), false);
						dictionaryList2.Add(this.m_items[i].Id, this.m_items[i]);
						this.m_categories.Add(this.m_items[i].Category, dictionaryList2);
					}
					DynamicAbility dynamicAbility;
					if (this.m_items[i].TryGetAsType(out dynamicAbility))
					{
						this.m_dynamicAbilityInstanceIds.Add(dynamicAbility.GetInstanceId(), dynamicAbility);
						this.m_dynamicAbilityArchetypeIds.Add(dynamicAbility.Id, dynamicAbility);
					}
				}
			}
		}

		// Token: 0x0600297F RID: 10623 RVA: 0x00141BCC File Offset: 0x0013FDCC
		public bool TryGetAsType<T>(UniqueId id, out T value) where T : class
		{
			value = default(T);
			BaseArchetype item = base.GetItem(id);
			return item != null && item.TryGetAsType(out value);
		}

		// Token: 0x06002980 RID: 10624 RVA: 0x00141BFC File Offset: 0x0013FDFC
		public BaseArchetype GetRandomItem()
		{
			DictionaryList<UniqueId, BaseArchetype> dictionaryList;
			if (this.m_categories.TryGetValue(ArchetypeCategory.Item, out dictionaryList))
			{
				int index = UnityEngine.Random.Range(0, dictionaryList.Count);
				return dictionaryList[index];
			}
			return null;
		}

		// Token: 0x06002981 RID: 10625 RVA: 0x0005CB14 File Offset: 0x0005AD14
		[ContextMenu("Init")]
		private void DoInit()
		{
			this.Initialize();
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x0005CB1C File Offset: 0x0005AD1C
		public bool GetDynamicAbilityByInstanceId(string instanceId, out DynamicAbility dynamicAbility)
		{
			return this.m_dynamicAbilityInstanceIds.TryGetValue(instanceId, out dynamicAbility);
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x0005CB2B File Offset: 0x0005AD2B
		public bool GetDynamicAbilityByArchetypeId(UniqueId archetypeId, out DynamicAbility dynamicAbility)
		{
			return this.m_dynamicAbilityArchetypeIds.TryGetValue(archetypeId, out dynamicAbility);
		}

		// Token: 0x04002A71 RID: 10865
		private Dictionary<ArchetypeCategory, DictionaryList<UniqueId, BaseArchetype>> m_categories;

		// Token: 0x04002A72 RID: 10866
		private Dictionary<string, DynamicAbility> m_dynamicAbilityInstanceIds = new Dictionary<string, DynamicAbility>();

		// Token: 0x04002A73 RID: 10867
		private Dictionary<UniqueId, DynamicAbility> m_dynamicAbilityArchetypeIds = new Dictionary<UniqueId, DynamicAbility>(default(UniqueIdComparer));
	}
}
