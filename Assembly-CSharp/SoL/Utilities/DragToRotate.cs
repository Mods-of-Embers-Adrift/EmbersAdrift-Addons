using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000278 RID: 632
	public class DragToRotate : MonoBehaviour
	{
		// Token: 0x060013CC RID: 5068 RVA: 0x000F7EEC File Offset: 0x000F60EC
		private void OnEnable()
		{
			if (this.m_startingRotation == null)
			{
				this.m_startingRotation = new Quaternion?(base.gameObject.transform.rotation);
				return;
			}
			if (this.m_resetOnEnable)
			{
				base.gameObject.transform.rotation = this.m_startingRotation.Value;
			}
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x000F7F48 File Offset: 0x000F6148
		private void LateUpdate()
		{
			if (this.m_mouseButtonHeld != null && !Input.GetMouseButton(this.m_mouseButtonHeld.Value))
			{
				this.m_mouseButtonHeld = null;
			}
			if (UIManager.EventSystem != null && !UIManager.EventSystem.IsPointerOverGameObject())
			{
				if (Input.GetMouseButtonDown(0))
				{
					this.m_mouseButtonHeld = new int?(0);
				}
				else if (Input.GetMouseButtonDown(1))
				{
					this.m_mouseButtonHeld = new int?(1);
				}
			}
			if (this.m_mouseButtonHeld != null)
			{
				float num = Input.mousePosition.x - this.m_previousMousePos;
				num *= -1f;
				base.gameObject.transform.Rotate(Vector3.up * (Time.deltaTime * this.m_speed * num), Space.Self);
			}
			this.m_previousMousePos = Input.mousePosition.x;
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x0004FE2D File Offset: 0x0004E02D
		public void ResetRotation()
		{
			base.gameObject.transform.rotation = ((this.m_startingRotation != null) ? this.m_startingRotation.Value : Quaternion.identity);
		}

		// Token: 0x04001BFF RID: 7167
		[SerializeField]
		private bool m_resetOnEnable;

		// Token: 0x04001C00 RID: 7168
		[SerializeField]
		private float m_speed = 1f;

		// Token: 0x04001C01 RID: 7169
		private Quaternion? m_startingRotation;

		// Token: 0x04001C02 RID: 7170
		private float m_previousMousePos;

		// Token: 0x04001C03 RID: 7171
		private int? m_mouseButtonHeld;
	}
}
