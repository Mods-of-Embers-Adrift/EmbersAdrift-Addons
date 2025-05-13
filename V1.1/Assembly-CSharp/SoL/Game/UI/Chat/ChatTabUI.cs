using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009AF RID: 2479
	public class ChatTabUI : ResizableWindow, IContextMenu, IInteractiveBase
	{
		// Token: 0x17001072 RID: 4210
		// (get) Token: 0x06004A6D RID: 19053 RVA: 0x0007225D File Offset: 0x0007045D
		// (set) Token: 0x06004A6E RID: 19054 RVA: 0x00072265 File Offset: 0x00070465
		public bool WasFocusedLastFrame { get; private set; }

		// Token: 0x17001073 RID: 4211
		// (get) Token: 0x06004A6F RID: 19055 RVA: 0x0007226E File Offset: 0x0007046E
		public ChatMessageQueue Queue
		{
			get
			{
				return this.m_queue;
			}
		}

		// Token: 0x06004A70 RID: 19056 RVA: 0x001B3324 File Offset: 0x001B1524
		private string GetChannelDropdownText(MessageType msgType)
		{
			string text = msgType.GetDisplayChannel();
			Color color;
			if (msgType.GetColor(out color, false))
			{
				text = text.Color(color);
			}
			return text;
		}

		// Token: 0x06004A71 RID: 19057 RVA: 0x00072276 File Offset: 0x00070476
		protected override void Awake()
		{
			base.Awake();
			this.m_queue = (this.m_chatQueue ? MessageManager.ChatQueue : MessageManager.CombatQueue);
			if (this.m_chatQueue)
			{
				UIManager.ActiveChatTabInput = this;
			}
		}

		// Token: 0x06004A72 RID: 19058 RVA: 0x001B334C File Offset: 0x001B154C
		protected override void Start()
		{
			base.Start();
			UIManager.RegisterChatTab(this);
			if (ChatTabUI.m_chatChannelToDropdownIndex == null)
			{
				ChatTabUI.m_chatChannelToDropdownIndex = new Dictionary<MessageType, int>(default(MessageTypeComparer));
				ChatTabUI.m_dropdownIndexToChatChannel = new Dictionary<int, MessageType>();
				for (int i = 0; i < ChatTabUI.m_validMessageTypes.Length; i++)
				{
					ChatTabUI.m_chatChannelToDropdownIndex.Add(ChatTabUI.m_validMessageTypes[i], i);
					ChatTabUI.m_dropdownIndexToChatChannel.Add(i, ChatTabUI.m_validMessageTypes[i]);
				}
			}
			this.m_channelDropdown.ClearOptions();
			List<string> list = new List<string>(ChatTabUI.m_validMessageTypes.Length);
			for (int j = 0; j < ChatTabUI.m_validMessageTypes.Length; j++)
			{
				list.Add(this.GetChannelDropdownText(ChatTabUI.m_validMessageTypes[j]));
			}
			this.m_channelDropdown.AddOptions(list);
			this.m_channelDropdown.value = 0;
			this.m_contentSizeFitterRectTransform = this.m_content.gameObject.GetComponent<RectTransform>();
			this.m_scrollRectRectTransform = this.m_scrollRect.GetComponent<RectTransform>();
			this.m_input.onValueChanged.AddListener(new UnityAction<string>(this.InputFieldChanged));
			this.m_sendButton.onClick.AddListener(new UnityAction(this.OnSendClicked));
			this.m_channelDropdown.onValueChanged.AddListener(new UnityAction<int>(this.ChannelDropdownChanged));
		}

		// Token: 0x06004A73 RID: 19059 RVA: 0x001B3494 File Offset: 0x001B1694
		protected override void OnDestroy()
		{
			base.OnDestroy();
			UIManager.UnregisterChatTab(this);
			this.m_input.onValueChanged.RemoveListener(new UnityAction<string>(this.InputFieldChanged));
			this.m_sendButton.onClick.RemoveListener(new UnityAction(this.OnSendClicked));
			this.m_channelDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.ChannelDropdownChanged));
		}

		// Token: 0x06004A74 RID: 19060 RVA: 0x001B3504 File Offset: 0x001B1704
		private void Update()
		{
			if (!LocalPlayer.GameEntity)
			{
				return;
			}
			if (this.m_updateScrollFrame != null && this.m_updateScrollFrame.Value < Time.frameCount)
			{
				this.m_scrollRect.verticalNormalizedPosition = 0f;
				this.m_updateScrollFrame = null;
			}
			this.WasFocusedLastFrame = this.m_input.isFocused;
			if (!this.m_chatQueue)
			{
				return;
			}
			bool flag = UIManager.EventSystem.currentSelectedGameObject == this.m_input.gameObject;
			bool flag2 = !ClientGameManager.InputManager.InputPreventionFlags.PreventForUI();
			if (!flag && flag2 && Input.GetKeyDown(KeyCode.Slash) && string.IsNullOrEmpty(this.m_input.text))
			{
				this.m_input.text = "/";
				this.m_input.Activate();
			}
			if (!flag && flag2 && SolInput.GetButtonDown(43))
			{
				if (string.IsNullOrEmpty(ChatHandler.LastTellReceivedFrom))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Tell, "You Have not received any Tells");
				}
				else
				{
					this.m_input.text = "/tell " + ChatHandler.LastTellReceivedFrom + " ";
					this.m_input.Activate();
				}
			}
			if (ClientGameManager.InputManager != null && ClientGameManager.InputManager.EnterDown)
			{
				if (flag)
				{
					this.OnSendClicked();
				}
				else if (flag2)
				{
					this.m_input.Activate();
				}
			}
			if (flag && this.m_history.Count > 0)
			{
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					this.m_input.text = this.m_history[this.m_historyIndex];
					this.m_input.CaretToEnd();
					this.m_historyIndex++;
					if (this.m_historyIndex >= this.m_history.Count)
					{
						this.m_historyIndex = 0;
						return;
					}
				}
				else if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					this.m_historyIndex--;
					if (this.m_historyIndex < 0)
					{
						this.m_historyIndex = this.m_history.Count - 1;
					}
					this.m_input.text = this.m_history[this.m_historyIndex];
					this.m_input.CaretToEnd();
				}
			}
		}

		// Token: 0x06004A75 RID: 19061 RVA: 0x001B3738 File Offset: 0x001B1938
		private void InputFieldChanged(string value)
		{
			Match match = Regex.Match(this.m_input.text, "^\\/(\\S+ )$");
			if (match.Success)
			{
				string key = match.Groups[1].Value.ToLowerInvariant().Trim();
				MessageType key2;
				int value2;
				if (ChatTabUI.m_chatChannelShortcuts.TryGetValue(key, out key2) && ChatTabUI.m_chatChannelToDropdownIndex.TryGetValue(key2, out value2))
				{
					this.m_channelDropdown.value = value2;
					this.m_input.text = null;
					this.UpdatePreviousDefaultChannelIndex();
				}
			}
		}

		// Token: 0x06004A76 RID: 19062 RVA: 0x000722A6 File Offset: 0x000704A6
		private void ChannelDropdownChanged(int index)
		{
			if (UIManager.EventSystem != null && UIManager.EventSystem.currentSelectedGameObject == this.m_channelDropdown.gameObject)
			{
				UIManager.EventSystem.SetSelectedGameObject(null);
			}
			this.UpdatePreviousDefaultChannelIndex();
		}

		// Token: 0x06004A77 RID: 19063 RVA: 0x001B37BC File Offset: 0x001B19BC
		private void UpdatePreviousDefaultChannelIndex()
		{
			MessageType type;
			if (ChatTabUI.m_dropdownIndexToChatChannel.TryGetValue(this.m_channelDropdown.value, out type) && type.CanBeDefaultChannel())
			{
				this.m_previousDefaultChannelIndex = this.m_channelDropdown.value;
			}
		}

		// Token: 0x06004A78 RID: 19064 RVA: 0x000722E2 File Offset: 0x000704E2
		private void FailedToSend()
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Failed to send...");
			this.m_input.Deactivate();
		}

		// Token: 0x06004A79 RID: 19065 RVA: 0x001B37FC File Offset: 0x001B19FC
		private void AppendHistory()
		{
			this.m_historyIndex = 0;
			if (this.m_history.Count <= 0 || !this.m_history[0].Equals(this.m_input.text))
			{
				this.m_history.Insert(0, this.m_input.text);
			}
			while (this.m_history.Count > 20)
			{
				this.m_history.RemoveAt(this.m_history.Count - 1);
			}
		}

		// Token: 0x06004A7A RID: 19066 RVA: 0x001B387C File Offset: 0x001B1A7C
		private void OnSendClicked()
		{
			if (string.IsNullOrEmpty(this.m_input.text))
			{
				this.m_input.Deactivate();
				return;
			}
			Match match = Regex.Match(this.m_input.text, "^\\/(\\S+) *(.*)$");
			if (match.Success)
			{
				string text = match.Groups[1].Value.ToLower().Trim();
				string value = match.Groups[2].Value;
				CommandClass commandClass;
				SoL.Networking.SolServer.CommandType commandType;
				if (ChatTabUI.m_validCommands.TryGetValue(text, out commandClass) && Enum.TryParse<SoL.Networking.SolServer.CommandType>(text, out commandType))
				{
					if (!SolServerConnectionManager.IsOnline && !commandClass.AllowOfflineCommand())
					{
						this.FailedToSend();
						return;
					}
					SolServerCommand solServerCommand = commandClass.NewCommand(commandType);
					switch (commandClass)
					{
					case CommandClass.group:
						solServerCommand.Args.Add("Receiver", value);
						break;
					case CommandClass.guild:
					case CommandClass.gm:
					case CommandClass.server:
						break;
					case CommandClass.local:
						if (this.ProcessLocalCommand(text, value))
						{
							this.AppendHistory();
							this.ClearAndDeactivateInput();
							return;
						}
						break;
					default:
						if (commandClass == CommandClass.relations)
						{
							switch (commandType)
							{
							case SoL.Networking.SolServer.CommandType.friend:
								ClientGameManager.SocialManager.Friend(value);
								break;
							case SoL.Networking.SolServer.CommandType.unfriend:
								ClientGameManager.SocialManager.Unfriend(value);
								break;
							case SoL.Networking.SolServer.CommandType.block:
								ClientGameManager.SocialManager.Block(value);
								break;
							case SoL.Networking.SolServer.CommandType.unblock:
								ClientGameManager.SocialManager.Unblock(value);
								break;
							}
							this.ClearAndDeactivateInput();
							return;
						}
						break;
					}
					if (commandType != SoL.Networking.SolServer.CommandType.tell)
					{
						if (commandType - SoL.Networking.SolServer.CommandType.roll > 1)
						{
							switch (commandType)
							{
							case SoL.Networking.SolServer.CommandType.startlfg:
							case SoL.Networking.SolServer.CommandType.startlfm:
							{
								string[] array = value.Split(' ', StringSplitOptions.None);
								int num;
								if (array.Length < 1 || array.Length > 3)
								{
									solServerCommand.State = false;
									solServerCommand.Args.Add("err", "malformed message");
								}
								else if (int.TryParse(array[0], out num))
								{
									solServerCommand.Args.Add("zone", num);
									if (array.Length > 1)
									{
										string text2 = array[1];
										if (text2.StartsWith("-") && int.TryParse(text2.TrimStart('-'), out num))
										{
											solServerCommand.Args.Add("maxlevel", num);
										}
										else if (text2.EndsWith("-") && int.TryParse(text2.TrimEnd('-'), out num))
										{
											solServerCommand.Args.Add("minlevel", num);
										}
										else if (text2.Contains("-"))
										{
											string[] array2 = text2.Split('-', StringSplitOptions.None);
											if (array2.Length != 2)
											{
												solServerCommand.State = false;
												solServerCommand.Args.Add("err", "malformed message");
												break;
											}
											if (int.TryParse(array2[0], out num))
											{
												solServerCommand.Args.Add("minlevel", num);
											}
											if (int.TryParse(array2[1], out num))
											{
												solServerCommand.Args.Add("maxlevel", num);
											}
										}
									}
									if (array.Length > 2 && int.TryParse(array[2], out num))
									{
										solServerCommand.Args.Add("roles", num);
									}
								}
								break;
							}
							case SoL.Networking.SolServer.CommandType.emote:
								solServerCommand.Args.Add("Message", value);
								break;
							}
						}
						else
						{
							solServerCommand.Args.Add("Message", value);
						}
					}
					else
					{
						Match match2 = Regex.Match(value, "^(\\S+) (.*)$", RegexOptions.IgnoreCase);
						if (match2.Success)
						{
							string value2 = match2.Groups[1].Value;
							string value3 = match2.Groups[2].Value;
							solServerCommand.State = !string.IsNullOrEmpty(value3);
							solServerCommand.Args.Add("Receiver", value2);
							solServerCommand.Args.Add("Message", value3);
						}
						else
						{
							solServerCommand.State = false;
							solServerCommand.Args.Add("err", "malformed tell message");
						}
					}
					solServerCommand.Send();
				}
				else if (!Emote.AttemptEmote(LocalPlayer.GameEntity, text))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unrecognized Command: " + text);
				}
			}
			else
			{
				if (!SolServerConnectionManager.IsOnline)
				{
					this.FailedToSend();
					return;
				}
				string text3 = this.m_input.text;
				MessageType type = ChatTabUI.m_validMessageTypes[this.m_channelDropdown.value];
				new SolServerCommand(CommandClass.chat, type.GetCommandType())
				{
					Args = 
					{
						{
							"Message",
							text3
						}
					}
				}.Send();
				if (!type.CanBeDefaultChannel())
				{
					this.m_channelDropdown.value = this.m_previousDefaultChannelIndex;
				}
			}
			this.AppendHistory();
			this.ClearAndDeactivateInput();
			LocalPlayer.UpdateTimeOfLastInput();
		}

		// Token: 0x06004A7B RID: 19067 RVA: 0x00072300 File Offset: 0x00070500
		private void ClearAndDeactivateInput()
		{
			this.m_input.text = string.Empty;
			this.m_input.Deactivate();
		}

		// Token: 0x06004A7C RID: 19068 RVA: 0x0007231D File Offset: 0x0007051D
		public void StartWhisper(string target)
		{
			this.m_input.text = "/tell " + target + " ";
			this.m_input.Activate();
		}

		// Token: 0x06004A7D RID: 19069 RVA: 0x001B3D44 File Offset: 0x001B1F44
		private bool ProcessLocalCommand(string inputCommand, string args)
		{
			if (inputCommand == "time")
			{
				string text = GameDateTime.UtcNow.DateTime.ToString("hh:mm tt on MMMM dd yyyy");
				if (GameTimeReplicator.Instance != null && GameTimeReplicator.Instance.TimeOverride.Value != null && SkyDomeManager.SkyDomeController != null)
				{
					DateTime time = SkyDomeManager.SkyDomeController.GetTime();
					string text2 = (time.Hour >= 12) ? "PM" : "AM";
					string text3 = time.ToString("MMMM dd yyyy");
					text = string.Concat(new string[]
					{
						time.Hour.ToString("00"),
						":",
						time.Minute.ToString("00"),
						" ",
						text2,
						" on ",
						text3
					});
				}
				string text4 = DateTime.Now.ToString("hh:mm tt on MMMM dd yyyy");
				string content = string.Concat(new string[]
				{
					"Current time:\nEmbers Adrift: ",
					text,
					"\n<size=80%>Local: ",
					text4,
					"</size>"
				});
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				return true;
			}
			if (inputCommand == "played")
			{
				string timePlayed = LocalPlayer.GetTimePlayed();
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, timePlayed);
				return true;
			}
			if (inputCommand == "stuck")
			{
				LocalPlayer.Motor.StuckRequested();
				return true;
			}
			if (inputCommand == "report")
			{
				DialogOptions dialogOptions = default(DialogOptions);
				dialogOptions.Title = "Report";
				dialogOptions.Text = args;
				dialogOptions.ConfirmationText = "Submit";
				dialogOptions.CancelText = "Cancel";
				dialogOptions.Callback = delegate(bool answer, object result)
				{
					string content2 = "Report Cancelled";
					if (answer)
					{
						string value = (string)result;
						if (!string.IsNullOrEmpty(value))
						{
							LocalPlayer.NetworkEntity.PlayerRpcHandler.SendReport(new LongString
							{
								Value = value
							});
							content2 = "Report Submitted";
						}
					}
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content2);
				};
				DialogOptions opts = dialogOptions;
				ClientGameManager.UIManager.TextEntryDialog.Init(opts);
				return true;
			}
			if (inputCommand == "debugposition")
			{
				LocalPlayer.CopyTeleportStringToClipboard();
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Debug position copied to clipboard");
				return true;
			}
			if (inputCommand == "gcreate")
			{
				ClientGameManager.SocialManager.CreateNewGuild(args);
				return true;
			}
			if (inputCommand == "ginvite")
			{
				ClientGameManager.SocialManager.InviteToGuild(args);
				return true;
			}
			if (inputCommand == "gpromote")
			{
				ClientGameManager.SocialManager.PromoteGuildMember(args);
				return true;
			}
			if (inputCommand == "gdemote")
			{
				ClientGameManager.SocialManager.DemoteGuildMember(args);
				return true;
			}
			if (inputCommand == "gkick")
			{
				ClientGameManager.SocialManager.KickGuildMember(args);
				return true;
			}
			if (inputCommand == "gleave")
			{
				ClientGameManager.SocialManager.LeaveGuild();
				return true;
			}
			if (inputCommand == "gdisband")
			{
				ClientGameManager.SocialManager.DisbandGuild();
				return true;
			}
			return false;
		}

		// Token: 0x17001074 RID: 4212
		// (get) Token: 0x06004A7E RID: 19070 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004A81 RID: 19073 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004537 RID: 17719
		[SerializeField]
		private TextMeshProUGUI m_content;

		// Token: 0x04004538 RID: 17720
		[SerializeField]
		private SolTMP_InputField m_input;

		// Token: 0x04004539 RID: 17721
		[SerializeField]
		private SolButton m_sendButton;

		// Token: 0x0400453A RID: 17722
		[SerializeField]
		private ScrollRect m_scrollRect;

		// Token: 0x0400453B RID: 17723
		[SerializeField]
		private TMP_Dropdown m_channelDropdown;

		// Token: 0x0400453C RID: 17724
		private const int kMaxHistory = 20;

		// Token: 0x0400453D RID: 17725
		private ChatMessageQueue m_queue;

		// Token: 0x0400453E RID: 17726
		private RectTransform m_scrollRectRectTransform;

		// Token: 0x0400453F RID: 17727
		private RectTransform m_contentSizeFitterRectTransform;

		// Token: 0x04004540 RID: 17728
		private int? m_updateScrollFrame;

		// Token: 0x04004542 RID: 17730
		private int m_historyIndex;

		// Token: 0x04004543 RID: 17731
		private readonly List<string> m_history = new List<string>();

		// Token: 0x04004544 RID: 17732
		private int m_previousDefaultChannelIndex;

		// Token: 0x04004545 RID: 17733
		[SerializeField]
		private bool m_chatQueue = true;

		// Token: 0x04004546 RID: 17734
		private static readonly Dictionary<string, CommandClass> m_validCommands = new Dictionary<string, CommandClass>
		{
			{
				"time",
				CommandClass.local
			},
			{
				"played",
				CommandClass.local
			},
			{
				"stuck",
				CommandClass.local
			},
			{
				"report",
				CommandClass.local
			},
			{
				"say",
				CommandClass.chat
			},
			{
				"yell",
				CommandClass.chat
			},
			{
				"tell",
				CommandClass.chat
			},
			{
				"group",
				CommandClass.chat
			},
			{
				"guild",
				CommandClass.chat
			},
			{
				"ooc",
				CommandClass.chat
			},
			{
				"who",
				CommandClass.chat
			},
			{
				"roll",
				CommandClass.chat
			},
			{
				"emote",
				CommandClass.chat
			},
			{
				"global",
				CommandClass.chat
			},
			{
				"invite",
				CommandClass.group
			},
			{
				"decline",
				CommandClass.group
			},
			{
				"accept",
				CommandClass.group
			},
			{
				"leave",
				CommandClass.group
			},
			{
				"promote",
				CommandClass.group
			},
			{
				"kick",
				CommandClass.group
			},
			{
				"gcreate",
				CommandClass.local
			},
			{
				"ginvite",
				CommandClass.local
			},
			{
				"gpromote",
				CommandClass.local
			},
			{
				"gdemote",
				CommandClass.local
			},
			{
				"gkick",
				CommandClass.local
			},
			{
				"gleave",
				CommandClass.local
			},
			{
				"gdisband",
				CommandClass.local
			},
			{
				"friend",
				CommandClass.relations
			},
			{
				"unfriend",
				CommandClass.relations
			},
			{
				"block",
				CommandClass.relations
			},
			{
				"unblock",
				CommandClass.relations
			},
			{
				"listlfg",
				CommandClass.lookingfor
			},
			{
				"startlfg",
				CommandClass.lookingfor
			},
			{
				"stoplfg",
				CommandClass.lookingfor
			},
			{
				"listlfm",
				CommandClass.lookingfor
			},
			{
				"startlfm",
				CommandClass.lookingfor
			},
			{
				"stoplfm",
				CommandClass.lookingfor
			},
			{
				"debugposition",
				CommandClass.local
			}
		};

		// Token: 0x04004547 RID: 17735
		private static readonly MessageType[] m_validMessageTypes = new MessageType[]
		{
			MessageType.Say,
			MessageType.Party,
			MessageType.Guild,
			MessageType.Yell,
			MessageType.Zone,
			MessageType.World
		};

		// Token: 0x04004548 RID: 17736
		private static Dictionary<MessageType, int> m_chatChannelToDropdownIndex = null;

		// Token: 0x04004549 RID: 17737
		private static Dictionary<int, MessageType> m_dropdownIndexToChatChannel = null;

		// Token: 0x0400454A RID: 17738
		private static readonly Dictionary<string, MessageType> m_chatChannelShortcuts = new Dictionary<string, MessageType>
		{
			{
				"say",
				MessageType.Say
			},
			{
				"sa",
				MessageType.Say
			},
			{
				"local",
				MessageType.Say
			},
			{
				"loca",
				MessageType.Say
			},
			{
				"loc",
				MessageType.Say
			},
			{
				"lo",
				MessageType.Say
			},
			{
				"group",
				MessageType.Party
			},
			{
				"grou",
				MessageType.Party
			},
			{
				"gro",
				MessageType.Party
			},
			{
				"gr",
				MessageType.Party
			},
			{
				"party",
				MessageType.Party
			},
			{
				"part",
				MessageType.Party
			},
			{
				"par",
				MessageType.Party
			},
			{
				"pa",
				MessageType.Party
			},
			{
				"p",
				MessageType.Party
			},
			{
				"guild",
				MessageType.Guild
			},
			{
				"guil",
				MessageType.Guild
			},
			{
				"gui",
				MessageType.Guild
			},
			{
				"gu",
				MessageType.Guild
			},
			{
				"g",
				MessageType.Guild
			},
			{
				"yell",
				MessageType.Yell
			},
			{
				"yel",
				MessageType.Yell
			},
			{
				"ye",
				MessageType.Yell
			},
			{
				"y",
				MessageType.Yell
			},
			{
				"ooc",
				MessageType.Zone
			},
			{
				"oo",
				MessageType.Zone
			},
			{
				"o",
				MessageType.Zone
			},
			{
				"zone",
				MessageType.Zone
			},
			{
				"zon",
				MessageType.Zone
			},
			{
				"zo",
				MessageType.Zone
			},
			{
				"z",
				MessageType.Zone
			},
			{
				"global",
				MessageType.World
			},
			{
				"globa",
				MessageType.World
			},
			{
				"glob",
				MessageType.World
			},
			{
				"glo",
				MessageType.World
			},
			{
				"gl",
				MessageType.World
			}
		};
	}
}
