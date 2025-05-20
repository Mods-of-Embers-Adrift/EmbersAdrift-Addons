using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ABC RID: 2748
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Learnable Title")]
	public class LearnableTitle : LearnableArchetype
	{
		// Token: 0x060054EE RID: 21742 RVA: 0x00078C5A File Offset: 0x00076E5A
		public string GetFormattedTitle()
		{
			if (string.IsNullOrEmpty(this.m_formattedTitle))
			{
				this.m_formattedTitle = (this.m_colorTitle ? this.DisplayName.Color(this.m_titleColor) : this.DisplayName);
			}
			return this.m_formattedTitle;
		}

		// Token: 0x04004B6B RID: 19307
		[SerializeField]
		private bool m_colorTitle;

		// Token: 0x04004B6C RID: 19308
		[SerializeField]
		private Color m_titleColor = Color.white;

		// Token: 0x04004B6D RID: 19309
		[NonSerialized]
		private string m_formattedTitle;
	}
}
