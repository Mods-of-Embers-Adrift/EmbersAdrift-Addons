using System;
using System.Collections.Generic;
using SoL.Game.Messages;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SoL.Game.AssetBundles
{
	// Token: 0x02000D44 RID: 3396
	public static class AddressableManager
	{
		// Token: 0x0600664C RID: 26188 RVA: 0x00210560 File Offset: 0x0020E760
		public static void SpawnInstance(AssetReference reference, Transform parent, IAddressableSpawnedNotifier toNotify = null)
		{
			if (reference == null || !reference.RuntimeKeyIsValid())
			{
				return;
			}
			AddressableManager.AsyncLoadData asyncLoadData;
			if (!AddressableManager.m_asyncOperationHandles.TryGetValue(reference, out asyncLoadData))
			{
				asyncLoadData = new AddressableManager.AsyncLoadData(reference);
				AddressableManager.m_asyncOperationHandles.Add(reference, asyncLoadData);
			}
			asyncLoadData.SpawnInstance(parent, toNotify);
		}

		// Token: 0x0600664D RID: 26189 RVA: 0x002105A4 File Offset: 0x0020E7A4
		public static void DestroyInstance(AssetReference reference, GameObject obj)
		{
			AddressableManager.AsyncLoadData asyncLoadData;
			if (AddressableManager.m_asyncOperationHandles.TryGetValue(reference, out asyncLoadData))
			{
				asyncLoadData.OnDestroyInstance(obj);
			}
		}

		// Token: 0x040058EC RID: 22764
		private static readonly Dictionary<AssetReference, AddressableManager.AsyncLoadData> m_asyncOperationHandles = new Dictionary<AssetReference, AddressableManager.AsyncLoadData>();

		// Token: 0x02000D45 RID: 3397
		private class AsyncLoadData
		{
			// Token: 0x0600664F RID: 26191 RVA: 0x002105C8 File Offset: 0x0020E7C8
			public AsyncLoadData(AssetReference reference)
			{
				this.m_reference = reference;
				this.m_spawned = new List<GameObject>();
				this.m_queue = new Queue<Transform>();
				this.m_notifications = new Dictionary<Transform, IAddressableSpawnedNotifier>();
				this.m_handle = Addressables.LoadAssetAsync<GameObject>(this.m_reference);
				this.m_handle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
				{
					while (this.m_queue.Count > 0)
					{
						this.SpawnInstance(this.m_queue.Dequeue(), null);
					}
				};
			}

			// Token: 0x06006650 RID: 26192 RVA: 0x0021062C File Offset: 0x0020E82C
			public void SpawnInstance(Transform parent, IAddressableSpawnedNotifier toNotify)
			{
				if (toNotify != null && parent)
				{
					this.m_notifications.AddOrReplace(parent, toNotify);
				}
				if (this.m_handle.IsDone)
				{
					this.m_reference.InstantiateAsync(parent, false).Completed += delegate(AsyncOperationHandle<GameObject> handle)
					{
						if (handle.Result)
						{
							this.m_spawned.Add(handle.Result);
							handle.Result.AddComponent<AddressableOnDestroy>().Reference = this.m_reference;
							Transform parent2 = handle.Result.transform.parent;
							IAddressableSpawnedNotifier addressableSpawnedNotifier;
							if (parent2 && this.m_notifications.TryGetValue(parent2, out addressableSpawnedNotifier))
							{
								if (addressableSpawnedNotifier != null)
								{
									addressableSpawnedNotifier.NotifyOfSpawn();
								}
								this.m_notifications.Remove(parent2);
								return;
							}
						}
						else
						{
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification | MessageType.PreFormatted, "<color=\"red\">ERROR!</color> Unable to load assets. Please exit the game and select <b>\"Force Check\"</b> from the launcher options.");
							Debug.LogError("Addressables error loading " + handle.DebugName + "!");
						}
					};
					return;
				}
				this.m_queue.Enqueue(parent);
			}

			// Token: 0x06006651 RID: 26193 RVA: 0x00210690 File Offset: 0x0020E890
			public void OnDestroyInstance(GameObject obj)
			{
				Addressables.ReleaseInstance(obj);
				this.m_spawned.Remove(obj);
				if (this.m_spawned.Count <= 0)
				{
					if (this.m_handle.IsValid())
					{
						Addressables.Release<GameObject>(this.m_handle);
					}
					AddressableManager.m_asyncOperationHandles.Remove(this.m_reference);
				}
			}

			// Token: 0x040058ED RID: 22765
			private readonly AssetReference m_reference;

			// Token: 0x040058EE RID: 22766
			private readonly List<GameObject> m_spawned;

			// Token: 0x040058EF RID: 22767
			private readonly Queue<Transform> m_queue;

			// Token: 0x040058F0 RID: 22768
			private readonly Dictionary<Transform, IAddressableSpawnedNotifier> m_notifications;

			// Token: 0x040058F1 RID: 22769
			private readonly AsyncOperationHandle<GameObject> m_handle;
		}
	}
}
