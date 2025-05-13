using System;
using Rewired;

namespace SoL
{
	// Token: 0x0200021B RID: 539
	public static class SolInput
	{
		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06001225 RID: 4645 RVA: 0x0004EE37 File Offset: 0x0004D037
		public static Mapper Mapper
		{
			get
			{
				if (SolInput.m_mapper == null)
				{
					SolInput.m_mapper = new Mapper();
				}
				return SolInput.m_mapper;
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06001226 RID: 4646 RVA: 0x0004EE4F File Offset: 0x0004D04F
		public static Player Player
		{
			get
			{
				if (SolInput.m_player == null)
				{
					SolInput.m_player = ReInput.players.GetPlayer(0);
				}
				return SolInput.m_player;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x0004EE6D File Offset: 0x0004D06D
		public static ControllerMap KeyboardMap
		{
			get
			{
				if (SolInput.m_keyboardMap == null)
				{
					SolInput.m_keyboardMap = SolInput.Player.controllers.maps.GetFirstMapInCategory(SolInput.Player.controllers.Keyboard, 0);
				}
				return SolInput.m_keyboardMap;
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06001228 RID: 4648 RVA: 0x0004EEA4 File Offset: 0x0004D0A4
		public static ControllerMap MouseMap
		{
			get
			{
				if (SolInput.m_mouseMap == null)
				{
					SolInput.m_mouseMap = SolInput.Player.controllers.maps.GetFirstMapInCategory(SolInput.Player.controllers.Mouse, 0);
				}
				return SolInput.m_mouseMap;
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x000E5D7C File Offset: 0x000E3F7C
		public static ControllerMap JoystickMap
		{
			get
			{
				if (SolInput.m_joystickMap == null && SolInput.Player.controllers.joystickCount > 0)
				{
					SolInput.m_joystickMap = SolInput.Player.controllers.maps.GetFirstMapInCategory(SolInput.Player.controllers.Joysticks[0], 1);
				}
				return SolInput.m_joystickMap;
			}
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x000E5DD8 File Offset: 0x000E3FD8
		public static void RefreshMaps()
		{
			SolInput.m_keyboardMap = SolInput.Player.controllers.maps.GetFirstMapInCategory(SolInput.Player.controllers.Keyboard, 0);
			SolInput.m_mouseMap = SolInput.Player.controllers.maps.GetFirstMapInCategory(SolInput.Player.controllers.Mouse, 0);
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0004EEDB File Offset: 0x0004D0DB
		public static bool GetButton(int actionId)
		{
			return SolInput.Player.GetButton(actionId);
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x0004EEE8 File Offset: 0x0004D0E8
		public static bool GetButtonDown(int actionId)
		{
			return SolInput.Player.GetButtonDown(actionId);
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x0004EEF5 File Offset: 0x0004D0F5
		public static bool GetButtonUp(int actionId)
		{
			return SolInput.Player.GetButtonUp(actionId);
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x0004EF02 File Offset: 0x0004D102
		public static float GetAxis(int actionId)
		{
			return SolInput.Player.GetAxis(actionId);
		}

		// Token: 0x04000FC1 RID: 4033
		private static Mapper m_mapper;

		// Token: 0x04000FC2 RID: 4034
		private static Player m_player;

		// Token: 0x04000FC3 RID: 4035
		private static ControllerMap m_keyboardMap;

		// Token: 0x04000FC4 RID: 4036
		private static ControllerMap m_mouseMap;

		// Token: 0x04000FC5 RID: 4037
		private static ControllerMap m_joystickMap;
	}
}
