using System;
using System.Collections.Generic;
using KinematicCharacterController;
using SoL.Game.Settings;
using SoL.GameCamera;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Player
{
	// Token: 0x020007F1 RID: 2033
	public class PlayerMotorControllerDummy : MonoBehaviour, ICharacterController
	{
		// Token: 0x17000D88 RID: 3464
		// (get) Token: 0x06003B1D RID: 15133 RVA: 0x00068085 File Offset: 0x00066285
		public KinematicCharacterMotor Motor
		{
			get
			{
				return this.m_motor;
			}
		}

		// Token: 0x17000D89 RID: 3465
		// (get) Token: 0x06003B1E RID: 15134 RVA: 0x0006808D File Offset: 0x0006628D
		// (set) Token: 0x06003B1F RID: 15135 RVA: 0x00068095 File Offset: 0x00066295
		private bool JumpConsumed
		{
			get
			{
				return this.m_jumpConsumed;
			}
			set
			{
				this.m_jumpConsumed = value;
			}
		}

		// Token: 0x06003B20 RID: 15136 RVA: 0x0006809E File Offset: 0x0006629E
		private void Awake()
		{
			this.m_motor.CharacterController = this;
			this.m_motor.MaxStableSlopeAngle = GlobalSettings.Values.Player.MaxStableSlopeAngle;
		}

		// Token: 0x06003B21 RID: 15137 RVA: 0x000680C6 File Offset: 0x000662C6
		private void Start()
		{
			this.m_targetRotation = this.m_motor.transform.rotation;
		}

		// Token: 0x06003B22 RID: 15138 RVA: 0x000680DE File Offset: 0x000662DE
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				this.m_timeSinceJumpRequested = 0f;
				this.m_jumpRequested = true;
			}
		}

		// Token: 0x06003B23 RID: 15139 RVA: 0x0017A708 File Offset: 0x00178908
		void ICharacterController.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			if (CameraManager.ActiveType.MatchAvatarRotationToCamera() || Input.GetMouseButton(1))
			{
				this.m_targetRotation = Quaternion.Euler(new Vector3(0f, Camera.main.transform.eulerAngles.y, 0f));
			}
			currentRotation = Quaternion.LerpUnclamped(this.m_motor.TransientRotation, this.m_targetRotation, this.m_turnSpeed * deltaTime);
		}

		// Token: 0x06003B24 RID: 15140 RVA: 0x0017A77C File Offset: 0x0017897C
		void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			if (this.m_motor.GroundingStatus.IsStableOnGround)
			{
				if (deltaTime > 0f)
				{
					currentVelocity = this.m_rootMotionDelta / deltaTime;
					currentVelocity = this.m_motor.GetDirectionTangentToSurface(currentVelocity, this.m_motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
				}
				else
				{
					currentVelocity = Vector3.zero;
				}
			}
			else
			{
				if (this.m_allowAirMovement)
				{
					Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
					Vector3 vector2 = vector * this.m_airAccelerationSpeed * deltaTime;
					if (this.Motor.GroundingStatus.FoundAnyGround)
					{
						Vector3 normalized = Vector3.Cross(Vector3.Cross(this.Motor.CharacterUp, this.Motor.GroundingStatus.GroundNormal), this.Motor.CharacterUp).normalized;
						vector2 = Vector3.ProjectOnPlane(vector2, normalized);
					}
					Vector3 vector3 = Vector3.ProjectOnPlane(currentVelocity + vector2, this.Motor.CharacterUp);
					if (vector3.magnitude > this.m_maxAirMoveSpeed && Vector3.Dot(vector, vector3) >= 0f)
					{
						vector2 = Vector3.zero;
					}
					else
					{
						Vector3 b = Vector3.ProjectOnPlane(currentVelocity, this.Motor.CharacterUp);
						vector2 = Vector3.ClampMagnitude(vector3, this.m_maxAirMoveSpeed) - b;
					}
					currentVelocity += vector2;
					currentVelocity += this.m_gravityMultiplier * Physics.gravity * deltaTime;
					currentVelocity *= 1f / (1f + this.m_airDrag * deltaTime);
				}
				else
				{
					currentVelocity += this.m_gravityMultiplier * Physics.gravity * deltaTime;
				}
				this.m_verticalVelocity = currentVelocity.y;
			}
			this.m_jumpedThisFrame = false;
			this.m_timeSinceJumpRequested += deltaTime;
			if (this.m_jumpRequested && !this.JumpConsumed && ((this.m_allowSlideJump ? this.m_motor.GroundingStatus.FoundAnyGround : this.m_motor.GroundingStatus.IsStableOnGround) || this.m_timeSinceLastAbleToJump <= this.m_jumpPostGroundingGraceTime))
			{
				Vector3 a = (this.m_motor.GroundingStatus.FoundAnyGround && !this.m_motor.GroundingStatus.IsStableOnGround) ? this.m_motor.GroundingStatus.GroundNormal : this.m_motor.CharacterUp;
				this.m_motor.ForceUnground(0.1f);
				currentVelocity += a * this.m_jumpSpeed - Vector3.Project(currentVelocity, this.m_motor.CharacterUp);
				this.m_jumpRequested = false;
				this.JumpConsumed = true;
				this.m_jumpedThisFrame = true;
			}
		}

		// Token: 0x06003B25 RID: 15141 RVA: 0x0004475B File Offset: 0x0004295B
		void ICharacterController.BeforeCharacterUpdate(float deltaTime)
		{
		}

		// Token: 0x06003B26 RID: 15142 RVA: 0x0004475B File Offset: 0x0004295B
		void ICharacterController.PostGroundingUpdate(float deltaTime)
		{
		}

		// Token: 0x06003B27 RID: 15143 RVA: 0x0017AA98 File Offset: 0x00178C98
		void ICharacterController.AfterCharacterUpdate(float deltaTime)
		{
			this.m_rootMotionDelta = Vector3.zero;
			if (this.m_jumpRequested && this.m_timeSinceJumpRequested > this.m_jumpPreGroundingGraceTime)
			{
				this.m_jumpRequested = false;
			}
			if (this.m_allowSlideJump ? this.m_motor.GroundingStatus.FoundAnyGround : this.m_motor.GroundingStatus.IsStableOnGround)
			{
				if (!this.m_jumpedThisFrame)
				{
					this.JumpConsumed = false;
				}
				this.m_timeSinceLastAbleToJump = 0f;
				return;
			}
			this.m_timeSinceLastAbleToJump += deltaTime;
		}

		// Token: 0x06003B28 RID: 15144 RVA: 0x000680FB File Offset: 0x000662FB
		bool ICharacterController.IsColliderValidForCollisions(Collider coll)
		{
			return this.m_layerMask.Contains(coll.gameObject.layer) && (this.m_ignroedColliders.Count == 0 || !this.m_ignroedColliders.Contains(coll));
		}

		// Token: 0x06003B29 RID: 15145 RVA: 0x0004475B File Offset: 0x0004295B
		void ICharacterController.OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06003B2A RID: 15146 RVA: 0x0004475B File Offset: 0x0004295B
		void ICharacterController.OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06003B2B RID: 15147 RVA: 0x0004475B File Offset: 0x0004295B
		void ICharacterController.ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06003B2C RID: 15148 RVA: 0x0004475B File Offset: 0x0004295B
		void ICharacterController.OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}

		// Token: 0x06003B2D RID: 15149 RVA: 0x00068137 File Offset: 0x00066337
		public void SetRootMotionDelta(Vector3 deltaPosition)
		{
			if (this.m_motor.GroundingStatus.IsStableOnGround)
			{
				this.m_rootMotionDelta += deltaPosition;
			}
		}

		// Token: 0x0400398E RID: 14734
		[SerializeField]
		private KinematicCharacterMotor m_motor;

		// Token: 0x0400398F RID: 14735
		[SerializeField]
		private float m_turnSpeed = 10f;

		// Token: 0x04003990 RID: 14736
		[SerializeField]
		private float m_gravityMultiplier = 1f;

		// Token: 0x04003991 RID: 14737
		[SerializeField]
		private float m_jumpSpeed = 10f;

		// Token: 0x04003992 RID: 14738
		[SerializeField]
		private float m_jumpPreGroundingGraceTime;

		// Token: 0x04003993 RID: 14739
		[SerializeField]
		private float m_jumpPostGroundingGraceTime;

		// Token: 0x04003994 RID: 14740
		[SerializeField]
		private bool m_allowSlideJump;

		// Token: 0x04003995 RID: 14741
		[SerializeField]
		private bool m_allowAirMovement;

		// Token: 0x04003996 RID: 14742
		[SerializeField]
		private float m_airAccelerationSpeed = 5f;

		// Token: 0x04003997 RID: 14743
		[SerializeField]
		private float m_airDrag = 0.1f;

		// Token: 0x04003998 RID: 14744
		[SerializeField]
		private float m_maxAirMoveSpeed = 20f;

		// Token: 0x04003999 RID: 14745
		[SerializeField]
		private LayerMask m_layerMask;

		// Token: 0x0400399A RID: 14746
		private bool m_jumpRequested;

		// Token: 0x0400399B RID: 14747
		private bool m_jumpConsumed;

		// Token: 0x0400399C RID: 14748
		private bool m_jumpedThisFrame;

		// Token: 0x0400399D RID: 14749
		private float m_timeSinceJumpRequested = float.PositiveInfinity;

		// Token: 0x0400399E RID: 14750
		private float m_timeSinceLastAbleToJump;

		// Token: 0x0400399F RID: 14751
		private float m_verticalVelocity;

		// Token: 0x040039A0 RID: 14752
		private List<Collider> m_ignroedColliders = new List<Collider>();

		// Token: 0x040039A1 RID: 14753
		public float StableMovementSharpness = 15f;

		// Token: 0x040039A2 RID: 14754
		private ICharacterController m_characterControllerImplementation;

		// Token: 0x040039A3 RID: 14755
		private Vector3 m_rootMotionDelta;

		// Token: 0x040039A4 RID: 14756
		private Quaternion m_targetRotation;
	}
}
