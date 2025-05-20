using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class CameraHolder : MonoBehaviour
{
	// Token: 0x0600015D RID: 349 RVA: 0x000994EC File Offset: 0x000976EC
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
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		this.Counter(0);
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00099568 File Offset: 0x00097768
	private void OnGUI()
	{
		if (GUI.Button(new Rect(5f * this.windowDpi, 5f * this.windowDpi, 110f * this.windowDpi, 35f * this.windowDpi), "Previous effect"))
		{
			this.Counter(-1);
		}
		if (GUI.Button(new Rect(120f * this.windowDpi, 5f * this.windowDpi, 110f * this.windowDpi, 35f * this.windowDpi), "Play again"))
		{
			this.Counter(0);
		}
		if (GUI.Button(new Rect(235f * this.windowDpi, 5f * this.windowDpi, 110f * this.windowDpi, 35f * this.windowDpi), "Next effect"))
		{
			this.Counter(1);
		}
		this.StartColor = this.HueColor;
		this.HueColor = GUI.HorizontalSlider(new Rect(5f * this.windowDpi, 45f * this.windowDpi, 340f * this.windowDpi, 35f * this.windowDpi), this.HueColor, 0f, 1f);
		GUI.DrawTexture(new Rect(5f * this.windowDpi, 65f * this.windowDpi, 340f * this.windowDpi, 15f * this.windowDpi), this.HueTexture, ScaleMode.StretchToFill, false, 0f);
		if (this.HueColor != this.StartColor)
		{
			int num = 0;
			ParticleSystem[] array = this.particleSystems;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				Color color = Color.HSVToRGB(this.HueColor + this.H * 0f, this.svList[num].S, this.svList[num].V);
				main.startColor = new Color(color.r, color.g, color.b, this.svList[num].A);
				num++;
			}
		}
	}

	// Token: 0x0600015F RID: 351 RVA: 0x000997A0 File Offset: 0x000979A0
	private void Counter(int count)
	{
		this.Prefab += count;
		if (this.Prefab > this.Prefabs.Length - 1)
		{
			this.Prefab = 0;
		}
		else if (this.Prefab < 0)
		{
			this.Prefab = this.Prefabs.Length - 1;
		}
		if (this.Instance != null)
		{
			UnityEngine.Object.Destroy(this.Instance);
		}
		this.Instance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.Prefab]);
		this.particleSystems = this.Instance.GetComponentsInChildren<ParticleSystem>();
		this.svList.Clear();
		ParticleSystem[] array = this.particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			Color color = array[i].main.startColor.color;
			CameraHolder.SVA item = default(CameraHolder.SVA);
			Color.RGBToHSV(color, out this.H, out item.S, out item.V);
			item.A = color.a;
			this.svList.Add(item);
		}
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000998A8 File Offset: 0x00097AA8
	private void LateUpdate()
	{
		if (this.currDistance < 2f)
		{
			this.currDistance = 2f;
		}
		this.currDistance -= Input.GetAxis("Mouse ScrollWheel") * 2f;
		if (this.Holder && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
		{
			Vector3 mousePosition = Input.mousePosition;
			if (Screen.dpi < 1f)
			{
			}
			float num;
			if (Screen.dpi < 200f)
			{
				num = 1f;
			}
			else
			{
				num = Screen.dpi / 200f;
			}
			if (mousePosition.x < 380f * num && (float)Screen.height - mousePosition.y < 250f * num)
			{
				return;
			}
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			this.x += (float)((double)(Input.GetAxis("Mouse X") * this.xRotate) * 0.02);
			this.y -= (float)((double)(Input.GetAxis("Mouse Y") * this.yRotate) * 0.02);
			this.y = CameraHolder.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 position = rotation * new Vector3(0f, 0f, -this.currDistance) + this.Holder.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
		}
		else
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		if (this.prevDistance != this.currDistance)
		{
			this.prevDistance = this.currDistance;
			Quaternion rotation2 = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 position2 = rotation2 * new Vector3(0f, 0f, -this.currDistance) + this.Holder.position;
			base.transform.rotation = rotation2;
			base.transform.position = position2;
		}
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000451D6 File Offset: 0x000433D6
	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x0400033A RID: 826
	public Transform Holder;

	// Token: 0x0400033B RID: 827
	public float currDistance = 5f;

	// Token: 0x0400033C RID: 828
	public float xRotate = 250f;

	// Token: 0x0400033D RID: 829
	public float yRotate = 120f;

	// Token: 0x0400033E RID: 830
	public float yMinLimit = -20f;

	// Token: 0x0400033F RID: 831
	public float yMaxLimit = 80f;

	// Token: 0x04000340 RID: 832
	public float prevDistance;

	// Token: 0x04000341 RID: 833
	private float x;

	// Token: 0x04000342 RID: 834
	private float y;

	// Token: 0x04000343 RID: 835
	[Header("GUI")]
	private float windowDpi;

	// Token: 0x04000344 RID: 836
	public GameObject[] Prefabs;

	// Token: 0x04000345 RID: 837
	private int Prefab;

	// Token: 0x04000346 RID: 838
	private GameObject Instance;

	// Token: 0x04000347 RID: 839
	private float StartColor;

	// Token: 0x04000348 RID: 840
	private float HueColor;

	// Token: 0x04000349 RID: 841
	public Texture HueTexture;

	// Token: 0x0400034A RID: 842
	private ParticleSystem[] particleSystems = new ParticleSystem[0];

	// Token: 0x0400034B RID: 843
	private List<CameraHolder.SVA> svList = new List<CameraHolder.SVA>();

	// Token: 0x0400034C RID: 844
	private float H;

	// Token: 0x02000047 RID: 71
	public struct SVA
	{
		// Token: 0x0400034D RID: 845
		public float S;

		// Token: 0x0400034E RID: 846
		public float V;

		// Token: 0x0400034F RID: 847
		public float A;
	}
}
