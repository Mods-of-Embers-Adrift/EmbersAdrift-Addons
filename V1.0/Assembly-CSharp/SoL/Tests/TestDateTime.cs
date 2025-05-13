using System;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DC0 RID: 3520
	public class TestDateTime : MonoBehaviour
	{
		// Token: 0x0600692B RID: 26923 RVA: 0x00216CC8 File Offset: 0x00214EC8
		private void Start()
		{
			DateTime now = DateTime.Now;
			Debug.Log(string.Format("NOW: {0}", now));
			DateTime dateTime = now.AddSeconds(30.0);
			Debug.Log(string.Format("NOW: {0}  LATER: {1}", now, dateTime));
		}
	}
}
