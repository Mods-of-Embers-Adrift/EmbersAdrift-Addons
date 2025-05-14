using System;
using SoL.Game.Interactives;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000874 RID: 2164
	public class CursorForwarder : MonoBehaviour, ICursor, IInteractiveBase
	{
		// Token: 0x06003EDE RID: 16094 RVA: 0x0006A86A File Offset: 0x00068A6A
		private void Awake()
		{
			this.m_cursor = this.m_target.GetComponent<ICursor>();
		}

		// Token: 0x17000E89 RID: 3721
		// (get) Token: 0x06003EDF RID: 16095 RVA: 0x0006A87D File Offset: 0x00068A7D
		CursorType ICursor.Type
		{
			get
			{
				if (this.m_cursor != null)
				{
					return this.m_cursor.Type;
				}
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17000E8A RID: 3722
		// (get) Token: 0x06003EE0 RID: 16096 RVA: 0x0006A894 File Offset: 0x00068A94
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				if (this.m_cursor != null)
				{
					return this.m_cursor.Settings;
				}
				return null;
			}
		}

		// Token: 0x06003EE2 RID: 16098 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003CDC RID: 15580
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04003CDD RID: 15581
		private ICursor m_cursor;
	}
}
