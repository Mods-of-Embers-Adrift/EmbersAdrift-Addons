using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005A1 RID: 1441
	public class PlayerChatBubbles : GameEntityComponent
	{
		// Token: 0x06002D24 RID: 11556 RVA: 0x0014D12C File Offset: 0x0014B32C
		private void Awake()
		{
			if (!this.m_chatBubbleCanvasPrefab)
			{
				base.enabled = false;
				return;
			}
			this.m_obj = UnityEngine.Object.Instantiate<GameObject>(this.m_chatBubbleCanvasPrefab, base.gameObject.transform);
			Options.GameOptions.ShowOverheadNameplates.Changed += this.ShowOverheadNameplatesOnChanged;
			Options.GameOptions.ShowOverheadNameplate_Self.Changed += this.ShowOverheadSelfNameplateOnChanged;
		}

		// Token: 0x06002D25 RID: 11557 RVA: 0x0005F518 File Offset: 0x0005D718
		private void Start()
		{
			this.RefreshPosition();
		}

		// Token: 0x06002D26 RID: 11558 RVA: 0x0005F520 File Offset: 0x0005D720
		private void OnDestroy()
		{
			Options.GameOptions.ShowOverheadNameplates.Changed -= this.ShowOverheadNameplatesOnChanged;
			Options.GameOptions.ShowOverheadNameplate_Self.Changed -= this.ShowOverheadSelfNameplateOnChanged;
		}

		// Token: 0x06002D27 RID: 11559 RVA: 0x0005F518 File Offset: 0x0005D718
		private void ShowOverheadSelfNameplateOnChanged()
		{
			this.RefreshPosition();
		}

		// Token: 0x06002D28 RID: 11560 RVA: 0x0005F518 File Offset: 0x0005D718
		private void ShowOverheadNameplatesOnChanged()
		{
			this.RefreshPosition();
		}

		// Token: 0x06002D29 RID: 11561 RVA: 0x0005F54E File Offset: 0x0005D74E
		private void RefreshPosition()
		{
			if (this.m_obj)
			{
				this.m_obj.transform.localPosition = this.GetPosition();
			}
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x0014D198 File Offset: 0x0014B398
		private Vector3 GetPosition()
		{
			if (!Options.GameOptions.ShowOverheadNameplates.Value)
			{
				return this.m_nameplateOffPosition;
			}
			if (!Options.GameOptions.ShowOverheadNameplate_Self.Value && base.GameEntity && base.GameEntity == LocalPlayer.GameEntity)
			{
				return this.m_nameplateOffPosition;
			}
			return this.m_nameplateOnPosition;
		}

		// Token: 0x04002CC2 RID: 11458
		[SerializeField]
		private GameObject m_chatBubbleCanvasPrefab;

		// Token: 0x04002CC3 RID: 11459
		[SerializeField]
		private Vector3 m_nameplateOnPosition = new Vector3(0f, 2.4f, 0f);

		// Token: 0x04002CC4 RID: 11460
		[SerializeField]
		private Vector3 m_nameplateOffPosition = new Vector3(0f, 1.8f, 0f);

		// Token: 0x04002CC5 RID: 11461
		private GameObject m_obj;
	}
}
