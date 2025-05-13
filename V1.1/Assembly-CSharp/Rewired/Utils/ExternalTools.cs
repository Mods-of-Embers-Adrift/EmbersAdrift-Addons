using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rewired.Internal;
using Rewired.Utils.Interfaces;
using Rewired.Utils.Platforms.Windows;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace Rewired.Utils
{
	// Token: 0x02000064 RID: 100
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ExternalTools : IExternalTools
	{
		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x00045B73 File Offset: 0x00043D73
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x00045B7A File Offset: 0x00043D7A
		public static Func<object> getPlatformInitializerDelegate
		{
			get
			{
				return ExternalTools._getPlatformInitializerDelegate;
			}
			set
			{
				ExternalTools._getPlatformInitializerDelegate = value;
			}
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0004475B File Offset: 0x0004295B
		public void Destroy()
		{
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060003DA RID: 986 RVA: 0x00045B82 File Offset: 0x00043D82
		public bool isEditorPaused
		{
			get
			{
				return this._isEditorPaused;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060003DB RID: 987 RVA: 0x00045B8A File Offset: 0x00043D8A
		// (remove) Token: 0x060003DC RID: 988 RVA: 0x00045BA3 File Offset: 0x00043DA3
		public event Action<bool> EditorPausedStateChangedEvent
		{
			add
			{
				this._EditorPausedStateChangedEvent = (Action<bool>)Delegate.Combine(this._EditorPausedStateChangedEvent, value);
			}
			remove
			{
				this._EditorPausedStateChangedEvent = (Action<bool>)Delegate.Remove(this._EditorPausedStateChangedEvent, value);
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00045BBC File Offset: 0x00043DBC
		public object GetPlatformInitializer()
		{
			return Main.GetPlatformInitializer();
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00045BC3 File Offset: 0x00043DC3
		public string GetFocusedEditorWindowTitle()
		{
			return string.Empty;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool IsEditorSceneViewFocused()
		{
			return false;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool LinuxInput_IsJoystickPreconfigured(string name)
		{
			return false;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060003E1 RID: 993 RVA: 0x0009AF00 File Offset: 0x00099100
		// (remove) Token: 0x060003E2 RID: 994 RVA: 0x0009AF38 File Offset: 0x00099138
		public event Action<uint, bool> XboxOneInput_OnGamepadStateChange;

		// Token: 0x060003E3 RID: 995 RVA: 0x00045BCA File Offset: 0x00043DCA
		public int XboxOneInput_GetUserIdForGamepad(uint id)
		{
			return 0;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00045BCD File Offset: 0x00043DCD
		public ulong XboxOneInput_GetControllerId(uint unityJoystickId)
		{
			return 0UL;
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool XboxOneInput_IsGamepadActive(uint unityJoystickId)
		{
			return false;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00045BC3 File Offset: 0x00043DC3
		public string XboxOneInput_GetControllerType(ulong xboxControllerId)
		{
			return string.Empty;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00045BCA File Offset: 0x00043DCA
		public uint XboxOneInput_GetJoystickId(ulong xboxControllerId)
		{
			return 0U;
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0004475B File Offset: 0x0004295B
		public void XboxOne_Gamepad_UpdatePlugin()
		{
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel)
		{
			return false;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0004475B File Offset: 0x0004295B
		public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS)
		{
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00045BD1 File Offset: 0x00043DD1
		public void GetDeviceVIDPIDs(out List<int> vids, out List<int> pids)
		{
			vids = new List<int>();
			pids = new List<int>();
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00045BE1 File Offset: 0x00043DE1
		public int GetAndroidAPILevel()
		{
			return -1;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00045BE4 File Offset: 0x00043DE4
		public void WindowsStandalone_ForwardRawInput(IntPtr rawInputHeaderIndices, IntPtr rawInputDataIndices, uint indicesCount, IntPtr rawInputData, uint rawInputDataSize)
		{
			UnityEngine.Windows.Input.ForwardRawInput(rawInputHeaderIndices, rawInputDataIndices, indicesCount, rawInputData, rawInputDataSize);
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00045BF2 File Offset: 0x00043DF2
		public bool UnityUI_Graphic_GetRaycastTarget(object graphic)
		{
			return !(graphic as Graphic == null) && (graphic as Graphic).raycastTarget;
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00045C0F File Offset: 0x00043E0F
		public void UnityUI_Graphic_SetRaycastTarget(object graphic, bool value)
		{
			if (graphic as Graphic == null)
			{
				return;
			}
			(graphic as Graphic).raycastTarget = value;
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00045C2C File Offset: 0x00043E2C
		public bool UnityInput_IsTouchPressureSupported
		{
			get
			{
				return UnityEngine.Input.touchPressureSupported;
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00045C33 File Offset: 0x00043E33
		public float UnityInput_GetTouchPressure(ref Touch touch)
		{
			return touch.pressure;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00045C3B File Offset: 0x00043E3B
		public float UnityInput_GetTouchMaximumPossiblePressure(ref Touch touch)
		{
			return touch.maximumPossiblePressure;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00045C43 File Offset: 0x00043E43
		public IControllerTemplate CreateControllerTemplate(Guid typeGuid, object payload)
		{
			return ControllerTemplateFactory.Create(typeGuid, payload);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00045C4C File Offset: 0x00043E4C
		public Type[] GetControllerTemplateTypes()
		{
			return ControllerTemplateFactory.templateTypes;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00045C53 File Offset: 0x00043E53
		public Type[] GetControllerTemplateInterfaceTypes()
		{
			return ControllerTemplateFactory.templateInterfaceTypes;
		}

		// Token: 0x0400054A RID: 1354
		private static Func<object> _getPlatformInitializerDelegate;

		// Token: 0x0400054B RID: 1355
		private bool _isEditorPaused;

		// Token: 0x0400054C RID: 1356
		private Action<bool> _EditorPausedStateChangedEvent;
	}
}
