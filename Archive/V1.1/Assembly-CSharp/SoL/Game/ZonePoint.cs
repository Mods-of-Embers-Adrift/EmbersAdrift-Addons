using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Audio;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000617 RID: 1559
	public abstract class ZonePoint : TargetPosition
	{
		// Token: 0x06003173 RID: 12659 RVA: 0x0015CC10 File Offset: 0x0015AE10
		protected static int RegisterZoneRequest(ZonePoint zp)
		{
			if (ZonePoint.m_zonePointLookup == null)
			{
				ZonePoint.m_zonePointLookup = new Dictionary<int, ZonePoint>(10);
			}
			int instanceID = zp.GetInstanceID();
			ZonePoint.m_zonePointLookup.AddOrReplace(instanceID, zp);
			return instanceID;
		}

		// Token: 0x06003174 RID: 12660 RVA: 0x0015CC44 File Offset: 0x0015AE44
		public static void ReceiveZoneResponse(int zonePointInstanceId, bool result)
		{
			if (zonePointInstanceId == -1)
			{
				return;
			}
			ZonePoint zonePoint;
			if (ZonePoint.m_zonePointLookup != null && ZonePoint.m_zonePointLookup.TryGetValue(zonePointInstanceId, out zonePoint))
			{
				if (zonePoint)
				{
					zonePoint.ZoneRequestResponseReceived(result);
				}
				ZonePoint.m_zonePointLookup.Remove(zonePointInstanceId);
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06003175 RID: 12661 RVA: 0x000621BE File Offset: 0x000603BE
		public ZoneId TargetZone
		{
			get
			{
				return this.m_targetZone;
			}
		}

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x06003176 RID: 12662 RVA: 0x000621C6 File Offset: 0x000603C6
		public int TargetZonePointIndex
		{
			get
			{
				return this.m_targetZonePointIndex;
			}
		}

		// Token: 0x06003177 RID: 12663 RVA: 0x000621CE File Offset: 0x000603CE
		private void OnEnable()
		{
			this.Register();
		}

		// Token: 0x06003178 RID: 12664 RVA: 0x000621D6 File Offset: 0x000603D6
		private void OnDisable()
		{
			this.Deregister();
		}

		// Token: 0x06003179 RID: 12665 RVA: 0x0015CC88 File Offset: 0x0015AE88
		protected override void OnValidate()
		{
			base.OnValidate();
			if (this.m_label)
			{
				this.m_label.text = this.m_targetZone.ToString() + " (" + this.m_targetZonePointIndex.ToString() + ")";
			}
		}

		// Token: 0x0600317A RID: 12666 RVA: 0x000621DE File Offset: 0x000603DE
		private void Register()
		{
			if (this.m_registered)
			{
				return;
			}
			this.m_registered = LocalZoneManager.RegisterZonePoint(this);
		}

		// Token: 0x0600317B RID: 12667 RVA: 0x000621F5 File Offset: 0x000603F5
		private void Deregister()
		{
			if (!this.m_registered)
			{
				return;
			}
			LocalZoneManager.DeregisterZonePoint(this);
			this.m_registered = false;
		}

		// Token: 0x0600317C RID: 12668 RVA: 0x0015CCE0 File Offset: 0x0015AEE0
		public virtual bool IsWithinRange(GameEntity entity)
		{
			return entity && entity.gameObject && Vector3.Distance(entity.gameObject.transform.position, base.gameObject.transform.position) <= 4f;
		}

		// Token: 0x0600317D RID: 12669 RVA: 0x0004479C File Offset: 0x0004299C
		public virtual bool ServerCanEntityInteract(GameEntity entity)
		{
			return true;
		}

		// Token: 0x0600317E RID: 12670 RVA: 0x0006220D File Offset: 0x0006040D
		public void SetZoneData(ZoneId targetZone, int targetZoneIndex)
		{
			this.Deregister();
			this.m_targetZone = targetZone;
			this.m_targetZonePointIndex = targetZoneIndex;
			this.Register();
		}

		// Token: 0x0600317F RID: 12671 RVA: 0x0015CD34 File Offset: 0x0015AF34
		private void ZoneRequestResponseReceived(bool result)
		{
			if (!GameManager.IsServer)
			{
				ClientGameManager.UIManager.PlayRandomClip(result ? this.m_successClips : this.m_failureClips, null);
			}
		}

		// Token: 0x06003180 RID: 12672 RVA: 0x0015CD6C File Offset: 0x0015AF6C
		protected bool LocalPlayerIsAlive(bool notify = false)
		{
			HealthState healthState = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.Vitals) ? LocalPlayer.GameEntity.Vitals.GetCurrentHealthState() : HealthState.None;
			bool flag = healthState == HealthState.Alive;
			if (!flag && notify)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot zone while " + healthState.ToString() + "!");
			}
			return flag;
		}

		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x06003181 RID: 12673 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetAudioClipCollections
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
			}
		}

		// Token: 0x04002FD7 RID: 12247
		public const int kNullZonePointInstanceId = -1;

		// Token: 0x04002FD8 RID: 12248
		private static Dictionary<int, ZonePoint> m_zonePointLookup;

		// Token: 0x04002FD9 RID: 12249
		private const string kAudioGroup = "Audio";

		// Token: 0x04002FDA RID: 12250
		private const float kDistanceThreshold = 4f;

		// Token: 0x04002FDB RID: 12251
		[SerializeField]
		protected ZoneId m_targetZone;

		// Token: 0x04002FDC RID: 12252
		[SerializeField]
		protected int m_targetZonePointIndex;

		// Token: 0x04002FDD RID: 12253
		[SerializeField]
		private TextMeshPro m_label;

		// Token: 0x04002FDE RID: 12254
		[SerializeField]
		private AudioClipCollection m_successClips;

		// Token: 0x04002FDF RID: 12255
		[SerializeField]
		private AudioClipCollection m_failureClips;

		// Token: 0x04002FE0 RID: 12256
		private bool m_registered;
	}
}
