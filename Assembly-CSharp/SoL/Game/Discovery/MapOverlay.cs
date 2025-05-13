using System;
using System.Collections.Generic;
using SoL.Game.UI;
using SoL.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CAE RID: 3246
	public class MapOverlay : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		// Token: 0x17001784 RID: 6020
		// (get) Token: 0x0600626B RID: 25195 RVA: 0x0008240E File Offset: 0x0008060E
		// (set) Token: 0x0600626C RID: 25196 RVA: 0x00082416 File Offset: 0x00080616
		internal bool RequiresTextWarp { get; private set; }

		// Token: 0x0600626D RID: 25197 RVA: 0x00204988 File Offset: 0x00202B88
		private void Awake()
		{
			this.m_rect = base.gameObject.GetComponent<RectTransform>();
			this.m_currentDiscoveries = new HashSet<UniqueId>(default(UniqueIdComparer));
			DiscoveryProgression.DiscoveryFound += this.DiscoveryProgressionOnDiscoveryFound;
			DiscoveryProgression.DiscoveryRefresh += this.RefreshDiscoveries;
			MapUI.HighlightedDiscoveriesChanged += this.RefreshHighlightedDiscoveries;
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x0008241F File Offset: 0x0008061F
		private void Start()
		{
			if (LocalPlayer.IsInitialized)
			{
				this.RefreshAll();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x002049F4 File Offset: 0x00202BF4
		private void OnDestroy()
		{
			DiscoveryProgression.DiscoveryFound -= this.DiscoveryProgressionOnDiscoveryFound;
			DiscoveryProgression.DiscoveryRefresh -= this.RefreshDiscoveries;
			MapUI.HighlightedDiscoveriesChanged -= this.RefreshHighlightedDiscoveries;
			if (this.m_controller)
			{
				this.m_controller.ZoomLevelChanged -= this.OnZoomLevelChanged;
			}
			this.m_controller = null;
		}

		// Token: 0x06006270 RID: 25200 RVA: 0x00082440 File Offset: 0x00080640
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.RefreshAll();
		}

		// Token: 0x06006271 RID: 25201 RVA: 0x00082459 File Offset: 0x00080659
		private void RefreshAll()
		{
			this.RefreshDiscoveries();
			this.RefreshHighlightedDiscoveries();
		}

		// Token: 0x06006272 RID: 25202 RVA: 0x00204A60 File Offset: 0x00202C60
		internal void Init(MapUI controller, ZoneId zoneToLoad)
		{
			this.m_controller = controller;
			this.m_zoneToLoad = zoneToLoad;
			this.m_controller.ZoomLevelChanged += this.OnZoomLevelChanged;
			this.OnZoomLevelChanged(this.m_controller.ZoomLevel);
			bool flag = false;
			this.m_zoneLines = base.gameObject.GetComponentsInChildren<ZoneLineTooltip>();
			if (this.m_zoneLines != null)
			{
				for (int i = 0; i < this.m_zoneLines.Length; i++)
				{
					if (this.m_zoneLines[i])
					{
						this.m_zoneLines[i].Init(this.m_controller);
						flag = (flag || this.m_zoneLines[i].HasLabelWarper);
					}
				}
			}
			this.RequiresTextWarp = flag;
			for (int j = 0; j < this.m_discoveries.Length; j++)
			{
				this.m_discoveries[j].Init(this.m_controller);
			}
		}

		// Token: 0x06006273 RID: 25203 RVA: 0x00204B34 File Offset: 0x00202D34
		internal void WarpText()
		{
			if (this.m_zoneLines != null)
			{
				for (int i = 0; i < this.m_zoneLines.Length; i++)
				{
					if (this.m_zoneLines[i])
					{
						this.m_zoneLines[i].WarpLabel();
					}
				}
			}
			this.RequiresTextWarp = false;
		}

		// Token: 0x06006274 RID: 25204 RVA: 0x00204B80 File Offset: 0x00202D80
		private void DiscoveryProgressionOnDiscoveryFound(DiscoveryProfile obj)
		{
			if (!obj)
			{
				return;
			}
			if (this.m_discoveries == null)
			{
				string str = (base.gameObject && base.gameObject != null) ? base.gameObject.name : "NULL";
				Debug.LogWarning("MapDiscoveries null on " + str + "!");
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.m_discoveries.Length; i++)
			{
				if (!this.m_discoveries[i])
				{
					string text = (base.gameObject && base.gameObject != null) ? base.gameObject.name : "NULL";
					Debug.LogWarning(string.Concat(new string[]
					{
						"MapDiscovery at index ",
						i.ToString(),
						" null for ",
						text,
						"!"
					}));
				}
				else if (this.m_discoveries[i].Profile && this.m_discoveries[i].Profile.Id == obj.Id)
				{
					this.m_discoveries[i].SetDiscovered();
					flag = true;
				}
			}
			if (flag)
			{
				UIManager.InvokeTriggerControlPanelUsageHighlight(WindowToggler.WindowType.Map);
			}
			this.RefreshZoneLines();
		}

		// Token: 0x06006275 RID: 25205 RVA: 0x00204CC4 File Offset: 0x00202EC4
		public void RefreshDiscoveries()
		{
			if (!LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null || LocalPlayer.GameEntity.CollectionController.Record.Discoveries == null)
			{
				for (int i = 0; i < this.m_discoveries.Length; i++)
				{
					this.m_discoveries[i].SetUndiscovered();
				}
				if (this.m_zoneLines != null)
				{
					for (int j = 0; j < this.m_zoneLines.Length; j++)
					{
						if (this.m_zoneLines[j])
						{
							this.m_zoneLines[j].RefreshDiscovered(null, this.m_zoneToLoad);
						}
					}
				}
				return;
			}
			if (this.m_controller)
			{
				this.m_controller.PopulateZoneDropdown();
			}
			ZoneId zoneToLoad = this.m_zoneToLoad;
			this.m_currentDiscoveries.Clear();
			if (zoneToLoad <= ZoneId.RedshoreForest)
			{
				if (zoneToLoad == ZoneId.None)
				{
					this.AddAllDiscoveries();
					goto IL_136;
				}
				if (zoneToLoad != ZoneId.RedshoreForest)
				{
					goto IL_12F;
				}
			}
			else
			{
				if (zoneToLoad == ZoneId.Dryfoot || zoneToLoad == ZoneId.DryfootStronghold)
				{
					this.AddZoneDiscoveriesToCurrentDiscoveries(ZoneId.Dryfoot);
					this.AddZoneDiscoveriesToCurrentDiscoveries(ZoneId.DryfootStronghold);
					goto IL_136;
				}
				if (zoneToLoad != ZoneId.RedshoreRidge)
				{
					goto IL_12F;
				}
			}
			this.AddZoneDiscoveriesToCurrentDiscoveries(ZoneId.RedshoreForest);
			this.AddZoneDiscoveriesToCurrentDiscoveries(ZoneId.RedshoreRidge);
			goto IL_136;
			IL_12F:
			this.AddZoneDiscoveriesToCurrentDiscoveries(zoneToLoad);
			IL_136:
			for (int k = 0; k < this.m_discoveries.Length; k++)
			{
				if ((this.m_discoveries[k].Profile && this.m_currentDiscoveries.Contains(this.m_discoveries[k].Profile.Id)) || (this.m_discoveries[k].ZoneId != ZoneId.None && LocalPlayer.GameEntity.CollectionController.Record.Discoveries.ContainsKey(this.m_discoveries[k].ZoneId)))
				{
					this.m_discoveries[k].SetDiscovered();
				}
				else
				{
					this.m_discoveries[k].SetUndiscovered();
				}
			}
			this.RefreshZoneLines();
		}

		// Token: 0x06006276 RID: 25206 RVA: 0x00204EAC File Offset: 0x002030AC
		private void RefreshZoneLines()
		{
			if (this.m_zoneLines != null)
			{
				for (int i = 0; i < this.m_zoneLines.Length; i++)
				{
					if (this.m_zoneLines[i])
					{
						this.m_zoneLines[i].RefreshDiscovered(LocalPlayer.GameEntity.CollectionController.Record.Discoveries, this.m_zoneToLoad);
					}
				}
			}
		}

		// Token: 0x06006277 RID: 25207 RVA: 0x00204F0C File Offset: 0x0020310C
		private void AddZoneDiscoveriesToCurrentDiscoveries(ZoneId zoneId)
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null)
			{
				return;
			}
			Dictionary<ZoneId, List<UniqueId>> discoveries = LocalPlayer.GameEntity.CollectionController.Record.Discoveries;
			List<UniqueId> list;
			if (discoveries != null && discoveries.TryGetValue(zoneId, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.m_currentDiscoveries.Add(list[i]);
				}
			}
		}

		// Token: 0x06006278 RID: 25208 RVA: 0x00204F8C File Offset: 0x0020318C
		private void AddAllDiscoveries()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null)
			{
				return;
			}
			Dictionary<ZoneId, List<UniqueId>> discoveries = LocalPlayer.GameEntity.CollectionController.Record.Discoveries;
			if (discoveries != null)
			{
				foreach (KeyValuePair<ZoneId, List<UniqueId>> keyValuePair in discoveries)
				{
					foreach (UniqueId item in keyValuePair.Value)
					{
						this.m_currentDiscoveries.Add(item);
					}
				}
			}
		}

		// Token: 0x06006279 RID: 25209 RVA: 0x00205064 File Offset: 0x00203264
		private void RefreshHighlightedDiscoveries()
		{
			if (ClientGameManager.UIManager == null || ClientGameManager.UIManager.MapUI == null)
			{
				return;
			}
			bool isNearMonolith = ClientGameManager.UIManager.MapUI.IsNearActiveMonolith();
			for (int i = 0; i < this.m_discoveries.Length; i++)
			{
				this.m_discoveries[i].RefreshHighlightVisuals(isNearMonolith);
			}
		}

		// Token: 0x0600627A RID: 25210 RVA: 0x002050C4 File Offset: 0x002032C4
		private void OnZoomLevelChanged(float obj)
		{
			float num = Mathf.Lerp(1f, 4f, obj);
			this.m_rect.localScale = Vector3.one * num;
			float num2 = Mathf.Lerp(0f, 1200f, (num - 1f) / 3f);
			Vector3 localPosition = this.m_rect.localPosition;
			localPosition.x = Mathf.Clamp(localPosition.x, -num2, num2);
			localPosition.y = Mathf.Clamp(localPosition.y, -num2, num2);
			this.m_rect.localPosition = localPosition;
		}

		// Token: 0x0600627B RID: 25211 RVA: 0x00082467 File Offset: 0x00080667
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.m_isDragging = true;
			}
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x00082478 File Offset: 0x00080678
		public void OnEndDrag(PointerEventData eventData)
		{
			this.m_isDragging = false;
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x00205158 File Offset: 0x00203358
		public void OnDrag(PointerEventData eventData)
		{
			if (this.m_isDragging)
			{
				float x = this.m_rect.localScale.x;
				Vector3 localPosition = this.m_rect.localPosition;
				Vector2 vector = new Vector2(localPosition.x, localPosition.y) + eventData.delta;
				float num = Mathf.Lerp(0f, 1200f, (x - 1f) / 3f);
				vector.x = Mathf.Clamp(vector.x, -num, num);
				vector.y = Mathf.Clamp(vector.y, -num, num);
				this.m_rect.localPosition = vector;
			}
		}

		// Token: 0x0600627E RID: 25214 RVA: 0x00082481 File Offset: 0x00080681
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.m_controller && eventData.button == PointerEventData.InputButton.Right)
			{
				this.m_pointerUpTime = Time.time + 0.2f;
			}
		}

		// Token: 0x0600627F RID: 25215 RVA: 0x000824AA File Offset: 0x000806AA
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.m_controller && eventData.button == PointerEventData.InputButton.Right && Time.time < this.m_pointerUpTime)
			{
				this.m_controller.ForceLoadWorldMap();
			}
		}

		// Token: 0x040055EA RID: 21994
		private const float kMaxSize = 4f;

		// Token: 0x040055EB RID: 21995
		[SerializeField]
		private MapDiscovery[] m_discoveries;

		// Token: 0x040055EC RID: 21996
		private HashSet<UniqueId> m_currentDiscoveries;

		// Token: 0x040055ED RID: 21997
		private MapUI m_controller;

		// Token: 0x040055EE RID: 21998
		private ZoneId m_zoneToLoad;

		// Token: 0x040055EF RID: 21999
		private RectTransform m_rect;

		// Token: 0x040055F0 RID: 22000
		private bool m_isDragging;

		// Token: 0x040055F1 RID: 22001
		private ZoneLineTooltip[] m_zoneLines;

		// Token: 0x040055F3 RID: 22003
		private float m_pointerUpTime;
	}
}
