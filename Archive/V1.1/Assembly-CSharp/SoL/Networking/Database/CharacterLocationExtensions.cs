using System;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x0200043A RID: 1082
	public static class CharacterLocationExtensions
	{
		// Token: 0x06001EE1 RID: 7905 RVA: 0x00056DF7 File Offset: 0x00054FF7
		public static Vector3 GetPosition(this CharacterLocation location)
		{
			return new Vector3(location.x, location.y, location.z);
		}

		// Token: 0x06001EE2 RID: 7906 RVA: 0x00056E10 File Offset: 0x00055010
		public static Quaternion GetRotation(this CharacterLocation location)
		{
			return Quaternion.Euler(new Vector3(0f, location.h, 0f));
		}
	}
}
