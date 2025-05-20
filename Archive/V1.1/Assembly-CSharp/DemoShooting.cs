using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class DemoShooting : MonoBehaviour
{
	// Token: 0x06000133 RID: 307 RVA: 0x00097DB0 File Offset: 0x00095FB0
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

	// Token: 0x06000134 RID: 308 RVA: 0x00097E08 File Offset: 0x00096008
	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			this.camAnim.Play(this.camAnim.clip.name);
			UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.Prefab], this.FirePoint.transform.position, this.FirePoint.transform.rotation);
		}
		if (Input.GetMouseButton(1) && this.fireCountdown <= 0f)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.Prefab], this.FirePoint.transform.position, this.FirePoint.transform.rotation);
			this.fireCountdown = 0f;
			this.fireCountdown += this.hSliderValue;
		}
		this.fireCountdown -= Time.deltaTime;
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
		if (this.Cam != null)
		{
			Vector3 mousePosition = Input.mousePosition;
			this.RayMouse = this.Cam.ScreenPointToRay(mousePosition);
			RaycastHit raycastHit;
			if (Physics.Raycast(this.RayMouse.origin, this.RayMouse.direction, out raycastHit, this.MaxLength))
			{
				this.RotateToMouseDirection(base.gameObject, raycastHit.point);
				return;
			}
		}
		else
		{
			Debug.Log("No camera");
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00097FD8 File Offset: 0x000961D8
	private void OnGUI()
	{
		GUI.Label(new Rect(10f * this.windowDpi, 5f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use left mouse button to single shoot!");
		GUI.Label(new Rect(10f * this.windowDpi, 25f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use and hold the right mouse button for quick shooting!");
		GUI.Label(new Rect(10f * this.windowDpi, 45f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Fire rate:");
		this.hSliderValue = GUI.HorizontalSlider(new Rect(70f * this.windowDpi, 50f * this.windowDpi, 100f * this.windowDpi, 20f * this.windowDpi), this.hSliderValue, 0f, 1f);
		GUI.Label(new Rect(10f * this.windowDpi, 65f * this.windowDpi, 400f * this.windowDpi, 20f * this.windowDpi), "Use the keyboard buttons A/<- and D/-> to change projectiles!");
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00098134 File Offset: 0x00096334
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

	// Token: 0x06000137 RID: 311 RVA: 0x00098184 File Offset: 0x00096384
	private void RotateToMouseDirection(GameObject obj, Vector3 destination)
	{
		this.direction = destination - obj.transform.position;
		this.rotation = Quaternion.LookRotation(this.direction);
		obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, this.rotation, 1f);
	}

	// Token: 0x040002DD RID: 733
	public GameObject FirePoint;

	// Token: 0x040002DE RID: 734
	public Camera Cam;

	// Token: 0x040002DF RID: 735
	public float MaxLength;

	// Token: 0x040002E0 RID: 736
	public GameObject[] Prefabs;

	// Token: 0x040002E1 RID: 737
	private Ray RayMouse;

	// Token: 0x040002E2 RID: 738
	private Vector3 direction;

	// Token: 0x040002E3 RID: 739
	private Quaternion rotation;

	// Token: 0x040002E4 RID: 740
	[Header("GUI")]
	private float windowDpi;

	// Token: 0x040002E5 RID: 741
	private int Prefab;

	// Token: 0x040002E6 RID: 742
	private GameObject Instance;

	// Token: 0x040002E7 RID: 743
	private float hSliderValue = 0.1f;

	// Token: 0x040002E8 RID: 744
	private float fireCountdown;

	// Token: 0x040002E9 RID: 745
	private float buttonSaver;

	// Token: 0x040002EA RID: 746
	public Animation camAnim;
}
