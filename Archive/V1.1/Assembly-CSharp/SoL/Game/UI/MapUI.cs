using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.AssetBundles;
using SoL.Game.Audio;
using SoL.Game.Discovery;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008A8 RID: 2216
	public class MapUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IAddressableSpawnedNotifier
	{
		// Token: 0x140000C1 RID: 193
		// (add) Token: 0x060040AA RID: 16554 RVA: 0x0018CCE4 File Offset: 0x0018AEE4
		// (remove) Token: 0x060040AB RID: 16555 RVA: 0x0018CD18 File Offset: 0x0018AF18
		public static event Action HighlightedDiscoveriesChanged;

		// Token: 0x17000ED8 RID: 3800
		// (get) Token: 0x060040AC RID: 16556 RVA: 0x0006BC30 File Offset: 0x00069E30
		internal MapOverlay Overlay
		{
			get
			{
				return this.m_mapOverlay;
			}
		}

		// Token: 0x17000ED9 RID: 3801
		// (get) Token: 0x060040AD RID: 16557 RVA: 0x0006BC38 File Offset: 0x00069E38
		internal UIWindow Window
		{
			get
			{
				return this.m_window;
			}
		}

		// Token: 0x17000EDA RID: 3802
		// (get) Token: 0x060040AE RID: 16558 RVA: 0x0006BC40 File Offset: 0x00069E40
		public bool Visible
		{
			get
			{
				return this.m_window && this.m_window.Visible;
			}
		}

		// Token: 0x17000EDB RID: 3803
		// (get) Token: 0x060040AF RID: 16559 RVA: 0x0006BC5C File Offset: 0x00069E5C
		private bool m_allowZoom
		{
			get
			{
				return !this.m_preventZoom;
			}
		}

		// Token: 0x060040B0 RID: 16560 RVA: 0x0018CD4C File Offset: 0x0018AF4C
		private void Awake()
		{
			this.m_window.ShowCallback = new Action(this.OnShow);
			this.m_window.PostShowCallback += this.OnPostShow;
			this.m_window.HideCallback = new Action(this.OnHide);
			if (this.m_mapImage)
			{
				this.m_mapImage.enabled = false;
			}
			if (this.m_mapLabel)
			{
				this.m_mapLabel.ZStringSetText("Map");
			}
			if (this.m_dropdown)
			{
				this.m_dropdown.ClearOptions();
			}
			SceneCompositionManager.ZoneLoaded += this.SceneCompositionManagerOnZoneLoaded;
			if (LocalZoneManager.ZoneRecord != null)
			{
				this.SceneCompositionManagerOnZoneLoaded((ZoneId)LocalZoneManager.ZoneRecord.ZoneId);
			}
			if (this.m_worldMap)
			{
				this.m_worldMap.onClick.AddListener(new UnityAction(this.WorldMapClicked));
			}
			if (this.m_zoomIn)
			{
				this.m_zoomIn.onClick.AddListener(new UnityAction(this.ZoomInClicked));
			}
			if (this.m_zoomOut)
			{
				this.m_zoomOut.onClick.AddListener(new UnityAction(this.ZoomOutClicked));
			}
			if (this.m_zoomReset)
			{
				this.m_zoomReset.onClick.AddListener(new UnityAction(this.ZoomResetClicked));
			}
			this.m_confirmationCancelButton.onClick.AddListener(new UnityAction(this.OnCancelClicked));
			this.m_confirmationCancelButton.interactable = false;
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.TravelClicked += this.UseTravelClicked;
				this.m_teleportButtonPanel.EssenceClicked += this.UseEssenceClicked;
				this.m_teleportButtonPanel.ToggleInteractivity(false);
			}
		}

		// Token: 0x060040B1 RID: 16561 RVA: 0x0006BC67 File Offset: 0x00069E67
		private void Start()
		{
			DiscoveryProgression.DiscoveryRefresh += this.DiscoveryProgressionOnDiscoveryRefresh;
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
			this.PopulateZoneDropdown();
		}

		// Token: 0x060040B2 RID: 16562 RVA: 0x0018CF28 File Offset: 0x0018B128
		private void OnDestroy()
		{
			DiscoveryProgression.DiscoveryRefresh -= this.DiscoveryProgressionOnDiscoveryRefresh;
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.m_window.ShowCallback = null;
			this.m_window.PostShowCallback -= this.OnPostShow;
			this.m_window.HideCallback = null;
			SceneCompositionManager.ZoneLoaded -= this.SceneCompositionManagerOnZoneLoaded;
			if (this.m_dropdown)
			{
				this.m_dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.ZoneDropdownChanged));
			}
			if (this.m_worldMap)
			{
				this.m_worldMap.onClick.RemoveListener(new UnityAction(this.WorldMapClicked));
			}
			if (this.m_zoomIn)
			{
				this.m_zoomIn.onClick.RemoveListener(new UnityAction(this.ZoomInClicked));
			}
			if (this.m_zoomOut)
			{
				this.m_zoomOut.onClick.RemoveListener(new UnityAction(this.ZoomOutClicked));
			}
			if (this.m_zoomReset)
			{
				this.m_zoomReset.onClick.RemoveListener(new UnityAction(this.ZoomResetClicked));
			}
			this.m_confirmationCancelButton.onClick.RemoveListener(new UnityAction(this.OnCancelClicked));
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.TravelClicked -= this.UseTravelClicked;
				this.m_teleportButtonPanel.EssenceClicked -= this.UseEssenceClicked;
			}
		}

		// Token: 0x060040B3 RID: 16563 RVA: 0x0018D0BC File Offset: 0x0018B2BC
		private void Update()
		{
			if (this.m_pointerInWindow && this.m_allowZoom && this.Visible && Input.mouseScrollDelta.y != 0f)
			{
				if (Input.mouseScrollDelta.y > 0f)
				{
					this.ZoomInClicked();
				}
				else if (Input.mouseScrollDelta.y < 0f)
				{
					this.ZoomOutClicked();
				}
			}
			if (this.m_confirmationPanel.activeInHierarchy && this.m_confirmationAutoCancel != null && this.m_confirmationAutoCancel())
			{
				this.CloseConfirmation(false, false);
			}
		}

		// Token: 0x060040B4 RID: 16564 RVA: 0x0006BC91 File Offset: 0x00069E91
		private void OnLocalPlayerInitialized()
		{
			this.PopulateZoneDropdown();
			this.LoadZoneMap(this.m_currentZoneId, false);
		}

		// Token: 0x060040B5 RID: 16565 RVA: 0x0018D14C File Offset: 0x0018B34C
		private void DiscoveryProgressionOnDiscoveryRefresh()
		{
			if (this.m_zoneIdDropdownMap != null)
			{
				if (this.m_dropdown)
				{
					this.m_dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.ZoneDropdownChanged));
				}
				this.m_zoneIdDropdownMap = null;
			}
			this.PopulateZoneDropdown();
			this.LoadZoneMap(this.m_currentZoneId, false);
		}

		// Token: 0x060040B6 RID: 16566 RVA: 0x0018D1A4 File Offset: 0x0018B3A4
		private void SceneCompositionManagerOnZoneLoaded(ZoneId obj)
		{
			if (this.m_highlightedDiscoveries == null)
			{
				this.m_highlightedDiscoveries = new HashSet<DiscoveryProfile>(default(DiscoveryProfileComparer));
			}
			else
			{
				this.m_highlightedDiscoveries.Clear();
			}
			this.m_currentZoneId = obj;
		}

		// Token: 0x060040B7 RID: 16567 RVA: 0x0018D1E8 File Offset: 0x0018B3E8
		private void LoadZoneMap(ZoneId obj, bool playAudioClip)
		{
			this.SetBackgroundText("Loading...");
			if (this.m_mapOverlay)
			{
				UnityEngine.Object.Destroy(this.m_mapOverlay.gameObject);
				this.m_mapOverlay = null;
			}
			this.m_zoomLevel = 0f;
			this.m_playMapAudio = playAudioClip;
			this.m_currentLoadedMapZoneId = obj;
			AssetReference reference;
			if (GlobalSettings.Values != null && GlobalSettings.Values.Maps != null && GlobalSettings.Values.Maps.TryGetMapPrefab(obj, out reference))
			{
				this.DisableInteractiveElements();
				AddressableManager.SpawnInstance(reference, this.m_mapParent, this);
				return;
			}
			this.SetBackgroundText("No Map Available");
		}

		// Token: 0x060040B8 RID: 16568 RVA: 0x0018D28C File Offset: 0x0018B48C
		private void LoadWorldMap(bool playAudioClip)
		{
			this.SetBackgroundText("Loading...");
			if (this.m_mapOverlay)
			{
				UnityEngine.Object.Destroy(this.m_mapOverlay.gameObject);
				this.m_mapOverlay = null;
			}
			this.m_zoomLevel = 0f;
			this.m_playMapAudio = playAudioClip;
			this.m_currentLoadedMapZoneId = ZoneId.None;
			if (GlobalSettings.Values != null && GlobalSettings.Values.Maps != null && GlobalSettings.Values.Maps.WorldMap != null)
			{
				this.DisableInteractiveElements();
				AddressableManager.SpawnInstance(GlobalSettings.Values.Maps.WorldMap, this.m_mapParent, this);
				return;
			}
			this.SetBackgroundText("No Map Available");
		}

		// Token: 0x060040B9 RID: 16569 RVA: 0x0018D338 File Offset: 0x0018B538
		internal void ZoneLinePoiClicked(ZoneId zid)
		{
			if (this.m_zoneIdDropdownMap != null)
			{
				foreach (KeyValuePair<int, ZoneId> keyValuePair in this.m_zoneIdDropdownMap)
				{
					if (keyValuePair.Value == zid)
					{
						this.m_dropdown.value = keyValuePair.Key;
						break;
					}
				}
			}
		}

		// Token: 0x060040BA RID: 16570 RVA: 0x0006BCA6 File Offset: 0x00069EA6
		public void ToggleWindow()
		{
			if (this.m_window)
			{
				this.m_window.ToggleWindow();
			}
		}

		// Token: 0x060040BB RID: 16571 RVA: 0x0006BCC0 File Offset: 0x00069EC0
		private void OnShow()
		{
			if (this.m_mapOverlay)
			{
				this.m_mapOverlay.gameObject.SetActive(false);
			}
			if (this.m_mapImage)
			{
				this.m_mapImage.enabled = true;
			}
		}

		// Token: 0x060040BC RID: 16572 RVA: 0x0018D3AC File Offset: 0x0018B5AC
		private void OnPostShow()
		{
			if (this.m_mapOverlay)
			{
				this.m_mapOverlay.gameObject.SetActive(true);
				this.m_mapOverlay.RefreshDiscoveries();
			}
			if (this.m_mapImage)
			{
				this.m_mapImage.enabled = false;
			}
		}

		// Token: 0x060040BD RID: 16573 RVA: 0x0018D3FC File Offset: 0x0018B5FC
		private void OnHide()
		{
			if (this.m_mapImage)
			{
				this.m_mapImage.enabled = true;
			}
			if (this.m_mapOverlay)
			{
				this.m_mapOverlay.gameObject.SetActive(false);
			}
			if (this.m_confirmationPanel.activeInHierarchy)
			{
				this.CloseConfirmation(false, false);
			}
		}

		// Token: 0x060040BE RID: 16574 RVA: 0x0004475B File Offset: 0x0004295B
		private void OnPostHide()
		{
		}

		// Token: 0x060040BF RID: 16575 RVA: 0x0006BCF9 File Offset: 0x00069EF9
		public void EnableDiscoveryHighlight(DiscoveryProfile profile)
		{
			HashSet<DiscoveryProfile> highlightedDiscoveries = this.m_highlightedDiscoveries;
			if (highlightedDiscoveries != null)
			{
				highlightedDiscoveries.Add(profile);
			}
			Action highlightedDiscoveriesChanged = MapUI.HighlightedDiscoveriesChanged;
			if (highlightedDiscoveriesChanged == null)
			{
				return;
			}
			highlightedDiscoveriesChanged();
		}

		// Token: 0x060040C0 RID: 16576 RVA: 0x0006BD1D File Offset: 0x00069F1D
		public void DisableDiscoveryHighlight(DiscoveryProfile profile)
		{
			HashSet<DiscoveryProfile> highlightedDiscoveries = this.m_highlightedDiscoveries;
			if (highlightedDiscoveries != null)
			{
				highlightedDiscoveries.Remove(profile);
			}
			Action highlightedDiscoveriesChanged = MapUI.HighlightedDiscoveriesChanged;
			if (highlightedDiscoveriesChanged == null)
			{
				return;
			}
			highlightedDiscoveriesChanged();
		}

		// Token: 0x060040C1 RID: 16577 RVA: 0x0018D458 File Offset: 0x0018B658
		private void ZoneDropdownChanged(int arg0)
		{
			if (arg0 == 0)
			{
				this.LoadWorldMap(true);
				return;
			}
			ZoneId obj;
			if (this.m_zoneIdDropdownMap != null && this.m_zoneIdDropdownMap.TryGetValue(arg0, out obj))
			{
				this.LoadZoneMap(obj, true);
			}
		}

		// Token: 0x060040C2 RID: 16578 RVA: 0x0018D490 File Offset: 0x0018B690
		internal void PopulateZoneDropdown()
		{
			if (!this.m_dropdown || this.m_zoneIdDropdownMap != null || !LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null)
			{
				return;
			}
			bool flag = false;
			Dictionary<ZoneId, List<UniqueId>> discoveries = LocalPlayer.GameEntity.CollectionController.Record.Discoveries;
			List<MapUI.ZoneData> list = new List<MapUI.ZoneData>(10);
			for (int i = 0; i < ZoneIdExtensions.MapDropdownOrder.Length; i++)
			{
				ZoneRecord zoneRecord = SessionData.GetZoneRecord(ZoneIdExtensions.MapDropdownOrder[i]);
				if (zoneRecord != null)
				{
					MapUI.ZoneData zoneData = new MapUI.ZoneData
					{
						Id = ZoneIdExtensions.MapDropdownOrder[i],
						DisplayName = zoneRecord.DisplayName
					};
					if (this.m_currentZoneId.IsMatchingZoneForMap(zoneData.Id))
					{
						flag = true;
						zoneData.DisplayName = ZString.Format<string>("{0} (Current)", zoneData.DisplayName);
						list.Insert(0, zoneData);
					}
					else if (discoveries != null && discoveries.ContainsKey(zoneData.Id))
					{
						list.Add(zoneData);
					}
				}
			}
			if (!flag)
			{
				list.Insert(0, new MapUI.ZoneData
				{
					Id = ZoneId.None,
					DisplayName = "Current (NO MAP)"
				});
			}
			list.Insert(0, new MapUI.ZoneData
			{
				Id = ZoneId.None,
				DisplayName = "World"
			});
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			this.m_zoneIdDropdownMap = new Dictionary<int, ZoneId>(list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				fromPool.Add(list[j].DisplayName);
				this.m_zoneIdDropdownMap.Add(j, list[j].Id);
			}
			this.m_dropdown.AddOptions(fromPool);
			this.m_dropdown.value = 1;
			StaticListPool<string>.ReturnToPool(fromPool);
			this.m_dropdown.onValueChanged.AddListener(new UnityAction<int>(this.ZoneDropdownChanged));
		}

		// Token: 0x060040C3 RID: 16579 RVA: 0x0006BD41 File Offset: 0x00069F41
		private void WorldMapClicked()
		{
			if (this.m_dropdown)
			{
				this.m_dropdown.value = ((this.m_dropdown.value == 0) ? 1 : 0);
			}
		}

		// Token: 0x060040C4 RID: 16580 RVA: 0x0006BD6C File Offset: 0x00069F6C
		internal void ForceLoadWorldMap()
		{
			if (this.m_dropdown && this.m_dropdown.value != 0)
			{
				this.m_dropdown.value = 0;
			}
		}

		// Token: 0x140000C2 RID: 194
		// (add) Token: 0x060040C5 RID: 16581 RVA: 0x0018D688 File Offset: 0x0018B888
		// (remove) Token: 0x060040C6 RID: 16582 RVA: 0x0018D6C0 File Offset: 0x0018B8C0
		internal event Action<float> ZoomLevelChanged;

		// Token: 0x17000EDC RID: 3804
		// (get) Token: 0x060040C7 RID: 16583 RVA: 0x0006BD94 File Offset: 0x00069F94
		// (set) Token: 0x060040C8 RID: 16584 RVA: 0x0006BD9C File Offset: 0x00069F9C
		internal float ZoomLevel
		{
			get
			{
				return this.m_zoomLevel;
			}
			set
			{
				if (this.m_zoomLevel == value)
				{
					return;
				}
				this.m_zoomLevel = value;
				this.RefreshZoomButtons();
				Action<float> zoomLevelChanged = this.ZoomLevelChanged;
				if (zoomLevelChanged == null)
				{
					return;
				}
				zoomLevelChanged(this.m_zoomLevel);
			}
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x0006BDCB File Offset: 0x00069FCB
		private void ZoomInClicked()
		{
			if (this.m_mapOverlay && this.m_allowZoom)
			{
				this.ZoomLevel = Mathf.Clamp01(this.ZoomLevel + 0.1f);
			}
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x0006BDF9 File Offset: 0x00069FF9
		private void ZoomOutClicked()
		{
			if (this.m_mapOverlay && this.m_allowZoom)
			{
				this.ZoomLevel = Mathf.Clamp01(this.ZoomLevel - 0.1f);
			}
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x0006BE27 File Offset: 0x0006A027
		private void ZoomResetClicked()
		{
			this.ZoomLevel = 0f;
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x0018D6F8 File Offset: 0x0018B8F8
		private void RefreshZoomButtons()
		{
			bool interactable = false;
			bool interactable2 = false;
			bool interactable3 = false;
			if (this.m_mapOverlay && this.m_allowZoom)
			{
				if (this.m_zoomLevel <= 0f)
				{
					interactable = true;
				}
				else if (this.m_zoomLevel >= 1f)
				{
					interactable2 = true;
					interactable3 = true;
				}
				else
				{
					interactable = true;
					interactable2 = true;
					interactable3 = true;
				}
			}
			if (this.m_zoomIn)
			{
				this.m_zoomIn.interactable = interactable;
			}
			if (this.m_zoomOut)
			{
				this.m_zoomOut.interactable = interactable2;
			}
			if (this.m_zoomReset)
			{
				this.m_zoomReset.interactable = interactable3;
			}
		}

		// Token: 0x060040CD RID: 16589 RVA: 0x0006BE34 File Offset: 0x0006A034
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_pointerInWindow = true;
		}

		// Token: 0x060040CE RID: 16590 RVA: 0x0006BE3D File Offset: 0x0006A03D
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_pointerInWindow = false;
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x0018D798 File Offset: 0x0018B998
		private void DisableInteractiveElements()
		{
			if (this.m_worldMap)
			{
				this.m_worldMap.interactable = false;
			}
			if (this.m_dropdown)
			{
				this.m_dropdown.interactable = false;
			}
			if (this.m_zoomIn)
			{
				this.m_zoomIn.interactable = false;
			}
			if (this.m_zoomOut)
			{
				this.m_zoomOut.interactable = false;
			}
			if (this.m_zoomReset)
			{
				this.m_zoomReset.interactable = false;
			}
			this.m_preventZoom = true;
		}

		// Token: 0x060040D0 RID: 16592 RVA: 0x0018D82C File Offset: 0x0018BA2C
		private void RefreshInteractiveElements()
		{
			if (this.m_worldMap)
			{
				this.m_worldMap.interactable = true;
			}
			if (this.m_dropdown)
			{
				this.m_dropdown.interactable = true;
			}
			this.m_preventZoom = false;
			this.RefreshZoomButtons();
		}

		// Token: 0x060040D1 RID: 16593 RVA: 0x0006BE46 File Offset: 0x0006A046
		private void SetBackgroundText(string txt)
		{
			if (this.m_backgroundLabel)
			{
				this.m_backgroundLabel.ZStringSetText(txt);
			}
		}

		// Token: 0x060040D2 RID: 16594 RVA: 0x0018D878 File Offset: 0x0018BA78
		void IAddressableSpawnedNotifier.NotifyOfSpawn()
		{
			this.m_mapOverlay = this.m_mapParent.GetComponentInChildren<MapOverlay>();
			if (this.m_mapOverlay)
			{
				if (this.m_dropdown.value == 0)
				{
					this.m_mapOverlay.Init(this, ZoneId.None);
				}
				else
				{
					this.m_mapOverlay.Init(this, this.m_currentLoadedMapZoneId);
				}
				this.m_mapOverlay.gameObject.SetActive(false);
				this.SetBackgroundText(string.Empty);
				if (this.m_playMapAudio && ClientGameManager.UIManager)
				{
					ClientGameManager.UIManager.PlayRandomClip(this.m_loadMapCollection, null);
				}
			}
			else
			{
				this.SetBackgroundText("No Map Available");
			}
			if (this.m_worldMap)
			{
				this.m_worldMap.interactable = true;
			}
			if (this.m_dropdown)
			{
				this.m_dropdown.interactable = true;
			}
			this.RefreshInteractiveElements();
			this.OnPostShow();
			if (this.m_mapOverlay && this.m_mapOverlay.RequiresTextWarp)
			{
				base.StartCoroutine("WarpTextCo");
			}
		}

		// Token: 0x060040D3 RID: 16595 RVA: 0x0006BE61 File Offset: 0x0006A061
		private IEnumerator WarpTextCo()
		{
			if (this.m_mapOverlay)
			{
				yield return null;
				this.m_mapOverlay.WarpText();
			}
			yield break;
		}

		// Token: 0x060040D4 RID: 16596 RVA: 0x0018D98C File Offset: 0x0018BB8C
		public static byte GetGroupEmberRingIndex()
		{
			if (GlobalSettings.Values && GlobalSettings.Values.Ashen != null && GlobalSettings.Values.Ashen.AllowGroupTeleportsFromMonolith && ClientGameManager.UIManager && ClientGameManager.UIManager.MapUI && ClientGameManager.UIManager.MapUI.m_highlightedDiscoveries != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				foreach (DiscoveryProfile discoveryProfile in ClientGameManager.UIManager.MapUI.m_highlightedDiscoveries)
				{
					if (discoveryProfile)
					{
						LeyLinkProfile leyLinkProfile = discoveryProfile as LeyLinkProfile;
						byte result;
						if (leyLinkProfile != null && LocalPlayer.GameEntity.CharacterData.AdventuringLevel >= leyLinkProfile.LevelRequirement && GlobalSettings.Values.Ashen.TryGetLeyLinkIndex(leyLinkProfile, out result))
						{
							return result;
						}
					}
				}
				return 0;
			}
			return 0;
		}

		// Token: 0x060040D5 RID: 16597 RVA: 0x0006BE70 File Offset: 0x0006A070
		internal bool HighlightedDiscoveriesContains(DiscoveryProfile profile)
		{
			return this.m_highlightedDiscoveries != null && profile && this.m_highlightedDiscoveries.Contains(profile);
		}

		// Token: 0x060040D6 RID: 16598 RVA: 0x0018DAB0 File Offset: 0x0018BCB0
		internal bool IsNearActiveMonolith()
		{
			if (this.m_highlightedDiscoveries != null)
			{
				foreach (DiscoveryProfile discoveryProfile in this.m_highlightedDiscoveries)
				{
					MonolithProfile monolithProfile = discoveryProfile as MonolithProfile;
					if (monolithProfile != null && monolithProfile.IsAvailable())
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060040D7 RID: 16599 RVA: 0x0018DB1C File Offset: 0x0018BD1C
		internal bool TryGetHighlightedMonolithDiscoveryProfile(out MonolithProfile profile)
		{
			profile = null;
			if (this.m_highlightedDiscoveries != null)
			{
				foreach (DiscoveryProfile discoveryProfile in this.m_highlightedDiscoveries)
				{
					MonolithProfile monolithProfile = discoveryProfile as MonolithProfile;
					if (monolithProfile != null)
					{
						profile = monolithProfile;
						break;
					}
				}
			}
			return profile != null;
		}

		// Token: 0x060040D8 RID: 16600 RVA: 0x0018DB88 File Offset: 0x0018BD88
		internal void InitConfirmationDialog(string txt, int cost, Action<bool, bool> callback, Func<bool> autoCancel)
		{
			this.DisableInteractiveElements();
			this.m_confirmationLabel.ZStringSetText(txt);
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.InitButtons(cost);
			}
			this.m_confirmationCancelButton.text = "Cancel";
			this.m_confirmationCancelButton.interactable = true;
			this.m_confirmationCallback = callback;
			this.m_confirmationAutoCancel = autoCancel;
			this.m_confirmationPanel.SetActive(true);
		}

		// Token: 0x060040D9 RID: 16601 RVA: 0x0006BE90 File Offset: 0x0006A090
		private void OnAcceptClicked()
		{
			this.CloseConfirmation(true, true);
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x0006BE90 File Offset: 0x0006A090
		private void UseTravelClicked()
		{
			this.CloseConfirmation(true, true);
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x0006BE9A File Offset: 0x0006A09A
		private void UseEssenceClicked()
		{
			this.CloseConfirmation(true, false);
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x0006BEA4 File Offset: 0x0006A0A4
		private void OnCancelClicked()
		{
			this.CloseConfirmation(false, false);
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x0018DBF8 File Offset: 0x0018BDF8
		private void CloseConfirmation(bool answer, bool useTravelEssence)
		{
			this.m_confirmationCancelButton.interactable = false;
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.ToggleInteractivity(false);
			}
			Action<bool, bool> confirmationCallback = this.m_confirmationCallback;
			if (confirmationCallback != null)
			{
				confirmationCallback(answer, useTravelEssence);
			}
			this.m_confirmationCallback = null;
			this.m_confirmationAutoCancel = null;
			this.m_confirmationPanel.SetActive(false);
			this.RefreshInteractiveElements();
		}

		// Token: 0x04003E55 RID: 15957
		internal const float kPointerClickTimeWindow = 0.2f;

		// Token: 0x04003E56 RID: 15958
		private const string kDefaultTitle = "Map";

		// Token: 0x04003E57 RID: 15959
		private const int kWorldMapIndex = 0;

		// Token: 0x04003E58 RID: 15960
		private const string kLoadingText = "Loading...";

		// Token: 0x04003E59 RID: 15961
		private const string kNoMapText = "No Map Available";

		// Token: 0x04003E5B RID: 15963
		private HashSet<DiscoveryProfile> m_highlightedDiscoveries;

		// Token: 0x04003E5C RID: 15964
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04003E5D RID: 15965
		[SerializeField]
		private Image m_mapImage;

		// Token: 0x04003E5E RID: 15966
		[SerializeField]
		private RectTransform m_mapParent;

		// Token: 0x04003E5F RID: 15967
		[SerializeField]
		private TextMeshProUGUI m_mapLabel;

		// Token: 0x04003E60 RID: 15968
		[SerializeField]
		private TextMeshProUGUI m_backgroundLabel;

		// Token: 0x04003E61 RID: 15969
		[SerializeField]
		private TMP_Dropdown m_dropdown;

		// Token: 0x04003E62 RID: 15970
		[SerializeField]
		private SolButton m_worldMap;

		// Token: 0x04003E63 RID: 15971
		[SerializeField]
		private SolButton m_zoomIn;

		// Token: 0x04003E64 RID: 15972
		[SerializeField]
		private SolButton m_zoomOut;

		// Token: 0x04003E65 RID: 15973
		[SerializeField]
		private SolButton m_zoomReset;

		// Token: 0x04003E66 RID: 15974
		[SerializeField]
		private AudioClipCollection m_loadMapCollection;

		// Token: 0x04003E67 RID: 15975
		[SerializeField]
		private GameObject m_confirmationPanel;

		// Token: 0x04003E68 RID: 15976
		[SerializeField]
		private TextMeshProUGUI m_confirmationLabel;

		// Token: 0x04003E69 RID: 15977
		[SerializeField]
		private SolButton m_confirmationAcceptButton;

		// Token: 0x04003E6A RID: 15978
		[SerializeField]
		private SolButton m_confirmationCancelButton;

		// Token: 0x04003E6B RID: 15979
		[SerializeField]
		private TeleportButtonPanel m_teleportButtonPanel;

		// Token: 0x04003E6C RID: 15980
		private MapOverlay m_mapOverlay;

		// Token: 0x04003E6D RID: 15981
		private ZoneId m_currentZoneId;

		// Token: 0x04003E6E RID: 15982
		private bool m_pointerInWindow;

		// Token: 0x04003E6F RID: 15983
		private bool m_preventZoom;

		// Token: 0x04003E70 RID: 15984
		private bool m_playMapAudio;

		// Token: 0x04003E71 RID: 15985
		private ZoneId m_currentLoadedMapZoneId;

		// Token: 0x04003E72 RID: 15986
		private Action<bool, bool> m_confirmationCallback;

		// Token: 0x04003E73 RID: 15987
		private Func<bool> m_confirmationAutoCancel;

		// Token: 0x04003E74 RID: 15988
		private Dictionary<int, ZoneId> m_zoneIdDropdownMap;

		// Token: 0x04003E75 RID: 15989
		private const float kDelta = 0.1f;

		// Token: 0x04003E77 RID: 15991
		private float m_zoomLevel;

		// Token: 0x020008A9 RID: 2217
		private struct ZoneData
		{
			// Token: 0x04003E78 RID: 15992
			public ZoneId Id;

			// Token: 0x04003E79 RID: 15993
			public string DisplayName;
		}
	}
}
