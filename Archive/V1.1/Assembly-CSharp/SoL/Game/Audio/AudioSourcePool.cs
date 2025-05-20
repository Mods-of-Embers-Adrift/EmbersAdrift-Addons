using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D08 RID: 3336
	public class AudioSourcePool : MonoBehaviour
	{
		// Token: 0x060064BF RID: 25791 RVA: 0x0020A754 File Offset: 0x00208954
		private void Awake()
		{
			if (AudioSourcePool.Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			AudioSourcePool.Instance = this;
			GameObject gameObject = new GameObject("PooledAudioSources");
			gameObject.transform.SetParent(base.gameObject.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			this.m_parent = gameObject.transform;
		}

		// Token: 0x060064C0 RID: 25792 RVA: 0x0020A7D8 File Offset: 0x002089D8
		private void Update()
		{
			if (this.m_toRequeue.Count > 0)
			{
				float time = Time.time;
				for (int i = this.m_toRequeue.Count - 1; i >= 0; i--)
				{
					if (time >= this.m_toRequeue[i].TimeToReturn)
					{
						this.m_toRequeue[i].Source.enabled = false;
						this.m_toRequeue[i].Source.priority = 128;
						this.m_toRequeue[i].Queue.Enqueue(this.m_toRequeue[i].Source);
						this.m_toRequeue.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x060064C1 RID: 25793 RVA: 0x0020A890 File Offset: 0x00208A90
		public AudioSource RentSource(int key, GameObject prefab, float? rentTime)
		{
			if (!prefab)
			{
				throw new ArgumentNullException("prefab");
			}
			Queue<AudioSource> queue;
			if (!this.m_poolQueue.TryGetValue(key, out queue))
			{
				queue = new Queue<AudioSource>(100);
				this.m_poolQueue.Add(key, queue);
			}
			AudioSource audioSource;
			if (queue.Count > 0)
			{
				audioSource = queue.Dequeue();
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, this.m_parent);
				gameObject.name = "PAS_" + key.ToString();
				audioSource = gameObject.GetComponent<AudioSource>();
				audioSource.RefreshMixerGroup();
			}
			if (rentTime != null && rentTime.Value > 0f)
			{
				AudioSourcePool.RequeueData item = new AudioSourcePool.RequeueData
				{
					Source = audioSource,
					Queue = queue,
					TimeToReturn = Time.time + rentTime.Value
				};
				this.m_toRequeue.Add(item);
			}
			return audioSource;
		}

		// Token: 0x060064C2 RID: 25794 RVA: 0x0020A96C File Offset: 0x00208B6C
		public void ReturnSource(int key, AudioSource source, float delay)
		{
			if (!source)
			{
				throw new ArgumentNullException("source");
			}
			Queue<AudioSource> queue;
			if (!this.m_poolQueue.TryGetValue(key, out queue))
			{
				queue = new Queue<AudioSource>(100);
				this.m_poolQueue.Add(key, queue);
			}
			if (delay <= 0f)
			{
				source.enabled = false;
				source.priority = 128;
				queue.Enqueue(source);
				return;
			}
			AudioSourcePool.RequeueData item = new AudioSourcePool.RequeueData
			{
				Source = source,
				Queue = queue,
				TimeToReturn = Time.time + delay
			};
			this.m_toRequeue.Add(item);
		}

		// Token: 0x04005771 RID: 22385
		public const int kDefaultPriority = 128;

		// Token: 0x04005772 RID: 22386
		public static AudioSourcePool Instance;

		// Token: 0x04005773 RID: 22387
		private readonly Dictionary<int, Queue<AudioSource>> m_poolQueue = new Dictionary<int, Queue<AudioSource>>();

		// Token: 0x04005774 RID: 22388
		private readonly List<AudioSourcePool.RequeueData> m_toRequeue = new List<AudioSourcePool.RequeueData>(100);

		// Token: 0x04005775 RID: 22389
		[NonSerialized]
		private Transform m_parent;

		// Token: 0x02000D09 RID: 3337
		private struct RequeueData
		{
			// Token: 0x04005776 RID: 22390
			public AudioSource Source;

			// Token: 0x04005777 RID: 22391
			public float TimeToReturn;

			// Token: 0x04005778 RID: 22392
			public Queue<AudioSource> Queue;
		}
	}
}
