using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class ProMouse
{
	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000087 RID: 135 RVA: 0x00044AE4 File Offset: 0x00042CE4
	public static ProMouse Instance
	{
		get
		{
			if (ProMouse.instance == null || ProMouse.coroutineGameObject == null)
			{
				ProMouse.instance = new ProMouse();
			}
			return ProMouse.instance;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000088 RID: 136 RVA: 0x00044B09 File Offset: 0x00042D09
	public bool MouseBusy
	{
		get
		{
			return this.mouseOperationBeingPerformed;
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00044B11 File Offset: 0x00042D11
	private ProMouse()
	{
		ProMouse.coroutineGameObject = new GameObject("___CursorHandler___");
		this.cursorHandlerRef = ProMouse.coroutineGameObject.AddComponent<CursorHandler>();
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00044B38 File Offset: 0x00042D38
	public void SetCursorPosition(int xPos, int yPos)
	{
		this.cursorHandlerRef.StartCoroutine(this._SetCursorPosition(xPos, yPos, true));
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00044B4F File Offset: 0x00042D4F
	public void SetGlobalCursorPosition(int xPos, int yPos)
	{
		this.cursorHandlerRef.StartCoroutine(this._SetCursorPosition(xPos, yPos, false));
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00044B66 File Offset: 0x00042D66
	private IEnumerator _SetCursorPosition(int xPos, int yPos, bool relativeToWindow)
	{
		while (this.mouseOperationBeingPerformed)
		{
			yield return null;
		}
		this.mouseOperationBeingPerformed = true;
		yield return null;
		Vector2 vector = Vector2.zero;
		if (relativeToWindow)
		{
			vector = MouseWrapper.GetGlobalMousePosition() - this.GetLocalMousePosition();
		}
		MouseWrapper.MoveCursorToPoint((int)((float)xPos + vector.x), (int)((float)yPos + vector.y));
		this.mouseOperationBeingPerformed = false;
		yield break;
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00044B8A File Offset: 0x00042D8A
	public Vector2 GetGlobalMousePosition()
	{
		return MouseWrapper.GetGlobalMousePosition();
	}

	// Token: 0x0600008E RID: 142 RVA: 0x000934EC File Offset: 0x000916EC
	public Vector2 GetMainScreenSize()
	{
		return new Vector2((float)Screen.currentResolution.width, (float)Screen.currentResolution.height);
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00044B91 File Offset: 0x00042D91
	public Vector2 GetLocalMousePosition()
	{
		return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	}

	// Token: 0x040001B0 RID: 432
	private CursorHandler cursorHandlerRef;

	// Token: 0x040001B1 RID: 433
	private static ProMouse instance;

	// Token: 0x040001B2 RID: 434
	private static GameObject coroutineGameObject;

	// Token: 0x040001B3 RID: 435
	private bool mouseOperationBeingPerformed;
}
