using System;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D26 RID: 3366
	public class AmbientAudioController : GameEntityComponent
	{
		// Token: 0x0600654E RID: 25934 RVA: 0x00084315 File Offset: 0x00082515
		private void Awake()
		{
			if (!this.m_attachToMainCamera && base.GameEntity != null)
			{
				base.GameEntity.AmbientAudioController = this;
			}
		}

		// Token: 0x0600654F RID: 25935 RVA: 0x0020C754 File Offset: 0x0020A954
		public void EnterZone(IAmbientAudioZone zone)
		{
			if (this.m_activeAmbientZones.ContainsKey(zone.Key))
			{
				return;
			}
			AmbientAudioSource ambientAudioSource = (this.m_inactiveAmbientSources.Count > 0) ? this.m_inactiveAmbientSources.Pop() : this.CreateNewSource();
			if (ambientAudioSource != null)
			{
				ambientAudioSource.Zone = zone;
				this.m_activeAmbientZones.Add(zone.Key, ambientAudioSource);
			}
		}

		// Token: 0x06006550 RID: 25936 RVA: 0x0020C7BC File Offset: 0x0020A9BC
		public void ExitZone(IAmbientAudioZone zone)
		{
			AmbientAudioSource ambientAudioSource;
			if (this.m_activeAmbientZones.TryGetValue(zone.Key, out ambientAudioSource) && this.m_activeAmbientZones.Remove(zone.Key) && ambientAudioSource != null)
			{
				ambientAudioSource.Zone = null;
				this.m_inactiveAmbientSources.Push(ambientAudioSource);
			}
		}

		// Token: 0x06006551 RID: 25937 RVA: 0x0020C810 File Offset: 0x0020AA10
		private AmbientAudioSource CreateNewSource()
		{
			if (this.m_ambientParent == null)
			{
				this.m_ambientParent = new GameObject("AmbientAudioSources");
				this.m_ambientParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
				if (this.m_attachToMainCamera && ClientGameManager.MainCamera)
				{
					this.m_ambientParent.transform.SetParent(ClientGameManager.MainCamera.transform);
					this.m_ambientParent.transform.localPosition = Vector3.zero;
					this.m_ambientParent.transform.localRotation = Quaternion.identity;
				}
			}
			GameObject gameObject = new GameObject("AmbientAudioSource");
			gameObject.transform.SetParent(this.m_ambientParent.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			return gameObject.AddComponent<AmbientAudioSource>();
		}

		// Token: 0x04005817 RID: 22551
		[SerializeField]
		private bool m_attachToMainCamera;

		// Token: 0x04005818 RID: 22552
		private readonly Dictionary<int, AmbientAudioSource> m_activeAmbientZones = new Dictionary<int, AmbientAudioSource>();

		// Token: 0x04005819 RID: 22553
		private readonly Stack<AmbientAudioSource> m_inactiveAmbientSources = new Stack<AmbientAudioSource>();

		// Token: 0x0400581A RID: 22554
		private GameObject m_ambientParent;
	}
}
