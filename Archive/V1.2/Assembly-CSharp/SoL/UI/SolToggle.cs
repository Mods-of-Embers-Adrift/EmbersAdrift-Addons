using System;
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
	// Token: 0x02000373 RID: 883
	public class SolToggle : Toggle, ITextMeshPro, ICursor, IInteractiveBase
	{
		// Token: 0x06001831 RID: 6193 RVA: 0x0010399C File Offset: 0x00101B9C
		protected override void Awake()
		{
			base.Awake();
			this.m_colorText = (this.m_colorText && base.transition == Selectable.Transition.ColorTint);
			if (this.m_colorText)
			{
				this.SetTextColor(base.colors.normalColor);
			}
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x001039E8 File Offset: 0x00101BE8
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

		// Token: 0x06001833 RID: 6195 RVA: 0x00103A88 File Offset: 0x00101C88
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (this.m_text != null)
			{
				this.m_text.CrossFadeColor(targetColor, (!instant) ? base.colors.fadeDuration : 0f, true, true);
			}
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x00053018 File Offset: 0x00051218
		public override void OnPointerClick(PointerEventData eventData)
		{
			this.PlayClipAudio();
			base.OnPointerClick(eventData);
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00103ACC File Offset: 0x00101CCC
		private void PlayClipAudio()
		{
			if (!base.interactable)
			{
				return;
			}
			AudioClip clip = (this.m_overrideAudioClip == null) ? GlobalSettings.Values.Audio.DefaultClickClip : this.m_overrideAudioClip;
			if (ClientGameManager.UIManager != null)
			{
				ClientGameManager.UIManager.PlayClip(clip, new float?(1f), new float?(GlobalSettings.Values.Audio.DefaultClickVolume));
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06001836 RID: 6198 RVA: 0x00052E36 File Offset: 0x00051036
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

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001837 RID: 6199 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06001838 RID: 6200 RVA: 0x00053027 File Offset: 0x00051227
		// (set) Token: 0x06001839 RID: 6201 RVA: 0x00053044 File Offset: 0x00051244
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

		// Token: 0x0600183A RID: 6202 RVA: 0x00053060 File Offset: 0x00051260
		public void SetTextColor(uint hexLiteral)
		{
			this.m_text.SetTextColor(hexLiteral);
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x0005306E File Offset: 0x0005126E
		public void SetTextColor(string hex)
		{
			this.m_text.SetTextColor(hex);
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x0005307C File Offset: 0x0005127C
		public void SetTextColor(Color c)
		{
			this.m_text.SetTextColor(c);
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001F8D RID: 8077
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04001F8E RID: 8078
		[SerializeField]
		private bool m_colorText;

		// Token: 0x04001F8F RID: 8079
		[SerializeField]
		private AudioClip m_overrideAudioClip;
	}
}
