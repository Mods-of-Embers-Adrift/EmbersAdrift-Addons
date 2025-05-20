using System;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x02000242 RID: 578
	public static class AntiAliasingTypeInternalExtensions
	{
		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06001309 RID: 4873 RVA: 0x0004F88A File Offset: 0x0004DA8A
		private static AntiAliasingTypeInternal[] InternalTypes
		{
			get
			{
				if (AntiAliasingTypeInternalExtensions.m_internalTypes == null)
				{
					AntiAliasingTypeInternalExtensions.m_internalTypes = (AntiAliasingTypeInternal[])Enum.GetValues(typeof(AntiAliasingTypeInternal));
				}
				return AntiAliasingTypeInternalExtensions.m_internalTypes;
			}
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0004F8B1 File Offset: 0x0004DAB1
		private static HDAdditionalCameraData.AntialiasingMode GetEngineAntiAliasingType(this AntiAliasingTypeInternal internalType)
		{
			switch (internalType)
			{
			default:
				return HDAdditionalCameraData.AntialiasingMode.None;
			case AntiAliasingTypeInternal.FXAA:
				return HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
			case AntiAliasingTypeInternal.SMAA:
				return HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			}
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x000E8AEC File Offset: 0x000E6CEC
		private static AntiAliasingTypeInternal GetInternalAntiAliasingType()
		{
			AntiAliasingTypeInternal result = AntiAliasingTypeInternal.None;
			if (Enum.IsDefined(typeof(AntiAliasingTypeInternal), Options.VideoOptions.AntiAliasingType.Value))
			{
				result = (AntiAliasingTypeInternal)Options.VideoOptions.AntiAliasingType.Value;
			}
			return result;
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0004F8CA File Offset: 0x0004DACA
		public static void ValidateAntiAliasingSelection()
		{
			Options.VideoOptions.AntiAliasingType.Value = (int)AntiAliasingTypeInternalExtensions.GetInternalAntiAliasingType();
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x000E8B28 File Offset: 0x000E6D28
		public static List<string> GetTypeStrings()
		{
			List<string> list = new List<string>(AntiAliasingTypeInternalExtensions.InternalTypes.Length);
			for (int i = 0; i < AntiAliasingTypeInternalExtensions.InternalTypes.Length; i++)
			{
				list.Add(AntiAliasingTypeInternalExtensions.InternalTypes[i].ToString());
			}
			return list;
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0004F8DB File Offset: 0x0004DADB
		public static HDAdditionalCameraData.AntialiasingMode GetEngineAntiAliasingType()
		{
			return AntiAliasingTypeInternalExtensions.GetInternalAntiAliasingType().GetEngineAntiAliasingType();
		}

		// Token: 0x040010DE RID: 4318
		private static AntiAliasingTypeInternal[] m_internalTypes;
	}
}
