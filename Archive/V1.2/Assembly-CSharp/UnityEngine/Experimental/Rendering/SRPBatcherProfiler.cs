using System;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	// Token: 0x0200020D RID: 525
	public class SRPBatcherProfiler : MonoBehaviour
	{
		// Token: 0x060011AD RID: 4525 RVA: 0x000E3FE0 File Offset: 0x000E21E0
		private void Awake()
		{
			for (int i = 0; i < this.recordersList.Length; i++)
			{
				Sampler sampler = Sampler.Get(this.recordersList[i].name);
				if (sampler.isValid)
				{
					this.recordersList[i].recorder = sampler.GetRecorder();
				}
				else if (this.recordersList[i].oldName != null)
				{
					sampler = Sampler.Get(this.recordersList[i].oldName);
					if (sampler.isValid)
					{
						this.recordersList[i].recorder = sampler.GetRecorder();
					}
				}
			}
			this.m_style = new GUIStyle();
			this.m_style.fontSize = 15;
			this.m_style.normal.textColor = Color.white;
			this.m_oldBatcherEnable = this.m_Enable;
			this.ResetStats();
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x000E40AC File Offset: 0x000E22AC
		private void RazCounters()
		{
			this.m_AccDeltaTime = 0f;
			this.m_frameCount = 0;
			for (int i = 0; i < this.recordersList.Length; i++)
			{
				this.recordersList[i].accTime = 0f;
				this.recordersList[i].callCount = 0;
			}
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0004EAD2 File Offset: 0x0004CCD2
		private void ResetStats()
		{
			this.m_statsLabel = "Gathering data...";
			this.RazCounters();
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0004EAE5 File Offset: 0x0004CCE5
		private void ToggleStats()
		{
			this.m_Enable = !this.m_Enable;
			this.ResetStats();
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x000E4100 File Offset: 0x000E2300
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F9))
			{
				GraphicsSettings.useScriptableRenderPipelineBatching = !GraphicsSettings.useScriptableRenderPipelineBatching;
			}
			if (GraphicsSettings.useScriptableRenderPipelineBatching != this.m_oldBatcherEnable)
			{
				this.ResetStats();
				this.m_oldBatcherEnable = GraphicsSettings.useScriptableRenderPipelineBatching;
			}
			if (Input.GetKeyDown(KeyCode.F8))
			{
				this.ToggleStats();
			}
			if (this.m_Enable)
			{
				bool useScriptableRenderPipelineBatching = GraphicsSettings.useScriptableRenderPipelineBatching;
				this.m_AccDeltaTime += Time.unscaledDeltaTime;
				this.m_frameCount++;
				for (int i = 0; i < this.recordersList.Length; i++)
				{
					if (this.recordersList[i].recorder != null)
					{
						this.recordersList[i].accTime += (float)this.recordersList[i].recorder.elapsedNanoseconds / 1000000f;
						this.recordersList[i].callCount += this.recordersList[i].recorder.sampleBlockCount;
					}
				}
				if (this.m_AccDeltaTime >= 1f)
				{
					float num = 1f / (float)this.m_frameCount;
					float num2 = this.recordersList[0].accTime * num;
					float num3 = this.recordersList[1].accTime * num;
					float num4 = this.recordersList[2].accTime * num;
					float num5 = this.recordersList[3].accTime * num;
					float num6 = this.recordersList[4].accTime * num;
					float num7 = this.recordersList[9].accTime * num;
					this.m_statsLabel = string.Format("Accumulated time for RenderLoop.Draw and ShadowLoop.Draw (all threads)\n{0:F2}ms CPU Rendering time ( incl {1:F2}ms RT idle )\n", num2 + num3 + num4 + num5 + num7, num6);
					if (useScriptableRenderPipelineBatching)
					{
						this.m_statsLabel += string.Format("  {0:F2}ms SRP Batcher code path\n", num4 + num5);
						this.m_statsLabel += string.Format("    {0:F2}ms All objects ( {1} ApplyShader calls )\n", num4, this.recordersList[7].callCount / this.m_frameCount);
						this.m_statsLabel += string.Format("    {0:F2}ms Shadows ( {1} ApplyShader calls )\n", num5, this.recordersList[8].callCount / this.m_frameCount);
					}
					this.m_statsLabel += string.Format("  {0:F2}ms Standard code path\n", num2 + num3);
					this.m_statsLabel += string.Format("    {0:F2}ms All objects ( {1} ApplyShader calls )\n", num2, this.recordersList[5].callCount / this.m_frameCount);
					this.m_statsLabel += string.Format("    {0:F2}ms Shadows ( {1} ApplyShader calls )\n", num3, this.recordersList[6].callCount / this.m_frameCount);
					this.m_statsLabel += string.Format("  {0:F2}ms PIR Prepare Group Nodes ( {1} calls )\n", num7, this.recordersList[9].callCount / this.m_frameCount);
					this.m_statsLabel += string.Format("Global Main Loop: {0:F2}ms ({1} FPS)\n", this.m_AccDeltaTime * 1000f * num, (int)((float)this.m_frameCount / this.m_AccDeltaTime));
					this.RazCounters();
				}
			}
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x000E4468 File Offset: 0x000E2668
		private void OnGUI()
		{
			float num = 50f;
			if (this.m_Enable)
			{
				bool useScriptableRenderPipelineBatching = GraphicsSettings.useScriptableRenderPipelineBatching;
				GUI.color = new Color(1f, 1f, 1f, 1f);
				float width = 700f;
				float num2 = 256f;
				num += num2 + 50f;
				if (useScriptableRenderPipelineBatching)
				{
					GUILayout.BeginArea(new Rect(32f, 50f, width, num2), "SRP batcher ON (F9)", GUI.skin.window);
				}
				else
				{
					GUILayout.BeginArea(new Rect(32f, 50f, width, num2), "SRP batcher OFF (F9)", GUI.skin.window);
				}
				GUILayout.Label(this.m_statsLabel, this.m_style, Array.Empty<GUILayoutOption>());
				GUILayout.EndArea();
			}
		}

		// Token: 0x04000F6F RID: 3951
		public bool m_Enable = true;

		// Token: 0x04000F70 RID: 3952
		private const float kAverageStatDuration = 1f;

		// Token: 0x04000F71 RID: 3953
		private int m_frameCount;

		// Token: 0x04000F72 RID: 3954
		private float m_AccDeltaTime;

		// Token: 0x04000F73 RID: 3955
		private string m_statsLabel;

		// Token: 0x04000F74 RID: 3956
		private GUIStyle m_style;

		// Token: 0x04000F75 RID: 3957
		private bool m_oldBatcherEnable;

		// Token: 0x04000F76 RID: 3958
		private SRPBatcherProfiler.RecorderEntry[] recordersList = new SRPBatcherProfiler.RecorderEntry[]
		{
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "RenderLoop.Draw"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "Shadows.Draw"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "SRPBatcher.Draw",
				oldName = "RenderLoopNewBatcher.Draw"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "SRPBatcherShadow.Draw",
				oldName = "ShadowLoopNewBatcher.Draw"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "RenderLoopDevice.Idle"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "StdRender.ApplyShader"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "StdShadow.ApplyShader"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "SRPBRender.ApplyShader"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "SRPBShadow.ApplyShader"
			},
			new SRPBatcherProfiler.RecorderEntry
			{
				name = "PrepareBatchRendererGroupNodes"
			}
		};

		// Token: 0x0200020E RID: 526
		internal class RecorderEntry
		{
			// Token: 0x04000F77 RID: 3959
			public string name;

			// Token: 0x04000F78 RID: 3960
			public string oldName;

			// Token: 0x04000F79 RID: 3961
			public int callCount;

			// Token: 0x04000F7A RID: 3962
			public float accTime;

			// Token: 0x04000F7B RID: 3963
			public Recorder recorder;
		}

		// Token: 0x0200020F RID: 527
		private enum SRPBMarkers
		{
			// Token: 0x04000F7D RID: 3965
			kStdRenderDraw,
			// Token: 0x04000F7E RID: 3966
			kStdShadowDraw,
			// Token: 0x04000F7F RID: 3967
			kSRPBRenderDraw,
			// Token: 0x04000F80 RID: 3968
			kSRPBShadowDraw,
			// Token: 0x04000F81 RID: 3969
			kRenderThreadIdle,
			// Token: 0x04000F82 RID: 3970
			kStdRenderApplyShader,
			// Token: 0x04000F83 RID: 3971
			kStdShadowApplyShader,
			// Token: 0x04000F84 RID: 3972
			kSRPBRenderApplyShader,
			// Token: 0x04000F85 RID: 3973
			kSRPBShadowApplyShader,
			// Token: 0x04000F86 RID: 3974
			kPrepareBatchRendererGroupNodes
		}
	}
}
