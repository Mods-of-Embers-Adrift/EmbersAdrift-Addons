using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A66 RID: 2662
	public class GroundTorch : GameEntityComponent, ITooltip, IInteractiveBase, ICursor
	{
		// Token: 0x170012B1 RID: 4785
		// (get) Token: 0x06005272 RID: 21106 RVA: 0x0007707B File Offset: 0x0007527B
		private bool m_showVerticalRange
		{
			get
			{
				return this.m_visuals != null;
			}
		}

		// Token: 0x170012B2 RID: 4786
		// (get) Token: 0x06005273 RID: 21107 RVA: 0x00077089 File Offset: 0x00075289
		// (set) Token: 0x06005274 RID: 21108 RVA: 0x00077091 File Offset: 0x00075291
		public GameEntity CurrentSource { get; private set; }

		// Token: 0x170012B3 RID: 4787
		// (get) Token: 0x06005275 RID: 21109 RVA: 0x0007709A File Offset: 0x0007529A
		public GroundTorchData TorchData
		{
			get
			{
				return this.m_replicator.Data;
			}
		}

		// Token: 0x06005276 RID: 21110 RVA: 0x001D3EB8 File Offset: 0x001D20B8
		private void Start()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			if (this.m_replicator != null)
			{
				this.DataOnChanged(this.m_replicator.Data);
				this.m_replicator.Data.Changed += this.DataOnChanged;
			}
		}

		// Token: 0x06005277 RID: 21111 RVA: 0x000770AC File Offset: 0x000752AC
		private void OnDestroy()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			if (this.m_replicator != null)
			{
				this.m_replicator.Data.Changed -= this.DataOnChanged;
			}
			this.CurrentSource = null;
		}

		// Token: 0x06005278 RID: 21112 RVA: 0x001D3F10 File Offset: 0x001D2110
		public void Initialize(GameEntity sourceEntity, int duration)
		{
			this.CurrentSource = sourceEntity;
			this.CurrentSource.GroundTorch = this;
			base.gameObject.transform.position = sourceEntity.gameObject.transform.position + sourceEntity.gameObject.transform.forward;
			DateTime expirationTime = DateTime.UtcNow.AddSeconds((double)duration);
			this.m_replicator.Data.Value = new GroundTorchData
			{
				SourceName = sourceEntity.CharacterData.Name.Value,
				Duration = duration,
				ExpirationTime = expirationTime,
				Position = base.gameObject.transform.position
			};
		}

		// Token: 0x06005279 RID: 21113 RVA: 0x001D3FCC File Offset: 0x001D21CC
		private void Update()
		{
			if (!GameManager.IsServer && this.m_visuals)
			{
				float num = (float)this.m_replicator.Data.Value.Duration;
				TimeSpan timeSpan = this.m_replicator.Data.Value.ExpirationTime - GameTimeReplicator.GetServerCorrectedDateTime(DateTime.UtcNow);
				float num2 = (num - (float)timeSpan.TotalSeconds) / num;
				if (num2 >= 0.8f)
				{
					num *= 0.19999999f;
					num2 = (num - (float)timeSpan.TotalSeconds) / num;
					Vector3 localPosition = this.m_visuals.localPosition;
					localPosition.y = Mathf.Lerp(this.m_verticalRange.x, this.m_verticalRange.y, num2);
					this.m_visuals.localPosition = localPosition;
					return;
				}
				if (this.m_visuals.localPosition.y < this.m_verticalRange.x)
				{
					Vector3 localPosition2 = this.m_visuals.localPosition;
					localPosition2.y = this.m_verticalRange.x;
					this.m_visuals.localPosition = localPosition2;
				}
			}
		}

		// Token: 0x0600527A RID: 21114 RVA: 0x001D40E0 File Offset: 0x001D22E0
		private void DataOnChanged(GroundTorchData obj)
		{
			System.Random random = new System.Random((int)base.GameEntity.NetworkEntity.NetworkId.Value);
			Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
			eulerAngles.y = (float)random.NextDouble() * 360f;
			base.gameObject.transform.SetPositionAndRotation(this.m_replicator.Data.Value.Position, Quaternion.Euler(eulerAngles));
		}

		// Token: 0x0600527B RID: 21115 RVA: 0x001D415C File Offset: 0x001D235C
		private ITooltipParameter GetTooltipParameter()
		{
			string formattedTime = (this.m_replicator.Data.Value.ExpirationTime - GameTimeReplicator.GetServerCorrectedDateTime(DateTime.UtcNow)).TotalSeconds.GetFormattedTime(true);
			return new ObjectTextTooltipParameter(this, this.m_replicator.Data.Value.SourceName + "'s ground torch.\nExpires in " + formattedTime, false);
		}

		// Token: 0x170012B4 RID: 4788
		// (get) Token: 0x0600527C RID: 21116 RVA: 0x000770E7 File Offset: 0x000752E7
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170012B5 RID: 4789
		// (get) Token: 0x0600527D RID: 21117 RVA: 0x000770F5 File Offset: 0x000752F5
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170012B6 RID: 4790
		// (get) Token: 0x0600527E RID: 21118 RVA: 0x00070E66 File Offset: 0x0006F066
		CursorType ICursor.Type
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170012B7 RID: 4791
		// (get) Token: 0x0600527F RID: 21119 RVA: 0x000770FD File Offset: 0x000752FD
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x06005281 RID: 21121 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040049C1 RID: 18881
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x040049C2 RID: 18882
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040049C3 RID: 18883
		[SerializeField]
		private GroundTorchSyncVarReplicator m_replicator;

		// Token: 0x040049C4 RID: 18884
		[SerializeField]
		private Transform m_visuals;

		// Token: 0x040049C5 RID: 18885
		[SerializeField]
		private Vector2 m_verticalRange = new Vector2(-0.1f, -1.4f);
	}
}
