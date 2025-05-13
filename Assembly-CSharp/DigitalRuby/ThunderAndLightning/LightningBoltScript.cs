using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C2 RID: 194
	public class LightningBoltScript : MonoBehaviour
	{
		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x00047D46 File Offset: 0x00045F46
		public Camera Camera
		{
			get
			{
				return ClientGameManager.MainCamera;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000715 RID: 1813 RVA: 0x00047D4D File Offset: 0x00045F4D
		// (set) Token: 0x06000716 RID: 1814 RVA: 0x00047D55 File Offset: 0x00045F55
		public Action<LightningBoltParameters, Vector3, Vector3> LightningStartedCallback { get; set; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x00047D5E File Offset: 0x00045F5E
		// (set) Token: 0x06000718 RID: 1816 RVA: 0x00047D66 File Offset: 0x00045F66
		public Action<LightningBoltParameters, Vector3, Vector3> LightningEndedCallback { get; set; }

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x00047D6F File Offset: 0x00045F6F
		// (set) Token: 0x0600071A RID: 1818 RVA: 0x00047D77 File Offset: 0x00045F77
		public Action<Light> LightAddedCallback { get; set; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x00047D80 File Offset: 0x00045F80
		// (set) Token: 0x0600071C RID: 1820 RVA: 0x00047D88 File Offset: 0x00045F88
		public Action<Light> LightRemovedCallback { get; set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x00047D91 File Offset: 0x00045F91
		public bool HasActiveBolts
		{
			get
			{
				return this.activeBolts.Count != 0;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x00047DA1 File Offset: 0x00045FA1
		// (set) Token: 0x0600071F RID: 1823 RVA: 0x00047DA8 File Offset: 0x00045FA8
		public static Vector4 TimeVectorSinceStart { get; private set; }

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000720 RID: 1824 RVA: 0x00047DB0 File Offset: 0x00045FB0
		// (set) Token: 0x06000721 RID: 1825 RVA: 0x00047DB7 File Offset: 0x00045FB7
		public static float TimeSinceStart { get; private set; }

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000722 RID: 1826 RVA: 0x00047DBF File Offset: 0x00045FBF
		// (set) Token: 0x06000723 RID: 1827 RVA: 0x00047DC6 File Offset: 0x00045FC6
		public static float DeltaTime { get; private set; }

		// Token: 0x06000724 RID: 1828 RVA: 0x000ABE48 File Offset: 0x000AA048
		public virtual void CreateLightningBolt(LightningBoltParameters p)
		{
			if (p != null && this.Camera != null)
			{
				this.UpdateTexture();
				this.oneParameterArray[0] = p;
				LightningBolt orCreateLightningBolt = this.GetOrCreateLightningBolt();
				LightningBoltDependencies dependencies = this.CreateLightningBoltDependencies(this.oneParameterArray);
				orCreateLightningBolt.SetupLightningBolt(dependencies);
			}
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x000ABE90 File Offset: 0x000AA090
		public void CreateLightningBolts(ICollection<LightningBoltParameters> parameters)
		{
			if (parameters != null && parameters.Count != 0 && this.Camera != null)
			{
				this.UpdateTexture();
				LightningBolt orCreateLightningBolt = this.GetOrCreateLightningBolt();
				LightningBoltDependencies dependencies = this.CreateLightningBoltDependencies(parameters);
				orCreateLightningBolt.SetupLightningBolt(dependencies);
			}
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00047DCE File Offset: 0x00045FCE
		protected virtual void Awake()
		{
			this.UpdateShaderIds();
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00047DD6 File Offset: 0x00045FD6
		protected virtual void Start()
		{
			this.UpdateCamera();
			this.UpdateMaterialsForLastTexture();
			this.UpdateShaderParameters();
			this.CheckCompensateForParentTransform();
			SceneManager.sceneLoaded += this.OnSceneLoaded;
			if (this.MultiThreaded)
			{
				this.threadState = new LightningThreadState();
			}
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x000ABED0 File Offset: 0x000AA0D0
		protected virtual void Update()
		{
			if (SceneCompositionManager.IsLoading)
			{
				return;
			}
			if (Time.timeScale <= 0f)
			{
				return;
			}
			if (LightningBoltScript.needsTimeUpdate)
			{
				LightningBoltScript.needsTimeUpdate = false;
				LightningBoltScript.DeltaTime = (this.UseGameTime ? Time.deltaTime : Time.unscaledDeltaTime) * LightningBoltScript.TimeScale;
				LightningBoltScript.TimeSinceStart += LightningBoltScript.DeltaTime;
			}
			if (this.HasActiveBolts)
			{
				this.UpdateCamera();
				this.UpdateShaderParameters();
				this.CheckCompensateForParentTransform();
				this.UpdateActiveBolts();
				Shader.SetGlobalVector(LightningBoltScript.shaderId_LightningTime, LightningBoltScript.TimeVectorSinceStart = new Vector4(LightningBoltScript.TimeSinceStart * 0.05f, LightningBoltScript.TimeSinceStart, LightningBoltScript.TimeSinceStart * 2f, LightningBoltScript.TimeSinceStart * 3f));
			}
			if (this.threadState != null)
			{
				this.threadState.UpdateMainThreadActions();
			}
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x00047E14 File Offset: 0x00046014
		protected virtual void LateUpdate()
		{
			LightningBoltScript.needsTimeUpdate = true;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x00047E1C File Offset: 0x0004601C
		protected virtual LightningBoltParameters OnCreateParameters()
		{
			return LightningBoltParameters.GetOrCreateParameters();
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x000ABF9C File Offset: 0x000AA19C
		protected LightningBoltParameters CreateParameters()
		{
			LightningBoltParameters lightningBoltParameters = this.OnCreateParameters();
			lightningBoltParameters.quality = this.QualitySetting;
			this.PopulateParameters(lightningBoltParameters);
			return lightningBoltParameters;
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00047E23 File Offset: 0x00046023
		protected virtual void PopulateParameters(LightningBoltParameters parameters)
		{
			parameters.MainTrunkTintColor = this.MainTrunkTintColor;
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x00047E36 File Offset: 0x00046036
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x00047E3E File Offset: 0x0004603E
		internal Material lightningMaterialMeshInternal { get; private set; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x00047E47 File Offset: 0x00046047
		// (set) Token: 0x06000730 RID: 1840 RVA: 0x00047E4F File Offset: 0x0004604F
		internal Material lightningMaterialMeshNoGlowInternal { get; private set; }

		// Token: 0x06000731 RID: 1841 RVA: 0x00047E58 File Offset: 0x00046058
		private Coroutine StartCoroutineWrapper(IEnumerator routine)
		{
			if (base.isActiveAndEnabled)
			{
				return base.StartCoroutine(routine);
			}
			return null;
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00047E6B File Offset: 0x0004606B
		private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			LightningBolt.ClearCache();
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x000ABFC4 File Offset: 0x000AA1C4
		private LightningBoltDependencies CreateLightningBoltDependencies(ICollection<LightningBoltParameters> parameters)
		{
			LightningBoltDependencies lightningBoltDependencies;
			if (this.dependenciesCache.Count == 0)
			{
				lightningBoltDependencies = new LightningBoltDependencies();
				lightningBoltDependencies.AddActiveBolt = new Action<LightningBolt>(this.AddActiveBolt);
				lightningBoltDependencies.LightAdded = new Action<Light>(this.OnLightAdded);
				lightningBoltDependencies.LightRemoved = new Action<Light>(this.OnLightRemoved);
				lightningBoltDependencies.ReturnToCache = new Action<LightningBoltDependencies>(this.ReturnLightningDependenciesToCache);
				lightningBoltDependencies.StartCoroutine = new Func<IEnumerator, Coroutine>(this.StartCoroutineWrapper);
				lightningBoltDependencies.Parent = base.gameObject;
			}
			else
			{
				int index = this.dependenciesCache.Count - 1;
				lightningBoltDependencies = this.dependenciesCache[index];
				this.dependenciesCache.RemoveAt(index);
			}
			lightningBoltDependencies.CameraPos = this.Camera.transform.position;
			lightningBoltDependencies.CameraIsOrthographic = this.Camera.orthographic;
			lightningBoltDependencies.CameraMode = this.calculatedCameraMode;
			lightningBoltDependencies.LevelOfDetailDistance = this.LevelOfDetailDistance;
			lightningBoltDependencies.DestParticleSystem = this.LightningDestinationParticleSystem;
			lightningBoltDependencies.LightningMaterialMesh = this.lightningMaterialMeshInternal;
			lightningBoltDependencies.LightningMaterialMeshNoGlow = this.lightningMaterialMeshNoGlowInternal;
			lightningBoltDependencies.OriginParticleSystem = this.LightningOriginParticleSystem;
			lightningBoltDependencies.SortLayerName = this.SortLayerName;
			lightningBoltDependencies.SortOrderInLayer = this.SortOrderInLayer;
			lightningBoltDependencies.UseWorldSpace = this.UseWorldSpace;
			lightningBoltDependencies.ThreadState = this.threadState;
			if (this.threadState != null)
			{
				lightningBoltDependencies.Parameters = new List<LightningBoltParameters>(parameters);
			}
			else
			{
				lightningBoltDependencies.Parameters = parameters;
			}
			lightningBoltDependencies.LightningBoltStarted = this.LightningStartedCallback;
			lightningBoltDependencies.LightningBoltEnded = this.LightningEndedCallback;
			return lightningBoltDependencies;
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00047E72 File Offset: 0x00046072
		private void ReturnLightningDependenciesToCache(LightningBoltDependencies d)
		{
			d.Parameters = null;
			d.OriginParticleSystem = null;
			d.DestParticleSystem = null;
			d.LightningMaterialMesh = null;
			d.LightningMaterialMeshNoGlow = null;
			this.dependenciesCache.Add(d);
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00047EA3 File Offset: 0x000460A3
		internal void OnLightAdded(Light l)
		{
			if (this.LightAddedCallback != null)
			{
				this.LightAddedCallback(l);
			}
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00047EB9 File Offset: 0x000460B9
		internal void OnLightRemoved(Light l)
		{
			if (this.LightRemovedCallback != null)
			{
				this.LightRemovedCallback(l);
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00047ECF File Offset: 0x000460CF
		internal void AddActiveBolt(LightningBolt bolt)
		{
			this.activeBolts.Add(bolt);
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x000AC148 File Offset: 0x000AA348
		private void UpdateShaderIds()
		{
			if (LightningBoltScript.shaderId_MainTex != -2147483648)
			{
				return;
			}
			LightningBoltScript.shaderId_MainTex = Shader.PropertyToID("_MainTex");
			LightningBoltScript.shaderId_TintColor = Shader.PropertyToID("_TintColor");
			LightningBoltScript.shaderId_JitterMultiplier = Shader.PropertyToID("_JitterMultiplier");
			LightningBoltScript.shaderId_Turbulence = Shader.PropertyToID("_Turbulence");
			LightningBoltScript.shaderId_TurbulenceVelocity = Shader.PropertyToID("_TurbulenceVelocity");
			LightningBoltScript.shaderId_SrcBlendMode = Shader.PropertyToID("_SrcBlendMode");
			LightningBoltScript.shaderId_DstBlendMode = Shader.PropertyToID("_DstBlendMode");
			LightningBoltScript.shaderId_InvFade = Shader.PropertyToID("_InvFade");
			LightningBoltScript.shaderId_LightningTime = Shader.PropertyToID("_LightningTime");
			LightningBoltScript.shaderId_IntensityFlicker = Shader.PropertyToID("_IntensityFlicker");
			LightningBoltScript.shaderId_IntensityFlickerTexture = Shader.PropertyToID("_IntensityFlickerTexture");
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x000AC208 File Offset: 0x000AA408
		private void UpdateMaterialsForLastTexture()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.calculatedCameraMode = CameraMode.Unknown;
			this.lightningMaterialMeshInternal = new Material(this.LightningMaterialMesh);
			this.lightningMaterialMeshNoGlowInternal = new Material(this.LightningMaterialMeshNoGlow);
			if (this.LightningTexture != null)
			{
				this.lightningMaterialMeshNoGlowInternal.SetTexture(LightningBoltScript.shaderId_MainTex, this.LightningTexture);
			}
			if (this.LightningGlowTexture != null)
			{
				this.lightningMaterialMeshInternal.SetTexture(LightningBoltScript.shaderId_MainTex, this.LightningGlowTexture);
			}
			this.SetupMaterialCamera();
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x000AC294 File Offset: 0x000AA494
		private void UpdateTexture()
		{
			if (this.LightningTexture != null && this.LightningTexture != this.lastLightningTexture)
			{
				this.lastLightningTexture = this.LightningTexture;
				this.UpdateMaterialsForLastTexture();
			}
			if (this.LightningGlowTexture != null && this.LightningGlowTexture != this.lastLightningGlowTexture)
			{
				this.lastLightningGlowTexture = this.LightningGlowTexture;
				this.UpdateMaterialsForLastTexture();
			}
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x000AC308 File Offset: 0x000AA508
		private void SetMaterialPerspective()
		{
			if (this.calculatedCameraMode != CameraMode.Perspective)
			{
				this.calculatedCameraMode = CameraMode.Perspective;
				this.lightningMaterialMeshInternal.EnableKeyword("PERSPECTIVE");
				this.lightningMaterialMeshNoGlowInternal.EnableKeyword("PERSPECTIVE");
				this.lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				this.lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
			}
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x000AC388 File Offset: 0x000AA588
		private void SetMaterialOrthographicXY()
		{
			if (this.calculatedCameraMode != CameraMode.OrthographicXY)
			{
				this.calculatedCameraMode = CameraMode.OrthographicXY;
				this.lightningMaterialMeshInternal.EnableKeyword("ORTHOGRAPHIC_XY");
				this.lightningMaterialMeshNoGlowInternal.EnableKeyword("ORTHOGRAPHIC_XY");
				this.lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XZ");
				this.lightningMaterialMeshInternal.DisableKeyword("PERSPECTIVE");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("PERSPECTIVE");
			}
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x000AC408 File Offset: 0x000AA608
		private void SetMaterialOrthographicXZ()
		{
			if (this.calculatedCameraMode != CameraMode.OrthographicXZ)
			{
				this.calculatedCameraMode = CameraMode.OrthographicXZ;
				this.lightningMaterialMeshInternal.EnableKeyword("ORTHOGRAPHIC_XZ");
				this.lightningMaterialMeshNoGlowInternal.EnableKeyword("ORTHOGRAPHIC_XZ");
				this.lightningMaterialMeshInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("ORTHOGRAPHIC_XY");
				this.lightningMaterialMeshInternal.DisableKeyword("PERSPECTIVE");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("PERSPECTIVE");
			}
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x000AC488 File Offset: 0x000AA688
		private void SetupMaterialCamera()
		{
			if (this.Camera == null && this.CameraMode == CameraMode.Auto)
			{
				this.SetMaterialPerspective();
				return;
			}
			if (this.CameraMode == CameraMode.Auto)
			{
				if (this.Camera.orthographic)
				{
					this.SetMaterialOrthographicXY();
					return;
				}
				this.SetMaterialPerspective();
				return;
			}
			else
			{
				if (this.CameraMode == CameraMode.Perspective)
				{
					this.SetMaterialPerspective();
					return;
				}
				if (this.CameraMode == CameraMode.OrthographicXY)
				{
					this.SetMaterialOrthographicXY();
					return;
				}
				this.SetMaterialOrthographicXZ();
				return;
			}
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00047EDD File Offset: 0x000460DD
		private void EnableKeyword(string keyword, bool enable, Material m)
		{
			if (enable)
			{
				m.EnableKeyword(keyword);
				return;
			}
			m.DisableKeyword(keyword);
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x000AC4FC File Offset: 0x000AA6FC
		private void UpdateShaderParameters()
		{
			this.lightningMaterialMeshInternal.SetColor(LightningBoltScript.shaderId_TintColor, this.GlowTintColor);
			this.lightningMaterialMeshInternal.SetFloat(LightningBoltScript.shaderId_JitterMultiplier, this.JitterMultiplier);
			this.lightningMaterialMeshInternal.SetFloat(LightningBoltScript.shaderId_Turbulence, this.Turbulence * LightningBoltParameters.Scale);
			this.lightningMaterialMeshInternal.SetVector(LightningBoltScript.shaderId_TurbulenceVelocity, this.TurbulenceVelocity * LightningBoltParameters.Scale);
			this.lightningMaterialMeshInternal.SetInt(LightningBoltScript.shaderId_SrcBlendMode, (int)this.SourceBlendMode);
			this.lightningMaterialMeshInternal.SetInt(LightningBoltScript.shaderId_DstBlendMode, (int)this.DestinationBlendMode);
			this.lightningMaterialMeshInternal.renderQueue = this.RenderQueue;
			this.lightningMaterialMeshInternal.SetFloat(LightningBoltScript.shaderId_InvFade, this.SoftParticlesFactor);
			this.lightningMaterialMeshNoGlowInternal.SetColor(LightningBoltScript.shaderId_TintColor, this.LightningTintColor);
			this.lightningMaterialMeshNoGlowInternal.SetFloat(LightningBoltScript.shaderId_JitterMultiplier, this.JitterMultiplier);
			this.lightningMaterialMeshNoGlowInternal.SetFloat(LightningBoltScript.shaderId_Turbulence, this.Turbulence * LightningBoltParameters.Scale);
			this.lightningMaterialMeshNoGlowInternal.SetVector(LightningBoltScript.shaderId_TurbulenceVelocity, this.TurbulenceVelocity * LightningBoltParameters.Scale);
			this.lightningMaterialMeshNoGlowInternal.SetInt(LightningBoltScript.shaderId_SrcBlendMode, (int)this.SourceBlendMode);
			this.lightningMaterialMeshNoGlowInternal.SetInt(LightningBoltScript.shaderId_DstBlendMode, (int)this.DestinationBlendMode);
			this.lightningMaterialMeshNoGlowInternal.renderQueue = this.RenderQueue;
			this.lightningMaterialMeshNoGlowInternal.SetFloat(LightningBoltScript.shaderId_InvFade, this.SoftParticlesFactor);
			if (this.IntensityFlicker != LightningBoltScript.intensityFlickerDefault && this.IntensityFlickerTexture != null)
			{
				this.lightningMaterialMeshInternal.SetVector(LightningBoltScript.shaderId_IntensityFlicker, this.IntensityFlicker);
				this.lightningMaterialMeshInternal.SetTexture(LightningBoltScript.shaderId_IntensityFlickerTexture, this.IntensityFlickerTexture);
				this.lightningMaterialMeshNoGlowInternal.SetVector(LightningBoltScript.shaderId_IntensityFlicker, this.IntensityFlicker);
				this.lightningMaterialMeshNoGlowInternal.SetTexture(LightningBoltScript.shaderId_IntensityFlickerTexture, this.IntensityFlickerTexture);
				this.lightningMaterialMeshInternal.EnableKeyword("INTENSITY_FLICKER");
				this.lightningMaterialMeshNoGlowInternal.EnableKeyword("INTENSITY_FLICKER");
			}
			else
			{
				this.lightningMaterialMeshInternal.DisableKeyword("INTENSITY_FLICKER");
				this.lightningMaterialMeshNoGlowInternal.DisableKeyword("INTENSITY_FLICKER");
			}
			this.SetupMaterialCamera();
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x000AC74C File Offset: 0x000AA94C
		private void CheckCompensateForParentTransform()
		{
			if (this.CompensateForParentTransform)
			{
				Transform parent = base.transform.parent;
				if (parent != null)
				{
					base.transform.position = parent.position;
					base.transform.localScale = new Vector3(1f / parent.localScale.x, 1f / parent.localScale.y, 1f / parent.localScale.z);
					base.transform.rotation = parent.rotation;
				}
			}
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0004475B File Offset: 0x0004295B
		private void UpdateCamera()
		{
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x000AC7DC File Offset: 0x000AA9DC
		private LightningBolt GetOrCreateLightningBolt()
		{
			if (this.lightningBoltCache.Count == 0)
			{
				return new LightningBolt();
			}
			LightningBolt result = this.lightningBoltCache[this.lightningBoltCache.Count - 1];
			this.lightningBoltCache.RemoveAt(this.lightningBoltCache.Count - 1);
			return result;
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x000AC82C File Offset: 0x000AAA2C
		private void UpdateActiveBolts()
		{
			for (int i = this.activeBolts.Count - 1; i >= 0; i--)
			{
				LightningBolt lightningBolt = this.activeBolts[i];
				if (!lightningBolt.Update())
				{
					this.activeBolts.RemoveAt(i);
					lightningBolt.Cleanup();
					this.lightningBoltCache.Add(lightningBolt);
				}
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x00047EF1 File Offset: 0x000460F1
		private void OnApplicationQuit()
		{
			if (this.threadState != null)
			{
				this.threadState.Running = false;
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x000AC884 File Offset: 0x000AAA84
		private void Cleanup()
		{
			foreach (LightningBolt lightningBolt in this.activeBolts)
			{
				lightningBolt.Cleanup();
			}
			this.activeBolts.Clear();
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x000AC8E0 File Offset: 0x000AAAE0
		private void OnDestroy()
		{
			if (this.threadState != null)
			{
				this.threadState.TerminateAndWaitForEnd(true);
			}
			if (this.lightningMaterialMeshInternal != null)
			{
				UnityEngine.Object.Destroy(this.lightningMaterialMeshInternal);
			}
			if (this.lightningMaterialMeshNoGlowInternal != null)
			{
				UnityEngine.Object.Destroy(this.lightningMaterialMeshNoGlowInternal);
			}
			this.Cleanup();
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00047F07 File Offset: 0x00046107
		private void OnDisable()
		{
			this.Cleanup();
		}

		// Token: 0x04000873 RID: 2163
		[Header("Lightning General Properties")]
		[Tooltip("The camera the lightning should be shown in. Defaults to the current camera, or the main camera if current camera is null. If you are using a different camera, you may want to put the lightning in it's own layer and cull that layer out of any other cameras.")]
		[FormerlySerializedAs("Camera")]
		[SerializeField]
		private Camera m_camera;

		// Token: 0x04000874 RID: 2164
		[Tooltip("Type of camera mode. Auto detects the camera and creates appropriate lightning. Can be overriden to do something more specific regardless of camera.")]
		public CameraMode CameraMode;

		// Token: 0x04000875 RID: 2165
		internal CameraMode calculatedCameraMode = CameraMode.Unknown;

		// Token: 0x04000876 RID: 2166
		[Tooltip("True if you are using world space coordinates for the lightning bolt, false if you are using coordinates relative to the parent game object.")]
		public bool UseWorldSpace = true;

		// Token: 0x04000877 RID: 2167
		[Tooltip("Whether to compensate for the parent transform. Default is false. If true, rotation, scale and position are altered by the parent transform. Use this to fix scaling, rotation and other offset problems with the lightning.")]
		public bool CompensateForParentTransform;

		// Token: 0x04000878 RID: 2168
		[Tooltip("Lightning quality setting. This allows setting limits on generations, lights and shadow casting lights based on the global quality setting.")]
		public LightningBoltQualitySetting QualitySetting;

		// Token: 0x04000879 RID: 2169
		[Tooltip("Whether to use multi-threaded generation of lightning. Lightning will be delayed by about 1 frame if this is turned on, but this can significantly improve performance.")]
		public bool MultiThreaded;

		// Token: 0x0400087A RID: 2170
		[Range(0f, 1000f)]
		[Tooltip("If non-zero, the Camera property is used to get distance of lightning from camera. Lightning generations is reduced for each distance from camera. For example, if LevelOfDetailDistance was 100 and the lightning was 200 away from camera, generations would be reduced by 2, to a minimum of 1.")]
		public float LevelOfDetailDistance;

		// Token: 0x0400087B RID: 2171
		[Tooltip("True to use game time, false to use real time")]
		public bool UseGameTime;

		// Token: 0x0400087C RID: 2172
		[Header("Lightning 2D Settings")]
		[Tooltip("Sort layer name")]
		public string SortLayerName;

		// Token: 0x0400087D RID: 2173
		[Tooltip("Order in sort layer")]
		public int SortOrderInLayer;

		// Token: 0x0400087E RID: 2174
		[Header("Lightning Rendering Properties")]
		[Tooltip("Soft particles factor. 0.01 to 3.0 are typical, 100.0 to disable.")]
		[Range(0.01f, 100f)]
		public float SoftParticlesFactor = 3f;

		// Token: 0x0400087F RID: 2175
		[Tooltip("The render queue for the lightning. -1 for default.")]
		public int RenderQueue = -1;

		// Token: 0x04000880 RID: 2176
		[Tooltip("Lightning material for mesh renderer - glow")]
		public Material LightningMaterialMesh;

		// Token: 0x04000881 RID: 2177
		[Tooltip("Lightning material for mesh renderer - bolt")]
		public Material LightningMaterialMeshNoGlow;

		// Token: 0x04000882 RID: 2178
		[Tooltip("The texture to use for the lightning bolts, or null for the material default texture.")]
		public Texture2D LightningTexture;

		// Token: 0x04000883 RID: 2179
		[Tooltip("The texture to use for the lightning glow, or null for the material default texture.")]
		public Texture2D LightningGlowTexture;

		// Token: 0x04000884 RID: 2180
		[Tooltip("Particle system to play at the point of emission (start). 'Emission rate' particles will be emitted all at once.")]
		public ParticleSystem LightningOriginParticleSystem;

		// Token: 0x04000885 RID: 2181
		[Tooltip("Particle system to play at the point of impact (end). 'Emission rate' particles will be emitted all at once.")]
		public ParticleSystem LightningDestinationParticleSystem;

		// Token: 0x04000886 RID: 2182
		[Tooltip("Tint color for the lightning")]
		public Color LightningTintColor = Color.white;

		// Token: 0x04000887 RID: 2183
		[Tooltip("Tint color for the lightning glow")]
		public Color GlowTintColor = new Color(0.1f, 0.2f, 1f, 1f);

		// Token: 0x04000888 RID: 2184
		[Tooltip("Allow tintint the main trunk differently than forks.")]
		public Color MainTrunkTintColor = new Color(1f, 1f, 1f, 1f);

		// Token: 0x04000889 RID: 2185
		[Tooltip("Source blend mode. Default is SrcAlpha.")]
		public BlendMode SourceBlendMode = BlendMode.SrcAlpha;

		// Token: 0x0400088A RID: 2186
		[Tooltip("Destination blend mode. Default is One. For additive blend use One. For alpha blend use OneMinusSrcAlpha.")]
		public BlendMode DestinationBlendMode = BlendMode.One;

		// Token: 0x0400088B RID: 2187
		[Header("Lightning Movement Properties")]
		[Tooltip("Jitter multiplier to randomize lightning size. Jitter depends on trunk width and will make the lightning move rapidly and jaggedly, giving a more lively and sometimes cartoony feel. Jitter may be shared with other bolts depending on materials. If you need different jitters for the same material, create a second script object.")]
		public float JitterMultiplier;

		// Token: 0x0400088C RID: 2188
		[Tooltip("Built in turbulance based on the direction of each segment. Small values usually work better, like 0.2.")]
		public float Turbulence;

		// Token: 0x0400088D RID: 2189
		[Tooltip("Global turbulence velocity for this script")]
		public Vector3 TurbulenceVelocity = Vector3.zero;

		// Token: 0x0400088E RID: 2190
		[Tooltip("Cause lightning to flicker, x = min, y = max, z = time multiplier, w = add to intensity")]
		public Vector4 IntensityFlicker = LightningBoltScript.intensityFlickerDefault;

		// Token: 0x0400088F RID: 2191
		private static readonly Vector4 intensityFlickerDefault = new Vector4(1f, 1f, 1f, 0f);

		// Token: 0x04000890 RID: 2192
		[Tooltip("Lightning intensity flicker lookup texture")]
		public Texture2D IntensityFlickerTexture;

		// Token: 0x04000898 RID: 2200
		public static float TimeScale = 1f;

		// Token: 0x04000899 RID: 2201
		private static bool needsTimeUpdate = true;

		// Token: 0x0400089C RID: 2204
		private Texture2D lastLightningTexture;

		// Token: 0x0400089D RID: 2205
		private Texture2D lastLightningGlowTexture;

		// Token: 0x0400089E RID: 2206
		private readonly List<LightningBolt> activeBolts = new List<LightningBolt>();

		// Token: 0x0400089F RID: 2207
		private readonly LightningBoltParameters[] oneParameterArray = new LightningBoltParameters[1];

		// Token: 0x040008A0 RID: 2208
		private readonly List<LightningBolt> lightningBoltCache = new List<LightningBolt>();

		// Token: 0x040008A1 RID: 2209
		private readonly List<LightningBoltDependencies> dependenciesCache = new List<LightningBoltDependencies>();

		// Token: 0x040008A2 RID: 2210
		private LightningThreadState threadState;

		// Token: 0x040008A3 RID: 2211
		private static int shaderId_MainTex = int.MinValue;

		// Token: 0x040008A4 RID: 2212
		private static int shaderId_TintColor;

		// Token: 0x040008A5 RID: 2213
		private static int shaderId_JitterMultiplier;

		// Token: 0x040008A6 RID: 2214
		private static int shaderId_Turbulence;

		// Token: 0x040008A7 RID: 2215
		private static int shaderId_TurbulenceVelocity;

		// Token: 0x040008A8 RID: 2216
		private static int shaderId_SrcBlendMode;

		// Token: 0x040008A9 RID: 2217
		private static int shaderId_DstBlendMode;

		// Token: 0x040008AA RID: 2218
		private static int shaderId_InvFade;

		// Token: 0x040008AB RID: 2219
		private static int shaderId_LightningTime;

		// Token: 0x040008AC RID: 2220
		private static int shaderId_IntensityFlicker;

		// Token: 0x040008AD RID: 2221
		private static int shaderId_IntensityFlickerTexture;
	}
}
