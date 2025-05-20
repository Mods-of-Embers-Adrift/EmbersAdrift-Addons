using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B7C RID: 2940
	[Serializable]
	public class InteractionSettings : InteractionSettingsBase
	{
		// Token: 0x1700151B RID: 5403
		// (get) Token: 0x06005A8E RID: 23182 RVA: 0x0007CC67 File Offset: 0x0007AE67
		private bool m_showExtension
		{
			get
			{
				return this.m_distanceSource == InteractionSettings.SourceType.Global && this.m_extendGlobalDistance;
			}
		}

		// Token: 0x1700151C RID: 5404
		// (get) Token: 0x06005A8F RID: 23183 RVA: 0x0007CC79 File Offset: 0x0007AE79
		protected override bool m_showIgnoreDistanceCheck
		{
			get
			{
				return this.m_distanceSource == InteractionSettings.SourceType.Manual;
			}
		}

		// Token: 0x1700151D RID: 5405
		// (get) Token: 0x06005A90 RID: 23184 RVA: 0x0007CC84 File Offset: 0x0007AE84
		protected override bool m_showManualDistance
		{
			get
			{
				return this.m_distanceSource == InteractionSettings.SourceType.Manual && !this.m_ignoreDistanceCheck;
			}
		}

		// Token: 0x1700151E RID: 5406
		// (get) Token: 0x06005A91 RID: 23185 RVA: 0x001ED1B4 File Offset: 0x001EB3B4
		public override float DistanceValue
		{
			get
			{
				InteractionSettings.SourceType distanceSource = this.m_distanceSource;
				if (distanceSource == InteractionSettings.SourceType.Global || distanceSource != InteractionSettings.SourceType.Manual)
				{
					float num = GlobalSettings.Values.General.GlobalInteractionSettings.DistanceValue;
					if (this.m_extendGlobalDistance)
					{
						num += this.m_extendGlobalDistanceValue;
					}
					return num;
				}
				return base.DistanceValue;
			}
		}

		// Token: 0x1700151F RID: 5407
		// (get) Token: 0x06005A92 RID: 23186 RVA: 0x0007CC9A File Offset: 0x0007AE9A
		public override InteractionManager.MouseButtonType InteractionMouseButton
		{
			get
			{
				if (this.m_interactionSource != InteractionSettings.SourceType.Global)
				{
					return base.InteractionMouseButton;
				}
				return GlobalSettings.Values.General.GlobalInteractionSettings.InteractionMouseButton;
			}
		}

		// Token: 0x17001520 RID: 5408
		// (get) Token: 0x06005A93 RID: 23187 RVA: 0x0007CCBF File Offset: 0x0007AEBF
		public override bool DoubleClickInteraction
		{
			get
			{
				if (this.m_interactionSource != InteractionSettings.SourceType.Global)
				{
					return base.DoubleClickInteraction;
				}
				return GlobalSettings.Values.General.GlobalInteractionSettings.DoubleClickInteraction;
			}
		}

		// Token: 0x17001521 RID: 5409
		// (get) Token: 0x06005A94 RID: 23188 RVA: 0x0007CCE4 File Offset: 0x0007AEE4
		protected override bool m_showInteractionOptions
		{
			get
			{
				return this.m_interactionSource == InteractionSettings.SourceType.Manual;
			}
		}

		// Token: 0x17001522 RID: 5410
		// (get) Token: 0x06005A95 RID: 23189 RVA: 0x0007CCEF File Offset: 0x0007AEEF
		public override InteractionManager.MouseButtonType ContextMouseButton
		{
			get
			{
				if (this.m_contextSource != InteractionSettings.SourceType.Global)
				{
					return base.ContextMouseButton;
				}
				return GlobalSettings.Values.General.GlobalInteractionSettings.ContextMouseButton;
			}
		}

		// Token: 0x17001523 RID: 5411
		// (get) Token: 0x06005A96 RID: 23190 RVA: 0x0007CD14 File Offset: 0x0007AF14
		public override bool DoubleClickContext
		{
			get
			{
				if (this.m_contextSource != InteractionSettings.SourceType.Global)
				{
					return base.DoubleClickContext;
				}
				return GlobalSettings.Values.General.GlobalInteractionSettings.DoubleClickContext;
			}
		}

		// Token: 0x17001524 RID: 5412
		// (get) Token: 0x06005A97 RID: 23191 RVA: 0x0007CD39 File Offset: 0x0007AF39
		protected override bool m_showContextOptions
		{
			get
			{
				return this.m_contextSource == InteractionSettings.SourceType.Manual;
			}
		}

		// Token: 0x04004F75 RID: 20341
		[SerializeField]
		private InteractionSettings.SourceType m_distanceSource;

		// Token: 0x04004F76 RID: 20342
		private const string kExtension = "Distance/Extension";

		// Token: 0x04004F77 RID: 20343
		[SerializeField]
		private bool m_extendGlobalDistance;

		// Token: 0x04004F78 RID: 20344
		[SerializeField]
		private float m_extendGlobalDistanceValue;

		// Token: 0x04004F79 RID: 20345
		[SerializeField]
		private InteractionSettings.SourceType m_interactionSource;

		// Token: 0x04004F7A RID: 20346
		[SerializeField]
		private InteractionSettings.SourceType m_contextSource;

		// Token: 0x02000B7D RID: 2941
		private enum SourceType
		{
			// Token: 0x04004F7C RID: 20348
			Global,
			// Token: 0x04004F7D RID: 20349
			Manual
		}
	}
}
