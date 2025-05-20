using System;
using SoL.Game.Crafting;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A6C RID: 2668
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Crafting Tool")]
	public class CraftingToolItem : EquipableItem, IDurability
	{
		// Token: 0x170012BB RID: 4795
		// (get) Token: 0x06005290 RID: 21136 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.Tool;
			}
		}

		// Token: 0x170012BC RID: 4796
		// (get) Token: 0x06005291 RID: 21137 RVA: 0x000771DF File Offset: 0x000753DF
		public CraftingToolType ToolType
		{
			get
			{
				return this.m_toolType;
			}
		}

		// Token: 0x170012BD RID: 4797
		// (get) Token: 0x06005292 RID: 21138 RVA: 0x000771E7 File Offset: 0x000753E7
		public int MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x170012BE RID: 4798
		// (get) Token: 0x06005293 RID: 21139 RVA: 0x000771EF File Offset: 0x000753EF
		public float ExecutionTimeMultiplier
		{
			get
			{
				return this.m_executionTimeMultiplier;
			}
		}

		// Token: 0x06005294 RID: 21140 RVA: 0x000771F7 File Offset: 0x000753F7
		public float GetCurrentDurability(float dmgAbsorbed)
		{
			return 1f - dmgAbsorbed / (float)this.m_maxDamageAbsorption;
		}

		// Token: 0x06005295 RID: 21141 RVA: 0x00077208 File Offset: 0x00075408
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.ItemData.Durability = new ItemDamage();
		}

		// Token: 0x06005296 RID: 21142 RVA: 0x001D43B8 File Offset: 0x001D25B8
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			foreach (EquipmentSlot equipmentSlot in EquipmentType.Tool.GetCachedCompatibleSlots())
			{
				ArchetypeInstance archetypeInstance;
				if (!entity.CollectionController.Equipment.TryGetInstanceForIndex((int)equipmentSlot, out archetypeInstance))
				{
					return equipmentSlot;
				}
			}
			return EquipmentSlot.Tool1;
		}

		// Token: 0x06005297 RID: 21143 RVA: 0x001D441C File Offset: 0x001D261C
		public static bool TryGetValidTool(GameEntity entity, IGatheringNode node, out CraftingToolItem tool, out ArchetypeInstance instance, out int effectiveness, out string message)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			tool = null;
			instance = null;
			effectiveness = 0;
			message = "Requires " + node.RequiredTool.ToString() + "!";
			bool result = false;
			foreach (EquipmentSlot index in EquipmentType.Tool.GetCachedCompatibleSlots())
			{
				ArchetypeInstance archetypeInstance;
				if (entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out tool) && tool.ToolType == node.RequiredTool)
				{
					ItemInstanceData itemData = archetypeInstance.ItemData;
					int? num;
					if (itemData == null)
					{
						num = null;
					}
					else
					{
						ItemDamage durability = itemData.Durability;
						num = ((durability != null) ? new int?(durability.Absorbed) : null);
					}
					int? num2 = num;
					int maxDamageAbsorption = tool.MaxDamageAbsorption;
					if (num2.GetValueOrDefault() < maxDamageAbsorption & num2 != null)
					{
						instance = archetypeInstance;
						message = "";
						result = true;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x170012BF RID: 4799
		// (get) Token: 0x06005298 RID: 21144 RVA: 0x000771E7 File Offset: 0x000753E7
		int IDurability.MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x06005299 RID: 21145 RVA: 0x00077221 File Offset: 0x00075421
		float IDurability.GetCurrentDurability(float dmgAbsorbed)
		{
			return this.GetCurrentDurability(dmgAbsorbed);
		}

		// Token: 0x170012C0 RID: 4800
		// (get) Token: 0x0600529A RID: 21146 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDurability.DegradeOnHit
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600529B RID: 21147 RVA: 0x001D455C File Offset: 0x001D275C
		public override void AddWeaponDataToTooltip(ArchetypeTooltip tooltip, bool isAutoAttack = false)
		{
			base.AddWeaponDataToTooltip(tooltip, false);
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			dataBlock.AppendLine("Tool Type:", this.m_toolType.GetTooltipDescription());
			if (this.m_executionTimeMultiplier < 1f || this.m_executionTimeMultiplier > 1f)
			{
				string right = (this.m_executionTimeMultiplier < 1f) ? ("+" + (1f - this.m_executionTimeMultiplier).GetAsPercentage() + "%") : ("-" + (this.m_executionTimeMultiplier - 1f).GetAsPercentage() + "%");
				dataBlock.AppendLine("Execution Modifier", right);
			}
		}

		// Token: 0x0600529C RID: 21148 RVA: 0x0007722A File Offset: 0x0007542A
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.MaxDamageAbsorption <= 1 || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x0600529D RID: 21149 RVA: 0x001D4604 File Offset: 0x001D2804
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.MaxDamageAbsorption)
			{
				this.m_maxDamageAbsorption = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_maxDamageAbsorption);
				return true;
			}
			if (assignerName == ComponentEffectAssignerName.MinimumToolEffectiveness)
			{
				this.m_minimumEffectiveness = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_minimumEffectiveness);
				return true;
			}
			if (assignerName != ComponentEffectAssignerName.ExecutionTimeMultiplier)
			{
				return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
			}
			this.m_executionTimeMultiplier = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_executionTimeMultiplier);
			return true;
		}

		// Token: 0x040049DB RID: 18907
		[SerializeField]
		private CraftingToolType m_toolType;

		// Token: 0x040049DC RID: 18908
		[SerializeField]
		protected int m_maxDamageAbsorption = 1000;

		// Token: 0x040049DD RID: 18909
		[HideInInspector]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_minimumEffectiveness = 0.5f;

		// Token: 0x040049DE RID: 18910
		[Range(0.2f, 2f)]
		[SerializeField]
		private float m_executionTimeMultiplier = 1f;
	}
}
