using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.SkyDome;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002B5 RID: 693
	public class RandomFireworksController : MonoBehaviour
	{
		// Token: 0x06001498 RID: 5272 RVA: 0x000505EA File Offset: 0x0004E7EA
		private string GetDateTimeRestriction()
		{
			return this.m_dateTimespanRestriction.GetStartEndDateTimeString();
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x000FB130 File Offset: 0x000F9330
		private void Awake()
		{
			if (!GameManager.IsServer || !GameManager.IsOnline || this.m_positions == null || this.m_positions.Length == 0 || this.m_fireworks == null || this.m_fireworks.Length == 0)
			{
				base.enabled = false;
				return;
			}
			if (RandomFireworksController.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			RandomFireworksController.Instance = this;
			this.m_locations = new List<RandomFireworksController.FireworksLocation>(this.m_positions.Length);
			for (int i = 0; i < this.m_positions.Length; i++)
			{
				if (this.m_positions[i])
				{
					this.m_locations.Add(new RandomFireworksController.FireworksLocation(this, this.m_positions[i]));
				}
			}
			this.m_wasDay = SkyDomeManager.IsDay();
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x000505F7 File Offset: 0x0004E7F7
		private void Start()
		{
			if (!this.m_bypassPeriodicFire)
			{
				base.InvokeRepeating("UpdateInternal", 10f, 10f);
			}
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x000FB1F0 File Offset: 0x000F93F0
		private void UpdateInternal()
		{
			if (this.m_restrictByDate && !this.m_dateTimespanRestriction.WithinTimespan())
			{
				return;
			}
			bool flag = SkyDomeManager.IsDay();
			if (flag != this.m_wasDay && !flag)
			{
				this.RefreshAllCooldowns();
			}
			this.m_wasDay = flag;
			if (!flag)
			{
				float time = Time.time;
				for (int i = 0; i < this.m_locations.Count; i++)
				{
					if (this.m_locations[i] != null && this.m_locations[i].TimeOfNextTrigger < time)
					{
						this.m_locations[i].Trigger(true);
					}
				}
			}
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x000FB288 File Offset: 0x000F9488
		private void RefreshAllCooldowns()
		{
			for (int i = 0; i < this.m_locations.Count; i++)
			{
				if (this.m_locations[i] != null)
				{
					this.m_locations[i].IncreaseTime();
				}
			}
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x000FB2CC File Offset: 0x000F94CC
		public void TriggerAllFireworks()
		{
			for (int i = 0; i < this.m_locations.Count; i++)
			{
				if (this.m_locations[i] != null)
				{
					this.m_locations[i].Trigger(false);
				}
			}
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x000FB310 File Offset: 0x000F9510
		private void OnDrawGizmosSelected()
		{
			if (this.m_positions != null)
			{
				for (int i = 0; i < this.m_positions.Length; i++)
				{
					if (this.m_positions[i])
					{
						Gizmos.DrawWireSphere(this.m_positions[i].position, 0.5f);
					}
				}
			}
		}

		// Token: 0x04001CD6 RID: 7382
		public static RandomFireworksController Instance;

		// Token: 0x04001CD7 RID: 7383
		[SerializeField]
		private MinMaxFloatRange m_cooldown = new MinMaxFloatRange(60f, 600f);

		// Token: 0x04001CD8 RID: 7384
		[SerializeField]
		private ConsumableItemUtility[] m_fireworks;

		// Token: 0x04001CD9 RID: 7385
		[SerializeField]
		private Transform[] m_positions;

		// Token: 0x04001CDA RID: 7386
		[SerializeField]
		private bool m_bypassPeriodicFire;

		// Token: 0x04001CDB RID: 7387
		[SerializeField]
		private bool m_restrictByDate;

		// Token: 0x04001CDC RID: 7388
		[SerializeField]
		private SerializedDateTimespan m_dateTimespanRestriction;

		// Token: 0x04001CDD RID: 7389
		private List<RandomFireworksController.FireworksLocation> m_locations;

		// Token: 0x04001CDE RID: 7390
		private bool m_wasDay;

		// Token: 0x020002B6 RID: 694
		private class FireworksLocation
		{
			// Token: 0x1700050C RID: 1292
			// (get) Token: 0x060014A0 RID: 5280 RVA: 0x00050633 File Offset: 0x0004E833
			// (set) Token: 0x060014A1 RID: 5281 RVA: 0x0005063B File Offset: 0x0004E83B
			public float TimeOfNextTrigger { get; private set; }

			// Token: 0x060014A2 RID: 5282 RVA: 0x000FB360 File Offset: 0x000F9560
			public FireworksLocation(RandomFireworksController controller, Transform trans)
			{
				if (!controller)
				{
					throw new ArgumentNullException("controller");
				}
				if (!trans)
				{
					throw new ArgumentNullException("trans");
				}
				this.m_controller = controller;
				this.m_transform = trans;
				this.IncreaseTime();
			}

			// Token: 0x060014A3 RID: 5283 RVA: 0x00050644 File Offset: 0x0004E844
			public void IncreaseTime()
			{
				this.TimeOfNextTrigger = Time.time + this.m_controller.m_cooldown.RandomWithinRange();
			}

			// Token: 0x060014A4 RID: 5284 RVA: 0x000FB3B0 File Offset: 0x000F95B0
			public void Trigger(bool increaseTime)
			{
				int num = UnityEngine.Random.Range(0, this.m_controller.m_fireworks.Length);
				ConsumableItemUtility consumableItemUtility = this.m_controller.m_fireworks[num];
				if (consumableItemUtility)
				{
					LocationEvent locationEvent = new LocationEvent
					{
						ArchetypeId = consumableItemUtility.Id,
						Location = this.m_transform.position
					};
					ServerGameManager.SpatialManager.LocationEventAllPlayers(null, locationEvent);
					if (increaseTime)
					{
						this.IncreaseTime();
					}
				}
			}

			// Token: 0x04001CDF RID: 7391
			private readonly RandomFireworksController m_controller;

			// Token: 0x04001CE0 RID: 7392
			private readonly Transform m_transform;
		}
	}
}
