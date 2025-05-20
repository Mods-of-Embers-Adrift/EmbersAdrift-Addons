using System;
using System.Collections;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000536 RID: 1334
	public class ActiveZoneRecordUpdater : MonoBehaviour
	{
		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06002880 RID: 10368 RVA: 0x0005C236 File Offset: 0x0005A436
		public static string ActiveZoneRecordId
		{
			get
			{
				ActiveZoneRecord record = ActiveZoneRecordUpdater.m_record;
				if (record == null)
				{
					return null;
				}
				return record.Id;
			}
		}

		// Token: 0x06002881 RID: 10369 RVA: 0x0005C248 File Offset: 0x0005A448
		private IEnumerator Start()
		{
			while (ServerNetworkManager.Port == 0)
			{
				yield return null;
			}
			ActiveZoneRecordUpdater.m_record = ActiveZoneRecord.StoreNew(ExternalGameDatabase.Database, LocalZoneManager.ZoneRecord, ServerNetworkManager.InstanceId);
			base.InvokeRepeating("UpdateActiveZoneRecord", 10f, 10f);
			yield break;
		}

		// Token: 0x06002882 RID: 10370 RVA: 0x0005C257 File Offset: 0x0005A457
		private void OnDestroy()
		{
			base.CancelInvoke("UpdateActiveZoneRecord");
			ActiveZoneRecord record = ActiveZoneRecordUpdater.m_record;
			if (record != null)
			{
				record.Delete(ExternalGameDatabase.Database);
			}
			ActiveZoneRecordUpdater.m_record = null;
		}

		// Token: 0x06002883 RID: 10371 RVA: 0x0005C27F File Offset: 0x0005A47F
		private void UpdateActiveZoneRecord()
		{
			ActiveZoneRecord record = ActiveZoneRecordUpdater.m_record;
			if (record == null)
			{
				return;
			}
			record.UpdateZoneState(ExternalGameDatabase.Database);
		}

		// Token: 0x040029B5 RID: 10677
		private const float kUpdateRate = 10f;

		// Token: 0x040029B6 RID: 10678
		private static ActiveZoneRecord m_record;
	}
}
