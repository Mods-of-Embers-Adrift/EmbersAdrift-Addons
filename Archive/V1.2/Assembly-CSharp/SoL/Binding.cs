using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Rewired;
using UnityEngine;

namespace SoL
{
	// Token: 0x02000212 RID: 530
	public class Binding
	{
		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x0004EB75 File Offset: 0x0004CD75
		// (set) Token: 0x060011C1 RID: 4545 RVA: 0x0004EB7D File Offset: 0x0004CD7D
		public int ActionId { get; set; }

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0004EB86 File Offset: 0x0004CD86
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x0004EB8E File Offset: 0x0004CD8E
		public Binding.BindingElement[] Elements { get; set; }

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x0004EB97 File Offset: 0x0004CD97
		[JsonIgnore]
		public Binding.BindingElement Primary
		{
			get
			{
				return this.GetElement(0);
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x060011C5 RID: 4549 RVA: 0x0004EBA0 File Offset: 0x0004CDA0
		[JsonIgnore]
		public Binding.BindingElement Secondary
		{
			get
			{
				return this.GetElement(1);
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060011C6 RID: 4550 RVA: 0x000E4690 File Offset: 0x000E2890
		// (remove) Token: 0x060011C7 RID: 4551 RVA: 0x000E46C8 File Offset: 0x000E28C8
		public event Action<BindingEventData> PrimaryMappingUpdated;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060011C8 RID: 4552 RVA: 0x000E4700 File Offset: 0x000E2900
		// (remove) Token: 0x060011C9 RID: 4553 RVA: 0x000E4738 File Offset: 0x000E2938
		private event Action<BindingEventData> MappingUpdated;

		// Token: 0x060011CA RID: 4554 RVA: 0x00044765 File Offset: 0x00042965
		public Binding()
		{
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x0004EBA9 File Offset: 0x0004CDA9
		public Binding(int numOfElements)
		{
			this.InitializeElements(numOfElements);
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x000E4770 File Offset: 0x000E2970
		public Binding(int actionId, params Binding.BindingElement[] elements)
		{
			this.ActionId = actionId;
			if (this.Elements == null)
			{
				this.InitializeElements(elements.Length);
			}
			for (int i = 0; i < this.Elements.Length; i++)
			{
				this.Elements[i].ControllerType = elements[i].ControllerType;
				this.Elements[i].ControllerId = elements[i].ControllerId;
				this.Elements[i].ElementId = elements[i].ElementId;
				this.Elements[i].ModifierKeyFlags = elements[i].ModifierKeyFlags;
				this.Elements[i].RuntimeMap = elements[i].RuntimeMap;
			}
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x0004EBB8 File Offset: 0x0004CDB8
		public Binding.BindingElement GetElement(int index)
		{
			if (this.Elements.Length > index)
			{
				return this.Elements[index];
			}
			return null;
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x000E4818 File Offset: 0x000E2A18
		public void CopyFrom(Binding other, ActionElementMap[] runtimeMaps = null)
		{
			if (this.Elements == null)
			{
				this.InitializeElements(other.Elements.Length);
			}
			this.ActionId = other.ActionId;
			if (runtimeMaps == null)
			{
				for (int i = 0; i < this.Elements.Length; i++)
				{
					if (other.Elements.Length <= i)
					{
						return;
					}
					this.Elements[i].ControllerType = other.Elements[i].ControllerType;
					this.Elements[i].ControllerId = other.Elements[i].ControllerId;
					this.Elements[i].ElementId = other.Elements[i].ElementId;
					this.Elements[i].ModifierKeyFlags = other.Elements[i].ModifierKeyFlags;
					this.Elements[i].RuntimeMap = other.Elements[i].RuntimeMap;
					this.EmitMappingUpdated(i, this.Elements[i].RuntimeMap);
				}
				return;
			}
			int num = 0;
			while (num < this.Elements.Length && other.Elements.Length > num && runtimeMaps.Length > num)
			{
				this.Elements[num].ControllerType = other.Elements[num].ControllerType;
				this.Elements[num].ControllerId = other.Elements[num].ControllerId;
				this.Elements[num].ElementId = other.Elements[num].ElementId;
				this.Elements[num].ModifierKeyFlags = other.Elements[num].ModifierKeyFlags;
				this.Elements[num].RuntimeMap = runtimeMaps[num];
				this.EmitMappingUpdated(num, this.Elements[num].RuntimeMap);
				num++;
			}
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x000E49C8 File Offset: 0x000E2BC8
		public int Bind(int bindIndex, int controllerType, ActionElementMap map)
		{
			for (int i = 0; i < this.Elements.Length; i++)
			{
				if (i == bindIndex || this.Elements[i].ElementId == -1)
				{
					this.Elements[i].ControllerType = controllerType;
					this.Elements[i].ControllerId = map.controllerMap.controllerId;
					this.Elements[i].ElementId = map.elementIndex;
					this.Elements[i].ModifierKeyFlags = (int)map.modifierKeyFlags;
					this.Elements[i].RuntimeMap = map;
					this.EmitMappingUpdated(i, this.Elements[i].RuntimeMap);
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x000E4A74 File Offset: 0x000E2C74
		public void Unbind(int indexToUnbind)
		{
			if (indexToUnbind == -1)
			{
				return;
			}
			bool[] array = new bool[this.Elements.Length];
			for (int i = 0; i < this.Elements.Length; i++)
			{
				if (i == indexToUnbind)
				{
					this.Elements[i].ControllerType = -1;
					this.Elements[i].ControllerId = -1;
					this.Elements[i].ElementId = -1;
					this.Elements[i].ModifierKeyFlags = -1;
					this.Elements[i].RuntimeMap = null;
					array[i] = true;
				}
				if (i > indexToUnbind)
				{
					this.Elements[i - 1].ControllerType = this.Elements[i].ControllerType;
					this.Elements[i].ControllerType = -1;
					this.Elements[i - 1].ControllerId = this.Elements[i].ControllerId;
					this.Elements[i].ControllerId = -1;
					this.Elements[i - 1].ElementId = this.Elements[i].ElementId;
					this.Elements[i].ElementId = -1;
					this.Elements[i - 1].ModifierKeyFlags = this.Elements[i].ModifierKeyFlags;
					this.Elements[i].ModifierKeyFlags = -1;
					this.Elements[i - 1].RuntimeMap = this.Elements[i].RuntimeMap;
					this.Elements[i].RuntimeMap = null;
					array[i - 1] = true;
					array[i] = true;
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j])
				{
					this.EmitMappingUpdated(j, null);
				}
			}
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x000E4BFC File Offset: 0x000E2DFC
		public void Unbind()
		{
			for (int i = 0; i < this.Elements.Length; i++)
			{
				this.Elements[i].ControllerType = -1;
				this.Elements[i].ControllerId = -1;
				this.Elements[i].ElementId = -1;
				this.Elements[i].ModifierKeyFlags = -1;
				this.Elements[i].RuntimeMap = null;
				this.EmitMappingUpdated(i, null);
			}
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x000E4C6C File Offset: 0x000E2E6C
		public int IndexOfRuntimeMapId(int runtimeMapId)
		{
			for (int i = 0; i < this.Elements.Length; i++)
			{
				ActionElementMap runtimeMap = this.Elements[i].RuntimeMap;
				if (runtimeMap != null && runtimeMap.id == runtimeMapId)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x000E4CB0 File Offset: 0x000E2EB0
		private void InitializeElements(int numOfElements)
		{
			this.Elements = new Binding.BindingElement[numOfElements];
			for (int i = 0; i < numOfElements; i++)
			{
				this.Elements[i] = new Binding.BindingElement
				{
					ControllerType = -1,
					ControllerId = -1,
					ElementId = -1,
					ModifierKeyFlags = -1,
					RuntimeMap = null
				};
			}
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x000E4D08 File Offset: 0x000E2F08
		public ActionElementMap[] GetMaps()
		{
			if (this.Elements[0].RuntimeMap != null)
			{
				return (from x in this.Elements
				select x.RuntimeMap).ToArray<ActionElementMap>();
			}
			return this.DeriveMaps();
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x000E4D5C File Offset: 0x000E2F5C
		public ActionElementMap[] DeriveMaps()
		{
			ActionElementMap[] array = new ActionElementMap[this.Elements.Length];
			int num = 0;
			foreach (Binding.BindingElement bindingElement in this.Elements)
			{
				if (bindingElement.ControllerType != -1)
				{
					foreach (ActionElementMap actionElementMap in this.GetMapsForController(this.ActionId, bindingElement.ControllerType, bindingElement.ControllerId))
					{
						if (num < array.Length)
						{
							int elementIndex = actionElementMap.elementIndex;
							int? num2 = (bindingElement != null) ? new int?(bindingElement.ElementId) : null;
							if (elementIndex == num2.GetValueOrDefault() & num2 != null)
							{
								int modifierKeyFlags = (int)actionElementMap.modifierKeyFlags;
								num2 = ((bindingElement != null) ? new int?(bindingElement.ModifierKeyFlags) : null);
								if (modifierKeyFlags == num2.GetValueOrDefault() & num2 != null)
								{
									bindingElement.RuntimeMap = actionElementMap;
									array[num] = actionElementMap;
									num++;
								}
							}
						}
					}
				}
			}
			return array;
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x000E4E70 File Offset: 0x000E3070
		private ActionElementMap[] GetMapsForController(int actionId, int controllerType, int controllerId)
		{
			if (controllerType == -1)
			{
				return null;
			}
			Controller controller = SolInput.Player.controllers.GetController((ControllerType)controllerType, controllerId);
			List<ActionElementMap> list = new List<ActionElementMap>();
			SolInput.Player.controllers.maps.GetElementMapsWithAction(controller, actionId, true, list);
			return list.ToArray();
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000E4EBC File Offset: 0x000E30BC
		private void EmitMappingUpdated(int i, ActionElementMap map = null)
		{
			ActionElementMap actionElementMap = map ?? this.Elements[i].RuntimeMap;
			BindingEventData obj = new BindingEventData
			{
				ActionId = this.ActionId,
				Binding = this,
				BindingIndex = i,
				RuntimeMap = actionElementMap,
				KeyCode = ((actionElementMap != null) ? actionElementMap.keyCode : KeyCode.None),
				ModifierKeyFlags = ((actionElementMap != null) ? actionElementMap.modifierKeyFlags : ModifierKeyFlags.None)
			};
			Action<BindingEventData> mappingUpdated = this.MappingUpdated;
			if (mappingUpdated != null)
			{
				mappingUpdated(obj);
			}
			if (i == 0)
			{
				Action<BindingEventData> primaryMappingUpdated = this.PrimaryMappingUpdated;
				if (primaryMappingUpdated == null)
				{
					return;
				}
				primaryMappingUpdated(obj);
			}
		}

		// Token: 0x02000213 RID: 531
		public class BindingElement
		{
			// Token: 0x170004B4 RID: 1204
			// (get) Token: 0x060011D8 RID: 4568 RVA: 0x0004EBCF File Offset: 0x0004CDCF
			// (set) Token: 0x060011D9 RID: 4569 RVA: 0x0004EBD7 File Offset: 0x0004CDD7
			public int ControllerType { get; set; }

			// Token: 0x170004B5 RID: 1205
			// (get) Token: 0x060011DA RID: 4570 RVA: 0x0004EBE0 File Offset: 0x0004CDE0
			// (set) Token: 0x060011DB RID: 4571 RVA: 0x0004EBE8 File Offset: 0x0004CDE8
			public int ControllerId { get; set; }

			// Token: 0x170004B6 RID: 1206
			// (get) Token: 0x060011DC RID: 4572 RVA: 0x0004EBF1 File Offset: 0x0004CDF1
			// (set) Token: 0x060011DD RID: 4573 RVA: 0x0004EBF9 File Offset: 0x0004CDF9
			public int ElementId { get; set; }

			// Token: 0x170004B7 RID: 1207
			// (get) Token: 0x060011DE RID: 4574 RVA: 0x0004EC02 File Offset: 0x0004CE02
			// (set) Token: 0x060011DF RID: 4575 RVA: 0x0004EC0A File Offset: 0x0004CE0A
			public int ModifierKeyFlags { get; set; }

			// Token: 0x170004B8 RID: 1208
			// (get) Token: 0x060011E0 RID: 4576 RVA: 0x0004EC13 File Offset: 0x0004CE13
			// (set) Token: 0x060011E1 RID: 4577 RVA: 0x0004EC1B File Offset: 0x0004CE1B
			[JsonIgnore]
			public ActionElementMap RuntimeMap { get; set; }
		}
	}
}
