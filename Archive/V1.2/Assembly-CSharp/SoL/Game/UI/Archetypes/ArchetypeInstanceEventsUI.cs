using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009BE RID: 2494
	public abstract class ArchetypeInstanceEventsUI<T> : MonoBehaviour, IArchetypeInstanceEventsUI where T : BaseCooldownUI
	{
		// Token: 0x17001091 RID: 4241
		// (get) Token: 0x06004B76 RID: 19318 RVA: 0x00073163 File Offset: 0x00071363
		protected ArchetypeInstance m_instance
		{
			get
			{
				if (!this.m_ui)
				{
					return null;
				}
				return this.m_ui.Instance;
			}
		}

		// Token: 0x17001092 RID: 4242
		// (get) Token: 0x06004B77 RID: 19319 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool CanPlace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001093 RID: 4243
		// (get) Token: 0x06004B78 RID: 19320 RVA: 0x0007317F File Offset: 0x0007137F
		protected virtual bool CanModify
		{
			get
			{
				return !this.m_execution.IsActive && !this.m_global.IsActive;
			}
		}

		// Token: 0x17001094 RID: 4244
		// (get) Token: 0x06004B79 RID: 19321 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool AuraIsActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001095 RID: 4245
		// (get) Token: 0x06004B7A RID: 19322 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool ContextualDisablePanel
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001096 RID: 4246
		// (get) Token: 0x06004B7B RID: 19323 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool ShowTargetOverlay
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004B7C RID: 19324 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void Subscribe()
		{
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void Unsubscribe()
		{
		}

		// Token: 0x17001097 RID: 4247
		// (get) Token: 0x06004B7E RID: 19326 RVA: 0x0007319E File Offset: 0x0007139E
		protected static ExecutionCache ExecutionCache
		{
			get
			{
				if (ArchetypeInstanceEventsUI<T>.m_executionCache == null)
				{
					ArchetypeInstanceEventsUI<T>.m_executionCache = new ExecutionCache();
				}
				return ArchetypeInstanceEventsUI<T>.m_executionCache;
			}
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x001B9138 File Offset: 0x001B7338
		protected virtual void Update()
		{
			if (this.m_subscribeFrame != null && this.m_subscribeFrame.Value <= Time.frameCount)
			{
				this.m_subscribeFrame = null;
				this.Subscribe();
				return;
			}
			if (this.EarlyOutOfUpdate())
			{
				return;
			}
			Image availableOverlay = this.m_ui.AvailableOverlay;
			TargetOverlayState targetOverlayState = TargetOverlayState.None;
			if (this.m_ui.DisabledPanel.alpha <= 0f && this.m_instance != null && this.m_instance.Index != -1)
			{
				IExecutable executable = this.m_ui.Executable;
				ArchetypeInstanceEventsUI<T>.ExecutionCache.InitForArchetypeInstanceUI(LocalPlayer.GameEntity, this.m_instance, executable, Time.frameCount > ArchetypeInstanceEventsUI<T>.m_lastExecutionCacheFrame);
				ArchetypeInstanceEventsUI<T>.m_lastExecutionCacheFrame = Time.frameCount;
				bool flag = executable.PreExecution(ArchetypeInstanceEventsUI<T>.ExecutionCache);
				if (availableOverlay.enabled == flag)
				{
					availableOverlay.enabled = !flag;
				}
				targetOverlayState = ArchetypeInstanceEventsUI<T>.ExecutionCache.TargetOverlay;
			}
			else if (availableOverlay.enabled)
			{
				availableOverlay.enabled = false;
			}
			this.m_ui.TargetOverlay = (this.ShowTargetOverlay ? targetOverlayState : TargetOverlayState.None);
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x000731B6 File Offset: 0x000713B6
		protected virtual bool EarlyOutOfUpdate()
		{
			return !this.m_ui || !this.m_ui.AvailableOverlay || this.m_ui.Executable == null || !LocalPlayer.IsInitialized;
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x001B9248 File Offset: 0x001B7448
		protected virtual void Init(ArchetypeInstanceUI instanceUI)
		{
			this.Unsubscribe();
			this.ResetInternal();
			this.m_ui = instanceUI;
			this.m_primary.Init(this.m_ui, AbilityCooldownFlags.Regular);
			this.m_execution.Init(this.m_ui, AbilityCooldownFlags.Execution);
			this.m_global.Init(this.m_ui, AbilityCooldownFlags.Global);
			this.m_subscribeFrame = new int?(Time.frameCount + 1);
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x000731EE File Offset: 0x000713EE
		protected virtual void ResetInternal()
		{
			this.m_primary.ResetCooldown();
			this.m_execution.ResetCooldown();
			this.m_global.ResetCooldown();
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RefreshDisabledPanel()
		{
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x00073216 File Offset: 0x00071416
		protected virtual bool CooldownsActive()
		{
			return this.m_primary.IsActive || this.m_execution.IsActive || this.m_global.IsActive;
		}

		// Token: 0x06004B85 RID: 19333 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool OverrideOnPointerUp(PointerEventData eventData)
		{
			return false;
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x00073244 File Offset: 0x00071444
		protected virtual bool OverrideFillActionsGetTitle(out string result)
		{
			result = string.Empty;
			return false;
		}

		// Token: 0x06004B87 RID: 19335 RVA: 0x0007324E File Offset: 0x0007144E
		private void ExternalDestroy()
		{
			this.Unsubscribe();
			this.ResetInternal();
		}

		// Token: 0x17001098 RID: 4248
		// (get) Token: 0x06004B88 RID: 19336 RVA: 0x0007325C File Offset: 0x0007145C
		bool IArchetypeInstanceEventsUI.CanPlace
		{
			get
			{
				return this.CanPlace;
			}
		}

		// Token: 0x17001099 RID: 4249
		// (get) Token: 0x06004B89 RID: 19337 RVA: 0x00073264 File Offset: 0x00071464
		bool IArchetypeInstanceEventsUI.CanModify
		{
			get
			{
				return this.CanModify;
			}
		}

		// Token: 0x1700109A RID: 4250
		// (get) Token: 0x06004B8A RID: 19338 RVA: 0x0007326C File Offset: 0x0007146C
		bool IArchetypeInstanceEventsUI.AuraIsActive
		{
			get
			{
				return this.AuraIsActive;
			}
		}

		// Token: 0x1700109B RID: 4251
		// (get) Token: 0x06004B8B RID: 19339 RVA: 0x00073274 File Offset: 0x00071474
		bool IArchetypeInstanceEventsUI.ContextualDisabledPanel
		{
			get
			{
				return this.ContextualDisablePanel;
			}
		}

		// Token: 0x06004B8C RID: 19340 RVA: 0x0007327C File Offset: 0x0007147C
		void IArchetypeInstanceEventsUI.Init(ArchetypeInstanceUI instanceUI)
		{
			this.Init(instanceUI);
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x00073285 File Offset: 0x00071485
		void IArchetypeInstanceEventsUI.ExternalOnDestroy()
		{
			this.ExternalDestroy();
		}

		// Token: 0x06004B8E RID: 19342 RVA: 0x0007328D File Offset: 0x0007148D
		void IArchetypeInstanceEventsUI.RefreshDisabledPanel()
		{
			this.RefreshDisabledPanel();
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x00073295 File Offset: 0x00071495
		bool IArchetypeInstanceEventsUI.CooldownsActive()
		{
			return this.CooldownsActive();
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x0007329D File Offset: 0x0007149D
		bool IArchetypeInstanceEventsUI.OverrideOnPointerUp(PointerEventData eventData)
		{
			return this.OverrideOnPointerUp(eventData);
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x000732A6 File Offset: 0x000714A6
		bool IArchetypeInstanceEventsUI.OverrideFillActionsGetTitle(out string result)
		{
			return this.OverrideFillActionsGetTitle(out result);
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x000732AF File Offset: 0x000714AF
		protected void SkillsControllerOnTriggerGlobalCooldown()
		{
			this.m_global.StartCooldown(GlobalSettings.Values.Combat.GlobalCooldown);
		}

		// Token: 0x06004B93 RID: 19347 RVA: 0x001B92B8 File Offset: 0x001B74B8
		protected void SkillsControllerOnPendingExecutionChanged(SkillsController.PendingExecution obj)
		{
			if (obj.Active && obj.Instance == this.m_instance)
			{
				this.m_execution.StartCooldown(obj.ExecutionTime);
				return;
			}
			this.m_execution.ClearCooldown();
			if (obj.Instance == this.m_instance)
			{
				this.m_ui.CenterLabel.SetText(string.Empty);
			}
		}

		// Token: 0x040045ED RID: 17901
		protected const string kCooldownsGroupName = "Cooldowns";

		// Token: 0x040045EE RID: 17902
		[SerializeField]
		protected T m_primary;

		// Token: 0x040045EF RID: 17903
		[SerializeField]
		protected FixedCooldownUI m_execution;

		// Token: 0x040045F0 RID: 17904
		[SerializeField]
		protected FixedCooldownUI m_global;

		// Token: 0x040045F1 RID: 17905
		protected ArchetypeInstanceUI m_ui;

		// Token: 0x040045F2 RID: 17906
		private int? m_subscribeFrame;

		// Token: 0x040045F3 RID: 17907
		protected static int m_lastExecutionCacheFrame = -1;

		// Token: 0x040045F4 RID: 17908
		private static ExecutionCache m_executionCache = null;
	}
}
