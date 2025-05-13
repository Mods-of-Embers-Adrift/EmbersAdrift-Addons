using System;
using SoL.Game.EffectSystem;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ADE RID: 2782
	[CreateAssetMenu(menuName = "SoL/Profiles/Armor Piece")]
	public class ArmorPieceProfile : ScriptableObject
	{
		// Token: 0x060055CB RID: 21963 RVA: 0x001DF9F8 File Offset: 0x001DDBF8
		public void FillTooltipText(TooltipTextBlock block)
		{
			if (this.StatModifiers != null && this.StatModifiers.Length != 0)
			{
				block.AppendLine("Stat Modifiers:", 0);
				for (int i = 0; i < this.StatModifiers.Length; i++)
				{
					this.StatModifiers[i].AddToTooltipBlock(block, null);
				}
			}
		}

		// Token: 0x04004C1B RID: 19483
		public StatModifier[] StatModifiers;
	}
}
