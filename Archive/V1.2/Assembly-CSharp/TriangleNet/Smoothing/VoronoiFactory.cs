using System;
using TriangleNet.Geometry;
using TriangleNet.Topology.DCEL;
using TriangleNet.Voronoi;

namespace TriangleNet.Smoothing
{
	// Token: 0x02000118 RID: 280
	internal class VoronoiFactory : IVoronoiFactory
	{
		// Token: 0x060009FB RID: 2555 RVA: 0x00049A95 File Offset: 0x00047C95
		public VoronoiFactory()
		{
			this.vertices = new VoronoiFactory.ObjectPool<TriangleNet.Topology.DCEL.Vertex>(3);
			this.edges = new VoronoiFactory.ObjectPool<HalfEdge>(3);
			this.faces = new VoronoiFactory.ObjectPool<Face>(3);
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x000C3AB0 File Offset: 0x000C1CB0
		public void Initialize(int vertexCount, int edgeCount, int faceCount)
		{
			this.vertices.Capacity = vertexCount;
			this.edges.Capacity = edgeCount;
			this.faces.Capacity = faceCount;
			for (int i = this.vertices.Count; i < vertexCount; i++)
			{
				this.vertices.Put(new TriangleNet.Topology.DCEL.Vertex(0.0, 0.0));
			}
			for (int j = this.edges.Count; j < edgeCount; j++)
			{
				this.edges.Put(new HalfEdge(null));
			}
			for (int k = this.faces.Count; k < faceCount; k++)
			{
				this.faces.Put(new Face(null));
			}
			this.Reset();
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00049AC1 File Offset: 0x00047CC1
		public void Reset()
		{
			this.vertices.Release();
			this.edges.Release();
			this.faces.Release();
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x000C3B70 File Offset: 0x000C1D70
		public TriangleNet.Topology.DCEL.Vertex CreateVertex(double x, double y)
		{
			TriangleNet.Topology.DCEL.Vertex vertex;
			if (this.vertices.TryGet(out vertex))
			{
				vertex.x = x;
				vertex.y = y;
				vertex.leaving = null;
				return vertex;
			}
			vertex = new TriangleNet.Topology.DCEL.Vertex(x, y);
			this.vertices.Put(vertex);
			return vertex;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x000C3BB8 File Offset: 0x000C1DB8
		public HalfEdge CreateHalfEdge(TriangleNet.Topology.DCEL.Vertex origin, Face face)
		{
			HalfEdge halfEdge;
			if (this.edges.TryGet(out halfEdge))
			{
				halfEdge.origin = origin;
				halfEdge.face = face;
				halfEdge.next = null;
				halfEdge.twin = null;
				if (face != null && face.edge == null)
				{
					face.edge = halfEdge;
				}
				return halfEdge;
			}
			halfEdge = new HalfEdge(origin, face);
			this.edges.Put(halfEdge);
			return halfEdge;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x000C3C1C File Offset: 0x000C1E1C
		public Face CreateFace(TriangleNet.Geometry.Vertex vertex)
		{
			Face face;
			if (this.faces.TryGet(out face))
			{
				face.id = vertex.id;
				face.generator = vertex;
				face.edge = null;
				return face;
			}
			face = new Face(vertex);
			this.faces.Put(face);
			return face;
		}

		// Token: 0x04000A8F RID: 2703
		private VoronoiFactory.ObjectPool<TriangleNet.Topology.DCEL.Vertex> vertices;

		// Token: 0x04000A90 RID: 2704
		private VoronoiFactory.ObjectPool<HalfEdge> edges;

		// Token: 0x04000A91 RID: 2705
		private VoronoiFactory.ObjectPool<Face> faces;

		// Token: 0x02000119 RID: 281
		private class ObjectPool<T> where T : class
		{
			// Token: 0x17000319 RID: 793
			// (get) Token: 0x06000A01 RID: 2561 RVA: 0x00049AE4 File Offset: 0x00047CE4
			public int Count
			{
				get
				{
					return this.count;
				}
			}

			// Token: 0x1700031A RID: 794
			// (get) Token: 0x06000A02 RID: 2562 RVA: 0x00049AEC File Offset: 0x00047CEC
			// (set) Token: 0x06000A03 RID: 2563 RVA: 0x00049AF6 File Offset: 0x00047CF6
			public int Capacity
			{
				get
				{
					return this.pool.Length;
				}
				set
				{
					this.Resize(value);
				}
			}

			// Token: 0x06000A04 RID: 2564 RVA: 0x00049AFF File Offset: 0x00047CFF
			public ObjectPool(int capacity = 3)
			{
				this.index = 0;
				this.count = 0;
				this.pool = new T[capacity];
			}

			// Token: 0x06000A05 RID: 2565 RVA: 0x00049B21 File Offset: 0x00047D21
			public ObjectPool(T[] pool)
			{
				this.index = 0;
				this.count = 0;
				this.pool = pool;
			}

			// Token: 0x06000A06 RID: 2566 RVA: 0x000C3C68 File Offset: 0x000C1E68
			public bool TryGet(out T obj)
			{
				if (this.index < this.count)
				{
					T[] array = this.pool;
					int num = this.index;
					this.index = num + 1;
					obj = array[num];
					return true;
				}
				obj = default(T);
				return false;
			}

			// Token: 0x06000A07 RID: 2567 RVA: 0x000C3CB0 File Offset: 0x000C1EB0
			public void Put(T obj)
			{
				int num = this.pool.Length;
				if (num <= this.count)
				{
					this.Resize(2 * num);
				}
				T[] array = this.pool;
				int num2 = this.count;
				this.count = num2 + 1;
				array[num2] = obj;
				this.index++;
			}

			// Token: 0x06000A08 RID: 2568 RVA: 0x00049B3E File Offset: 0x00047D3E
			public void Release()
			{
				this.index = 0;
			}

			// Token: 0x06000A09 RID: 2569 RVA: 0x00049B47 File Offset: 0x00047D47
			private void Resize(int size)
			{
				if (size > this.count)
				{
					Array.Resize<T>(ref this.pool, size);
				}
			}

			// Token: 0x04000A92 RID: 2706
			private int index;

			// Token: 0x04000A93 RID: 2707
			private int count;

			// Token: 0x04000A94 RID: 2708
			private T[] pool;
		}
	}
}
