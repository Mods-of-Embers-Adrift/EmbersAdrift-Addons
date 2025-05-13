using System;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CC7 RID: 3271
	public class CullingManager : MonoBehaviour
	{
		// Token: 0x170017AE RID: 6062
		// (get) Token: 0x0600631A RID: 25370 RVA: 0x00082CC8 File Offset: 0x00080EC8
		// (set) Token: 0x0600631B RID: 25371 RVA: 0x00082CCF File Offset: 0x00080ECF
		public static int MaxShadowCastingLights { get; private set; } = 3;

		// Token: 0x0600631C RID: 25372 RVA: 0x00082CD7 File Offset: 0x00080ED7
		public static void SetMaxShadowCastingLights(int value)
		{
			CullingManager.MaxShadowCastingLights = Mathf.Clamp(value, 0, 10);
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x00082CE7 File Offset: 0x00080EE7
		public static void ResetMaxShadowCastingLights()
		{
			CullingManager.MaxShadowCastingLights = 3;
		}

		// Token: 0x170017AF RID: 6063
		// (get) Token: 0x0600631E RID: 25374 RVA: 0x00047D46 File Offset: 0x00045F46
		private Camera CullingCamera
		{
			get
			{
				return ClientGameManager.MainCamera;
			}
		}

		// Token: 0x0600631F RID: 25375 RVA: 0x00206074 File Offset: 0x00204274
		private int SortBySqrMagnitudeDistance(ICullee a, ICullee b)
		{
			return a.SqrMagnitudeDistance.CompareTo(b.SqrMagnitudeDistance);
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x00206098 File Offset: 0x00204298
		private void Awake()
		{
			if (CullingManager.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			CullingManager.Instance = this;
			this.m_sorter = new Comparison<ICullee>(this.SortBySqrMagnitudeDistance);
			SceneCompositionManager.ZoneLoadStarted += this.ZoneLoadStarted;
			this.m_nearestFilters = new CullingManager.NearestFilter[]
			{
				new CullingManager.NearestFilter(CullingFlags.ObjectShadowLimit, new Func<int>(this.GetObjectShadowLimit)),
				new CullingManager.NearestFilter(CullingFlags.LightShadowLimit, new Func<int>(this.GetShadowCastingLightLimit)),
				new CullingManager.NearestFilterUma(CullingFlags.UmaFeatureLimit, new Func<int>(this.GetUmaFeatureLimit)),
				new CullingManager.NearestFilter(CullingFlags.IKLimit, new Func<int>(this.GetIkEnabledLimit)),
				new CullingManager.NearestFilter(CullingFlags.Physics, new Func<int>(this.GetPhysicEnabledLimit))
			};
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x00082CEF File Offset: 0x00080EEF
		private void OnDestroy()
		{
			this.CleanupCullingGroup();
			SceneCompositionManager.ZoneLoadStarted -= this.ZoneLoadStarted;
		}

		// Token: 0x06006322 RID: 25378 RVA: 0x00206168 File Offset: 0x00204368
		private void Update()
		{
			if (this.m_cullingGroup == null || !this.CullingCamera)
			{
				return;
			}
			if (this.m_delayedCullees.Count > 0)
			{
				int frameCount = Time.frameCount;
				this.m_delayedCulleesToRemove.Clear();
				foreach (KeyValuePair<int, CullingManager.DelayedCulleeData> keyValuePair in this.m_delayedCullees)
				{
					CullingManager.DelayedCulleeData value = keyValuePair.Value;
					bool flag = false;
					if (value.Cullee.Index == null || value.Cullee.IsNull())
					{
						flag = true;
					}
					else if (value.FrameCount < frameCount)
					{
						int distanceBand = this.GetDistanceBand(value.Cullee.Index.Value);
						value.Cullee.OnDistanceBandChanged(0, distanceBand, true);
						if (distanceBand == 0 && !this.m_nearest.Contains(value.Cullee))
						{
							this.m_nearest.Add(value.Cullee);
						}
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
			if (Time.time >= this.m_timeOfNextBoundingSphereUpdate)
			{
				this.m_timeOfNextBoundingSphereUpdate = Time.time + 1f;
				this.m_frameCountOfNextCulleeUpdate = new int?(Time.frameCount + 1);
				if (!this.m_cullingGroup.targetCamera)
				{
					this.m_cullingGroup.targetCamera = this.CullingCamera;
					this.m_cullingGroup.SetDistanceReferencePoint(this.CullingCamera.gameObject.transform);
				}
				this.UpdateBoundingSpheres();
				this.SortNearest();
				return;
			}
			if (this.m_frameCountOfNextCulleeUpdate != null && this.m_frameCountOfNextCulleeUpdate.Value <= Time.frameCount)
			{
				this.m_frameCountOfNextCulleeUpdate = null;
				this.UpdateNearest();
			}
		}

		// Token: 0x06006323 RID: 25379 RVA: 0x00082D08 File Offset: 0x00080F08
		private void ZoneLoadStarted(ZoneId obj)
		{
			this.CleanupCullingGroup();
		}

		// Token: 0x06006324 RID: 25380 RVA: 0x000447AE File Offset: 0x000429AE
		private int GetObjectShadowLimit()
		{
			return 12;
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x00082D10 File Offset: 0x00080F10
		private int GetShadowCastingLightLimit()
		{
			return CullingManager.MaxShadowCastingLights;
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x0006ADA7 File Offset: 0x00068FA7
		private int GetUmaFeatureLimit()
		{
			return 5;
		}

		// Token: 0x06006327 RID: 25383 RVA: 0x0005D9AA File Offset: 0x0005BBAA
		private int GetIkEnabledLimit()
		{
			return 8;
		}

		// Token: 0x06006328 RID: 25384 RVA: 0x000580DD File Offset: 0x000562DD
		private int GetPhysicEnabledLimit()
		{
			return 4;
		}

		// Token: 0x06006329 RID: 25385 RVA: 0x00206394 File Offset: 0x00204594
		private void InitializeCullingGroup()
		{
			if (this.m_cullingGroup != null || !this.CullingCamera)
			{
				return;
			}
			this.m_cullingGroup = new CullingGroup();
			this.m_cullingGroup.SetBoundingSpheres(this.m_boundingSpheres);
			this.m_cullingGroup.SetBoundingSphereCount(this.m_currentIndexPlusOne);
			float[] array = new float[CullingDistanceExt.CullingDistances.Length];
			for (int i = 0; i < CullingDistanceExt.CullingDistances.Length; i++)
			{
				array[i] = CullingDistanceExt.CullingDistances[i].GetDistance();
			}
			this.m_cullingGroup.SetBoundingDistances(array);
			this.m_cullingGroup.onStateChanged = new CullingGroup.StateChanged(this.OnStateChanged);
			this.m_cullingGroup.SetDistanceReferencePoint(this.CullingCamera.transform);
			this.m_cullingGroup.targetCamera = this.CullingCamera;
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x0020645C File Offset: 0x0020465C
		private void CleanupCullingGroup()
		{
			if (this.m_cullingGroup == null)
			{
				return;
			}
			this.m_delayedCullees.Clear();
			this.m_cullingGroup.onStateChanged = null;
			this.m_cullingGroup.Dispose();
			this.m_cullingGroup = null;
			this.m_currentIndexPlusOne = 0;
			this.m_availableIndexes.Clear();
			for (int i = 0; i < this.m_culledObjects.Length; i++)
			{
				if (!this.m_culledObjects[i].IsNull())
				{
					this.m_culledObjects[i].Index = null;
				}
				this.m_culledObjects[i] = null;
			}
		}

		// Token: 0x0600632B RID: 25387 RVA: 0x002064F0 File Offset: 0x002046F0
		private void UpdateBoundingSpheres()
		{
			for (int i = 0; i < this.m_currentIndexPlusOne; i++)
			{
				if (this.m_culledObjects[i].IsNull())
				{
					if (!this.m_availableIndexes.Contains(i))
					{
						this.m_availableIndexes.Enqueue(i);
					}
				}
				else
				{
					ICullee cullee = this.m_culledObjects[i];
					if (cullee.Index == null)
					{
						Debug.LogWarning("Missing culling index for " + cullee.gameObject.name);
					}
					else
					{
						this.m_boundingSpheres[cullee.Index.Value].position = cullee.gameObject.transform.position;
						this.m_boundingSpheres[cullee.Index.Value].radius = cullee.Radius;
					}
				}
			}
		}

		// Token: 0x0600632C RID: 25388 RVA: 0x002065CC File Offset: 0x002047CC
		private void SortNearest()
		{
			if (!this.CullingCamera || this.m_nearest.Count < 1)
			{
				return;
			}
			Vector3 position = this.CullingCamera.gameObject.transform.position;
			for (int i = 0; i < this.m_nearest.Count; i++)
			{
				if (this.m_nearest[i].IsNull())
				{
					this.m_nearest.RemoveAt(i);
					i--;
				}
				else
				{
					this.m_nearest[i].SqrMagnitudeDistance = (position - this.m_nearest[i].gameObject.transform.position).sqrMagnitude;
				}
			}
			this.m_nearest.Sort(this.m_sorter);
		}

		// Token: 0x0600632D RID: 25389 RVA: 0x00206694 File Offset: 0x00204894
		private void UpdateNearest()
		{
			for (int i = 0; i < this.m_nearestFilters.Length; i++)
			{
				this.m_nearestFilters[i].ResetCount();
				for (int j = 0; j < this.m_nearest.Count; j++)
				{
					this.m_nearestFilters[i].UpdateForCullee(this.m_nearest[j]);
				}
			}
		}

		// Token: 0x0600632E RID: 25390 RVA: 0x002066F0 File Offset: 0x002048F0
		public void RegisterCulledObject(ICullee cullee)
		{
			if (cullee.IsNull())
			{
				return;
			}
			int hashCode = cullee.GetHashCode();
			if (this.m_delayedCullees.ContainsKey(hashCode))
			{
				return;
			}
			int currentIndexPlusOne = this.m_currentIndexPlusOne;
			this.InitializeCullingGroup();
			int num;
			if (cullee.Index != null)
			{
				num = cullee.Index.Value;
			}
			else if (this.m_availableIndexes.Count > 0)
			{
				num = this.m_availableIndexes.Dequeue();
			}
			else
			{
				if (this.m_currentIndexPlusOne > 10000)
				{
					Debug.LogWarning("Exceeded kMaxCulledObjects!  Returning without assigning a valid index.");
					return;
				}
				num = this.m_currentIndexPlusOne;
				this.m_currentIndexPlusOne++;
			}
			cullee.Index = new int?(num);
			this.m_culledObjects[num] = cullee;
			this.m_boundingSpheres[num].position = cullee.gameObject.transform.position;
			this.m_boundingSpheres[num].radius = cullee.Radius;
			if (currentIndexPlusOne != this.m_currentIndexPlusOne)
			{
				this.SetBoundingSphereCount();
			}
			this.m_delayedCullees.Add(hashCode, new CullingManager.DelayedCulleeData(cullee));
		}

		// Token: 0x0600632F RID: 25391 RVA: 0x00206804 File Offset: 0x00204A04
		public void DeregisterCulledObject(ICullee cullee)
		{
			if (cullee.IsNull())
			{
				return;
			}
			if (cullee.Index != null)
			{
				this.m_nearest.Remove(cullee);
				if (!this.m_availableIndexes.Contains(cullee.Index.Value))
				{
					this.m_availableIndexes.Enqueue(cullee.Index.Value);
				}
				this.m_culledObjects[cullee.Index.Value] = null;
				cullee.Index = null;
			}
			this.m_delayedCullees.Remove(cullee.GetHashCode());
		}

		// Token: 0x06006330 RID: 25392 RVA: 0x002068A4 File Offset: 0x00204AA4
		private void OnStateChanged(CullingGroupEvent sphere)
		{
			ICullee cullee = this.m_culledObjects[sphere.index];
			if (cullee.IsNull())
			{
				return;
			}
			if (sphere.hasBecomeVisible)
			{
				cullee.OnCulleeBecameVisible();
			}
			if (sphere.hasBecomeInvisible)
			{
				cullee.OnCulleeBecameInvisible();
			}
			int hashCode = cullee.GetHashCode();
			bool flag = this.m_delayedCullees.ContainsKey(hashCode);
			if (flag)
			{
				this.m_delayedCullees.Remove(hashCode);
			}
			if (flag || sphere.currentDistance != sphere.previousDistance || sphere.currentDistance != (int)cullee.CurrentDistance)
			{
				cullee.OnDistanceBandChanged(sphere.previousDistance, sphere.currentDistance, flag);
				bool flag2 = this.m_nearest.Contains(cullee);
				if (sphere.currentDistance == 0)
				{
					if (!flag2)
					{
						this.m_nearest.Add(cullee);
						return;
					}
				}
				else if (flag2)
				{
					this.m_nearest.Remove(cullee);
				}
			}
		}

		// Token: 0x06006331 RID: 25393 RVA: 0x00082D17 File Offset: 0x00080F17
		public int GetDistanceBand(int index)
		{
			if (this.m_cullingGroup != null)
			{
				return this.m_cullingGroup.GetDistance(index);
			}
			return 0;
		}

		// Token: 0x06006332 RID: 25394 RVA: 0x00082D2F File Offset: 0x00080F2F
		private void SetBoundingSphereCount()
		{
			CullingGroup cullingGroup = this.m_cullingGroup;
			if (cullingGroup == null)
			{
				return;
			}
			cullingGroup.SetBoundingSphereCount(this.m_currentIndexPlusOne);
		}

		// Token: 0x06006333 RID: 25395 RVA: 0x0004475B File Offset: 0x0004295B
		private void VerboseLog(ICullee cullee, string msg)
		{
		}

		// Token: 0x0400564C RID: 22092
		public static CullingManager Instance = null;

		// Token: 0x0400564D RID: 22093
		private const int kMaxCulledObjects = 10000;

		// Token: 0x0400564E RID: 22094
		private const float kUpdateFrequency = 1f;

		// Token: 0x0400564F RID: 22095
		private const int kMaxShadowCastingObjects = 12;

		// Token: 0x04005650 RID: 22096
		private const int kMaxUmaFeatures = 5;

		// Token: 0x04005651 RID: 22097
		private const int kMaxIkEnabled = 8;

		// Token: 0x04005652 RID: 22098
		private const int kMaxPhysicsEnabled = 4;

		// Token: 0x04005653 RID: 22099
		private const int kMaximumShadowCastingLights = 10;

		// Token: 0x04005654 RID: 22100
		public const int kInitialMaxShadowCastingLights = 3;

		// Token: 0x04005656 RID: 22102
		private float m_timeOfNextBoundingSphereUpdate;

		// Token: 0x04005657 RID: 22103
		private int? m_frameCountOfNextCulleeUpdate;

		// Token: 0x04005658 RID: 22104
		private int m_currentIndexPlusOne;

		// Token: 0x04005659 RID: 22105
		private CullingGroup m_cullingGroup;

		// Token: 0x0400565A RID: 22106
		private readonly Queue<int> m_availableIndexes = new Queue<int>();

		// Token: 0x0400565B RID: 22107
		private readonly List<ICullee> m_nearest = new List<ICullee>();

		// Token: 0x0400565C RID: 22108
		private readonly ICullee[] m_culledObjects = new ICullee[10000];

		// Token: 0x0400565D RID: 22109
		private readonly BoundingSphere[] m_boundingSpheres = new BoundingSphere[10000];

		// Token: 0x0400565E RID: 22110
		private readonly List<int> m_delayedCulleesToRemove = new List<int>(10);

		// Token: 0x0400565F RID: 22111
		private readonly Dictionary<int, CullingManager.DelayedCulleeData> m_delayedCullees = new Dictionary<int, CullingManager.DelayedCulleeData>();

		// Token: 0x04005660 RID: 22112
		private Comparison<ICullee> m_sorter;

		// Token: 0x04005661 RID: 22113
		private CullingManager.NearestFilter[] m_nearestFilters;

		// Token: 0x02000CC8 RID: 3272
		private struct DelayedCulleeData
		{
			// Token: 0x06006336 RID: 25398 RVA: 0x00082D55 File Offset: 0x00080F55
			public DelayedCulleeData(ICullee cullee)
			{
				this.Cullee = cullee;
				this.FrameCount = Time.frameCount + 2;
			}

			// Token: 0x04005662 RID: 22114
			public readonly int FrameCount;

			// Token: 0x04005663 RID: 22115
			public readonly ICullee Cullee;
		}

		// Token: 0x02000CC9 RID: 3273
		private class NearestFilter
		{
			// Token: 0x06006337 RID: 25399 RVA: 0x00082D6B File Offset: 0x00080F6B
			public NearestFilter(CullingFlags flag, Func<int> getMaxCountCallback)
			{
				this.m_flag = flag;
				this.GetMaxCountCallback = getMaxCountCallback;
			}

			// Token: 0x06006338 RID: 25400 RVA: 0x00082D81 File Offset: 0x00080F81
			protected int GetMaxCount()
			{
				if (this.GetMaxCountCallback == null)
				{
					return 0;
				}
				return this.GetMaxCountCallback();
			}

			// Token: 0x06006339 RID: 25401 RVA: 0x00082D98 File Offset: 0x00080F98
			public virtual void ResetCount()
			{
				this.m_currentCount = 0;
			}

			// Token: 0x0600633A RID: 25402 RVA: 0x002069DC File Offset: 0x00204BDC
			public virtual void UpdateForCullee(ICullee cullee)
			{
				if (cullee.LimitFlags.HasBitFlag(this.m_flag))
				{
					if (this.m_currentCount < this.GetMaxCount())
					{
						cullee.UnsetFlag(this.m_flag);
						this.m_currentCount++;
						return;
					}
					cullee.SetFlag(this.m_flag);
				}
			}

			// Token: 0x04005664 RID: 22116
			private readonly CullingFlags m_flag;

			// Token: 0x04005665 RID: 22117
			private readonly Func<int> GetMaxCountCallback;

			// Token: 0x04005666 RID: 22118
			private int m_currentCount;
		}

		// Token: 0x02000CCA RID: 3274
		private class NearestFilterUma : CullingManager.NearestFilter
		{
			// Token: 0x0600633B RID: 25403 RVA: 0x00082DA1 File Offset: 0x00080FA1
			public NearestFilterUma(CullingFlags flag, Func<int> getMaxCountCallback) : base(flag, getMaxCountCallback)
			{
			}

			// Token: 0x0600633C RID: 25404 RVA: 0x00082DB6 File Offset: 0x00080FB6
			public override void ResetCount()
			{
				base.ResetCount();
				this.m_atlasCount = 0;
				this.m_atlasScale = 1f;
			}

			// Token: 0x0600633D RID: 25405 RVA: 0x00206A34 File Offset: 0x00204C34
			public override void UpdateForCullee(ICullee cullee)
			{
				base.UpdateForCullee(cullee);
				CulledEntity culledEntity;
				if (cullee.TryGetAsType(out culledEntity))
				{
					culledEntity.SetAtlasScale(this.m_atlasScale);
					this.m_atlasCount++;
					if (this.m_atlasCount >= base.GetMaxCount())
					{
						this.m_atlasCount = 0;
						this.m_atlasScale = Mathf.Clamp(this.m_atlasScale * 0.5f, 0.1f, 1f);
					}
				}
			}

			// Token: 0x04005667 RID: 22119
			private int m_atlasCount;

			// Token: 0x04005668 RID: 22120
			private float m_atlasScale = 1f;
		}
	}
}
