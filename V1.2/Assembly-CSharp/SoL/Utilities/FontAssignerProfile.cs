using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000289 RID: 649
	[CreateAssetMenu(menuName = "SoL/Profiles/Font Set")]
	public class FontAssignerProfile : ScriptableObject
	{
		// Token: 0x04001C31 RID: 7217
		public List<FontAssignerCategory> FontList = new List<FontAssignerCategory>();
	}
}
