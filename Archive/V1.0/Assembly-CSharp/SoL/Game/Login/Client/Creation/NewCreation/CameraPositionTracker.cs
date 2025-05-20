using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation.NewCreation
{
	// Token: 0x02000B60 RID: 2912
	public class CameraPositionTracker : MonoBehaviour
	{
		// Token: 0x170014DD RID: 5341
		// (get) Token: 0x0600598F RID: 22927 RVA: 0x0007BFCF File Offset: 0x0007A1CF
		private bool m_showHeadOffset
		{
			get
			{
				return this.m_camType != CameraPositionTracker.CamType.Bottom;
			}
		}

		// Token: 0x06005990 RID: 22928 RVA: 0x0007BFDD File Offset: 0x0007A1DD
		private void Awake()
		{
			if (this.m_camType == CameraPositionTracker.CamType.Bottom)
			{
				this.ManagerOnStageEnterInitializeEvent();
				this.m_manager.StageEnterInitializeEvent += this.ManagerOnStageEnterInitializeEvent;
			}
		}

		// Token: 0x06005991 RID: 22929 RVA: 0x0007C005 File Offset: 0x0007A205
		private void OnDestroy()
		{
			if (this.m_camType == CameraPositionTracker.CamType.Bottom)
			{
				this.m_manager.StageEnterInitializeEvent -= this.ManagerOnStageEnterInitializeEvent;
			}
		}

		// Token: 0x06005992 RID: 22930 RVA: 0x001EA3F8 File Offset: 0x001E85F8
		private void Update()
		{
			switch (this.m_camType)
			{
			case CameraPositionTracker.CamType.Top:
			case CameraPositionTracker.CamType.Face:
				base.gameObject.transform.position = Vector3.Lerp(base.gameObject.transform.position, this.m_targetPos, Time.deltaTime * this.m_lerpSpeed);
				return;
			case CameraPositionTracker.CamType.Bottom:
			{
				float y = Input.mouseScrollDelta.y;
				if (y > 0f)
				{
					this.m_zoomSlider.value = Mathf.Clamp01(this.m_zoomSlider.value + this.m_mouseScrollScale * Time.deltaTime);
				}
				else if (y < 0f)
				{
					this.m_zoomSlider.value = Mathf.Clamp01(this.m_zoomSlider.value - this.m_mouseScrollScale * Time.deltaTime);
				}
				this.m_zoomAmount = Mathf.Lerp(this.m_zoomAmount, this.m_zoomSlider.value, Time.deltaTime * this.m_zoomLerpRate);
				this.LerpToY(this.m_zoomAmount, 0f, this.m_topTracker.gameObject.transform.localPosition.y - this.m_topDelta);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06005993 RID: 22931 RVA: 0x001EA520 File Offset: 0x001E8720
		private void LateUpdate()
		{
			if (this.m_camType != CameraPositionTracker.CamType.Bottom)
			{
				this.UpdateCharacter();
				Vector3 position = base.gameObject.transform.position;
				position.y = (this.m_headTransform ? (this.m_headTransform.position.y + this.m_headOffset) : this.m_headOffset);
				this.m_targetPos = position;
			}
		}

		// Token: 0x06005994 RID: 22932 RVA: 0x001EA588 File Offset: 0x001E8788
		private void LerpToY(float lerpFactor, float minPos, float maxPos)
		{
			float y = Mathf.Lerp(minPos, maxPos, lerpFactor);
			Vector3 b = new Vector3(0f, y, 0f);
			base.gameObject.transform.localPosition = Vector3.Lerp(base.gameObject.transform.localPosition, b, Time.deltaTime * this.m_lerpSpeed);
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x001EA5E4 File Offset: 0x001E87E4
		private void UpdateCharacter()
		{
			if (this.m_character != this.m_manager.CurrentCharacter)
			{
				this.m_headTransform = null;
				this.m_character = this.m_manager.CurrentCharacter;
				if (this.m_character != null && this.m_character.HeadPosObj != null)
				{
					this.m_headTransform = this.m_character.HeadPosObj.transform;
				}
			}
		}

		// Token: 0x06005996 RID: 22934 RVA: 0x0007C027 File Offset: 0x0007A227
		private void ManagerOnStageEnterInitializeEvent()
		{
			this.m_zoomAmount = 0f;
			if (this.m_zoomSlider)
			{
				this.m_zoomSlider.value = 0f;
			}
		}

		// Token: 0x04004EC2 RID: 20162
		[SerializeField]
		private NewCharacterManager m_manager;

		// Token: 0x04004EC3 RID: 20163
		[SerializeField]
		private CameraPositionTracker.CamType m_camType;

		// Token: 0x04004EC4 RID: 20164
		[SerializeField]
		private float m_lerpSpeed = 10f;

		// Token: 0x04004EC5 RID: 20165
		[SerializeField]
		private float m_headOffset;

		// Token: 0x04004EC6 RID: 20166
		[SerializeField]
		private CameraPositionTracker m_topTracker;

		// Token: 0x04004EC7 RID: 20167
		[SerializeField]
		private float m_mouseScrollScale = 0.1f;

		// Token: 0x04004EC8 RID: 20168
		[SerializeField]
		private float m_zoomLerpRate = 0.1f;

		// Token: 0x04004EC9 RID: 20169
		[SerializeField]
		private float m_topDelta = 0.5f;

		// Token: 0x04004ECA RID: 20170
		[SerializeField]
		private Slider m_zoomSlider;

		// Token: 0x04004ECB RID: 20171
		private NewCharacter m_character;

		// Token: 0x04004ECC RID: 20172
		private Transform m_headTransform;

		// Token: 0x04004ECD RID: 20173
		private Vector3 m_targetPos;

		// Token: 0x04004ECE RID: 20174
		private float m_zoomAmount;

		// Token: 0x02000B61 RID: 2913
		private enum CamType
		{
			// Token: 0x04004ED0 RID: 20176
			Top,
			// Token: 0x04004ED1 RID: 20177
			Bottom,
			// Token: 0x04004ED2 RID: 20178
			Face
		}
	}
}
