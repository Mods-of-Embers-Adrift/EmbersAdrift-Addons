using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class Hovl_DemoLasers : MonoBehaviour
{
	// Token: 0x06000143 RID: 323 RVA: 0x00098518 File Offset: 0x00096718
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

	// Token: 0x06000144 RID: 324 RVA: 0x00098570 File Offset: 0x00096770
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			UnityEngine.Object.Destroy(this.Instance);
			this.Instance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.Prefab], this.FirePoint.transform.position, this.FirePoint.transform.rotation);
			this.Instance.transform.parent = base.transform;
			this.LaserScript = this.Instance.GetComponent<Hovl_Laser>();
			this.LaserScript2 = this.Instance.GetComponent<Hovl_Laser2>();
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (this.LaserScript)
			{
				this.LaserScript.DisablePrepare();
			}
			if (this.LaserScript2)
			{
				this.LaserScript2.DisablePrepare();
			}
			UnityEngine.Object.Destroy(this.Instance, 1f);
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
		if (!(this.Cam != null))
		{
			Debug.Log("No camera");
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		this.RayMouse = this.Cam.ScreenPointToRay(mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(this.RayMouse.origin, this.RayMouse.direction, out raycastHit, this.MaxLength))
		{
			this.RotateToMouseDirection(base.gameObject, raycastHit.point);
			return;
		}
		Vector3 point = this.RayMouse.GetPoint(this.MaxLength);
		this.RotateToMouseDirection(base.gameObject, point);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00098758 File Offset: 0x00096958
	private void OnGUI()
	{
		GUI.Label(new Rect(10f * this.windowDpi, 5f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use the keyboard buttons A/<- and D/-> to change lazers!");
		GUI.Label(new Rect(10f * this.windowDpi, 20f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use left mouse button for shooting!");
	}

	// Token: 0x06000146 RID: 326 RVA: 0x000987E4 File Offset: 0x000969E4
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

	// Token: 0x06000147 RID: 327 RVA: 0x00098834 File Offset: 0x00096A34
	private void RotateToMouseDirection(GameObject obj, Vector3 destination)
	{
		this.direction = destination - obj.transform.position;
		this.rotation = Quaternion.LookRotation(this.direction);
		obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, this.rotation, 1f);
	}

	// Token: 0x040002FB RID: 763
	public GameObject FirePoint;

	// Token: 0x040002FC RID: 764
	public Camera Cam;

	// Token: 0x040002FD RID: 765
	public float MaxLength;

	// Token: 0x040002FE RID: 766
	public GameObject[] Prefabs;

	// Token: 0x040002FF RID: 767
	private Ray RayMouse;

	// Token: 0x04000300 RID: 768
	private Vector3 direction;

	// Token: 0x04000301 RID: 769
	private Quaternion rotation;

	// Token: 0x04000302 RID: 770
	[Header("GUI")]
	private float windowDpi;

	// Token: 0x04000303 RID: 771
	private int Prefab;

	// Token: 0x04000304 RID: 772
	private GameObject Instance;

	// Token: 0x04000305 RID: 773
	private Hovl_Laser LaserScript;

	// Token: 0x04000306 RID: 774
	private Hovl_Laser2 LaserScript2;

	// Token: 0x04000307 RID: 775
	private float buttonSaver;
}
