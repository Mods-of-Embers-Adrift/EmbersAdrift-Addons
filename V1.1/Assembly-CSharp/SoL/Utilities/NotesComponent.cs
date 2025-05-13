using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002A6 RID: 678
	public class NotesComponent : MonoBehaviour
	{
		// Token: 0x04001C96 RID: 7318
		[TextArea(4, 20)]
		[SerializeField]
		private string m_notes;
	}
}
