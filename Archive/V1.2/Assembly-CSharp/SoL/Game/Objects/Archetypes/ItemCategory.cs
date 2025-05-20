using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A84 RID: 2692
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Item Category")]
	public class ItemCategory : ScriptableObject
	{
		// Token: 0x170012FF RID: 4863
		// (get) Token: 0x06005377 RID: 21367 RVA: 0x00077B96 File Offset: 0x00075D96
		public ItemCategory.ColorFlags ColorFlag
		{
			get
			{
				return this.m_colorFlag;
			}
		}

		// Token: 0x17001300 RID: 4864
		// (get) Token: 0x06005378 RID: 21368 RVA: 0x00077B9E File Offset: 0x00075D9E
		public Color Color
		{
			get
			{
				return this.m_color;
			}
		}

		// Token: 0x17001301 RID: 4865
		// (get) Token: 0x06005379 RID: 21369 RVA: 0x00077BA6 File Offset: 0x00075DA6
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x17001302 RID: 4866
		// (get) Token: 0x0600537A RID: 21370 RVA: 0x00077BAE File Offset: 0x00075DAE
		public bool OverrideProgressionDescription
		{
			get
			{
				return this.m_overrideProgressionDescription;
			}
		}

		// Token: 0x17001303 RID: 4867
		// (get) Token: 0x0600537B RID: 21371 RVA: 0x00077BB6 File Offset: 0x00075DB6
		public string ProgressionDescription
		{
			get
			{
				return this.m_progressionDescription;
			}
		}

		// Token: 0x17001304 RID: 4868
		// (get) Token: 0x0600537C RID: 21372 RVA: 0x00077BBE File Offset: 0x00075DBE
		public ulong Postage
		{
			get
			{
				return this.m_postage;
			}
		}

		// Token: 0x17001305 RID: 4869
		// (get) Token: 0x0600537D RID: 21373 RVA: 0x00077BC6 File Offset: 0x00075DC6
		public ulong StackablePostage
		{
			get
			{
				return this.m_stackablePostage;
			}
		}

		// Token: 0x17001306 RID: 4870
		// (get) Token: 0x0600537E RID: 21374 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x04004A7A RID: 19066
		[SerializeField]
		private Color m_color = Color.white;

		// Token: 0x04004A7B RID: 19067
		[SerializeField]
		private ItemCategory.ColorFlags m_colorFlag;

		// Token: 0x04004A7C RID: 19068
		[SerializeField]
		private string m_description;

		// Token: 0x04004A7D RID: 19069
		[SerializeField]
		private bool m_overrideProgressionDescription;

		// Token: 0x04004A7E RID: 19070
		[TextArea]
		[SerializeField]
		private string m_progressionDescription;

		// Token: 0x04004A7F RID: 19071
		[SerializeField]
		private ulong m_postage;

		// Token: 0x04004A80 RID: 19072
		[SerializeField]
		private ulong m_stackablePostage;

		// Token: 0x02000A85 RID: 2693
		[Flags]
		public enum ColorFlags
		{
			// Token: 0x04004A82 RID: 19074
			None = 0,
			// Token: 0x04004A83 RID: 19075
			DisplayName = 1,
			// Token: 0x04004A84 RID: 19076
			Description = 2,
			// Token: 0x04004A85 RID: 19077
			IconBorder = 4
		}
	}
}
