using System;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Utilities.Extensions;

namespace SoL.Game
{
	// Token: 0x020005EF RID: 1519
	public class PlayerVitals_Client : Vitals
	{
		// Token: 0x0600301A RID: 12314 RVA: 0x000612B2 File Offset: 0x0005F4B2
		public override float GetHealthPercent()
		{
			return this.Health / (float)this.MaxHealth;
		}

		// Token: 0x0600301B RID: 12315 RVA: 0x000612C2 File Offset: 0x0005F4C2
		public override float GetArmorClassPercent()
		{
			if (this.MaxArmorClass <= 0)
			{
				return 0f;
			}
			return (float)this.ArmorClass / (float)this.MaxArmorClass;
		}

		// Token: 0x0600301C RID: 12316 RVA: 0x000612E2 File Offset: 0x0005F4E2
		protected override void Subscribe()
		{
			base.Subscribe();
			if (this.PlayerReplicator)
			{
				this.PlayerReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x00061314 File Offset: 0x0005F514
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			if (this.PlayerReplicator)
			{
				this.PlayerReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x001589EC File Offset: 0x00156BEC
		protected virtual void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious && base.GameEntity && base.GameEntity != LocalPlayer.GameEntity && ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsMemberOfMyGroup(base.GameEntity) && base.GameEntity.CharacterData)
			{
				string arg = SoL.Utilities.Extensions.TextMeshProExtensions.CreatePlayerLink(base.GameEntity.CharacterData.Name.Value);
				string content = ZString.Format<string>("{0} has been knocked unconscious!", arg);
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
			}
		}

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x0600301F RID: 12319 RVA: 0x00061346 File Offset: 0x0005F546
		public override float Health
		{
			get
			{
				return (float)this.PlayerReplicator.Health.Value;
			}
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06003020 RID: 12320 RVA: 0x00061359 File Offset: 0x0005F559
		public override float HealthWound
		{
			get
			{
				return (float)this.PlayerReplicator.HealthWound.Value;
			}
		}

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06003021 RID: 12321 RVA: 0x0006136C File Offset: 0x0005F56C
		public override int MaxHealth
		{
			get
			{
				return this.PlayerReplicator.MaxHealth.Value;
			}
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06003022 RID: 12322 RVA: 0x0006137E File Offset: 0x0005F57E
		public override float Stamina
		{
			get
			{
				return (float)this.PlayerReplicator.Stamina.Value;
			}
		}

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06003023 RID: 12323 RVA: 0x00061391 File Offset: 0x0005F591
		public override float StaminaWound
		{
			get
			{
				return (float)this.PlayerReplicator.StaminaWound.Value;
			}
		}

		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06003024 RID: 12324 RVA: 0x000613A4 File Offset: 0x0005F5A4
		public override int ArmorClass
		{
			get
			{
				if (this.Stance != Stance.Combat)
				{
					return this.PlayerReplicator.ArmorClass.Value;
				}
				return this.PlayerReplicator.ArmorClass.Value + this.m_shieldArmorClass;
			}
		}

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06003025 RID: 12325 RVA: 0x000613D7 File Offset: 0x0005F5D7
		public override int MaxArmorClass
		{
			get
			{
				if (this.Stance != Stance.Combat)
				{
					return this.PlayerReplicator.MaxArmorClass.Value;
				}
				return this.PlayerReplicator.MaxArmorClass.Value + this.m_shieldMaxArmorClass;
			}
		}

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06003026 RID: 12326 RVA: 0x0006140A File Offset: 0x0005F60A
		protected VitalsReplicatorPlayer PlayerReplicator
		{
			get
			{
				if (this.m_playerReplicator == null)
				{
					this.m_playerReplicator = (base.m_replicator as VitalsReplicatorPlayer);
				}
				return this.m_playerReplicator;
			}
		}

		// Token: 0x04002EEC RID: 12012
		private VitalsReplicatorPlayer m_playerReplicator;
	}
}
