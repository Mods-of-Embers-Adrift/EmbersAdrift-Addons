using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
	// Token: 0x02000082 RID: 130
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class RigidbodyFirstPersonController : MonoBehaviour
	{
		// Token: 0x1700026C RID: 620
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x00046C5D File Offset: 0x00044E5D
		public Vector3 Velocity
		{
			get
			{
				return this.m_RigidBody.velocity;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x00046C6A File Offset: 0x00044E6A
		public bool Grounded
		{
			get
			{
				return this.m_IsGrounded;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x00046C72 File Offset: 0x00044E72
		public bool Jumping
		{
			get
			{
				return this.m_Jumping;
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x00046C7A File Offset: 0x00044E7A
		public bool Running
		{
			get
			{
				return this.movementSettings.Running;
			}
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00046C87 File Offset: 0x00044E87
		private void Start()
		{
			this.m_RigidBody = base.GetComponent<Rigidbody>();
			this.m_Capsule = base.GetComponent<CapsuleCollider>();
			this.mouseLook.Init(base.transform, this.cam.transform);
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x00046CBD File Offset: 0x00044EBD
		private void Update()
		{
			this.RotateView();
			if (Input.GetButtonDown("Jump") && !this.m_Jump)
			{
				this.m_Jump = true;
			}
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0009F248 File Offset: 0x0009D448
		private void FixedUpdate()
		{
			this.GroundCheck();
			Vector2 input = this.GetInput();
			if ((Mathf.Abs(input.x) > 1E-45f || Mathf.Abs(input.y) > 1E-45f) && (this.advancedSettings.airControl || this.m_IsGrounded))
			{
				Vector3 vector = this.cam.transform.forward * input.y + this.cam.transform.right * input.x;
				vector = Vector3.ProjectOnPlane(vector, this.m_GroundContactNormal).normalized;
				vector.x *= this.movementSettings.CurrentTargetSpeed;
				vector.z *= this.movementSettings.CurrentTargetSpeed;
				vector.y *= this.movementSettings.CurrentTargetSpeed;
				if (this.m_RigidBody.velocity.sqrMagnitude < this.movementSettings.CurrentTargetSpeed * this.movementSettings.CurrentTargetSpeed)
				{
					this.m_RigidBody.AddForce(vector * this.SlopeMultiplier(), ForceMode.Impulse);
				}
			}
			if (this.m_IsGrounded)
			{
				this.m_RigidBody.drag = 5f;
				if (this.m_Jump)
				{
					this.m_RigidBody.drag = 0f;
					this.m_RigidBody.velocity = new Vector3(this.m_RigidBody.velocity.x, 0f, this.m_RigidBody.velocity.z);
					this.m_RigidBody.AddForce(new Vector3(0f, this.movementSettings.JumpForce, 0f), ForceMode.Impulse);
					this.m_Jumping = true;
				}
				if (!this.m_Jumping && Mathf.Abs(input.x) < 1E-45f && Mathf.Abs(input.y) < 1E-45f && this.m_RigidBody.velocity.magnitude < 1f)
				{
					this.m_RigidBody.Sleep();
				}
			}
			else
			{
				this.m_RigidBody.drag = 0f;
				if (this.m_PreviouslyGrounded && !this.m_Jumping)
				{
					this.StickToGroundHelper();
				}
			}
			this.m_Jump = false;
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0009F498 File Offset: 0x0009D698
		private float SlopeMultiplier()
		{
			float time = Vector3.Angle(this.m_GroundContactNormal, Vector3.up);
			return this.movementSettings.SlopeCurveModifier.Evaluate(time);
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x0009F4C8 File Offset: 0x0009D6C8
		private void StickToGroundHelper()
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius * (1f - this.advancedSettings.shellOffset), Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.stickToGroundHelperDistance, -1, QueryTriggerInteraction.Ignore) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
			{
				this.m_RigidBody.velocity = Vector3.ProjectOnPlane(this.m_RigidBody.velocity, raycastHit.normal);
			}
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0009F578 File Offset: 0x0009D778
		private Vector2 GetInput()
		{
			Vector2 vector = new Vector2
			{
				x = Input.GetAxis("Horizontal"),
				y = Input.GetAxis("Vertical")
			};
			this.movementSettings.UpdateDesiredTargetSpeed(vector);
			return vector;
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0009F5C0 File Offset: 0x0009D7C0
		private void RotateView()
		{
			if (Mathf.Abs(Time.timeScale) < 1E-45f)
			{
				return;
			}
			float y = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, this.cam.transform);
			if (this.m_IsGrounded || this.advancedSettings.airControl)
			{
				Quaternion rotation = Quaternion.AngleAxis(base.transform.eulerAngles.y - y, Vector3.up);
				this.m_RigidBody.velocity = rotation * this.m_RigidBody.velocity;
			}
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0009F65C File Offset: 0x0009D85C
		private void GroundCheck()
		{
			this.m_PreviouslyGrounded = this.m_IsGrounded;
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this.m_Capsule.radius * (1f - this.advancedSettings.shellOffset), Vector3.down, out raycastHit, this.m_Capsule.height / 2f - this.m_Capsule.radius + this.advancedSettings.groundCheckDistance, -1, QueryTriggerInteraction.Ignore))
			{
				this.m_IsGrounded = true;
				this.m_GroundContactNormal = raycastHit.normal;
			}
			else
			{
				this.m_IsGrounded = false;
				this.m_GroundContactNormal = Vector3.up;
			}
			if (!this.m_PreviouslyGrounded && this.m_IsGrounded && this.m_Jumping)
			{
				this.m_Jumping = false;
			}
		}

		// Token: 0x040005DE RID: 1502
		public Camera cam;

		// Token: 0x040005DF RID: 1503
		public RigidbodyFirstPersonController.MovementSettings movementSettings = new RigidbodyFirstPersonController.MovementSettings();

		// Token: 0x040005E0 RID: 1504
		public MouseLook mouseLook = new MouseLook();

		// Token: 0x040005E1 RID: 1505
		public RigidbodyFirstPersonController.AdvancedSettings advancedSettings = new RigidbodyFirstPersonController.AdvancedSettings();

		// Token: 0x040005E2 RID: 1506
		private Rigidbody m_RigidBody;

		// Token: 0x040005E3 RID: 1507
		private CapsuleCollider m_Capsule;

		// Token: 0x040005E4 RID: 1508
		private float m_YRotation;

		// Token: 0x040005E5 RID: 1509
		private Vector3 m_GroundContactNormal;

		// Token: 0x040005E6 RID: 1510
		private bool m_Jump;

		// Token: 0x040005E7 RID: 1511
		private bool m_PreviouslyGrounded;

		// Token: 0x040005E8 RID: 1512
		private bool m_Jumping;

		// Token: 0x040005E9 RID: 1513
		private bool m_IsGrounded;

		// Token: 0x02000083 RID: 131
		[Serializable]
		public class MovementSettings
		{
			// Token: 0x0600055A RID: 1370 RVA: 0x0009F720 File Offset: 0x0009D920
			public void UpdateDesiredTargetSpeed(Vector2 input)
			{
				if (input == Vector2.zero)
				{
					return;
				}
				if (input.x > 0f || input.x < 0f)
				{
					this.CurrentTargetSpeed = this.StrafeSpeed;
				}
				if (input.y < 0f)
				{
					this.CurrentTargetSpeed = this.BackwardSpeed;
				}
				if (input.y > 0f)
				{
					this.CurrentTargetSpeed = this.ForwardSpeed;
				}
				if (Input.GetKey(this.RunKey))
				{
					this.CurrentTargetSpeed *= this.RunMultiplier;
					this.m_Running = true;
					return;
				}
				this.m_Running = false;
			}

			// Token: 0x17000270 RID: 624
			// (get) Token: 0x0600055B RID: 1371 RVA: 0x00046D09 File Offset: 0x00044F09
			public bool Running
			{
				get
				{
					return this.m_Running;
				}
			}

			// Token: 0x040005EA RID: 1514
			public float ForwardSpeed = 8f;

			// Token: 0x040005EB RID: 1515
			public float BackwardSpeed = 4f;

			// Token: 0x040005EC RID: 1516
			public float StrafeSpeed = 4f;

			// Token: 0x040005ED RID: 1517
			public float RunMultiplier = 2f;

			// Token: 0x040005EE RID: 1518
			public KeyCode RunKey = KeyCode.LeftShift;

			// Token: 0x040005EF RID: 1519
			public float JumpForce = 30f;

			// Token: 0x040005F0 RID: 1520
			public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(-90f, 1f),
				new Keyframe(0f, 1f),
				new Keyframe(90f, 0f)
			});

			// Token: 0x040005F1 RID: 1521
			[HideInInspector]
			public float CurrentTargetSpeed = 8f;

			// Token: 0x040005F2 RID: 1522
			private bool m_Running;
		}

		// Token: 0x02000084 RID: 132
		[Serializable]
		public class AdvancedSettings
		{
			// Token: 0x040005F3 RID: 1523
			public float groundCheckDistance = 0.01f;

			// Token: 0x040005F4 RID: 1524
			public float stickToGroundHelperDistance = 0.5f;

			// Token: 0x040005F5 RID: 1525
			public float slowDownRate = 20f;

			// Token: 0x040005F6 RID: 1526
			public bool airControl;

			// Token: 0x040005F7 RID: 1527
			[Tooltip("set it to 0.1 or more if you get stuck in wall")]
			public float shellOffset;
		}
	}
}
