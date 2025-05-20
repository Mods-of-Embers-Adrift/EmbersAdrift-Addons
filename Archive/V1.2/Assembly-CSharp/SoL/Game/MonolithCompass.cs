using System;
using Cysharp.Text;
using NetStack.Serialization;
using SoL.Game.Audio;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.Replication;
using SoL.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SoL.Game
{
	// Token: 0x02000553 RID: 1363
	public class MonolithCompass : SyncVarReplicator, ITooltip, IInteractiveBase
	{
		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x06002962 RID: 10594 RVA: 0x001413E4 File Offset: 0x0013F5E4
		public Vector3? CompassPos
		{
			get
			{
				if (!this.m_toManipulate)
				{
					return null;
				}
				return new Vector3?(this.m_toManipulate.position);
			}
		}

		// Token: 0x06002963 RID: 10595 RVA: 0x00141418 File Offset: 0x0013F618
		private void Awake()
		{
			if (!GameManager.IsServer)
			{
				this.m_state.Changed += this.StateOnChanged;
				this.m_splineIndex.Changed += this.IndexOnChanged;
				this.m_position.Changed += this.PositionOnChanged;
			}
		}

		// Token: 0x06002964 RID: 10596 RVA: 0x00141474 File Offset: 0x0013F674
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!GameManager.IsServer)
			{
				this.m_state.Changed -= this.StateOnChanged;
				this.m_splineIndex.Changed -= this.IndexOnChanged;
				this.m_position.Changed -= this.PositionOnChanged;
			}
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x0005CA01 File Offset: 0x0005AC01
		private void Update()
		{
			this.UpdateClient();
			this.UpdateServer();
		}

		// Token: 0x06002966 RID: 10598 RVA: 0x001414D4 File Offset: 0x0013F6D4
		public override BitBuffer ReadInitialData(BitBuffer inBuffer)
		{
			BitBuffer result = base.ReadInitialData(inBuffer);
			if (!GameManager.IsServer && this.m_toManipulate)
			{
				if (this.m_targetRot != null)
				{
					this.m_toManipulate.rotation = this.m_targetRot.Value;
					this.m_targetRot = null;
				}
				if (this.m_targetPos != null)
				{
					this.m_toManipulate.position = this.m_targetPos.Value;
					this.m_targetPos = null;
				}
				if (!this.m_activeSplineContainer)
				{
					this.IndexOnChanged(this.m_splineIndex.Value);
				}
			}
			return result;
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x0014157C File Offset: 0x0013F77C
		private void UpdateClient()
		{
			if (GameManager.IsServer || !this.m_toManipulate)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			Vector3 vector = this.m_toManipulate.position;
			if (this.m_targetPos != null)
			{
				vector = Vector3.Lerp(vector, this.m_targetPos.Value, Time.deltaTime * this.m_moveTowardsSpeed);
				if ((this.m_toManipulate.position - vector).sqrMagnitude >= 9f)
				{
					vector = this.m_targetPos.Value;
				}
				if (vector == this.m_targetPos.Value)
				{
					this.m_targetPos = null;
				}
				flag = true;
			}
			Quaternion quaternion = this.m_toManipulate.rotation;
			if (this.m_targetRot != null)
			{
				quaternion = Quaternion.RotateTowards(quaternion, this.m_targetRot.Value, Time.deltaTime * this.m_rotateTowardsSpeed);
				if (quaternion == this.m_targetRot.Value)
				{
					this.m_targetRot = null;
				}
				flag2 = true;
			}
			if (flag && flag2)
			{
				this.m_toManipulate.SetPositionAndRotation(vector, quaternion);
				return;
			}
			if (flag)
			{
				this.m_toManipulate.position = vector;
				return;
			}
			if (flag2)
			{
				this.m_toManipulate.rotation = quaternion;
			}
		}

		// Token: 0x06002968 RID: 10600 RVA: 0x0004475B File Offset: 0x0004295B
		private void UpdateServer()
		{
		}

		// Token: 0x06002969 RID: 10601 RVA: 0x001416B4 File Offset: 0x0013F8B4
		private Quaternion GetRotation()
		{
			float num;
			switch (this.m_state.Value)
			{
			case 1:
				num = 90f;
				break;
			case 2:
				num = 180f;
				break;
			case 3:
				num = 270f;
				break;
			default:
				num = 0f;
				break;
			}
			float y = num;
			return Quaternion.Euler(0f, y, 0f);
		}

		// Token: 0x0600296A RID: 10602 RVA: 0x0005CA0F File Offset: 0x0005AC0F
		private void StateOnChanged(byte obj)
		{
			if (this.m_toManipulate)
			{
				this.m_targetRot = new Quaternion?(this.GetRotation());
			}
			AudioEvent stateAudioEvent = this.m_stateAudioEvent;
			if (stateAudioEvent == null)
			{
				return;
			}
			stateAudioEvent.Play(1f);
		}

		// Token: 0x0600296B RID: 10603 RVA: 0x0005CA44 File Offset: 0x0005AC44
		private void IndexOnChanged(byte obj)
		{
			if (this.m_splines != null && (int)obj < this.m_splines.Length)
			{
				this.m_activeSplineContainer = this.m_splines[(int)obj];
			}
		}

		// Token: 0x0600296C RID: 10604 RVA: 0x00141714 File Offset: 0x0013F914
		private void PositionOnChanged(float obj)
		{
			if (this.m_activeSplineContainer && this.m_toManipulate)
			{
				float3 v = this.m_activeSplineContainer.EvaluatePosition(obj);
				this.m_targetPos = new Vector3?(v);
			}
		}

		// Token: 0x0600296D RID: 10605 RVA: 0x0014175C File Offset: 0x0013F95C
		private ITooltipParameter GetTooltipParameter()
		{
			ZoneId zoneId = (ZoneId)((LocalZoneManager.ZoneRecord != null) ? LocalZoneManager.ZoneRecord.ZoneId : 0);
			string arg = this.m_profileOverride ? this.m_profileOverride.name : LocalZoneManager.GetFormattedZoneName(zoneId, SubZoneId.None);
			string txt = ZString.Format<string, string>("{0} {1}", arg, "Ley Finder");
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x0600296E RID: 10606 RVA: 0x0005CA67 File Offset: 0x0005AC67
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x0600296F RID: 10607 RVA: 0x0005CA6F File Offset: 0x0005AC6F
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06002970 RID: 10608 RVA: 0x0005CA7D File Offset: 0x0005AC7D
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06002971 RID: 10609 RVA: 0x001417C0 File Offset: 0x0013F9C0
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_position);
			this.m_position.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_splineIndex);
			this.m_splineIndex.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_state);
			this.m_state.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x06002973 RID: 10611 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002A53 RID: 10835
		private readonly SynchronizedByte m_state = new SynchronizedByte();

		// Token: 0x04002A54 RID: 10836
		private readonly SynchronizedByte m_splineIndex = new SynchronizedByte();

		// Token: 0x04002A55 RID: 10837
		private readonly SynchronizedFloat m_position = new SynchronizedFloat();

		// Token: 0x04002A56 RID: 10838
		[SerializeField]
		private Vector2 m_moveAlongSplineSpeed = new Vector2(3f, 3f);

		// Token: 0x04002A57 RID: 10839
		[SerializeField]
		private Vector2 m_perlinScrollSpeed = new Vector2(0.1f, 0.1f);

		// Token: 0x04002A58 RID: 10840
		[SerializeField]
		private float m_moveTowardsSpeed = 30f;

		// Token: 0x04002A59 RID: 10841
		[SerializeField]
		private float m_rotateTowardsSpeed = 30f;

		// Token: 0x04002A5A RID: 10842
		[SerializeField]
		private SplineContainer[] m_splines;

		// Token: 0x04002A5B RID: 10843
		[SerializeField]
		private ZoneSettingsProfile m_profileOverride;

		// Token: 0x04002A5C RID: 10844
		[SerializeField]
		private Transform m_toManipulate;

		// Token: 0x04002A5D RID: 10845
		[SerializeField]
		private AudioEvent m_stateAudioEvent;

		// Token: 0x04002A5E RID: 10846
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04002A5F RID: 10847
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04002A60 RID: 10848
		private Vector2 m_perlinOffset;

		// Token: 0x04002A61 RID: 10849
		private float? m_splineLength = new float?(0f);

		// Token: 0x04002A62 RID: 10850
		private bool m_backward;

		// Token: 0x04002A63 RID: 10851
		private SplineContainer m_activeSplineContainer;

		// Token: 0x04002A64 RID: 10852
		private Vector3? m_targetPos;

		// Token: 0x04002A65 RID: 10853
		private Quaternion? m_targetRot;
	}
}
