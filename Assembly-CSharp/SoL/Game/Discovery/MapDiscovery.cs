using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Quests;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CAB RID: 3243
	public class MapDiscovery : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17001779 RID: 6009
		// (get) Token: 0x06006244 RID: 25156 RVA: 0x000822DF File Offset: 0x000804DF
		internal ZoneId ZoneId
		{
			get
			{
				return this.m_zoneId;
			}
		}

		// Token: 0x1700177A RID: 6010
		// (get) Token: 0x06006245 RID: 25157 RVA: 0x000822A0 File Offset: 0x000804A0
		private IEnumerable GetDiscoveryProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<DiscoveryProfile>();
			}
		}

		// Token: 0x1700177B RID: 6011
		// (get) Token: 0x06006246 RID: 25158 RVA: 0x000822E7 File Offset: 0x000804E7
		private IEnumerable GetBulletinBoards
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BulletinBoard>();
			}
		}

		// Token: 0x1700177C RID: 6012
		// (get) Token: 0x06006247 RID: 25159 RVA: 0x000822EE File Offset: 0x000804EE
		public DiscoveryProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x1700177D RID: 6013
		// (get) Token: 0x06006248 RID: 25160 RVA: 0x000822F6 File Offset: 0x000804F6
		public bool IsDiscovered
		{
			get
			{
				return this.m_isDiscovered;
			}
		}

		// Token: 0x06006249 RID: 25161 RVA: 0x00203E6C File Offset: 0x0020206C
		private void Awake()
		{
			this.ToggleDiscovered(false, true);
			MapTeleportProfile profile;
			if (this.m_interactButtonParent && this.m_bulletinBoard == null && this.m_profile && this.m_profile.TryGetAsType(out profile) && GlobalSettings.Values.Ashen.AllowEmberRingTeleportsFromMonolith && GlobalSettings.Values.Maps.TeleportButton)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GlobalSettings.Values.Maps.TeleportButton, this.m_interactButtonParent);
				this.m_interactButton = gameObject.GetComponent<SolButton>();
				if (this.m_interactButton)
				{
					this.m_interactButton.onClick.AddListener(new UnityAction(this.InteractButtonClicked));
				}
				MapDiscoveryTeleportTooltip component = gameObject.GetComponent<MapDiscoveryTeleportTooltip>();
				if (component)
				{
					component.Initialize(profile);
				}
			}
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x00203F54 File Offset: 0x00202154
		private void OnDestroy()
		{
			if (this.m_interactButton)
			{
				this.m_interactButton.onClick.RemoveListener(new UnityAction(this.InteractButtonClicked));
				ActivatedMonolithReplicator.ActiveMonolithListChanged -= this.RefreshInteractButton;
			}
			this.m_controller = null;
		}

		// Token: 0x0600624B RID: 25163 RVA: 0x00203FA4 File Offset: 0x002021A4
		public void Init(MapUI controller)
		{
			this.m_controller = controller;
			MonolithProfile monolithProfile;
			if (this.m_controller && this.m_interactButtonParent && this.m_bulletinBoard == null && this.m_profile && this.m_profile.TryGetAsType(out monolithProfile) && LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId != (int)monolithProfile.MonolithFlag.GetZoneIdForFlag() && GlobalSettings.Values.Maps.TeleportButton)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GlobalSettings.Values.Maps.TeleportButton, this.m_interactButtonParent);
				this.m_interactButton = gameObject.GetComponent<SolButton>();
				if (this.m_interactButton)
				{
					this.m_interactButton.onClick.AddListener(new UnityAction(this.InteractButtonClicked));
				}
				ActivatedMonolithReplicator.ActiveMonolithListChanged += this.RefreshInteractButton;
			}
			this.RefreshInteractButton();
		}

		// Token: 0x0600624C RID: 25164 RVA: 0x000822FE File Offset: 0x000804FE
		public void SetDiscovered()
		{
			this.ToggleDiscovered(true, false);
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x00082308 File Offset: 0x00080508
		public void SetUndiscovered()
		{
			this.ToggleDiscovered(false, false);
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x002040AC File Offset: 0x002022AC
		private void ToggleDiscovered(bool isDiscovered, bool instant = false)
		{
			this.m_isDiscovered = isDiscovered;
			if (this.m_fogGroup)
			{
				this.m_fogGroup.SetActive(!isDiscovered);
			}
			if (this.m_poiGroup)
			{
				this.m_poiGroup.SetActive(isDiscovered);
			}
			this.RefreshHighlightGroup();
			if (this.m_fogImage)
			{
				if (this.m_fogImage.sprite == null)
				{
					this.m_fogImage.enabled = false;
					return;
				}
				if (instant)
				{
					Color color = this.m_fogImage.color;
					color.a = (isDiscovered ? 0f : 1f);
					this.m_fogImage.color = color;
					return;
				}
				this.m_fogImage.CrossFadeAlpha(isDiscovered ? 0f : 1f, 0.2f, true);
			}
		}

		// Token: 0x0600624F RID: 25167 RVA: 0x00082312 File Offset: 0x00080512
		internal void RefreshHighlightVisuals(bool isNearMonolith)
		{
			this.m_isNearMonolith = isNearMonolith;
			this.m_isHighlighted = (this.m_profile && ClientGameManager.UIManager.MapUI.HighlightedDiscoveriesContains(this.m_profile));
			this.RefreshHighlightGroup();
			this.RefreshInteractButton();
		}

		// Token: 0x06006250 RID: 25168 RVA: 0x00082352 File Offset: 0x00080552
		private void RefreshHighlightGroup()
		{
			if (this.m_poiHighlight)
			{
				this.m_poiHighlight.enabled = (this.m_isDiscovered && this.m_isHighlighted);
			}
		}

		// Token: 0x06006251 RID: 25169 RVA: 0x0020417C File Offset: 0x0020237C
		private void RefreshInteractButton()
		{
			if (!this.m_interactButton)
			{
				return;
			}
			bool interactable = false;
			bool flag = this.m_isDiscovered && this.m_isNearMonolith && this.m_bulletinBoard == null && this.m_profile && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.CurrentEmberStone != null;
			if (flag)
			{
				MapTeleportProfile mapTeleportProfile = this.m_profile as MapTeleportProfile;
				if (mapTeleportProfile != null)
				{
					DiscoveryMapTeleportDestination discoveryMapTeleportDestination;
					flag = LocalZoneManager.TryGetMapTeleportDestination(this.m_profile.Id, out discoveryMapTeleportDestination);
					if (flag)
					{
						interactable = (LocalPlayer.GameEntity.CollectionController.GetAvailableEmberEssenceForTravel() >= mapTeleportProfile.EssenceCost);
					}
				}
				else if (this.m_controller)
				{
					MonolithProfile monolithProfile = this.m_profile as MonolithProfile;
					if (monolithProfile != null)
					{
						MonolithProfile monolithProfile2;
						flag = (this.m_controller.TryGetHighlightedMonolithDiscoveryProfile(out monolithProfile2) && monolithProfile2 && monolithProfile2.IsAvailable() && monolithProfile.IsAvailable() && LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId != (int)monolithProfile.MonolithFlag.GetZoneIdForFlag());
						if (flag)
						{
							MonolithFlags flags = monolithProfile.MonolithFlag | monolithProfile2.MonolithFlag;
							int monolithCost = GlobalSettings.Values.Ashen.GetMonolithCost(flags);
							interactable = (LocalPlayer.GameEntity.CollectionController.GetAvailableEmberEssenceForTravel() >= monolithCost);
						}
					}
				}
			}
			this.m_interactButton.interactable = interactable;
			this.m_interactButton.gameObject.SetActive(flag);
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x00204314 File Offset: 0x00202514
		private void InteractButtonClicked()
		{
			if (!this.m_interactButton || !this.m_interactButton.interactable || this.m_bulletinBoard || !this.m_profile)
			{
				return;
			}
			MapTeleportProfile mapTeleportProfile;
			if (this.m_profile.TryGetAsType(out mapTeleportProfile))
			{
				this.TeleportRequestWithConfirmation(this.m_profile.DisplayName, mapTeleportProfile.EssenceCost);
				return;
			}
			MonolithProfile monolithProfile;
			MonolithProfile monolithProfile2;
			if (this.m_controller && this.m_profile.TryGetAsType(out monolithProfile) && this.m_controller.TryGetHighlightedMonolithDiscoveryProfile(out monolithProfile2))
			{
				ZoneId zoneIdForFlag = monolithProfile.MonolithFlag.GetZoneIdForFlag();
				if (LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId != (int)zoneIdForFlag)
				{
					ZoneRecord zoneRecord = SessionData.GetZoneRecord(zoneIdForFlag);
					string description = (zoneRecord != null) ? zoneRecord.DisplayName : zoneIdForFlag.ToString();
					MonolithFlags flags = monolithProfile.MonolithFlag | monolithProfile2.MonolithFlag;
					int monolithCost = GlobalSettings.Values.Ashen.GetMonolithCost(flags);
					this.TeleportRequestWithConfirmation(description, monolithCost);
				}
			}
		}

		// Token: 0x06006253 RID: 25171 RVA: 0x0020441C File Offset: 0x0020261C
		private bool CanTeleport()
		{
			if (GameManager.IsServer)
			{
				return false;
			}
			if (!this.m_profile)
			{
				return false;
			}
			if (!LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || !LocalPlayer.NetworkEntity || !LocalPlayer.NetworkEntity.PlayerRpcHandler)
			{
				return false;
			}
			if (LocalPlayer.GameEntity.InCombat)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot use this while in combat!");
				return false;
			}
			if (!LocalZoneManager.HasMonolithInRange(LocalPlayer.GameEntity))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Not near a Monolith!");
				return false;
			}
			int availableEmberEssenceForTravel = LocalPlayer.GameEntity.CollectionController.GetAvailableEmberEssenceForTravel();
			MapTeleportProfile mapTeleportProfile;
			if (this.m_profile.TryGetAsType(out mapTeleportProfile) && availableEmberEssenceForTravel < mapTeleportProfile.EssenceCost)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Not enough essence!");
				return false;
			}
			MonolithProfile monolithProfile;
			if (this.m_controller && this.m_profile.TryGetAsType(out monolithProfile))
			{
				ZoneId zoneIdForFlag = monolithProfile.MonolithFlag.GetZoneIdForFlag();
				if (LocalZoneManager.ZoneRecord == null || LocalZoneManager.ZoneRecord.ZoneId == (int)zoneIdForFlag)
				{
					return false;
				}
				MonolithProfile monolithProfile2;
				if (!this.m_controller || !this.m_controller.TryGetHighlightedMonolithDiscoveryProfile(out monolithProfile2))
				{
					return false;
				}
				if (!monolithProfile2.IsAvailable() || !monolithProfile.IsAvailable())
				{
					this.RefreshInteractButton();
					return false;
				}
				MonolithFlags flags = monolithProfile.MonolithFlag | monolithProfile2.MonolithFlag;
				int monolithCost = GlobalSettings.Values.Ashen.GetMonolithCost(flags);
				if (availableEmberEssenceForTravel < monolithCost)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Not enough essence!");
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006254 RID: 25172 RVA: 0x002045A8 File Offset: 0x002027A8
		private void TeleportRequestWithConfirmation(string description, int cost)
		{
			if (!this.CanTeleport())
			{
				return;
			}
			if (this.m_controller)
			{
				this.m_controller.InitConfirmationDialog(MapDiscovery.GetTeleportConfirmationText(description), cost, new Action<bool, bool>(this.TeleportCallback), new Func<bool>(this.AutoCancel));
			}
		}

		// Token: 0x06006255 RID: 25173 RVA: 0x002045F8 File Offset: 0x002027F8
		public static string GetTeleportConfirmationText(string destinationDescription, int cost)
		{
			int num = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null) ? LocalPlayer.GameEntity.CollectionController.GetDisplayValueForTravelEssence() : 0;
			string arg = string.Empty;
			if (num <= 0)
			{
				arg = ZString.Format<int>("{0} Ember Essence", cost);
			}
			else if (num >= cost)
			{
				arg = ZString.Format<int>("{0} Travel Essence", cost);
			}
			else
			{
				arg = ZString.Format<int, int>("{0} Travel Essence + {1} Ember Essence", num, cost - num);
			}
			return ZString.Format<string, string>("Are you sure you want to teleport to {0} for {1}?", destinationDescription, arg);
		}

		// Token: 0x06006256 RID: 25174 RVA: 0x0008237D File Offset: 0x0008057D
		public static string GetTeleportConfirmationText(string destinationDescription)
		{
			return ZString.Format<string>("Are you sure you want to teleport to {0}?", destinationDescription);
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x00204678 File Offset: 0x00202878
		public static void InitTeleportConfirmationButtons(SolButton travel, SolButton essence, int cost)
		{
			if (!travel || !essence)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				ValueTuple<int, int> emberAndTravelEssenceCounts = LocalPlayer.GameEntity.CollectionController.GetEmberAndTravelEssenceCounts();
				num = emberAndTravelEssenceCounts.Item1;
				num2 = emberAndTravelEssenceCounts.Item2;
			}
			if (cost > num + num2)
			{
				travel.interactable = false;
				travel.SetText("Insufficient Travel + Ember Essence");
				essence.interactable = false;
				essence.SetText("Insufficient Ember Essence");
				return;
			}
			travel.interactable = (num2 > 0);
			string text = "No Travel Essence";
			if (num2 > 0)
			{
				text = ((num2 >= cost) ? ZString.Format<int>("{0} Travel Essence", cost) : ZString.Format<int, int>("{0} Travel + {1} Ember Essence", num2, cost - num2));
			}
			travel.SetText(text);
			essence.interactable = (num >= cost);
			essence.SetText(ZString.Format<int>("{0} Ember Essence", cost));
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x0008238A File Offset: 0x0008058A
		private void TeleportCallback(bool answer, bool useTravelEssence)
		{
			if (answer)
			{
				this.TeleportRequest(useTravelEssence);
			}
		}

		// Token: 0x06006259 RID: 25177 RVA: 0x00082396 File Offset: 0x00080596
		private bool AutoCancel()
		{
			return !this.CanTeleport();
		}

		// Token: 0x0600625A RID: 25178 RVA: 0x00204754 File Offset: 0x00202954
		private void TeleportRequest(bool useTravelEssence)
		{
			if (this.CanTeleport())
			{
				if (this.m_controller)
				{
					MonolithProfile monolithProfile = this.m_profile as MonolithProfile;
					if (monolithProfile != null)
					{
						ZoneId targetZoneId = monolithProfile.MonolithFlag.GetZoneIdForFlag();
						LoginApiManager.PerformZoneCheck(targetZoneId, delegate(bool response)
						{
							this.ZoneCheckResponse(response, targetZoneId, this.m_profile, useTravelEssence);
						});
						return;
					}
				}
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestZoneToMapDiscovery(this.m_profile.Id, useTravelEssence);
				if (ClientGameManager.UIManager && ClientGameManager.UIManager.MapUI && ClientGameManager.UIManager.MapUI.Visible)
				{
					ClientGameManager.UIManager.MapUI.ToggleWindow();
				}
			}
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x00204824 File Offset: 0x00202A24
		private void ZoneCheckResponse(bool result, ZoneId targetZoneId, DiscoveryProfile targetDiscovery, bool useTravelEssence)
		{
			if (result)
			{
				if (!this.CanTeleport())
				{
					return;
				}
				MonolithProfile monolithProfile;
				if (this.m_controller && this.m_controller.TryGetHighlightedMonolithDiscoveryProfile(out monolithProfile))
				{
					LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestZoneToDiscovery(monolithProfile.Id, (int)targetZoneId, targetDiscovery.Id, useTravelEssence);
				}
			}
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x00204878 File Offset: 0x00202A78
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_profile == null)
			{
				return null;
			}
			string text;
			if (this.m_overrideTooltipText)
			{
				text = this.m_tooltipTextOverride;
			}
			else if (this.m_bulletinBoard)
			{
				text = ZString.Format<string>("Bulletin Board\n{0}", this.m_bulletinBoard.Title);
			}
			else if (string.IsNullOrEmpty(this.m_profile.Description))
			{
				text = this.m_profile.DisplayName;
			}
			else
			{
				text = ZString.Format<string, string>("{0}\n{1}", this.m_profile.DisplayName, this.m_profile.Description);
			}
			MonolithProfile monolithProfile = this.m_profile as MonolithProfile;
			if (monolithProfile != null && !monolithProfile.IsAvailable())
			{
				text = ZString.Format<string>("{0}\n<size=80%>(Offline)</size>", text);
			}
			return new ObjectTextTooltipParameter(this, text, false);
		}

		// Token: 0x1700177E RID: 6014
		// (get) Token: 0x0600625D RID: 25181 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700177F RID: 6015
		// (get) Token: 0x0600625E RID: 25182 RVA: 0x000823A1 File Offset: 0x000805A1
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001780 RID: 6016
		// (get) Token: 0x0600625F RID: 25183 RVA: 0x000823AF File Offset: 0x000805AF
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06006261 RID: 25185 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040055CF RID: 21967
		private const float kFadeDuration = 0.2f;

		// Token: 0x040055D0 RID: 21968
		[SerializeField]
		private ZoneId m_zoneId;

		// Token: 0x040055D1 RID: 21969
		[SerializeField]
		private DiscoveryProfile m_profile;

		// Token: 0x040055D2 RID: 21970
		[SerializeField]
		private BulletinBoard m_bulletinBoard;

		// Token: 0x040055D3 RID: 21971
		[SerializeField]
		private Image m_poiImage;

		// Token: 0x040055D4 RID: 21972
		[SerializeField]
		private Image m_fogImage;

		// Token: 0x040055D5 RID: 21973
		[SerializeField]
		private CanvasGroup m_fogGroup;

		// Token: 0x040055D6 RID: 21974
		[SerializeField]
		private CanvasGroup m_poiGroup;

		// Token: 0x040055D7 RID: 21975
		[SerializeField]
		private Image m_poiHighlight;

		// Token: 0x040055D8 RID: 21976
		[SerializeField]
		private Transform m_interactButtonParent;

		// Token: 0x040055D9 RID: 21977
		[HideInInspector]
		[SerializeField]
		private Material m_fogR;

		// Token: 0x040055DA RID: 21978
		[HideInInspector]
		[SerializeField]
		private Material m_fogG;

		// Token: 0x040055DB RID: 21979
		[HideInInspector]
		[SerializeField]
		private Material m_fogB;

		// Token: 0x040055DC RID: 21980
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040055DD RID: 21981
		[SerializeField]
		private bool m_overrideTooltipText;

		// Token: 0x040055DE RID: 21982
		[SerializeField]
		private string m_tooltipTextOverride = string.Empty;

		// Token: 0x040055DF RID: 21983
		private bool m_isDiscovered;

		// Token: 0x040055E0 RID: 21984
		private bool m_isHighlighted;

		// Token: 0x040055E1 RID: 21985
		private bool m_isNearMonolith;

		// Token: 0x040055E2 RID: 21986
		private SolButton m_interactButton;

		// Token: 0x040055E3 RID: 21987
		private MapUI m_controller;
	}
}
