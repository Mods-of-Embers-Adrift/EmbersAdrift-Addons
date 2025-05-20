using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.Crafting;
using SoL.Game.Loot;
using SoL.Game.Settings;
using SoL.Networking;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A3C RID: 2620
	[BsonIgnoreExtraElements]
	[Serializable]
	public class ItemInstanceData : INetworkSerializable
	{
		// Token: 0x1400010B RID: 267
		// (add) Token: 0x060050FA RID: 20730 RVA: 0x001CE128 File Offset: 0x001CC328
		// (remove) Token: 0x060050FB RID: 20731 RVA: 0x001CE160 File Offset: 0x001CC360
		public event Action CountChanged;

		// Token: 0x1400010C RID: 268
		// (add) Token: 0x060050FC RID: 20732 RVA: 0x001CE198 File Offset: 0x001CC398
		// (remove) Token: 0x060050FD RID: 20733 RVA: 0x001CE1D0 File Offset: 0x001CC3D0
		public event Action ChargesChanged;

		// Token: 0x1400010D RID: 269
		// (add) Token: 0x060050FE RID: 20734 RVA: 0x001CE208 File Offset: 0x001CC408
		// (remove) Token: 0x060050FF RID: 20735 RVA: 0x001CE240 File Offset: 0x001CC440
		public event Action QuantityChanged;

		// Token: 0x1400010E RID: 270
		// (add) Token: 0x06005100 RID: 20736 RVA: 0x001CE278 File Offset: 0x001CC478
		// (remove) Token: 0x06005101 RID: 20737 RVA: 0x001CE2B0 File Offset: 0x001CC4B0
		public event Action AugmentChanged;

		// Token: 0x1400010F RID: 271
		// (add) Token: 0x06005102 RID: 20738 RVA: 0x001CE2E8 File Offset: 0x001CC4E8
		// (remove) Token: 0x06005103 RID: 20739 RVA: 0x001CE320 File Offset: 0x001CC520
		public event Action<bool> LockFlagsChanged;

		// Token: 0x17001217 RID: 4631
		// (get) Token: 0x06005104 RID: 20740 RVA: 0x0007623B File Offset: 0x0007443B
		// (set) Token: 0x06005105 RID: 20741 RVA: 0x00076243 File Offset: 0x00074443
		[BsonIgnore]
		[JsonIgnore]
		public ILootRollSource CurrentLootRollSource { get; set; }

		// Token: 0x17001218 RID: 4632
		// (get) Token: 0x06005106 RID: 20742 RVA: 0x0007624C File Offset: 0x0007444C
		// (set) Token: 0x06005107 RID: 20743 RVA: 0x00076254 File Offset: 0x00074454
		[BsonIgnore]
		[JsonIgnore]
		public bool Locked
		{
			get
			{
				return this.m_locked;
			}
			set
			{
				this.m_locked = value;
				Action<bool> lockFlagsChanged = this.LockFlagsChanged;
				if (lockFlagsChanged == null)
				{
					return;
				}
				lockFlagsChanged(this.m_locked);
			}
		}

		// Token: 0x17001219 RID: 4633
		// (get) Token: 0x06005108 RID: 20744 RVA: 0x00076273 File Offset: 0x00074473
		[BsonIgnore]
		[JsonIgnore]
		public bool HasAugment
		{
			get
			{
				return this.Augment != null;
			}
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x0007627E File Offset: 0x0007447E
		public void TriggerAugmentChanged()
		{
			Action augmentChanged = this.AugmentChanged;
			if (augmentChanged == null)
			{
				return;
			}
			augmentChanged();
		}

		// Token: 0x1700121A RID: 4634
		// (get) Token: 0x0600510A RID: 20746 RVA: 0x00076290 File Offset: 0x00074490
		[BsonIgnore]
		[JsonIgnore]
		public bool HasComponents
		{
			get
			{
				ItemComponentTree itemComponentTree = this.ItemComponentTree;
				return ((itemComponentTree != null) ? itemComponentTree.Components : null) != null && this.ItemComponentTree.Components.Length != 0;
			}
		}

		// Token: 0x1700121B RID: 4635
		// (get) Token: 0x0600510B RID: 20747 RVA: 0x000762B7 File Offset: 0x000744B7
		// (set) Token: 0x0600510C RID: 20748 RVA: 0x000762BF File Offset: 0x000744BF
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? Quantity
		{
			get
			{
				return this.m_quantity;
			}
			set
			{
				this.m_quantity = value;
				Action quantityChanged = this.QuantityChanged;
				if (quantityChanged == null)
				{
					return;
				}
				quantityChanged();
			}
		}

		// Token: 0x1700121C RID: 4636
		// (get) Token: 0x0600510D RID: 20749 RVA: 0x000762B7 File Offset: 0x000744B7
		// (set) Token: 0x0600510E RID: 20750 RVA: 0x000762D8 File Offset: 0x000744D8
		[BsonIgnore]
		[JsonIgnore]
		public int? Count
		{
			get
			{
				return this.m_quantity;
			}
			set
			{
				this.Quantity = value;
				Action countChanged = this.CountChanged;
				if (countChanged == null)
				{
					return;
				}
				countChanged();
			}
		}

		// Token: 0x1700121D RID: 4637
		// (get) Token: 0x0600510F RID: 20751 RVA: 0x000762B7 File Offset: 0x000744B7
		// (set) Token: 0x06005110 RID: 20752 RVA: 0x000762F1 File Offset: 0x000744F1
		[BsonIgnore]
		[JsonIgnore]
		public int? Charges
		{
			get
			{
				return this.m_quantity;
			}
			set
			{
				this.Quantity = value;
				Action chargesChanged = this.ChargesChanged;
				if (chargesChanged == null)
				{
					return;
				}
				chargesChanged();
			}
		}

		// Token: 0x1700121E RID: 4638
		// (get) Token: 0x06005111 RID: 20753 RVA: 0x0007630A File Offset: 0x0007450A
		[BsonIgnore]
		[JsonIgnore]
		public bool IsSoulbound
		{
			get
			{
				return this.ItemFlags.IsSoulbound();
			}
		}

		// Token: 0x1700121F RID: 4639
		// (get) Token: 0x06005112 RID: 20754 RVA: 0x00076317 File Offset: 0x00074517
		[BsonIgnore]
		[JsonIgnore]
		public bool IsNoTrade
		{
			get
			{
				return this.ItemFlags.HasBitFlag(ItemFlags.NoTrade);
			}
		}

		// Token: 0x17001220 RID: 4640
		// (get) Token: 0x06005113 RID: 20755 RVA: 0x00076325 File Offset: 0x00074525
		[BsonElement]
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string SoulboundPlayerName { get; }

		// Token: 0x17001221 RID: 4641
		// (get) Token: 0x06005114 RID: 20756 RVA: 0x0007632D File Offset: 0x0007452D
		[BsonElement]
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string SoulboundPlayerId { get; }

		// Token: 0x06005115 RID: 20757 RVA: 0x00076335 File Offset: 0x00074535
		public void MarkAsSoulbound(CharacterRecord record)
		{
			this.ItemFlags |= ItemFlags.Soulbound;
		}

		// Token: 0x17001222 RID: 4642
		// (get) Token: 0x06005116 RID: 20758 RVA: 0x00076345 File Offset: 0x00074545
		// (set) Token: 0x06005117 RID: 20759 RVA: 0x0007634D File Offset: 0x0007454D
		[BsonIgnore]
		[JsonIgnore]
		public DateTime? TimeOfLastShieldBlock { get; set; }

		// Token: 0x17001223 RID: 4643
		// (get) Token: 0x06005118 RID: 20760 RVA: 0x00076356 File Offset: 0x00074556
		// (set) Token: 0x06005119 RID: 20761 RVA: 0x0007635E File Offset: 0x0007455E
		[BsonIgnore]
		[JsonIgnore]
		public DiceSet? WeaponDynamicDiceSet { get; set; }

		// Token: 0x0600511A RID: 20762 RVA: 0x001CE358 File Offset: 0x001CC558
		public void CopyDataFrom(ItemInstanceData data)
		{
			if (data.Durability != null)
			{
				this.Durability = new ItemDamage
				{
					Absorbed = data.Durability.Absorbed,
					Repaired = data.Durability.Repaired
				};
			}
			if (data.Augment != null)
			{
				this.Augment = new ItemAugment
				{
					ArchetypeId = data.Augment.ArchetypeId,
					Count = data.Augment.Count,
					StackCount = data.Augment.StackCount
				};
			}
			if (data.ItemComponentTree != null)
			{
				this.ItemComponentTree = new ItemComponentTree();
				this.ItemComponentTree.CopyDataFrom(data.ItemComponentTree);
			}
			if (data.Quantity != null)
			{
				this.Quantity = data.Quantity;
			}
			if (data.Count != null)
			{
				this.Count = new int?(data.Count.Value);
			}
			if (data.Charges != null)
			{
				this.Charges = new int?(data.Charges.Value);
			}
			if (data.AssociatedMasteryId != null)
			{
				this.AssociatedMasteryId = new UniqueId?(data.AssociatedMasteryId.Value);
			}
			this.PlayerName = data.PlayerName;
			this.ItemFlags = data.ItemFlags;
			this.VisualIndex = data.VisualIndex;
			this.ColorIndex = data.ColorIndex;
		}

		// Token: 0x0600511B RID: 20763 RVA: 0x001CE4C8 File Offset: 0x001CC6C8
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.Durability != null);
			ItemDamage durability = this.Durability;
			if (durability != null)
			{
				durability.PackData(buffer);
			}
			buffer.AddBool(this.Augment != null);
			ItemAugment augment = this.Augment;
			if (augment != null)
			{
				augment.PackData(buffer);
			}
			buffer.AddBool(this.ItemComponentTree != null);
			ItemComponentTree itemComponentTree = this.ItemComponentTree;
			if (itemComponentTree != null)
			{
				itemComponentTree.PackData(buffer);
			}
			buffer.AddNullableInt(this.Quantity);
			buffer.AddNullableByte(this.VisualIndex);
			buffer.AddNullableByte(this.ColorIndex);
			buffer.AddBool(this.ItemFlags > ItemFlags.None);
			if (this.ItemFlags != ItemFlags.None)
			{
				buffer.AddEnum(this.ItemFlags);
			}
			buffer.AddBool(this.AssociatedMasteryId != null);
			if (this.AssociatedMasteryId != null)
			{
				buffer.AddUniqueId(this.AssociatedMasteryId.Value);
			}
			buffer.AddNullableString(this.PlayerName);
			buffer.AddBool(this.Locked);
			return buffer;
		}

		// Token: 0x0600511C RID: 20764 RVA: 0x001CE5D8 File Offset: 0x001CC7D8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Durability = null;
			this.Augment = null;
			this.ItemComponentTree = null;
			this.Count = null;
			this.Charges = null;
			this.ItemFlags = ItemFlags.None;
			if (buffer.ReadBool())
			{
				this.Durability = new ItemDamage();
				this.Durability.ReadData(buffer);
			}
			if (buffer.ReadBool())
			{
				this.Augment = new ItemAugment();
				this.Augment.ReadData(buffer);
			}
			if (buffer.ReadBool())
			{
				this.ItemComponentTree = new ItemComponentTree();
				this.ItemComponentTree.ReadData(buffer);
			}
			this.Quantity = buffer.ReadNullableInt();
			this.VisualIndex = buffer.ReadNullableByte();
			this.ColorIndex = buffer.ReadNullableByte();
			if (buffer.ReadBool())
			{
				this.ItemFlags = buffer.ReadEnum<ItemFlags>();
			}
			if (buffer.ReadBool())
			{
				this.AssociatedMasteryId = new UniqueId?(buffer.ReadUniqueId());
			}
			this.PlayerName = buffer.ReadNullableString();
			this.Locked = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x0600511D RID: 20765 RVA: 0x001CE6EC File Offset: 0x001CC8EC
		public BitBuffer PackData_BinaryIDs(BitBuffer buffer)
		{
			buffer.AddBool(this.Durability != null);
			ItemDamage durability = this.Durability;
			if (durability != null)
			{
				durability.PackData(buffer);
			}
			buffer.AddBool(this.Augment != null);
			ItemAugment augment = this.Augment;
			if (augment != null)
			{
				augment.PackDataBinary(buffer);
			}
			buffer.AddBool(this.ItemComponentTree != null);
			ItemComponentTree itemComponentTree = this.ItemComponentTree;
			if (itemComponentTree != null)
			{
				itemComponentTree.PackData_BinaryIDs(buffer);
			}
			buffer.AddNullableInt(this.Quantity);
			buffer.AddNullableByte(this.VisualIndex);
			buffer.AddNullableByte(this.ColorIndex);
			buffer.AddBool(this.ItemFlags > ItemFlags.None);
			if (this.ItemFlags != ItemFlags.None)
			{
				buffer.AddEnum(this.ItemFlags);
			}
			buffer.AddBool(this.AssociatedMasteryId != null);
			if (this.AssociatedMasteryId != null)
			{
				buffer.AddUniqueId(this.AssociatedMasteryId.Value);
			}
			buffer.AddNullableString(this.PlayerName);
			buffer.AddBool(this.Locked);
			return buffer;
		}

		// Token: 0x0600511E RID: 20766 RVA: 0x001CE7FC File Offset: 0x001CC9FC
		public BitBuffer ReadData_BinaryIDs(BitBuffer buffer)
		{
			this.Durability = null;
			this.Augment = null;
			this.ItemComponentTree = null;
			this.Count = null;
			this.Charges = null;
			this.ItemFlags = ItemFlags.None;
			if (buffer.ReadBool())
			{
				this.Durability = new ItemDamage();
				this.Durability.ReadData(buffer);
			}
			if (buffer.ReadBool())
			{
				this.Augment = new ItemAugment();
				this.Augment.ReadDataBinary(buffer);
			}
			if (buffer.ReadBool())
			{
				this.ItemComponentTree = new ItemComponentTree();
				this.ItemComponentTree.ReadData_BinaryIDs(buffer);
			}
			this.Quantity = buffer.ReadNullableInt();
			this.VisualIndex = buffer.ReadNullableByte();
			this.ColorIndex = buffer.ReadNullableByte();
			if (buffer.ReadBool())
			{
				this.ItemFlags = buffer.ReadEnum<ItemFlags>();
			}
			if (buffer.ReadBool())
			{
				this.AssociatedMasteryId = new UniqueId?(buffer.ReadUniqueId());
			}
			this.PlayerName = buffer.ReadNullableString();
			this.Locked = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x0600511F RID: 20767 RVA: 0x001CE910 File Offset: 0x001CCB10
		public bool CanMergeWith(ItemInstanceData incoming)
		{
			bool flag = this.ItemFlags == incoming.ItemFlags && this.AssociatedMasteryId == incoming.AssociatedMasteryId;
			if (flag && this.ItemComponentTree != null)
			{
				flag = this.ItemComponentTree.Equals(incoming.ItemComponentTree);
			}
			return flag;
		}

		// Token: 0x06005120 RID: 20768 RVA: 0x00076367 File Offset: 0x00074567
		public int GetTreeFingerprint()
		{
			if (this.m_treeFingerprint != 0)
			{
				return this.m_treeFingerprint;
			}
			if (this.ItemComponentTree != null)
			{
				this.m_treeFingerprint = this.ItemComponentTree.GetHashCode();
				return this.m_treeFingerprint;
			}
			return 0;
		}

		// Token: 0x06005121 RID: 20769 RVA: 0x001CE990 File Offset: 0x001CCB90
		public void DeriveComponentData(ArchetypeInstance newInstance, Recipe recipe, List<ItemUsage> itemsUsed, float level)
		{
			if (itemsUsed != null)
			{
				UniqueId archetypeId = newInstance.ArchetypeId;
				ItemArchetype itemToCreateOnFailure = recipe.ItemToCreateOnFailure;
				if (!(archetypeId == ((itemToCreateOnFailure != null) ? new UniqueId?(itemToCreateOnFailure.Id) : null)) && !recipe.DisableHistory)
				{
					itemsUsed.Sort((ItemUsage x, ItemUsage y) => x.Instance.ItemData.GetTreeFingerprint() - y.Instance.ItemData.GetTreeFingerprint());
					RecipeSubstitution recipeSubstitution = null;
					if (recipe.Substitutions != null)
					{
						foreach (RecipeSubstitution recipeSubstitution2 in recipe.Substitutions)
						{
							if (recipeSubstitution2.Substitute != null)
							{
								if (recipeSubstitution2.WasMadeFrom == null || recipeSubstitution2.WasMadeFrom.Length == 0)
								{
									recipeSubstitution = recipeSubstitution2;
									break;
								}
								bool flag = true;
								foreach (ItemArchetype itemArchetype in recipeSubstitution2.WasMadeFrom)
								{
									bool flag2 = false;
									foreach (ItemUsage itemUsage in itemsUsed)
									{
										if (itemUsage.Instance.ArchetypeId == itemArchetype.Id)
										{
											flag2 = true;
										}
										else if (itemUsage.Instance.ItemData.ItemComponentTree != null && itemUsage.Instance.ItemData.ItemComponentTree.Components != null)
										{
											foreach (ItemComponentData itemComponentData in itemUsage.Instance.ItemData.ItemComponentTree.Components)
											{
												if (itemComponentData.ArchetypeId == itemArchetype.Id)
												{
													flag2 = true;
													break;
												}
												if (itemComponentData.Contains(itemArchetype, null, 0))
												{
													flag2 = true;
													break;
												}
											}
										}
										if (flag2)
										{
											break;
										}
									}
									if (!flag2)
									{
										flag = false;
										break;
									}
								}
								if (flag)
								{
									recipeSubstitution = recipeSubstitution2;
									break;
								}
							}
						}
					}
					if (recipeSubstitution == null)
					{
						List<RecipeComponent> list = itemsUsed.ComponentsFulfilled();
						ItemComponentTree itemComponentTree = new ItemComponentTree();
						itemComponentTree.Components = new ItemComponentData[list.Count];
						for (int l = 0; l < list.Count; l++)
						{
							ItemComponentData itemComponentData2 = new ItemComponentData();
							foreach (ItemUsage itemUsage2 in itemsUsed)
							{
								if (itemUsage2.UsedFor == list[l])
								{
									itemComponentData2.ArchetypeId = itemUsage2.Instance.ArchetypeId;
									itemComponentData2.RecipeComponentId = itemUsage2.UsedFor.Id;
									if (itemUsage2.Instance.ItemData.ItemComponentTree != null && itemUsage2.Instance.ItemData.ItemComponentTree.Components != null && itemUsage2.Instance.ItemData.ItemComponentTree.Components.Length != 0)
									{
										itemComponentData2.ComponentItems = new ItemComponentData[itemUsage2.Instance.ItemData.ItemComponentTree.Components.Length];
										itemComponentData2.QualityModifier = itemUsage2.Instance.ItemData.ItemComponentTree.QualityModifier;
										for (int m = 0; m < itemUsage2.Instance.ItemData.ItemComponentTree.Components.Length; m++)
										{
											itemComponentData2.ComponentItems[m] = new ItemComponentData();
											itemComponentData2.ComponentItems[m].CopyDataFrom(itemUsage2.Instance.ItemData.ItemComponentTree.Components[m]);
										}
									}
								}
							}
							itemComponentTree.Components[l] = itemComponentData2;
							if (!(recipe.GetItemToCreate(itemsUsed, level) is IStackable) && recipe.QualityModifierEnabled)
							{
								int i;
								int j;
								int k;
								int num;
								int num2;
								int num3;
								itemsUsed.GetAggregateMaterialLevel(false, out i, out j, out num, out num2, out k, out num3);
								level = Mathf.Max(level, (float)num);
								float t = (num < num2) ? ((level - (float)num) / (float)(num2 - num)) : 1f;
								itemComponentTree.QualityModifier = new int?((int)Mathf.Lerp((float)GlobalSettings.Values.Crafting.MinimumQualityModifier, (float)GlobalSettings.Values.Crafting.MaximumQualityModifier, t));
							}
						}
						newInstance.ItemData.ItemComponentTree = itemComponentTree;
					}
					else
					{
						newInstance.ItemData.ItemComponentTree = new ItemComponentTree();
						newInstance.ItemData.ItemComponentTree.Components = new ItemComponentData[1];
						newInstance.ItemData.ItemComponentTree.Components[0] = new ItemComponentData();
						newInstance.ItemData.ItemComponentTree.Components[0].ArchetypeId = recipeSubstitution.Substitute.Id;
					}
					newInstance.ItemData.GetTreeFingerprint();
					return;
				}
			}
		}

		// Token: 0x04004889 RID: 18569
		private bool m_locked;

		// Token: 0x0400488A RID: 18570
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public UniqueId? AssociatedMasteryId;

		// Token: 0x0400488B RID: 18571
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ItemDamage Durability;

		// Token: 0x0400488C RID: 18572
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ItemAugment Augment;

		// Token: 0x0400488D RID: 18573
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ItemComponentTree ItemComponentTree;

		// Token: 0x0400488E RID: 18574
		private int? m_quantity;

		// Token: 0x0400488F RID: 18575
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string PlayerName;

		// Token: 0x04004894 RID: 18580
		public ItemFlags ItemFlags;

		// Token: 0x04004895 RID: 18581
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public byte? VisualIndex;

		// Token: 0x04004896 RID: 18582
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public byte? ColorIndex;

		// Token: 0x04004897 RID: 18583
		private int m_treeFingerprint;
	}
}
