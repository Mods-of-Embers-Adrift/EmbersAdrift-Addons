using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.Networking.Replication;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE1 RID: 3297
	public class InteractiveRefinementStationPortable : InteractiveRefinementStation, IRefinementStation
	{
		// Token: 0x170017EF RID: 6127
		// (get) Token: 0x060063E6 RID: 25574 RVA: 0x00083413 File Offset: 0x00081613
		string IRefinementStation.DisplayName
		{
			get
			{
				if (!(base.Profile != null))
				{
					return "Portable Crafting Station";
				}
				return base.Profile.DisplayName;
			}
		}

		// Token: 0x060063E7 RID: 25575 RVA: 0x00207D9C File Offset: 0x00205F9C
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				this.DataOnChanged(this.Data.Value);
				this.Data.Changed += this.DataOnChanged;
				return;
			}
			if (this.m_nearbyDetection)
			{
				base.InvokeRepeating("CheckNearby", 5f, 5f);
			}
		}

		// Token: 0x060063E8 RID: 25576 RVA: 0x00083434 File Offset: 0x00081634
		protected override void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.Data.Changed -= this.DataOnChanged;
			}
			else if (this.m_nearbyDetection)
			{
				base.CancelInvoke("CheckNearby");
			}
			base.OnDestroy();
		}

		// Token: 0x060063E9 RID: 25577 RVA: 0x00207DFC File Offset: 0x00205FFC
		private void CheckNearby()
		{
			if (this.m_nearbyDetection && GameManager.IsServer)
			{
				this.m_nearbyDetection.FlushNulls();
				if (this.m_nearbyDetection.CurrentlyNearby > 0 || base.CurrentInteractorCount > 0)
				{
					this.m_cumulativeTimeEmpty = 0f;
				}
				else
				{
					this.m_cumulativeTimeEmpty += Time.time - this.m_timeOfLastCheck;
				}
				this.m_timeOfLastCheck = Time.time;
				if (this.m_cumulativeTimeEmpty >= this.m_expirationTime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x060063EA RID: 25578 RVA: 0x00207E8C File Offset: 0x0020608C
		public void Initialize(GameEntity sourceEntity)
		{
			Transform transform = sourceEntity.gameObject.transform;
			Vector3 position = transform.position + transform.forward;
			base.gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);
			this.Data.Value = new PortableStationData
			{
				SourceName = sourceEntity.CharacterData.Name.Value,
				PlacementTime = DateTime.UtcNow,
				Position = position
			};
		}

		// Token: 0x060063EB RID: 25579 RVA: 0x00207F0C File Offset: 0x0020610C
		private void DataOnChanged(PortableStationData obj)
		{
			System.Random random = new System.Random((int)base.GameEntity.NetworkEntity.NetworkId.Value);
			Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
			eulerAngles.y = (float)random.NextDouble() * 360f;
			base.gameObject.transform.SetPositionAndRotation(this.Data.Value.Position, Quaternion.Euler(eulerAngles));
		}

		// Token: 0x060063EC RID: 25580 RVA: 0x00207F84 File Offset: 0x00206184
		protected override string GetTooltipText()
		{
			double totalSeconds = (GameTimeReplicator.GetServerCorrectedDateTimeUtc() - this.Data.Value.PlacementTime).TotalSeconds;
			string result = string.Empty;
			string tooltipText = base.GetTooltipText();
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append(tooltipText);
				utf16ValueStringBuilder.AppendLine();
				utf16ValueStringBuilder.AppendFormat<string, string>("Placed by {0} {1} ago.", this.Data.Value.SourceName, totalSeconds.GetFormattedTime(false));
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060063ED RID: 25581 RVA: 0x0020802C File Offset: 0x0020622C
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.Data);
			this.Data.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x040056BF RID: 22207
		private const string kDefaultDisplayName = "Portable Crafting Station";

		// Token: 0x040056C0 RID: 22208
		private const float kCheckCadence = 5f;

		// Token: 0x040056C1 RID: 22209
		[SerializeField]
		private float m_expirationTime = 300f;

		// Token: 0x040056C2 RID: 22210
		[SerializeField]
		private NearbyDetection m_nearbyDetection;

		// Token: 0x040056C3 RID: 22211
		private readonly SynchronizedStruct<PortableStationData> Data = new SynchronizedStruct<PortableStationData>();

		// Token: 0x040056C4 RID: 22212
		private float m_cumulativeTimeEmpty;

		// Token: 0x040056C5 RID: 22213
		private float m_timeOfLastCheck;
	}
}
