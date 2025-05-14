using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.States
{
	// Token: 0x0200065D RID: 1629
	public class DelayedStateSet : MonoBehaviour
	{
		// Token: 0x04003119 RID: 12569
		[FormerlySerializedAs("m_resetDelay")]
		[SerializeField]
		private int m_delay;

		// Token: 0x0400311A RID: 12570
		[FormerlySerializedAs("m_toReset")]
		[SerializeField]
		private StateSetting[] m_toSet;
	}
}
