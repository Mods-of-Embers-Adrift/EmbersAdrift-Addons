using System;
using Cysharp.Text;
using SoL.Game.Audio;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000370 RID: 880
	public class SolButton : Button, ITextMeshPro, ICursor, IInteractiveBase
	{
		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06001808 RID: 6152 RVA: 0x001035C0 File Offset: 0x001017C0
		// (remove) Token: 0x06001809 RID: 6153 RVA: 0x001035F8 File Offset: 0x001017F8
		public event Action<PointerEventData> PointerClicked;

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x0600180A RID: 6154 RVA: 0x00103630 File Offset: 0x00101830
		// (remove) Token: 0x0600180B RID: 6155 RVA: 0x00103668 File Offset: 0x00101868
		public event Action<SolButton> MouseEnter;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x0600180C RID: 6156 RVA: 0x001036A0 File Offset: 0x001018A0
		// (remove) Token: 0x0600180D RID: 6157 RVA: 0x001036D8 File Offset: 0x001018D8
		public event Action<SolButton> MouseExit;

		// Token: 0x0600180E RID: 6158 RVA: 0x00103710 File Offset: 0x00101910
		protected override void Awake()
		{
			base.Awake();
			this.m_colorText = (this.m_colorText && base.transition == Selectable.Transition.ColorTint);
			if (this.m_text != null)
			{
				this.m_text.raycastTarget = false;
			}
			bool colorText = this.m_colorText;
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x00103760 File Offset: 0x00101960
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			if (this.m_colorText && this.m_text != null)
			{
				switch (state)
				{
				case Selectable.SelectionState.Normal:
					this.StartColorTween(base.colors.normalColor, instant);
					return;
				case Selectable.SelectionState.Highlighted:
					this.StartColorTween(base.colors.highlightedColor, instant);
					break;
				case Selectable.SelectionState.Pressed:
					this.StartColorTween(base.colors.pressedColor, instant);
					return;
				case Selectable.SelectionState.Selected:
					break;
				case Selectable.SelectionState.Disabled:
					this.StartColorTween(base.colors.disabledColor, instant);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x00103800 File Offset: 0x00101A00
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (this.m_text != null)
			{
				this.m_text.CrossFadeColor(targetColor, (!instant) ? base.colors.fadeDuration : 0f, true, true);
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06001811 RID: 6161 RVA: 0x00052D70 File Offset: 0x00050F70
		// (set) Token: 0x06001812 RID: 6162 RVA: 0x00052D8D File Offset: 0x00050F8D
		public string text
		{
			get
			{
				if (!(this.m_text == null))
				{
					return this.m_text.text;
				}
				return null;
			}
			set
			{
				if (this.m_text != null)
				{
					this.m_text.text = value;
				}
			}
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x00052DA9 File Offset: 0x00050FA9
		public void SetText(string txt)
		{
			if (this.m_text)
			{
				this.m_text.ZStringSetText(txt);
			}
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x00052DC4 File Offset: 0x00050FC4
		public void SetTextColor(string hex)
		{
			this.m_text.SetTextColor(hex);
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x00052DD2 File Offset: 0x00050FD2
		public void SetTextColor(Color color)
		{
			this.m_text.SetTextColor(color);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x00052DE0 File Offset: 0x00050FE0
		public void UnderlineText()
		{
			this.m_text.fontStyle |= FontStyles.Underline;
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x00052DF5 File Offset: 0x00050FF5
		public void RemoveUnderlineText()
		{
			this.m_text.fontStyle &= ~FontStyles.Underline;
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x00052E0B File Offset: 0x0005100B
		public void BoldText()
		{
			this.m_text.fontStyle |= FontStyles.Bold;
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x00052E20 File Offset: 0x00051020
		public void RemoveBoldText()
		{
			this.m_text.fontStyle &= ~FontStyles.Bold;
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x0600181A RID: 6170 RVA: 0x00052E36 File Offset: 0x00051036
		CursorType ICursor.Type
		{
			get
			{
				if (!base.interactable)
				{
					return CursorType.MainCursor;
				}
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x0600181B RID: 6171 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x00052E43 File Offset: 0x00051043
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			Action<SolButton> mouseEnter = this.MouseEnter;
			if (mouseEnter == null)
			{
				return;
			}
			mouseEnter(this);
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x00052E5D File Offset: 0x0005105D
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			Action<SolButton> mouseExit = this.MouseExit;
			if (mouseExit == null)
			{
				return;
			}
			mouseExit(this);
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x00052E77 File Offset: 0x00051077
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.PlayClipAudio();
			}
			base.OnPointerClick(eventData);
			Action<PointerEventData> pointerClicked = this.PointerClicked;
			if (pointerClicked == null)
			{
				return;
			}
			pointerClicked(eventData);
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x00103844 File Offset: 0x00101A44
		private void PlayClipAudio()
		{
			if (!base.interactable || ClientGameManager.UIManager == null)
			{
				return;
			}
			AudioClip clip;
			if (this.m_overrideAudioClipCollection != null)
			{
				clip = this.m_overrideAudioClipCollection.GetRandomClip();
			}
			else if (this.m_overrideAudioClip != null)
			{
				clip = this.m_overrideAudioClip;
			}
			else
			{
				clip = GlobalSettings.Values.Audio.DefaultClickClip;
			}
			ClientGameManager.UIManager.PlayClip(clip, new float?(1f), new float?(GlobalSettings.Values.Audio.DefaultClickVolume));
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001F85 RID: 8069
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04001F86 RID: 8070
		[SerializeField]
		private bool m_colorText;

		// Token: 0x04001F87 RID: 8071
		[SerializeField]
		private AudioClip m_overrideAudioClip;

		// Token: 0x04001F88 RID: 8072
		[SerializeField]
		private AudioClipCollection m_overrideAudioClipCollection;
	}
}
