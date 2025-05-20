using System;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008D7 RID: 2263
	public class StanceSelectionBubbleController : MonoBehaviour
	{
		// Token: 0x06004233 RID: 16947 RVA: 0x00191E14 File Offset: 0x00190014
		private void Awake()
		{
			for (int i = 0; i < this.m_bubbles.Length; i++)
			{
				this.m_bubbles[i].Bubble.Init(this, this.m_bubbles[i].Stance, this.m_bubbles[i].Icon);
			}
			LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
		}

		// Token: 0x06004234 RID: 16948 RVA: 0x0006CABE File Offset: 0x0006ACBE
		private void OnDestroy()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.VitalsReplicator)
			{
				LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
			}
		}

		// Token: 0x06004235 RID: 16949 RVA: 0x00191E84 File Offset: 0x00190084
		public void BubbleClicked(StanceSelectionBubble bubble)
		{
			if (LocalPlayer.IsStunned || !LocalPlayer.IsAlive)
			{
				return;
			}
			bool flag = this.m_selected == bubble;
			Stance stance = flag ? Stance.Idle : bubble.Stance;
			if (!LocalPlayer.Animancer.SetStance(stance))
			{
				return;
			}
			for (int i = 0; i < this.m_bubbles.Length; i++)
			{
				this.m_bubbles[i].Bubble.SetState(this.m_bubbles[i].Bubble == bubble && !flag);
			}
			this.m_selected = (flag ? null : bubble);
		}

		// Token: 0x06004236 RID: 16950 RVA: 0x00191F18 File Offset: 0x00190118
		private void CurrentStanceOnChanged(Stance obj)
		{
			switch (obj)
			{
			case Stance.Idle:
				for (int i = 0; i < this.m_bubbles.Length; i++)
				{
					this.m_bubbles[i].Bubble.SetState(false);
					this.m_bubbles[i].Bubble.ToggleInteractive(true);
				}
				this.m_selected = null;
				return;
			case Stance.Crouch:
			case Stance.Looting:
				return;
			case Stance.Swim:
			case Stance.Unconscious:
				for (int j = 0; j < this.m_bubbles.Length; j++)
				{
					this.m_bubbles[j].Bubble.SetState(false);
					this.m_bubbles[j].Bubble.ToggleInteractive(false);
				}
				this.m_selected = null;
				return;
			}
			for (int k = 0; k < this.m_bubbles.Length; k++)
			{
				bool flag = this.m_bubbles[k].Bubble.Stance == obj;
				this.m_bubbles[k].Bubble.SetState(flag);
				this.m_bubbles[k].Bubble.ToggleInteractive(true);
				if (flag)
				{
					this.m_selected = this.m_bubbles[k].Bubble;
				}
			}
		}

		// Token: 0x04003F47 RID: 16199
		[SerializeField]
		private StanceSelectionBubbleController.BubbleConfig[] m_bubbles;

		// Token: 0x04003F48 RID: 16200
		private StanceSelectionBubble m_selected;

		// Token: 0x020008D8 RID: 2264
		[Serializable]
		private class BubbleConfig
		{
			// Token: 0x04003F49 RID: 16201
			public Stance Stance;

			// Token: 0x04003F4A RID: 16202
			public StanceSelectionBubble Bubble;

			// Token: 0x04003F4B RID: 16203
			public Sprite Icon;
		}
	}
}
