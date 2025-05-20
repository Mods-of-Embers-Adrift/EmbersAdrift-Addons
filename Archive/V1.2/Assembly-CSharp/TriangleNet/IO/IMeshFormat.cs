using System;
using System.IO;
using TriangleNet.Meshing;

namespace TriangleNet.IO
{
	// Token: 0x0200013C RID: 316
	public interface IMeshFormat : IFileFormat
	{
		// Token: 0x06000AC5 RID: 2757
		IMesh Import(string filename);

		// Token: 0x06000AC6 RID: 2758
		void Write(IMesh mesh, string filename);

		// Token: 0x06000AC7 RID: 2759
		void Write(IMesh mesh, Stream stream);
	}
}
