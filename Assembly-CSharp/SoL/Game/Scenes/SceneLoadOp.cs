using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SoL.Game.Scenes
{
	// Token: 0x02000758 RID: 1880
	public struct SceneLoadOp
	{
		// Token: 0x17000CC5 RID: 3269
		// (get) Token: 0x060037F4 RID: 14324 RVA: 0x00066295 File Offset: 0x00064495
		// (set) Token: 0x060037F5 RID: 14325 RVA: 0x0006629D File Offset: 0x0006449D
		public bool Activated { readonly get; private set; }

		// Token: 0x060037F6 RID: 14326 RVA: 0x0016C504 File Offset: 0x0016A704
		public SceneLoadOp(SceneReference sceneReference)
		{
			if (sceneReference.IsAddressable())
			{
				this.Type = SceneLoadOpType.Addressable;
				this.StandardOp = null;
				this.AddressableOp = Addressables.LoadSceneAsync(sceneReference.AddressableReference, LoadSceneMode.Additive, false, 100);
			}
			else
			{
				this.Type = SceneLoadOpType.Standard;
				this.StandardOp = SceneManager.LoadSceneAsync(sceneReference.ScenePath, LoadSceneMode.Additive);
				this.StandardOp.allowSceneActivation = false;
				this.AddressableOp = default(AsyncOperationHandle<SceneInstance>);
			}
			this.Activated = false;
		}

		// Token: 0x060037F7 RID: 14327 RVA: 0x0016C578 File Offset: 0x0016A778
		public float GetProgress()
		{
			SceneLoadOpType type = this.Type;
			if (type == SceneLoadOpType.Standard)
			{
				return this.StandardOp.progress;
			}
			if (type != SceneLoadOpType.Addressable)
			{
				return 0f;
			}
			return this.AddressableOp.PercentComplete * 0.9f;
		}

		// Token: 0x060037F8 RID: 14328 RVA: 0x0016C5BC File Offset: 0x0016A7BC
		public bool IsPreLoading()
		{
			SceneLoadOpType type = this.Type;
			if (type != SceneLoadOpType.Standard)
			{
				return type == SceneLoadOpType.Addressable && this.AddressableOp.PercentComplete < 1f;
			}
			return this.StandardOp.progress < 0.9f;
		}

		// Token: 0x060037F9 RID: 14329 RVA: 0x0016C604 File Offset: 0x0016A804
		public void AllowActivation()
		{
			if (this.Activated)
			{
				return;
			}
			SceneLoadOpType type = this.Type;
			if (type != SceneLoadOpType.Standard)
			{
				if (type == SceneLoadOpType.Addressable)
				{
					this.AddressableOp.Result.ActivateAsync();
				}
			}
			else
			{
				this.StandardOp.allowSceneActivation = true;
			}
			this.Activated = true;
		}

		// Token: 0x060037FA RID: 14330 RVA: 0x0016C658 File Offset: 0x0016A858
		public bool IsDone()
		{
			SceneLoadOpType type = this.Type;
			if (type != SceneLoadOpType.Standard)
			{
				return type == SceneLoadOpType.Addressable && this.AddressableOp.IsDone;
			}
			return this.StandardOp.isDone;
		}

		// Token: 0x040036DA RID: 14042
		private const float kStandardSceneLoadIsDone = 0.9f;

		// Token: 0x040036DB RID: 14043
		public readonly SceneLoadOpType Type;

		// Token: 0x040036DC RID: 14044
		public readonly AsyncOperation StandardOp;

		// Token: 0x040036DD RID: 14045
		public readonly AsyncOperationHandle<SceneInstance> AddressableOp;
	}
}
