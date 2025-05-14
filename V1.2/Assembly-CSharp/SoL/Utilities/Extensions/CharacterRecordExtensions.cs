using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000329 RID: 809
	public static class CharacterRecordExtensions
	{
		// Token: 0x0600164B RID: 5707 RVA: 0x000FFA20 File Offset: 0x000FDC20
		public static ArchetypeInstance InitializeItem(ItemArchetype item, CharacterRecord characterRecord = null)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			ArchetypeInstance archetypeInstance = item.CreateNewInstance();
			if (item is RangedAmmoItem)
			{
				archetypeInstance.ItemData.Count = new int?(500);
			}
			else if (item.ArchetypeHasCount())
			{
				archetypeInstance.ItemData.Count = new int?(20);
			}
			else if (item.ArchetypeHasCharges())
			{
				archetypeInstance.ItemData.Charges = new int?(CharacterRecordExtensions.kInitialChargeCount);
			}
			archetypeInstance.MarkAsSoulbound(characterRecord);
			return archetypeInstance;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x000FFAA8 File Offset: 0x000FDCA8
		public static void InitializeCharacterStorage(this CharacterRecord record)
		{
			CharacterSettings settings = new CharacterSettings
			{
				HideEmberStone = false,
				HideHelm = false,
				SecondaryWeaponsActive = false,
				BlockInspections = false,
				PauseAdventuringExperience = false,
				TrackedMastery = new UniqueId(""),
				PortraitId = ((record.Settings == null) ? new UniqueId("") : record.Settings.PortraitId)
			};
			ContainerRecord containerRecord = new ContainerRecord
			{
				Type = ContainerType.Equipment,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord2 = new ContainerRecord
			{
				Type = ContainerType.Inventory,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord3 = new ContainerRecord
			{
				Type = ContainerType.Pouch,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord4 = new ContainerRecord
			{
				Type = ContainerType.ReagentPouch,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord5 = new ContainerRecord
			{
				Type = ContainerType.PersonalBank,
				Instances = new List<ArchetypeInstance>()
			};
			ContainerRecord containerRecord6 = new ContainerRecord
			{
				Type = ContainerType.Gathering,
				Instances = new List<ArchetypeInstance>()
			};
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			StartingData startingData = GlobalSettings.Values.Player.StartingData;
			for (int i = 0; i < startingData.Equipment.Length; i++)
			{
				if (startingData.Equipment[i] == null)
				{
					Debug.LogWarning(string.Format("Encountered a NULL starting equipment item in index {0}!", i));
				}
				else
				{
					bool flag = false;
					ItemArchetype itemArchetype = startingData.Equipment[i];
					ArchetypeInstance archetypeInstance = CharacterRecordExtensions.InitializeItem(itemArchetype, record);
					if (archetypeInstance.ItemData != null)
					{
						archetypeInstance.ItemData.ItemFlags |= ItemFlags.NoSale;
					}
					EquipableItem equipableItem;
					if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(itemArchetype.Id, out equipableItem))
					{
						using (IEnumerator<EquipmentSlot> enumerator = equipableItem.Type.GetCachedCompatibleSlots().GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								int num4 = (int)enumerator.Current;
								bool flag2 = false;
								for (int j = 0; j < containerRecord.Instances.Count; j++)
								{
									flag2 = (flag2 || containerRecord.Instances[j].Index == num4);
								}
								if (!flag2)
								{
									archetypeInstance.Index = num4;
									containerRecord.Instances.Add(archetypeInstance);
									flag = true;
									break;
								}
							}
						}
					}
					if (!flag)
					{
						archetypeInstance.Index = num3;
						containerRecord2.Instances.Add(archetypeInstance);
						num3++;
					}
				}
			}
			for (int k = 0; k < startingData.Inventory.Length; k++)
			{
				if (startingData.Inventory[k] == null)
				{
					Debug.LogWarning(string.Format("Encountered a NULL starting inventory item in index {0}!", k));
				}
				else
				{
					ArchetypeInstance archetypeInstance2 = startingData.Inventory[k].InitializeItem();
					archetypeInstance2.MarkAsSoulbound(record);
					if (archetypeInstance2.ItemData != null)
					{
						archetypeInstance2.ItemData.ItemFlags |= ItemFlags.NoSale;
					}
					if (archetypeInstance2.Archetype is ReagentItem && num2 < ContainerType.ReagentPouch.GetMaxCapacity())
					{
						containerRecord4.Instances.Add(archetypeInstance2);
						archetypeInstance2.Index = num2;
						num2++;
					}
					else if (archetypeInstance2.Archetype is ConsumableItemStackable && num < ContainerType.Pouch.GetMaxCapacity())
					{
						containerRecord3.Instances.Add(archetypeInstance2);
						archetypeInstance2.Index = num;
						num++;
					}
					else
					{
						containerRecord2.Instances.Add(archetypeInstance2);
						archetypeInstance2.Index = num3;
						num3++;
					}
				}
			}
			ContainerRecord containerRecord7 = new ContainerRecord
			{
				Type = ContainerType.Masteries,
				Instances = new List<ArchetypeInstance>(startingData.Masteries.Length)
			};
			ContainerRecord containerRecord8 = new ContainerRecord
			{
				Type = ContainerType.Abilities,
				Instances = new List<ArchetypeInstance>(startingData.Abilities.Length)
			};
			LearnableContainerRecord learnableContainerRecord = new LearnableContainerRecord
			{
				Type = ContainerType.Recipes,
				LearnableIds = new List<UniqueId>(startingData.Recipes.Length)
			};
			for (int l = 0; l < startingData.Masteries.Length; l++)
			{
				if (startingData.Masteries[l] == null)
				{
					Debug.LogWarning(string.Format("Encountered a NULL starting mastery in index {0}!", l));
				}
				else
				{
					ArchetypeInstance archetypeInstance3 = startingData.Masteries[l].CreateNewInstance();
					archetypeInstance3.MasteryData.BaseLevel = ((GameManager.IsServer && ServerGameManager.GameServerConfig != null) ? ((float)ServerGameManager.GameServerConfig.StartingLevel) : 1f);
					containerRecord7.Instances.Add(archetypeInstance3);
				}
			}
			for (int m = 0; m < startingData.Abilities.Length; m++)
			{
				if (startingData.Abilities[m] == null)
				{
					Debug.LogWarning(string.Format("Encountered a NULL starting ability in index {0}!", m));
				}
				else
				{
					ArchetypeInstance archetypeInstance4 = startingData.Abilities[m].CreateNewInstance();
					containerRecord8.Instances.Add(archetypeInstance4);
					if (m < 10)
					{
						archetypeInstance4.AbilityData.MemorizationTimestamp = new DateTime?(DateTime.MinValue);
						archetypeInstance4.Index = m;
					}
					else
					{
						archetypeInstance4.Index = -1;
					}
				}
			}
			for (int n = 0; n < startingData.Recipes.Length; n++)
			{
				if (startingData.Recipes[n] == null)
				{
					Debug.LogWarning(string.Format("Encountered a NULL starting recipe in index {0}!", n));
				}
				else
				{
					Recipe recipe = startingData.Recipes[n];
					learnableContainerRecord.LearnableIds.Add(recipe.Id);
				}
			}
			if (record.Storage == null)
			{
				record.Storage = new Dictionary<ContainerType, ContainerRecord>(default(ContainerTypeComparer));
			}
			else
			{
				record.Storage.Clear();
			}
			if (record.Learnables == null)
			{
				record.Learnables = new Dictionary<ContainerType, LearnableContainerRecord>(default(ContainerTypeComparer));
			}
			else
			{
				record.Learnables.Clear();
			}
			if (record.Discoveries == null)
			{
				record.Discoveries = new Dictionary<ZoneId, List<UniqueId>>(default(ZoneIdComparer));
			}
			else
			{
				record.Discoveries.Clear();
			}
			if (GameManager.IsServer)
			{
				containerRecord2.Currency = new ulong?(GlobalSettings.Values.Player.StartingCurrency.GetCurrency());
			}
			else
			{
				containerRecord2.Currency = new ulong?(12345UL);
			}
			record.Storage.Add(containerRecord.Type, containerRecord);
			record.Storage.Add(containerRecord2.Type, containerRecord2);
			record.Storage.Add(containerRecord3.Type, containerRecord3);
			record.Storage.Add(containerRecord4.Type, containerRecord4);
			record.Storage.Add(containerRecord7.Type, containerRecord7);
			record.Storage.Add(containerRecord8.Type, containerRecord8);
			record.Storage.Add(containerRecord5.Type, containerRecord5);
			record.Storage.Add(containerRecord6.Type, containerRecord6);
			record.Learnables.Add(learnableContainerRecord.Type, learnableContainerRecord);
			record.Settings = settings;
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x00100188 File Offset: 0x000FE388
		public static BaseRole GetFirstRole(this CharacterRecord record)
		{
			ContainerRecord containerRecord;
			if (record != null && record.Storage != null && record.Storage.TryGetValue(ContainerType.Masteries, out containerRecord))
			{
				if (containerRecord == null || containerRecord.Instances == null)
				{
					return null;
				}
				for (int i = 0; i < containerRecord.Instances.Count; i++)
				{
					BaseRole result;
					if (containerRecord.Instances[i].Archetype.TryGetAsType(out result))
					{
						return result;
					}
				}
			}
			return null;
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x001001F4 File Offset: 0x000FE3F4
		public static bool TryGetLoginRoleData(this CharacterRecord record, out CharacterRecordExtensions.LoginRoleData loginRoleData)
		{
			loginRoleData = default(CharacterRecordExtensions.LoginRoleData);
			ContainerRecord containerRecord;
			if (record == null || record.Storage == null || !record.Storage.TryGetValue(ContainerType.Masteries, out containerRecord))
			{
				return false;
			}
			for (int i = 0; i < containerRecord.Instances.Count; i++)
			{
				ArchetypeInstance archetypeInstance = containerRecord.Instances[i];
				BaseRole baseRole;
				if (archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out baseRole) && baseRole.Type == MasteryType.Combat)
				{
					loginRoleData = new CharacterRecordExtensions.LoginRoleData
					{
						Name = baseRole.DisplayName,
						Icon = baseRole.Icon,
						IconTint = baseRole.IconTint,
						Level = Mathf.FloorToInt(archetypeInstance.MasteryData.BaseLevel)
					};
					SpecializedRole specializedRole;
					if (archetypeInstance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(archetypeInstance.MasteryData.Specialization.Value, out specializedRole))
					{
						loginRoleData.Name = specializedRole.DisplayName;
						loginRoleData.Icon = specializedRole.Icon;
						loginRoleData.IconTint = specializedRole.IconTint;
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001E46 RID: 7750
		private static int kInitialChargeCount = 1000;

		// Token: 0x0200032A RID: 810
		public struct LoginRoleData
		{
			// Token: 0x04001E47 RID: 7751
			public string Name;

			// Token: 0x04001E48 RID: 7752
			public Sprite Icon;

			// Token: 0x04001E49 RID: 7753
			public Color IconTint;

			// Token: 0x04001E4A RID: 7754
			public int Level;
		}
	}
}
