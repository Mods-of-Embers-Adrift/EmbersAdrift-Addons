using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CAA RID: 3242
	[CreateAssetMenu(menuName = "SoL/Profiles/Discovery (Lore)")]
	public class LoreProfile : DiscoveryProfile
	{
		// Token: 0x17001775 RID: 6005
		// (get) Token: 0x0600623E RID: 25150 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_showCategory
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001776 RID: 6006
		// (get) Token: 0x0600623F RID: 25151 RVA: 0x0006ADA7 File Offset: 0x00068FA7
		protected override DiscoveryCategory DiscoveryCategory
		{
			get
			{
				return DiscoveryCategory.Lore;
			}
		}

		// Token: 0x17001777 RID: 6007
		// (get) Token: 0x06006240 RID: 25152 RVA: 0x000822CF File Offset: 0x000804CF
		public TextAsset InkFile
		{
			get
			{
				return this.m_inkFile;
			}
		}

		// Token: 0x17001778 RID: 6008
		// (get) Token: 0x06006241 RID: 25153 RVA: 0x00203DF0 File Offset: 0x00201FF0
		public override string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_title) && this.m_inkFile != null)
				{
					Story story = new Story(this.m_inkFile.text);
					Container container = story.KnotContainerWithName("DOCUMENT");
					bool? flag;
					if (container == null)
					{
						flag = null;
					}
					else
					{
						Dictionary<string, Ink.Runtime.Object> namedOnlyContent = container.namedOnlyContent;
						flag = ((namedOnlyContent != null) ? new bool?(namedOnlyContent.ContainsKey("TITLE")) : null);
					}
					bool? flag2 = flag;
					if (flag2.GetValueOrDefault())
					{
						story.ChoosePathString("DOCUMENT.TITLE", true, Array.Empty<object>());
						if (!story.canContinue)
						{
							Debug.LogError("Invalid document ink file!");
							this.m_title = "a bit of lore";
						}
						else
						{
							this.m_title = story.Continue().Trim();
						}
					}
					else
					{
						this.m_title = "a bit of lore";
					}
				}
				return this.m_title;
			}
		}

		// Token: 0x06006242 RID: 25154 RVA: 0x00049FFA File Offset: 0x000481FA
		private IEnumerable GetTextAssets()
		{
			return null;
		}

		// Token: 0x040055C8 RID: 21960
		public const string kKnot = "DOCUMENT";

		// Token: 0x040055C9 RID: 21961
		public const string kTitleStitch = "TITLE";

		// Token: 0x040055CA RID: 21962
		public const string kBodyStitch = "BODY";

		// Token: 0x040055CB RID: 21963
		public const string kTitleKnotStitch = "DOCUMENT.TITLE";

		// Token: 0x040055CC RID: 21964
		public const string kBodyKnotStitch = "DOCUMENT.BODY";

		// Token: 0x040055CD RID: 21965
		[SerializeField]
		private TextAsset m_inkFile;

		// Token: 0x040055CE RID: 21966
		private string m_title;
	}
}
