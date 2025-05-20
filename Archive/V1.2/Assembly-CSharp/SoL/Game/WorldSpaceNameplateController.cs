using System;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.GameCamera;
using SoL.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game
{
	// Token: 0x020005A7 RID: 1447
	public class WorldSpaceNameplateController : MonoBehaviour
	{
		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x06002D65 RID: 11621 RVA: 0x0005F901 File Offset: 0x0005DB01
		public NameplateControllerUI Nameplate
		{
			get
			{
				return this.m_nameplate;
			}
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06002D66 RID: 11622 RVA: 0x0005F909 File Offset: 0x0005DB09
		public bool IsActive
		{
			get
			{
				return this.m_canvasGroup.alpha > 0f;
			}
		}

		// Token: 0x06002D67 RID: 11623 RVA: 0x0014DE0C File Offset: 0x0014C00C
		public void Init(WorldSpaceOverheadController controller)
		{
			this.m_controller = controller;
			this.m_canvasGroup.alpha = 0f;
			this.m_leaderIcon.enabled = false;
			if (this.m_controller.GameEntity)
			{
				if (this.m_controller.GameEntity.Type == GameEntityType.Npc)
				{
					this.m_nameplate.NameText.color = GlobalSettings.Values.Npcs.NameplateColor;
					if (this.m_controller.GameEntity.Interactive != null)
					{
						this.m_interactiveNpc = (this.m_controller.GameEntity.Interactive as InteractiveNpc);
						if (this.m_interactiveNpc != null)
						{
							this.m_interactiveNpc.Tagger.Changed += this.InteractiveTaggerChanged;
							this.m_interactiveNpc.GroupId.Changed += this.InteractiveGroupChanged;
							this.m_interactiveNpc.RaidId.Changed += this.InteractiveRaidChanged;
							this.m_interactiveNpc.NpcInteractiveFlags.Changed += this.NpcInteractiveFlagsOnChanged;
							this.NpcInteractiveFlagsOnChanged(this.m_interactiveNpc.NpcInteractiveFlags.Value);
						}
					}
				}
				else
				{
					this.m_nameplate.NameText.color = this.m_defaultNameColor;
				}
				this.CurrentHealthStateOnChanged(this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Value);
				this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
			}
			this.m_nameplate.InitWorldSpace(this.m_controller.GameEntity.Targetable);
		}

		// Token: 0x06002D68 RID: 11624 RVA: 0x0014DFC8 File Offset: 0x0014C1C8
		private void Unsubscribe()
		{
			if (this.m_interactiveNpc)
			{
				this.m_interactiveNpc.Tagger.Changed -= this.InteractiveTaggerChanged;
				this.m_interactiveNpc.GroupId.Changed -= this.InteractiveGroupChanged;
				this.m_interactiveNpc.RaidId.Changed -= this.InteractiveRaidChanged;
				this.m_interactiveNpc.NpcInteractiveFlags.Changed -= this.NpcInteractiveFlagsOnChanged;
			}
			if (this.m_controller && this.m_controller.GameEntity && this.m_controller.GameEntity.VitalsReplicator)
			{
				this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x06002D69 RID: 11625 RVA: 0x0014E0B0 File Offset: 0x0014C2B0
		public void ResetData()
		{
			this.Unsubscribe();
			this.m_isMemberOfMyGroup = false;
			this.m_isMemberOfMyGuild = false;
			this.m_isLeader = false;
			this.m_maxAlpha = 1f;
			this.m_controller = null;
			this.m_interactiveNpc = null;
			this.m_isMemberOfMyRaid = false;
			this.m_isRaidLeader = false;
			if (this.m_leaderIcon)
			{
				this.m_leaderIcon.enabled = false;
			}
			if (this.m_nameplate)
			{
				this.m_nameplate.Init(null);
			}
			this.m_showNameplate = true;
			this.m_showStatBar = true;
			this.UpdateNameColor();
			this.UpdateGuildColor();
		}

		// Token: 0x06002D6A RID: 11626 RVA: 0x0014E14C File Offset: 0x0014C34C
		private void UpdateNameColor()
		{
			if (this.m_nameplate && this.m_nameplate.NameText)
			{
				Color color = this.m_defaultNameColor;
				if (this.m_isMemberOfMyGroup)
				{
					color = GlobalSettings.Values.Nameplates.GroupColor;
				}
				else if (this.m_isPvp)
				{
					color = UIManager.RedColor;
				}
				else if (this.m_isMemberOfMyRaid)
				{
					color = UIManager.RaidColor;
				}
				this.m_nameplate.NameText.color = color;
			}
		}

		// Token: 0x06002D6B RID: 11627 RVA: 0x0014E1C8 File Offset: 0x0014C3C8
		private void UpdateGuildColor()
		{
			if (this.m_nameplate && this.m_nameplate.GuildText)
			{
				this.m_nameplate.GuildText.color = (this.m_isMemberOfMyGuild ? GlobalSettings.Values.Nameplates.GroupColor : this.m_defaultGuildColor);
			}
		}

		// Token: 0x06002D6C RID: 11628 RVA: 0x0005F91D File Offset: 0x0005DB1D
		private void Awake()
		{
			if (this.m_chatRectTransform)
			{
				this.m_chatRectDefaultY = this.m_chatRectTransform.anchoredPosition.y;
			}
		}

		// Token: 0x06002D6D RID: 11629 RVA: 0x0005F942 File Offset: 0x0005DB42
		private void OnDestroy()
		{
			this.Unsubscribe();
		}

		// Token: 0x06002D6E RID: 11630 RVA: 0x0014E224 File Offset: 0x0014C424
		public void LateUpdateExternal()
		{
			if (!this.m_nameplate || this.m_nameplate.IsEmpty || !LocalPlayer.GameEntity || !ClientGameManager.MainCamera)
			{
				return;
			}
			this.UpdateShowVariables();
			float num = 0f;
			if ((!this.m_controller || !this.m_controller.OffScreen) && (this.m_showNameplate || this.m_showStatBar))
			{
				Vector3 start = this.m_controller ? this.m_controller.WorldPos : base.gameObject.transform.position;
				Transform transform = ClientGameManager.MainCamera.transform;
				Vector3 vector = transform.position;
				if (CameraManager.ActiveType == ActiveCameraTypes.FirstPerson && this.m_controller && this.m_controller.Mode == OverheadNameplateMode.UISpace)
				{
					Vector3 b = transform.forward * 0.25f;
					b.y = 0f;
					vector += b;
				}
				if (!Physics.Linecast(start, vector, this.m_losLayerMask, QueryTriggerInteraction.Ignore))
				{
					num = this.GetTargetAlphaForDistance();
					this.UpdateScale();
				}
			}
			if (this.m_canvasGroup.alpha != num)
			{
				this.m_canvasGroup.alpha = Mathf.MoveTowards(this.m_canvasGroup.alpha, num, Time.deltaTime * 2f);
			}
			this.m_nameplate.ProcessUpdates = this.IsActive;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			if (this.m_nameplate.Targetable.Entity && this.m_nameplate.Targetable.Entity.Type == GameEntityType.Player)
			{
				flag = (ClientGameManager.GroupManager.IsGrouped && (this.m_nameplate.Targetable.Entity == LocalPlayer.GameEntity || ClientGameManager.GroupManager.IsMemberOfMyGroup(this.m_nameplate.Targetable.Entity)));
				flag3 = (ClientGameManager.GroupManager.IsGrouped && flag && ClientGameManager.GroupManager.Leader != null && ClientGameManager.GroupManager.Leader.Entity && ClientGameManager.GroupManager.Leader.Entity == this.m_nameplate.Targetable.Entity);
				if (ClientGameManager.SocialManager && this.m_nameplate.Targetable.Entity.CharacterData)
				{
					if (ClientGameManager.SocialManager.IsInGuild)
					{
						flag2 = (this.m_nameplate.Targetable.Entity == LocalPlayer.GameEntity || this.m_nameplate.Targetable.Entity.CharacterData.GuildName.Value == LocalPlayer.GameEntity.CharacterData.GuildName.Value);
					}
					if (ClientGameManager.SocialManager.IsInRaid)
					{
						flag5 = (this.m_nameplate.Targetable.Entity == LocalPlayer.GameEntity || this.m_nameplate.Targetable.Entity.CharacterData.RaidId.Value == LocalPlayer.GameEntity.CharacterData.RaidId.Value);
						flag6 = ClientGameManager.SocialManager.IsRaidLeader(this.m_nameplate.Targetable.Entity.CharacterData.Name.Value);
					}
				}
				flag4 = (this.m_nameplate.Targetable.Entity.CharacterData && this.m_nameplate.Targetable.Entity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Pvp));
			}
			bool flag7 = false;
			if (flag4 != this.m_isPvp)
			{
				this.m_isPvp = flag4;
				flag7 = true;
			}
			if (flag != this.m_isMemberOfMyGroup)
			{
				this.m_isMemberOfMyGroup = flag;
				flag7 = true;
			}
			if (flag5 != this.m_isMemberOfMyRaid)
			{
				this.m_isMemberOfMyRaid = flag5;
				flag7 = true;
			}
			if (flag7)
			{
				this.UpdateNameColor();
			}
			if (flag2 != this.m_isMemberOfMyGuild)
			{
				this.m_isMemberOfMyGuild = flag2;
				this.UpdateGuildColor();
			}
			bool flag8 = false;
			if (flag3 != this.m_isLeader)
			{
				this.m_isLeader = flag3;
				flag8 = true;
			}
			if (flag6 != this.m_isRaidLeader)
			{
				this.m_isRaidLeader = flag6;
				flag8 = true;
			}
			if (flag8 && this.m_leaderIcon)
			{
				if (this.m_defaultLeaderIconColor == null)
				{
					this.m_defaultLeaderIconColor = new Color?(this.m_leaderIcon.color);
				}
				this.m_leaderIcon.color = (this.m_isRaidLeader ? UIManager.RaidColor : this.m_defaultLeaderIconColor.Value);
				this.m_leaderIcon.enabled = (this.m_isLeader || this.m_isRaidLeader);
			}
			this.ToggleTMP(WorldSpaceNameplateController.TextType.Name);
			this.ToggleTMP(WorldSpaceNameplateController.TextType.Title);
			this.ToggleTMP(WorldSpaceNameplateController.TextType.Guild);
			if (this.m_showStatBar && !this.m_statBars.activeSelf)
			{
				this.m_statBars.SetActive(true);
				return;
			}
			if (!this.m_showStatBar && this.m_statBars.activeSelf)
			{
				this.m_statBars.SetActive(false);
			}
		}

		// Token: 0x06002D6F RID: 11631 RVA: 0x0014E74C File Offset: 0x0014C94C
		private void ToggleTMP(WorldSpaceNameplateController.TextType txtType)
		{
			if (!this.m_nameplate)
			{
				return;
			}
			TextMeshProUGUI textMeshProUGUI = null;
			switch (txtType)
			{
			case WorldSpaceNameplateController.TextType.Name:
				textMeshProUGUI = this.m_nameplate.NameText;
				break;
			case WorldSpaceNameplateController.TextType.Title:
				textMeshProUGUI = this.m_nameplate.TitleText;
				break;
			case WorldSpaceNameplateController.TextType.Guild:
				textMeshProUGUI = this.m_nameplate.GuildText;
				break;
			}
			if (!textMeshProUGUI || !textMeshProUGUI.gameObject)
			{
				return;
			}
			if (textMeshProUGUI.gameObject.activeSelf != this.m_showNameplate)
			{
				textMeshProUGUI.gameObject.SetActive(this.m_showNameplate);
			}
		}

		// Token: 0x06002D70 RID: 11632 RVA: 0x0014E7E0 File Offset: 0x0014C9E0
		private void UpdateShowVariables()
		{
			if (!Options.GameOptions.ShowOverheadNameplates.Value || !this.m_controller || !this.m_controller.GameEntity || !this.m_controller.GameEntity.NetworkEntity)
			{
				this.m_showNameplate = false;
				this.m_showStatBar = false;
				return;
			}
			GameEntityType type = this.m_controller.GameEntity.Type;
			if (type != GameEntityType.Player)
			{
				if (type != GameEntityType.Npc)
				{
					this.m_showNameplate = false;
					this.m_showStatBar = false;
					return;
				}
				this.m_showNameplate = Options.GameOptions.ShowOverheadNameplate_Npcs.Value;
				this.m_showStatBar = Options.GameOptions.ShowOverheadStatBar_Npcs.Value;
				return;
			}
			else
			{
				if (this.m_controller.GameEntity.CharacterData && this.m_controller.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Invisible))
				{
					this.m_showNameplate = false;
					this.m_showStatBar = false;
					return;
				}
				if (this.m_controller.GameEntity.NetworkEntity.IsLocal)
				{
					bool flag = CameraManager.ActiveType != ActiveCameraTypes.FirstPerson;
					this.m_showNameplate = (flag && Options.GameOptions.ShowOverheadNameplate_Self.Value);
					this.m_showStatBar = (flag && Options.GameOptions.ShowOverheadStatBar_Self.Value);
					return;
				}
				if (this.m_isMemberOfMyGroup)
				{
					this.m_showNameplate = Options.GameOptions.ShowOverheadNameplate_Group.Value;
					this.m_showStatBar = Options.GameOptions.ShowOverheadStatBar_Group.Value;
					return;
				}
				if (this.m_isMemberOfMyGuild)
				{
					this.m_showNameplate = Options.GameOptions.ShowOverheadNameplate_Guild.Value;
					this.m_showStatBar = Options.GameOptions.ShowOverheadStatBar_Guild.Value;
					return;
				}
				this.m_showNameplate = Options.GameOptions.ShowOverheadNameplate_OtherPlayers.Value;
				this.m_showStatBar = Options.GameOptions.ShowOverheadStatBar_OtherPlayers.Value;
				return;
			}
		}

		// Token: 0x06002D71 RID: 11633 RVA: 0x0014E9A0 File Offset: 0x0014CBA0
		private float GetTargetAlphaForDistance()
		{
			if (this.m_controller && this.m_controller.OffScreen)
			{
				return 0f;
			}
			float cachedSqrDistanceFromLocalPlayer = this.m_nameplate.Targetable.Entity.GetCachedSqrDistanceFromLocalPlayer();
			if (cachedSqrDistanceFromLocalPlayer <= LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr)
			{
				float distanceFraction = Mathf.Sqrt(cachedSqrDistanceFromLocalPlayer / LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr);
				return GlobalSettings.Values.Nameplates.GetOverheadAlpha(this.m_maxAlpha, distanceFraction, this.m_controller.Mode);
			}
			return 0f;
		}

		// Token: 0x06002D72 RID: 11634 RVA: 0x0014EA34 File Offset: 0x0014CC34
		internal void UpdateScale()
		{
			if (this.m_nameplate.Targetable == null || !this.m_nameplate.Targetable.Entity || !LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Vitals)
			{
				return;
			}
			float cachedSqrDistanceFromCamera = this.m_nameplate.Targetable.Entity.GetCachedSqrDistanceFromCamera();
			float distanceFraction = (cachedSqrDistanceFromCamera <= LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr) ? Mathf.Sqrt(cachedSqrDistanceFromCamera / LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr) : 1f;
			float overheadScale = GlobalSettings.Values.Nameplates.GetOverheadScale(this.m_defaultScale, distanceFraction, this.m_controller.Mode);
			this.m_rectTransform.localScale = Vector3.one * overheadScale;
			if (this.m_chatRectTransform)
			{
				Vector2 sizeDelta = this.m_rectTransform.sizeDelta;
				float y = sizeDelta.y;
				float y2 = Mathf.Clamp(sizeDelta.y * overheadScale - y + this.m_chatRectDefaultY, this.m_chatRectDefaultY, float.MaxValue);
				this.m_chatRectTransform.anchoredPosition = new Vector2(0f, y2);
			}
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x0005F94A File Offset: 0x0005DB4A
		public void OnLocalPlayerMasteryLevelChanged()
		{
			this.m_nameplate.RefreshDifficultyIndicator();
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x0005F957 File Offset: 0x0005DB57
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			this.m_maxAlpha = ((obj == HealthState.Dead) ? 0.5f : 1f);
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x0014EB5C File Offset: 0x0014CD5C
		private void NpcInteractiveFlagsOnChanged(InteractiveFlags obj)
		{
			if (this.m_controller && this.m_controller.GameEntity && this.m_controller.GameEntity.VitalsReplicator && this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Value == HealthState.Dead)
			{
				this.m_maxAlpha = (obj.HasBitFlag(InteractiveFlags.Interactive) ? 0.5f : 0f);
			}
			if (this.m_nameplate)
			{
				this.m_nameplate.RefreshEncounterLockIndicator();
				this.m_nameplate.RefreshPendingLootRollIndicator();
			}
		}

		// Token: 0x06002D76 RID: 11638 RVA: 0x0005F96F File Offset: 0x0005DB6F
		private void InteractiveTaggerChanged(string obj)
		{
			if (this.m_nameplate)
			{
				this.m_nameplate.RefreshEncounterLockIndicator();
			}
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x0005F96F File Offset: 0x0005DB6F
		private void InteractiveGroupChanged(UniqueId obj)
		{
			if (this.m_nameplate)
			{
				this.m_nameplate.RefreshEncounterLockIndicator();
			}
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x0005F96F File Offset: 0x0005DB6F
		private void InteractiveRaidChanged(UniqueId obj)
		{
			if (this.m_nameplate)
			{
				this.m_nameplate.RefreshEncounterLockIndicator();
			}
		}

		// Token: 0x04002CF1 RID: 11505
		[SerializeField]
		private NameplateControllerUI m_nameplate;

		// Token: 0x04002CF2 RID: 11506
		[SerializeField]
		private CanvasGroup m_canvasGroup;

		// Token: 0x04002CF3 RID: 11507
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04002CF4 RID: 11508
		[SerializeField]
		private GameObject m_statBars;

		// Token: 0x04002CF5 RID: 11509
		[SerializeField]
		private Image m_leaderIcon;

		// Token: 0x04002CF6 RID: 11510
		[SerializeField]
		private LayerMask m_losLayerMask;

		// Token: 0x04002CF7 RID: 11511
		[SerializeField]
		private float m_defaultScale = 1f;

		// Token: 0x04002CF8 RID: 11512
		[SerializeField]
		private Color m_defaultNameColor = Color.white;

		// Token: 0x04002CF9 RID: 11513
		[SerializeField]
		private Color m_defaultGuildColor = Color.white;

		// Token: 0x04002CFA RID: 11514
		[SerializeField]
		private RectTransform m_chatRectTransform;

		// Token: 0x04002CFB RID: 11515
		private float m_chatRectDefaultY = 8f;

		// Token: 0x04002CFC RID: 11516
		private bool m_isMemberOfMyGroup;

		// Token: 0x04002CFD RID: 11517
		private bool m_isMemberOfMyGuild;

		// Token: 0x04002CFE RID: 11518
		private bool m_isLeader;

		// Token: 0x04002CFF RID: 11519
		private bool m_isPvp;

		// Token: 0x04002D00 RID: 11520
		private Color? m_defaultLeaderIconColor;

		// Token: 0x04002D01 RID: 11521
		private bool m_isMemberOfMyRaid;

		// Token: 0x04002D02 RID: 11522
		private bool m_isRaidLeader;

		// Token: 0x04002D03 RID: 11523
		private float m_maxAlpha = 1f;

		// Token: 0x04002D04 RID: 11524
		private WorldSpaceOverheadController m_controller;

		// Token: 0x04002D05 RID: 11525
		private InteractiveNpc m_interactiveNpc;

		// Token: 0x04002D06 RID: 11526
		private bool m_showNameplate = true;

		// Token: 0x04002D07 RID: 11527
		private bool m_showStatBar = true;

		// Token: 0x020005A8 RID: 1448
		private enum TextType
		{
			// Token: 0x04002D09 RID: 11529
			Name,
			// Token: 0x04002D0A RID: 11530
			Title,
			// Token: 0x04002D0B RID: 11531
			Guild
		}
	}
}
