using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class RaycastInstance : MonoBehaviour
{
	// Token: 0x06000139 RID: 313 RVA: 0x000981E0 File Offset: 0x000963E0
	private void Start()
	{
		if (Screen.dpi < 1f)
		{
			this.windowDpi = 1f;
		}
		if (Screen.dpi < 200f)
		{
			this.windowDpi = 1f;
		}
		else
		{
			this.windowDpi = Screen.dpi / 200f;
		}
		this.Counter(0);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00098238 File Offset: 0x00096438
	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (this.Cam != null)
			{
				Vector3 mousePosition = Input.mousePosition;
				this.RayMouse = this.Cam.ScreenPointToRay(mousePosition);
				RaycastHit raycastHit;
				if (Physics.Raycast(this.RayMouse.origin, this.RayMouse.direction, out raycastHit, 40f))
				{
					this.Instance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.Prefab]);
					this.Instance.transform.position = raycastHit.point + raycastHit.normal * 0.01f;
					UnityEngine.Object.Destroy(this.Instance, 1.5f);
				}
			}
			else
			{
				Debug.Log("No camera");
			}
		}
		if ((Input.GetKey(KeyCode.A) || Input.GetAxis("Horizontal") < 0f) && this.buttonSaver >= 0.4f)
		{
			this.buttonSaver = 0f;
			this.Counter(-1);
		}
		if ((Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 0f) && this.buttonSaver >= 0.4f)
		{
			this.buttonSaver = 0f;
			this.Counter(1);
		}
		this.buttonSaver += Time.deltaTime;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00098388 File Offset: 0x00096588
	private void OnGUI()
	{
		GUI.Label(new Rect(10f * this.windowDpi, 5f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use the keyboard buttons A/<- and D/-> to change prefabs!");
		GUI.Label(new Rect(10f * this.windowDpi, 20f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use left mouse button for instancing!");
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00098414 File Offset: 0x00096614
	private void Counter(int count)
	{
		this.Prefab += count;
		if (this.Prefab > this.Prefabs.Length - 1)
		{
			this.Prefab = 0;
			return;
		}
		if (this.Prefab < 0)
		{
			this.Prefab = this.Prefabs.Length - 1;
		}
	}

	// Token: 0x040002EB RID: 747
	public Camera Cam;

	// Token: 0x040002EC RID: 748
	public GameObject[] Prefabs;

	// Token: 0x040002ED RID: 749
	private int Prefab;

	// Token: 0x040002EE RID: 750
	private Ray RayMouse;

	// Token: 0x040002EF RID: 751
	private GameObject Instance;

	// Token: 0x040002F0 RID: 752
	private float windowDpi;

	// Token: 0x040002F1 RID: 753
	private float buttonSaver;
}
