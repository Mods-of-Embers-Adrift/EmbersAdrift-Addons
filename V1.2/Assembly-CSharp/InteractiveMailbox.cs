using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class InteractiveMailbox : BaseNetworkInteractiveStation
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000018 RID: 24 RVA: 0x0004479C File Offset: 0x0004299C
	protected override bool AllowInteractionWhileMissingBag
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000019 RID: 25 RVA: 0x0004479F File Offset: 0x0004299F
	protected override string m_tooltipText
	{
		get
		{
			return "Mailbox";
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600001A RID: 26 RVA: 0x000447A6 File Offset: 0x000429A6
	protected override ContainerType m_containerType
	{
		get
		{
			return ContainerType.PostOutgoing;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600001B RID: 27 RVA: 0x000447AA File Offset: 0x000429AA
	protected override CursorType ActiveCursorType
	{
		get
		{
			return CursorType.MailCursor;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600001C RID: 28 RVA: 0x000447AE File Offset: 0x000429AE
	protected override CursorType InactiveCursorType
	{
		get
		{
			return CursorType.MailCursorInactive;
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x000879D0 File Offset: 0x00085BD0
	private void Start()
	{
		if (!GameManager.IsServer && ClientGameManager.SocialManager)
		{
			ClientGameManager.SocialManager.UnreadMailUpdated += this.OnUnreadUpdated;
			if (ClientGameManager.SocialManager.HasUnreadMail)
			{
				this.m_animator.SetInteger(InteractiveMailbox.m_stateParamID, 1);
				this.m_hadUnreadMail = true;
			}
		}
	}

	// Token: 0x0600001E RID: 30 RVA: 0x000447B2 File Offset: 0x000429B2
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (!GameManager.IsServer && ClientGameManager.SocialManager)
		{
			ClientGameManager.SocialManager.UnreadMailUpdated -= this.OnUnreadUpdated;
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00087A2C File Offset: 0x00085C2C
	private void OnUnreadUpdated()
	{
		if (ClientGameManager.SocialManager.HasUnreadMail && !this.m_hadUnreadMail)
		{
			this.m_animator.SetInteger(InteractiveMailbox.m_stateParamID, 1);
			this.m_hadUnreadMail = true;
			return;
		}
		if (!ClientGameManager.SocialManager.HasUnreadMail && this.m_hadUnreadMail)
		{
			this.m_animator.SetInteger(InteractiveMailbox.m_stateParamID, 0);
			this.m_hadUnreadMail = false;
		}
	}

	// Token: 0x04000011 RID: 17
	private static readonly int m_stateParamID = Animator.StringToHash("State");

	// Token: 0x04000012 RID: 18
	[SerializeField]
	private Animator m_animator;

	// Token: 0x04000013 RID: 19
	private bool m_hadUnreadMail;
}
