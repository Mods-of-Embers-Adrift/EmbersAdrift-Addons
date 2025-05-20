using System;
using System.Collections;
using SoL.Game.Messages;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000729 RID: 1833
	[Serializable]
	public class MessageColorSetting
	{
		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x06003705 RID: 14085 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x0400358E RID: 13710
		public MessageType MsgType;

		// Token: 0x0400358F RID: 13711
		public Color Color;
	}
}
