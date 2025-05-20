using System;
using SoL.Game.Animation;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CD8 RID: 3288
	[CreateAssetMenu(menuName = "SoL/Profiles/Crafting Station")]
	public class CraftingStationProfile : ScriptableObject
	{
		// Token: 0x170017BF RID: 6079
		// (get) Token: 0x0600637F RID: 25471 RVA: 0x000830CD File Offset: 0x000812CD
		public string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
		}

		// Token: 0x170017C0 RID: 6080
		// (get) Token: 0x06006380 RID: 25472 RVA: 0x000830D5 File Offset: 0x000812D5
		public CraftingStationCategory Category
		{
			get
			{
				return this.m_category;
			}
		}

		// Token: 0x170017C1 RID: 6081
		// (get) Token: 0x06006381 RID: 25473 RVA: 0x000830DD File Offset: 0x000812DD
		public AbilityAnimation Animation
		{
			get
			{
				return this.m_animation;
			}
		}

		// Token: 0x0400569D RID: 22173
		[SerializeField]
		private string m_displayName;

		// Token: 0x0400569E RID: 22174
		[SerializeField]
		private CraftingStationCategory m_category;

		// Token: 0x0400569F RID: 22175
		[SerializeField]
		private AbilityAnimation m_animation;
	}
}
