using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x02000147 RID: 327
	public interface IPolygon
	{
		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000B1D RID: 2845
		List<Vertex> Points { get; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000B1E RID: 2846
		List<ISegment> Segments { get; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06000B1F RID: 2847
		List<Point> Holes { get; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06000B20 RID: 2848
		List<RegionPointer> Regions { get; }

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06000B21 RID: 2849
		// (set) Token: 0x06000B22 RID: 2850
		bool HasPointMarkers { get; set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000B23 RID: 2851
		// (set) Token: 0x06000B24 RID: 2852
		bool HasSegmentMarkers { get; set; }

		// Token: 0x06000B25 RID: 2853
		[Obsolete("Use polygon.Add(contour) method instead.")]
		void AddContour(IEnumerable<Vertex> points, int marker, bool hole, bool convex);

		// Token: 0x06000B26 RID: 2854
		[Obsolete("Use polygon.Add(contour) method instead.")]
		void AddContour(IEnumerable<Vertex> points, int marker, Point hole);

		// Token: 0x06000B27 RID: 2855
		Rectangle Bounds();

		// Token: 0x06000B28 RID: 2856
		void Add(Vertex vertex);

		// Token: 0x06000B29 RID: 2857
		void Add(ISegment segment, bool insert = false);

		// Token: 0x06000B2A RID: 2858
		void Add(ISegment segment, int index);

		// Token: 0x06000B2B RID: 2859
		void Add(Contour contour, bool hole = false);

		// Token: 0x06000B2C RID: 2860
		void Add(Contour contour, Point hole);
	}
}
