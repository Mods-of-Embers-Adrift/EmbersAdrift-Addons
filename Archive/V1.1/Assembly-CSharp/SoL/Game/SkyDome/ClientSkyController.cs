using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006E7 RID: 1767
	[ExecuteInEditMode]
	public abstract class ClientSkyController : BaseSkyController
	{
		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x0600357B RID: 13691 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool AutoInitialize
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600357C RID: 13692 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetCloudHeight()
		{
		}

		// Token: 0x0600357D RID: 13693 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateClouds()
		{
		}

		// Token: 0x0600357E RID: 13694 RVA: 0x0006491A File Offset: 0x00062B1A
		protected virtual void Start()
		{
			if (GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
			if (this.AutoInitialize)
			{
				this.Initialize();
			}
			if (Application.isPlaying)
			{
				this.SetCloudHeight();
			}
		}

		// Token: 0x0600357F RID: 13695 RVA: 0x00166E14 File Offset: 0x00165014
		protected virtual void Update()
		{
			if (Application.isPlaying && this.m_progressTime)
			{
				float num = 12.743362f * Time.deltaTime;
				DateTime dateTime = this.m_internalDateTime.DateTime.AddSeconds((double)num);
				this.m_internalDateTime.DateTime = dateTime;
				this.SetTime(dateTime, false);
			}
			base.UpdateCelestials();
			if (Application.isPlaying)
			{
				this.UpdateClouds();
			}
		}
	}
}
