using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D90 RID: 3472
	public class BoundsContainsTester : MonoBehaviour
	{
		// Token: 0x06006863 RID: 26723 RVA: 0x00214250 File Offset: 0x00212450
		private void WithinBounds()
		{
			Collider component = base.gameObject.GetComponent<Collider>();
			if (component == null)
			{
				return;
			}
			Bounds bounds = component.bounds;
			bounds.size += this.m_boundsSizeBuffer;
			Debug.Log("Within bounds?  " + bounds.WithinBounds(this.m_obj.transform.position).ToString());
		}

		// Token: 0x06006864 RID: 26724 RVA: 0x002142C0 File Offset: 0x002124C0
		private void PrintBounds()
		{
			Collider component = base.gameObject.GetComponent<Collider>();
			if (component == null)
			{
				return;
			}
			Debug.Log(component.bounds.ToString());
		}

		// Token: 0x06006865 RID: 26725 RVA: 0x002142FC File Offset: 0x002124FC
		private void OnDrawGizmosSelected()
		{
			Collider component = base.gameObject.GetComponent<Collider>();
			if (component == null)
			{
				return;
			}
			Bounds bounds = component.bounds;
			bounds.size += this.m_boundsSizeBuffer;
			Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
			Vector3 size = new Vector3(bounds.size.x / base.gameObject.transform.localScale.x, bounds.size.y / base.gameObject.transform.localScale.y, bounds.size.z / base.gameObject.transform.localScale.z);
			Gizmos.DrawWireCube(bounds.center, size);
		}

		// Token: 0x04005A97 RID: 23191
		[SerializeField]
		private GameObject m_obj;

		// Token: 0x04005A98 RID: 23192
		[SerializeField]
		private Vector3 m_boundsSizeBuffer = Vector3.zero;
	}
}
