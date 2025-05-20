using System;
using UnityEngine;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004AC RID: 1196
	[Serializable]
	public class DistanceBand : IEquatable<DistanceBand>
	{
		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06002159 RID: 8537 RVA: 0x000581E4 File Offset: 0x000563E4
		public float Distance
		{
			get
			{
				return this.m_distance;
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x0600215A RID: 8538 RVA: 0x000581EC File Offset: 0x000563EC
		public float UpdateTime
		{
			get
			{
				return this.m_updateTime;
			}
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x0600215B RID: 8539 RVA: 0x000581F4 File Offset: 0x000563F4
		// (set) Token: 0x0600215C RID: 8540 RVA: 0x000581FC File Offset: 0x000563FC
		public int Index { get; set; }

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x0600215D RID: 8541 RVA: 0x00058205 File Offset: 0x00056405
		// (set) Token: 0x0600215E RID: 8542 RVA: 0x0005820D File Offset: 0x0005640D
		public int UpdateCount { get; set; }

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x0600215F RID: 8543 RVA: 0x00058216 File Offset: 0x00056416
		public bool HasObservers
		{
			get
			{
				return this.Index == 0 || this.Observers.Count > 0;
			}
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06002160 RID: 8544 RVA: 0x00058230 File Offset: 0x00056430
		public DictionaryList<int, Observer> Observers
		{
			get
			{
				if (this.m_observers == null)
				{
					this.m_observers = new DictionaryList<int, Observer>(false);
				}
				return this.m_observers;
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06002161 RID: 8545 RVA: 0x0005824C File Offset: 0x0005644C
		// (set) Token: 0x06002162 RID: 8546 RVA: 0x00058254 File Offset: 0x00056454
		public DateTime TimeOfLastUpdate { get; set; } = DateTime.MinValue;

		// Token: 0x06002163 RID: 8547 RVA: 0x0005825D File Offset: 0x0005645D
		public DistanceBand(float distance, float updateTime)
		{
			this.m_distance = distance;
			this.m_updateTime = updateTime;
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x00058289 File Offset: 0x00056489
		public bool CanUpdate()
		{
			return DateTime.UtcNow >= this.m_timeOfNextUpdate;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x0005829B File Offset: 0x0005649B
		public void MarkUpdateTime(DateTime now)
		{
			this.TimeOfLastUpdate = now;
			this.m_timeOfNextUpdate = now.AddSeconds((double)this.m_updateTime);
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x000582B8 File Offset: 0x000564B8
		public void AddObserver(Observer obs)
		{
			DistanceBand distanceBand = obs.DistanceBand;
			if (distanceBand != null)
			{
				distanceBand.RemoveObserver(obs);
			}
			obs.DistanceBand = this;
			this.Observers.Add(obs.Id, obs);
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x000582E5 File Offset: 0x000564E5
		public void RemoveObserver(Observer obs)
		{
			this.Observers.Remove(obs.Id);
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x000582F9 File Offset: 0x000564F9
		public bool Equals(DistanceBand other)
		{
			return other != null && (this == other || this.m_distance.Equals(other.m_distance));
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x00058317 File Offset: 0x00056517
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((DistanceBand)obj)));
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x00058345 File Offset: 0x00056545
		public override int GetHashCode()
		{
			return this.m_distance.GetHashCode();
		}

		// Token: 0x040025D2 RID: 9682
		private DictionaryList<int, Observer> m_observers;

		// Token: 0x040025D3 RID: 9683
		[SerializeField]
		private float m_distance;

		// Token: 0x040025D4 RID: 9684
		[SerializeField]
		private float m_updateTime = 1f;

		// Token: 0x040025D5 RID: 9685
		private DateTime m_timeOfNextUpdate;
	}
}
