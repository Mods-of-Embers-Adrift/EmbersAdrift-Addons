using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
[Serializable]
public class MouseLook
{
	// Token: 0x06000098 RID: 152 RVA: 0x00044BC3 File Offset: 0x00042DC3
	public void Init(Transform character, Transform camera)
	{
		this.m_CharacterTargetRot = character.localRotation;
		this.m_CameraTargetRot = camera.localRotation;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x000935DC File Offset: 0x000917DC
	public void LookRotation(Transform character, Transform camera)
	{
		float y = Input.GetAxis("Mouse X") * this.XSensitivity;
		float num = Input.GetAxis("Mouse Y") * this.YSensitivity;
		this.m_CharacterTargetRot *= Quaternion.Euler(0f, y, 0f);
		this.m_CameraTargetRot *= Quaternion.Euler(-num, 0f, 0f);
		if (this.clampVerticalRotation)
		{
			this.m_CameraTargetRot = this.ClampRotationAroundXAxis(this.m_CameraTargetRot);
		}
		if (this.smooth)
		{
			character.localRotation = Quaternion.Slerp(character.localRotation, this.m_CharacterTargetRot, this.smoothTime * Time.deltaTime);
			camera.localRotation = Quaternion.Slerp(camera.localRotation, this.m_CameraTargetRot, this.smoothTime * Time.deltaTime);
		}
		else
		{
			character.localRotation = this.m_CharacterTargetRot;
			camera.localRotation = this.m_CameraTargetRot;
		}
		this.UpdateCursorLock();
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00044BDD File Offset: 0x00042DDD
	public void SetCursorLock(bool value)
	{
		this.lockCursor = value;
		if (!this.lockCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00044BFA File Offset: 0x00042DFA
	public void UpdateCursorLock()
	{
		if (this.lockCursor)
		{
			this.InternalLockUpdate();
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000936D8 File Offset: 0x000918D8
	private void InternalLockUpdate()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			this.m_cursorIsLocked = false;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			this.m_cursorIsLocked = true;
		}
		if (this.m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			return;
		}
		if (!this.m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00093730 File Offset: 0x00091930
	private Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float num = 114.59156f * Mathf.Atan(q.x);
		num = Mathf.Clamp(num, this.MinimumX, this.MaximumX);
		q.x = Mathf.Tan(0.008726646f * num);
		return q;
	}

	// Token: 0x040001BA RID: 442
	public float XSensitivity = 2f;

	// Token: 0x040001BB RID: 443
	public float YSensitivity = 2f;

	// Token: 0x040001BC RID: 444
	public bool clampVerticalRotation = true;

	// Token: 0x040001BD RID: 445
	public float MinimumX = -90f;

	// Token: 0x040001BE RID: 446
	public float MaximumX = 90f;

	// Token: 0x040001BF RID: 447
	public bool smooth;

	// Token: 0x040001C0 RID: 448
	public float smoothTime = 5f;

	// Token: 0x040001C1 RID: 449
	public bool lockCursor = true;

	// Token: 0x040001C2 RID: 450
	private Quaternion m_CharacterTargetRot;

	// Token: 0x040001C3 RID: 451
	private Quaternion m_CameraTargetRot;

	// Token: 0x040001C4 RID: 452
	private bool m_cursorIsLocked = true;
}
