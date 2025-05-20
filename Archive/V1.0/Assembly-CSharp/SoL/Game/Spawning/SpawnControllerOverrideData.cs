using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006CE RID: 1742
	[Serializable]
	public class SpawnControllerOverrideData
	{
		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x060034F5 RID: 13557 RVA: 0x0006446E File Offset: 0x0006266E
		public SpawnProfile SpawnProfile
		{
			get
			{
				return this.m_spawnProfile;
			}
		}

		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x060034F6 RID: 13558 RVA: 0x00064476 File Offset: 0x00062676
		public bool IsPlaceholder
		{
			get
			{
				return this.m_isPlaceholder;
			}
		}

		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x060034F7 RID: 13559 RVA: 0x0006447E File Offset: 0x0006267E
		public bool OverrideName
		{
			get
			{
				return this.m_overrideName;
			}
		}

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x060034F8 RID: 13560 RVA: 0x00064486 File Offset: 0x00062686
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x060034F9 RID: 13561 RVA: 0x0006448E File Offset: 0x0006268E
		public bool OverrideTitle
		{
			get
			{
				return this.m_overrideTitle;
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x060034FA RID: 13562 RVA: 0x00064496 File Offset: 0x00062696
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x04003325 RID: 13093
		[SerializeField]
		private SpawnProfile m_spawnProfile;

		// Token: 0x04003326 RID: 13094
		[SerializeField]
		private bool m_isPlaceholder;

		// Token: 0x04003327 RID: 13095
		[SerializeField]
		private bool m_overrideName;

		// Token: 0x04003328 RID: 13096
		[SerializeField]
		private string m_name;

		// Token: 0x04003329 RID: 13097
		[SerializeField]
		private bool m_overrideTitle;

		// Token: 0x0400332A RID: 13098
		[SerializeField]
		private string m_title;
	}
}
