using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Messages;
using SoL.Game.Quests;
using SoL.Game.UI;
using SoL.Networking.SolServer;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Managers
{
	// Token: 0x020004F7 RID: 1271
	public class InputManager : MonoBehaviour, IInputManager
	{
		// Token: 0x14000045 RID: 69
		// (add) Token: 0x060023CB RID: 9163 RVA: 0x0012B5E4 File Offset: 0x001297E4
		// (remove) Token: 0x060023CC RID: 9164 RVA: 0x0012B618 File Offset: 0x00129818
		public static event Action<bool> MoveUIHeldChanged;

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x060023CD RID: 9165 RVA: 0x0012B64C File Offset: 0x0012984C
		// (remove) Token: 0x060023CE RID: 9166 RVA: 0x0012B680 File Offset: 0x00129880
		public static event Action<int> MacroActionPressed;

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x060023CF RID: 9167 RVA: 0x00059AAD File Offset: 0x00057CAD
		// (set) Token: 0x060023D0 RID: 9168 RVA: 0x00059AB5 File Offset: 0x00057CB5
		public bool HoldingLMBRaw { get; private set; }

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x060023D1 RID: 9169 RVA: 0x00059ABE File Offset: 0x00057CBE
		// (set) Token: 0x060023D2 RID: 9170 RVA: 0x00059AC6 File Offset: 0x00057CC6
		public bool HoldingRMBRaw { get; private set; }

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x060023D3 RID: 9171 RVA: 0x00059ACF File Offset: 0x00057CCF
		// (set) Token: 0x060023D4 RID: 9172 RVA: 0x00059AD7 File Offset: 0x00057CD7
		public bool HoldingLMB { get; private set; }

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x060023D5 RID: 9173 RVA: 0x00059AE0 File Offset: 0x00057CE0
		// (set) Token: 0x060023D6 RID: 9174 RVA: 0x00059AE8 File Offset: 0x00057CE8
		public bool HoldingRMB { get; private set; }

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x060023D7 RID: 9175 RVA: 0x00059AF1 File Offset: 0x00057CF1
		// (set) Token: 0x060023D8 RID: 9176 RVA: 0x00059AF9 File Offset: 0x00057CF9
		public bool HoldingShift { get; private set; }

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x060023D9 RID: 9177 RVA: 0x00059B02 File Offset: 0x00057D02
		// (set) Token: 0x060023DA RID: 9178 RVA: 0x00059B0A File Offset: 0x00057D0A
		public bool HoldingCtrl { get; private set; }

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x060023DB RID: 9179 RVA: 0x00059B13 File Offset: 0x00057D13
		// (set) Token: 0x060023DC RID: 9180 RVA: 0x00059B1B File Offset: 0x00057D1B
		public bool HoldingAlt { get; private set; }

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x060023DD RID: 9181 RVA: 0x00059B24 File Offset: 0x00057D24
		// (set) Token: 0x060023DE RID: 9182 RVA: 0x00059B2C File Offset: 0x00057D2C
		public Vector2 MovementInput { get; private set; }

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x060023DF RID: 9183 RVA: 0x00059B35 File Offset: 0x00057D35
		// (set) Token: 0x060023E0 RID: 9184 RVA: 0x00059B3D File Offset: 0x00057D3D
		public Vector2 LookInput { get; private set; }

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x060023E1 RID: 9185 RVA: 0x00059B46 File Offset: 0x00057D46
		// (set) Token: 0x060023E2 RID: 9186 RVA: 0x00059B4E File Offset: 0x00057D4E
		public bool IsTurning { get; private set; }

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x060023E3 RID: 9187 RVA: 0x00059B57 File Offset: 0x00057D57
		// (set) Token: 0x060023E4 RID: 9188 RVA: 0x00059B5F File Offset: 0x00057D5F
		public bool IsWalking { get; private set; }

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x060023E5 RID: 9189 RVA: 0x00059B68 File Offset: 0x00057D68
		// (set) Token: 0x060023E6 RID: 9190 RVA: 0x00059B70 File Offset: 0x00057D70
		public bool IsCrouching { get; private set; }

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x060023E7 RID: 9191 RVA: 0x00059B79 File Offset: 0x00057D79
		// (set) Token: 0x060023E8 RID: 9192 RVA: 0x00059B81 File Offset: 0x00057D81
		public bool DisableControllerTurning { get; private set; }

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x060023E9 RID: 9193 RVA: 0x00059B8A File Offset: 0x00057D8A
		// (set) Token: 0x060023EA RID: 9194 RVA: 0x00059B92 File Offset: 0x00057D92
		public float MovementInputSqrMagnitude { get; private set; }

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x060023EB RID: 9195 RVA: 0x00059B9B File Offset: 0x00057D9B
		// (set) Token: 0x060023EC RID: 9196 RVA: 0x00059BA3 File Offset: 0x00057DA3
		public Vector2 NormalizedMovementInput { get; private set; }

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x060023ED RID: 9197 RVA: 0x00059BAC File Offset: 0x00057DAC
		// (set) Token: 0x060023EE RID: 9198 RVA: 0x00059BB4 File Offset: 0x00057DB4
		public float NormalizedMovementInputMagnitude { get; private set; }

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x060023EF RID: 9199 RVA: 0x00059BBD File Offset: 0x00057DBD
		// (set) Token: 0x060023F0 RID: 9200 RVA: 0x00059BC5 File Offset: 0x00057DC5
		public bool EnterDown { get; private set; }

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x060023F1 RID: 9201 RVA: 0x00059BCE File Offset: 0x00057DCE
		// (set) Token: 0x060023F2 RID: 9202 RVA: 0x00059BD6 File Offset: 0x00057DD6
		public bool SpaceDown { get; private set; }

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x060023F3 RID: 9203 RVA: 0x00059BDF File Offset: 0x00057DDF
		// (set) Token: 0x060023F4 RID: 9204 RVA: 0x00059BE7 File Offset: 0x00057DE7
		public bool TabDown { get; private set; }

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x060023F5 RID: 9205 RVA: 0x00059BF0 File Offset: 0x00057DF0
		public bool PreventInput
		{
			get
			{
				return this.m_inputPreventionFlags > InputPreventionFlags.None;
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x060023F6 RID: 9206 RVA: 0x00059BFB File Offset: 0x00057DFB
		public bool PreventCharacterMovement
		{
			get
			{
				return this.PreventInput || this.m_isStunned;
			}
		}

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x060023F7 RID: 9207 RVA: 0x00059C0D File Offset: 0x00057E0D
		public bool PreventCharacterRotation
		{
			get
			{
				return this.PreventCharacterMovement || LocalPlayer.LootInteractive != null;
			}
		}

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x060023F8 RID: 9208 RVA: 0x00059C21 File Offset: 0x00057E21
		public bool PreventInputForUI
		{
			get
			{
				return this.m_inputPreventionFlags.PreventForUI();
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x060023F9 RID: 9209 RVA: 0x00059C2E File Offset: 0x00057E2E
		public bool PreventInputForLook
		{
			get
			{
				return this.m_inputPreventionFlags.PreventForLook();
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x060023FA RID: 9210 RVA: 0x00059C3B File Offset: 0x00057E3B
		public InputPreventionFlags InputPreventionFlags
		{
			get
			{
				return this.m_inputPreventionFlags;
			}
		}

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x060023FB RID: 9211 RVA: 0x00059C43 File Offset: 0x00057E43
		// (set) Token: 0x060023FC RID: 9212 RVA: 0x00059C4B File Offset: 0x00057E4B
		private bool MoveUiHeld
		{
			get
			{
				return this.m_moveUiHeld;
			}
			set
			{
				if (this.m_moveUiHeld == value)
				{
					return;
				}
				this.m_moveUiHeld = value;
				Action<bool> moveUIHeldChanged = InputManager.MoveUIHeldChanged;
				if (moveUIHeldChanged == null)
				{
					return;
				}
				moveUIHeldChanged(this.m_moveUiHeld);
			}
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x0012B6B4 File Offset: 0x001298B4
		private void Awake()
		{
			SceneCompositionManager.ZoneLoadStarted += this.SceneCompositionManagerOnZoneLoadStarted;
			InteractionManager.InteractionSuccess += this.InteractionManagerOnInteractionSuccess;
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.FollowTargetChanged += this.LocalPlayerOnFollowTargetChanged;
			this.m_movementInputValues = new InputManager.InputValues();
			this.m_lookInputValues = new InputManager.InputValues();
		}

		// Token: 0x060023FE RID: 9214 RVA: 0x0004475B File Offset: 0x0004295B
		private void Start()
		{
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x0012B71C File Offset: 0x0012991C
		private void OnDestroy()
		{
			SceneCompositionManager.ZoneLoadStarted -= this.SceneCompositionManagerOnZoneLoadStarted;
			InteractionManager.InteractionSuccess -= this.InteractionManagerOnInteractionSuccess;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.FollowTargetChanged -= this.LocalPlayerOnFollowTargetChanged;
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x0012B770 File Offset: 0x00129970
		private void Update()
		{
			bool flag = EventSystem.current.IsPointerOverGameObject();
			this.HoldingLMBRaw = Input.GetMouseButton(0);
			this.HoldingRMBRaw = Input.GetMouseButton(1);
			this.HoldingLMB = (flag ? (this.HoldingLMB && this.HoldingLMBRaw) : this.HoldingLMBRaw);
			this.HoldingRMB = (flag ? (this.HoldingRMB && this.HoldingRMBRaw) : this.HoldingRMBRaw);
			this.HoldingShift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			this.HoldingCtrl = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
			this.HoldingAlt = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
			this.EnterDown = (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
			this.SpaceDown = Input.GetKeyDown(KeyCode.Space);
			this.TabDown = Input.GetKeyDown(KeyCode.Tab);
			this.m_isTurningLeft = SolInput.GetButton(51);
			this.m_isTurningRight = SolInput.GetButton(52);
			this.MoveUiHeld = (!this.PreventInputForUI && SolInput.GetButton(115));
			LocalPlayer.ValidateFollowTarget();
			Vector2 movement = this.GetMovement();
			this.MovementInput = movement;
			this.MovementInputSqrMagnitude = movement.sqrMagnitude;
			this.NormalizedMovementInput = ((this.MovementInputSqrMagnitude > 1f) ? movement.normalized : movement);
			this.NormalizedMovementInputMagnitude = this.NormalizedMovementInput.magnitude;
			this.LookInput = this.GetLookInput();
			this.IsTurning = false;
			this.m_isStunned = LocalPlayer.IsStunned;
			this.HandlePlayerInput();
			bool keyDown = Input.GetKeyDown(KeyCode.Escape);
			if (keyDown)
			{
				this.EscapePressed();
			}
			if (LocalPlayer.LootInteractive != null && this.MovementInput != Vector2.zero)
			{
				LocalPlayer.LootInteractive.EndInteraction(LocalPlayer.GameEntity, true);
				LocalPlayer.LootInteractive = null;
			}
			if (this.m_autoRunEnabled && (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.VitalsReplicator || LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive))
			{
				this.m_autoRunEnabled = false;
			}
			if (CursorManager.GameMode != CursorGameMode.None && Input.GetMouseButtonDown(1))
			{
				CursorManager.ResetGameMode();
			}
			bool flag2 = this.HandleChatWindow();
			if (keyDown || flag2 || this.EnterDown || this.SpaceDown || this.TabDown)
			{
				LocalPlayer.UpdateTimeOfLastInput();
			}
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x0012B9DC File Offset: 0x00129BDC
		private bool HandleChatWindow()
		{
			if (this.InputPreventionFlags.PreventForUI() || LocalPlayer.GameEntity == null)
			{
				return false;
			}
			bool result = false;
			if (LocalPlayer.GameEntity.TargetController)
			{
				if (SolInput.GetButtonDown(44))
				{
					this.HailTarget(false);
					result = true;
				}
				else if (SolInput.GetButtonDown(116))
				{
					this.HailTarget(true);
					result = true;
				}
			}
			if (SolInput.GetButtonDown(110))
			{
				UIManager.ActiveChatInput.EnterPressed();
				result = true;
			}
			if (SolInput.GetButtonDown(111))
			{
				UIManager.ActiveChatInput.SlashPressed();
				result = true;
			}
			if (SolInput.GetButtonDown(43))
			{
				UIManager.ActiveChatInput.TellPressed();
				result = true;
			}
			return result;
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x00059C73 File Offset: 0x00057E73
		private bool IsValidHailTarget(GameEntity entity)
		{
			return entity && entity != LocalPlayer.GameEntity && entity.Vitals && entity.Vitals.GetCurrentHealthState() != HealthState.Dead;
		}

		// Token: 0x06002403 RID: 9219 RVA: 0x0012BA80 File Offset: 0x00129C80
		private void HailTarget(bool invertPriority)
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.TargetController == null)
			{
				return;
			}
			GameEntity gameEntity = null;
			GameEntity gameEntity2 = LocalPlayer.GameEntity.TargetController.DefensiveTarget;
			GameEntity gameEntity3 = LocalPlayer.GameEntity.TargetController.OffensiveTarget;
			if (Options.GameOptions.HailOffensiveTargetFirst)
			{
				gameEntity2 = LocalPlayer.GameEntity.TargetController.OffensiveTarget;
				gameEntity3 = LocalPlayer.GameEntity.TargetController.DefensiveTarget;
			}
			if (invertPriority)
			{
				GameEntity gameEntity4 = gameEntity2;
				gameEntity2 = gameEntity3;
				gameEntity3 = gameEntity4;
			}
			if (this.IsValidHailTarget(gameEntity2))
			{
				gameEntity = gameEntity2;
			}
			else if (this.IsValidHailTarget(gameEntity3))
			{
				gameEntity = gameEntity3;
			}
			else
			{
				Collider[] colliders = Hits.Colliders100;
				int num = Physics.OverlapSphereNonAlloc(LocalPlayer.GameEntity.gameObject.transform.position, 20f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
				float num2 = float.MaxValue;
				GameEntity gameEntity5 = null;
				float num3 = float.MaxValue;
				GameEntity gameEntity6 = null;
				for (int i = 0; i < num; i++)
				{
					GameEntity gameEntity7;
					if (colliders[i] && DetectionCollider.TryGetEntityForCollider(colliders[i], out gameEntity7) && gameEntity7 && gameEntity7.Type == GameEntityType.Player && this.IsValidHailTarget(gameEntity7))
					{
						if (LocalPlayer.GameEntity.gameObject.transform.InverseTransformPoint(gameEntity7.gameObject.transform.position).z > 0f)
						{
							float cachedSqrDistanceFromLocalPlayer = gameEntity7.GetCachedSqrDistanceFromLocalPlayer();
							if (cachedSqrDistanceFromLocalPlayer < num2)
							{
								num2 = cachedSqrDistanceFromLocalPlayer;
								gameEntity5 = gameEntity7;
							}
						}
						else if (ClientGameManager.MainCamera.gameObject.transform.InverseTransformPoint(gameEntity7.gameObject.transform.position).z > 0f)
						{
							float cachedSqrDistanceFromLocalPlayer2 = gameEntity7.GetCachedSqrDistanceFromLocalPlayer();
							if (cachedSqrDistanceFromLocalPlayer2 < num3)
							{
								num3 = cachedSqrDistanceFromLocalPlayer2;
								gameEntity6 = gameEntity7;
							}
						}
					}
				}
				if (gameEntity5)
				{
					gameEntity = gameEntity5;
				}
				else if (gameEntity6)
				{
					gameEntity = gameEntity6;
				}
			}
			string value = "Hail";
			if (gameEntity && gameEntity.CharacterData)
			{
				if (gameEntity.Type == GameEntityType.Npc)
				{
					IDialogueNpc component = gameEntity.gameObject.GetComponent<IDialogueNpc>();
					if (component != null && component.CanConverseWith(LocalPlayer.GameEntity))
					{
						component.InitiateDialogue();
					}
				}
				string text = gameEntity.CharacterData.Title.Value;
				string value2 = gameEntity.CharacterData.Name.Value;
				bool flag = !string.IsNullOrEmpty(text);
				if (flag && text.Contains("</color>"))
				{
					string[] array = text.Split("</color>", StringSplitOptions.None);
					if (array.Length > 1)
					{
						string[] array2 = array[0].Split(">", StringSplitOptions.None);
						if (array2.Length > 1)
						{
							text = array2[1];
						}
					}
				}
				value = (flag ? ZString.Format<string, string>("Hail {0} {1}", text, value2) : ZString.Format<string>("Hail {0}", value2));
			}
			new SolServerCommand(CommandClass.chat, MessageType.Say.GetCommandType())
			{
				Args = 
				{
					{
						"Message",
						value
					}
				}
			}.Send();
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x0012BD84 File Offset: 0x00129F84
		private void HandlePlayerInput()
		{
			if (this.PreventInput || !LocalPlayer.GameEntity)
			{
				return;
			}
			if (SolInput.GetButtonDown(68))
			{
				this.IsWalking = !this.IsWalking;
			}
			if (SolInput.GetButtonDown(72))
			{
				this.DisableControllerTurning = !this.DisableControllerTurning;
			}
			if (SolInput.GetButtonDown(40))
			{
				Options.GameOptions.ShowOverheadNameplates.Value = !Options.GameOptions.ShowOverheadNameplates.Value;
				if (MessageManager.ChatQueue != null)
				{
					string arg = Options.GameOptions.ShowOverheadNameplates.Value ? "SHOWN" : "HIDDEN";
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string>("Overhead nameplates {0}", arg));
				}
			}
			if (Options.GameOptions.HoldKeybindToShowCompass.Value)
			{
				bool button = SolInput.GetButton(122);
				if (Options.GameOptions.ShowUiCompass.Value != button)
				{
					Options.GameOptions.ShowUiCompass.Value = button;
				}
			}
			else if (SolInput.GetButtonDown(122))
			{
				Options.GameOptions.ShowUiCompass.Value = !Options.GameOptions.ShowUiCompass.Value;
			}
			if (SolInput.GetButtonDown(21))
			{
				this.m_autoRunEnabled = !this.m_autoRunEnabled;
				if (this.m_autoRunEnabled)
				{
					LocalPlayer.ClearFollowTarget();
				}
			}
			if (SolInput.GetButtonDown(104) && LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.ConsiderTarget();
			}
			if (SolInput.GetButtonDown(109))
			{
				InteractionManager.LootNearbyViaKeybind();
			}
			if (SolInput.GetButtonDown(113))
			{
				CorpseManager.DragSelfOrClosestGroupCorpse();
			}
			if (this.m_isStunned)
			{
				this.IsCrouching = false;
				return;
			}
			this.IsTurning = ((!this.HoldingRMB && (this.m_isTurningLeft || this.m_isTurningRight)) || (!this.DisableControllerTurning && SolInput.GetAxis(48) != 0f));
			if (!LocalPlayer.GameEntity.SkillsController.PendingIsActive && SolInput.GetButtonDown(13))
			{
				bool secondaryWeaponsActive = !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
				LocalPlayer.GameEntity.CharacterData.SetSecondaryWeaponsActive(secondaryWeaponsActive);
			}
			if (LocalPlayer.Animancer && LocalPlayer.Animancer.Stance.PlayerCanExit())
			{
				if (SolInput.GetButtonDown(11))
				{
					LocalPlayer.Animancer.ToggleSit();
				}
				else if (SolInput.GetButtonDown(10))
				{
					LocalPlayer.Animancer.ToggleCombat();
				}
				else if (SolInput.GetButtonDown(12))
				{
					LocalPlayer.Animancer.ToggleLight();
				}
			}
			if (Options.GameOptions.CrouchIsToggle.Value)
			{
				if (SolInput.GetButtonDown(18))
				{
					this.IsCrouching = !this.IsCrouching;
				}
			}
			else
			{
				this.IsCrouching = SolInput.GetButton(18);
			}
			this.HandleActionBar();
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x0012C010 File Offset: 0x0012A210
		private void HandleActionBar()
		{
			if (!ClientGameManager.UIManager || !ClientGameManager.UIManager.ActionBar)
			{
				return;
			}
			for (int i = 0; i < InputManager.kAbilityActions.Length; i++)
			{
				if (SolInput.GetButtonDown(InputManager.kAbilityActions[i]))
				{
					ClientGameManager.UIManager.ActionBar.TriggerActionBarIndex(i);
					return;
				}
			}
			for (int j = 0; j < InputManager.kConsumableActions.Length; j++)
			{
				if (SolInput.GetButtonDown(InputManager.kConsumableActions[j]))
				{
					ClientGameManager.UIManager.ActionBar.TriggerConsumableIndex(j);
					return;
				}
			}
			for (int k = 0; k < InputManager.kReagentActions.Length; k++)
			{
				if (SolInput.GetButtonDown(InputManager.kReagentActions[k]))
				{
					ClientGameManager.UIManager.ActionBar.TriggerReagentIndex(k);
					return;
				}
			}
			int l = 0;
			while (l < InputManager.kMacroActions.Length)
			{
				if (SolInput.GetButtonDown(InputManager.kMacroActions[l]))
				{
					Action<int> macroActionPressed = InputManager.MacroActionPressed;
					if (macroActionPressed == null)
					{
						return;
					}
					macroActionPressed(l);
					return;
				}
				else
				{
					l++;
				}
			}
			if (ClientGameManager.UIManager.AlchemySelectionUI)
			{
				ClientGameManager.UIManager.AlchemySelectionUI.HandleInput();
			}
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x0012C120 File Offset: 0x0012A320
		private Vector2 GetMovement()
		{
			Vector2 rawMovementInput = this.m_rawMovementInput;
			Vector2 vector = this.GetMoveInput();
			this.m_rawMovementInput = vector;
			bool flag = !this.PreventCharacterMovement && this.HoldingLMB && this.HoldingRMB;
			if (flag)
			{
				vector.y = 1f;
			}
			if (this.m_wasMovingWithMouse != flag)
			{
				LocalPlayer.UpdateTimeOfLastInput();
			}
			this.m_wasMovingWithMouse = flag;
			if (this.m_autoRunEnabled)
			{
				if (vector.y < 0f)
				{
					this.m_autoRunEnabled = false;
				}
				else if (rawMovementInput.y == 0f && vector.y != 0f)
				{
					this.m_autoRunEnabled = false;
				}
				else if (this.m_inputPreventionFlags.AllowAutoRun())
				{
					vector.y = 1f;
				}
			}
			bool isConfused = this.m_isConfused;
			this.m_isConfused = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.IsConfused);
			if (LocalPlayer.FollowTarget)
			{
				if (this.m_isConfused || vector != Vector2.zero)
				{
					LocalPlayer.ClearFollowTarget();
				}
				else
				{
					float cachedSqrDistanceFromLocalPlayer = LocalPlayer.FollowTarget.GetCachedSqrDistanceFromLocalPlayer();
					if (cachedSqrDistanceFromLocalPlayer > 144f)
					{
						LocalPlayer.ClearFollowTarget();
					}
					else if (cachedSqrDistanceFromLocalPlayer > 16f)
					{
						vector.y = 1f;
					}
				}
			}
			if (this.m_isStunned)
			{
				vector = Vector2.zero;
			}
			else if (this.m_isConfused)
			{
				if (!isConfused)
				{
					this.RandomizeMovement();
				}
				float num = (vector.y > 0f) ? 1f : 0f;
				float num2 = (vector.y < 0f) ? 1f : 0f;
				float num3 = (vector.x > 0f) ? 1f : 0f;
				float num4 = (vector.x < 0f) ? 1f : 0f;
				this.m_confusedMovement[this.m_movementIndexes[0]] = num;
				this.m_confusedMovement[this.m_movementIndexes[1]] = num2;
				this.m_confusedMovement[this.m_movementIndexes[2]] = num3;
				this.m_confusedMovement[this.m_movementIndexes[3]] = num4;
				vector.y = this.m_confusedMovement[0] + -1f * this.m_confusedMovement[1];
				vector.x = this.m_confusedMovement[2] + -1f * this.m_confusedMovement[3];
			}
			return vector;
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x0012C370 File Offset: 0x0012A570
		private Vector2 GetMoveInput()
		{
			if (this.PreventCharacterMovement)
			{
				this.m_movementInputValues.ResetValues();
				return Vector2.zero;
			}
			float axis = SolInput.GetAxis(46);
			float axis2 = SolInput.GetAxis(45);
			int num = SolInput.GetButton(0) ? 1 : 0;
			int num2 = SolInput.GetButton(1) ? -1 : 0;
			int num3 = SolInput.GetButton(2) ? -1 : 0;
			int num4 = SolInput.GetButton(3) ? 1 : 0;
			if (this.HoldingRMB)
			{
				if (this.m_isTurningLeft && this.m_isTurningRight)
				{
					num3 = 0;
					num4 = 0;
				}
				else if (this.m_isTurningLeft)
				{
					num3 = -1;
				}
				else if (this.m_isTurningRight)
				{
					num4 = 1;
				}
			}
			this.m_movementInputValues.UpdateValues(axis, axis2, num, num2, num3, num4);
			return new Vector2((axis != 0f) ? axis : ((float)(num3 + num4)), (axis2 != 0f) ? axis2 : ((float)(num + num2)));
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x0012C44C File Offset: 0x0012A64C
		private Vector2 GetLookInput()
		{
			if (this.PreventInputForLook)
			{
				this.m_lookInputValues.ResetValues();
				return Vector2.zero;
			}
			float axis = SolInput.GetAxis(48);
			float axis2 = SolInput.GetAxis(47);
			int num = SolInput.GetButton(53) ? -1 : 0;
			int num2 = SolInput.GetButton(54) ? 1 : 0;
			int num3 = (SolInput.GetButton(55) || SolInput.GetButton(51)) ? -1 : 0;
			int num4 = (SolInput.GetButton(56) || SolInput.GetButton(52)) ? 1 : 0;
			this.m_lookInputValues.UpdateValues(axis, axis2, num, num2, num3, num4);
			return new Vector2((axis != 0f) ? axis : ((float)(num3 + num4)), (axis2 != 0f) ? axis2 : ((float)(num + num2)));
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x0012C508 File Offset: 0x0012A708
		private void RandomizeMovement()
		{
			this.m_consumedIndexes.Clear();
			for (int i = 0; i < 4; i++)
			{
				this.RandomizeMovementForIndex(i);
			}
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x0012C534 File Offset: 0x0012A734
		private void RandomizeMovementForIndex(int index)
		{
			this.m_shuffleIndexes.Shuffle<int>();
			for (int i = 0; i < this.m_shuffleIndexes.Length; i++)
			{
				int num = this.m_shuffleIndexes[i];
				if (num != index && !this.m_consumedIndexes.Contains(num))
				{
					this.m_movementIndexes[index] = num;
					this.m_consumedIndexes.Add(num);
					return;
				}
			}
			if (this.m_consumedIndexes.Count == 3 && !this.m_consumedIndexes.Contains(index))
			{
				this.m_movementIndexes[index] = index;
				this.m_consumedIndexes.Add(index);
				return;
			}
			throw new ArgumentException("Did not find a free index?  CurrentIndex: " + index.ToString() + " | Consumed: " + string.Join<int>(", ", this.m_consumedIndexes));
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x00059CAA File Offset: 0x00057EAA
		public void SetInputPreventionFlag(InputPreventionFlags flag)
		{
			this.m_inputPreventionFlags = this.m_inputPreventionFlags.SetBitFlag(flag);
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x00059CBE File Offset: 0x00057EBE
		public void UnsetInputPreventionFlag(InputPreventionFlags flag)
		{
			this.m_inputPreventionFlags = this.m_inputPreventionFlags.UnsetBitFlag(flag);
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x0012C5F0 File Offset: 0x0012A7F0
		private void EscapePressed()
		{
			if (UIManager.MenuEscapePressed())
			{
				return;
			}
			if (UIManager.HelpEscapePressed())
			{
				return;
			}
			if (UIManager.ChatWindowEscapePressed())
			{
				return;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.SkillsController.EscapePressed())
			{
				return;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController.EscapePressed())
			{
				return;
			}
			UIManager.EscapePressed();
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x00059CD2 File Offset: 0x00057ED2
		private void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			this.m_autoRunEnabled = false;
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x00059CDB File Offset: 0x00057EDB
		private void OnGMConsoleOpenStateChanged(bool isOpen)
		{
			this.m_inputPreventionFlags = (isOpen ? this.m_inputPreventionFlags.SetBitFlag(InputPreventionFlags.GMConsole) : this.m_inputPreventionFlags.UnsetBitFlag(InputPreventionFlags.GMConsole));
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x00059D02 File Offset: 0x00057F02
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.m_inputPreventionFlags = InputPreventionFlags.None;
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x00059D0B File Offset: 0x00057F0B
		private void LocalPlayerOnFollowTargetChanged(GameEntity obj)
		{
			if (this.m_autoRunEnabled && obj)
			{
				this.m_autoRunEnabled = false;
			}
		}

		// Token: 0x06002412 RID: 9234 RVA: 0x00059D24 File Offset: 0x00057F24
		private void InteractionManagerOnInteractionSuccess()
		{
			if (InteractionManager.HoveredUIElement == null)
			{
				this.m_autoRunEnabled = false;
			}
		}

		// Token: 0x04002707 RID: 9991
		private static readonly int[] kAbilityActions = new int[]
		{
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29
		};

		// Token: 0x04002708 RID: 9992
		private static readonly int[] kConsumableActions = new int[]
		{
			88,
			89,
			90,
			91
		};

		// Token: 0x04002709 RID: 9993
		private static readonly int[] kReagentActions = new int[]
		{
			92,
			93,
			94,
			95
		};

		// Token: 0x0400270A RID: 9994
		private static readonly int[] kMacroActions = new int[]
		{
			117,
			118,
			119,
			120,
			121
		};

		// Token: 0x0400270D RID: 9997
		public const float kDeadZone = 0.2f;

		// Token: 0x04002721 RID: 10017
		private InputPreventionFlags m_inputPreventionFlags;

		// Token: 0x04002722 RID: 10018
		private Vector2 m_rawMovementInput;

		// Token: 0x04002723 RID: 10019
		private bool m_autoRunEnabled;

		// Token: 0x04002724 RID: 10020
		private bool m_isConfused;

		// Token: 0x04002725 RID: 10021
		private bool m_isStunned;

		// Token: 0x04002726 RID: 10022
		private bool m_wasMovingWithMouse;

		// Token: 0x04002727 RID: 10023
		private bool m_isTurningLeft;

		// Token: 0x04002728 RID: 10024
		private bool m_isTurningRight;

		// Token: 0x04002729 RID: 10025
		private bool m_moveUiHeld;

		// Token: 0x0400272A RID: 10026
		private InputManager.InputValues m_movementInputValues;

		// Token: 0x0400272B RID: 10027
		private InputManager.InputValues m_lookInputValues;

		// Token: 0x0400272C RID: 10028
		private const int kMovementAxis = 4;

		// Token: 0x0400272D RID: 10029
		private readonly int[] m_shuffleIndexes = new int[]
		{
			0,
			1,
			2,
			3
		};

		// Token: 0x0400272E RID: 10030
		private readonly int[] m_movementIndexes = new int[]
		{
			0,
			1,
			2,
			3
		};

		// Token: 0x0400272F RID: 10031
		private readonly HashSet<int> m_consumedIndexes = new HashSet<int>();

		// Token: 0x04002730 RID: 10032
		private readonly float[] m_confusedMovement = new float[4];

		// Token: 0x020004F8 RID: 1272
		private class InputValues
		{
			// Token: 0x06002415 RID: 9237 RVA: 0x0012C718 File Offset: 0x0012A918
			public void UpdateValues(float x, float y, int a, int b, int c, int d)
			{
				if (x != this.m_x || y != this.m_y || a != this.m_a || b != this.m_b || c != this.m_c || d != this.m_d)
				{
					LocalPlayer.UpdateTimeOfLastInput();
				}
				this.m_x = x;
				this.m_y = y;
				this.m_a = a;
				this.m_b = b;
				this.m_c = c;
				this.m_d = d;
			}

			// Token: 0x06002416 RID: 9238 RVA: 0x00059D3A File Offset: 0x00057F3A
			public void ResetValues()
			{
				this.m_x = 0f;
				this.m_y = 0f;
				this.m_a = 0;
				this.m_b = 0;
				this.m_c = 0;
				this.m_d = 0;
			}

			// Token: 0x04002731 RID: 10033
			private float m_x;

			// Token: 0x04002732 RID: 10034
			private float m_y;

			// Token: 0x04002733 RID: 10035
			private int m_a;

			// Token: 0x04002734 RID: 10036
			private int m_b;

			// Token: 0x04002735 RID: 10037
			private int m_c;

			// Token: 0x04002736 RID: 10038
			private int m_d;
		}
	}
}
