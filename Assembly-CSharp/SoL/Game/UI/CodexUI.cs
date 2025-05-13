using System;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000863 RID: 2147
	public class CodexUI : DraggableUIWindow
	{
		// Token: 0x17000E55 RID: 3669
		// (get) Token: 0x06003E09 RID: 15881 RVA: 0x00069EFD File Offset: 0x000680FD
		public CodexMasteryUI Mastery
		{
			get
			{
				return this.m_mastery;
			}
		}

		// Token: 0x17000E56 RID: 3670
		// (get) Token: 0x06003E0A RID: 15882 RVA: 0x00069F05 File Offset: 0x00068105
		public CodexAbilityUI Ability
		{
			get
			{
				return this.m_ability;
			}
		}

		// Token: 0x06003E0B RID: 15883 RVA: 0x00069F0D File Offset: 0x0006810D
		protected override void Awake()
		{
			base.Awake();
			this.m_mastery.InstanceRemoved += this.MasteryOnInstanceRemoved;
		}

		// Token: 0x06003E0C RID: 15884 RVA: 0x00069F2C File Offset: 0x0006812C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_mastery.InstanceRemoved -= this.MasteryOnInstanceRemoved;
		}

		// Token: 0x06003E0D RID: 15885 RVA: 0x00069F4B File Offset: 0x0006814B
		private void MasteryOnInstanceRemoved(ArchetypeInstance obj)
		{
			this.m_tocToggle.isOn = true;
		}

		// Token: 0x04003C68 RID: 15464
		[SerializeField]
		private CodexMasteryUI m_mastery;

		// Token: 0x04003C69 RID: 15465
		[SerializeField]
		private CodexAbilityUI m_ability;

		// Token: 0x04003C6A RID: 15466
		[SerializeField]
		private SolToggle m_tocToggle;
	}
}
