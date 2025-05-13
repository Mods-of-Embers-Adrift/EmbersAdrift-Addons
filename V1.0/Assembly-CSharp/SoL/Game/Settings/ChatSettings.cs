using System;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200071F RID: 1823
	[Serializable]
	public class ChatSettings
	{
		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x060036CB RID: 14027 RVA: 0x00065804 File Offset: 0x00063A04
		public Color NpcNameCombatLogColor
		{
			get
			{
				return this.m_npcNameCombatLogColor;
			}
		}

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x060036CC RID: 14028 RVA: 0x0006580C File Offset: 0x00063A0C
		public float AutoAttackColorMultiplier
		{
			get
			{
				return this.m_autoAttackColorMultiplier;
			}
		}

		// Token: 0x040034F0 RID: 13552
		public float OverheadChatTime = 5f;

		// Token: 0x040034F1 RID: 13553
		public MessageColorSetting[] MessageColorSettings;

		// Token: 0x040034F2 RID: 13554
		[SerializeField]
		private Color m_npcNameCombatLogColor = Color.white;

		// Token: 0x040034F3 RID: 13555
		[Range(0f, 1f)]
		[SerializeField]
		private float m_autoAttackColorMultiplier = 0.8f;
	}
}
