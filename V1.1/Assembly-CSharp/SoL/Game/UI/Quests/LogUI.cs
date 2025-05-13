using System;
using SoL.Game.HuntingLog;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000943 RID: 2371
	public class LogUI : DraggableUIWindow
	{
		// Token: 0x17000FA0 RID: 4000
		// (get) Token: 0x060045FD RID: 17917 RVA: 0x0006F136 File Offset: 0x0006D336
		public QuestLogUI Quests
		{
			get
			{
				return this.m_questLogUI;
			}
		}

		// Token: 0x17000FA1 RID: 4001
		// (get) Token: 0x060045FE RID: 17918 RVA: 0x0006F13E File Offset: 0x0006D33E
		public BulletinLogUI Bulletins
		{
			get
			{
				return this.m_bulletinLogUI;
			}
		}

		// Token: 0x17000FA2 RID: 4002
		// (get) Token: 0x060045FF RID: 17919 RVA: 0x0006F146 File Offset: 0x0006D346
		public TutorialsLogUI Tutorials
		{
			get
			{
				return this.m_tutorialsLogUI;
			}
		}

		// Token: 0x17000FA3 RID: 4003
		// (get) Token: 0x06004600 RID: 17920 RVA: 0x0006F14E File Offset: 0x0006D34E
		public HuntingLogUI Hunting
		{
			get
			{
				return this.m_huntingLogUI;
			}
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x0006F156 File Offset: 0x0006D356
		protected override void Start()
		{
			base.Start();
			this.m_tabController.TabChanged += this.OnTabChanged;
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x0006F175 File Offset: 0x0006D375
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_tabController.TabChanged -= this.OnTabChanged;
		}

		// Token: 0x06004603 RID: 17923 RVA: 0x001A1F3C File Offset: 0x001A013C
		public override void Show(bool skipTransition = false)
		{
			base.Show(skipTransition);
			this.m_questLogUI.PlayerPrefsKey = base.PlayerPrefsKey + "_Quests";
			this.m_bulletinLogUI.PlayerPrefsKey = base.PlayerPrefsKey + "_Bulletins";
			this.m_tutorialsLogUI.PlayerPrefsKey = base.PlayerPrefsKey + "_Tutorials";
			if (this.m_questLogUI.gameObject.activeInHierarchy)
			{
				this.m_questLogUI.Show();
			}
			if (this.m_bulletinLogUI.gameObject.activeInHierarchy)
			{
				this.m_bulletinLogUI.Show();
			}
			if (this.m_tutorialsLogUI.gameObject.activeInHierarchy)
			{
				this.m_tutorialsLogUI.Show();
			}
			if (this.m_huntingLogUI.gameObject.activeInHierarchy)
			{
				this.m_huntingLogUI.Show();
			}
		}

		// Token: 0x06004604 RID: 17924 RVA: 0x0006F194 File Offset: 0x0006D394
		public void Show(LogUITabs tab)
		{
			this.Show(false);
			this.m_tabController.SwitchToTab((int)tab);
		}

		// Token: 0x06004605 RID: 17925 RVA: 0x001A2018 File Offset: 0x001A0218
		private void OnTabChanged()
		{
			if (this.m_questLogUI.gameObject.activeInHierarchy)
			{
				this.m_questLogUI.Show();
			}
			if (this.m_bulletinLogUI.gameObject.activeInHierarchy)
			{
				this.m_bulletinLogUI.Show();
			}
			if (this.m_tutorialsLogUI.gameObject.activeInHierarchy)
			{
				this.m_tutorialsLogUI.Show();
			}
			if (this.m_huntingLogUI.gameObject.activeInHierarchy)
			{
				this.m_huntingLogUI.Show();
			}
		}

		// Token: 0x04004236 RID: 16950
		[SerializeField]
		private QuestLogUI m_questLogUI;

		// Token: 0x04004237 RID: 16951
		[SerializeField]
		private BulletinLogUI m_bulletinLogUI;

		// Token: 0x04004238 RID: 16952
		[SerializeField]
		private TutorialsLogUI m_tutorialsLogUI;

		// Token: 0x04004239 RID: 16953
		[SerializeField]
		private HuntingLogUI m_huntingLogUI;

		// Token: 0x0400423A RID: 16954
		[SerializeField]
		private TabController m_tabController;
	}
}
