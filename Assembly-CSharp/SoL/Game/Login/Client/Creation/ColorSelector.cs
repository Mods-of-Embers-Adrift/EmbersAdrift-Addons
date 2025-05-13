using System;
using System.Collections.Generic;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Login.Client.Creation
{
	// Token: 0x02000B57 RID: 2903
	public class ColorSelector : Feature
	{
		// Token: 0x1400011B RID: 283
		// (add) Token: 0x06005959 RID: 22873 RVA: 0x001E9B3C File Offset: 0x001E7D3C
		// (remove) Token: 0x0600595A RID: 22874 RVA: 0x001E9B74 File Offset: 0x001E7D74
		public event Action<int> ColorSelectedEvent;

		// Token: 0x1400011C RID: 284
		// (add) Token: 0x0600595B RID: 22875 RVA: 0x001E9BAC File Offset: 0x001E7DAC
		// (remove) Token: 0x0600595C RID: 22876 RVA: 0x001E9BE4 File Offset: 0x001E7DE4
		public event Action<bool> LockStateChangedEvent;

		// Token: 0x170014DA RID: 5338
		// (get) Token: 0x0600595D RID: 22877 RVA: 0x0007BD33 File Offset: 0x00079F33
		// (set) Token: 0x0600595E RID: 22878 RVA: 0x0007BD3B File Offset: 0x00079F3B
		public int SelectedIndex { get; private set; }

		// Token: 0x0600595F RID: 22879 RVA: 0x0007BD44 File Offset: 0x00079F44
		public override void Initialize(CreationDirector director, CreationDirector.CharacterToCreate toCreate, CreationDirector.FeatureSetting settings)
		{
			base.Initialize(director, toCreate, settings);
			this.InitColors();
		}

		// Token: 0x06005960 RID: 22880 RVA: 0x001E9C1C File Offset: 0x001E7E1C
		private void InitColors()
		{
			this.m_colorButtons = new List<ColorButton>();
			for (int i = 0; i < this.m_settings.ColorCollection.Colors.Length; i++)
			{
				ColorButton component = UnityEngine.Object.Instantiate<GameObject>(this.m_colorButtonPrefab, this.m_content).GetComponent<ColorButton>();
				component.Initialize(i, this.m_settings.ColorCollection.Colors[i], this);
				this.m_colorButtons.Add(component);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.gameObject.GetComponent<RectTransform>());
			this.ColorSelected(0);
		}

		// Token: 0x06005961 RID: 22881 RVA: 0x001E9CAC File Offset: 0x001E7EAC
		public void ColorSelected(int index)
		{
			this.SelectedIndex = index;
			Action<int> colorSelectedEvent = this.ColorSelectedEvent;
			if (colorSelectedEvent != null)
			{
				colorSelectedEvent(this.SelectedIndex);
			}
			this.m_director.SetSharedColor(this.m_settings.ColorType, this.m_colorButtons[this.SelectedIndex].Color);
		}

		// Token: 0x06005962 RID: 22882 RVA: 0x0007BD55 File Offset: 0x00079F55
		public override void Randomize()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Randomize();
			this.ColorSelected(UnityEngine.Random.Range(0, this.m_colorButtons.Count));
		}

		// Token: 0x06005963 RID: 22883 RVA: 0x0007BD7D File Offset: 0x00079F7D
		public override void Reset()
		{
			if (base.m_locked)
			{
				return;
			}
			base.Reset();
			this.ColorSelected(0);
		}

		// Token: 0x06005964 RID: 22884 RVA: 0x0007BD95 File Offset: 0x00079F95
		public override void OnLockStateChanged(ToggleController.ToggleState obj)
		{
			base.OnLockStateChanged(obj);
			Action<bool> lockStateChangedEvent = this.LockStateChangedEvent;
			if (lockStateChangedEvent == null)
			{
				return;
			}
			lockStateChangedEvent(base.m_locked);
		}

		// Token: 0x04004EA8 RID: 20136
		[SerializeField]
		private GameObject m_colorButtonPrefab;

		// Token: 0x04004EA9 RID: 20137
		[SerializeField]
		private RectTransform m_content;

		// Token: 0x04004EAA RID: 20138
		private List<ColorButton> m_colorButtons;
	}
}
