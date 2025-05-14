using System;
using SoL.Networking.Database;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B5B RID: 2907
	public abstract class Feature : MonoBehaviour
	{
		// Token: 0x170014DB RID: 5339
		// (get) Token: 0x06005975 RID: 22901 RVA: 0x0007BEA0 File Offset: 0x0007A0A0
		protected bool m_locked
		{
			get
			{
				return this.m_lrrPanel != null && this.m_lrrPanel.Lock != null && this.m_lrrPanel.Lock.Locked;
			}
		}

		// Token: 0x170014DC RID: 5340
		// (get) Token: 0x06005976 RID: 22902 RVA: 0x0007BED5 File Offset: 0x0007A0D5
		public CharacterSex Sex
		{
			get
			{
				if (this.m_character != null)
				{
					return this.m_character.Sex;
				}
				return CharacterSex.None;
			}
		}

		// Token: 0x06005977 RID: 22903 RVA: 0x0007BEEC File Offset: 0x0007A0EC
		private void Awake()
		{
			this.Subscribe();
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x0007BEF4 File Offset: 0x0007A0F4
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x0007BEFC File Offset: 0x0007A0FC
		public virtual void Initialize(CreationDirector director, CreationDirector.CharacterToCreate toCreate, CreationDirector.FeatureSetting settings)
		{
			this.m_director = director;
			this.m_character = toCreate;
			this.m_settings = settings;
			if (this.m_label != null)
			{
				this.m_label.text = this.m_settings.Label;
			}
		}

		// Token: 0x0600597A RID: 22906 RVA: 0x001EA148 File Offset: 0x001E8348
		protected virtual void Subscribe()
		{
			if (this.m_lrrPanel == null)
			{
				return;
			}
			if (this.m_lrrPanel.Lock != null)
			{
				this.m_lrrPanel.Lock.ToggleChanged += this.OnLockStateChanged;
			}
			if (this.m_lrrPanel.Reset != null)
			{
				this.m_lrrPanel.Reset.onClick.AddListener(new UnityAction(this.Reset));
			}
			if (this.m_lrrPanel.Randomize != null)
			{
				this.m_lrrPanel.Randomize.onClick.AddListener(new UnityAction(this.Randomize));
			}
		}

		// Token: 0x0600597B RID: 22907 RVA: 0x001EA200 File Offset: 0x001E8400
		protected virtual void Unsubscribe()
		{
			if (this.m_lrrPanel == null)
			{
				return;
			}
			if (this.m_lrrPanel.Lock != null)
			{
				this.m_lrrPanel.Lock.ToggleChanged -= this.OnLockStateChanged;
			}
			if (this.m_lrrPanel.Reset != null)
			{
				this.m_lrrPanel.Reset.onClick.RemoveListener(new UnityAction(this.Reset));
			}
			if (this.m_lrrPanel.Randomize != null)
			{
				this.m_lrrPanel.Randomize.onClick.RemoveListener(new UnityAction(this.Randomize));
			}
		}

		// Token: 0x0600597C RID: 22908 RVA: 0x001EA2B8 File Offset: 0x001E84B8
		public virtual void OnLockStateChanged(ToggleController.ToggleState obj)
		{
			bool flag = obj == ToggleController.ToggleState.ON;
			if (this.m_lrrPanel.Reset != null)
			{
				this.m_lrrPanel.Reset.interactable = !flag;
			}
			if (this.m_lrrPanel.Randomize != null)
			{
				this.m_lrrPanel.Randomize.interactable = !flag;
			}
		}

		// Token: 0x0600597D RID: 22909 RVA: 0x0007BF37 File Offset: 0x0007A137
		public void Toggle(CharacterSex selectedSex)
		{
			base.gameObject.SetActive(selectedSex == this.m_character.Sex);
		}

		// Token: 0x0600597E RID: 22910 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Reset()
		{
		}

		// Token: 0x0600597F RID: 22911 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Randomize()
		{
		}

		// Token: 0x06005980 RID: 22912 RVA: 0x0007BF52 File Offset: 0x0007A152
		public void Lock()
		{
			this.ToggleLock(true);
		}

		// Token: 0x06005981 RID: 22913 RVA: 0x0007BF5B File Offset: 0x0007A15B
		public void Unlock()
		{
			this.ToggleLock(false);
		}

		// Token: 0x06005982 RID: 22914 RVA: 0x001EA318 File Offset: 0x001E8518
		public virtual void ToggleLock(bool locked)
		{
			if (this.m_lrrPanel != null && this.m_lrrPanel.Lock != null && this.m_lrrPanel.Lock.Locked != locked)
			{
				this.m_lrrPanel.Lock.State = (locked ? ToggleController.ToggleState.ON : ToggleController.ToggleState.OFF);
			}
		}

		// Token: 0x06005983 RID: 22915 RVA: 0x0007BF64 File Offset: 0x0007A164
		public void ToggleLockInteractable(bool interactable)
		{
			if (this.m_lrrPanel.Lock != null)
			{
				this.m_lrrPanel.Lock.Interactable = interactable;
			}
		}

		// Token: 0x04004EB6 RID: 20150
		[SerializeField]
		protected TextMeshProUGUI m_label;

		// Token: 0x04004EB7 RID: 20151
		[SerializeField]
		private LockResetRandomizePanel m_lrrPanel;

		// Token: 0x04004EB8 RID: 20152
		protected CreationDirector.FeatureSetting m_settings;

		// Token: 0x04004EB9 RID: 20153
		protected CreationDirector.CharacterToCreate m_character;

		// Token: 0x04004EBA RID: 20154
		protected CreationDirector m_director;
	}
}
