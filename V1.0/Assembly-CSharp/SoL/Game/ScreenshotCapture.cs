using System;
using System.IO;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005B6 RID: 1462
	public class ScreenshotCapture : MonoBehaviour
	{
		// Token: 0x06002E2B RID: 11819 RVA: 0x0006004B File Offset: 0x0005E24B
		private void Start()
		{
			if (this.VerifyPath())
			{
				Debug.Log("Screenshot folder set to: " + this.m_fullPath);
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06002E2C RID: 11820 RVA: 0x001518D4 File Offset: 0x0014FAD4
		private void LateUpdate()
		{
			if (GameManager.Instance == null)
			{
				return;
			}
			bool flag = ClientGameManager.InputManager == null || !ClientGameManager.InputManager.PreventInputForUI;
			if (flag && SolInput.GetButtonDown(41) && ClientGameManager.UIManager != null && LocalPlayer.GameEntity != null)
			{
				ClientGameManager.UIManager.ToggleUI();
			}
			if (flag && SolInput.GetButtonDown(42))
			{
				this.TakeScreenshot();
			}
		}

		// Token: 0x06002E2D RID: 11821 RVA: 0x00151948 File Offset: 0x0014FB48
		private bool VerifyPath()
		{
			if (string.IsNullOrEmpty(this.m_fullPath))
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				this.m_fullPath = Path.Combine(folderPath, "EmbersAdrift");
			}
			if (!Directory.Exists(this.m_fullPath))
			{
				try
				{
					Directory.CreateDirectory(this.m_fullPath);
				}
				catch (Exception arg)
				{
					Debug.LogWarning(string.Format("Unable to create directory \"{0}\"\n{1}", this.m_fullPath, arg));
					return false;
				}
				return true;
			}
			return true;
		}

		// Token: 0x06002E2E RID: 11822 RVA: 0x001519C4 File Offset: 0x0014FBC4
		private void TakeScreenshot()
		{
			if (!this.VerifyPath())
			{
				Debug.LogWarning("Unable to validate path `" + this.m_fullPath + "`!");
				return;
			}
			string text = ZString.Format<string>("EmbersAdrift_{0}.png", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.fff"));
			ScreenCapture.CaptureScreenshot(Path.Combine(this.m_fullPath, text));
			string text2 = ZString.Format<string, string>("{0} saved to {1}", text, this.m_fullPath);
			Debug.Log(text2);
			if (LocalPlayer.GameEntity)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text2);
			}
			if (ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.PlayScreenshotAudio();
			}
		}

		// Token: 0x04002DAD RID: 11693
		private const string kSubDir = "EmbersAdrift";

		// Token: 0x04002DAE RID: 11694
		private string m_fullPath;
	}
}
