using System;
using System.Collections.Generic;
using System.Globalization;
using KinematicCharacterController;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Player;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.GameCamera;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game
{
	// Token: 0x020005B1 RID: 1457
	public class PlayerMotorController : GameEntityComponent, ICharacterController, IOnAnimatorMoveReceiver, IMotor
	{
		// Token: 0x1400009D RID: 157
		// (add) Token: 0x06002DEA RID: 11754 RVA: 0x001500B8 File Offset: 0x0014E2B8
		// (remove) Token: 0x06002DEB RID: 11755 RVA: 0x001500F0 File Offset: 0x0014E2F0
		public event Action<bool> TookOff;

		// Token: 0x1400009E RID: 158
		// (add) Token: 0x06002DEC RID: 11756 RVA: 0x00150128 File Offset: 0x0014E328
		// (remove) Token: 0x06002DED RID: 11757 RVA: 0x00150160 File Offset: 0x0014E360
		public event Action Landed;

		// Token: 0x1400009F RID: 159
		// (add) Token: 0x06002DEE RID: 11758 RVA: 0x00150198 File Offset: 0x0014E398
		// (remove) Token: 0x06002DEF RID: 11759 RVA: 0x001501CC File Offset: 0x0014E3CC
		public static event Action ResetCameraRotation;

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06002DF0 RID: 11760 RVA: 0x0005FD8F File Offset: 0x0005DF8F
		// (set) Token: 0x06002DF1 RID: 11761 RVA: 0x0005FD97 File Offset: 0x0005DF97
		public float AngleDelta { get; private set; }

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06002DF2 RID: 11762 RVA: 0x0005FDA0 File Offset: 0x0005DFA0
		// (set) Token: 0x06002DF3 RID: 11763 RVA: 0x0005FDA8 File Offset: 0x0005DFA8
		private bool JumpConsumed
		{
			get
			{
				return this.m_jumpConsumed;
			}
			set
			{
				this.m_jumpConsumed = value;
				if (this.m_jumpConsumed)
				{
					base.GameEntity.NetworkEntity.PlayerRpcHandler.ClientJumped();
				}
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x06002DF4 RID: 11764 RVA: 0x0005FDCE File Offset: 0x0005DFCE
		// (set) Token: 0x06002DF5 RID: 11765 RVA: 0x0005FDD6 File Offset: 0x0005DFD6
		public float SpeedMod
		{
			get
			{
				return this.m_speedMod;
			}
			set
			{
				this.m_speedMod = value;
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06002DF6 RID: 11766 RVA: 0x0005FDDF File Offset: 0x0005DFDF
		public Vector2 TargetLocomotion
		{
			get
			{
				return this.m_targetLocomotion;
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06002DF7 RID: 11767 RVA: 0x0005FDE7 File Offset: 0x0005DFE7
		public KinematicCharacterMotor Motor
		{
			get
			{
				return this.m_motor;
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06002DF8 RID: 11768 RVA: 0x0005FDEF File Offset: 0x0005DFEF
		public bool IsCrouching
		{
			get
			{
				return this.m_isCrouching;
			}
		}

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06002DF9 RID: 11769 RVA: 0x0005FDF7 File Offset: 0x0005DFF7
		public bool StepDetected
		{
			get
			{
				return this.m_stepDetected;
			}
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x06002DFA RID: 11770 RVA: 0x0005FDFF File Offset: 0x0005DFFF
		public float VerticalVelocity
		{
			get
			{
				return this.m_verticalVelocity;
			}
		}

		// Token: 0x06002DFB RID: 11771 RVA: 0x00150200 File Offset: 0x0014E400
		private void Awake()
		{
			this.m_armstrong = new Armstrong();
			LocalPlayer.Motor = this;
			this.m_motor.CharacterController = this;
			this.m_motor.MaxStableSlopeAngle = GlobalSettings.Values.Player.MaxStableSlopeAngle;
			if (base.GameEntity != null)
			{
				base.GameEntity.Motor = this;
			}
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x0005FE07 File Offset: 0x0005E007
		private void Start()
		{
			this.m_targetRotation = this.m_motor.transform.rotation;
		}

		// Token: 0x06002DFD RID: 11773 RVA: 0x00150260 File Offset: 0x0014E460
		private void Update()
		{
			bool flag = !ClientGameManager.InputManager.PreventCharacterMovement;
			Stance currentStance = base.GameEntity.Vitals.GetCurrentStance();
			if (flag && SolInput.GetButtonDown(9) && currentStance.CanJump() && base.GameEntity.Vitals.Stamina / 100f >= GlobalSettings.Values.Player.StaminaCostJump && (!base.GameEntity.SkillsController.PendingIsActive || GlobalSettings.Values.Player.AllowJumpToCancelExecution))
			{
				this.RequestJump();
			}
			this.ToggleCrouch(ClientGameManager.InputManager.IsCrouching && flag && currentStance == Stance.Crouch);
			this.UpdateStuck();
			this.UpdateRope();
			this.UpdateFall();
			if (this.m_cameraDisabledFrame != null && Time.frameCount > this.m_cameraDisabledFrame.Value)
			{
				CameraSettings.EnableCameras();
				this.m_cameraDisabledFrame = null;
			}
			this.m_armstrong.UpdateExternal();
		}

		// Token: 0x06002DFE RID: 11774 RVA: 0x00150358 File Offset: 0x0014E558
		private void UpdatePositionBreadCrumbs()
		{
			if (Time.time < this.m_nextBreadcrumbUpdateTime || this.m_stuckRequestTime != null)
			{
				return;
			}
			this.m_nextBreadcrumbUpdateTime = Time.time + 10f;
			if (this.m_positionBreadcrumbs.Count <= 0)
			{
				this.m_positionBreadcrumbs.Add(base.GameEntity.gameObject.transform.position);
				return;
			}
			Vector3 b = this.m_positionBreadcrumbs[this.m_positionBreadcrumbs.Count - 1];
			if ((base.GameEntity.gameObject.transform.position - b).sqrMagnitude > 16f)
			{
				this.m_positionBreadcrumbs.Add(base.GameEntity.gameObject.transform.position);
			}
			while (this.m_positionBreadcrumbs.Count > 10)
			{
				this.m_positionBreadcrumbs.RemoveAt(0);
			}
		}

		// Token: 0x06002DFF RID: 11775 RVA: 0x00150440 File Offset: 0x0014E640
		private void UpdateStuck()
		{
			if (this.m_stuckRequestTime == null)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow - this.m_stuckRequestTime.Value).TotalSeconds >= 10.0)
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.StuckRequest();
				this.m_stuckRequestTime = null;
				this.m_lastStuckRopeUsed = utcNow;
			}
		}

		// Token: 0x06002E00 RID: 11776 RVA: 0x001504B0 File Offset: 0x0014E6B0
		private void UpdateRope()
		{
			if (this.m_ropeRequestTime == null)
			{
				return;
			}
			if (!PlayerMotorController.ValidRopeRequest(base.GameEntity))
			{
				this.m_ropeRequestTime = null;
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow - this.m_ropeRequestTime.Value).TotalSeconds >= 10.0)
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.RopeRequest();
				this.m_ropeRequestTime = null;
				this.m_lastStuckRopeUsed = utcNow;
			}
		}

		// Token: 0x06002E01 RID: 11777 RVA: 0x00150538 File Offset: 0x0014E738
		private void UpdateFall()
		{
			if (base.GameEntity.Vitals.GetCurrentHealthState() != HealthState.Alive)
			{
				this.m_startingFallHeight = null;
				return;
			}
			if (this.m_motor.GroundingStatus.FoundAnyGround)
			{
				if (this.m_startingFallHeight != null)
				{
					float num = this.m_startingFallHeight.Value - base.GameEntity.gameObject.transform.position.y;
					if (GlobalSettings.Values.Player.FallDamageCurve.Evaluate(num) > 0f)
					{
						Debug.Log("Taking damage for falling " + num.ToString(CultureInfo.InvariantCulture) + "m!");
						base.GameEntity.NetworkEntity.PlayerRpcHandler.TakeFallDamage(num);
					}
					this.m_startingFallHeight = null;
				}
				return;
			}
			if (this.m_startingFallHeight == null)
			{
				this.m_startingFallHeight = new float?(base.GameEntity.gameObject.transform.position.y);
			}
		}

		// Token: 0x06002E02 RID: 11778 RVA: 0x00150644 File Offset: 0x0014E844
		public void StuckRequested()
		{
			if (this.m_stuckRequestTime != null)
			{
				float value = 10f - (float)(DateTime.UtcNow - this.m_stuckRequestTime.Value).TotalSeconds;
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Stuck pending! " + value.GetFormattedTime(true) + " remaining...");
				return;
			}
			if (this.m_ropeRequestTime != null)
			{
				float value2 = 10f - (float)(DateTime.UtcNow - this.m_ropeRequestTime.Value).TotalSeconds;
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Rope pending! " + value2.GetFormattedTime(true) + " remaining...");
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow - this.m_lastStuckRopeUsed).TotalSeconds < 60.0)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Stuck/Rope can only be used every " + 60f.GetFormattedTime(true) + "!");
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Stuck executing in " + 10f.GetFormattedTime(true) + "...");
			this.m_stuckRequestTime = new DateTime?(utcNow);
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x0015077C File Offset: 0x0014E97C
		public void RopeRequested()
		{
			if (this.m_ropeRequestTime != null)
			{
				float value = 10f - (float)(DateTime.UtcNow - this.m_ropeRequestTime.Value).TotalSeconds;
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Rope pending! " + value.GetFormattedTime(true) + " remaining...");
				return;
			}
			if (this.m_stuckRequestTime != null)
			{
				float value2 = 10f - (float)(DateTime.UtcNow - this.m_stuckRequestTime.Value).TotalSeconds;
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Stuck pending! " + value2.GetFormattedTime(true) + " remaining...");
				return;
			}
			if (!PlayerMotorController.ValidRopeRequest(base.GameEntity))
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow - this.m_lastStuckRopeUsed).TotalSeconds < 60.0)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Stuck/Rope can only be used every " + 60f.GetFormattedTime(true) + "!");
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Rope executing in " + 10f.GetFormattedTime(true) + "...");
			this.m_ropeRequestTime = new DateTime?(utcNow);
		}

		// Token: 0x06002E04 RID: 11780 RVA: 0x001508C0 File Offset: 0x0014EAC0
		public static bool ValidRopeRequest(GameEntity source)
		{
			if (source == null || source.TargetController == null)
			{
				return false;
			}
			if (source.TargetController.DefensiveTarget == null)
			{
				if (!GameManager.IsServer)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No defensive target!");
				}
				return false;
			}
			Vector3 position = source.gameObject.transform.position;
			Vector3 position2 = source.TargetController.DefensiveTarget.gameObject.transform.position;
			if ((position - position2).sqrMagnitude > PlayerMotorController.kMaxRopeDistanceSqr)
			{
				if (!GameManager.IsServer)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Defensive target out of range!");
				}
				return false;
			}
			return true;
		}

		// Token: 0x06002E05 RID: 11781 RVA: 0x00150970 File Offset: 0x0014EB70
		public void StuckResponse(StuckTransaction response)
		{
			if (response.Op == OpCodes.Ok)
			{
				if (response.Position != null)
				{
					RaycastHit raycastHit;
					if (Physics.Raycast(response.Position.Value + Vector3.up * 0.2f, Vector3.down, out raycastHit, 50f, this.m_layerMask, QueryTriggerInteraction.Ignore))
					{
						this.SetPosition(raycastHit.point);
					}
					else
					{
						this.SetPosition(response.Position.Value);
					}
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Stuck processed.");
					return;
				}
			}
			else
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unable to process stuck request");
			}
		}

		// Token: 0x06002E06 RID: 11782 RVA: 0x0005FE1F File Offset: 0x0005E01F
		void IOnAnimatorMoveReceiver.OnAnimatorMoveExternal(Vector3 deltaPosition)
		{
			if (this.Motor.GroundingStatus.IsStableOnGround)
			{
				this.m_rootMotionDelta += deltaPosition;
			}
		}

		// Token: 0x06002E07 RID: 11783 RVA: 0x00150A20 File Offset: 0x0014EC20
		private float GetSpeedModifier()
		{
			Stance stance = LocalPlayer.GameEntity.Vitals.Stance;
			StanceProfile stanceProfile = stance.GetStanceProfile();
			float num = this.m_vitalsSpeedMod;
			if (LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.OnRoad))
			{
				num += GlobalSettings.Values.Player.OnRoadSpeedMod;
			}
			float num2 = (LocalPlayer.GameEntity.SkillsController.PendingIsActive && LocalPlayer.GameEntity.SkillsController.Pending.ExecutionParams != null) ? LocalPlayer.GameEntity.SkillsController.Pending.ExecutionParams.GetMovementModifier() : 0f;
			float num3 = stanceProfile.MovementModifier;
			if (stance == Stance.Combat && num3 < 0f)
			{
				float num4 = (float)LocalPlayer.GameEntity.Vitals.GetStatusEffectValue(StatType.CombatMovement) * 0.01f;
				num3 = stanceProfile.MovementModifier + Mathf.Abs(stanceProfile.MovementModifier) * num4;
			}
			float slopeMovementSpeedFraction = this.GetSlopeMovementSpeedFraction();
			float num5 = 1f;
			if (this.m_rootEffects.Count > 0 || num3 <= -1f || num <= -1f || num2 <= -1f || slopeMovementSpeedFraction <= -1f)
			{
				num5 = 0f;
			}
			else
			{
				num5 += num5 * num3;
				num5 += num5 * num;
				num5 += num5 * num2;
				num5 += num5 * slopeMovementSpeedFraction;
			}
			return num5 - 1f;
		}

		// Token: 0x06002E08 RID: 11784 RVA: 0x00150B7C File Offset: 0x0014ED7C
		private float GetSlopeMovementSpeedFraction()
		{
			float num = 0f;
			if (!this.StepDetected && this.m_motor.GroundingStatus.IsStableOnGround && !this.m_motor.GroundingStatus.GroundCollider.CompareTag("Stair"))
			{
				bool flag = false;
				if (this.m_motor.BaseVelocity.y > 0f)
				{
					flag = true;
				}
				else if (base.GameEntity.AnimancerController.MovementLocomotionVector != Vector2.zero && Vector3.ProjectOnPlane(this.m_motor.CharacterForward * base.GameEntity.AnimancerController.MovementLocomotionVector.y + this.m_motor.CharacterRight * base.GameEntity.AnimancerController.MovementLocomotionVector.x, this.m_motor.GroundingStatus.GroundNormal).y > 0f)
				{
					flag = true;
				}
				if (flag)
				{
					float num2 = Vector3.Angle(this.m_motor.CharacterUp, this.m_motor.GroundingStatus.GroundNormal);
					float minGroundSlowAngle = GlobalSettings.Values.Player.MinGroundSlowAngle;
					if (num2 > minGroundSlowAngle)
					{
						float num3 = this.m_motor.MaxStableSlopeAngle - minGroundSlowAngle;
						num = (num2 - minGroundSlowAngle) / num3;
						num = Mathf.Clamp(num, GlobalSettings.Values.Player.MinGroundSlowFraciton, GlobalSettings.Values.Player.MaxGroundSlowFraction);
					}
				}
			}
			return -1f * num;
		}

		// Token: 0x06002E09 RID: 11785 RVA: 0x0005FE45 File Offset: 0x0005E045
		private void DisableCameras()
		{
			CameraSettings.DisableCameras();
			this.m_cameraDisabledFrame = new int?(Time.frameCount);
		}

		// Token: 0x06002E0A RID: 11786 RVA: 0x0005FE5C File Offset: 0x0005E05C
		public void SetPosition(Vector3 pos)
		{
			this.DisableCameras();
			this.SetPositionInternal(pos);
		}

		// Token: 0x06002E0B RID: 11787 RVA: 0x0005FE6B File Offset: 0x0005E06B
		public void SetRotation(Quaternion rot, bool resetCameraRot)
		{
			this.DisableCameras();
			this.SetRotationInternal(rot, resetCameraRot);
		}

		// Token: 0x06002E0C RID: 11788 RVA: 0x0005FE7B File Offset: 0x0005E07B
		public void SetPositionAndRotation(Vector3 pos, Quaternion rot, bool resetCameraRot)
		{
			this.DisableCameras();
			this.SetPositionRotationInternal(pos, rot, resetCameraRot);
		}

		// Token: 0x06002E0D RID: 11789 RVA: 0x0005FE8C File Offset: 0x0005E08C
		private void SetPositionInternal(Vector3 pos)
		{
			if (this.m_motor)
			{
				this.m_motor.SetPosition(pos, true);
			}
		}

		// Token: 0x06002E0E RID: 11790 RVA: 0x0005FEA8 File Offset: 0x0005E0A8
		private void SetRotationInternal(Quaternion rot, bool resetCameraRot)
		{
			if (this.m_motor)
			{
				this.m_motor.SetRotation(rot, true);
			}
			this.m_targetRotation = rot;
			if (resetCameraRot)
			{
				Action resetCameraRotation = PlayerMotorController.ResetCameraRotation;
				if (resetCameraRotation == null)
				{
					return;
				}
				resetCameraRotation();
			}
		}

		// Token: 0x06002E0F RID: 11791 RVA: 0x0005FEDD File Offset: 0x0005E0DD
		private void SetPositionRotationInternal(Vector3 pos, Quaternion rot, bool resetCameraRot)
		{
			if (this.m_motor)
			{
				this.m_motor.SetPositionAndRotation(pos, rot, true);
			}
			this.m_targetRotation = rot;
			if (resetCameraRot)
			{
				Action resetCameraRotation = PlayerMotorController.ResetCameraRotation;
				if (resetCameraRotation == null)
				{
					return;
				}
				resetCameraRotation();
			}
		}

		// Token: 0x06002E10 RID: 11792 RVA: 0x0005FF13 File Offset: 0x0005E113
		public void RequestJump()
		{
			if (this.m_isCrouching)
			{
				return;
			}
			this.m_timeSinceJumpRequested = 0f;
			this.m_jumpRequested = true;
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x0005FF30 File Offset: 0x0005E130
		public void ToggleCrouch(bool crouch)
		{
			this.m_shouldBeCrouching = crouch;
			if (!this.m_isCrouching && crouch)
			{
				this.m_isCrouching = true;
				this.m_motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
			}
		}

		// Token: 0x06002E12 RID: 11794 RVA: 0x00150CF8 File Offset: 0x0014EEF8
		public void OutOfBounds()
		{
			SpawnVolumeOverride spawnVolumeOverride;
			PlayerSpawn playerSpawn2;
			PlayerSpawn playerSpawn = (LocalZoneManager.TryGetSpawnVolumeOverride(base.GameEntity.gameObject.transform.position, out spawnVolumeOverride) && spawnVolumeOverride.IsActive(base.GameEntity, out playerSpawn2)) ? playerSpawn2 : LocalZoneManager.GetDefaultPlayerSpawn(base.GameEntity);
			this.m_motor.SetPosition(playerSpawn.GetPosition(), true);
			this.m_motor.SetRotation(playerSpawn.GetRotation(), true);
		}

		// Token: 0x06002E13 RID: 11795 RVA: 0x00150D68 File Offset: 0x0014EF68
		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			if (base.GameEntity.Vitals.GetCurrentStance() == Stance.Sit || this.SpeedMod <= -1f)
			{
				return;
			}
			if (base.GameEntity.SkillsController.PendingIsActive && base.GameEntity.SkillsController.Pending.ExecutionParams != null && base.GameEntity.SkillsController.Pending.ExecutionParams.PreventRotation)
			{
				return;
			}
			if (ClientGameManager.InputManager != null && !ClientGameManager.InputManager.PreventCharacterRotation && ClientGameManager.MainCamera && CameraManager.ActiveType != ActiveCameraTypes.Death)
			{
				if (LocalPlayer.FollowTarget)
				{
					float y = Quaternion.LookRotation(LocalPlayer.FollowTarget.gameObject.transform.position - base.gameObject.transform.position, Vector3.up).eulerAngles.y;
					this.m_targetRotation = Quaternion.Euler(new Vector3(0f, y, 0f));
				}
				else if (CameraManager.ActiveType.MatchAvatarRotationToCamera())
				{
					this.SetTargetRotationToCameraRotation();
				}
				else if (ClientGameManager.InputManager.IsTurning)
				{
					if (ClientGameManager.InputManager.HoldingLMB)
					{
						Vector2 xydelta = CameraManager.GetXYDelta();
						this.AddDeltaTargetRotation(xydelta.x);
					}
					else
					{
						this.SetTargetRotationToCameraRotation();
					}
				}
				else if (ClientGameManager.InputManager.HoldingRMB)
				{
					int frameCount = Time.frameCount;
					if (ClientGameManager.InputManager.HoldingRMB != this.m_wasHoldingRMB)
					{
						if (Mathf.Abs(frameCount - InteractionManager.LastMouseInteractionFrame) < 3)
						{
							this.m_delayRotationUntil = frameCount + 2;
						}
					}
					else if (frameCount >= this.m_delayRotationUntil)
					{
						this.SetTargetRotationToCameraRotation();
					}
				}
				this.m_wasHoldingRMB = ClientGameManager.InputManager.HoldingRMB;
			}
			this.AngleDelta = this.m_targetRotation.eulerAngles.y - this.m_motor.TransientRotation.eulerAngles.y;
			currentRotation = Quaternion.LerpUnclamped(this.m_motor.TransientRotation, this.m_targetRotation, this.m_turnSpeed * deltaTime);
		}

		// Token: 0x06002E14 RID: 11796 RVA: 0x0005FF67 File Offset: 0x0005E167
		private void SetTargetRotationToCameraRotation()
		{
			this.m_targetRotation = Quaternion.Euler(new Vector3(0f, ClientGameManager.MainCamera.transform.eulerAngles.y, 0f));
		}

		// Token: 0x06002E15 RID: 11797 RVA: 0x00150F78 File Offset: 0x0014F178
		private void AddDeltaTargetRotation(float delta)
		{
			Vector3 eulerAngles = this.m_targetRotation.eulerAngles;
			eulerAngles.y += delta;
			this.m_targetRotation = Quaternion.Euler(eulerAngles);
		}

		// Token: 0x06002E16 RID: 11798 RVA: 0x00150FAC File Offset: 0x0014F1AC
		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			if (this.m_motor.GroundingStatus.IsStableOnGround)
			{
				bool flag = base.GameEntity.Vitals.Stance == Stance.Crouch;
				bool flag2 = !flag && ClientGameManager.InputManager.IsWalking;
				bool flag3 = !flag && !flag2 && (GlobalSettings.Values.Player.AlwaysRun || ClientGameManager.InputManager.HoldingShift);
				float num = this.GetSpeedModifier();
				Vector2 vector = ClientGameManager.InputManager.MovementInput;
				if (vector.y < 0f)
				{
					vector *= 0.9f;
					num -= 0.1f;
				}
				num = Mathf.Clamp(num, -1f, float.MaxValue);
				this.SpeedMod = 1f;
				this.m_locomotion = Vector2.MoveTowards(this.m_locomotion, vector, GlobalSettings.Values.Animation.MovementLerpRate * Time.deltaTime);
				Vector3 vector2 = Vector3.ClampMagnitude(new Vector3(this.m_locomotion.x, 0f, this.m_locomotion.y), 1f);
				float num2 = 0f;
				if (flag)
				{
					num2 = 2f;
				}
				else if (flag2)
				{
					num2 = 1.56595f;
				}
				else if (flag3)
				{
					num2 = 5f;
				}
				num2 = num2.PercentModification(num);
				this.m_targetLocomotion = new Vector2(vector2.x, vector2.z) * num2;
				Vector3 normalized = Vector3.ProjectOnPlane(LocalPlayer.GameEntity.transform.rotation * Vector3.forward, this.Motor.CharacterUp).normalized;
				if (normalized.sqrMagnitude == 0f)
				{
					normalized = Vector3.ProjectOnPlane(LocalPlayer.GameEntity.transform.rotation * Vector3.up, this.Motor.CharacterUp).normalized;
				}
				vector2 = Quaternion.LookRotation(normalized, this.Motor.CharacterUp) * vector2;
				float magnitude = currentVelocity.magnitude;
				Vector3 vector3 = this.Motor.GroundingStatus.GroundNormal;
				if (magnitude > 0f && this.Motor.GroundingStatus.SnappingPrevented)
				{
					Vector3 rhs = this.Motor.TransientPosition - this.Motor.GroundingStatus.GroundPoint;
					if (Vector3.Dot(currentVelocity, rhs) >= 0f)
					{
						vector3 = this.Motor.GroundingStatus.OuterGroundNormal;
					}
					else
					{
						vector3 = this.Motor.GroundingStatus.InnerGroundNormal;
					}
				}
				currentVelocity = this.Motor.GetDirectionTangentToSurface(currentVelocity, vector3) * magnitude;
				Vector3 rhs2 = Vector3.Cross(vector2, this.Motor.CharacterUp);
				Vector3 b = Vector3.Cross(vector3, rhs2).normalized * vector2.magnitude * num2;
				currentVelocity = Vector3.Lerp(currentVelocity, b, 1f - Mathf.Exp(-this.StableMovementSharpness * deltaTime));
			}
			else
			{
				this.SpeedMod = 0f;
				if (this.m_allowAirMovement)
				{
					if (ClientGameManager.InputManager.MovementInputSqrMagnitude > 0f)
					{
						Vector3 vector4 = new Vector3(ClientGameManager.InputManager.MovementInput.x, 0f, ClientGameManager.InputManager.MovementInput.y);
						Vector3 vector5 = vector4 * this.m_airAccelerationSpeed * deltaTime;
						if (this.Motor.GroundingStatus.FoundAnyGround)
						{
							Vector3 normalized2 = Vector3.Cross(Vector3.Cross(this.Motor.CharacterUp, this.Motor.GroundingStatus.GroundNormal), this.Motor.CharacterUp).normalized;
							vector5 = Vector3.ProjectOnPlane(vector5, normalized2);
						}
						Vector3 vector6 = Vector3.ProjectOnPlane(currentVelocity + vector5, this.Motor.CharacterUp);
						if (vector6.magnitude > this.m_maxAirMoveSpeed && Vector3.Dot(vector4, vector6) >= 0f)
						{
							vector5 = Vector3.zero;
						}
						else
						{
							Vector3 b2 = Vector3.ProjectOnPlane(currentVelocity, this.Motor.CharacterUp);
							vector5 = Vector3.ClampMagnitude(vector6, this.m_maxAirMoveSpeed) - b2;
						}
						currentVelocity += vector5;
					}
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

		// Token: 0x06002E17 RID: 11799 RVA: 0x0004475B File Offset: 0x0004295B
		public void BeforeCharacterUpdate(float deltaTime)
		{
		}

		// Token: 0x06002E18 RID: 11800 RVA: 0x0015158C File Offset: 0x0014F78C
		public void PostGroundingUpdate(float deltaTime)
		{
			if (!this.m_motor.GroundingStatus.IsStableOnGround || this.m_motor.LastGroundingStatus.IsStableOnGround)
			{
				if (!this.m_motor.GroundingStatus.IsStableOnGround && this.m_motor.LastGroundingStatus.IsStableOnGround)
				{
					Action<bool> tookOff = this.TookOff;
					if (tookOff == null)
					{
						return;
					}
					tookOff(this.m_jumpedThisFrame);
				}
				return;
			}
			Action landed = this.Landed;
			if (landed == null)
			{
				return;
			}
			landed();
		}

		// Token: 0x06002E19 RID: 11801 RVA: 0x00151608 File Offset: 0x0014F808
		public void AfterCharacterUpdate(float deltaTime)
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
			}
			else
			{
				this.m_timeSinceLastAbleToJump += deltaTime;
			}
			if (this.m_isCrouching && !this.m_shouldBeCrouching)
			{
				this.m_motor.SetCapsuleDimensions(0.5f, 2f, 1f);
				if (this.m_motor.CharacterOverlap(this.m_motor.TransientPosition, this.m_motor.TransientRotation, this.m_probedColliders, this.m_motor.CollidableLayers, QueryTriggerInteraction.Ignore, 0f) > 0)
				{
					this.m_motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
					return;
				}
				this.m_isCrouching = false;
			}
		}

		// Token: 0x06002E1A RID: 11802 RVA: 0x0005FF97 File Offset: 0x0005E197
		public bool IsColliderValidForCollisions(Collider coll)
		{
			return this.m_layerMask.Contains(coll.gameObject.layer) && (this.m_ignroedColliders.Count == 0 || !this.m_ignroedColliders.Contains(coll));
		}

		// Token: 0x06002E1B RID: 11803 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06002E1C RID: 11804 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		// Token: 0x06002E1D RID: 11805 RVA: 0x0005FFD3 File Offset: 0x0005E1D3
		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
			this.m_stepDetected = hitStabilityReport.ValidStepDetected;
		}

		// Token: 0x06002E1E RID: 11806 RVA: 0x0004475B File Offset: 0x0004295B
		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x06002E1F RID: 11807 RVA: 0x0005FFE2 File Offset: 0x0005E1E2
		// (set) Token: 0x06002E20 RID: 11808 RVA: 0x0005FFEA File Offset: 0x0005E1EA
		float IMotor.SpeedModifier
		{
			get
			{
				return this.m_vitalsSpeedMod;
			}
			set
			{
				this.m_vitalsSpeedMod = value;
			}
		}

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x06002E21 RID: 11809 RVA: 0x00049FFA File Offset: 0x000481FA
		NavMeshAgent IMotor.UnityNavAgent
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002E22 RID: 11810 RVA: 0x0005FFF3 File Offset: 0x0005E1F3
		void IMotor.ApplyRootEffect(bool adding, CombatEffect effect)
		{
			if (effect == null)
			{
				return;
			}
			if (adding)
			{
				this.m_rootEffects.Add(effect);
				return;
			}
			this.m_rootEffects.Remove(effect);
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x06002E23 RID: 11811 RVA: 0x00060017 File Offset: 0x0005E217
		bool IMotor.IsRooted
		{
			get
			{
				return this.m_rootEffects.Count > 0;
			}
		}

		// Token: 0x04002D56 RID: 11606
		public const float kCrouchSpeed = 2f;

		// Token: 0x04002D57 RID: 11607
		public const float kWalkSpeed = 1.56595f;

		// Token: 0x04002D58 RID: 11608
		public const float kRunSpeed = 5f;

		// Token: 0x04002D59 RID: 11609
		public const float kWalkRunSpeedRatio = 0.31319f;

		// Token: 0x04002D5A RID: 11610
		private const float kAutoFollowDistance = 4f;

		// Token: 0x04002D5B RID: 11611
		public const float kAutoFollowDistanceSqr = 16f;

		// Token: 0x04002D5C RID: 11612
		public const float kAutoFollowDropDistance = 12f;

		// Token: 0x04002D5D RID: 11613
		public const float kAutoFollowDropDistanceSqr = 144f;

		// Token: 0x04002D61 RID: 11617
		[SerializeField]
		private KinematicCharacterMotor m_motor;

		// Token: 0x04002D62 RID: 11618
		private Vector3 m_rootMotionDelta;

		// Token: 0x04002D63 RID: 11619
		private Quaternion m_targetRotation;

		// Token: 0x04002D64 RID: 11620
		private Armstrong m_armstrong;

		// Token: 0x04002D65 RID: 11621
		[SerializeField]
		private float m_turnSpeed = 10f;

		// Token: 0x04002D66 RID: 11622
		private List<Collider> m_ignroedColliders = new List<Collider>();

		// Token: 0x04002D68 RID: 11624
		[SerializeField]
		private float m_gravityMultiplier = 1f;

		// Token: 0x04002D69 RID: 11625
		[SerializeField]
		private float m_jumpSpeed = 10f;

		// Token: 0x04002D6A RID: 11626
		[SerializeField]
		private float m_jumpPreGroundingGraceTime;

		// Token: 0x04002D6B RID: 11627
		[SerializeField]
		private float m_jumpPostGroundingGraceTime;

		// Token: 0x04002D6C RID: 11628
		[SerializeField]
		private bool m_allowSlideJump;

		// Token: 0x04002D6D RID: 11629
		[SerializeField]
		private bool m_allowAirMovement;

		// Token: 0x04002D6E RID: 11630
		[SerializeField]
		private float m_airAccelerationSpeed = 5f;

		// Token: 0x04002D6F RID: 11631
		[SerializeField]
		private float m_airDrag = 0.1f;

		// Token: 0x04002D70 RID: 11632
		[SerializeField]
		private float m_maxAirMoveSpeed = 20f;

		// Token: 0x04002D71 RID: 11633
		[SerializeField]
		private LayerMask m_layerMask;

		// Token: 0x04002D72 RID: 11634
		public float StableMovementSharpness = 15f;

		// Token: 0x04002D73 RID: 11635
		private readonly HashSet<CombatEffect> m_rootEffects = new HashSet<CombatEffect>(10);

		// Token: 0x04002D74 RID: 11636
		private readonly List<Vector3> m_positionBreadcrumbs = new List<Vector3>();

		// Token: 0x04002D75 RID: 11637
		private float m_nextBreadcrumbUpdateTime;

		// Token: 0x04002D76 RID: 11638
		private const int kMaxBreadcrumbs = 10;

		// Token: 0x04002D77 RID: 11639
		private const float kBreadcrumbUpdateTime = 10f;

		// Token: 0x04002D78 RID: 11640
		private const float kBreadcrumbDistanceThreshold = 16f;

		// Token: 0x04002D79 RID: 11641
		private Vector3 m_lastBreadcrumbAtRequest;

		// Token: 0x04002D7A RID: 11642
		private DateTime? m_stuckRequestTime;

		// Token: 0x04002D7B RID: 11643
		private DateTime? m_ropeRequestTime;

		// Token: 0x04002D7C RID: 11644
		private DateTime m_lastStuckRopeUsed = DateTime.MinValue;

		// Token: 0x04002D7D RID: 11645
		private const float kStuckRopeExecuteTime = 10f;

		// Token: 0x04002D7E RID: 11646
		private const float kMinStuckRopeExecuteDelta = 60f;

		// Token: 0x04002D7F RID: 11647
		private float m_speedMod;

		// Token: 0x04002D80 RID: 11648
		private Vector2 m_targetLocomotion = Vector2.zero;

		// Token: 0x04002D81 RID: 11649
		private Vector2 m_locomotion = Vector2.zero;

		// Token: 0x04002D82 RID: 11650
		private bool m_jumpRequested;

		// Token: 0x04002D83 RID: 11651
		private bool m_jumpConsumed;

		// Token: 0x04002D84 RID: 11652
		private bool m_jumpedThisFrame;

		// Token: 0x04002D85 RID: 11653
		private float m_timeSinceJumpRequested = float.PositiveInfinity;

		// Token: 0x04002D86 RID: 11654
		private float m_timeSinceLastAbleToJump;

		// Token: 0x04002D87 RID: 11655
		private float m_verticalVelocity;

		// Token: 0x04002D88 RID: 11656
		private float m_vitalsSpeedMod;

		// Token: 0x04002D89 RID: 11657
		private Collider[] m_probedColliders = new Collider[8];

		// Token: 0x04002D8A RID: 11658
		private bool m_isCrouching;

		// Token: 0x04002D8B RID: 11659
		private bool m_shouldBeCrouching;

		// Token: 0x04002D8C RID: 11660
		private bool m_stepDetected;

		// Token: 0x04002D8D RID: 11661
		private float? m_startingFallHeight;

		// Token: 0x04002D8E RID: 11662
		public static float kMaxRopeDistance = 4f;

		// Token: 0x04002D8F RID: 11663
		private static float kMaxRopeDistanceSqr = PlayerMotorController.kMaxRopeDistance * PlayerMotorController.kMaxRopeDistance;

		// Token: 0x04002D90 RID: 11664
		private int? m_cameraDisabledFrame;

		// Token: 0x04002D91 RID: 11665
		private bool m_wasHoldingRMB;

		// Token: 0x04002D92 RID: 11666
		private int m_delayRotationUntil;
	}
}
