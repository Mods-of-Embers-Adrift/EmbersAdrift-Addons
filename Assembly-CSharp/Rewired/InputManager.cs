using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Rewired.Platforms;
using Rewired.Utils;
using Rewired.Utils.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rewired
{
	// Token: 0x02000063 RID: 99
	[AddComponentMenu("Rewired/Input Manager")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class InputManager : InputManager_Base
	{
		// Token: 0x060003CC RID: 972 RVA: 0x00045B0A File Offset: 0x00043D0A
		protected override void OnInitialized()
		{
			this.SubscribeEvents();
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00045B12 File Offset: 0x00043D12
		protected override void OnDeinitialized()
		{
			this.UnsubscribeEvents();
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0009AE98 File Offset: 0x00099098
		protected override void DetectPlatform()
		{
			this.scriptingBackend = ScriptingBackend.Mono;
			this.scriptingAPILevel = ScriptingAPILevel.Net20;
			this.editorPlatform = EditorPlatform.None;
			this.platform = Platform.Unknown;
			this.webplayerPlatform = WebplayerPlatform.None;
			this.isEditor = false;
			if (SystemInfo.deviceName == null)
			{
				string empty = string.Empty;
			}
			if (SystemInfo.deviceModel == null)
			{
				string empty2 = string.Empty;
			}
			this.platform = Platform.Windows;
			this.scriptingBackend = ScriptingBackend.Mono;
			this.scriptingAPILevel = ScriptingAPILevel.Net46;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void CheckRecompile()
		{
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00045B1A File Offset: 0x00043D1A
		protected override IExternalTools GetExternalTools()
		{
			return new ExternalTools();
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00045B21 File Offset: 0x00043D21
		private bool CheckDeviceName(string searchPattern, string deviceName, string deviceModel)
		{
			return Regex.IsMatch(deviceName, searchPattern, RegexOptions.IgnoreCase) || Regex.IsMatch(deviceModel, searchPattern, RegexOptions.IgnoreCase);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00045B37 File Offset: 0x00043D37
		private void SubscribeEvents()
		{
			this.UnsubscribeEvents();
			SceneManager.sceneLoaded += this.OnSceneLoaded;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00045B50 File Offset: 0x00043D50
		private void UnsubscribeEvents()
		{
			SceneManager.sceneLoaded -= this.OnSceneLoaded;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00045B63 File Offset: 0x00043D63
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			base.OnSceneLoaded();
		}

		// Token: 0x04000549 RID: 1353
		private bool ignoreRecompile;
	}
}
