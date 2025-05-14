using System;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005BF RID: 1471
	public class HandHeldItemCache : IHandHeldItems
	{
		// Token: 0x06002E87 RID: 11911 RVA: 0x00060484 File Offset: 0x0005E684
		public HandHeldItemCache()
		{
			this.m_mainHand = new CachedHandHeldItem(true);
			this.m_offHand = new CachedHandHeldItem(false);
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x00153148 File Offset: 0x00151348
		public void AccessReset(GameEntity entity)
		{
			if (this.m_LastFrameAccessed >= 2147483647)
			{
				this.m_LastFrameAccessed = -1;
			}
			if (Time.frameCount > this.m_LastFrameAccessed)
			{
				this.m_mainHand.AccessReset(entity);
				this.m_offHand.AccessReset(entity);
				this.m_LastFrameAccessed = Time.frameCount;
			}
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000604AB File Offset: 0x0005E6AB
		public void ResetLastFrameAccessed()
		{
			this.m_LastFrameAccessed = -1;
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x000604B4 File Offset: 0x0005E6B4
		public void ResetReferences()
		{
			CachedHandHeldItem mainHand = this.m_mainHand;
			if (mainHand != null)
			{
				mainHand.Reset();
			}
			CachedHandHeldItem offHand = this.m_offHand;
			if (offHand == null)
			{
				return;
			}
			offHand.Reset();
		}

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06002E8B RID: 11915 RVA: 0x000604D7 File Offset: 0x0005E6D7
		public CachedHandHeldItem MainHand
		{
			get
			{
				return this.m_mainHand;
			}
		}

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06002E8C RID: 11916 RVA: 0x000604DF File Offset: 0x0005E6DF
		public CachedHandHeldItem OffHand
		{
			get
			{
				return this.m_offHand;
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06002E8D RID: 11917 RVA: 0x000604D7 File Offset: 0x0005E6D7
		CachedHandHeldItem IHandHeldItems.MainHand
		{
			get
			{
				return this.m_mainHand;
			}
		}

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x06002E8E RID: 11918 RVA: 0x000604DF File Offset: 0x0005E6DF
		CachedHandHeldItem IHandHeldItems.OffHand
		{
			get
			{
				return this.m_offHand;
			}
		}

		// Token: 0x04002E04 RID: 11780
		private const int kDefaultFrame = -1;

		// Token: 0x04002E05 RID: 11781
		private readonly CachedHandHeldItem m_mainHand;

		// Token: 0x04002E06 RID: 11782
		private readonly CachedHandHeldItem m_offHand;

		// Token: 0x04002E07 RID: 11783
		private int m_LastFrameAccessed = -1;
	}
}
