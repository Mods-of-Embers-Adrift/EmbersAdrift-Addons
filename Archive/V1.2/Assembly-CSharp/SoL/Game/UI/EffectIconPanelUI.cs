using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000878 RID: 2168
	public class EffectIconPanelUI : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x06003F05 RID: 16133 RVA: 0x0006AA11 File Offset: 0x00068C11
		private void Awake()
		{
			Vitals.RefreshEffectPanelForEntity += this.VitalsOnRefreshEffectPanelForEntity;
		}

		// Token: 0x06003F06 RID: 16134 RVA: 0x0006AA24 File Offset: 0x00068C24
		private void OnDestroy()
		{
			this.Unsubscribe();
			Vitals.RefreshEffectPanelForEntity -= this.VitalsOnRefreshEffectPanelForEntity;
		}

		// Token: 0x06003F07 RID: 16135 RVA: 0x0006AA3D File Offset: 0x00068C3D
		public void Init(NetworkEntity netEntity, bool isSelfPanel = false)
		{
			this.m_isSelfPanel = isSelfPanel;
			this.Unsubscribe();
			this.m_networkEntity = netEntity;
			this.RefreshEffects();
			this.Subscribe();
			this.m_resizableWindow = (isSelfPanel ? base.gameObject.GetComponent<ResizableWindow>() : null);
		}

		// Token: 0x06003F08 RID: 16136 RVA: 0x00186FA8 File Offset: 0x001851A8
		private void Subscribe()
		{
			if (this.m_networkEntity && this.m_networkEntity.GameEntity && this.m_networkEntity.GameEntity.VitalsReplicator && this.m_networkEntity.GameEntity.VitalsReplicator.Effects != null)
			{
				this.m_networkEntity.GameEntity.VitalsReplicator.Effects.Changed += this.EffectsOnChanged;
			}
		}

		// Token: 0x06003F09 RID: 16137 RVA: 0x00187028 File Offset: 0x00185228
		private void Unsubscribe()
		{
			if (this.m_networkEntity && this.m_networkEntity.GameEntity && this.m_networkEntity.GameEntity.VitalsReplicator && this.m_networkEntity.GameEntity.VitalsReplicator.Effects != null)
			{
				this.m_networkEntity.GameEntity.VitalsReplicator.Effects.Changed -= this.EffectsOnChanged;
			}
		}

		// Token: 0x06003F0A RID: 16138 RVA: 0x0006AA76 File Offset: 0x00068C76
		private void VitalsOnRefreshEffectPanelForEntity(NetworkEntity obj)
		{
			if (this.m_networkEntity != null && obj == this.m_networkEntity)
			{
				this.RefreshEffects();
			}
		}

		// Token: 0x06003F0B RID: 16139 RVA: 0x001870A8 File Offset: 0x001852A8
		public void RefreshEffects()
		{
			if (!this.m_networkEntity || !this.m_networkEntity.GameEntity || !this.m_networkEntity.GameEntity.VitalsReplicator || this.m_networkEntity.GameEntity.VitalsReplicator.Effects == null)
			{
				for (int i = 0; i < this.m_effectIcons.Count; i++)
				{
					this.m_effectIcons[i].Init(this.m_networkEntity, null, this.m_isSelfPanel);
				}
				return;
			}
			int count = this.m_networkEntity.GameEntity.VitalsReplicator.Effects.Count;
			for (int j = 0; j < count; j++)
			{
				if (j > this.m_effectIcons.Count - 1)
				{
					EffectIconUI component = UnityEngine.Object.Instantiate<GameObject>(this.m_effectIconPrefab, base.transform).GetComponent<EffectIconUI>();
					this.m_effectIcons.Add(component);
				}
				EffectSyncData value = this.m_networkEntity.GameEntity.VitalsReplicator.Effects[j];
				this.m_effectIcons[j].Init(this.m_networkEntity, new EffectSyncData?(value), this.m_isSelfPanel);
			}
			if (this.m_effectIcons.Count > count)
			{
				for (int k = count; k < this.m_effectIcons.Count; k++)
				{
					this.m_effectIcons[k].Init(this.m_networkEntity, null, this.m_isSelfPanel);
				}
			}
		}

		// Token: 0x06003F0C RID: 16140 RVA: 0x0006AA9A File Offset: 0x00068C9A
		private void EffectsOnChanged(SynchronizedCollection<UniqueId, EffectSyncData>.Operation arg1, UniqueId arg2, EffectSyncData arg3, EffectSyncData arg4)
		{
			this.RefreshEffects();
		}

		// Token: 0x17000E94 RID: 3732
		// (get) Token: 0x06003F0D RID: 16141 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003F0E RID: 16142 RVA: 0x0018722C File Offset: 0x0018542C
		string IContextMenu.FillActionsGetTitle()
		{
			if (!this.m_resizableWindow)
			{
				return null;
			}
			string text = this.m_resizableWindow.FillActionsGetTitle();
			if (this.m_lockToggle)
			{
				ContextMenuUI.AddContextAction((this.m_lockToggle.State == ToggleController.ToggleState.ON) ? ZString.Format<string>("{0} Unlock Position", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>") : ZString.Format<string>("{0} Lock Position", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>"), true, delegate()
				{
					this.m_lockToggle.Toggle();
				}, null, null);
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "Panel";
		}

		// Token: 0x06003F10 RID: 16144 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003CFB RID: 15611
		[SerializeField]
		private GameObject m_effectIconPrefab;

		// Token: 0x04003CFC RID: 15612
		[SerializeField]
		private ToggleController m_lockToggle;

		// Token: 0x04003CFD RID: 15613
		private readonly List<EffectIconUI> m_effectIcons = new List<EffectIconUI>(10);

		// Token: 0x04003CFE RID: 15614
		private NetworkEntity m_networkEntity;

		// Token: 0x04003CFF RID: 15615
		private bool m_isSelfPanel;

		// Token: 0x04003D00 RID: 15616
		private ResizableWindow m_resizableWindow;

		// Token: 0x04003D01 RID: 15617
		private InteractionSettings m_settings;
	}
}
