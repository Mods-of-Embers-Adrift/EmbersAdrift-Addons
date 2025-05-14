using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006A0 RID: 1696
	[CreateAssetMenu(menuName = "SoL/Probability/String (Scriptable)")]
	public class StringScriptableProbabilityCollection : ScriptableObject
	{
		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x060033D9 RID: 13273 RVA: 0x00063987 File Offset: 0x00061B87
		public StringProbabilityEntry[] Entries
		{
			get
			{
				StringProbabilityCollection collection = this.m_collection;
				if (collection == null)
				{
					return null;
				}
				return collection.Entries;
			}
		}

		// Token: 0x060033DA RID: 13274 RVA: 0x0006399A File Offset: 0x00061B9A
		public StringProbabilityEntry GetEntry()
		{
			StringProbabilityCollection collection = this.m_collection;
			if (collection == null)
			{
				return null;
			}
			return collection.GetEntry(null, false);
		}

		// Token: 0x060033DB RID: 13275 RVA: 0x000639AF File Offset: 0x00061BAF
		public StringProbabilityEntry GetEntry(System.Random seededRandom)
		{
			StringProbabilityCollection collection = this.m_collection;
			if (collection == null)
			{
				return null;
			}
			return collection.GetEntry(seededRandom, false);
		}

		// Token: 0x060033DC RID: 13276 RVA: 0x000639C4 File Offset: 0x00061BC4
		private void OnValidate()
		{
			StringProbabilityCollection collection = this.m_collection;
			if (collection == null)
			{
				return;
			}
			collection.Normalize();
		}

		// Token: 0x040031AE RID: 12718
		[SerializeField]
		private StringProbabilityCollection m_collection;
	}
}
