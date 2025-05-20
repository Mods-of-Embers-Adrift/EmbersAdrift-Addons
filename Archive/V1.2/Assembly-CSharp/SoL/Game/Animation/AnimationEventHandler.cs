using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D70 RID: 3440
	public class AnimationEventHandler : GameEntityComponent
	{
		// Token: 0x0600678B RID: 26507 RVA: 0x00085956 File Offset: 0x00083B56
		private void AudioEvent(string eventName)
		{
			if (base.GameEntity != null && base.GameEntity.AudioEventController != null)
			{
				base.GameEntity.AudioEventController.PlayAudioEvent(eventName);
			}
		}

		// Token: 0x0600678C RID: 26508 RVA: 0x0004475B File Offset: 0x0004295B
		private void SendEvent(string eventName)
		{
		}

		// Token: 0x0600678D RID: 26509 RVA: 0x0008598A File Offset: 0x00083B8A
		protected virtual void AttachWeapons()
		{
			this.AudioEvent("WeaponEquip");
		}

		// Token: 0x0600678E RID: 26510 RVA: 0x00085997 File Offset: 0x00083B97
		protected virtual void DetachWeapons()
		{
			this.AudioEvent("WeaponUnequip");
		}

		// Token: 0x0600678F RID: 26511 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AttachLight()
		{
		}

		// Token: 0x06006790 RID: 26512 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void DetachLight()
		{
		}

		// Token: 0x06006791 RID: 26513 RVA: 0x00213270 File Offset: 0x00211470
		private void ExecuteAbility()
		{
			if (base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.Type == GameEntityType.Player && ClientGameManager.DelayedEventManager)
			{
				ClientGameManager.DelayedEventManager.ExecuteAbility(base.GameEntity.NetworkEntity.NetworkId.Value);
			}
		}

		// Token: 0x06006792 RID: 26514 RVA: 0x002132D8 File Offset: 0x002114D8
		private void Defend(string eventName)
		{
			if (!base.GameEntity || !base.GameEntity.NetworkEntity || base.GameEntity.Type != GameEntityType.Player || !ClientGameManager.DelayedEventManager || !ClientGameManager.DelayedEventManager.Defend(base.GameEntity.NetworkEntity.NetworkId.Value))
			{
				this.PlayAudioEvent(eventName);
			}
		}

		// Token: 0x06006793 RID: 26515 RVA: 0x000859A4 File Offset: 0x00083BA4
		private void Run()
		{
			this.PlayLocomotionEvent("Run");
		}

		// Token: 0x06006794 RID: 26516 RVA: 0x000859B1 File Offset: 0x00083BB1
		private void Walk()
		{
			this.PlayLocomotionEvent("Walk");
		}

		// Token: 0x06006795 RID: 26517 RVA: 0x000859B1 File Offset: 0x00083BB1
		private void Swim()
		{
			this.PlayLocomotionEvent("Walk");
		}

		// Token: 0x06006796 RID: 26518 RVA: 0x0004475B File Offset: 0x0004295B
		private void Turn()
		{
		}

		// Token: 0x06006797 RID: 26519 RVA: 0x0021334C File Offset: 0x0021154C
		private void PlayLocomotionEvent(string eventName)
		{
			int frameCount = Time.frameCount;
			if (frameCount > this.m_lastLocomotionFrame)
			{
				this.AudioEvent(eventName);
				this.m_lastLocomotionFrame = frameCount;
			}
		}

		// Token: 0x06006798 RID: 26520 RVA: 0x000859BE File Offset: 0x00083BBE
		private void SwingWeapon()
		{
			this.AudioEvent("WeaponSwing");
		}

		// Token: 0x06006799 RID: 26521 RVA: 0x000859BE File Offset: 0x00083BBE
		private void WeaponSwing()
		{
			this.AudioEvent("WeaponSwing");
		}

		// Token: 0x0600679A RID: 26522 RVA: 0x000859CB File Offset: 0x00083BCB
		private void Punch()
		{
			this.AudioEvent("Punch");
		}

		// Token: 0x0600679B RID: 26523 RVA: 0x000859D8 File Offset: 0x00083BD8
		private void Death()
		{
			this.AudioEvent("Death");
		}

		// Token: 0x0600679C RID: 26524 RVA: 0x000859E5 File Offset: 0x00083BE5
		private void BodyFall()
		{
			this.AudioEvent("BodyFall");
		}

		// Token: 0x0600679D RID: 26525 RVA: 0x000859F2 File Offset: 0x00083BF2
		private void DrawBow()
		{
			this.AudioEvent("DrawBow");
		}

		// Token: 0x0600679E RID: 26526 RVA: 0x000859FF File Offset: 0x00083BFF
		private void BowLoadArrow()
		{
			this.AudioEvent("BowLoadArrow");
		}

		// Token: 0x0600679F RID: 26527 RVA: 0x00085A0C File Offset: 0x00083C0C
		private void WeaponShoot()
		{
			this.AudioEvent("WeaponShoot");
			this.ExecuteAbility();
		}

		// Token: 0x060067A0 RID: 26528 RVA: 0x00085A1F File Offset: 0x00083C1F
		private void Attack()
		{
			this.AudioEvent("Attack");
		}

		// Token: 0x060067A1 RID: 26529 RVA: 0x00085A2C File Offset: 0x00083C2C
		private void Avoid()
		{
			this.AudioEvent("Avoid");
		}

		// Token: 0x060067A2 RID: 26530 RVA: 0x00085A39 File Offset: 0x00083C39
		private void Block()
		{
			this.Defend("Block");
		}

		// Token: 0x060067A3 RID: 26531 RVA: 0x00085A46 File Offset: 0x00083C46
		private void ShieldBlock()
		{
			this.Defend("ShieldBlock");
		}

		// Token: 0x060067A4 RID: 26532 RVA: 0x00085A53 File Offset: 0x00083C53
		private void Parry()
		{
			this.Defend("Parry");
		}

		// Token: 0x060067A5 RID: 26533 RVA: 0x00085A60 File Offset: 0x00083C60
		private void Riposte()
		{
			this.Defend("Riposte");
		}

		// Token: 0x060067A6 RID: 26534 RVA: 0x00085A6D File Offset: 0x00083C6D
		private void Stomp()
		{
			this.AudioEvent("Stomp");
		}

		// Token: 0x060067A7 RID: 26535 RVA: 0x00085A7A File Offset: 0x00083C7A
		private void PlaySound(string eventName)
		{
			this.AudioEvent(eventName);
		}

		// Token: 0x060067A8 RID: 26536 RVA: 0x00085A7A File Offset: 0x00083C7A
		private void PlayAudioEvent(string eventName)
		{
			this.AudioEvent(eventName);
		}

		// Token: 0x060067A9 RID: 26537 RVA: 0x0004475B File Offset: 0x0004295B
		private void EnableHeldItems()
		{
		}

		// Token: 0x060067AA RID: 26538 RVA: 0x0004475B File Offset: 0x0004295B
		private void DisableArmUse()
		{
		}

		// Token: 0x04005A0B RID: 23051
		private const string kEquipEvent = "WeaponEquip";

		// Token: 0x04005A0C RID: 23052
		private const string kUnequipEvent = "WeaponUnequip";

		// Token: 0x04005A0D RID: 23053
		public const string kWalkEvent = "Walk";

		// Token: 0x04005A0E RID: 23054
		public const string kRunEvent = "Run";

		// Token: 0x04005A0F RID: 23055
		private const string kWeaponSwingEvent = "WeaponSwing";

		// Token: 0x04005A10 RID: 23056
		private const string kPunchEvent = "Punch";

		// Token: 0x04005A11 RID: 23057
		private const string kDeathEvent = "Death";

		// Token: 0x04005A12 RID: 23058
		private const string kBodyFallEvent = "BodyFall";

		// Token: 0x04005A13 RID: 23059
		private const string kDrawBowEvent = "DrawBow";

		// Token: 0x04005A14 RID: 23060
		private const string kBowLoadArrowEvent = "BowLoadArrow";

		// Token: 0x04005A15 RID: 23061
		private const string kWeaponShootEvent = "WeaponShoot";

		// Token: 0x04005A16 RID: 23062
		private const string kAttackEvent = "Attack";

		// Token: 0x04005A17 RID: 23063
		private const string kAvoidEvent = "Avoid";

		// Token: 0x04005A18 RID: 23064
		public const string kBlockEvent = "Block";

		// Token: 0x04005A19 RID: 23065
		public const string kShieldBlockEvent = "ShieldBlock";

		// Token: 0x04005A1A RID: 23066
		public const string kParryEvent = "Parry";

		// Token: 0x04005A1B RID: 23067
		public const string kRiposteEvent = "Riposte";

		// Token: 0x04005A1C RID: 23068
		private const string kStompEvent = "Stomp";

		// Token: 0x04005A1D RID: 23069
		private int m_lastLocomotionFrame;
	}
}
