using System;
using System.Collections;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200088F RID: 2191
	public class InGameUICanvas : InGameUIElement
	{
		// Token: 0x17000EBC RID: 3772
		// (get) Token: 0x06003FCA RID: 16330 RVA: 0x0006B211 File Offset: 0x00069411
		// (set) Token: 0x06003FCB RID: 16331 RVA: 0x0006B219 File Offset: 0x00069419
		private bool Visible
		{
			get
			{
				return this.m_visible;
			}
			set
			{
				if (this.m_visible == value)
				{
					return;
				}
				this.m_visible = value;
				if (this.m_visible)
				{
					this.m_triggerVisible = true;
					return;
				}
				this.m_triggerVisible = false;
				this.HideCanvasGroups();
			}
		}

		// Token: 0x06003FCC RID: 16332 RVA: 0x00189BE8 File Offset: 0x00187DE8
		private void Awake()
		{
			this.HideCanvasGroups();
			for (int i = 0; i < this.m_canvasGroups.Length; i++)
			{
				if (this.m_canvasGroups[i])
				{
					this.m_canvasGroups[i].gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06003FCD RID: 16333 RVA: 0x0006B249 File Offset: 0x00069449
		protected override void Start()
		{
			base.Start();
			this.m_updateCo = this.UpdateCo();
			base.StartCoroutine(this.m_updateCo);
		}

		// Token: 0x06003FCE RID: 16334 RVA: 0x0006B26A File Offset: 0x0006946A
		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.StopCoroutine(this.m_updateCo);
		}

		// Token: 0x06003FCF RID: 16335 RVA: 0x00189C30 File Offset: 0x00187E30
		private void HideCanvasGroups()
		{
			for (int i = 0; i < this.m_canvasGroups.Length; i++)
			{
				if (this.m_canvasGroups[i])
				{
					if (this.m_canvasGroups[i].alpha > 0f)
					{
						this.m_canvasGroups[i].alpha = 0f;
					}
					if (this.m_canvasGroups[i].blocksRaycasts)
					{
						this.m_canvasGroups[i].blocksRaycasts = false;
					}
				}
			}
		}

		// Token: 0x06003FD0 RID: 16336 RVA: 0x0006B27E File Offset: 0x0006947E
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				if (this.m_triggerVisible)
				{
					int num;
					for (int i = 0; i < this.m_canvasGroups.Length; i = num + 1)
					{
						if (this.m_canvasGroups[i])
						{
							if (this.m_triggerVisible && this.m_canvasGroups[i].alpha < 1f)
							{
								if (i == 0)
								{
									SolDebug.LogWithTime("UI Alpha", true);
								}
								this.m_canvasGroups[i].alpha = 1f;
								yield return null;
							}
							if (this.m_triggerVisible && !this.m_canvasGroups[i].blocksRaycasts)
							{
								if (i == 0)
								{
									SolDebug.LogWithTime("UI Raycasts", true);
								}
								this.m_canvasGroups[i].blocksRaycasts = true;
								yield return null;
							}
						}
						num = i;
					}
					this.m_triggerVisible = false;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06003FD1 RID: 16337 RVA: 0x0006B28D File Offset: 0x0006948D
		protected override void UiManagerOnResetUi()
		{
			this.Visible = false;
		}

		// Token: 0x06003FD2 RID: 16338 RVA: 0x0006B28D File Offset: 0x0006948D
		protected override void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			this.Visible = false;
		}

		// Token: 0x06003FD3 RID: 16339 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void LocalPlayerOnLocalPlayerInitialized()
		{
		}

		// Token: 0x06003FD4 RID: 16340 RVA: 0x0006B296 File Offset: 0x00069496
		protected override void LocalPlayerOnFadeLoadingScreenShowUi()
		{
			this.Visible = true;
		}

		// Token: 0x04003D6C RID: 15724
		[SerializeField]
		private CanvasGroup[] m_canvasGroups;

		// Token: 0x04003D6D RID: 15725
		private bool m_triggerVisible;

		// Token: 0x04003D6E RID: 15726
		private bool m_visible;

		// Token: 0x04003D6F RID: 15727
		private IEnumerator m_updateCo;
	}
}
