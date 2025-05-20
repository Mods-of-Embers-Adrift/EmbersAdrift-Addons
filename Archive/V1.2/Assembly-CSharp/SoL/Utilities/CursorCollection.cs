using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000260 RID: 608
	[CreateAssetMenu(menuName = "SoL/Collections/Cursor Collection")]
	public class CursorCollection : ScriptableObject, ISerializationCallbackReceiver
	{
		// Token: 0x0600136D RID: 4973 RVA: 0x000F6D7C File Offset: 0x000F4F7C
		public Texture2D GetCursorImage(CursorType type)
		{
			Texture2D mainCursor = this.m_mainCursor;
			this.m_cursorDict.TryGetValue(type, out mainCursor);
			return mainCursor;
		}

		// Token: 0x0600136E RID: 4974 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x000F6DA0 File Offset: 0x000F4FA0
		public void OnAfterDeserialize()
		{
			this.m_cursorDict = new Dictionary<CursorType, Texture2D>(default(CursorImageComparer));
			this.m_cursorDict.Add(CursorType.MainCursor, this.m_mainCursor);
			for (int i = 0; i < this.m_cursors.Length; i++)
			{
				if (this.m_cursors[i].Type != CursorType.None && this.m_cursors[i].Image != null)
				{
					this.m_cursorDict.Add(this.m_cursors[i].Type, this.m_cursors[i].Image);
				}
			}
		}

		// Token: 0x04001BA0 RID: 7072
		[SerializeField]
		private Texture2D m_mainCursor;

		// Token: 0x04001BA1 RID: 7073
		[SerializeField]
		private CursorCollection.CursorData[] m_cursors;

		// Token: 0x04001BA2 RID: 7074
		[NonSerialized]
		private Dictionary<CursorType, Texture2D> m_cursorDict;

		// Token: 0x02000261 RID: 609
		[Serializable]
		private class CursorData
		{
			// Token: 0x04001BA3 RID: 7075
			public CursorType Type;

			// Token: 0x04001BA4 RID: 7076
			public Texture2D Image;
		}
	}
}
