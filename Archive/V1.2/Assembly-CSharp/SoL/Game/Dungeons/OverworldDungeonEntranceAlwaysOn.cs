using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C95 RID: 3221
	public class OverworldDungeonEntranceAlwaysOn : SyncVarReplicator, ITooltip, IInteractiveBase
	{
		// Token: 0x17001762 RID: 5986
		// (get) Token: 0x060061C6 RID: 25030 RVA: 0x00081E6F File Offset: 0x0008006F
		internal SubZoneId SubZone
		{
			get
			{
				return this.m_subZoneId;
			}
		}

		// Token: 0x060061C7 RID: 25031 RVA: 0x00201A44 File Offset: 0x001FFC44
		private void Start()
		{
			if (GameManager.IsServer)
			{
				this.m_cyclePosCo = this.CyclePositionCo();
				base.StartCoroutine(this.m_cyclePosCo);
				return;
			}
			if (base.GameEntity && base.GameEntity.NetworkEntity)
			{
				if (base.GameEntity.NetworkEntity.IsInitialized)
				{
					this.Initialize();
					return;
				}
				base.GameEntity.NetworkEntity.OnStartClient += this.NetworkEntityOnOnStartClient;
			}
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x00201AC8 File Offset: 0x001FFCC8
		protected override void OnDestroy()
		{
			if (GameManager.IsServer)
			{
				if (this.m_cyclePosCo != null)
				{
					base.StopCoroutine(this.m_cyclePosCo);
					this.m_cyclePosCo = null;
				}
			}
			else
			{
				this.m_location.Changed -= this.LocationOnChanged;
			}
			base.OnDestroy();
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x00081E77 File Offset: 0x00080077
		private void NetworkEntityOnOnStartClient()
		{
			base.GameEntity.NetworkEntity.OnStartClient -= this.NetworkEntityOnOnStartClient;
			this.Initialize();
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x00081E9B File Offset: 0x0008009B
		private void Initialize()
		{
			this.m_location.Changed += this.LocationOnChanged;
			this.LocationOnChanged(this.m_location.Value);
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x00201B18 File Offset: 0x001FFD18
		private void LocationOnChanged(CharacterLocation obj)
		{
			if (!GameManager.IsServer && this.m_location.Value != null)
			{
				if (this.m_visuals)
				{
					this.m_visuals.SetActive(false);
				}
				base.gameObject.transform.SetPositionAndRotation(this.m_location.Value.GetPosition(), this.m_location.Value.GetRotation());
				if (this.m_visuals)
				{
					this.m_visuals.SetActive(true);
				}
				if (this.m_zonePoint)
				{
					this.m_zonePoint.RefreshBounds(true);
				}
			}
		}

		// Token: 0x060061CC RID: 25036 RVA: 0x00081EC5 File Offset: 0x000800C5
		private IEnumerator CyclePositionCo()
		{
			bool isServer = GameManager.IsServer;
			yield break;
		}

		// Token: 0x060061CD RID: 25037 RVA: 0x0004475B File Offset: 0x0004295B
		private void MoveToNextPoint()
		{
		}

		// Token: 0x060061CE RID: 25038 RVA: 0x0020052C File Offset: 0x001FE72C
		private string GetMinutesString(float seconds)
		{
			return (seconds / 60f).ToString("0.##", CultureInfo.InvariantCulture) + " minutes";
		}

		// Token: 0x060061CF RID: 25039 RVA: 0x0004475B File Offset: 0x0004295B
		internal void RegisterSpawnController(SpawnControllerMovable spawnController)
		{
		}

		// Token: 0x060061D0 RID: 25040 RVA: 0x00201BB8 File Offset: 0x001FFDB8
		public void OnDrawGizmosSelected()
		{
			if (this.m_points == null)
			{
				return;
			}
			for (int i = 0; i < this.m_points.Length; i++)
			{
				this.DrawLineToPoint(this.m_points[i]);
			}
		}

		// Token: 0x060061D1 RID: 25041 RVA: 0x00081ECD File Offset: 0x000800CD
		public void DrawGizmosForSelected(GameObject point)
		{
			this.OnDrawGizmosSelected();
		}

		// Token: 0x060061D2 RID: 25042 RVA: 0x00201BF0 File Offset: 0x001FFDF0
		private void DrawLineToPoint(GameObject point)
		{
			if (!point || !point.activeSelf)
			{
				return;
			}
			Gizmos.DrawCube(point.transform.position, Vector3.one * 0.25f);
			Gizmos.DrawLine(point.transform.position, base.gameObject.transform.position);
		}

		// Token: 0x060061D3 RID: 25043 RVA: 0x00201C50 File Offset: 0x001FFE50
		private ITooltipParameter GetTooltipParameter()
		{
			string formattedZoneName = LocalZoneManager.GetFormattedZoneName(this.m_zonePoint.TargetZone, this.m_subZoneId);
			double num = (this.m_nextMoveTime.Value - GameTimeReplicator.GetServerCorrectedDateTimeUtc()).TotalSeconds;
			if (num < 0.0)
			{
				num = 0.0;
			}
			string txt = ZString.Format<string, string>("Zone to {0}\nCollapse in approximately {1}", formattedZoneName, num.GetFormattedTime(false));
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17001763 RID: 5987
		// (get) Token: 0x060061D4 RID: 25044 RVA: 0x00081ED5 File Offset: 0x000800D5
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x17001764 RID: 5988
		// (get) Token: 0x060061D5 RID: 25045 RVA: 0x00081EDD File Offset: 0x000800DD
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001765 RID: 5989
		// (get) Token: 0x060061D6 RID: 25046 RVA: 0x00081EEB File Offset: 0x000800EB
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060061D7 RID: 25047 RVA: 0x00201CC8 File Offset: 0x001FFEC8
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_location);
			this.m_location.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_nextMoveTime);
			this.m_nextMoveTime.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400554B RID: 21835
		[SerializeField]
		private ZonePointTrigger m_zonePoint;

		// Token: 0x0400554C RID: 21836
		[SerializeField]
		private GameObject m_visuals;

		// Token: 0x0400554D RID: 21837
		[SerializeField]
		private GameObject[] m_points;

		// Token: 0x0400554E RID: 21838
		[SerializeField]
		private SubZoneId m_subZoneId;

		// Token: 0x0400554F RID: 21839
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04005550 RID: 21840
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04005551 RID: 21841
		private readonly SynchronizedLocation m_location = new SynchronizedLocation();

		// Token: 0x04005552 RID: 21842
		private readonly SynchronizedDateTime m_nextMoveTime = new SynchronizedDateTime();

		// Token: 0x04005553 RID: 21843
		private IEnumerator m_cyclePosCo;

		// Token: 0x04005554 RID: 21844
		private SpawnControllerMovable m_spawnController;

		// Token: 0x04005555 RID: 21845
		private WaitForSeconds m_wait;

		// Token: 0x04005556 RID: 21846
		private List<GameObject> m_pointList;

		// Token: 0x04005557 RID: 21847
		private int m_pointListIndex;
	}
}
