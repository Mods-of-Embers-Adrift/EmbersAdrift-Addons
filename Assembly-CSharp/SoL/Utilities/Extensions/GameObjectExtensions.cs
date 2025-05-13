using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000331 RID: 817
	public static class GameObjectExtensions
	{
		// Token: 0x06001660 RID: 5728 RVA: 0x00100640 File Offset: 0x000FE840
		public static T GetOrAddComponent<T>(this GameObject obj) where T : MonoBehaviour
		{
			T t = obj.GetComponent<T>();
			if (t == null)
			{
				t = obj.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x0010066C File Offset: 0x000FE86C
		public static float DistanceTo(this GameObject from, GameObject to)
		{
			Vector3 vector = to.transform.position - from.transform.position;
			if (vector.y <= GlobalSettings.Values.General.VerticalDistanceToIgnoreForDistanceChecks)
			{
				vector.y = 0f;
			}
			return vector.magnitude;
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x001006C0 File Offset: 0x000FE8C0
		public static float AngleTo(this GameObject from, GameObject to, bool ignoreY = true)
		{
			Vector3 to2 = to.transform.position - from.transform.position;
			Vector3 forward = from.transform.forward;
			if (ignoreY)
			{
				to2.y = 0f;
				forward.y = 0f;
			}
			return Vector3.Angle(forward, to2);
		}
	}
}
