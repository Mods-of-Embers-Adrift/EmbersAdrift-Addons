using System;
using Cysharp.Text;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000569 RID: 1385
	public class Corpse : BaseInteractive, ITooltip, IInteractiveBase
	{
		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06002ABA RID: 10938 RVA: 0x0005D87F File Offset: 0x0005BA7F
		private string CharacterId
		{
			get
			{
				if (!(this.m_replicator != null))
				{
					return string.Empty;
				}
				return this.m_replicator.CorpseData.Value.CharacterId;
			}
		}

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06002ABB RID: 10939 RVA: 0x0005D8AA File Offset: 0x0005BAAA
		// (set) Token: 0x06002ABC RID: 10940 RVA: 0x0005D8B2 File Offset: 0x0005BAB2
		public NetworkEntity Owner { get; private set; }

		// Token: 0x06002ABD RID: 10941 RVA: 0x00144948 File Offset: 0x00142B48
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				this.m_replicator.CorpseLocation.Changed += this.CorpseLocationOnChanged;
				this.SetPositionRotation();
				this.m_isMyCorpse = (!string.IsNullOrEmpty(this.CharacterId) && this.CharacterId == SessionData.GetSelectedCharacterId());
				if (this.m_isMyCorpse)
				{
					LocalPlayer.Corpse = this;
				}
				else if (ClientGameManager.GroupManager)
				{
					ClientGameManager.GroupManager.RefreshGroup += this.RefreshGroupVisuals;
					this.RefreshGroupVisuals();
				}
				if (this.m_selfVisuals != null)
				{
					this.m_selfVisuals.SetActive(this.m_isMyCorpse);
				}
			}
		}

		// Token: 0x06002ABE RID: 10942 RVA: 0x00144A00 File Offset: 0x00142C00
		private void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.m_replicator.CorpseLocation.Changed -= this.CorpseLocationOnChanged;
				if (this.m_isMyCorpse && LocalPlayer.Corpse == this)
				{
					LocalPlayer.Corpse = null;
				}
				if (!this.m_isMyCorpse && ClientGameManager.GroupManager)
				{
					ClientGameManager.GroupManager.RefreshGroup -= this.RefreshGroupVisuals;
				}
			}
			this.Owner = null;
		}

		// Token: 0x06002ABF RID: 10943 RVA: 0x0005D8BB File Offset: 0x0005BABB
		public bool NameMatches(string queryName)
		{
			return this.m_replicator != null && this.m_replicator.CorpseData.Value.CharacterName.Equals(queryName, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x06002AC0 RID: 10944 RVA: 0x0005D8E9 File Offset: 0x0005BAE9
		public Vector3 GetPosition()
		{
			if (!(this.m_replicator != null))
			{
				return base.gameObject.transform.position;
			}
			return this.m_replicator.CorpseLocation.Value.GetPosition();
		}

		// Token: 0x06002AC1 RID: 10945 RVA: 0x0005D91F File Offset: 0x0005BB1F
		private void CorpseLocationOnChanged(CharacterLocation obj)
		{
			this.SetPositionRotation();
		}

		// Token: 0x06002AC2 RID: 10946 RVA: 0x00144A7C File Offset: 0x00142C7C
		public bool Initialize(CharacterRecord record, NetworkEntity owner)
		{
			if (record.Corpse != null)
			{
				this.m_replicator.CorpseData.Value = new CorpseData
				{
					CharacterId = record.Id,
					CharacterName = record.Name,
					TimeCreated = record.Corpse.TimeCreated
				};
				this.m_replicator.CorpseLocation.Value = record.Corpse.Location.Clone();
				this.SetPositionRotation();
				this.Owner = owner;
				return true;
			}
			return false;
		}

		// Token: 0x06002AC3 RID: 10947 RVA: 0x0005D927 File Offset: 0x0005BB27
		public void UpdateLocation(CharacterLocation newLocation)
		{
			this.m_replicator.CorpseLocation.Value = newLocation.Clone();
			this.SetPositionRotation();
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x00144B08 File Offset: 0x00142D08
		private void SetPositionRotation()
		{
			Vector3 position = this.m_replicator.CorpseLocation.Value.GetPosition();
			Quaternion rotation = this.m_replicator.CorpseLocation.Value.GetRotation();
			base.gameObject.transform.SetPositionAndRotation(position, rotation);
		}

		// Token: 0x06002AC5 RID: 10949 RVA: 0x0005D945 File Offset: 0x0005BB45
		public override bool CanInteract(GameEntity entity)
		{
			return base.CanInteract(entity) && entity.CollectionController.Record.Id == this.CharacterId && entity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag);
		}

		// Token: 0x06002AC6 RID: 10950 RVA: 0x0005D985 File Offset: 0x0005BB85
		public override void BeginInteraction(GameEntity interactionSource)
		{
			if (interactionSource.CollectionController.Record.Id == this.CharacterId)
			{
				CorpseManager.RemoveCorpseForPlayer(interactionSource);
			}
		}

		// Token: 0x06002AC7 RID: 10951 RVA: 0x00144B54 File Offset: 0x00142D54
		private void RefreshGroupVisuals()
		{
			if (this.m_isMyCorpse || !this.m_groupVisuals)
			{
				return;
			}
			GroupMember groupMember;
			if (this.m_replicator && this.m_replicator.CorpseData != null && !string.IsNullOrEmpty(this.m_replicator.CorpseData.Value.CharacterName) && ClientGameManager.GroupManager && ClientGameManager.GroupManager.TryGetGroupMember(this.m_replicator.CorpseData.Value.CharacterName, out groupMember))
			{
				this.m_groupVisuals.SetActive(true);
				return;
			}
			this.m_groupVisuals.SetActive(false);
		}

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x0005D9AA File Offset: 0x0005BBAA
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.HammerCursor;
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06002AC9 RID: 10953 RVA: 0x0005D9AD File Offset: 0x0005BBAD
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.HammerCursorInactive;
			}
		}

		// Token: 0x06002ACA RID: 10954 RVA: 0x00144BF8 File Offset: 0x00142DF8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_replicator != null)
			{
				string characterName = this.m_replicator.CorpseData.Value.CharacterName;
				string text = ZString.Format<string>("{0}'s bag.", this.m_replicator.CorpseData.Value.CharacterName);
				GroupMember groupMember;
				if (this.m_isMyCorpse)
				{
					text = ZString.Format<string>("{0}\n(YOURS)", text);
				}
				else if (ClientGameManager.GroupManager && ClientGameManager.GroupManager.TryGetGroupMember(characterName, out groupMember))
				{
					text = ZString.Format<string>("{0}\n(GROUP MEMBER)", text);
				}
				else if (ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsRaidMember(characterName))
				{
					text = ZString.Format<string>("{0}\n(RAID MEMBER)", text);
				}
				return new ObjectTextTooltipParameter(this, text, false);
			}
			return null;
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06002ACB RID: 10955 RVA: 0x0005D9B1 File Offset: 0x0005BBB1
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06002ACC RID: 10956 RVA: 0x0005D9BF File Offset: 0x0005BBBF
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06002ACE RID: 10958 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04002B0F RID: 11023
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04002B10 RID: 11024
		[SerializeField]
		private CorpseSyncVarReplicator m_replicator;

		// Token: 0x04002B11 RID: 11025
		[SerializeField]
		private GameObject m_selfVisuals;

		// Token: 0x04002B12 RID: 11026
		[SerializeField]
		private GameObject m_groupVisuals;

		// Token: 0x04002B13 RID: 11027
		private bool m_isMyCorpse;
	}
}
