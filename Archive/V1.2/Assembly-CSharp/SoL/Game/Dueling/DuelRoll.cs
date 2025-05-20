using System;
using NetStack.Serialization;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;

namespace SoL.Game.Dueling
{
	// Token: 0x02000C9D RID: 3229
	public struct DuelRoll : INetworkSerializable
	{
		// Token: 0x060061F5 RID: 25077 RVA: 0x00202724 File Offset: 0x00200924
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Status);
			buffer.AddInt(this.NSides);
			buffer.AddInt(this.RollResult);
			buffer.AddInt(this.RollCount);
			buffer.AddBool(this.IsSourceRoll);
			buffer.AddBool(this.IsSourceForfeit);
			buffer.AddString(this.SourceName);
			buffer.AddUInt(this.SourceId);
			buffer.AddString(this.OpponentName);
			buffer.AddUInt(this.OpponentId);
			return buffer;
		}

		// Token: 0x060061F6 RID: 25078 RVA: 0x002027B4 File Offset: 0x002009B4
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Status = buffer.ReadEnum<DuelStatus>();
			this.NSides = buffer.ReadInt();
			this.RollResult = buffer.ReadInt();
			this.RollCount = buffer.ReadInt();
			this.IsSourceRoll = buffer.ReadBool();
			this.IsSourceForfeit = buffer.ReadBool();
			this.SourceName = buffer.ReadString();
			this.SourceId = buffer.ReadUInt();
			this.OpponentName = buffer.ReadString();
			this.OpponentId = buffer.ReadUInt();
			return buffer;
		}

		// Token: 0x060061F7 RID: 25079 RVA: 0x0020283C File Offset: 0x00200A3C
		public void Notify()
		{
			string text = string.Empty;
			bool flag = false;
			bool flag2 = false;
			if (LocalPlayer.NetworkEntity)
			{
				if (LocalPlayer.NetworkEntity.NetworkId.Value == this.SourceId)
				{
					flag = true;
				}
				else if (LocalPlayer.NetworkEntity.NetworkId.Value == this.OpponentId)
				{
					flag2 = true;
				}
			}
			switch (this.Status)
			{
			case DuelStatus.Accepted:
				if (flag)
				{
					text = this.OpponentName + " has accepted <color=#00FFFF>your</color> duel request.";
				}
				else if (flag2)
				{
					text = "<color=#00FFFF>You</color> have accepted " + this.SourceName + "'s duel request.";
				}
				else
				{
					text = this.OpponentName + " has accepted " + this.SourceName + "'s duel request.";
				}
				break;
			case DuelStatus.Forfeited:
				if (flag)
				{
					if (this.IsSourceForfeit)
					{
						text = "<color=#00FFFF>You</color> have forfeited your duel against " + this.OpponentName + "!";
					}
					else
					{
						text = this.OpponentName + " has forfeited their duel against <color=#00FFFF>you</color>!";
					}
				}
				else if (flag2)
				{
					if (!this.IsSourceForfeit)
					{
						text = "<color=#00FFFF>You</color> have forfeited <color=#00FFFF>your</color> duel against " + this.SourceName + "!";
					}
					else
					{
						text = this.SourceName + " has forfeited their duel against <color=#00FFFF>You</color>!";
					}
				}
				else
				{
					string str = this.IsSourceForfeit ? this.SourceName : this.OpponentName;
					string str2 = this.IsSourceForfeit ? this.OpponentName : this.SourceName;
					text = str + " has forfeited their duel against " + str2 + "!";
				}
				break;
			case DuelStatus.Executing:
			{
				uint id = this.IsSourceRoll ? this.SourceId : this.OpponentId;
				NetworkEntity networkEntity;
				if (NetworkManager.EntityManager.TryGetNetworkEntity(id, out networkEntity) && networkEntity.GameEntity)
				{
					if (networkEntity.GameEntity.AudioEventController)
					{
						networkEntity.GameEntity.AudioEventController.PlayAudioEvent("DuelRoll", 0.5f);
					}
					if (networkEntity.GameEntity.WorldSpaceOverheadController)
					{
						string text2 = "<sprite=\"SolIcons\" name=\"Swords\" tint=1><sprite=\"SolIcons\" name=\"NeedIcon\" tint=1> " + this.RollResult.ToString();
						if (this.RollResult == 1)
						{
							text2 += " <color=\"red\">LOSER</color>";
						}
						ChatMessage fromPool = StaticPool<ChatMessage>.GetFromPool();
						fromPool.Init(MessageType.Chat, text2, null, null, PresenceFlags.Online, AccessFlags.None);
						networkEntity.GameEntity.WorldSpaceOverheadController.InitializeParsedChat(fromPool);
					}
					if (networkEntity.GameEntity.AnimancerController != null && GlobalSettings.Values && GlobalSettings.Values.Animation != null)
					{
						Emote emote = (this.RollResult == 1) ? GlobalSettings.Values.Animation.DuelLossEmote : GlobalSettings.Values.Animation.DuelRollEmote;
						if (emote)
						{
							networkEntity.GameEntity.AnimancerController.PlayEmote(emote);
						}
					}
				}
				if (this.RollResult == 1)
				{
					uint id2 = this.IsSourceRoll ? this.OpponentId : this.SourceId;
					NetworkEntity networkEntity2;
					if (NetworkManager.EntityManager.TryGetNetworkEntity(id2, out networkEntity2) && networkEntity2.GameEntity)
					{
						if (networkEntity2.GameEntity.WorldSpaceOverheadController)
						{
							ChatMessage fromPool2 = StaticPool<ChatMessage>.GetFromPool();
							fromPool2.Init(MessageType.Chat, "<sprite=\"SolIcons\" name=\"Swords\" tint=1><sprite=\"SolIcons\" name=\"NeedIcon\" tint=1> <color=\"yellow\">WINNER</color>", null, null, PresenceFlags.Online, AccessFlags.None);
							networkEntity2.GameEntity.WorldSpaceOverheadController.InitializeParsedChat(fromPool2);
						}
						Emote emote2;
						if (networkEntity2.GameEntity.AnimancerController != null && GlobalSettings.Values && GlobalSettings.Values.Animation != null && GlobalSettings.Values.Animation.TryGetDuelWinEmote(this, out emote2))
						{
							networkEntity2.GameEntity.AnimancerController.PlayEmote(emote2);
						}
					}
				}
				string str3 = this.IsSourceRoll ? this.SourceName : this.OpponentName;
				string text3 = ((flag && this.IsSourceRoll) || (flag2 && !this.IsSourceRoll)) ? "<color=#00FFFF>You</color> roll" : (str3 + " rolls");
				text = string.Concat(new string[]
				{
					"<sprite=\"SolIcons\" name=\"Swords\" tint=1><sprite=\"SolIcons\" name=\"NeedIcon\" tint=1> ",
					text3,
					" a 1d",
					this.NSides.ToString(),
					" for ",
					this.RollResult.ToString(),
					"!"
				});
				break;
			}
			case DuelStatus.Complete:
			{
				string text4 = this.IsSourceRoll ? this.SourceName : this.OpponentName;
				string text5 = this.IsSourceRoll ? this.OpponentName : this.SourceName;
				text = string.Concat(new string[]
				{
					"<color=\"yellow\">",
					text5,
					"</color> has defeated <color=\"red\">",
					text4,
					"</color> in a duel!"
				});
				break;
			}
			}
			if (!string.IsNullOrEmpty(text))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Emote | MessageType.PreFormatted, text);
			}
		}

		// Token: 0x0400557F RID: 21887
		private const string kRollAudioEvent = "DuelRoll";

		// Token: 0x04005580 RID: 21888
		private const string kLoserColor = "red";

		// Token: 0x04005581 RID: 21889
		private const string kWinnerColor = "yellow";

		// Token: 0x04005582 RID: 21890
		private const string kYouColor = "#00FFFF";

		// Token: 0x04005583 RID: 21891
		private const string kYou = "<color=#00FFFF>You</color>";

		// Token: 0x04005584 RID: 21892
		private const string kYouLower = "<color=#00FFFF>you</color>";

		// Token: 0x04005585 RID: 21893
		private const string kYourLower = "<color=#00FFFF>your</color>";

		// Token: 0x04005586 RID: 21894
		private const float kAudioFraction = 0.5f;

		// Token: 0x04005587 RID: 21895
		public DuelStatus Status;

		// Token: 0x04005588 RID: 21896
		public int NSides;

		// Token: 0x04005589 RID: 21897
		public int RollResult;

		// Token: 0x0400558A RID: 21898
		public int RollCount;

		// Token: 0x0400558B RID: 21899
		public bool IsSourceRoll;

		// Token: 0x0400558C RID: 21900
		public bool IsSourceForfeit;

		// Token: 0x0400558D RID: 21901
		public string SourceName;

		// Token: 0x0400558E RID: 21902
		public uint SourceId;

		// Token: 0x0400558F RID: 21903
		public string OpponentName;

		// Token: 0x04005590 RID: 21904
		public uint OpponentId;
	}
}
