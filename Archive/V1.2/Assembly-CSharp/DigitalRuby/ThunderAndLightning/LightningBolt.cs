using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000B3 RID: 179
	public class LightningBolt
	{
		// Token: 0x1700027D RID: 637
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x00047906 File Offset: 0x00045B06
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x0004790E File Offset: 0x00045B0E
		public float MinimumDelay { get; private set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x00047917 File Offset: 0x00045B17
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0004791F File Offset: 0x00045B1F
		public bool HasGlow { get; private set; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x00047928 File Offset: 0x00045B28
		public bool IsActive
		{
			get
			{
				return this.elapsedTime < this.lifeTime;
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x00047938 File Offset: 0x00045B38
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x00047940 File Offset: 0x00045B40
		public CameraMode CameraMode { get; private set; }

		// Token: 0x060006A8 RID: 1704 RVA: 0x000A8EB4 File Offset: 0x000A70B4
		public void SetupLightningBolt(LightningBoltDependencies dependencies)
		{
			if (dependencies == null || dependencies.Parameters.Count == 0)
			{
				Debug.LogError("Lightning bolt dependencies must not be null");
				return;
			}
			if (this.dependencies != null)
			{
				Debug.LogError("This lightning bolt is already in use!");
				return;
			}
			this.dependencies = dependencies;
			this.CameraMode = dependencies.CameraMode;
			this.timeSinceLevelLoad = LightningBoltScript.TimeSinceStart;
			this.CheckForGlow(dependencies.Parameters);
			this.MinimumDelay = float.MaxValue;
			if (dependencies.ThreadState != null)
			{
				this.startTimeOffset = DateTime.UtcNow;
				dependencies.ThreadState.AddActionForBackgroundThread(new Action(this.ProcessAllLightningParameters));
				return;
			}
			this.ProcessAllLightningParameters();
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00047972 File Offset: 0x00045B72
		public bool Update()
		{
			this.elapsedTime += LightningBoltScript.DeltaTime;
			if (this.elapsedTime > this.maxLifeTime)
			{
				return false;
			}
			if (this.hasLight)
			{
				this.UpdateLights();
			}
			return true;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000A8F58 File Offset: 0x000A7158
		public void Cleanup()
		{
			foreach (LightningBoltSegmentGroup lightningBoltSegmentGroup in this.segmentGroupsWithLight)
			{
				foreach (Light l in lightningBoltSegmentGroup.Lights)
				{
					this.CleanupLight(l);
				}
				lightningBoltSegmentGroup.Lights.Clear();
			}
			List<LightningBoltSegmentGroup> obj = LightningBolt.groupCache;
			lock (obj)
			{
				foreach (LightningBoltSegmentGroup item in this.segmentGroups)
				{
					LightningBolt.groupCache.Add(item);
				}
			}
			this.hasLight = false;
			this.elapsedTime = 0f;
			this.lifeTime = 0f;
			this.maxLifeTime = 0f;
			if (this.dependencies != null)
			{
				this.dependencies.ReturnToCache(this.dependencies);
				this.dependencies = null;
			}
			foreach (LightningBolt.LineRendererMesh lineRendererMesh in this.activeLineRenderers)
			{
				if (lineRendererMesh != null)
				{
					lineRendererMesh.Reset();
					LightningBolt.lineRendererCache.Add(lineRendererMesh);
				}
			}
			this.segmentGroups.Clear();
			this.segmentGroupsWithLight.Clear();
			this.activeLineRenderers.Clear();
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x000A9128 File Offset: 0x000A7328
		public LightningBoltSegmentGroup AddGroup()
		{
			List<LightningBoltSegmentGroup> obj = LightningBolt.groupCache;
			LightningBoltSegmentGroup lightningBoltSegmentGroup;
			lock (obj)
			{
				if (LightningBolt.groupCache.Count == 0)
				{
					lightningBoltSegmentGroup = new LightningBoltSegmentGroup();
				}
				else
				{
					int index = LightningBolt.groupCache.Count - 1;
					lightningBoltSegmentGroup = LightningBolt.groupCache[index];
					lightningBoltSegmentGroup.Reset();
					LightningBolt.groupCache.RemoveAt(index);
				}
			}
			this.segmentGroups.Add(lightningBoltSegmentGroup);
			return lightningBoltSegmentGroup;
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x000A91AC File Offset: 0x000A73AC
		public static void ClearCache()
		{
			foreach (LightningBolt.LineRendererMesh lineRendererMesh in LightningBolt.lineRendererCache)
			{
				if (lineRendererMesh != null)
				{
					UnityEngine.Object.Destroy(lineRendererMesh.GameObject);
				}
			}
			foreach (Light light in LightningBolt.lightCache)
			{
				if (light != null)
				{
					UnityEngine.Object.Destroy(light.gameObject);
				}
			}
			LightningBolt.lineRendererCache.Clear();
			LightningBolt.lightCache.Clear();
			List<LightningBoltSegmentGroup> obj = LightningBolt.groupCache;
			lock (obj)
			{
				LightningBolt.groupCache.Clear();
			}
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x000479A5 File Offset: 0x00045BA5
		private void CleanupLight(Light l)
		{
			if (l != null)
			{
				this.dependencies.LightRemoved(l);
				LightningBolt.lightCache.Add(l);
				l.gameObject.SetActive(false);
				LightningBolt.lightCount--;
			}
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x000479E4 File Offset: 0x00045BE4
		private void EnableLineRenderer(LightningBolt.LineRendererMesh lineRenderer, int tag)
		{
			if (lineRenderer != null && lineRenderer.GameObject != null && lineRenderer.Tag == tag && this.IsActive)
			{
				lineRenderer.PopulateMesh();
			}
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00047A11 File Offset: 0x00045C11
		private IEnumerator EnableLastRendererCoRoutine()
		{
			LightningBolt.LineRendererMesh lineRenderer = this.activeLineRenderers[this.activeLineRenderers.Count - 1];
			LightningBolt.LineRendererMesh lineRendererMesh = lineRenderer;
			int num = lineRendererMesh.Tag + 1;
			lineRendererMesh.Tag = num;
			int tag = num;
			yield return new WaitForSecondsLightning(this.MinimumDelay);
			this.EnableLineRenderer(lineRenderer, tag);
			yield break;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x000A92A0 File Offset: 0x000A74A0
		private LightningBolt.LineRendererMesh GetOrCreateLineRenderer()
		{
			LightningBolt.LineRendererMesh lineRendererMesh;
			while (LightningBolt.lineRendererCache.Count != 0)
			{
				int index = LightningBolt.lineRendererCache.Count - 1;
				lineRendererMesh = LightningBolt.lineRendererCache[index];
				LightningBolt.lineRendererCache.RemoveAt(index);
				if (lineRendererMesh != null && !(lineRendererMesh.Transform == null))
				{
					IL_49:
					lineRendererMesh.Transform.parent = null;
					lineRendererMesh.Transform.rotation = Quaternion.identity;
					lineRendererMesh.Transform.localScale = Vector3.one;
					lineRendererMesh.Transform.parent = this.dependencies.Parent.transform;
					lineRendererMesh.GameObject.layer = (lineRendererMesh.MeshRendererBolt.gameObject.layer = (lineRendererMesh.MeshRendererGlow.gameObject.layer = this.dependencies.Parent.layer));
					if (this.dependencies.UseWorldSpace)
					{
						lineRendererMesh.GameObject.transform.position = Vector3.zero;
					}
					else
					{
						lineRendererMesh.GameObject.transform.localPosition = Vector3.zero;
					}
					lineRendererMesh.MaterialGlow = this.dependencies.LightningMaterialMesh;
					lineRendererMesh.MaterialBolt = this.dependencies.LightningMaterialMeshNoGlow;
					if (!string.IsNullOrEmpty(this.dependencies.SortLayerName))
					{
						lineRendererMesh.MeshRendererGlow.sortingLayerName = (lineRendererMesh.MeshRendererBolt.sortingLayerName = this.dependencies.SortLayerName);
						lineRendererMesh.MeshRendererGlow.sortingOrder = (lineRendererMesh.MeshRendererBolt.sortingOrder = this.dependencies.SortOrderInLayer);
					}
					else
					{
						lineRendererMesh.MeshRendererGlow.sortingLayerName = (lineRendererMesh.MeshRendererBolt.sortingLayerName = null);
						lineRendererMesh.MeshRendererGlow.sortingOrder = (lineRendererMesh.MeshRendererBolt.sortingOrder = 0);
					}
					this.activeLineRenderers.Add(lineRendererMesh);
					return lineRendererMesh;
				}
			}
			lineRendererMesh = new LightningBolt.LineRendererMesh();
			goto IL_49;
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x000A947C File Offset: 0x000A767C
		private void RenderGroup(LightningBoltSegmentGroup group, LightningBoltParameters p)
		{
			if (group.SegmentCount == 0)
			{
				return;
			}
			float num = (this.dependencies.ThreadState == null) ? 0f : ((float)(DateTime.UtcNow - this.startTimeOffset).TotalSeconds);
			float num2 = this.timeSinceLevelLoad + group.Delay + num;
			Vector4 fadeLifeTime = new Vector4(num2, num2 + group.PeakStart, num2 + group.PeakEnd, num2 + group.LifeTime);
			float num3 = group.LineWidth * 0.5f * LightningBoltParameters.Scale;
			int num4 = group.Segments.Count - group.StartIndex;
			float num5 = (num3 - num3 * group.EndWidthMultiplier) / (float)num4;
			float num6;
			if (p.GrowthMultiplier > 0f)
			{
				num6 = group.LifeTime / (float)num4 * p.GrowthMultiplier;
				num = 0f;
			}
			else
			{
				num6 = 0f;
				num = 0f;
			}
			LightningBolt.LineRendererMesh currentLineRenderer = (this.activeLineRenderers.Count == 0) ? this.GetOrCreateLineRenderer() : this.activeLineRenderers[this.activeLineRenderers.Count - 1];
			if (!currentLineRenderer.PrepareForLines(num4))
			{
				if (currentLineRenderer.CustomTransform != null)
				{
					return;
				}
				if (this.dependencies.ThreadState != null)
				{
					this.dependencies.ThreadState.AddActionForMainThread(delegate(bool inDestroy)
					{
						if (!inDestroy)
						{
							this.EnableCurrentLineRenderer();
							currentLineRenderer = this.GetOrCreateLineRenderer();
						}
					}, true);
				}
				else
				{
					this.EnableCurrentLineRenderer();
					currentLineRenderer = this.GetOrCreateLineRenderer();
				}
			}
			currentLineRenderer.BeginLine(group.Segments[group.StartIndex].Start, group.Segments[group.StartIndex].End, num3, group.Color, p.Intensity, fadeLifeTime, p.GlowWidthMultiplier, p.GlowIntensity);
			for (int i = group.StartIndex + 1; i < group.Segments.Count; i++)
			{
				num3 -= num5;
				if (p.GrowthMultiplier < 1f)
				{
					num += num6;
					fadeLifeTime = new Vector4(num2 + num, num2 + group.PeakStart + num, num2 + group.PeakEnd, num2 + group.LifeTime);
				}
				currentLineRenderer.AppendLine(group.Segments[i].Start, group.Segments[i].End, num3, group.Color, p.Intensity, fadeLifeTime, p.GlowWidthMultiplier, p.GlowIntensity);
			}
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x00047A20 File Offset: 0x00045C20
		private static IEnumerator NotifyBolt(LightningBoltDependencies dependencies, LightningBoltParameters p, Transform transform, Vector3 start, Vector3 end)
		{
			float delaySeconds = p.delaySeconds;
			float lifeTime = p.LifeTime;
			yield return new WaitForSecondsLightning(delaySeconds);
			if (dependencies.LightningBoltStarted != null)
			{
				dependencies.LightningBoltStarted(p, start, end);
			}
			LightningCustomTransformStateInfo state = (p.CustomTransform == null) ? null : LightningCustomTransformStateInfo.GetOrCreateStateInfo();
			if (state != null)
			{
				state.Parameters = p;
				state.BoltStartPosition = start;
				state.BoltEndPosition = end;
				state.State = LightningCustomTransformState.Started;
				state.Transform = transform;
				p.CustomTransform(state);
				state.State = LightningCustomTransformState.Executing;
			}
			if (p.CustomTransform == null)
			{
				yield return new WaitForSecondsLightning(lifeTime);
			}
			else
			{
				while (lifeTime > 0f)
				{
					p.CustomTransform(state);
					lifeTime -= LightningBoltScript.DeltaTime;
					yield return null;
				}
			}
			if (p.CustomTransform != null)
			{
				state.State = LightningCustomTransformState.Ended;
				p.CustomTransform(state);
				LightningCustomTransformStateInfo.ReturnStateInfoToCache(state);
			}
			if (dependencies.LightningBoltEnded != null)
			{
				dependencies.LightningBoltEnded(p, start, end);
			}
			LightningBoltParameters.ReturnParametersToCache(p);
			yield break;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x000A96FC File Offset: 0x000A78FC
		private void ProcessParameters(LightningBoltParameters p, RangeOfFloats delay, LightningBoltDependencies depends)
		{
			this.MinimumDelay = Mathf.Min(delay.Minimum, this.MinimumDelay);
			p.delaySeconds = delay.Random(p.Random);
			if (depends.LevelOfDetailDistance > Mathf.Epsilon)
			{
				float num;
				if (p.Points.Count > 1)
				{
					num = Vector3.Distance(depends.CameraPos, p.Points[0]);
					num = Mathf.Min(new float[]
					{
						Vector3.Distance(depends.CameraPos, p.Points[p.Points.Count - 1])
					});
				}
				else
				{
					num = Vector3.Distance(depends.CameraPos, p.Start);
					num = Mathf.Min(new float[]
					{
						Vector3.Distance(depends.CameraPos, p.End)
					});
				}
				int num2 = Mathf.Min(8, (int)(num / depends.LevelOfDetailDistance));
				p.Generations = Mathf.Max(1, p.Generations - num2);
				p.GenerationWhereForksStopSubtractor = Mathf.Clamp(p.GenerationWhereForksStopSubtractor - num2, 0, 8);
			}
			p.generationWhereForksStop = p.Generations - p.GenerationWhereForksStopSubtractor;
			this.lifeTime = Mathf.Max(p.LifeTime + p.delaySeconds, this.lifeTime);
			this.maxLifeTime = Mathf.Max(this.lifeTime, this.maxLifeTime);
			p.forkednessCalculated = (int)Mathf.Ceil(p.Forkedness * (float)p.Generations);
			if (p.Generations > 0)
			{
				p.Generator = (p.Generator ?? LightningGenerator.GeneratorInstance);
				Vector3 start;
				Vector3 end;
				p.Generator.GenerateLightningBolt(this, p, out start, out end);
				p.Start = start;
				p.End = end;
			}
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x000A98A8 File Offset: 0x000A7AA8
		private void ProcessAllLightningParameters()
		{
			int maxLights = LightningBolt.MaximumLightsPerBatch / this.dependencies.Parameters.Count;
			RangeOfFloats delay = default(RangeOfFloats);
			List<int> list = new List<int>(this.dependencies.Parameters.Count + 1);
			int num = 0;
			foreach (LightningBoltParameters lightningBoltParameters in this.dependencies.Parameters)
			{
				delay.Minimum = lightningBoltParameters.DelayRange.Minimum + lightningBoltParameters.Delay;
				delay.Maximum = lightningBoltParameters.DelayRange.Maximum + lightningBoltParameters.Delay;
				lightningBoltParameters.maxLights = maxLights;
				list.Add(this.segmentGroups.Count);
				this.ProcessParameters(lightningBoltParameters, delay, this.dependencies);
			}
			list.Add(this.segmentGroups.Count);
			LightningBoltDependencies dependenciesRef = this.dependencies;
			using (IEnumerator<LightningBoltParameters> enumerator = dependenciesRef.Parameters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LightningBoltParameters parameters = enumerator.Current;
					Transform transform = this.RenderLightningBolt(parameters.quality, parameters.Generations, list[num], list[++num], parameters);
					if (dependenciesRef.ThreadState != null)
					{
						dependenciesRef.ThreadState.AddActionForMainThread(delegate(bool inDestroy)
						{
							if (!inDestroy)
							{
								dependenciesRef.StartCoroutine(LightningBolt.NotifyBolt(dependenciesRef, parameters, transform, parameters.Start, parameters.End));
							}
						}, false);
					}
					else
					{
						dependenciesRef.StartCoroutine(LightningBolt.NotifyBolt(dependenciesRef, parameters, transform, parameters.Start, parameters.End));
					}
				}
			}
			if (this.dependencies.ThreadState != null)
			{
				this.dependencies.ThreadState.AddActionForMainThread(new Action<bool>(this.EnableCurrentLineRendererFromThread), false);
				return;
			}
			this.EnableCurrentLineRenderer();
			this.dependencies.AddActiveBolt(this);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x000A9B1C File Offset: 0x000A7D1C
		private void EnableCurrentLineRendererFromThread(bool inDestroy)
		{
			try
			{
				if (!inDestroy)
				{
					this.EnableCurrentLineRenderer();
					this.dependencies.AddActiveBolt(this);
				}
			}
			finally
			{
				this.dependencies.ThreadState = null;
			}
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x000A9B64 File Offset: 0x000A7D64
		private void EnableCurrentLineRenderer()
		{
			if (this.activeLineRenderers.Count == 0)
			{
				return;
			}
			if (this.MinimumDelay <= 0f)
			{
				this.EnableLineRenderer(this.activeLineRenderers[this.activeLineRenderers.Count - 1], this.activeLineRenderers[this.activeLineRenderers.Count - 1].Tag);
				return;
			}
			this.dependencies.StartCoroutine(this.EnableLastRendererCoRoutine());
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x000A9BE0 File Offset: 0x000A7DE0
		private void RenderParticleSystems(Vector3 start, Vector3 end, float trunkWidth, float lifeTime, float delaySeconds)
		{
			if (trunkWidth > 0f)
			{
				if (this.dependencies.OriginParticleSystem != null)
				{
					this.dependencies.StartCoroutine(this.GenerateParticleCoRoutine(this.dependencies.OriginParticleSystem, start, delaySeconds));
				}
				if (this.dependencies.DestParticleSystem != null)
				{
					this.dependencies.StartCoroutine(this.GenerateParticleCoRoutine(this.dependencies.DestParticleSystem, end, delaySeconds + lifeTime * 0.8f));
				}
			}
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x000A9C70 File Offset: 0x000A7E70
		private Transform RenderLightningBolt(LightningBoltQualitySetting quality, int generations, int startGroupIndex, int endGroupIndex, LightningBoltParameters parameters)
		{
			if (this.segmentGroups.Count == 0 || startGroupIndex >= this.segmentGroups.Count || endGroupIndex > this.segmentGroups.Count)
			{
				return null;
			}
			Transform result = null;
			LightningLightParameters lp = parameters.LightParameters;
			if (lp != null)
			{
				if (this.hasLight |= lp.HasLight)
				{
					lp.LightPercent = Mathf.Clamp(lp.LightPercent, Mathf.Epsilon, 1f);
					lp.LightShadowPercent = Mathf.Clamp(lp.LightShadowPercent, 0f, 1f);
				}
				else
				{
					lp = null;
				}
			}
			LightningBoltSegmentGroup lightningBoltSegmentGroup = this.segmentGroups[startGroupIndex];
			Vector3 start = lightningBoltSegmentGroup.Segments[lightningBoltSegmentGroup.StartIndex].Start;
			Vector3 end = lightningBoltSegmentGroup.Segments[lightningBoltSegmentGroup.StartIndex + lightningBoltSegmentGroup.SegmentCount - 1].End;
			parameters.FadePercent = Mathf.Clamp(parameters.FadePercent, 0f, 0.5f);
			if (parameters.CustomTransform != null)
			{
				LightningBolt.LineRendererMesh currentLineRenderer = (this.activeLineRenderers.Count == 0 || !this.activeLineRenderers[this.activeLineRenderers.Count - 1].Empty) ? null : this.activeLineRenderers[this.activeLineRenderers.Count - 1];
				if (currentLineRenderer == null)
				{
					if (this.dependencies.ThreadState != null)
					{
						this.dependencies.ThreadState.AddActionForMainThread(delegate(bool inDestroy)
						{
							if (!inDestroy)
							{
								this.EnableCurrentLineRenderer();
								currentLineRenderer = this.GetOrCreateLineRenderer();
							}
						}, true);
					}
					else
					{
						this.EnableCurrentLineRenderer();
						currentLineRenderer = this.GetOrCreateLineRenderer();
					}
				}
				if (currentLineRenderer == null)
				{
					return null;
				}
				currentLineRenderer.CustomTransform = parameters.CustomTransform;
				result = currentLineRenderer.Transform;
			}
			for (int i = startGroupIndex; i < endGroupIndex; i++)
			{
				LightningBoltSegmentGroup lightningBoltSegmentGroup2 = this.segmentGroups[i];
				lightningBoltSegmentGroup2.Delay = parameters.delaySeconds;
				lightningBoltSegmentGroup2.LifeTime = parameters.LifeTime;
				lightningBoltSegmentGroup2.PeakStart = lightningBoltSegmentGroup2.LifeTime * parameters.FadePercent;
				lightningBoltSegmentGroup2.PeakEnd = lightningBoltSegmentGroup2.LifeTime - lightningBoltSegmentGroup2.PeakStart;
				float num = lightningBoltSegmentGroup2.PeakEnd - lightningBoltSegmentGroup2.PeakStart;
				float num2 = lightningBoltSegmentGroup2.LifeTime - lightningBoltSegmentGroup2.PeakEnd;
				lightningBoltSegmentGroup2.PeakStart *= parameters.FadeInMultiplier;
				lightningBoltSegmentGroup2.PeakEnd = lightningBoltSegmentGroup2.PeakStart + num * parameters.FadeFullyLitMultiplier;
				lightningBoltSegmentGroup2.LifeTime = lightningBoltSegmentGroup2.PeakEnd + num2 * parameters.FadeOutMultiplier;
				lightningBoltSegmentGroup2.LightParameters = lp;
				this.RenderGroup(lightningBoltSegmentGroup2, parameters);
			}
			if (this.dependencies.ThreadState != null)
			{
				this.dependencies.ThreadState.AddActionForMainThread(delegate(bool inDestroy)
				{
					if (!inDestroy)
					{
						this.RenderParticleSystems(start, end, parameters.TrunkWidth, parameters.LifeTime, parameters.delaySeconds);
						if (lp != null)
						{
							this.CreateLightsForGroup(this.segmentGroups[startGroupIndex], lp, quality, parameters.maxLights);
						}
					}
				}, false);
			}
			else
			{
				this.RenderParticleSystems(start, end, parameters.TrunkWidth, parameters.LifeTime, parameters.delaySeconds);
				if (lp != null)
				{
					this.CreateLightsForGroup(this.segmentGroups[startGroupIndex], lp, quality, parameters.maxLights);
				}
			}
			return result;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x000AA068 File Offset: 0x000A8268
		private void CreateLightsForGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, LightningBoltQualitySetting quality, int maxLights)
		{
			if (LightningBolt.lightCount == LightningBolt.MaximumLightCount || maxLights <= 0)
			{
				return;
			}
			float num = (this.lifeTime - group.PeakEnd) * lp.FadeOutMultiplier;
			float num2 = (group.PeakEnd - group.PeakStart) * lp.FadeFullyLitMultiplier;
			float num3 = group.PeakStart * lp.FadeInMultiplier + num2 + num;
			this.maxLifeTime = Mathf.Max(this.maxLifeTime, group.Delay + num3);
			this.segmentGroupsWithLight.Add(group);
			int segmentCount = group.SegmentCount;
			float num4;
			float num5;
			if (quality == LightningBoltQualitySetting.LimitToQualitySetting)
			{
				int qualityLevel = QualitySettings.GetQualityLevel();
				LightningQualityMaximum lightningQualityMaximum;
				if (LightningBoltParameters.QualityMaximums.TryGetValue(qualityLevel, out lightningQualityMaximum))
				{
					num4 = Mathf.Min(lp.LightPercent, lightningQualityMaximum.MaximumLightPercent);
					num5 = Mathf.Min(lp.LightShadowPercent, lightningQualityMaximum.MaximumShadowPercent);
				}
				else
				{
					Debug.LogError("Unable to read lightning quality for level " + qualityLevel.ToString());
					num4 = lp.LightPercent;
					num5 = lp.LightShadowPercent;
				}
			}
			else
			{
				num4 = lp.LightPercent;
				num5 = lp.LightShadowPercent;
			}
			maxLights = Mathf.Max(1, Mathf.Min(maxLights, (int)((float)segmentCount * num4)));
			int num6 = Mathf.Max(1, segmentCount / maxLights);
			int num7 = maxLights - (int)((float)maxLights * num5);
			int num8 = num7;
			for (int i = group.StartIndex + (int)((float)num6 * 0.5f); i < group.Segments.Count; i += num6)
			{
				if (this.AddLightToGroup(group, lp, i, num6, num7, ref maxLights, ref num8))
				{
					return;
				}
			}
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x000AA1E0 File Offset: 0x000A83E0
		private bool AddLightToGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, int segmentIndex, int nthLight, int nthShadows, ref int maxLights, ref int nthShadowCounter)
		{
			Light orCreateLight = this.GetOrCreateLight(lp);
			group.Lights.Add(orCreateLight);
			Vector3 vector = (group.Segments[segmentIndex].Start + group.Segments[segmentIndex].End) * 0.5f;
			if (this.dependencies.CameraIsOrthographic)
			{
				if (this.dependencies.CameraMode == CameraMode.OrthographicXZ)
				{
					vector.y = this.dependencies.CameraPos.y + lp.OrthographicOffset;
				}
				else
				{
					vector.z = this.dependencies.CameraPos.z + lp.OrthographicOffset;
				}
			}
			if (this.dependencies.UseWorldSpace)
			{
				orCreateLight.gameObject.transform.position = vector;
			}
			else
			{
				orCreateLight.gameObject.transform.localPosition = vector;
			}
			if (lp.LightShadowPercent != 0f)
			{
				int num = nthShadowCounter + 1;
				nthShadowCounter = num;
				if (num >= nthShadows)
				{
					orCreateLight.shadows = LightShadows.Soft;
					nthShadowCounter = 0;
					goto IL_100;
				}
			}
			orCreateLight.shadows = LightShadows.None;
			IL_100:
			if (++LightningBolt.lightCount != LightningBolt.MaximumLightCount)
			{
				int num = maxLights - 1;
				maxLights = num;
				return num == 0;
			}
			return true;
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x000AA314 File Offset: 0x000A8514
		private Light GetOrCreateLight(LightningLightParameters lp)
		{
			Light light;
			while (LightningBolt.lightCache.Count != 0)
			{
				light = LightningBolt.lightCache[LightningBolt.lightCache.Count - 1];
				LightningBolt.lightCache.RemoveAt(LightningBolt.lightCache.Count - 1);
				if (!(light == null))
				{
					IL_5B:
					light.bounceIntensity = lp.BounceIntensity;
					light.shadowNormalBias = lp.ShadowNormalBias;
					light.color = lp.LightColor;
					light.renderMode = lp.RenderMode;
					light.range = lp.LightRange;
					light.shadowStrength = lp.ShadowStrength;
					light.shadowBias = lp.ShadowBias;
					light.intensity = 0f;
					light.gameObject.transform.parent = this.dependencies.Parent.transform;
					light.gameObject.SetActive(true);
					this.dependencies.LightAdded(light);
					return light;
				}
			}
			light = new GameObject("LightningBoltLight").AddComponent<Light>();
			light.type = LightType.Point;
			goto IL_5B;
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x000AA41C File Offset: 0x000A861C
		private void UpdateLight(LightningLightParameters lp, List<Light> lights, float delay, float peakStart, float peakEnd, float lifeTime)
		{
			if (this.elapsedTime < delay)
			{
				return;
			}
			float num = (lifeTime - peakEnd) * lp.FadeOutMultiplier;
			float num2 = (peakEnd - peakStart) * lp.FadeFullyLitMultiplier;
			peakStart *= lp.FadeInMultiplier;
			peakEnd = peakStart + num2;
			lifeTime = peakEnd + num;
			float num3 = this.elapsedTime - delay;
			if (num3 >= peakStart)
			{
				if (num3 <= peakEnd)
				{
					using (List<Light>.Enumerator enumerator = lights.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Light light = enumerator.Current;
							light.intensity = lp.LightIntensity;
						}
						return;
					}
				}
				float t = (num3 - peakEnd) / (lifeTime - peakEnd);
				using (List<Light>.Enumerator enumerator = lights.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Light light2 = enumerator.Current;
						light2.intensity = Mathf.Lerp(lp.LightIntensity, 0f, t);
					}
					return;
				}
			}
			float t2 = num3 / peakStart;
			foreach (Light light3 in lights)
			{
				light3.intensity = Mathf.Lerp(0f, lp.LightIntensity, t2);
			}
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x000AA56C File Offset: 0x000A876C
		private void UpdateLights()
		{
			foreach (LightningBoltSegmentGroup lightningBoltSegmentGroup in this.segmentGroupsWithLight)
			{
				this.UpdateLight(lightningBoltSegmentGroup.LightParameters, lightningBoltSegmentGroup.Lights, lightningBoltSegmentGroup.Delay, lightningBoltSegmentGroup.PeakStart, lightningBoltSegmentGroup.PeakEnd, lightningBoltSegmentGroup.LifeTime);
			}
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00047A4C File Offset: 0x00045C4C
		private IEnumerator GenerateParticleCoRoutine(ParticleSystem p, Vector3 pos, float delay)
		{
			yield return new WaitForSecondsLightning(delay);
			p.transform.position = pos;
			if (p.emission.burstCount > 0)
			{
				ParticleSystem.Burst burst = p.emission.GetBurst(0);
				int num = UnityEngine.Random.Range((int)burst.minCount, (int)(burst.maxCount + 1));
				p.Emit(num);
			}
			else
			{
				ParticleSystem.MinMaxCurve rateOverTime = p.emission.rateOverTime;
				int num = (int)((rateOverTime.constantMax - rateOverTime.constantMin) * 0.5f);
				num = UnityEngine.Random.Range(num, num * 2);
				p.Emit(num);
			}
			yield break;
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x000AA5E4 File Offset: 0x000A87E4
		private void CheckForGlow(IEnumerable<LightningBoltParameters> parameters)
		{
			foreach (LightningBoltParameters lightningBoltParameters in parameters)
			{
				this.HasGlow = (lightningBoltParameters.GlowIntensity >= Mathf.Epsilon && lightningBoltParameters.GlowWidthMultiplier >= Mathf.Epsilon);
				if (this.HasGlow)
				{
					break;
				}
			}
		}

		// Token: 0x040007EF RID: 2031
		public static int MaximumLightCount = 128;

		// Token: 0x040007F0 RID: 2032
		public static int MaximumLightsPerBatch = 8;

		// Token: 0x040007F4 RID: 2036
		private DateTime startTimeOffset;

		// Token: 0x040007F5 RID: 2037
		private LightningBoltDependencies dependencies;

		// Token: 0x040007F6 RID: 2038
		private float elapsedTime;

		// Token: 0x040007F7 RID: 2039
		private float lifeTime;

		// Token: 0x040007F8 RID: 2040
		private float maxLifeTime;

		// Token: 0x040007F9 RID: 2041
		private bool hasLight;

		// Token: 0x040007FA RID: 2042
		private float timeSinceLevelLoad;

		// Token: 0x040007FB RID: 2043
		private readonly List<LightningBoltSegmentGroup> segmentGroups = new List<LightningBoltSegmentGroup>();

		// Token: 0x040007FC RID: 2044
		private readonly List<LightningBoltSegmentGroup> segmentGroupsWithLight = new List<LightningBoltSegmentGroup>();

		// Token: 0x040007FD RID: 2045
		private readonly List<LightningBolt.LineRendererMesh> activeLineRenderers = new List<LightningBolt.LineRendererMesh>();

		// Token: 0x040007FE RID: 2046
		private static int lightCount;

		// Token: 0x040007FF RID: 2047
		private static readonly List<LightningBolt.LineRendererMesh> lineRendererCache = new List<LightningBolt.LineRendererMesh>();

		// Token: 0x04000800 RID: 2048
		private static readonly List<LightningBoltSegmentGroup> groupCache = new List<LightningBoltSegmentGroup>();

		// Token: 0x04000801 RID: 2049
		private static readonly List<Light> lightCache = new List<Light>();

		// Token: 0x020000B4 RID: 180
		public class LineRendererMesh
		{
			// Token: 0x17000281 RID: 641
			// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00047A99 File Offset: 0x00045C99
			// (set) Token: 0x060006C2 RID: 1730 RVA: 0x00047AA1 File Offset: 0x00045CA1
			public GameObject GameObject { get; private set; }

			// Token: 0x17000282 RID: 642
			// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00047AAA File Offset: 0x00045CAA
			// (set) Token: 0x060006C4 RID: 1732 RVA: 0x00047AB7 File Offset: 0x00045CB7
			public Material MaterialGlow
			{
				get
				{
					return this.meshRendererGlow.sharedMaterial;
				}
				set
				{
					this.meshRendererGlow.sharedMaterial = value;
				}
			}

			// Token: 0x17000283 RID: 643
			// (get) Token: 0x060006C5 RID: 1733 RVA: 0x00047AC5 File Offset: 0x00045CC5
			// (set) Token: 0x060006C6 RID: 1734 RVA: 0x00047AD2 File Offset: 0x00045CD2
			public Material MaterialBolt
			{
				get
				{
					return this.meshRendererBolt.sharedMaterial;
				}
				set
				{
					this.meshRendererBolt.sharedMaterial = value;
				}
			}

			// Token: 0x17000284 RID: 644
			// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00047AE0 File Offset: 0x00045CE0
			public MeshRenderer MeshRendererGlow
			{
				get
				{
					return this.meshRendererGlow;
				}
			}

			// Token: 0x17000285 RID: 645
			// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00047AE8 File Offset: 0x00045CE8
			public MeshRenderer MeshRendererBolt
			{
				get
				{
					return this.meshRendererBolt;
				}
			}

			// Token: 0x17000286 RID: 646
			// (get) Token: 0x060006C9 RID: 1737 RVA: 0x00047AF0 File Offset: 0x00045CF0
			// (set) Token: 0x060006CA RID: 1738 RVA: 0x00047AF8 File Offset: 0x00045CF8
			public int Tag { get; set; }

			// Token: 0x17000287 RID: 647
			// (get) Token: 0x060006CB RID: 1739 RVA: 0x00047B01 File Offset: 0x00045D01
			// (set) Token: 0x060006CC RID: 1740 RVA: 0x00047B09 File Offset: 0x00045D09
			public Action<LightningCustomTransformStateInfo> CustomTransform { get; set; }

			// Token: 0x17000288 RID: 648
			// (get) Token: 0x060006CD RID: 1741 RVA: 0x00047B12 File Offset: 0x00045D12
			// (set) Token: 0x060006CE RID: 1742 RVA: 0x00047B1A File Offset: 0x00045D1A
			public Transform Transform { get; private set; }

			// Token: 0x17000289 RID: 649
			// (get) Token: 0x060006CF RID: 1743 RVA: 0x00047B23 File Offset: 0x00045D23
			public bool Empty
			{
				get
				{
					return this.vertices.Count == 0;
				}
			}

			// Token: 0x060006D0 RID: 1744 RVA: 0x000AA658 File Offset: 0x000A8858
			public LineRendererMesh()
			{
				this.GameObject = new GameObject("LightningBoltMeshRenderer");
				this.GameObject.SetActive(false);
				this.mesh = new Mesh
				{
					name = "ProceduralLightningMesh"
				};
				this.mesh.MarkDynamic();
				GameObject gameObject = new GameObject("LightningBoltMeshRendererGlow");
				gameObject.transform.parent = this.GameObject.transform;
				GameObject gameObject2 = new GameObject("LightningBoltMeshRendererBolt");
				gameObject2.transform.parent = this.GameObject.transform;
				this.meshFilterGlow = gameObject.AddComponent<MeshFilter>();
				this.meshFilterBolt = gameObject2.AddComponent<MeshFilter>();
				this.meshFilterGlow.sharedMesh = (this.meshFilterBolt.sharedMesh = this.mesh);
				this.meshRendererGlow = gameObject.AddComponent<MeshRenderer>();
				this.meshRendererBolt = gameObject2.AddComponent<MeshRenderer>();
				this.meshRendererGlow.shadowCastingMode = (this.meshRendererBolt.shadowCastingMode = ShadowCastingMode.Off);
				this.meshRendererGlow.reflectionProbeUsage = (this.meshRendererBolt.reflectionProbeUsage = ReflectionProbeUsage.Off);
				this.meshRendererGlow.lightProbeUsage = (this.meshRendererBolt.lightProbeUsage = LightProbeUsage.Off);
				this.meshRendererGlow.receiveShadows = (this.meshRendererBolt.receiveShadows = false);
				this.Transform = this.GameObject.GetComponent<Transform>();
			}

			// Token: 0x060006D1 RID: 1745 RVA: 0x00047B33 File Offset: 0x00045D33
			public void PopulateMesh()
			{
				if (this.vertices.Count == 0)
				{
					this.mesh.Clear();
					return;
				}
				this.PopulateMeshInternal();
			}

			// Token: 0x060006D2 RID: 1746 RVA: 0x000AA86C File Offset: 0x000A8A6C
			public bool PrepareForLines(int lineCount)
			{
				int num = lineCount * 4;
				return this.vertices.Count + num <= 64999;
			}

			// Token: 0x060006D3 RID: 1747 RVA: 0x000AA894 File Offset: 0x000A8A94
			public void BeginLine(Vector3 start, Vector3 end, float radius, Color32 color, float colorIntensity, Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
			{
				Vector4 vector = end - start;
				vector.w = radius;
				this.AppendLineInternal(ref start, ref end, ref vector, ref vector, ref vector, color, colorIntensity, ref fadeLifeTime, glowWidthModifier, glowIntensity);
			}

			// Token: 0x060006D4 RID: 1748 RVA: 0x000AA8D0 File Offset: 0x000A8AD0
			public void AppendLine(Vector3 start, Vector3 end, float radius, Color32 color, float colorIntensity, Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
			{
				Vector4 vector = end - start;
				vector.w = radius;
				Vector4 vector2 = this.lineDirs[this.lineDirs.Count - 3];
				Vector4 vector3 = this.lineDirs[this.lineDirs.Count - 1];
				this.AppendLineInternal(ref start, ref end, ref vector, ref vector2, ref vector3, color, colorIntensity, ref fadeLifeTime, glowWidthModifier, glowIntensity);
			}

			// Token: 0x060006D5 RID: 1749 RVA: 0x000AA940 File Offset: 0x000A8B40
			public void Reset()
			{
				this.CustomTransform = null;
				int tag = this.Tag;
				this.Tag = tag + 1;
				this.GameObject.SetActive(false);
				this.mesh.Clear();
				this.indices.Clear();
				this.vertices.Clear();
				this.colors.Clear();
				this.lineDirs.Clear();
				this.ends.Clear();
				this.texCoordsAndGlowModifiers.Clear();
				this.fadeLifetimes.Clear();
				this.currentBoundsMaxX = (this.currentBoundsMaxY = (this.currentBoundsMaxZ = -1147483648));
				this.currentBoundsMinX = (this.currentBoundsMinY = (this.currentBoundsMinZ = 1147483647));
			}

			// Token: 0x060006D6 RID: 1750 RVA: 0x000AAA04 File Offset: 0x000A8C04
			private void PopulateMeshInternal()
			{
				this.GameObject.SetActive(true);
				this.mesh.SetVertices(this.vertices);
				this.mesh.SetTangents(this.lineDirs);
				this.mesh.SetColors(this.colors);
				this.mesh.SetUVs(0, this.texCoordsAndGlowModifiers);
				this.mesh.SetUVs(1, this.fadeLifetimes);
				this.mesh.SetNormals(this.ends);
				this.mesh.SetTriangles(this.indices, 0);
				Bounds bounds = default(Bounds);
				Vector3 b = new Vector3((float)(this.currentBoundsMinX - 2), (float)(this.currentBoundsMinY - 2), (float)(this.currentBoundsMinZ - 2));
				Vector3 a = new Vector3((float)(this.currentBoundsMaxX + 2), (float)(this.currentBoundsMaxY + 2), (float)(this.currentBoundsMaxZ + 2));
				bounds.center = (a + b) * 0.5f;
				bounds.size = (a - b) * 1.2f;
				this.mesh.bounds = bounds;
			}

			// Token: 0x060006D7 RID: 1751 RVA: 0x000AAB20 File Offset: 0x000A8D20
			private void UpdateBounds(ref Vector3 point1, ref Vector3 point2)
			{
				int num = (int)point1.x - (int)point2.x;
				num &= num >> 31;
				int num2 = (int)point2.x + num;
				int num3 = (int)point1.x - num;
				num = this.currentBoundsMinX - num2;
				num &= num >> 31;
				this.currentBoundsMinX = num2 + num;
				num = this.currentBoundsMaxX - num3;
				num &= num >> 31;
				this.currentBoundsMaxX -= num;
				int num4 = (int)point1.y - (int)point2.y;
				num4 &= num4 >> 31;
				int num5 = (int)point2.y + num4;
				int num6 = (int)point1.y - num4;
				num4 = this.currentBoundsMinY - num5;
				num4 &= num4 >> 31;
				this.currentBoundsMinY = num5 + num4;
				num4 = this.currentBoundsMaxY - num6;
				num4 &= num4 >> 31;
				this.currentBoundsMaxY -= num4;
				int num7 = (int)point1.z - (int)point2.z;
				num7 &= num7 >> 31;
				int num8 = (int)point2.z + num7;
				int num9 = (int)point1.z - num7;
				num7 = this.currentBoundsMinZ - num8;
				num7 &= num7 >> 31;
				this.currentBoundsMinZ = num8 + num7;
				num7 = this.currentBoundsMaxZ - num9;
				num7 &= num7 >> 31;
				this.currentBoundsMaxZ -= num7;
			}

			// Token: 0x060006D8 RID: 1752 RVA: 0x000AAC70 File Offset: 0x000A8E70
			private void AddIndices()
			{
				int count = this.vertices.Count;
				this.indices.Add(count++);
				this.indices.Add(count++);
				this.indices.Add(count);
				this.indices.Add(count--);
				this.indices.Add(count);
				this.indices.Add(count + 2);
			}

			// Token: 0x060006D9 RID: 1753 RVA: 0x000AACE4 File Offset: 0x000A8EE4
			private void AppendLineInternal(ref Vector3 start, ref Vector3 end, ref Vector4 dir, ref Vector4 dirPrev1, ref Vector4 dirPrev2, Color32 color, float colorIntensity, ref Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
			{
				this.AddIndices();
				color.a = (byte)Mathf.Lerp(0f, 255f, colorIntensity * 0.1f);
				Vector4 item = new Vector4(LightningBolt.LineRendererMesh.uv1.x, LightningBolt.LineRendererMesh.uv1.y, glowWidthModifier, glowIntensity);
				this.vertices.Add(start);
				this.lineDirs.Add(dirPrev1);
				this.colors.Add(color);
				this.ends.Add(dir);
				this.vertices.Add(end);
				this.lineDirs.Add(dir);
				this.colors.Add(color);
				this.ends.Add(dir);
				dir.w = -dir.w;
				this.vertices.Add(start);
				this.lineDirs.Add(dirPrev2);
				this.colors.Add(color);
				this.ends.Add(dir);
				this.vertices.Add(end);
				this.lineDirs.Add(dir);
				this.colors.Add(color);
				this.ends.Add(dir);
				this.texCoordsAndGlowModifiers.Add(item);
				item.x = LightningBolt.LineRendererMesh.uv2.x;
				item.y = LightningBolt.LineRendererMesh.uv2.y;
				this.texCoordsAndGlowModifiers.Add(item);
				item.x = LightningBolt.LineRendererMesh.uv3.x;
				item.y = LightningBolt.LineRendererMesh.uv3.y;
				this.texCoordsAndGlowModifiers.Add(item);
				item.x = LightningBolt.LineRendererMesh.uv4.x;
				item.y = LightningBolt.LineRendererMesh.uv4.y;
				this.texCoordsAndGlowModifiers.Add(item);
				this.fadeLifetimes.Add(fadeLifeTime);
				this.fadeLifetimes.Add(fadeLifeTime);
				this.fadeLifetimes.Add(fadeLifeTime);
				this.fadeLifetimes.Add(fadeLifeTime);
				this.UpdateBounds(ref start, ref end);
			}

			// Token: 0x04000806 RID: 2054
			private const int defaultListCapacity = 2048;

			// Token: 0x04000807 RID: 2055
			private static readonly Vector2 uv1 = new Vector2(0f, 0f);

			// Token: 0x04000808 RID: 2056
			private static readonly Vector2 uv2 = new Vector2(1f, 0f);

			// Token: 0x04000809 RID: 2057
			private static readonly Vector2 uv3 = new Vector2(0f, 1f);

			// Token: 0x0400080A RID: 2058
			private static readonly Vector2 uv4 = new Vector2(1f, 1f);

			// Token: 0x0400080B RID: 2059
			private readonly List<int> indices = new List<int>(2048);

			// Token: 0x0400080C RID: 2060
			private readonly List<Vector3> vertices = new List<Vector3>(2048);

			// Token: 0x0400080D RID: 2061
			private readonly List<Vector4> lineDirs = new List<Vector4>(2048);

			// Token: 0x0400080E RID: 2062
			private readonly List<Color32> colors = new List<Color32>(2048);

			// Token: 0x0400080F RID: 2063
			private readonly List<Vector3> ends = new List<Vector3>(2048);

			// Token: 0x04000810 RID: 2064
			private readonly List<Vector4> texCoordsAndGlowModifiers = new List<Vector4>(2048);

			// Token: 0x04000811 RID: 2065
			private readonly List<Vector4> fadeLifetimes = new List<Vector4>(2048);

			// Token: 0x04000812 RID: 2066
			private const int boundsPadder = 1000000000;

			// Token: 0x04000813 RID: 2067
			private int currentBoundsMinX = 1147483647;

			// Token: 0x04000814 RID: 2068
			private int currentBoundsMinY = 1147483647;

			// Token: 0x04000815 RID: 2069
			private int currentBoundsMinZ = 1147483647;

			// Token: 0x04000816 RID: 2070
			private int currentBoundsMaxX = -1147483648;

			// Token: 0x04000817 RID: 2071
			private int currentBoundsMaxY = -1147483648;

			// Token: 0x04000818 RID: 2072
			private int currentBoundsMaxZ = -1147483648;

			// Token: 0x04000819 RID: 2073
			private Mesh mesh;

			// Token: 0x0400081A RID: 2074
			private MeshFilter meshFilterGlow;

			// Token: 0x0400081B RID: 2075
			private MeshFilter meshFilterBolt;

			// Token: 0x0400081C RID: 2076
			private MeshRenderer meshRendererGlow;

			// Token: 0x0400081D RID: 2077
			private MeshRenderer meshRendererBolt;
		}
	}
}
