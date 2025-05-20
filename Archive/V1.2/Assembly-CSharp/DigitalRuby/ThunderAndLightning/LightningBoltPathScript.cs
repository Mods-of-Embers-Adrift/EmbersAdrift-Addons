using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000BF RID: 191
	public class LightningBoltPathScript : LightningBoltPathScriptBase
	{
		// Token: 0x06000700 RID: 1792 RVA: 0x000AB794 File Offset: 0x000A9994
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			Vector3? vector = null;
			List<GameObject> currentPathObjects = base.GetCurrentPathObjects();
			if (currentPathObjects.Count < 2)
			{
				return;
			}
			if (this.nextIndex >= currentPathObjects.Count)
			{
				if (!this.Repeat)
				{
					return;
				}
				if (currentPathObjects[currentPathObjects.Count - 1] == currentPathObjects[0])
				{
					this.nextIndex = 1;
				}
				else
				{
					this.nextIndex = 0;
					this.lastPoint = null;
				}
			}
			try
			{
				if (this.lastPoint == null)
				{
					List<GameObject> list = currentPathObjects;
					int num = this.nextIndex;
					this.nextIndex = num + 1;
					this.lastPoint = new Vector3?(list[num].transform.position);
				}
				vector = new Vector3?(currentPathObjects[this.nextIndex].transform.position);
				if (this.lastPoint != null && vector != null)
				{
					parameters.Start = this.lastPoint.Value;
					parameters.End = vector.Value;
					base.CreateLightningBolt(parameters);
					if ((this.nextInterval -= this.Speed) <= 0f)
					{
						float num2 = UnityEngine.Random.Range(this.SpeedIntervalRange.Minimum, this.SpeedIntervalRange.Maximum);
						this.nextInterval = num2 + this.nextInterval;
						this.lastPoint = vector;
						this.nextIndex++;
					}
				}
			}
			catch (NullReferenceException)
			{
			}
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x00047C16 File Offset: 0x00045E16
		public void Reset()
		{
			this.lastPoint = null;
			this.nextIndex = 0;
			this.nextInterval = 1f;
		}

		// Token: 0x04000848 RID: 2120
		[Tooltip("How fast the lightning moves through the points or objects. 1 is normal speed, 0.01 is slower, so the lightning will move slowly between the points or objects.")]
		[Range(0.01f, 1f)]
		public float Speed = 1f;

		// Token: 0x04000849 RID: 2121
		[Tooltip("Repeat when the path completes?")]
		[SingleLineClamp("When each new point is moved to, this can provide a random value to make the movement to the next point appear more staggered or random. Leave as 1 and 1 to have constant speed. Use a higher maximum to create more randomness.", 1.0, 500.0)]
		public RangeOfFloats SpeedIntervalRange = new RangeOfFloats
		{
			Minimum = 1f,
			Maximum = 1f
		};

		// Token: 0x0400084A RID: 2122
		[Tooltip("Repeat when the path completes?")]
		public bool Repeat = true;

		// Token: 0x0400084B RID: 2123
		private float nextInterval = 1f;

		// Token: 0x0400084C RID: 2124
		private int nextIndex;

		// Token: 0x0400084D RID: 2125
		private Vector3? lastPoint;
	}
}
