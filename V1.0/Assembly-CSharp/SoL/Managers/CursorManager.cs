using System;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x020004F5 RID: 1269
	public static class CursorManager
	{
		// Token: 0x060023A0 RID: 9120 RVA: 0x0012B3AC File Offset: 0x001295AC
		public static void Initialize()
		{
			CursorManager.m_collection = Resources.Load<CursorCollection>("CursorCollection");
			CursorManager.SetCursorImage(CursorType.MainCursor, null);
		}

		// Token: 0x060023A1 RID: 9121 RVA: 0x0012B3D8 File Offset: 0x001295D8
		public static void ResetCursor()
		{
			CursorManager.UnlockCursorImage();
			CursorManager.SetCursorImage(CursorType.MainCursor, null);
			CursorManager.ResetCursorLockState();
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x0012B400 File Offset: 0x00129600
		public static void SetCursorImage(CursorType type, Vector2? hotSpot = null)
		{
			if (type == CursorManager.m_currentType || CursorManager.m_imageLocked)
			{
				return;
			}
			Vector2 hotspot = Vector2.zero;
			if (type - CursorType.ResizeVertical <= 3 || type - CursorType.ResizeVertical_Locked <= 3)
			{
				hotspot = ResizableWindowTrigger.kCursorHotspot;
			}
			CursorManager.m_currentType = type;
			Cursor.SetCursor(CursorManager.m_collection.GetCursorImage(type), hotspot, CursorMode.Auto);
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x00059A28 File Offset: 0x00057C28
		public static void LockCursorImage()
		{
			CursorManager.m_imageLocked = true;
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x00059A30 File Offset: 0x00057C30
		public static void UnlockCursorImage()
		{
			CursorManager.m_imageLocked = false;
		}

		// Token: 0x060023A5 RID: 9125 RVA: 0x00059A38 File Offset: 0x00057C38
		public static void SetCursorLockState(CursorLockMode lockMode)
		{
			Cursor.lockState = lockMode;
		}

		// Token: 0x060023A6 RID: 9126 RVA: 0x00059A40 File Offset: 0x00057C40
		public static void ResetCursorLockState()
		{
			Cursor.lockState = CursorLockMode.None;
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x0012B450 File Offset: 0x00129650
		public static void SetCursorHidden(bool hidden)
		{
			if (hidden && CursorManager.m_hiddenCursorPos == null)
			{
				CursorManager.m_hiddenCursorPos = new Vector2?(MouseWrapper.GetGlobalMousePosition());
				Cursor.visible = false;
				CursorManager.m_hideTime = Time.time;
				return;
			}
			if (!hidden && CursorManager.m_hiddenCursorPos != null)
			{
				CursorManager.ResetCursorLockState();
				MouseWrapper.MoveCursorToPoint((int)CursorManager.m_hiddenCursorPos.Value.x, (int)CursorManager.m_hiddenCursorPos.Value.y);
				Cursor.visible = true;
				CursorManager.m_hideTime = 0f;
				CursorManager.m_hiddenCursorPos = null;
			}
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x00059A48 File Offset: 0x00057C48
		public static bool IsCursorHidden()
		{
			return CursorManager.m_hiddenCursorPos != null;
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x00059A54 File Offset: 0x00057C54
		public static bool IsCursorHiddenForDoubleClickThreshold()
		{
			return CursorManager.m_hiddenCursorPos != null && Time.time - CursorManager.m_hideTime > InteractionManager.kDoubleClickThreshold;
		}

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x060023AA RID: 9130 RVA: 0x0012B4E0 File Offset: 0x001296E0
		// (remove) Token: 0x060023AB RID: 9131 RVA: 0x0012B514 File Offset: 0x00129714
		public static event Action GameModeUpdated;

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x060023AC RID: 9132 RVA: 0x00059A76 File Offset: 0x00057C76
		// (set) Token: 0x060023AD RID: 9133 RVA: 0x0012B548 File Offset: 0x00129748
		public static CursorGameMode GameMode
		{
			get
			{
				return CursorManager.m_mode;
			}
			private set
			{
				if (CursorManager.m_mode == value)
				{
					return;
				}
				CursorGameMode mode = CursorManager.m_mode;
				CursorManager.m_mode = value;
				if (CursorManager.m_mode != CursorGameMode.None)
				{
					CursorManager.SetCursorImage(CursorManager.m_mode.GetCursorTypeForMode(), null);
					CursorManager.LockCursorImage();
					CursorManager.m_mode.PlayAudioForMode();
				}
				else
				{
					CursorManager.UnlockCursorImage();
					CursorManager.SetCursorImage(CursorType.MainCursor, null);
					mode.PlayAudioForMode();
				}
				Action gameModeUpdated = CursorManager.GameModeUpdated;
				if (gameModeUpdated == null)
				{
					return;
				}
				gameModeUpdated();
			}
		}

		// Token: 0x060023AE RID: 9134 RVA: 0x00059A7D File Offset: 0x00057C7D
		public static void ToggleGameMode(CursorGameMode mode)
		{
			CursorManager.GameMode = ((CursorManager.GameMode == mode) ? CursorGameMode.None : mode);
		}

		// Token: 0x060023AF RID: 9135 RVA: 0x00059A90 File Offset: 0x00057C90
		public static void ExitGameMode(CursorGameMode mode)
		{
			if (CursorManager.GameMode == mode)
			{
				CursorManager.GameMode = CursorGameMode.None;
			}
		}

		// Token: 0x060023B0 RID: 9136 RVA: 0x00059AA0 File Offset: 0x00057CA0
		public static void ResetGameMode()
		{
			CursorManager.GameMode = CursorGameMode.None;
			UtilityItemExtensions.ResetUtilityItemMode();
		}

		// Token: 0x04002700 RID: 9984
		private static CursorType m_currentType;

		// Token: 0x04002701 RID: 9985
		private static bool m_imageLocked;

		// Token: 0x04002702 RID: 9986
		private static CursorCollection m_collection;

		// Token: 0x04002703 RID: 9987
		private static float m_hideTime;

		// Token: 0x04002704 RID: 9988
		private static Vector2? m_hiddenCursorPos;

		// Token: 0x04002706 RID: 9990
		private static CursorGameMode m_mode;
	}
}
