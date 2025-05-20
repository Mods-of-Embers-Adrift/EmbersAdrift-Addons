using System;
using SoL.Game.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B3A RID: 2874
	public class RandomEyeshineBlink : MonoBehaviour
	{
		// Token: 0x1400011A RID: 282
		// (add) Token: 0x06005858 RID: 22616 RVA: 0x001E5A98 File Offset: 0x001E3C98
		// (remove) Token: 0x06005859 RID: 22617 RVA: 0x001E5AD0 File Offset: 0x001E3CD0
		public event Action<RandomEyeshineBlink> StateClosed;

		// Token: 0x0600585A RID: 22618 RVA: 0x001E5B08 File Offset: 0x001E3D08
		private void Awake()
		{
			if (this.m_renderer == null)
			{
				Debug.LogWarning("NO RENDER REF FOR RANDOMEYESHINE BLINK?");
				this.m_renderer = base.gameObject.GetComponent<Renderer>();
			}
			if (this.m_renderer == null)
			{
				base.enabled = false;
				return;
			}
			this.m_block = MaterialPropertyBlockCache.GetMaterialPropertyBlock(this.m_renderer);
			this.m_block.SetFloat(RandomEyeshineBlink.BlinkBlendId, this.m_blinkBlend);
			this.m_renderer.SetPropertyBlock(this.m_block);
			this.m_renderer.enabled = false;
		}

		// Token: 0x0600585B RID: 22619 RVA: 0x0007AFD1 File Offset: 0x000791D1
		private void OnEnable()
		{
			this.m_timeOfNextOpen = Time.time + this.m_closedDurationRange.RandomWithinRange();
			this.m_timeOfNextClose = Time.time + this.m_openDurationRange.RandomWithinRange();
		}

		// Token: 0x0600585C RID: 22620 RVA: 0x001E5B98 File Offset: 0x001E3D98
		private void Update()
		{
			if (!this.m_renderer)
			{
				return;
			}
			RandomEyeshineBlink.State state = this.m_state;
			switch (this.m_state)
			{
			case RandomEyeshineBlink.State.Closed:
				if (Time.time > this.m_timeOfNextOpen)
				{
					this.m_state = RandomEyeshineBlink.State.Opening;
				}
				break;
			case RandomEyeshineBlink.State.Opening:
				this.m_blinkBlend = Mathf.MoveTowards(this.m_blinkBlend, 1f, 0.1f);
				this.UpdateBlinkBlend();
				if (this.m_blinkBlend >= 1f)
				{
					this.m_state = RandomEyeshineBlink.State.Open;
					this.m_timeOfNextClose = Time.time + this.m_openDurationRange.RandomWithinRange();
				}
				break;
			case RandomEyeshineBlink.State.Open:
				if (Time.time > this.m_timeOfNextClose)
				{
					this.m_state = RandomEyeshineBlink.State.Closing;
				}
				break;
			case RandomEyeshineBlink.State.Closing:
				this.m_blinkBlend = Mathf.MoveTowards(this.m_blinkBlend, 0f, 0.1f);
				this.UpdateBlinkBlend();
				if (this.m_blinkBlend <= 0f)
				{
					this.m_state = RandomEyeshineBlink.State.Closed;
					this.m_timeOfNextOpen = Time.time + this.m_closedDurationRange.RandomWithinRange();
				}
				break;
			}
			this.m_renderer.enabled = (this.m_state > RandomEyeshineBlink.State.Closed);
			if (this.m_state != state && this.m_state == RandomEyeshineBlink.State.Closed)
			{
				Action<RandomEyeshineBlink> stateClosed = this.StateClosed;
				if (stateClosed == null)
				{
					return;
				}
				stateClosed(this);
			}
		}

		// Token: 0x0600585D RID: 22621 RVA: 0x0007B001 File Offset: 0x00079201
		private void UpdateBlinkBlend()
		{
			this.m_block.SetFloat(RandomEyeshineBlink.BlinkBlendId, this.m_blinkBlend);
			this.m_renderer.SetPropertyBlock(this.m_block);
		}

		// Token: 0x04004DBA RID: 19898
		private static readonly int BlinkBlendId = Shader.PropertyToID("_Blend");

		// Token: 0x04004DBB RID: 19899
		private const float kMaxBlinkBlendDelta = 0.1f;

		// Token: 0x04004DBC RID: 19900
		[SerializeField]
		private Renderer m_renderer;

		// Token: 0x04004DBD RID: 19901
		[SerializeField]
		private MinMaxFloatRange m_closedDurationRange = new MinMaxFloatRange(10f, 30f);

		// Token: 0x04004DBE RID: 19902
		[SerializeField]
		private MinMaxFloatRange m_openDurationRange = new MinMaxFloatRange(10f, 30f);

		// Token: 0x04004DBF RID: 19903
		private MaterialPropertyBlock m_block;

		// Token: 0x04004DC0 RID: 19904
		private RandomEyeshineBlink.State m_state;

		// Token: 0x04004DC1 RID: 19905
		private float m_blinkBlend;

		// Token: 0x04004DC2 RID: 19906
		private float m_timeOfNextOpen;

		// Token: 0x04004DC3 RID: 19907
		private float m_timeOfNextClose;

		// Token: 0x02000B3B RID: 2875
		private enum State
		{
			// Token: 0x04004DC5 RID: 19909
			Closed,
			// Token: 0x04004DC6 RID: 19910
			Opening,
			// Token: 0x04004DC7 RID: 19911
			Open,
			// Token: 0x04004DC8 RID: 19912
			Blinking,
			// Token: 0x04004DC9 RID: 19913
			Closing
		}
	}
}
