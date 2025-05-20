using System;
using SoL.Game.Audio;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AAE RID: 2734
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Repair Kit Item")]
	public class RepairKitItem : StackableUtilityItem
	{
		// Token: 0x17001367 RID: 4967
		// (get) Token: 0x06005478 RID: 21624 RVA: 0x000786B0 File Offset: 0x000768B0
		protected override AudioClipCollection ClipCollection
		{
			get
			{
				return GlobalSettings.Values.Audio.RepairModeClipCollection;
			}
		}

		// Token: 0x06005479 RID: 21625 RVA: 0x0007879B File Offset: 0x0007699B
		protected override bool IsValidItem(ArchetypeInstance targetInstance)
		{
			return targetInstance != null && targetInstance.Archetype != null && targetInstance.ItemData != null && targetInstance.ItemData.Durability != null && targetInstance.ItemData.Durability.Absorbed > 0;
		}

		// Token: 0x0600547A RID: 21626 RVA: 0x00053500 File Offset: 0x00051700
		public override CursorType GetCursorType()
		{
			return CursorType.AnvilCursor;
		}

		// Token: 0x0600547B RID: 21627 RVA: 0x000787D8 File Offset: 0x000769D8
		protected override void ClientRequestExecuteUtilityInternal(GameEntity entity, ArchetypeInstance sourceItemInstance, ArchetypeInstance targetItemInstance)
		{
			base.SendExecuteUtilityRequest(entity, sourceItemInstance, targetItemInstance);
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x001DAA78 File Offset: 0x001D8C78
		protected override bool ExecuteUtilityInternal(GameEntity entity, ArchetypeInstance targetItemInstance)
		{
			if (targetItemInstance != null && targetItemInstance.ItemData != null && targetItemInstance.ItemData.Durability != null)
			{
				targetItemInstance.ItemData.Durability.RepairAmount(this.m_repairAmount);
				if (entity && entity.Vitals)
				{
					entity.Vitals.RecalculateTotalArmorClass();
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x000787E3 File Offset: 0x000769E3
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			tooltip.DataBlock.AppendLine("Damage Repaired:", this.m_repairAmount.ToString());
		}

		// Token: 0x04004B18 RID: 19224
		[SerializeField]
		private int m_repairAmount = 100;
	}
}
