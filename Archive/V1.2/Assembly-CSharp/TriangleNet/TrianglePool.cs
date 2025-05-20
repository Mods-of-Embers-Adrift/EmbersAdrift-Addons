using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020000F1 RID: 241
	public class TrianglePool : ICollection<Triangle>, IEnumerable<Triangle>, IEnumerable
	{
		// Token: 0x0600089E RID: 2206 RVA: 0x000BD36C File Offset: 0x000BB56C
		public TrianglePool()
		{
			this.size = 0;
			int num = Math.Max(1, 64);
			this.pool = new Triangle[num][];
			this.pool[0] = new Triangle[1024];
			this.stack = new Stack<Triangle>(1024);
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x000BD3C0 File Offset: 0x000BB5C0
		public Triangle Get()
		{
			Triangle triangle;
			if (this.stack.Count > 0)
			{
				triangle = this.stack.Pop();
				triangle.hash = -triangle.hash - 1;
				this.Cleanup(triangle);
			}
			else if (this.count < this.size)
			{
				triangle = this.pool[this.count / 1024][this.count % 1024];
				triangle.id = triangle.hash;
				this.Cleanup(triangle);
				this.count++;
			}
			else
			{
				triangle = new Triangle();
				triangle.hash = this.size;
				triangle.id = triangle.hash;
				int num = this.size / 1024;
				if (this.pool[num] == null)
				{
					this.pool[num] = new Triangle[1024];
					if (num + 1 == this.pool.Length)
					{
						Array.Resize<Triangle[]>(ref this.pool, 2 * this.pool.Length);
					}
				}
				this.pool[num][this.size % 1024] = triangle;
				int num2 = this.size + 1;
				this.size = num2;
				this.count = num2;
			}
			return triangle;
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x00048A57 File Offset: 0x00046C57
		public void Release(Triangle triangle)
		{
			this.stack.Push(triangle);
			triangle.hash = -triangle.hash - 1;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x000BD4F0 File Offset: 0x000BB6F0
		public TrianglePool Restart()
		{
			foreach (Triangle triangle in this.stack)
			{
				triangle.hash = -triangle.hash - 1;
			}
			this.stack.Clear();
			this.count = 0;
			return this;
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00048A74 File Offset: 0x00046C74
		internal IEnumerable<Triangle> Sample(int k, Random random)
		{
			int count = this.Count;
			if (k > count)
			{
				k = count;
			}
			while (k > 0)
			{
				int num = random.Next(0, count);
				Triangle triangle = this.pool[num / 1024][num % 1024];
				if (triangle.hash >= 0)
				{
					int num2 = k;
					k = num2 - 1;
					yield return triangle;
				}
			}
			yield break;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x000BD55C File Offset: 0x000BB75C
		private void Cleanup(Triangle triangle)
		{
			triangle.label = 0;
			triangle.area = 0.0;
			triangle.infected = false;
			for (int i = 0; i < 3; i++)
			{
				triangle.vertices[i] = null;
				triangle.subsegs[i] = default(Osub);
				triangle.neighbors[i] = default(Otri);
			}
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00048A92 File Offset: 0x00046C92
		public void Add(Triangle item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x000BD5C0 File Offset: 0x000BB7C0
		public void Clear()
		{
			this.stack.Clear();
			int num = this.size / 1024 + 1;
			for (int i = 0; i < num; i++)
			{
				Triangle[] array = this.pool[i];
				int num2 = (this.size - i * 1024) % 1024;
				for (int j = 0; j < num2; j++)
				{
					array[j] = null;
				}
			}
			this.size = (this.count = 0);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x000BD638 File Offset: 0x000BB838
		public bool Contains(Triangle item)
		{
			int hash = item.hash;
			return hash >= 0 && hash <= this.size && this.pool[hash / 1024][hash % 1024].hash >= 0;
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x000BD67C File Offset: 0x000BB87C
		public void CopyTo(Triangle[] array, int index)
		{
			foreach (Triangle triangle in this)
			{
				array[index] = triangle;
				index++;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x00048A99 File Offset: 0x00046C99
		public int Count
		{
			get
			{
				return this.count - this.stack.Count;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x0004479C File Offset: 0x0004299C
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00048A92 File Offset: 0x00046C92
		public bool Remove(Triangle item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x00048AAD File Offset: 0x00046CAD
		public IEnumerator<Triangle> GetEnumerator()
		{
			return new TrianglePool.Enumerator(this);
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x00048AB5 File Offset: 0x00046CB5
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x040009EB RID: 2539
		private const int BLOCKSIZE = 1024;

		// Token: 0x040009EC RID: 2540
		private int size;

		// Token: 0x040009ED RID: 2541
		private int count;

		// Token: 0x040009EE RID: 2542
		private Triangle[][] pool;

		// Token: 0x040009EF RID: 2543
		private Stack<Triangle> stack;

		// Token: 0x020000F2 RID: 242
		private class Enumerator : IEnumerator<Triangle>, IEnumerator, IDisposable
		{
			// Token: 0x060008AD RID: 2221 RVA: 0x00048ABD File Offset: 0x00046CBD
			public Enumerator(TrianglePool pool)
			{
				this.count = pool.Count;
				this.pool = pool.pool;
				this.index = 0;
				this.offset = 0;
			}

			// Token: 0x170002D6 RID: 726
			// (get) Token: 0x060008AE RID: 2222 RVA: 0x00048AEB File Offset: 0x00046CEB
			public Triangle Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x060008AF RID: 2223 RVA: 0x0004475B File Offset: 0x0004295B
			public void Dispose()
			{
			}

			// Token: 0x170002D7 RID: 727
			// (get) Token: 0x060008B0 RID: 2224 RVA: 0x00048AEB File Offset: 0x00046CEB
			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x060008B1 RID: 2225 RVA: 0x000BD6A8 File Offset: 0x000BB8A8
			public bool MoveNext()
			{
				while (this.index < this.count)
				{
					this.current = this.pool[this.offset / 1024][this.offset % 1024];
					this.offset++;
					if (this.current.hash >= 0)
					{
						this.index++;
						return true;
					}
				}
				return false;
			}

			// Token: 0x060008B2 RID: 2226 RVA: 0x000BD718 File Offset: 0x000BB918
			public void Reset()
			{
				this.index = (this.offset = 0);
			}

			// Token: 0x040009F0 RID: 2544
			private int count;

			// Token: 0x040009F1 RID: 2545
			private Triangle[][] pool;

			// Token: 0x040009F2 RID: 2546
			private Triangle current;

			// Token: 0x040009F3 RID: 2547
			private int index;

			// Token: 0x040009F4 RID: 2548
			private int offset;
		}
	}
}
