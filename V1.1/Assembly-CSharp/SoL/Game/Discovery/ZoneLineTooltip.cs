using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CB2 RID: 3250
	public class ZoneLineTooltip : MonoBehaviour, ITooltip, IInteractiveBase, ICursor, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		// Token: 0x1700178B RID: 6027
		// (get) Token: 0x06006291 RID: 25233 RVA: 0x000822A0 File Offset: 0x000804A0
		private IEnumerable GetDiscoveryProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<DiscoveryProfile>();
			}
		}

		// Token: 0x1700178C RID: 6028
		// (get) Token: 0x06006292 RID: 25234 RVA: 0x00082539 File Offset: 0x00080739
		internal bool HasLabelWarper
		{
			get
			{
				return this.m_labelWarp != null;
			}
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x0020529C File Offset: 0x0020349C
		internal void Init(MapUI controller)
		{
			this.m_controller = controller;
			if (this.m_label && this.m_zoneId != ZoneId.None)
			{
				ZoneRecord zoneRecord = SessionData.GetZoneRecord(this.m_zoneId);
				if (zoneRecord != null)
				{
					string text = zoneRecord.DisplayName;
					if (!this.m_showFullName && text.Contains(" "))
					{
						text = text.Split(' ', StringSplitOptions.None)[0];
					}
					this.m_label.ZStringSetText(text);
				}
			}
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x00082547 File Offset: 0x00080747
		internal void WarpLabel()
		{
			if (this.m_label && this.m_zoneId != ZoneId.None && this.m_labelWarp)
			{
				this.m_labelWarp.WarpText();
			}
		}

		// Token: 0x06006295 RID: 25237 RVA: 0x00205308 File Offset: 0x00203508
		internal void RefreshDiscovered(Dictionary<ZoneId, List<UniqueId>> discoveryDict, ZoneId currentZoneId)
		{
			this.m_targetZoneIsDiscovered = (discoveryDict != null && discoveryDict.ContainsKey(this.m_zoneId));
			List<UniqueId> list;
			this.m_canInteract = (!this.m_fogProfile || (discoveryDict != null && discoveryDict.TryGetValue(currentZoneId, out list) && list.Contains(this.m_fogProfile.Id)));
			if (this.m_isWorldMapZoneLabel)
			{
				this.m_canInteract = (this.m_targetZoneIsDiscovered || (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == (int)this.m_zoneId));
			}
		}

		// Token: 0x06006296 RID: 25238 RVA: 0x0020539C File Offset: 0x0020359C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_zoneId != ZoneId.None && this.m_canInteract)
			{
				ZoneRecord zoneRecord = SessionData.GetZoneRecord(this.m_zoneId);
				if (zoneRecord != null)
				{
					string arg = (this.m_controller && this.m_targetZoneIsDiscovered) ? "click to load map" : "undiscovered region";
					string txt = ZString.Format<string, string>("<align=\"center\">{0}</align>\n<size=80%><align=\"center\">({1})</align></size>", zoneRecord.DisplayName, arg);
					return new ObjectTextTooltipParameter(this, txt, false);
				}
			}
			return null;
		}

		// Token: 0x1700178D RID: 6029
		// (get) Token: 0x06006297 RID: 25239 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700178E RID: 6030
		// (get) Token: 0x06006298 RID: 25240 RVA: 0x00082576 File Offset: 0x00080776
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700178F RID: 6031
		// (get) Token: 0x06006299 RID: 25241 RVA: 0x00082584 File Offset: 0x00080784
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17001790 RID: 6032
		// (get) Token: 0x0600629A RID: 25242 RVA: 0x0008258C File Offset: 0x0008078C
		CursorType ICursor.Type
		{
			get
			{
				if (!this.m_canInteract)
				{
					return CursorType.MainCursor;
				}
				if (!this.m_controller || !this.m_targetZoneIsDiscovered)
				{
					return CursorType.GloveCursorInactive;
				}
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x000825B1 File Offset: 0x000807B1
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_mouseWithin = true;
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x000825BA File Offset: 0x000807BA
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_mouseWithin = false;
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x000825C3 File Offset: 0x000807C3
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.m_mouseWithin && this.m_canInteract && this.m_controller && eventData.button == PointerEventData.InputButton.Left)
			{
				this.m_pointerUpTime = Time.time + 0.2f;
			}
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x0020540C File Offset: 0x0020360C
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.m_mouseWithin && this.m_canInteract && this.m_controller && eventData.button == PointerEventData.InputButton.Left && Time.time < this.m_pointerUpTime)
			{
				this.m_controller.ZoneLinePoiClicked(this.m_zoneId);
			}
		}

		// Token: 0x0600629F RID: 25247 RVA: 0x000825FB File Offset: 0x000807FB
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (this.m_controller && this.m_controller.Overlay)
			{
				this.m_controller.Overlay.OnBeginDrag(eventData);
			}
		}

		// Token: 0x060062A0 RID: 25248 RVA: 0x0008262D File Offset: 0x0008082D
		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (this.m_controller && this.m_controller.Overlay)
			{
				this.m_controller.Overlay.OnEndDrag(eventData);
			}
		}

		// Token: 0x060062A1 RID: 25249 RVA: 0x0008265F File Offset: 0x0008085F
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (this.m_controller && this.m_controller.Overlay)
			{
				this.m_controller.Overlay.OnDrag(eventData);
			}
		}

		// Token: 0x060062A3 RID: 25251 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040055FB RID: 22011
		[SerializeField]
		private DiscoveryProfile m_fogProfile;

		// Token: 0x040055FC RID: 22012
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040055FD RID: 22013
		[SerializeField]
		private ZoneId m_zoneId;

		// Token: 0x040055FE RID: 22014
		[SerializeField]
		private Image m_image;

		// Token: 0x040055FF RID: 22015
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04005600 RID: 22016
		[SerializeField]
		private WarpTextMeshPro m_labelWarp;

		// Token: 0x04005601 RID: 22017
		[SerializeField]
		private bool m_showFullName;

		// Token: 0x04005602 RID: 22018
		[SerializeField]
		private bool m_isWorldMapZoneLabel;

		// Token: 0x04005603 RID: 22019
		private MapUI m_controller;

		// Token: 0x04005604 RID: 22020
		private bool m_canInteract;

		// Token: 0x04005605 RID: 22021
		private bool m_targetZoneIsDiscovered;

		// Token: 0x04005606 RID: 22022
		private bool m_mouseWithin;

		// Token: 0x04005607 RID: 22023
		private float m_pointerUpTime;
	}
}
