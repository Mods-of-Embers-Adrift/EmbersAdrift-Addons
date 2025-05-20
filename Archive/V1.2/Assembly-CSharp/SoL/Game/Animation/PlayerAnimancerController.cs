using System;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D83 RID: 3459
	public class PlayerAnimancerController : GameEntityComponent
	{
		// Token: 0x17001909 RID: 6409
		// (get) Token: 0x06006830 RID: 26672 RVA: 0x00085EC8 File Offset: 0x000840C8
		// (set) Token: 0x06006831 RID: 26673 RVA: 0x002139DC File Offset: 0x00211BDC
		public Stance Stance
		{
			get
			{
				return this.m_stance;
			}
			private set
			{
				if (this.m_stance == value)
				{
					return;
				}
				if (value == Stance.Sit && ClientGameManager.InputManager.MovementInput != Vector2.zero)
				{
					return;
				}
				Stance stance = this.m_stance;
				this.m_stance = value;
				base.GameEntity.Vitals.Stance = this.m_stance;
				switch (this.m_stance)
				{
				case Stance.Idle:
					this.SetStanceId(this.m_idleSet);
					break;
				case Stance.Crouch:
					this.SetStanceId(this.m_crouchSet);
					break;
				case Stance.Combat:
					this.SetStanceId((this.m_combatSet == null) ? this.m_fallbackCombatSet : this.m_combatSet);
					if (this.m_bypassTransition || this.m_combatSet.EnterTransitionSequence.IsEmpty)
					{
						base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.Weapons;
					}
					break;
				case Stance.Torch:
					this.SetStanceId(this.m_torchSet);
					break;
				case Stance.Sit:
					this.SetStanceId(this.m_sitPose);
					break;
				case Stance.Swim:
					this.SetStanceId(GlobalSettings.Values.Stance.SwimmingStance.AnimationSet);
					break;
				case Stance.Looting:
				{
					BaseNetworkedInteractive baseNetworkedInteractive;
					AnimancerAnimationPose stanceId = (LocalPlayer.LootInteractive != null && LocalPlayer.LootInteractive.TryGetAsType(out baseNetworkedInteractive) && baseNetworkedInteractive.InteractionPose != null) ? baseNetworkedInteractive.InteractionPose : this.m_lootPose;
					this.SetStanceId(stanceId);
					break;
				}
				}
				if (this.m_stance != Stance.Idle)
				{
					if (stance != Stance.Combat)
					{
						if (stance == Stance.Torch && base.GameEntity.CharacterData.ItemsAttached.Value == ItemsAttached.Light)
						{
							base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.None;
						}
					}
					else if (base.GameEntity.CharacterData.ItemsAttached.Value == ItemsAttached.Weapons)
					{
						base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.None;
					}
				}
				this.QueueItemsAttachValidation();
			}
		}

		// Token: 0x06006832 RID: 26674 RVA: 0x00085ED0 File Offset: 0x000840D0
		private void Awake()
		{
			LocalPlayer.Animancer = this;
		}

		// Token: 0x06006833 RID: 26675 RVA: 0x00213BC4 File Offset: 0x00211DC4
		private void Start()
		{
			this.m_replicator = base.GameEntity.AnimatorReplicator;
			OnAnimatorMoveForwarder orAddComponent = base.GameEntity.AnimancerController.gameObject.GetOrAddComponent<OnAnimatorMoveForwarder>();
			PlayerMotorController component = base.gameObject.GetComponent<PlayerMotorController>();
			if (orAddComponent != null && component != null)
			{
				orAddComponent.Receiver = component;
			}
			this.m_idleSet = GlobalSettings.Values.Animation.IdleSetPair;
			this.SetStanceId(this.m_idleSet);
			this.OnSettingsChanged();
			LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged += this.OnSettingsChanged;
			LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged += this.OnSettingsChanged;
			if (base.GameEntity.CollectionController.Equipment != null)
			{
				this.EquipmentOnContentsChanged();
				base.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
			}
			else
			{
				base.GameEntity.NetworkEntity.OnStartLocalClient += this.NetworkEntityOnOnStartLocalClient;
			}
			base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
		}

		// Token: 0x06006834 RID: 26676 RVA: 0x00213CF4 File Offset: 0x00211EF4
		private void OnDestroy()
		{
			if (base.GameEntity)
			{
				if (base.GameEntity.CharacterData)
				{
					base.GameEntity.CharacterData.MasteryConfigurationChanged -= this.OnSettingsChanged;
					base.GameEntity.CharacterData.HandConfigurationChanged -= this.OnSettingsChanged;
				}
				if (base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Equipment != null)
				{
					base.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
				}
				if (base.GameEntity.VitalsReplicator)
				{
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
				}
			}
			ClientGameManager.InputManager.UnsetInputPreventionFlag(InputPreventionFlags.HealthState);
		}

		// Token: 0x06006835 RID: 26677 RVA: 0x00085ED8 File Offset: 0x000840D8
		private void OnSettingsChanged()
		{
			this.SetCurrentCombatStance();
			if (this.Stance == Stance.Combat)
			{
				this.SetStanceId(this.m_combatSet);
			}
		}

		// Token: 0x06006836 RID: 26678 RVA: 0x00213DDC File Offset: 0x00211FDC
		private void NetworkEntityOnOnStartLocalClient()
		{
			base.GameEntity.NetworkEntity.OnStartLocalClient -= this.NetworkEntityOnOnStartLocalClient;
			this.EquipmentOnContentsChanged();
			base.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
		}

		// Token: 0x06006837 RID: 26679 RVA: 0x00085EF5 File Offset: 0x000840F5
		private void EquipmentOnContentsChanged()
		{
			this.SetCurrentCombatStance();
		}

		// Token: 0x06006838 RID: 26680 RVA: 0x00213E2C File Offset: 0x0021202C
		private void SetCurrentCombatStance()
		{
			if (!GlobalSettings.Values.Animation.HumanoidWeaponAnimationSets)
			{
				this.m_combatSet = this.m_fallbackCombatSet;
				return;
			}
			this.m_combatSet = GlobalSettings.Values.Animation.HumanoidWeaponAnimationSets.GetAnimationSet(base.GameEntity);
			LocalPlayer.GameEntity.CharacterData.CurrentCombatId.Value = this.m_combatSet.Id;
		}

		// Token: 0x06006839 RID: 26681 RVA: 0x00085EFD File Offset: 0x000840FD
		public void ForceCombat(bool instant)
		{
			this.m_bypassTransition = instant;
			if (this.Stance != Stance.Combat)
			{
				this.m_previousStanceCombat = new Stance?(this.Stance);
				this.Stance = Stance.Combat;
			}
			this.m_bypassTransition = false;
		}

		// Token: 0x0600683A RID: 26682 RVA: 0x00085F2E File Offset: 0x0008412E
		public void ToggleCombat()
		{
			this.Stance = ((this.Stance == Stance.Combat) ? Stance.Idle : Stance.Combat);
			if (this.Stance == Stance.Combat)
			{
				ClientGameManager.NotificationsManager.TryShowTutorial(TutorialProgress.SheatheWeapon);
			}
		}

		// Token: 0x0600683B RID: 26683 RVA: 0x00085F5C File Offset: 0x0008415C
		public void ToggleSit()
		{
			this.Stance = ((this.Stance == Stance.Sit) ? Stance.Idle : Stance.Sit);
		}

		// Token: 0x0600683C RID: 26684 RVA: 0x00085F71 File Offset: 0x00084171
		public void ToggleLight()
		{
			this.Stance = ((this.Stance == Stance.Torch) ? Stance.Idle : Stance.Torch);
		}

		// Token: 0x0600683D RID: 26685 RVA: 0x00213E9C File Offset: 0x0021209C
		public void ToggleLooting(bool looting)
		{
			if (looting)
			{
				this.m_previousStanceLooting = new Stance?(this.Stance);
				this.Stance = Stance.Looting;
				return;
			}
			if (this.Stance == Stance.Looting)
			{
				if (Options.GameOptions.LeaveCombatAfterLooting && this.m_previousStanceLooting != null && this.m_previousStanceLooting.Value == Stance.Combat)
				{
					this.Stance = Stance.Idle;
				}
				else
				{
					this.Stance = ((this.m_previousStanceLooting != null && this.m_previousStanceLooting.Value != Stance.Looting && this.m_previousStanceLooting.Value != Stance.Sit) ? this.m_previousStanceLooting.Value : Stance.Idle);
				}
			}
			this.m_previousStanceLooting = null;
		}

		// Token: 0x0600683E RID: 26686 RVA: 0x00085F86 File Offset: 0x00084186
		public bool SetStance(Stance stance)
		{
			this.m_previousStanceCombat = null;
			Stance stance2 = this.Stance;
			this.Stance = stance;
			return stance2 != this.Stance;
		}

		// Token: 0x0600683F RID: 26687 RVA: 0x00213F48 File Offset: 0x00212148
		private void Update()
		{
			if (LocalPlayer.Motor != null && LocalPlayer.Motor.Motor != null && LocalPlayer.Motor.Motor.GroundingStatus.IsStableOnGround && LocalPlayer.Motor.Motor.GroundingStatus.GroundCollider != null && LocalPlayer.Motor.Motor.GroundingStatus.GroundCollider.gameObject.CompareTag("Water"))
			{
				this.Stance = Stance.Swim;
			}
			else if (this.Stance == Stance.Swim)
			{
				this.Stance = Stance.Idle;
			}
			else
			{
				if (this.Stance == Stance.Sit)
				{
					this.SetLocomotionRotation(Vector2.zero, 0f);
					if (ClientGameManager.InputManager.MovementInput != Vector2.zero)
					{
						this.Stance = Stance.Idle;
					}
					return;
				}
				bool flag = !LocalPlayer.IsStunned && ClientGameManager.InputManager.IsCrouching;
				if (flag && this.Stance == Stance.Idle)
				{
					this.Stance = Stance.Crouch;
				}
				else if (!flag && this.Stance == Stance.Crouch)
				{
					this.Stance = Stance.Idle;
				}
			}
			float angleDelta = LocalPlayer.Motor.AngleDelta;
			Vector2 locomotion = (ClientGameManager.InputManager.MovementInput != Vector2.zero) ? LocalPlayer.Motor.TargetLocomotion : Vector2.zero;
			this.SetLocomotionRotation(locomotion, angleDelta);
			this.ValidateItemsAttached();
		}

		// Token: 0x06006840 RID: 26688 RVA: 0x00085FAC File Offset: 0x000841AC
		private void SetLocomotionRotation(Vector2 locomotion, float rotation)
		{
			this.m_replicator.RawLocomotion = locomotion;
			this.m_replicator.RawRotation = rotation;
		}

		// Token: 0x06006841 RID: 26689 RVA: 0x00085FC6 File Offset: 0x000841C6
		private void QueueItemsAttachValidation()
		{
			this.m_itemsAttachedValidateTime = new float?(Time.time + 1.1f);
		}

		// Token: 0x06006842 RID: 26690 RVA: 0x002140A0 File Offset: 0x002122A0
		private void ValidateItemsAttached()
		{
			if (this.m_itemsAttachedValidateTime == null || this.m_itemsAttachedValidateTime.Value > Time.time)
			{
				return;
			}
			switch (this.Stance)
			{
			case Stance.Idle:
			case Stance.Crouch:
			case Stance.Sit:
			case Stance.Swim:
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.None;
				break;
			case Stance.Combat:
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.Weapons;
				break;
			case Stance.Torch:
				base.GameEntity.CharacterData.ItemsAttached.Value = ItemsAttached.Light;
				break;
			}
			this.m_itemsAttachedValidateTime = null;
		}

		// Token: 0x06006843 RID: 26691 RVA: 0x0006109C File Offset: 0x0005F29C
		private float GetSpeedModifier()
		{
			return 1f;
		}

		// Token: 0x06006844 RID: 26692 RVA: 0x00214150 File Offset: 0x00212350
		private void SetStanceId(IAnimancerAnimation set)
		{
			base.GameEntity.CharacterData.CurrentStanceData.Value = new StanceData
			{
				BypassTransition = this.m_bypassTransition,
				StanceId = set.Id
			};
		}

		// Token: 0x06006845 RID: 26693 RVA: 0x00085FDE File Offset: 0x000841DE
		private void HealthStateOnChanged(HealthState obj)
		{
			if (obj - HealthState.Unconscious <= 2)
			{
				this.Stance = Stance.Idle;
				ClientGameManager.InputManager.SetInputPreventionFlag(InputPreventionFlags.HealthState);
				return;
			}
			ClientGameManager.InputManager.UnsetInputPreventionFlag(InputPreventionFlags.HealthState);
		}

		// Token: 0x04005A6A RID: 23146
		private AnimancerReplicator m_replicator;

		// Token: 0x04005A6B RID: 23147
		private IAnimancerAnimation m_idleSet;

		// Token: 0x04005A6C RID: 23148
		[SerializeField]
		private AnimancerAnimationSet m_crouchSet;

		// Token: 0x04005A6D RID: 23149
		[SerializeField]
		private AnimancerAnimationSet m_fallbackCombatSet;

		// Token: 0x04005A6E RID: 23150
		[SerializeField]
		private AnimancerAnimationSet m_sitSet;

		// Token: 0x04005A6F RID: 23151
		[SerializeField]
		private AnimancerAnimationSet m_lootingSet;

		// Token: 0x04005A70 RID: 23152
		[SerializeField]
		private AnimancerAnimationPose m_torchSet;

		// Token: 0x04005A71 RID: 23153
		[SerializeField]
		private AnimancerAnimationPose m_sitPose;

		// Token: 0x04005A72 RID: 23154
		[SerializeField]
		private AnimancerAnimationPose m_lootPose;

		// Token: 0x04005A73 RID: 23155
		private AnimancerAnimationSet m_combatSet;

		// Token: 0x04005A74 RID: 23156
		private bool m_bypassTransition;

		// Token: 0x04005A75 RID: 23157
		private Stance m_stance;

		// Token: 0x04005A76 RID: 23158
		private Stance? m_previousStanceCombat;

		// Token: 0x04005A77 RID: 23159
		private Stance? m_previousStanceLooting;

		// Token: 0x04005A78 RID: 23160
		private float? m_itemsAttachedValidateTime = new float?(0f);
	}
}
