using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SoL.Game;
using SoL.Networking.Database;

namespace SoL.Utilities
{
	// Token: 0x02000280 RID: 640
	public static class ZoneRecordExtensions
	{
		// Token: 0x060013E4 RID: 5092 RVA: 0x000F8394 File Offset: 0x000F6594
		private static Task<Dictionary<ZoneId, ZoneRecord>> GetZoneRecordDict()
		{
			ZoneRecordExtensions.<GetZoneRecordDict>d__1 <GetZoneRecordDict>d__;
			<GetZoneRecordDict>d__.<>t__builder = AsyncTaskMethodBuilder<Dictionary<ZoneId, ZoneRecord>>.Create();
			<GetZoneRecordDict>d__.<>1__state = -1;
			<GetZoneRecordDict>d__.<>t__builder.Start<ZoneRecordExtensions.<GetZoneRecordDict>d__1>(ref <GetZoneRecordDict>d__);
			return <GetZoneRecordDict>d__.<>t__builder.Task;
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x000F83D0 File Offset: 0x000F65D0
		private static Task<ZoneRecord> GetZoneRecordForId(ZoneId zoneId)
		{
			ZoneRecordExtensions.<GetZoneRecordForId>d__2 <GetZoneRecordForId>d__;
			<GetZoneRecordForId>d__.<>t__builder = AsyncTaskMethodBuilder<ZoneRecord>.Create();
			<GetZoneRecordForId>d__.zoneId = zoneId;
			<GetZoneRecordForId>d__.<>1__state = -1;
			<GetZoneRecordForId>d__.<>t__builder.Start<ZoneRecordExtensions.<GetZoneRecordForId>d__2>(ref <GetZoneRecordForId>d__);
			return <GetZoneRecordForId>d__.<>t__builder.Task;
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x000F8414 File Offset: 0x000F6614
		public static Task<ActiveZoneRecord> GetActiveZoneRecordForId(ZoneId zoneId)
		{
			ZoneRecordExtensions.<GetActiveZoneRecordForId>d__3 <GetActiveZoneRecordForId>d__;
			<GetActiveZoneRecordForId>d__.<>t__builder = AsyncTaskMethodBuilder<ActiveZoneRecord>.Create();
			<GetActiveZoneRecordForId>d__.zoneId = zoneId;
			<GetActiveZoneRecordForId>d__.<>1__state = -1;
			<GetActiveZoneRecordForId>d__.<>t__builder.Start<ZoneRecordExtensions.<GetActiveZoneRecordForId>d__3>(ref <GetActiveZoneRecordForId>d__);
			return <GetActiveZoneRecordForId>d__.<>t__builder.Task;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x000F8458 File Offset: 0x000F6658
		public static Task<List<ActiveZoneRecord>> GetActiveZoneRecordsForId(ZoneId zoneId)
		{
			ZoneRecordExtensions.<GetActiveZoneRecordsForId>d__4 <GetActiveZoneRecordsForId>d__;
			<GetActiveZoneRecordsForId>d__.<>t__builder = AsyncTaskMethodBuilder<List<ActiveZoneRecord>>.Create();
			<GetActiveZoneRecordsForId>d__.zoneId = zoneId;
			<GetActiveZoneRecordsForId>d__.<>1__state = -1;
			<GetActiveZoneRecordsForId>d__.<>t__builder.Start<ZoneRecordExtensions.<GetActiveZoneRecordsForId>d__4>(ref <GetActiveZoneRecordsForId>d__);
			return <GetActiveZoneRecordsForId>d__.<>t__builder.Task;
		}

		// Token: 0x04001C0F RID: 7183
		private static Dictionary<ZoneId, ZoneRecord> m_zoneRecords;
	}
}
