using System;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Replication;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005AE RID: 1454
	public class PlayerAnimatorController : MonoBehaviour
	{
		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06002DD8 RID: 11736 RVA: 0x0005FC62 File Offset: 0x0005DE62
		private IInputManager m_input
		{
			get
			{
				return ClientGameManager.InputManager;
			}
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x0014FE44 File Offset: 0x0014E044
		private void Awake()
		{
			this.m_controller.TookOff += this.OnTookOff;
			this.m_controller.Landed += this.OnLanded;
			this.m_leftFoot = this.m_animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			this.m_rightFoot = this.m_animator.GetBoneTransform(HumanBodyBones.RightFoot);
			this.m_animator.gameObject.AddComponent<OnAnimatorMoveForwarder>().Receiver = this.m_controller;
		}

		// Token: 0x06002DDA RID: 11738 RVA: 0x0005FC69 File Offset: 0x0005DE69
		private void OnDestroy()
		{
			this.m_controller.TookOff -= this.OnTookOff;
			this.m_controller.Landed -= this.OnLanded;
		}

		// Token: 0x06002DDB RID: 11739 RVA: 0x0014FEC0 File Offset: 0x0014E0C0
		private void Update()
		{
			bool holdingShift = this.m_input.HoldingShift;
			bool isCrouching = this.m_controller.IsCrouching;
			float value = isCrouching ? 1f : 0f;
			float value2 = 0f;
			if (this.m_controller.AngleDelta > 0.2f)
			{
				value2 = 1f;
			}
			else if (this.m_controller.AngleDelta < -0.2f)
			{
				value2 = -1f;
			}
			float num = 1f;
			float num2 = ((holdingShift && !isCrouching) ? 2f : 1f) * num;
			float num3 = this.m_input.NormalizedMovementInputMagnitude * num2;
			Vector2 movementInput = this.m_input.MovementInput;
			this.SetFloat("InputY", movementInput.y * num3);
			this.SetFloat("InputX", movementInput.x * num3);
			this.SetFloat("Crouched", value);
			this.SetFloat("Rotation", value2);
			if (Input.GetKeyDown(KeyCode.Z))
			{
				int value3 = (this.m_animator.GetInteger("State") == 0) ? 1 : 0;
				this.SetInteger("State", value3);
			}
			if (Input.GetKeyDown(KeyCode.LeftBracket))
			{
				int num4 = this.m_animator.GetInteger("SubState");
				if (num4 == 0)
				{
					num4 = 7;
				}
				else
				{
					num4--;
				}
				this.SetInteger("SubState", num4);
			}
			if (Input.GetKeyDown(KeyCode.RightBracket))
			{
				int num5 = this.m_animator.GetInteger("SubState");
				if (num5 == 7)
				{
					num5 = 0;
				}
				else
				{
					num5++;
				}
				this.SetInteger("SubState", num5);
			}
			if (!this.m_grounded)
			{
				this.SetFloat("Vertical", this.m_controller.VerticalVelocity);
			}
		}

		// Token: 0x06002DDC RID: 11740 RVA: 0x0005FC99 File Offset: 0x0005DE99
		private void SetFloat(string key, float value)
		{
			this.m_replicator.SetFloat(key, value);
		}

		// Token: 0x06002DDD RID: 11741 RVA: 0x0005FCA8 File Offset: 0x0005DEA8
		private void SetInteger(string key, int value)
		{
			this.m_replicator.SetInteger(key, value);
		}

		// Token: 0x06002DDE RID: 11742 RVA: 0x0005FCB7 File Offset: 0x0005DEB7
		private void SetBool(string key, bool value)
		{
			this.m_replicator.SetBool(key, value);
		}

		// Token: 0x06002DDF RID: 11743 RVA: 0x00150064 File Offset: 0x0014E264
		public static float GetInterpolatedValue(float current, float target, NetworkedAnimatorParamSetting setting)
		{
			float num = (target == 0f) ? setting.Deceleration : setting.Acceleration;
			return Mathf.MoveTowards(current, target, num * Time.deltaTime);
		}

		// Token: 0x06002DE0 RID: 11744 RVA: 0x0005FCC6 File Offset: 0x0005DEC6
		private void OnTookOff(bool jumped)
		{
			this.m_grounded = false;
			this.SetBool("Grounded", this.m_grounded);
		}

		// Token: 0x06002DE1 RID: 11745 RVA: 0x0005FCE2 File Offset: 0x0005DEE2
		private void OnLanded()
		{
			this.m_grounded = true;
			this.SetBool("Grounded", this.m_grounded);
		}

		// Token: 0x04002D3B RID: 11579
		private const float kMinSlowGroundAngle = 20f;

		// Token: 0x04002D3C RID: 11580
		private const float kMaxSlowFraction = 0.9f;

		// Token: 0x04002D3D RID: 11581
		private const float kMinSlowFraction = 0.4f;

		// Token: 0x04002D3E RID: 11582
		[SerializeField]
		private AnimatorReplicator m_replicator;

		// Token: 0x04002D3F RID: 11583
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04002D40 RID: 11584
		[SerializeField]
		private PlayerMotorController m_controller;

		// Token: 0x04002D41 RID: 11585
		private bool m_grounded = true;

		// Token: 0x04002D42 RID: 11586
		private Transform m_leftFoot;

		// Token: 0x04002D43 RID: 11587
		private Transform m_rightFoot;
	}
}
