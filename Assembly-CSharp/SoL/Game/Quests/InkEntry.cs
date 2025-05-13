using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200078E RID: 1934
	[Serializable]
	public class InkEntry
	{
		// Token: 0x17000D0C RID: 3340
		// (get) Token: 0x0600390D RID: 14605 RVA: 0x00066A35 File Offset: 0x00064C35
		private bool m_showQuestMembers
		{
			get
			{
				return this.m_quest != null;
			}
		}

		// Token: 0x17000D0D RID: 3341
		// (get) Token: 0x0600390E RID: 14606 RVA: 0x00066A43 File Offset: 0x00064C43
		// (set) Token: 0x0600390F RID: 14607 RVA: 0x00066A4B File Offset: 0x00064C4B
		public Quest Quest
		{
			get
			{
				return this.m_quest;
			}
			set
			{
				this.m_quest = value;
			}
		}

		// Token: 0x06003910 RID: 14608 RVA: 0x00049FFA File Offset: 0x000481FA
		private IEnumerable GetNpcs()
		{
			return null;
		}

		// Token: 0x06003911 RID: 14609 RVA: 0x00049FFA File Offset: 0x000481FA
		private IEnumerable GetTextAssets()
		{
			return null;
		}

		// Token: 0x06003912 RID: 14610 RVA: 0x001719B4 File Offset: 0x0016FBB4
		private void RefreshKnots()
		{
			if (this.InkStory == null)
			{
				this.m_availableKnots = null;
			}
			try
			{
				Story story = new Story(this.InkStory.text);
				this.m_availableKnots = new List<string>();
				foreach (string text in story.mainContentContainer.namedOnlyContent.Keys)
				{
					Container container = story.KnotContainerWithName(text);
					if (container != null)
					{
						Dictionary<string, Ink.Runtime.Object> namedOnlyContent = container.namedOnlyContent;
						int? num = (namedOnlyContent != null) ? new int?(namedOnlyContent.Count) : null;
						int num2 = 0;
						if (num.GetValueOrDefault() > num2 & num != null)
						{
							using (Dictionary<string, Ink.Runtime.Object>.KeyCollection.Enumerator enumerator2 = container.namedOnlyContent.Keys.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									string str = enumerator2.Current;
									this.m_availableKnots.Add(text + "." + str);
								}
								continue;
							}
						}
					}
					this.m_availableKnots.Add(text);
				}
			}
			catch (Exception arg)
			{
				Debug.LogWarning(string.Format("Invalid story file! {0}", arg));
				this.InkStory = null;
			}
		}

		// Token: 0x040037E1 RID: 14305
		public DialogueSource Source;

		// Token: 0x040037E2 RID: 14306
		public TextAsset InkStory;

		// Token: 0x040037E3 RID: 14307
		public string TargetPath;

		// Token: 0x040037E4 RID: 14308
		public bool OverrideNonQuestDialogue;

		// Token: 0x040037E5 RID: 14309
		private Quest m_quest;

		// Token: 0x040037E6 RID: 14310
		private List<string> m_availableKnots;
	}
}
