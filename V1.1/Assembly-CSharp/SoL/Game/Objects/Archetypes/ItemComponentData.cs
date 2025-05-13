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
	// Token: 0x02000A41 RID: 2625
	public class ItemComponentData : IEquatable<ItemComponentData>
	{
		// Token: 0x0600514A RID: 20810 RVA: 0x001CF9E8 File Offset: 0x001CDBE8
		public void CopyDataFrom(ItemComponentData other)
		{
			if (other == null)
			{
				return;
			}
			this.ArchetypeId = other.ArchetypeId;
			this.RecipeComponentId = other.RecipeComponentId;
			this.QualityModifier = other.QualityModifier;
			if (other.ComponentItems != null && other.ComponentItems.Length != 0)
			{
				this.ComponentItems = new ItemComponentData[other.ComponentItems.Length];
				for (int i = 0; i < other.ComponentItems.Length; i++)
				{
					this.ComponentItems[i] = new ItemComponentData();
					this.ComponentItems[i].CopyDataFrom(other.ComponentItems[i]);
				}
			}
		}

		// Token: 0x0600514B RID: 20811 RVA: 0x001CFA78 File Offset: 0x001CDC78
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddUniqueId(this.RecipeComponentId);
			buffer.AddNullableInt(this.QualityModifier);
			int num = (this.ComponentItems != null) ? this.ComponentItems.Length : 0;
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.ComponentItems[i].PackData(buffer);
				}
			}
			return buffer;
		}

		// Token: 0x0600514C RID: 20812 RVA: 0x001CFAE8 File Offset: 0x001CDCE8
		public BitBuffer ReadData(BitBuffer buffer, int depth = 0)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.RecipeComponentId = buffer.ReadUniqueId();
			this.QualityModifier = buffer.ReadNullableInt();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.ComponentItems = new ItemComponentData[num];
				for (int i = 0; i < num; i++)
				{
					this.ComponentItems[i] = new ItemComponentData();
					this.ComponentItems[i].ReadData(buffer, depth + 1);
				}
			}
			return buffer;
		}

		// Token: 0x0600514D RID: 20813 RVA: 0x001CFB5C File Offset: 0x001CDD5C
		public BitBuffer PackData_BinaryIDs(BitBuffer buffer)
		{
			this.ArchetypeId.PackData_Binary(buffer);
			this.RecipeComponentId.PackData_Binary(buffer);
			buffer.AddNullableInt(this.QualityModifier);
			int num = (this.ComponentItems != null) ? this.ComponentItems.Length : 0;
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.ComponentItems[i].PackData_BinaryIDs(buffer);
				}
			}
			return buffer;
		}

		// Token: 0x0600514E RID: 20814 RVA: 0x001CFBCC File Offset: 0x001CDDCC
		public BitBuffer ReadData_BinaryIDs(BitBuffer buffer, int depth = 0)
		{
			this.ArchetypeId.ReadData_Binary(buffer, false);
			this.RecipeComponentId.ReadData_Binary(buffer, true);
			this.QualityModifier = buffer.ReadNullableInt();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.ComponentItems = new ItemComponentData[num];
				for (int i = 0; i < num; i++)
				{
					this.ComponentItems[i] = new ItemComponentData();
					this.ComponentItems[i].ReadData_BinaryIDs(buffer, depth + 1);
				}
			}
			return buffer;
		}

		// Token: 0x0600514F RID: 20815 RVA: 0x001CFC44 File Offset: 0x001CDE44
		public bool Contains(ItemArchetype archetype, ComponentParentage[] componentFilters = null, int componentDepth = 0)
		{
			ValueTuple<bool, bool> valueTuple = componentFilters.ShouldFilterOut(this.RecipeComponentId, componentDepth);
			if (valueTuple.Item1 && !valueTuple.Item2)
			{
				return false;
			}
			if (archetype.Id == this.ArchetypeId)
			{
				return true;
			}
			if (this.ComponentItems != null)
			{
				foreach (ItemComponentData itemComponentData in this.ComponentItems)
				{
					if (itemComponentData.ArchetypeId == archetype.Id || itemComponentData.Contains(archetype, componentFilters, ++componentDepth))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005150 RID: 20816 RVA: 0x001CFCD0 File Offset: 0x001CDED0
		public bool ContainsOnly(ItemArchetype[] archetypes, ComponentParentage[] componentFilters = null, int componentDepth = 0)
		{
			ValueTuple<bool, bool> valueTuple = componentFilters.ShouldFilterOut(this.RecipeComponentId, componentDepth);
			if (valueTuple.Item1 && !valueTuple.Item2)
			{
				return false;
			}
			if (this.ComponentItems != null)
			{
				foreach (ItemArchetype itemArchetype in archetypes)
				{
					if (itemArchetype.Id != this.ArchetypeId)
					{
						return false;
					}
					bool flag = false;
					foreach (ItemComponentData itemComponentData in this.ComponentItems)
					{
						flag = (flag || itemArchetype.Id == itemComponentData.ArchetypeId || itemComponentData.ContainsOnly(archetypes, valueTuple.Item1 ? componentFilters : null, ++componentDepth));
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06005151 RID: 20817 RVA: 0x001CFD98 File Offset: 0x001CDF98
		public ItemAndComponent? FindItemProducedBy(ItemAndComponent contributor)
		{
			if (this.ComponentItems != null)
			{
				foreach (ItemComponentData itemComponentData in this.ComponentItems)
				{
					if (itemComponentData.ArchetypeId == contributor.ArchetypeId && itemComponentData.RecipeComponentId == contributor.RecipeComponentId)
					{
						return new ItemAndComponent?(new ItemAndComponent
						{
							ArchetypeId = this.ArchetypeId,
							RecipeComponentId = this.RecipeComponentId
						});
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

		// Token: 0x06005152 RID: 20818 RVA: 0x001CFE34 File Offset: 0x001CE034
		public ItemComponentData FindItemComponentDataFor(ItemAndComponent contributor)
		{
			if (this.ComponentItems != null)
			{
				if (this.ArchetypeId == contributor.ArchetypeId && this.RecipeComponentId == contributor.RecipeComponentId)
				{
					return this;
				}
				ItemComponentData[] componentItems = this.ComponentItems;
				for (int i = 0; i < componentItems.Length; i++)
				{
					ItemComponentData itemComponentData = componentItems[i].FindItemComponentDataFor(contributor);
					if (itemComponentData != null)
					{
						return itemComponentData;
					}
				}
			}
			return null;
		}

		// Token: 0x06005153 RID: 20819 RVA: 0x001CFE98 File Offset: 0x001CE098
		public void PopulateArchetypeLeafList<T>(List<ItemArchetype> outList, ComponentParentage[] componentFilters = null, int componentDepth = 0) where T : ItemArchetype
		{
			bool flag = false;
			bool flag2 = false;
			if (componentFilters != null)
			{
				foreach (ComponentParentage componentParentage in componentFilters)
				{
					flag = (flag || (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length));
					if (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length)
					{
						flag2 = (flag2 || this.RecipeComponentId == componentParentage.ComponentList[componentDepth].ComponentId);
					}
				}
			}
			if (flag && !flag2)
			{
				return;
			}
			if (this.ComponentItems == null)
			{
				T t;
				if (InternalGameDatabase.Archetypes.TryGetAsType<T>(this.ArchetypeId, out t))
				{
					outList.Add(t);
					return;
				}
			}
			else
			{
				ItemComponentData[] componentItems = this.ComponentItems;
				for (int i = 0; i < componentItems.Length; i++)
				{
					componentItems[i].PopulateArchetypeLeafList<T>(outList, componentFilters, ++componentDepth);
				}
			}
		}

		// Token: 0x06005154 RID: 20820 RVA: 0x001CFF88 File Offset: 0x001CE188
		public void PopulateArchetypeLeafListWithComponents(List<ItemAndComponent> outList, ComponentParentage[] componentFilters = null, int componentDepth = 0)
		{
			bool flag = false;
			bool flag2 = false;
			if (componentFilters != null)
			{
				foreach (ComponentParentage componentParentage in componentFilters)
				{
					flag = (flag || (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length));
					if (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length)
					{
						flag2 = (flag2 || this.RecipeComponentId == componentParentage.ComponentList[componentDepth].ComponentId);
					}
				}
			}
			if (flag && !flag2)
			{
				return;
			}
			if (this.ComponentItems == null)
			{
				ItemArchetype itemArchetype;
				if (InternalGameDatabase.Archetypes.TryGetAsType<ItemArchetype>(this.ArchetypeId, out itemArchetype))
				{
					outList.Add(new ItemAndComponent
					{
						ArchetypeId = this.ArchetypeId,
						RecipeComponentId = this.RecipeComponentId
					});
					return;
				}
			}
			else
			{
				ItemComponentData[] componentItems = this.ComponentItems;
				for (int i = 0; i < componentItems.Length; i++)
				{
					componentItems[i].PopulateArchetypeLeafListWithComponents(outList, componentFilters, ++componentDepth);
				}
			}
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x001D008C File Offset: 0x001CE28C
		public void PopulateRawMaterialsListWithQualities(List<RawComponentWithQuality> outList, float? quality = null, ComponentParentage[] componentFilters = null, int componentDepth = 0)
		{
			bool flag = false;
			bool flag2 = false;
			if (componentFilters != null)
			{
				foreach (ComponentParentage componentParentage in componentFilters)
				{
					flag = (flag || (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length));
					if (((componentParentage != null) ? componentParentage.ComponentList : null) != null && componentDepth < componentParentage.ComponentList.Length)
					{
						flag2 = (flag2 || this.RecipeComponentId == componentParentage.ComponentList[componentDepth].ComponentId);
					}
				}
			}
			if (flag && !flag2)
			{
				return;
			}
			int num = (int)((quality ?? 100f) / 100f * (float)((this.QualityModifier ?? 100) / 100) * 100f);
			if (this.ComponentItems == null)
			{
				RawComponent rawComponent;
				if (InternalGameDatabase.Archetypes.TryGetAsType<RawComponent>(this.ArchetypeId, out rawComponent))
				{
					RawComponentWithQuality fromPool = StaticPool<RawComponentWithQuality>.GetFromPool();
					fromPool.RawComponent = rawComponent;
					fromPool.Quality = num;
					outList.Add(fromPool);
					return;
				}
			}
			else
			{
				ItemComponentData[] componentItems = this.ComponentItems;
				for (int i = 0; i < componentItems.Length; i++)
				{
					componentItems[i].PopulateRawMaterialsListWithQualities(outList, new float?((float)num), componentFilters, ++componentDepth);
				}
			}
		}

		// Token: 0x06005156 RID: 20822 RVA: 0x001D01E8 File Offset: 0x001CE3E8
		public bool Equals(ItemComponentData other)
		{
			bool flag;
			if (other != null && this.ArchetypeId == other.ArchetypeId && this.RecipeComponentId == other.RecipeComponentId)
			{
				int? qualityModifier = this.QualityModifier;
				int? num = other.QualityModifier;
				if ((qualityModifier.GetValueOrDefault() == num.GetValueOrDefault() & qualityModifier != null == (num != null)) && ((this.ComponentItems == null && other.ComponentItems == null) || (this.ComponentItems != null && other.ComponentItems != null)))
				{
					if (this.ComponentItems != null)
					{
						int num2 = this.ComponentItems.Length;
						ItemComponentData[] componentItems = other.ComponentItems;
						num = ((componentItems != null) ? new int?(componentItems.Length) : null);
						flag = (num2 == num.GetValueOrDefault() & num != null);
						goto IL_C7;
					}
					flag = true;
					goto IL_C7;
				}
			}
			flag = false;
			IL_C7:
			bool flag2 = flag;
			if (flag2 && this.ComponentItems != null && other.ComponentItems != null)
			{
				for (int i = 0; i < this.ComponentItems.Length; i++)
				{
					flag2 = (flag2 && this.ComponentItems[i].Equals(other.ComponentItems[i]));
				}
			}
			return flag2;
		}

		// Token: 0x06005157 RID: 20823 RVA: 0x001D0300 File Offset: 0x001CE500
		public override int GetHashCode()
		{
			if (this.ComponentItems == null)
			{
				return this.ArchetypeId.GetHashCode() * 397 ^ this.RecipeComponentId.GetHashCode();
			}
			Array.Sort<ItemComponentData>(this.ComponentItems, (ItemComponentData x, ItemComponentData y) => x.GetHashCode().CompareTo(y.GetHashCode()));
			int num = this.ArchetypeId.GetHashCode();
			num = (num * 397 ^ this.RecipeComponentId.GetHashCode());
			foreach (ItemComponentData itemComponentData in this.ComponentItems)
			{
				num = (num * 397 ^ itemComponentData.GetHashCode());
			}
			return num;
		}

		// Token: 0x040048A1 RID: 18593
		public UniqueId ArchetypeId;

		// Token: 0x040048A2 RID: 18594
		public UniqueId RecipeComponentId;

		// Token: 0x040048A3 RID: 18595
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? QualityModifier;

		// Token: 0x040048A4 RID: 18596
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ItemComponentData[] ComponentItems;
	}
}
