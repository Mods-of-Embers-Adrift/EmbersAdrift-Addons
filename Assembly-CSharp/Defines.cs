using System;
using UnityEngine;

// Token: 0x02000007 RID: 7
[CreateAssetMenu(menuName = "SoL/Defines")]
public class Defines : ScriptableObject
{
	// Token: 0x0600000F RID: 15 RVA: 0x0004475B File Offset: 0x0004295B
	private void ReadDefines()
	{
	}

	// Token: 0x06000010 RID: 16 RVA: 0x0004475B File Offset: 0x0004295B
	private void SortDefines()
	{
	}

	// Token: 0x06000011 RID: 17 RVA: 0x0004475B File Offset: 0x0004295B
	private void WriteDefines()
	{
	}

	// Token: 0x04000008 RID: 8
	[SerializeField]
	private Defines.DefineOptions[] m_defineOptions;

	// Token: 0x04000009 RID: 9
	private const string kButtonGroup = "Actions";

	// Token: 0x02000008 RID: 8
	[Serializable]
	private class DefineOptions
	{
		// Token: 0x0400000A RID: 10
		private const string kGroupName = "grp";

		// Token: 0x0400000B RID: 11
		public bool Enabled;

		// Token: 0x0400000C RID: 12
		public string Define;
	}
}
