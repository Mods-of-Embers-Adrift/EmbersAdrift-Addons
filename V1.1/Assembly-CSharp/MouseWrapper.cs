using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200001F RID: 31
public static class MouseWrapper
{
	// Token: 0x06000083 RID: 131
	[DllImport("user32.dll")]
	private static extern int SetCursorPos(int x, int y);

	// Token: 0x06000084 RID: 132
	[DllImport("user32.dll")]
	private static extern bool GetCursorPos(out MouseWrapper.POINT mousePos);

	// Token: 0x06000085 RID: 133 RVA: 0x00093434 File Offset: 0x00091634
	public static void MoveCursorToPoint(int x, int y)
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
		{
			MouseWrapper.SetCursorPos(x, Screen.currentResolution.height - y);
			return;
		}
		throw new Exception("Unsupported platform for moving mouse");
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00093470 File Offset: 0x00091670
	public static Vector2 GetGlobalMousePosition()
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
		{
			MouseWrapper.POINT point;
			MouseWrapper.GetCursorPos(out point);
			int x = point.X;
			int y = point.Y;
			return new Vector2((float)x, (float)(Screen.currentResolution.height - y));
		}
		throw new Exception("Unsupported platform for getting global mouse position");
	}

	// Token: 0x02000020 RID: 32
	private struct POINT
	{
		// Token: 0x040001AE RID: 430
		public int X;

		// Token: 0x040001AF RID: 431
		public int Y;
	}
}
