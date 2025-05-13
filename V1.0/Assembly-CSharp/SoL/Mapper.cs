using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Rewired;
using UnityEngine;

namespace SoL
{
	// Token: 0x02000216 RID: 534
	public class Mapper
	{
		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x060011F3 RID: 4595 RVA: 0x0004EC9E File Offset: 0x0004CE9E
		public IDictionary<int, Binding> Bindings
		{
			get
			{
				if (Mapper.m_bindings == null)
				{
					this.Load();
				}
				return Mapper.m_bindings;
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x060011F4 RID: 4596 RVA: 0x0004ECB2 File Offset: 0x0004CEB2
		public bool IsBinding
		{
			get
			{
				return this.m_bindingInProgress;
			}
		}

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060011F5 RID: 4597 RVA: 0x000E4F30 File Offset: 0x000E3130
		// (remove) Token: 0x060011F6 RID: 4598 RVA: 0x000E4F68 File Offset: 0x000E3168
		public event Action<Mapper.ConflictFoundEventData> ConflictFoundEvent;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x060011F7 RID: 4599 RVA: 0x000E4FA0 File Offset: 0x000E31A0
		// (remove) Token: 0x060011F8 RID: 4600 RVA: 0x000E4FD8 File Offset: 0x000E31D8
		public event Action<Mapper.InputMappedEventData> InputMappedEvent;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060011F9 RID: 4601 RVA: 0x000E5010 File Offset: 0x000E3210
		// (remove) Token: 0x060011FA RID: 4602 RVA: 0x000E5048 File Offset: 0x000E3248
		public event Action StartedEvent;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060011FB RID: 4603 RVA: 0x000E5080 File Offset: 0x000E3280
		// (remove) Token: 0x060011FC RID: 4604 RVA: 0x000E50B8 File Offset: 0x000E32B8
		public event Action StoppedEvent;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060011FD RID: 4605 RVA: 0x000E50F0 File Offset: 0x000E32F0
		// (remove) Token: 0x060011FE RID: 4606 RVA: 0x000E5128 File Offset: 0x000E3328
		public event Action MappingsChanged;

		// Token: 0x060011FF RID: 4607 RVA: 0x000E5160 File Offset: 0x000E3360
		public Mapper()
		{
			this.m_keyboardMapper.options = this.m_mappingOptions;
			this.m_mouseMapper.options = this.m_mappingOptions;
			this.m_keyboardMapper.ErrorEvent += this.OnError;
			this.m_keyboardMapper.ConflictFoundEvent += this.OnConflict;
			this.m_keyboardMapper.InputMappedEvent += this.OnMapped;
			this.m_keyboardMapper.StartedEvent += this.OnStarted;
			this.m_keyboardMapper.StoppedEvent += this.OnStopped;
			this.m_mouseMapper.ErrorEvent += this.OnError;
			this.m_mouseMapper.ConflictFoundEvent += this.OnConflict;
			this.m_mouseMapper.InputMappedEvent += this.OnMapped;
			this.m_mouseMapper.StartedEvent += this.OnStarted;
			this.m_mouseMapper.StoppedEvent += this.OnStopped;
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x0004ECBA File Offset: 0x0004CEBA
		public Binding GetBinding(int actionId)
		{
			if (Mapper.m_bindings == null)
			{
				this.Load();
			}
			if (Mapper.m_bindings.ContainsKey(actionId))
			{
				return Mapper.m_bindings[actionId];
			}
			return null;
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x0004ECE3 File Offset: 0x0004CEE3
		public Binding GetDefaultBinding(int actionId)
		{
			if (Mapper.m_defaultBindings == null)
			{
				this.LoadDefaults(null);
			}
			if (Mapper.m_defaultBindings.ContainsKey(actionId))
			{
				return Mapper.m_defaultBindings[actionId];
			}
			return null;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x000E52D4 File Offset: 0x000E34D4
		public void StartMapping(int actionId, int bindIndex)
		{
			if (this.m_bindingInProgress)
			{
				return;
			}
			Binding binding = this.GetBinding(actionId);
			ActionElementMap actionElementMap = null;
			if (binding.Elements[bindIndex].ControllerType != -1)
			{
				ControllerType controllerType = (ControllerType)binding.Elements[bindIndex].ControllerType;
				if (controllerType != ControllerType.Keyboard)
				{
					if (controllerType == ControllerType.Mouse)
					{
						List<ActionElementMap> list = new List<ActionElementMap>();
						SolInput.MouseMap.GetElementMapMatches((ActionElementMap x) => x.elementIndex == binding.Elements[bindIndex].ElementId && x.modifierKeyFlags == (ModifierKeyFlags)binding.Elements[bindIndex].ModifierKeyFlags, list);
						actionElementMap = list.FirstOrDefault<ActionElementMap>();
					}
				}
				else
				{
					List<ActionElementMap> list2 = new List<ActionElementMap>();
					SolInput.KeyboardMap.GetElementMapMatches((ActionElementMap x) => x.elementIndex == binding.Elements[bindIndex].ElementId && x.modifierKeyFlags == (ModifierKeyFlags)binding.Elements[bindIndex].ModifierKeyFlags, list2);
					actionElementMap = list2.FirstOrDefault<ActionElementMap>();
				}
			}
			SolInput.KeyboardMap.enabled = false;
			SolInput.MouseMap.enabled = false;
			InputMapper keyboardMapper = this.m_keyboardMapper;
			InputMapper.Context context = new InputMapper.Context();
			context.actionId = actionId;
			context.controllerMap = SolInput.KeyboardMap;
			int? num = (actionElementMap != null) ? new int?(actionElementMap.controllerMap.id) : null;
			int id = SolInput.KeyboardMap.id;
			context.actionElementMapToReplace = ((num.GetValueOrDefault() == id & num != null) ? actionElementMap : null);
			keyboardMapper.Start(context);
			InputMapper mouseMapper = this.m_mouseMapper;
			InputMapper.Context context2 = new InputMapper.Context();
			context2.actionId = actionId;
			context2.controllerMap = SolInput.MouseMap;
			num = ((actionElementMap != null) ? new int?(actionElementMap.controllerMap.id) : null);
			id = SolInput.MouseMap.id;
			context2.actionElementMapToReplace = ((num.GetValueOrDefault() == id & num != null) ? actionElementMap : null);
			mouseMapper.Start(context2);
			this.m_bindingInProgress = true;
			this.m_indexBeingBound = bindIndex;
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0004ED0D File Offset: 0x0004CF0D
		public void StopMapping()
		{
			this.m_keyboardMapper.Stop();
			this.m_mouseMapper.Stop();
			SolInput.KeyboardMap.enabled = true;
			SolInput.MouseMap.enabled = true;
			this.m_bindingInProgress = false;
			this.m_indexBeingBound = -1;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x000E5498 File Offset: 0x000E3698
		public void ResetAll()
		{
			SolInput.Player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
			SolInput.Player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
			SolInput.RefreshMaps();
			foreach (KeyValuePair<int, Binding> keyValuePair in Mapper.m_defaultBindings)
			{
				Binding binding = this.GetBinding(keyValuePair.Key);
				if (binding == null)
				{
					binding = new Binding(keyValuePair.Value.Elements.Length);
					Mapper.m_bindings.Add(keyValuePair.Key, binding);
				}
				binding.CopyFrom(keyValuePair.Value, keyValuePair.Value.DeriveMaps());
			}
			this.Save();
			Action mappingsChanged = this.MappingsChanged;
			if (mappingsChanged == null)
			{
				return;
			}
			mappingsChanged();
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x000E5574 File Offset: 0x000E3774
		public void ResetToDefaults(int actionId)
		{
			this.ResetActionForController(ControllerType.Keyboard, SolInput.Player.controllers.Keyboard.id, actionId);
			this.ResetActionForController(ControllerType.Mouse, SolInput.Player.controllers.Mouse.id, actionId);
			Binding defaultBinding = this.GetDefaultBinding(actionId);
			Mapper.m_bindings[actionId].CopyFrom(defaultBinding, defaultBinding.DeriveMaps());
			this.Save();
			Action mappingsChanged = this.MappingsChanged;
			if (mappingsChanged == null)
			{
				return;
			}
			mappingsChanged();
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x000E55F0 File Offset: 0x000E37F0
		private void ResetActionForController(ControllerType controllerType, int controllerId, int actionId)
		{
			Controller controller = SolInput.Player.controllers.GetController(controllerType, controllerId);
			SolInput.Player.controllers.maps.GetFirstMapInCategory(controller, 0).DeleteElementMapsWithAction(actionId);
			ControllerMap controllerMapInstance = ReInput.mapping.GetControllerMapInstance(controller, 0, 0);
			ControllerMap firstMapInCategory = SolInput.Player.controllers.maps.GetFirstMapInCategory(controller, 0);
			ActionElementMap[] elementMapsWithAction = controllerMapInstance.GetElementMapsWithAction(actionId);
			for (int i = 0; i < elementMapsWithAction.Length; i++)
			{
				firstMapInCategory.RemoveElementAssignmentConflicts(elementMapsWithAction[i]);
				if (!firstMapInCategory.CreateElementMap(elementMapsWithAction[i].actionId, elementMapsWithAction[i].axisContribution, elementMapsWithAction[i].keyCode, elementMapsWithAction[i].modifierKey1, elementMapsWithAction[i].modifierKey2, elementMapsWithAction[i].modifierKey3))
				{
					Debug.LogError("Unable to reset keybind to default, failed to create new keybind.");
				}
			}
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x0004ED49 File Offset: 0x0004CF49
		public void UnbindAction(int actionId)
		{
			Mapper.m_bindings[actionId].Unbind();
			SolInput.KeyboardMap.DeleteElementMapsWithAction(actionId);
			SolInput.MouseMap.DeleteElementMapsWithAction(actionId);
			this.Save();
			Action mappingsChanged = this.MappingsChanged;
			if (mappingsChanged == null)
			{
				return;
			}
			mappingsChanged();
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x000E56B4 File Offset: 0x000E38B4
		private void ClearConflictingAssignments(IEnumerable<BindingEventData> conflicts)
		{
			foreach (BindingEventData bindingEventData in conflicts)
			{
				if (Mapper.m_bindings.ContainsKey(bindingEventData.ActionId))
				{
					Mapper.m_bindings[bindingEventData.ActionId].Unbind(bindingEventData.BindingIndex);
				}
			}
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x0004ED89 File Offset: 0x0004CF89
		private void OnError(InputMapper.ErrorEventData eventData)
		{
			Debug.LogError(eventData.message);
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x0004ED96 File Offset: 0x0004CF96
		private void OnCancel(InputMapper.CanceledEventData eventData)
		{
			Debug.LogWarning(eventData.message);
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x000E5724 File Offset: 0x000E3924
		private void OnConflict(InputMapper.ConflictFoundEventData eventData)
		{
			IEnumerable<BindingEventData> enumerable = from x in eventData.conflicts
			select new BindingEventData
			{
				ActionId = x.actionId,
				Binding = (Mapper.m_bindings.ContainsKey(x.actionId) ? Mapper.m_bindings[x.actionId] : null),
				BindingIndex = (Mapper.m_bindings.ContainsKey(x.actionId) ? Mapper.m_bindings[x.actionId].IndexOfRuntimeMapId(x.elementMapId) : -1),
				RuntimeMap = x.elementMap,
				KeyCode = x.keyCode,
				ModifierKeyFlags = x.modifierKeyFlags
			};
			Action<Mapper.ConflictFoundEventData> conflictFoundEvent = this.ConflictFoundEvent;
			if (conflictFoundEvent != null)
			{
				conflictFoundEvent(new Mapper.ConflictFoundEventData
				{
					IsProtected = eventData.isProtected,
					Assignment = new BindingEventData
					{
						ActionId = eventData.assignment.action.id,
						Binding = Mapper.m_bindings[eventData.assignment.action.id],
						BindingIndex = this.m_indexBeingBound,
						RuntimeMap = eventData.assignment.elementMap,
						KeyCode = eventData.assignment.keyCode,
						ModifierKeyFlags = eventData.assignment.modifierKeyFlags
					},
					Conflicts = enumerable.ToArray<BindingEventData>()
				});
			}
			if (eventData.isProtected)
			{
				eventData.responseCallback(InputMapper.ConflictResponse.Cancel);
				this.StopMapping();
				return;
			}
			this.ClearConflictingAssignments(enumerable);
			eventData.responseCallback(InputMapper.ConflictResponse.Replace);
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x000E583C File Offset: 0x000E3A3C
		private void OnMapped(InputMapper.InputMappedEventData eventData)
		{
			int bindingIndex = Mapper.m_bindings[eventData.actionElementMap.actionId].Bind(this.m_indexBeingBound, (int)eventData.actionElementMap.controllerMap.controllerType, eventData.actionElementMap);
			Action<Mapper.InputMappedEventData> inputMappedEvent = this.InputMappedEvent;
			if (inputMappedEvent != null)
			{
				inputMappedEvent(new Mapper.InputMappedEventData
				{
					Assignment = new BindingEventData
					{
						ActionId = eventData.actionElementMap.actionId,
						Binding = Mapper.m_bindings[eventData.actionElementMap.actionId],
						BindingIndex = bindingIndex,
						RuntimeMap = eventData.actionElementMap,
						KeyCode = eventData.actionElementMap.keyCode,
						ModifierKeyFlags = eventData.actionElementMap.modifierKeyFlags
					}
				});
			}
			this.StopMapping();
			this.Save();
			Action mappingsChanged = this.MappingsChanged;
			if (mappingsChanged == null)
			{
				return;
			}
			mappingsChanged();
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0004EDA3 File Offset: 0x0004CFA3
		private void OnStarted(InputMapper.StartedEventData eventData)
		{
			Action startedEvent = this.StartedEvent;
			if (startedEvent == null)
			{
				return;
			}
			startedEvent();
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x0004EDB5 File Offset: 0x0004CFB5
		private void OnStopped(InputMapper.StoppedEventData eventData)
		{
			Action stoppedEvent = this.StoppedEvent;
			if (stoppedEvent == null)
			{
				return;
			}
			stoppedEvent();
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x000E5920 File Offset: 0x000E3B20
		private void Save()
		{
			ReInput.userDataStore.SaveControllerData(SolInput.Player.id, ControllerType.Keyboard, SolInput.Player.controllers.Keyboard.id);
			ReInput.userDataStore.SaveControllerData(SolInput.Player.id, ControllerType.Mouse, SolInput.Player.controllers.Mouse.id);
			PlayerPrefs.SetString("Bindings", JsonConvert.SerializeObject(Mapper.m_bindings));
			PlayerPrefs.SetInt("BindingsVersion", 1);
			PlayerPrefs.Save();
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x000E59A4 File Offset: 0x000E3BA4
		private void Load()
		{
			IEnumerable<InputAction> actions = this.GetActions();
			if (Mapper.m_defaultBindings == null)
			{
				this.LoadDefaults(actions);
			}
			if (PlayerPrefs.HasKey("Bindings") && PlayerPrefs.GetInt("BindingsVersion", 0) == 1)
			{
				IDictionary<int, Binding> dictionary = JsonConvert.DeserializeObject<IDictionary<int, Binding>>(PlayerPrefs.GetString("Bindings"));
				Mapper.m_bindings = new DictionaryList<int, Binding>(false);
				using (IEnumerator<InputAction> enumerator = actions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						InputAction inputAction = enumerator.Current;
						if (!inputAction.userAssignable)
						{
							SolInput.KeyboardMap.DeleteElementMapsWithAction(inputAction.id);
							SolInput.MouseMap.DeleteElementMapsWithAction(inputAction.id);
						}
						else
						{
							Binding binding = dictionary.ContainsKey(inputAction.id) ? dictionary[inputAction.id] : new Binding(inputAction.id, Mapper.m_defaultBindings[inputAction.id].Elements);
							binding.DeriveMaps();
							Mapper.m_bindings.Add(inputAction.id, binding);
						}
					}
					return;
				}
			}
			Mapper.m_bindings = new DictionaryList<int, Binding>(false);
			this.ResetAll();
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x000E5AD0 File Offset: 0x000E3CD0
		private void LoadDefaults(IEnumerable<InputAction> actions = null)
		{
			actions = (actions ?? this.GetActions());
			int num = 2;
			Mapper.m_defaultBindings = new DictionaryList<int, Binding>(false);
			foreach (InputAction inputAction in actions)
			{
				if (inputAction.userAssignable)
				{
					Binding binding = new Binding(num);
					binding.ActionId = inputAction.id;
					int num2 = 0;
					foreach (Controller controller in ReInput.controllers.Controllers)
					{
						foreach (ActionElementMap actionElementMap in ReInput.mapping.GetControllerMapInstance(controller, 0, 0).GetElementMapsWithAction(inputAction.id))
						{
							if (num2 >= num)
							{
								break;
							}
							binding.Elements[num2].ControllerType = (int)controller.type;
							binding.Elements[num2].ControllerId = controller.id;
							binding.Elements[num2].ElementId = actionElementMap.elementIndex;
							binding.Elements[num2].ModifierKeyFlags = (int)actionElementMap.modifierKeyFlags;
							num2++;
						}
					}
					Mapper.m_defaultBindings.Add(inputAction.id, binding);
				}
			}
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x000E5C54 File Offset: 0x000E3E54
		public string GetPrimaryBindingNameForAction(int actionId)
		{
			ActionElementMap[] mapsForBinding = this.GetMapsForBinding(actionId);
			if (mapsForBinding == null || mapsForBinding.Length == 0)
			{
				return null;
			}
			ActionElementMap actionElementMap = mapsForBinding[0];
			if (actionElementMap == null)
			{
				return null;
			}
			return actionElementMap.elementIdentifierName;
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x0004EDC7 File Offset: 0x0004CFC7
		public ActionElementMap[] GetMapsForBinding(int actionId)
		{
			return this.GetBinding(actionId).GetMaps();
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x0004EDD5 File Offset: 0x0004CFD5
		private IEnumerable<InputAction> GetActions()
		{
			return ReInput.mapping.ActionsInCategory("Default", true);
		}

		// Token: 0x04000FA8 RID: 4008
		private static IDictionary<int, Binding> m_bindings;

		// Token: 0x04000FA9 RID: 4009
		private static IDictionary<int, Binding> m_defaultBindings;

		// Token: 0x04000FAA RID: 4010
		private bool m_bindingInProgress;

		// Token: 0x04000FAB RID: 4011
		private int m_indexBeingBound = -1;

		// Token: 0x04000FAC RID: 4012
		private InputMapper m_keyboardMapper = new InputMapper();

		// Token: 0x04000FAD RID: 4013
		private InputMapper m_mouseMapper = new InputMapper();

		// Token: 0x04000FAE RID: 4014
		private InputMapper.Options m_mappingOptions = new InputMapper.Options
		{
			allowAxes = false,
			allowButtons = true,
			allowKeyboardKeysWithModifiers = true,
			allowKeyboardModifierKeyAsPrimary = true,
			checkForConflicts = true,
			checkForConflictsWithPlayerIds = new int[1]
		};

		// Token: 0x04000FAF RID: 4015
		public const string kUnboundText = "<i>Not currently bound.</i>";

		// Token: 0x04000FB0 RID: 4016
		private const string kBindingsPrefName = "Bindings";

		// Token: 0x04000FB1 RID: 4017
		private const string kBindingsVersionPrefName = "BindingsVersion";

		// Token: 0x04000FB2 RID: 4018
		private const string kDefaultBindingsPrefName = "DefaultBindings";

		// Token: 0x04000FB3 RID: 4019
		private const int kCurrentBindingsVersion = 1;

		// Token: 0x02000217 RID: 535
		public class ConflictFoundEventData
		{
			// Token: 0x170004C1 RID: 1217
			// (get) Token: 0x06001215 RID: 4629 RVA: 0x0004EDE7 File Offset: 0x0004CFE7
			// (set) Token: 0x06001216 RID: 4630 RVA: 0x0004EDEF File Offset: 0x0004CFEF
			public bool IsProtected { get; set; }

			// Token: 0x170004C2 RID: 1218
			// (get) Token: 0x06001217 RID: 4631 RVA: 0x0004EDF8 File Offset: 0x0004CFF8
			// (set) Token: 0x06001218 RID: 4632 RVA: 0x0004EE00 File Offset: 0x0004D000
			public BindingEventData Assignment { get; set; }

			// Token: 0x170004C3 RID: 1219
			// (get) Token: 0x06001219 RID: 4633 RVA: 0x0004EE09 File Offset: 0x0004D009
			// (set) Token: 0x0600121A RID: 4634 RVA: 0x0004EE11 File Offset: 0x0004D011
			public BindingEventData[] Conflicts { get; set; }
		}

		// Token: 0x02000218 RID: 536
		public class InputMappedEventData
		{
			// Token: 0x170004C4 RID: 1220
			// (get) Token: 0x0600121C RID: 4636 RVA: 0x0004EE1A File Offset: 0x0004D01A
			// (set) Token: 0x0600121D RID: 4637 RVA: 0x0004EE22 File Offset: 0x0004D022
			public BindingEventData Assignment { get; set; }
		}
	}
}
