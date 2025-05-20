using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.Crafting;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A3F RID: 2623
	public class ItemComponentTree : INetworkSerializable, IEquatable<ItemComponentTree>
	{
		// Token: 0x06005131 RID: 20785 RVA: 0x001CEF48 File Offset: 0x001CD148
		public void CopyDataFrom(ItemComponentTree other)
		{
			if (other == null)
			{
				return;
			}
			this.QualityModifier = other.QualityModifier;
			if (other.Components != null && other.Components.Length != 0)
			{
				this.Components = new ItemComponentData[other.Components.Length];
				for (int i = 0; i < other.Components.Length; i++)
				{
					this.Components[i] = new ItemComponentData();
					this.Components[i].CopyDataFrom(other.Components[i]);
				}
			}
		}

		// Token: 0x06005132 RID: 20786 RVA: 0x001CEFC0 File Offset: 0x001CD1C0
		public void CopyDataFrom(ItemComponentData other)
		{
			if (other == null)
			{
				return;
			}
			this.QualityModifier = other.QualityModifier;
			if (other.ComponentItems != null && other.ComponentItems.Length != 0)
			{
				this.Components = new ItemComponentData[other.ComponentItems.Length];
				for (int i = 0; i < other.ComponentItems.Length; i++)
				{
					this.Components[i] = new ItemComponentData();
					this.Components[i].CopyDataFrom(other.ComponentItems[i]);
				}
			}
		}

		// Token: 0x06005133 RID: 20787 RVA: 0x001CF038 File Offset: 0x001CD238
		public BitBuffer PackData(BitBuffer buffer)
		{
			int num = (this.Components != null) ? this.Components.Length : 0;
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.Components[i].PackData(buffer);
				}
			}
			buffer.AddNullableInt(this.QualityModifier);
			return buffer;
		}

		// Token: 0x06005134 RID: 20788 RVA: 0x001CF090 File Offset: 0x001CD290
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Components = new ItemComponentData[num];
				for (int i = 0; i < num; i++)
				{
					this.Components[i] = new ItemComponentData();
					this.Components[i].ReadData(buffer, 0);
				}
			}
			this.QualityModifier = buffer.ReadNullableInt();
			return buffer;
		}

		// Token: 0x06005135 RID: 20789 RVA: 0x001CF0EC File Offset: 0x001CD2EC
		public BitBuffer PackData_BinaryIDs(BitBuffer buffer)
		{
			int num = (this.Components != null) ? this.Components.Length : 0;
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.Components[i].PackData_BinaryIDs(buffer);
				}
			}
			buffer.AddNullableInt(this.QualityModifier);
			return buffer;
		}

		// Token: 0x06005136 RID: 20790 RVA: 0x001CF144 File Offset: 0x001CD344
		public BitBuffer ReadData_BinaryIDs(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Components = new ItemComponentData[num];
				for (int i = 0; i < num; i++)
				{
					this.Components[i] = new ItemComponentData();
					this.Components[i].ReadData_BinaryIDs(buffer, 0);
				}
			}
			this.QualityModifier = buffer.ReadNullableInt();
			return buffer;
		}

		// Token: 0x06005137 RID: 20791 RVA: 0x001CF1A0 File Offset: 0x001CD3A0
		public bool ContainsArchetype(ItemArchetype archetype)
		{
			if (this.Components != null)
			{
				foreach (ItemComponentData itemComponentData in this.Components)
				{
					if (itemComponentData.ArchetypeId == archetype.Id || itemComponentData.Contains(archetype, null, 0))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005138 RID: 20792 RVA: 0x001CF1F0 File Offset: 0x001CD3F0
		public bool ContainsAll(ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null)
		{
			if (archetypes == null || archetypes.Length == 0 || this.Components == null)
			{
				return false;
			}
			foreach (ItemArchetype archetype in archetypes)
			{
				bool flag = false;
				ItemComponentData[] components = this.Components;
				for (int j = 0; j < components.Length; j++)
				{
					if (components[j].Contains(archetype, componentFilters, 0))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005139 RID: 20793 RVA: 0x001CF258 File Offset: 0x001CD458
		public bool ComponentContainsAll(UniqueId[] parentage, ItemArchetype[] archetypes)
		{
			if (archetypes == null || archetypes.Length == 0 || this.Components == null)
			{
				return false;
			}
			if (parentage == null || parentage.Length == 0)
			{
				return this.ContainsAll(archetypes, null);
			}
			ItemComponentData itemComponentData = null;
			foreach (UniqueId b in parentage)
			{
				ItemComponentData[] array = (itemComponentData == null) ? this.Components : itemComponentData.ComponentItems;
				if (array == null)
				{
					break;
				}
				foreach (ItemComponentData itemComponentData2 in array)
				{
					if (itemComponentData2.ArchetypeId == b)
					{
						itemComponentData = itemComponentData2;
						break;
					}
				}
			}
			if (itemComponentData.ComponentItems == null)
			{
				return false;
			}
			foreach (ItemArchetype itemArchetype in archetypes)
			{
				bool flag = false;
				foreach (ItemComponentData itemComponentData3 in itemComponentData.ComponentItems)
				{
					if (itemComponentData3.ArchetypeId == itemArchetype.Id || itemComponentData3.Contains(itemArchetype, null, 0))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600513A RID: 20794 RVA: 0x001CF35C File Offset: 0x001CD55C
		public bool ContainsAny(ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null)
		{
			if (archetypes == null || archetypes.Length == 0 || this.Components == null)
			{
				return false;
			}
			foreach (ItemArchetype itemArchetype in archetypes)
			{
				if (itemArchetype)
				{
					foreach (ItemComponentData itemComponentData in this.Components)
					{
						if (itemComponentData != null && itemComponentData.Contains(itemArchetype, componentFilters, 0))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600513B RID: 20795 RVA: 0x001CF3C8 File Offset: 0x001CD5C8
		public bool ComponentContainsAny(UniqueId[] parentage, ItemArchetype[] archetypes)
		{
			if (archetypes == null || archetypes.Length == 0 || this.Components == null)
			{
				return false;
			}
			if (parentage == null || parentage.Length == 0)
			{
				return this.ContainsAny(archetypes, null);
			}
			ItemComponentData itemComponentData = null;
			foreach (UniqueId b in parentage)
			{
				ItemComponentData[] array = (itemComponentData == null) ? this.Components : itemComponentData.ComponentItems;
				if (array == null)
				{
					break;
				}
				foreach (ItemComponentData itemComponentData2 in array)
				{
					if (itemComponentData2.ArchetypeId == b)
					{
						itemComponentData = itemComponentData2;
						break;
					}
				}
			}
			if (itemComponentData.ComponentItems == null)
			{
				return false;
			}
			foreach (ItemArchetype itemArchetype in archetypes)
			{
				foreach (ItemComponentData itemComponentData3 in itemComponentData.ComponentItems)
				{
					if (itemComponentData3.ArchetypeId == itemArchetype.Id || itemComponentData3.Contains(itemArchetype, null, 0))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600513C RID: 20796 RVA: 0x001CF4C4 File Offset: 0x001CD6C4
		public bool ContainsOnly(ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null)
		{
			if (archetypes == null || archetypes.Length == 0 || this.Components == null)
			{
				return false;
			}
			foreach (ItemArchetype itemArchetype in archetypes)
			{
				bool flag = false;
				foreach (ItemComponentData itemComponentData in this.Components)
				{
					flag = (flag || itemComponentData.ContainsOnly(archetypes, componentFilters, 0));
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600513D RID: 20797 RVA: 0x001CF530 File Offset: 0x001CD730
		public bool ComponentContainsOnly(UniqueId[] parentage, ItemArchetype[] archetypes)
		{
			if (archetypes == null || archetypes.Length == 0 || this.Components == null)
			{
				return false;
			}
			if (parentage == null || parentage.Length == 0)
			{
				return this.ContainsAny(archetypes, null);
			}
			ItemComponentData itemComponentData = null;
			foreach (UniqueId b in parentage)
			{
				ItemComponentData[] array = (itemComponentData == null) ? this.Components : itemComponentData.ComponentItems;
				if (array == null)
				{
					break;
				}
				foreach (ItemComponentData itemComponentData2 in array)
				{
					if (itemComponentData2.ArchetypeId == b)
					{
						itemComponentData = itemComponentData2;
						break;
					}
				}
			}
			if (itemComponentData.ComponentItems == null)
			{
				return false;
			}
			foreach (ItemArchetype itemArchetype in archetypes)
			{
				bool flag = false;
				foreach (ItemComponentData itemComponentData3 in itemComponentData.ComponentItems)
				{
					flag = (flag || (itemComponentData3.ArchetypeId == itemArchetype.Id && itemComponentData3.ContainsOnly(archetypes, null, 0)));
				}
				if (!flag)
				{
					return false;
				}
			}
			return false;
		}

		// Token: 0x0600513E RID: 20798 RVA: 0x001CF63C File Offset: 0x001CD83C
		public ItemAndComponent? FindItemProducedBy(ItemAndComponent contributor)
		{
			if (this.Components != null)
			{
				foreach (ItemComponentData itemComponentData in this.Components)
				{
					if (itemComponentData.ArchetypeId == contributor.ArchetypeId && itemComponentData.RecipeComponentId == contributor.RecipeComponentId)
					{
						return null;
					}
					ItemAndComponent? result = itemComponentData.FindItemProducedBy(contributor);
					if (result != null)
					{
						return result;
					}
				}
			}
			return null;
		}

		// Token: 0x0600513F RID: 20799 RVA: 0x001CF6B8 File Offset: 0x001CD8B8
		public ItemComponentData FindItemComponentDataFor(ItemAndComponent contributor)
		{
			if (this.Components != null)
			{
				ItemComponentData[] components = this.Components;
				for (int i = 0; i < components.Length; i++)
				{
					ItemComponentData itemComponentData = components[i].FindItemComponentDataFor(contributor);
					if (itemComponentData != null)
					{
						return itemComponentData;
					}
				}
			}
			return null;
		}

		// Token: 0x06005140 RID: 20800 RVA: 0x001CF6F4 File Offset: 0x001CD8F4
		public List<ItemArchetype> GetArchetypeLeafList<T>(ComponentParentage[] componentFilters = null) where T : ItemArchetype
		{
			if (this.Components == null)
			{
				return null;
			}
			List<ItemArchetype> fromPool = StaticListPool<ItemArchetype>.GetFromPool();
			ItemComponentData[] components = this.Components;
			for (int i = 0; i < components.Length; i++)
			{
				components[i].PopulateArchetypeLeafList<T>(fromPool, componentFilters, 0);
			}
			return fromPool;
		}

		// Token: 0x06005141 RID: 20801 RVA: 0x001CF734 File Offset: 0x001CD934
		public List<ItemAndComponent> GetArchetypeLeafListWithComponents(ComponentParentage[] componentFilters = null)
		{
			if (this.Components == null)
			{
				return null;
			}
			List<ItemAndComponent> fromPool = StaticListPool<ItemAndComponent>.GetFromPool();
			ItemComponentData[] components = this.Components;
			for (int i = 0; i < components.Length; i++)
			{
				components[i].PopulateArchetypeLeafListWithComponents(fromPool, componentFilters, 0);
			}
			return fromPool;
		}

		// Token: 0x06005142 RID: 20802 RVA: 0x001CF774 File Offset: 0x001CD974
		public List<RawComponentWithQuality> GetRawMaterialsListWithQualities(ComponentParentage[] componentFilters = null)
		{
			if (this.Components == null)
			{
				return null;
			}
			List<RawComponentWithQuality> fromPool = StaticPoolableListPool<RawComponentWithQuality>.GetFromPool();
			foreach (ItemComponentData itemComponentData in this.Components)
			{
				List<RawComponentWithQuality> outList = fromPool;
				int? qualityModifier = this.QualityModifier;
				itemComponentData.PopulateRawMaterialsListWithQualities(outList, (qualityModifier != null) ? new float?((float)qualityModifier.GetValueOrDefault()) : null, componentFilters, 0);
			}
			return fromPool;
		}

		// Token: 0x06005143 RID: 20803 RVA: 0x001CF7DC File Offset: 0x001CD9DC
		public bool Equals(ItemComponentTree other)
		{
			bool flag;
			if (other != null && ((this.Components == null && other.Components == null) || (this.Components != null && other.Components != null)))
			{
				int? num2;
				if (this.Components != null)
				{
					int num = this.Components.Length;
					ItemComponentData[] components = other.Components;
					num2 = ((components != null) ? new int?(components.Length) : null);
					if (!(num == num2.GetValueOrDefault() & num2 != null))
					{
						goto IL_97;
					}
				}
				num2 = this.QualityModifier;
				int? qualityModifier = other.QualityModifier;
				flag = (num2.GetValueOrDefault() == qualityModifier.GetValueOrDefault() & num2 != null == (qualityModifier != null));
				goto IL_98;
			}
			IL_97:
			flag = false;
			IL_98:
			bool flag2 = flag;
			if (flag2 && this.Components != null && other.Components != null)
			{
				for (int i = 0; i < this.Components.Length; i++)
				{
					flag2 = (flag2 && this.Components[i].Equals(other.Components[i]));
				}
			}
			return flag2;
		}

		// Token: 0x06005144 RID: 20804 RVA: 0x001CF8C8 File Offset: 0x001CDAC8
		public bool Equals(ItemComponentData other)
		{
			bool flag;
			if (other != null && ((this.Components == null && other.ComponentItems == null) || (this.Components != null && other.ComponentItems != null)))
			{
				int? num2;
				if (this.Components != null)
				{
					int num = this.Components.Length;
					ItemComponentData[] componentItems = other.ComponentItems;
					num2 = ((componentItems != null) ? new int?(componentItems.Length) : null);
					if (!(num == num2.GetValueOrDefault() & num2 != null))
					{
						goto IL_97;
					}
				}
				num2 = this.QualityModifier;
				int? qualityModifier = other.QualityModifier;
				flag = (num2.GetValueOrDefault() == qualityModifier.GetValueOrDefault() & num2 != null == (qualityModifier != null));
				goto IL_98;
			}
			IL_97:
			flag = false;
			IL_98:
			bool flag2 = flag;
			if (flag2 && this.Components != null && other.ComponentItems != null)
			{
				for (int i = 0; i < this.Components.Length; i++)
				{
					flag2 = (flag2 && this.Components[i].Equals(other.ComponentItems[i]));
				}
			}
			return flag2;
		}

		// Token: 0x06005145 RID: 20805 RVA: 0x001CF9B4 File Offset: 0x001CDBB4
		public override int GetHashCode()
		{
			if (this.Components == null)
			{
				return 0;
			}
			Array.Sort<ItemComponentData>(this.Components, (ItemComponentData x, ItemComponentData y) => x.GetHashCode().CompareTo(y.GetHashCode()));
			int num = 0;
			foreach (ItemComponentData itemComponentData in this.Components)
			{
				num = (num * 397 ^ itemComponentData.GetHashCode());
			}
			return num;
		}

		// Token: 0x0400489D RID: 18589
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ItemComponentData[] Components;

		// Token: 0x0400489E RID: 18590
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? QualityModifier;
	}
}
