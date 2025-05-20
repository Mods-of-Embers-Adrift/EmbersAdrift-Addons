using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x0200014B RID: 331
	public class Polygon : IPolygon
	{
		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000B4E RID: 2894 RVA: 0x0004A31A File Offset: 0x0004851A
		public List<Vertex> Points
		{
			get
			{
				return this.points;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000B4F RID: 2895 RVA: 0x0004A322 File Offset: 0x00048522
		public List<Point> Holes
		{
			get
			{
				return this.holes;
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000B50 RID: 2896 RVA: 0x0004A32A File Offset: 0x0004852A
		public List<RegionPointer> Regions
		{
			get
			{
				return this.regions;
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0004A332 File Offset: 0x00048532
		public List<ISegment> Segments
		{
			get
			{
				return this.segments;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000B52 RID: 2898 RVA: 0x0004A33A File Offset: 0x0004853A
		// (set) Token: 0x06000B53 RID: 2899 RVA: 0x0004A342 File Offset: 0x00048542
		public bool HasPointMarkers { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000B54 RID: 2900 RVA: 0x0004A34B File Offset: 0x0004854B
		// (set) Token: 0x06000B55 RID: 2901 RVA: 0x0004A353 File Offset: 0x00048553
		public bool HasSegmentMarkers { get; set; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000B56 RID: 2902 RVA: 0x0004A35C File Offset: 0x0004855C
		public int Count
		{
			get
			{
				return this.points.Count;
			}
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x0004A369 File Offset: 0x00048569
		public Polygon() : this(3, false)
		{
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0004A369 File Offset: 0x00048569
		public Polygon(int capacity) : this(3, false)
		{
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x000CC394 File Offset: 0x000CA594
		public Polygon(int capacity, bool markers)
		{
			this.points = new List<Vertex>(capacity);
			this.holes = new List<Point>();
			this.regions = new List<RegionPointer>();
			this.segments = new List<ISegment>();
			this.HasPointMarkers = markers;
			this.HasSegmentMarkers = markers;
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0004A373 File Offset: 0x00048573
		[Obsolete("Use polygon.Add(contour) method instead.")]
		public void AddContour(IEnumerable<Vertex> points, int marker = 0, bool hole = false, bool convex = false)
		{
			this.Add(new Contour(points, marker, convex), hole);
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x0004A385 File Offset: 0x00048585
		[Obsolete("Use polygon.Add(contour) method instead.")]
		public void AddContour(IEnumerable<Vertex> points, int marker, Point hole)
		{
			this.Add(new Contour(points, marker), hole);
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x000CC3E4 File Offset: 0x000CA5E4
		public Rectangle Bounds()
		{
			List<Point> list = this.points.ConvertAll<Point>((Vertex x) => x);
			Rectangle rectangle = new Rectangle();
			rectangle.Expand(list);
			return rectangle;
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0004A395 File Offset: 0x00048595
		public void Add(Vertex vertex)
		{
			this.points.Add(vertex);
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0004A3A3 File Offset: 0x000485A3
		public void Add(ISegment segment, bool insert = false)
		{
			this.segments.Add(segment);
			if (insert)
			{
				this.points.Add(segment.GetVertex(0));
				this.points.Add(segment.GetVertex(1));
			}
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0004A3D8 File Offset: 0x000485D8
		public void Add(ISegment segment, int index)
		{
			this.segments.Add(segment);
			this.points.Add(segment.GetVertex(index));
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0004A3F8 File Offset: 0x000485F8
		public void Add(Contour contour, bool hole = false)
		{
			if (hole)
			{
				this.Add(contour, contour.FindInteriorPoint(5, 2E-05));
				return;
			}
			this.points.AddRange(contour.Points);
			this.segments.AddRange(contour.GetSegments());
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0004A437 File Offset: 0x00048637
		public void Add(Contour contour, Point hole)
		{
			this.points.AddRange(contour.Points);
			this.segments.AddRange(contour.GetSegments());
			this.holes.Add(hole);
		}

		// Token: 0x04000B12 RID: 2834
		private List<Vertex> points;

		// Token: 0x04000B13 RID: 2835
		private List<Point> holes;

		// Token: 0x04000B14 RID: 2836
		private List<RegionPointer> regions;

		// Token: 0x04000B15 RID: 2837
		private List<ISegment> segments;
	}
}
