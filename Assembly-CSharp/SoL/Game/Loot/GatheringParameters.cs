using System;
using Cysharp.Text;
using NetStack.Serialization;
using SoL.Game.Crafting;
using SoL.Game.Interactives;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;

namespace SoL.Game.Loot
{
	// Token: 0x02000B04 RID: 2820
	public struct GatheringParameters : INetworkSerializable, IEquatable<GatheringParameters>
	{
		// Token: 0x17001472 RID: 5234
		// (get) Token: 0x06005716 RID: 22294 RVA: 0x00079FE2 File Offset: 0x000781E2
		private GatheringAbility Ability
		{
			get
			{
				if (this.m_gatheringAbility == null && !this.AbilityId.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<GatheringAbility>(this.AbilityId, out this.m_gatheringAbility);
				}
				return this.m_gatheringAbility;
			}
		}

		// Token: 0x17001473 RID: 5235
		// (get) Token: 0x06005717 RID: 22295 RVA: 0x0007A01C File Offset: 0x0007821C
		private DynamicAbility DynamicAbility
		{
			get
			{
				if (this.m_dynamicAbility == null && !this.AbilityId.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<DynamicAbility>(this.AbilityId, out this.m_dynamicAbility);
				}
				return this.m_dynamicAbility;
			}
		}

		// Token: 0x17001474 RID: 5236
		// (get) Token: 0x06005718 RID: 22296 RVA: 0x001E2734 File Offset: 0x001E0934
		private ItemArchetype RequiredItem
		{
			get
			{
				if (this.m_requiredItem == null && this.RequiredItemId != null && !this.RequiredItemId.Value.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<ItemArchetype>(this.RequiredItemId.Value, out this.m_requiredItem);
				}
				return this.m_requiredItem;
			}
		}

		// Token: 0x06005719 RID: 22297 RVA: 0x001E2794 File Offset: 0x001E0994
		public ArchetypeInstance GetAbilityInstance(GameEntity entity)
		{
			if (!entity || entity.CollectionController == null || entity.CollectionController.Abilities == null)
			{
				return null;
			}
			ArchetypeInstance result = null;
			if (this.RequiredTool == CraftingToolType.None)
			{
				if (this.DynamicAbility)
				{
					result = this.DynamicAbility.DynamicallyLoad(entity.CollectionController.Abilities);
				}
			}
			else if (this.Ability)
			{
				entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.Ability.Id, out result);
			}
			return result;
		}

		// Token: 0x0600571A RID: 22298 RVA: 0x0007A056 File Offset: 0x00078256
		public string GetGatheringMasteryDisplayName()
		{
			if (!(this.Ability != null) || !(this.Ability.Mastery != null))
			{
				return "UNKNOWN";
			}
			return this.Ability.Mastery.DisplayName;
		}

		// Token: 0x0600571B RID: 22299 RVA: 0x0007A08F File Offset: 0x0007828F
		public MasteryArchetype GetGatheringMastery()
		{
			if (!(this.Ability != null))
			{
				return null;
			}
			return this.Ability.Mastery;
		}

		// Token: 0x0600571C RID: 22300 RVA: 0x0007A0AC File Offset: 0x000782AC
		public CursorType GetCursor(bool isActive)
		{
			if (this.RequiredTool != CraftingToolType.None)
			{
				return this.RequiredTool.GetCursorForTool(isActive);
			}
			if (!isActive)
			{
				return CursorType.GloveCursorInactive;
			}
			return CursorType.GloveCursor;
		}

