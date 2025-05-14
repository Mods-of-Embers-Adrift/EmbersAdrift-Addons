using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs.Senses
{
	// Token: 0x02000830 RID: 2096
	[Serializable]
	public class SensorDebugSettings
	{
		// Token: 0x17000DFC RID: 3580
		// (get) Token: 0x06003CBF RID: 15551 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x04003B97 RID: 15255
		public bool Draw;

		// Token: 0x04003B98 RID: 15256
		public SensorType SensorType;

		// Token: 0x04003B99 RID: 15257
		public Color Color;
	}
}
