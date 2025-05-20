using System;
using Cinemachine;
using SoL.Game;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.GameCamera
{
	// Token: 0x02000DE4 RID: 3556
	[RequireComponent(typeof(CinemachineFreeLook))]
	internal class CinemachineFreeLookZoom : MonoBehaviour
	{
		// Token: 0x17001942 RID: 6466
		// (get) Token: 0x06006A02 RID: 27138 RVA: 0x000870B9 File Offset: 0x000852B9
		// (set) Token: 0x06006A03 RID: 27139 RVA: 0x000870C1 File Offset: 0x000852C1
		private bool AllowAxisZoom
		{
			get
			{
				return this.m_allowAxisZoom;
			}
			set
			{
				if (this.m_allowAxisZoom == value)
				{
					return;
				}
				this.m_allowAxisZoom = value;
				this.m_zAxis.m_InputAxisName = (this.m_allowAxisZoom ? this.m_zoomAxisName : null);
			}
		}

		// Token: 0x06006A04 RID: 27140 RVA: 0x000870F0 File Offset: 0x000852F0
		private void OnValidate()
		{
			this.m_minScale = Mathf.Max(0.01f, this.m_minScale);
			this.m_maxScale = Mathf.Max(this.m_minScale, this.m_maxScale);
		}

		// Token: 0x06006A05 RID: 27141 RVA: 0x002197FC File Offset: 0x002179FC
		private void Awake()
		{
			this.m_freeLook = base.GetComponentInChildren<CinemachineFreeLook>();
			if (this.m_freeLook != null)
			{
				this.m_originalOrbits = new CinemachineFreeLook.Orbit[this.m_freeLook.m_Orbits.Length];
				for (int i = 0; i < this.m_originalOrbits.Length; i++)
				{
					this.m_originalOrbits[i].m_Height = this.m_freeLook.m_Orbits[i].m_Height;
					this.m_originalOrbits[i].m_Radius = this.m_freeLook.m_Orbits[i].m_Radius;
				}
			}
		}

		// Token: 0x06006A06 RID: 27142 RVA: 0x0021989C File Offset: 0x00217A9C
		private void Start()
		{
			float num = 0.5f;
			if (this.m_saveLoadZoom)
			{
				if (string.IsNullOrEmpty(this.m_saveLoadKey))
				{
					this.m_saveLoadZoom = false;
				}
				else
				{
					num = PlayerPrefs.GetFloat(this.m_saveLoadKey, 0.5f);
				}
			}
			this.m_zoomAxisName = this.m_zAxis.m_InputAxisName;
			this.m_zAxis.m_InputAxisName = null;
			this.m_zAxis.Value = num;
			this.UpdateOrbits(false);
			this.m_lastFrameZoom = num;
		}

		// Token: 0x06006A07 RID: 27143 RVA: 0x00219918 File Offset: 0x00217B18
		private void Update()
		{
			this.AllowAxisZoom = (Application.isFocused && (GameManager.IsOffline || !UIManager.EventSystem.IsPointerOverGameObject()));
			bool flag = LocalPlayer.Animancer && LocalPlayer.Animancer.Stance == Stance.Swim;
			RaycastHit raycastHit;
			if (!flag && Physics.Raycast(this.m_freeLook.gameObject.transform.position + Vector3.up * this.m_waterRaycastOffset, Vector3.down, out raycastHit, 20f, this.m_waterCheckLayerMask, QueryTriggerInteraction.Ignore))
			{
				flag = (raycastHit.collider && raycastHit.collider.gameObject && raycastHit.collider.gameObject.CompareTag("Water"));
			}
			this.m_zAxis.Update(Time.deltaTime);
			bool flag2 = false;
			if (this.AllowAxisZoom)
			{
				if (CinemachineFreeLookZoom.ZoomIn())
				{
					this.m_zAxis.Value = Mathf.Clamp01(this.m_zAxis.Value - Time.deltaTime * 2f);
					flag2 = true;
				}
				else if (CinemachineFreeLookZoom.ZoomOut())
				{
					this.m_zAxis.Value = Mathf.Clamp01(this.m_zAxis.Value + Time.deltaTime * 2f);
				}
				if (this.m_allowZoomTransition && Options.GameOptions.EnableZoomToFirstPerson.Value && this.m_manager && this.m_zAxis.Value <= 0f && (this.m_zAxis.m_InputAxisValue != 0f || flag2))
				{
					if (Time.time - this.m_lastZoomTransitionTime > InteractionManager.kDoubleClickThreshold)
					{
						this.m_zoomTransitionFrameCount = 0;
					}
					this.m_zoomTransitionFrameCount++;
					this.m_lastZoomTransitionTime = Time.time;
					if (this.m_zoomTransitionFrameCount > this.m_manager.ZoomTransitionFrameCount)
					{
						this.m_manager.ZoomToFirstPerson();
					}
				}
			}
			this.UpdateOrbits(flag);
			if (this.m_saveLoadZoom && !Mathf.Approximately(this.m_zAxis.Value, this.m_lastFrameZoom))
			{
				PlayerPrefs.SetFloat(this.m_saveLoadKey, this.m_zAxis.Value);
			}
			this.m_lastFrameZoom = this.m_zAxis.Value;
		}

		// Token: 0x06006A08 RID: 27144 RVA: 0x00219B64 File Offset: 0x00217D64
		private void UpdateOrbits(bool isOverWater)
		{
			if (this.m_originalOrbits == null || !this.m_freeLook)
			{
				return;
			}
			float num = Mathf.Lerp(this.m_minScale, this.m_maxScale, this.m_zAxis.Value);
			for (int i = 0; i < this.m_originalOrbits.Length; i++)
			{
				float num2 = this.m_originalOrbits[i].m_Height * num;
				float num3 = this.m_originalOrbits[i].m_Radius * num;
				if (isOverWater)
				{
					num2 = Mathf.Max(num2, this.m_minRigHeightOverWater);
					num3 = Mathf.Max(num3, this.m_minRigRadiusOverWater);
				}
				this.m_freeLook.m_Orbits[i].m_Height = num2;
				this.m_freeLook.m_Orbits[i].m_Radius = num3;
			}
		}

		// Token: 0x06006A09 RID: 27145 RVA: 0x0008711F File Offset: 0x0008531F
		private static bool ZoomIn()
		{
			return GameManager.IsOnline && SolInput.GetButton(98);
		}

		// Token: 0x06006A0A RID: 27146 RVA: 0x00087131 File Offset: 0x00085331
		internal static bool ZoomOut()
		{
			return GameManager.IsOnline && SolInput.GetButton(99);
		}

		// Token: 0x04005C3C RID: 23612
		private const float kDefaultZoom = 0.5f;

		// Token: 0x04005C3D RID: 23613
		private const float kZoomKeyMultiplier = 2f;

		// Token: 0x04005C3E RID: 23614
		[SerializeField]
		private CameraManager m_manager;

		// Token: 0x04005C3F RID: 23615
		[SerializeField]
		private bool m_allowZoomTransition;

		// Token: 0x04005C40 RID: 23616
		[SerializeField]
		private bool m_saveLoadZoom;

		// Token: 0x04005C41 RID: 23617
		[SerializeField]
		private string m_saveLoadKey;

		// Token: 0x04005C42 RID: 23618
		[FormerlySerializedAs("minScale")]
		[Tooltip("The minimum scale for the orbits")]
		[Range(0.01f, 1f)]
		[SerializeField]
		private float m_minScale = 0.5f;

		// Token: 0x04005C43 RID: 23619
		[FormerlySerializedAs("maxScale")]
		[Tooltip("The maximum scale for the orbits")]
		[Range(1f, 5f)]
		[SerializeField]
		private float m_maxScale = 1f;

		// Token: 0x04005C44 RID: 23620
		[FormerlySerializedAs("zAxis")]
		[Tooltip("The zoom axis.  Value is 0..1.  How much to scale the orbits")]
		[AxisStateProperty]
		public AxisState m_zAxis = new AxisState(0f, 1f, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);

		// Token: 0x04005C45 RID: 23621
		[SerializeField]
		private LayerMask m_waterCheckLayerMask;

		// Token: 0x04005C46 RID: 23622
		[SerializeField]
		private float m_minRigHeightOverWater;

		// Token: 0x04005C47 RID: 23623
		[SerializeField]
		private float m_minRigRadiusOverWater = 1.8f;

		// Token: 0x04005C48 RID: 23624
		[SerializeField]
		private float m_waterRaycastOffset = 0.4f;

		// Token: 0x04005C49 RID: 23625
		[NonSerialized]
		private CinemachineFreeLook m_freeLook;

		// Token: 0x04005C4A RID: 23626
		[NonSerialized]
		private CinemachineFreeLook.Orbit[] m_originalOrbits;

		// Token: 0x04005C4B RID: 23627
		[NonSerialized]
		private float m_lastFrameZoom;

		// Token: 0x04005C4C RID: 23628
		[NonSerialized]
		private int m_zoomTransitionFrameCount;

		// Token: 0x04005C4D RID: 23629
		[NonSerialized]
		private float m_lastZoomTransitionTime;

		// Token: 0x04005C4E RID: 23630
		[NonSerialized]
		private string m_zoomAxisName = string.Empty;

		// Token: 0x04005C4F RID: 23631
		[NonSerialized]
		private bool m_allowAxisZoom;
	}
}