		// Token: 0x0600571D RID: 22301 RVA: 0x001E281C File Offset: 0x001E0A1C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.RequiredTool);
			buffer.AddNullableUniqueId(this.RequiredItemId);
			buffer.AddUniqueId(this.AbilityId);
			buffer.AddInt(this.GatherTime);
			buffer.AddInt(this.Level);
			buffer.AddBool(this.RemoveRequiredItemOnUse);
			return buffer;
		}

		// Token: 0x0600571E RID: 22302 RVA: 0x001E2878 File Offset: 0x001E0A78
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.RequiredTool = buffer.ReadEnum<CraftingToolType>();
			this.RequiredItemId = buffer.ReadNullableUniqueId();
			this.AbilityId = buffer.ReadUniqueId();
			this.GatherTime = buffer.ReadInt();
			this.Level = buffer.ReadInt();
			this.RemoveRequiredItemOnUse = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x0600571F RID: 22303 RVA: 0x001E28D0 File Offset: 0x001E0AD0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"RequiredTool: ",
				this.RequiredTool.ToString(),
				", AbilityId: ",
				this.AbilityId.ToString(),
				", GatherTime: ",
				this.GatherTime.ToString(),
				", Level: ",
				this.Level.ToString()
			});
		}

		// Token: 0x06005720 RID: 22304 RVA: 0x001E294C File Offset: 0x001E0B4C
		public string GetTooltipDescription(IGatheringNode node)
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				float num = (float)this.GatherTime;
				string arg = "Interaction time";
				utf16ValueStringBuilder.AppendFormat<int>("Level: {0}", this.Level);
				utf16ValueStringBuilder.AppendLine();
				if (this.RequiredTool != CraftingToolType.None)
				{
					arg = "Gather time";
					utf16ValueStringBuilder.AppendFormat<string>("Req Tool: {0}", this.RequiredTool.GetTooltipDescription());
					utf16ValueStringBuilder.AppendLine();
					CraftingToolItem craftingToolItem;
					ArchetypeInstance archetypeInstance;
					int num2;
					string text;
					if (LocalPlayer.GameEntity && CraftingToolItem.TryGetValidTool(LocalPlayer.GameEntity, node, out craftingToolItem, out archetypeInstance, out num2, out text))
					{
						num *= craftingToolItem.ExecutionTimeMultiplier;
					}
				}
				if (this.RequiredItem && (!this.RemoveRequiredItemOnUse || !node.InteractiveFlags.HasBitFlag(InteractiveFlags.RecordGenerated)))
				{
					utf16ValueStringBuilder.AppendFormat<string>("Req Item: {0}", this.RequiredItem.DisplayName);
					if (this.RemoveRequiredItemOnUse)
					{
						utf16ValueStringBuilder.Append(" (consumed)");
					}
					utf16ValueStringBuilder.AppendLine();
				}
				utf16ValueStringBuilder.AppendFormat<string, string>("{0}: {1}", arg, num.FormattedTime(1));
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005721 RID: 22305 RVA: 0x001E2A7C File Offset: 0x001E0C7C
		public bool EntityCanInteract(GameEntity entity, IGatheringNode node, out ArchetypeInstance requiredItemInstance)
		{
			requiredItemInstance = null;
			if (node == null)
			{
				return false;
			}
			bool flag = this.EntityMeetsLevelRequirements(entity);
			if (flag && this.RequiredItem && (!this.RemoveRequiredItemOnUse || !node.InteractiveFlags.HasBitFlag(InteractiveFlags.RecordGenerated)))
			{
				flag = (this.EntityHasItemInContainer(entity, ContainerType.Pouch, out requiredItemInstance) || this.EntityHasItemInContainer(entity, ContainerType.Inventory, out requiredItemInstance));
				if (flag && entity && entity.IsMissingBag && requiredItemInstance != null && requiredItemInstance.ContainerInstance != null && requiredItemInstance.ContainerInstance.ContainerType.LockedInDeath())
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06005722 RID: 22306 RVA: 0x001E2B10 File Offset: 0x001E0D10
		private bool EntityMeetsLevelRequirements(GameEntity entity)
		{
			if (!entity)
			{
				return false;
			}
			if (this.RequiredTool == CraftingToolType.None)
			{
				return entity.CharacterData && entity.CharacterData.AdventuringLevel >= this.Level;
			}
			ArchetypeInstance archetypeInstance;
			return this.Ability != null && this.Ability.Mastery != null && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.Ability.Mastery.Id, out archetypeInstance) && archetypeInstance.GetAssociatedLevel(entity) >= (float)this.Level;
		}

		// Token: 0x06005723 RID: 22307 RVA: 0x001E2BC4 File Offset: 0x001E0DC4
		private bool EntityHasItemInContainer(GameEntity entity, ContainerType containerType, out ArchetypeInstance itemInstance)
		{
			itemInstance = null;
			ContainerInstance containerInstance;
			return entity && this.RequiredItem && entity.CollectionController != null && entity.CollectionController.TryGetInstance(containerType, out containerInstance) && containerInstance.TryGetInstanceForArchetypeId(this.RequiredItem.Id, out itemInstance);
		}

		// Token: 0x06005724 RID: 22308 RVA: 0x001E2C18 File Offset: 0x001E0E18
		public bool Equals(GatheringParameters other)
		{
			return this.RequiredTool == other.RequiredTool && Nullable.Equals<UniqueId>(this.RequiredItemId, other.RequiredItemId) && this.AbilityId.Equals(other.AbilityId) && this.GatherTime == other.GatherTime && this.Level == other.Level && this.RemoveRequiredItemOnUse == other.RemoveRequiredItemOnUse;
		}

		// Token: 0x06005725 RID: 22309 RVA: 0x001E2C88 File Offset: 0x001E0E88
		public override bool Equals(object obj)
		{
			if (obj is GatheringParameters)
			{
				GatheringParameters other = (GatheringParameters)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06005726 RID: 22310 RVA: 0x0007A0CA File Offset: 0x000782CA
		public override int GetHashCode()
		{
			return HashCode.Combine<int, UniqueId?, UniqueId, int, int, bool>((int)this.RequiredTool, this.RequiredItemId, this.AbilityId, this.GatherTime, this.Level, this.RemoveRequiredItemOnUse);
		}

		// Token: 0x04004CDD RID: 19677
		public CraftingToolType RequiredTool;

		// Token: 0x04004CDE RID: 19678
		public UniqueId? RequiredItemId;

		// Token: 0x04004CDF RID: 19679
		public UniqueId AbilityId;

		// Token: 0x04004CE0 RID: 19680
		public int GatherTime;

		// Token: 0x04004CE1 RID: 19681
		public int Level;

		// Token: 0x04004CE2 RID: 19682
		public bool RemoveRequiredItemOnUse;

		// Token: 0x04004CE3 RID: 19683
		private GatheringAbility m_gatheringAbility;

		// Token: 0x04004CE4 RID: 19684
		private DynamicAbility m_dynamicAbility;

		// Token: 0x04004CE5 RID: 19685
		private ItemArchetype m_requiredItem;
	}
}
