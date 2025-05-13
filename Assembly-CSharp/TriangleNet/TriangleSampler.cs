using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020000F4 RID: 244
	internal class TriangleSampler : IEnumerable<Triangle>, IEnumerable
	{
		// Token: 0x060008BB RID: 2235 RVA: 0x00048B1D File Offset: 0x00046D1D
		public TriangleSampler(Mesh mesh) : this(mesh, new Random(110503))
		{
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x00048B30 File Offset: 0x00046D30
		public TriangleSampler(Mesh mesh, Random random)
		{
			this.mesh = mesh;
			this.random = random;
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x00048B4D File Offset: 0x00046D4D
		public void Reset()
		{
			this.samples = 1;
			this.triangleCount = 0;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x000BD830 File Offset: 0x000BBA30
		public void Update()
		{
			int count = this.mesh.triangles.Count;
			if (this.triangleCount != count)
			{
				this.triangleCount = count;
				while (11 * this.samples * this.samples * this.samples < count)
				{
					this.samples++;
				}
			}
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00048B5D File Offset: 0x00046D5D
		public IEnumerator<Triangle> GetEnumerator()
		{
			return this.mesh.triangles.Sample(this.samples, this.random).GetEnumerator();
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x00048B80 File Offset: 0x00046D80
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x040009FE RID: 2558
		private const int RANDOM_SEED = 110503;

		// Token: 0x040009FF RID: 2559
		private const int samplefactor = 11;

		// Token: 0x04000A00 RID: 2560
		private Random random;

		// Token: 0x04000A01 RID: 2561
		private Mesh mesh;

		// Token: 0x04000A02 RID: 2562
		private int samples = 1;

		// Token: 0x04000A03 RID: 2563
		private int triangleCount;
	}
}
