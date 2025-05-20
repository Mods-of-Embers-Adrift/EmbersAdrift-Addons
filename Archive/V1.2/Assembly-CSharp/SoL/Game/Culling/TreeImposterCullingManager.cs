using System;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CD3 RID: 3283
	public class TreeImposterCullingManager : MonoBehaviour
	{
		// Token: 0x170017BE RID: 6078
		// (get) Token: 0x06006368 RID: 25448 RVA: 0x00047D46 File Offset: 0x00045F46
		private Camera CullingCamera
		{
			get
			{
				return ClientGameManager.MainCamera;
			}
		}

		// Token: 0x06006369 RID: 25449 RVA: 0x00206C38 File Offset: 0x00204E38
		private void CloseThresholdChanged()
		{
			TreeImposterCullingManager.CloseThreshold = this.m_closeThreshold;
			if (this.m_culledObjects != null)
			{
				for (int i = 0; i < this.m_culledObjects.Length; i++)
				{
					if (!this.IsNull(this.m_culledObjects[i]))
					{
						this.m_culledObjects[i].RefreshCullee();
					}
				}
			}
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x00206C88 File Offset: 0x00204E88
		private void Awake()
		{
			if (TreeImposterCullingManager.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			TreeImposterCullingManager.Instance = this;
			TreeImposterCullingManager.CloseThreshold = this.m_closeThreshold;
			SceneCompositionManager.ZoneLoadStarted += this.ZoneLoadStarted;
			Options.VideoOptions.TreeImposterDistance.Changed += this.TreeImposterDistanceOnChanged;
			this.TreeImposterDistanceOnChanged();
		}

		// Token: 0x0600636B RID: 25451 RVA: 0x00082FB5 File Offset: 0x000811B5
		private void OnDestroy()
		{
			this.CleanupCullingGroups();
			SceneCompositionManager.ZoneLoadStarted -= this.ZoneLoadStarted;
			Options.VideoOptions.TreeImposterDistance.Changed -= this.TreeImposterDistanceOnChanged;
			TreeImposterCullingManager.Instance = null;
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x00206CEC File Offset: 0x00204EEC
		private void Update()
		{
			if (this.m_distanceCullingGroup == null || !this.CullingCamera)
			{
				return;
			}
			if (this.m_delayedCullees.Count > 0)
			{
				int frameCount = Time.frameCount;
				this.m_delayedCulleesToRemove.Clear();
				foreach (KeyValuePair<int, TreeImposterCullingManager.DelayedCulleeData> keyValuePair in this.m_delayedCullees)
				{
					TreeImposterCullingManager.DelayedCulleeData value = keyValuePair.Value;
					bool flag = false;
					if (value.Imposter.Index == null || this.IsNull(value.Imposter))
					{
						flag = true;
					}
					else if (value.FrameCount < frameCount)
					{
						int distanceBand = this.GetDistanceBand(value.Imposter.Index.Value);
						bool isVisible = this.m_frustumCullingGroup.IsVisible(value.Imposter.Index.Value);
						value.Imposter.DelayedInit(isVisible, distanceBand);
						flag = true;
					}
					if (flag)
					{
						this.m_delayedCulleesToRemove.Add(keyValuePair.Key);
					}
				}
				if (this.m_delayedCulleesToRemove.Count > 0)
				{
					for (int i = 0; i < this.m_delayedCulleesToRemove.Count; i++)
					{
						this.m_delayedCullees.Remove(this.m_delayedCulleesToRemove[i]);
					}
					this.m_delayedCulleesToRemove.Clear();
				}
			}
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x00082FEA File Offset: 0x000811EA
		private void ZoneLoadStarted(ZoneId obj)
		{
			this.CleanupCullingGroups();
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x00206E64 File Offset: 0x00205064
		private void TreeImposterDistanceOnChanged()
		{
			switch (Options.VideoOptions.TreeImposterDistance.Value)
			{
			case 0:
				this.m_closeThreshold = TreeImposterCullingDistance.Low;
				goto IL_45;
			case 2:
				this.m_closeThreshold = TreeImposterCullingDistance.High;
				goto IL_45;
			case 3:
				this.m_closeThreshold = TreeImposterCullingDistance.Ultra;
				goto IL_45;
			}
			this.m_closeThreshold = TreeImposterCullingDistance.Medium;
			IL_45:
			this.CloseThresholdChanged();
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x00082FF2 File Offset: 0x000811F2
		private void UseImposterBillboardsOnChanged()
		{
			base.gameObject.SetActive(Options.VideoOptions.UseImposterBillboards.Value);
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x00083009 File Offset: 0x00081209
		private bool IsNull(CulledTreeImposter imposter)
		{
			return !imposter || imposter == null || imposter.Equals(null);
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x00206EBC File Offset: 0x002050BC
		private void InitializeCullingGroups()
		{
			if (this.m_distanceCullingGroup != null || !this.CullingCamera)
			{
				return;
			}
			this.m_culledObjects = new CulledTreeImposter[this.m_capacity];
			this.m_distanceBoundingSpheres = new BoundingSphere[this.m_capacity];
			this.m_frustumBoundingSpheres = new BoundingSphere[this.m_capacity];
			float[] array = new float[TreeImposterCullingDistanceExt.TreeImposterCullingDistances.Length];
			for (int i = 0; i < TreeImposterCullingDistanceExt.TreeImposterCullingDistances.Length; i++)
			{
				array[i] = TreeImposterCullingDistanceExt.TreeImposterCullingDistances[i].GetDistance();
			}
			this.m_distanceCullingGroup = new CullingGroup();
			this.m_distanceCullingGroup.SetBoundingSpheres(this.m_distanceBoundingSpheres);
			this.m_distanceCullingGroup.SetBoundingSphereCount(this.m_currentIndexPlusOne);
			this.m_distanceCullingGroup.SetBoundingDistances(array);
			this.m_distanceCullingGroup.onStateChanged = new CullingGroup.StateChanged(this.OnDistanceGroupStateChanged);
			this.m_distanceCullingGroup.SetDistanceReferencePoint(this.CullingCamera.transform);
			this.m_distanceCullingGroup.targetCamera = this.CullingCamera;
			this.m_frustumCullingGroup = new CullingGroup();
			this.m_frustumCullingGroup.SetBoundingSpheres(this.m_frustumBoundingSpheres);
			this.m_frustumCullingGroup.SetBoundingSphereCount(this.m_currentIndexPlusOne);
			this.m_frustumCullingGroup.SetBoundingDistances(array);
			this.m_frustumCullingGroup.onStateChanged = new CullingGroup.StateChanged(this.OnFrustumGroupStateChanged);
			this.m_frustumCullingGroup.SetDistanceReferencePoint(this.CullingCamera.transform);
			this.m_frustumCullingGroup.targetCamera = this.CullingCamera;
		}

		// Token: 0x06006372 RID: 25458 RVA: 0x00207030 File Offset: 0x00205230
		private void CleanupCullingGroups()
		{
			if (this.m_distanceCullingGroup != null)
			{
				this.m_distanceCullingGroup.onStateChanged = null;
				this.m_distanceCullingGroup.Dispose();
				this.m_distanceCullingGroup = null;
			}
			if (this.m_frustumCullingGroup != null)
			{
				this.m_frustumCullingGroup.onStateChanged = null;
				this.m_frustumCullingGroup.Dispose();
				this.m_frustumCullingGroup = null;
			}
			this.m_delayedCullees.Clear();
			this.m_currentIndexPlusOne = 0;
			this.m_availableIndexes.Clear();
			if (this.m_culledObjects != null)
			{
				for (int i = 0; i < this.m_culledObjects.Length; i++)
				{
					if (!this.IsNull(this.m_culledObjects[i]))
					{
						this.m_culledObjects[i].Index = null;
					}
					this.m_culledObjects[i] = null;
				}
			}
			this.m_culledObjects = null;
			this.m_distanceBoundingSpheres = null;
			this.m_frustumBoundingSpheres = null;
		}

		// Token: 0x06006373 RID: 25459 RVA: 0x00207108 File Offset: 0x00205308
		public void RegisterCulledObject(CulledTreeImposter imposter)
		{
			if (this.IsNull(imposter))
			{
				return;
			}
			int hashCode = imposter.GetHashCode();
			if (this.m_delayedCullees.ContainsKey(hashCode))
			{
				return;
			}
			int currentIndexPlusOne = this.m_currentIndexPlusOne;
			this.InitializeCullingGroups();
			int num;
			if (imposter.Index != null)
			{
				num = imposter.Index.Value;
			}
			else if (this.m_availableIndexes.Count > 0)
			{
				num = this.m_availableIndexes.Dequeue();
			}
			else
			{
				if (this.m_currentIndexPlusOne >= this.m_culledObjects.Length)
				{
					Debug.LogWarning("Exceeded kMaxCulledObjects!  Returning without assigning a valid index.");
					return;
				}
				num = this.m_currentIndexPlusOne;
				this.m_currentIndexPlusOne++;
			}
			imposter.Index = new int?(num);
			this.m_culledObjects[num] = imposter;
			this.m_distanceBoundingSpheres[num].position = imposter.DistanceCenter;
			this.m_distanceBoundingSpheres[num].radius = imposter.DistanceRadius;
			this.m_frustumBoundingSpheres[num].position = imposter.FrustumCenter;
			this.m_frustumBoundingSpheres[num].radius = imposter.FrustumRadius;
			if (currentIndexPlusOne != this.m_currentIndexPlusOne)
			{
				this.SetBoundingSphereCount();
			}
			this.m_delayedCullees.Add(hashCode, new TreeImposterCullingManager.DelayedCulleeData(imposter));
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x00207244 File Offset: 0x00205444
		public void DeregisterCulledObject(CulledTreeImposter imposter)
		{
			if (this.IsNull(imposter))
			{
				return;
			}
			if (imposter.Index != null)
			{
				if (!this.m_availableIndexes.Contains(imposter.Index.Value))
				{
					this.m_availableIndexes.Enqueue(imposter.Index.Value);
				}
				this.m_culledObjects[imposter.Index.Value] = null;
				imposter.Index = null;
			}
			this.m_delayedCullees.Remove(imposter.GetHashCode());
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x002072D8 File Offset: 0x002054D8
		private void OnDistanceGroupStateChanged(CullingGroupEvent sphere)
		{
			CulledTreeImposter culledTreeImposter = this.m_culledObjects[sphere.index];
			if (this.IsNull(culledTreeImposter))
			{
				return;
			}
			int hashCode = culledTreeImposter.GetHashCode();
			bool flag = this.m_delayedCullees.ContainsKey(hashCode);
			if (flag)
			{
				this.m_delayedCullees.Remove(hashCode);
			}
			if (flag || sphere.currentDistance != sphere.previousDistance)
			{
				culledTreeImposter.OnDistanceBandChanged(sphere.previousDistance, sphere.currentDistance, flag);
			}
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x0020734C File Offset: 0x0020554C
		private void OnFrustumGroupStateChanged(CullingGroupEvent sphere)
		{
			bool hasBecomeVisible = sphere.hasBecomeVisible;
			bool hasBecomeInvisible = sphere.hasBecomeInvisible;
			if (hasBecomeVisible || hasBecomeInvisible)
			{
				CulledTreeImposter culledTreeImposter = this.m_culledObjects[sphere.index];
				if (!this.IsNull(culledTreeImposter))
				{
					if (hasBecomeVisible)
					{
						culledTreeImposter.OnCulleeBecameVisible();
					}
					if (hasBecomeInvisible)
					{
						culledTreeImposter.OnCulleeBecameInvisible();
					}
				}
			}
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x00083025 File Offset: 0x00081225
		public int GetDistanceBand(int index)
		{
			if (this.m_distanceCullingGroup != null)
			{
				return this.m_distanceCullingGroup.GetDistance(index);
			}
			return 0;
		}

		// Token: 0x06006378 RID: 25464 RVA: 0x0008303D File Offset: 0x0008123D
		private void SetBoundingSphereCount()
		{
			CullingGroup distanceCullingGroup = this.m_distanceCullingGroup;
			if (distanceCullingGroup != null)
			{
				distanceCullingGroup.SetBoundingSphereCount(this.m_currentIndexPlusOne);
			}
			CullingGroup frustumCullingGroup = this.m_frustumCullingGroup;
			if (frustumCullingGroup == null)
			{
				return;
			}
			frustumCullingGroup.SetBoundingSphereCount(this.m_currentIndexPlusOne);
		}

		// Token: 0x06006379 RID: 25465 RVA: 0x0004475B File Offset: 0x0004295B
		private void VerboseLog(ICullee cullee, string msg)
		{
		}

		// Token: 0x0400567D RID: 22141
		public static TreeImposterCullingManager Instance = null;

		// Token: 0x0400567E RID: 22142
		public static TreeImposterCullingDistance CloseThreshold = TreeImposterCullingDistance.Medium;

		// Token: 0x0400567F RID: 22143
		public const TreeImposterCullingDistance FarThreshold = TreeImposterCullingDistance.FarCull;

		// Token: 0x04005680 RID: 22144
		private int m_currentIndexPlusOne;

		// Token: 0x04005681 RID: 22145
		private CullingGroup m_distanceCullingGroup;

		// Token: 0x04005682 RID: 22146
		private CullingGroup m_frustumCullingGroup;

		// Token: 0x04005683 RID: 22147
		private readonly Queue<int> m_availableIndexes = new Queue<int>();

		// Token: 0x04005684 RID: 22148
		private CulledTreeImposter[] m_culledObjects;

		// Token: 0x04005685 RID: 22149
		private BoundingSphere[] m_distanceBoundingSpheres;

		// Token: 0x04005686 RID: 22150
		private BoundingSphere[] m_frustumBoundingSpheres;

		// Token: 0x04005687 RID: 22151
		[SerializeField]
		private int m_capacity = 10000;

		// Token: 0x04005688 RID: 22152
		[SerializeField]
		private TreeImposterCullingDistance m_closeThreshold = TreeImposterCullingDistance.Medium;

		// Token: 0x04005689 RID: 22153
		private readonly List<int> m_delayedCulleesToRemove = new List<int>(10);

		// Token: 0x0400568A RID: 22154
		private readonly Dictionary<int, TreeImposterCullingManager.DelayedCulleeData> m_delayedCullees = new Dictionary<int, TreeImposterCullingManager.DelayedCulleeData>();

		// Token: 0x02000CD4 RID: 3284
		private struct DelayedCulleeData
		{
			// Token: 0x0600637C RID: 25468 RVA: 0x000830B7 File Offset: 0x000812B7
			public DelayedCulleeData(CulledTreeImposter imposter)
			{
				this.Imposter = imposter;
				this.FrameCount = Time.frameCount + 2;
			}

			// Token: 0x0400568B RID: 22155
			public readonly int FrameCount;

			// Token: 0x0400568C RID: 22156
			public readonly CulledTreeImposter Imposter;
		}
	}
}
