using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Targeting;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000876 RID: 2166
	public class DirectionIndicatorUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000E8F RID: 3727
		// (get) Token: 0x06003EF2 RID: 16114 RVA: 0x0006A958 File Offset: 0x00068B58
		// (set) Token: 0x06003EF3 RID: 16115 RVA: 0x0006A960 File Offset: 0x00068B60
		public GameObject Target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				if (value == this.m_target)
				{
					return;
				}
				this.m_target = value;
				this.Update();
			}
		}

		// Token: 0x17000E90 RID: 3728
		// (get) Token: 0x06003EF4 RID: 16116 RVA: 0x0006A97E File Offset: 0x00068B7E
		// (set) Token: 0x06003EF5 RID: 16117 RVA: 0x00186A58 File Offset: 0x00184C58
		public bool DisableDynamics
		{
			get
			{
				return this.m_disableDynamics;
			}
			set
			{
				this.m_disableDynamics = value;
				if (this.m_toRotate)
				{
					this.m_toRotate.gameObject.SetActive(!this.m_disableDynamics);
				}
				if (this.m_toScale && this.m_disableDynamics)
				{
					this.m_toScale.localScale = Vector3.one;
				}
			}
		}

		// Token: 0x06003EF6 RID: 16118 RVA: 0x0006A986 File Offset: 0x00068B86
		public void SetTarget(ITargetable targetable)
		{
			this.Target = ((targetable != null) ? targetable.Entity.gameObject : null);
		}

		// Token: 0x06003EF7 RID: 16119 RVA: 0x00186AB8 File Offset: 0x00184CB8
		private void Update()
		{
			if (!this.DisableDynamics && this.m_target && ClientGameManager.MainCamera && LocalPlayer.GameEntity)
			{
				Vector3 to = this.m_target.transform.position - LocalPlayer.GameEntity.gameObject.transform.position;
				to.y = 0f;
				Vector3 from = Vector3.ProjectOnPlane(ClientGameManager.MainCamera.gameObject.transform.forward, Vector3.up);
				float num = -1f * Vector3.SignedAngle(from, to, Vector3.up);
				this.m_toRotate.localEulerAngles = new Vector3(0f, 0f, num + this.m_rotationOffset);
				if (this.m_scaleIndicator)
				{
					Vector3 vector = Vector3.one;
					float sqrMagnitude = to.sqrMagnitude;
					if (sqrMagnitude > 25f)
					{
						if (sqrMagnitude >= 2500f)
						{
							vector = this.m_minScale;
						}
						else
						{
							float t = (sqrMagnitude - 25f) / 2475f;
							vector = Vector3.Lerp(Vector3.one, this.m_minScale, t);
						}
					}
					if (this.m_toScale.localScale != vector)
					{
						this.m_toScale.localScale = vector;
					}
				}
				if (this.m_image && !this.m_image.enabled)
				{
					this.m_image.enabled = true;
					return;
				}
			}
			else if (this.m_image && this.m_image.enabled)
			{
				this.m_image.enabled = false;
			}
		}

		// Token: 0x06003EF8 RID: 16120 RVA: 0x00186C4C File Offset: 0x00184E4C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.Target && LocalPlayer.GameEntity)
			{
				int arg = Mathf.FloorToInt(Vector3.Distance(this.Target.transform.position, LocalPlayer.GameEntity.gameObject.transform.position));
				string txt = ZString.Format<int>("{0}m", arg);
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17000E91 RID: 3729
		// (get) Token: 0x06003EF9 RID: 16121 RVA: 0x0006A99F File Offset: 0x00068B9F
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E92 RID: 3730
		// (get) Token: 0x06003EFA RID: 16122 RVA: 0x0006A9AD File Offset: 0x00068BAD
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000E93 RID: 3731
		// (get) Token: 0x06003EFB RID: 16123 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003EFD RID: 16125 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003CE5 RID: 15589
		[SerializeField]
		private RectTransform m_toRotate;

		// Token: 0x04003CE6 RID: 15590
		[SerializeField]
		private RectTransform m_toScale;

		// Token: 0x04003CE7 RID: 15591
		[SerializeField]
		private Image m_image;

		// Token: 0x04003CE8 RID: 15592
		[SerializeField]
		private float m_rotationOffset = 45f;

		// Token: 0x04003CE9 RID: 15593
		[SerializeField]
		private bool m_scaleIndicator;

		// Token: 0x04003CEA RID: 15594
		[SerializeField]
		private Vector3 m_minScale = Vector3.one * 0.5f;

		// Token: 0x04003CEB RID: 15595
		private const float kMaxScaleDistance = 5f;

		// Token: 0x04003CEC RID: 15596
		private const float kMinScaleDistance = 50f;

		// Token: 0x04003CED RID: 15597
		private const float kMaxScaleDistanceSqr = 25f;

		// Token: 0x04003CEE RID: 15598
		private const float kMinScaleDistanceSqr = 2500f;

		// Token: 0x04003CEF RID: 15599
		private GameObject m_target;

		// Token: 0x04003CF0 RID: 15600
		private bool m_disableDynamics;

		// Token: 0x04003CF1 RID: 15601
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
