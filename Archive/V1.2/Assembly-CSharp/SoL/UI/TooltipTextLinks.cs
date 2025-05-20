using System;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Subscription;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.UI
{
	// Token: 0x0200038E RID: 910
	public class TooltipTextLinks : MonoBehaviour, ITooltip, IInteractiveBase, ICursor, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
	{
		// Token: 0x060018EA RID: 6378 RVA: 0x00105860 File Offset: 0x00103A60
		private bool TryGetLink(out TMP_LinkInfo linkInfo)
		{
			linkInfo = default(TMP_LinkInfo);
			if (this.m_text != null)
			{
				int num = TMP_TextUtilities.FindIntersectingLink(this.m_text, Input.mousePosition, null);
				if (num != -1)
				{
					linkInfo = this.m_text.textInfo.linkInfo[num];
					return true;
				}
			}
			return false;
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x001058B8 File Offset: 0x00103AB8
		private TooltipTextLinks.TextLinkType GetLinkIdAndType(TMP_LinkInfo linkInfo, out string value)
		{
			value = "";
			TooltipTextLinks.TextLinkType result = TooltipTextLinks.TextLinkType.None;
			string linkID = linkInfo.GetLinkID();
			if (linkID.StartsWith("playerName"))
			{
				value = linkInfo.GetLinkText();
				result = TooltipTextLinks.TextLinkType.PlayerName;
			}
			else if (linkID.StartsWith("instanceId"))
			{
				value = linkID.Remove(0, "instanceId".Length + 1);
				result = TooltipTextLinks.TextLinkType.InstanceId;
			}
			else if (linkID.StartsWith("archetypeId"))
			{
				value = linkID.Remove(0, "archetypeId".Length + 1);
				result = TooltipTextLinks.TextLinkType.ArchetypeId;
			}
			else if (linkID.StartsWith("http://") || linkID.StartsWith("https://"))
			{
				value = linkID;
				result = TooltipTextLinks.TextLinkType.Website;
			}
			else if (linkID.StartsWith("text"))
			{
				value = linkID.Remove(0, "text".Length + 1);
				result = TooltipTextLinks.TextLinkType.Text;
			}
			else if (linkID.StartsWith("longtext"))
			{
				value = linkID.Remove(0, "longtext".Length + 1);
				result = TooltipTextLinks.TextLinkType.LongText;
			}
			else if (linkID.StartsWith("activateSub"))
			{
				result = TooltipTextLinks.TextLinkType.ActivateSub;
			}
			return result;
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x0005377D File Offset: 0x0005197D
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_mouseHovered = true;
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x00053786 File Offset: 0x00051986
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_mouseHovered = false;
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x001059C0 File Offset: 0x00103BC0
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			TMP_LinkInfo linkInfo;
			if (this.m_mouseHovered && this.TryGetLink(out linkInfo))
			{
				string text;
				TooltipTextLinks.TextLinkType linkIdAndType = this.GetLinkIdAndType(linkInfo, out text);
				if (linkIdAndType == TooltipTextLinks.TextLinkType.Website)
				{
					Application.OpenURL(text);
					return;
				}
				if (linkIdAndType != TooltipTextLinks.TextLinkType.PlayerName)
				{
					if (linkIdAndType != TooltipTextLinks.TextLinkType.ActivateSub)
					{
						return;
					}
					SubscriptionExtensions.ExecuteActivateSubscription();
				}
				else if (SessionData.SelectedCharacter == null || !SessionData.SelectedCharacter.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase))
				{
					PointerEventData.InputButton button = eventData.button;
					if (button != PointerEventData.InputButton.Left)
					{
						if (button != PointerEventData.InputButton.Right)
						{
							return;
						}
						ContextMenuUI.ClearContextActions();
						InteractivePlayer.FillActionsForEntityName(text, null);
						ClientGameManager.UIManager.ContextMenu.Init(text);
						return;
					}
					else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						UIManager.ActiveChatInput.StartWhisper(text);
						return;
					}
				}
			}
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x060018EF RID: 6383 RVA: 0x0005378F File Offset: 0x0005198F
		CursorType ICursor.Type
		{
			get
			{
				return this.m_linkCursor;
			}
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x00105A74 File Offset: 0x00103C74
		private CursorType GetCursorTypeForHover(TooltipTextLinks.TextLinkType linkType, string value)
		{
			switch (linkType)
			{
			case TooltipTextLinks.TextLinkType.Text:
			case TooltipTextLinks.TextLinkType.LongText:
				return CursorType.GloveCursor;
			case TooltipTextLinks.TextLinkType.Website:
			case TooltipTextLinks.TextLinkType.InstanceId:
			case TooltipTextLinks.TextLinkType.ArchetypeId:
			case TooltipTextLinks.TextLinkType.ActivateSub:
				return CursorType.IdentifyingGlassCursor;
			case TooltipTextLinks.TextLinkType.PlayerName:
				if (SessionData.SelectedCharacter == null || !SessionData.SelectedCharacter.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					return CursorType.TextCursor;
				}
				break;
			}
			return CursorType.None;
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x00105ACC File Offset: 0x00103CCC
		private ITooltipParameter GetTooltipParameter()
		{
			TMP_LinkInfo linkInfo;
			if (this.m_mouseHovered && this.TryGetLink(out linkInfo))
			{
				string text;
				TooltipTextLinks.TextLinkType linkIdAndType = this.GetLinkIdAndType(linkInfo, out text);
				this.m_linkCursor = this.GetCursorTypeForHover(linkIdAndType, text);
				switch (linkIdAndType)
				{
				case TooltipTextLinks.TextLinkType.Text:
					return new ObjectTextTooltipParameter(this, text, this.m_isOptionsMenu);
				case TooltipTextLinks.TextLinkType.InstanceId:
				{
					ArchetypeInstance instance;
					if (MessageManager.TryGetLinkedInstance(text, out instance))
					{
						return new ArchetypeTooltipParameter
						{
							Instance = instance
						};
					}
					break;
				}
				case TooltipTextLinks.TextLinkType.ArchetypeId:
				{
					BaseArchetype archetype;
					if (InternalGameDatabase.Archetypes.TryGetItem(text, out archetype))
					{
						return new ArchetypeTooltipParameter
						{
							Archetype = archetype
						};
					}
					break;
				}
				case TooltipTextLinks.TextLinkType.LongText:
				{
					int key;
					if (int.TryParse(text, out key) && TextMeshProExtensions.LongTooltips.TryGetValue(key, out text))
					{
						return new ObjectTextTooltipParameter(this, text, this.m_isOptionsMenu);
					}
					break;
				}
				case TooltipTextLinks.TextLinkType.ActivateSub:
					return new ObjectTextTooltipParameter(this, "Activate Subscription", this.m_isOptionsMenu);
				}
			}
			else
			{
				this.m_linkCursor = CursorType.None;
			}
			return null;
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x060018F2 RID: 6386 RVA: 0x00053797 File Offset: 0x00051997
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x060018F3 RID: 6387 RVA: 0x000537A5 File Offset: 0x000519A5
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x060018F4 RID: 6388 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FED RID: 8173
		public const string kTextPrefix = "text";

		// Token: 0x04001FEE RID: 8174
		public const string kLongTextPrefix = "longtext";

		// Token: 0x04001FEF RID: 8175
		public const string kPlayerNamePrefix = "playerName";

		// Token: 0x04001FF0 RID: 8176
		public const string kInstanceIdPrefix = "instanceId";

		// Token: 0x04001FF1 RID: 8177
		public const string kArchetypeIdPrefix = "archetypeId";

		// Token: 0x04001FF2 RID: 8178
		public const string kActivateSubPrefix = "activateSub";

		// Token: 0x04001FF3 RID: 8179
		public const string kWebsitePrefixHttp = "http://";

		// Token: 0x04001FF4 RID: 8180
		public const string kWebsitePrefixHttps = "https://";

		// Token: 0x04001FF5 RID: 8181
		public const string kSteamPurchaseUrl = "https://store.steampowered.com/app/3336530/Embers_Adrift/";

		// Token: 0x04001FF6 RID: 8182
		public const string kSubscribeNowUrl = "https://www.embersadrift.com/Account";

		// Token: 0x04001FF7 RID: 8183
		public const string kActivateSubLinkPrefix = "<link=\"activateSub\">";

		// Token: 0x04001FF8 RID: 8184
		public const string kDonateUrl = "https://www.embersadrift.com/Donate";

		// Token: 0x04001FF9 RID: 8185
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04001FFA RID: 8186
		[SerializeField]
		private bool m_isOptionsMenu;

		// Token: 0x04001FFB RID: 8187
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04001FFC RID: 8188
		private bool m_mouseHovered;

		// Token: 0x04001FFD RID: 8189
		private CursorType m_linkCursor;

		// Token: 0x0200038F RID: 911
		private enum TextLinkType
		{
			// Token: 0x04001FFF RID: 8191
			None,
			// Token: 0x04002000 RID: 8192
			Text,
			// Token: 0x04002001 RID: 8193
			Website,
			// Token: 0x04002002 RID: 8194
			PlayerName,
			// Token: 0x04002003 RID: 8195
			InstanceId,
			// Token: 0x04002004 RID: 8196
			ArchetypeId,
			// Token: 0x04002005 RID: 8197
			LongText,
			// Token: 0x04002006 RID: 8198
			ActivateSub
		}
	}
}
