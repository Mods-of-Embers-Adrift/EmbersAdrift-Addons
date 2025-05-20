using System;
using System.Collections.Generic;
using Cinemachine;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE5 RID: 3557
	public class FirstPersonCamera : CameraBase<CinemachineVirtualCamera>
	{
		// Token: 0x06006A0C RID: 27148 RVA: 0x00219D08 File Offset: 0x00217F08
		protected override void Awake()
		{
			this.m_pov = this.m_camera.GetCinemachineComponent<CinemachinePOV>();
			if (this.m_pov == null)
			{
				Debug.LogWarning("MISSING POV FROM FIRST PERSON CAMERA");
				base.gameObject.SetActive(false);
				return;
			}
			this.m_body = this.m_camera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			if (this.m_body == null)
			{
				Debug.LogWarning("MISSING BODY FROM FIRST PERSON CAMERA");
				base.gameObject.SetActive(false);
				return;
			}
			this.m_defaultShoulderOffset = this.m_body.ShoulderOffset.y;
			base.gameObject.transform.SetParent(null);
			base.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			base.Awake();
		}

		// Token: 0x06006A0D RID: 27149 RVA: 0x00219DC8 File Offset: 0x00217FC8
		protected override void InitializeCamera()
		{
			this.m_pov.m_HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
			this.m_pov.m_HorizontalAxis.m_AccelTime = this.m_settings.AccelerationTime;
			this.m_pov.m_HorizontalAxis.m_DecelTime = this.m_settings.DecelerationTime;
			this.m_pov.m_HorizontalAxis.m_MaxSpeed = 0f;
			this.m_pov.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
			this.m_pov.m_VerticalAxis.m_AccelTime = this.m_settings.AccelerationTime;
			this.m_pov.m_VerticalAxis.m_DecelTime = this.m_settings.DecelerationTime;
			this.m_pov.m_VerticalAxis.m_MaxSpeed = 0f;
		}

		// Token: 0x06006A0E RID: 27150 RVA: 0x00087143 File Offset: 0x00085343
		protected override void InvertMouseOnChanged()
		{
			this.m_pov.m_VerticalAxis.m_InvertInput = !Options.GameOptions.InvertMouse.Value;
		}

		// Token: 0x17001943 RID: 6467
		// (get) Token: 0x06006A0F RID: 27151 RVA: 0x00087162 File Offset: 0x00085362
		// (set) Token: 0x06006A10 RID: 27152 RVA: 0x00087174 File Offset: 0x00085374
		protected override float XValue
		{
			get
			{
				return this.m_pov.m_HorizontalAxis.Value;
			}
			set
			{
				this.m_pov.m_HorizontalAxis.Value = value;
			}
		}

		// Token: 0x17001944 RID: 6468
		// (get) Token: 0x06006A11 RID: 27153 RVA: 0x00087187 File Offset: 0x00085387
		// (set) Token: 0x06006A12 RID: 27154 RVA: 0x00087199 File Offset: 0x00085399
		protected override float YValue
		{
			get
			{
				return this.m_pov.m_VerticalAxis.Value;
			}
			set
			{
				this.m_pov.m_VerticalAxis.Value = value;
			}
		}

		// Token: 0x06006A13 RID: 27155 RVA: 0x00219E90 File Offset: 0x00218090
		protected override bool AllowMovementX()
		{
			if (GameManager.IsOnline && (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Vitals))
			{
				return false;
			}
			Stance currentStance = LocalPlayer.GameEntity.Vitals.GetCurrentStance();
			HealthState currentHealthState = LocalPlayer.GameEntity.Vitals.GetCurrentHealthState();
			return currentStance != Stance.Sit && currentStance - Stance.Unconscious > 1 && currentHealthState == HealthState.Alive;
		}

		// Token: 0x06006A14 RID: 27156 RVA: 0x00219EF4 File Offset: 0x002180F4
		protected override void SetMaxSpeeds(float xMultiplier, float yMultiplier)
		{
			this.m_pov.m_HorizontalAxis.m_MaxSpeed = xMultiplier * this.m_settings.XSpeed * Options.GameOptions.XMouseSensitivity.Value;
			this.m_pov.m_VerticalAxis.m_MaxSpeed = yMultiplier * this.m_settings.YSpeed * Options.GameOptions.YMouseSensitivity.Value;
		}

		// Token: 0x06006A15 RID: 27157 RVA: 0x00219F54 File Offset: 0x00218154
		protected override void SetXValueOnActive()
		{
			if (this.m_type.SetAngleOnActive() && this.m_manager)
			{
				this.XValue = ((this.m_manager.LastActiveCameraXValue != null) ? this.m_manager.LastActiveCameraXValue.Value : this.m_manager.Focus.eulerAngles.y);
			}
		}

		// Token: 0x06006A16 RID: 27158 RVA: 0x00219FC0 File Offset: 0x002181C0
		protected override void UpdateVerticalOffset()
		{
			float num = this.m_defaultShoulderOffset;
			float num2;
			if (base.TryGetStanceOffset(out num2))
			{
				num = num2;
			}
			else if (this.TryGetCachedEyeOffset(out num2))
			{
				num = num2;
				if (this.m_camera.Follow)
				{
					num -= this.m_camera.Follow.localPosition.y;
				}
			}
			this.m_body.ShoulderOffset.y = Mathf.Lerp(this.m_body.ShoulderOffset.y, num, Time.deltaTime * this.m_settings.OffsetLerpSpeed);
		}

		// Token: 0x06006A17 RID: 27159 RVA: 0x0021A050 File Offset: 0x00218250
		protected override void UpdateExternal()
		{
			base.UpdateExternal();
			if (this.m_allowZoomTransition && Options.GameOptions.EnableZoomToFirstPerson.Value && (GameManager.IsOffline || !UIManager.EventSystem.IsPointerOverGameObject()) && (CinemachineFreeLookZoom.ZoomOut() || Input.mouseScrollDelta.y < 0f))
			{
				if (Time.time - this.m_lastZoomTransitionTime > InteractionManager.kDoubleClickThreshold)
				{
					this.m_zoomTransitionFrameCount = 0;
				}
				this.m_zoomTransitionFrameCount++;
				this.m_lastZoomTransitionTime = Time.time;
				if (this.m_zoomTransitionFrameCount > this.m_manager.ZoomTransitionFrameCount)
				{
					this.m_manager.ZoomOutFromFirstPerson();
				}
			}
		}

		// Token: 0x06006A18 RID: 27160 RVA: 0x0021A0F8 File Offset: 0x002182F8
		private bool TryGetCachedEyeOffset(out float offset)
		{
			if (!this.m_cachedEyeOffset)
			{
				CharacterData.CharacterSizeData characterSizeData;
				Vector2 vector;
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.TryGetCharacterVisualData(out characterSizeData) && ((characterSizeData.Sex == CharacterSex.Male) ? this.m_maleEyeOffsets : this.m_femaleEyeOffsets).TryGetValue(characterSizeData.BuildType, out vector))
				{
					this.m_eyeOffset = new float?(Mathf.Lerp(vector.x, vector.y, characterSizeData.Size));
				}
				this.m_cachedEyeOffset = true;
			}
			offset = ((this.m_eyeOffset != null) ? this.m_eyeOffset.Value : 0f);
			return this.m_eyeOffset != null;
		}

		// Token: 0x04005C50 RID: 23632
		[SerializeField]
		private bool m_allowZoomTransition;

		// Token: 0x04005C51 RID: 23633
		private CinemachinePOV m_pov;

		// Token: 0x04005C52 RID: 23634
		private Cinemachine3rdPersonFollow m_body;

		// Token: 0x04005C53 RID: 23635
		private float m_defaultShoulderOffset;

		// Token: 0x04005C54 RID: 23636
		private int m_zoomTransitionFrameCount;

		// Token: 0x04005C55 RID: 23637
		private float m_lastZoomTransitionTime;

		// Token: 0x04005C56 RID: 23638
		private readonly Dictionary<CharacterBuildType, Vector2> m_maleEyeOffsets = new Dictionary<CharacterBuildType, Vector2>(default(CharacterBuildTypeComparer))
		{
			{
				CharacterBuildType.Rotund,
				new Vector2(1.434767f, 1.918168f)
			},
			{
				CharacterBuildType.Heavyset,
				new Vector2(1.347773f, 2.126688f)
			},
			{
				CharacterBuildType.Lean,
				new Vector2(1.316083f, 2.052832f)
			},
			{
				CharacterBuildType.Brawny,
				new Vector2(1.42288f, 1.996266f)
			},
			{
				CharacterBuildType.Stoic,
				new Vector2(1.389284f, 2.123266f)
			}
		};

		// Token: 0x04005C57 RID: 23639
		private readonly Dictionary<CharacterBuildType, Vector2> m_femaleEyeOffsets = new Dictionary<CharacterBuildType, Vector2>(default(CharacterBuildTypeComparer))
		{
			{
				CharacterBuildType.Rotund,
				new Vector2(1.390539f, 1.824842f)
			},
			{
				CharacterBuildType.Heavyset,
				new Vector2(1.332321f, 1.829567f)
			},
			{
				CharacterBuildType.Lean,
				new Vector2(1.292496f, 1.982685f)
			},
			{
				CharacterBuildType.Brawny,
				new Vector2(1.379734f, 1.911302f)
			},
			{
				CharacterBuildType.Stoic,
				new Vector2(1.346252f, 2.023543f)
			}
		};

		// Token: 0x04005C58 RID: 23640
		[NonSerialized]
		private bool m_cachedEyeOffset;

		// Token: 0x04005C59 RID: 23641
		private float? m_eyeOffset;
	}
}
