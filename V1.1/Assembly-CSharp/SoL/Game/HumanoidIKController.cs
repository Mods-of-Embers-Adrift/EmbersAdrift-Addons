using System;
using RootMotion;
using RootMotion.FinalIK;
using SoL.Game.Settings;

namespace SoL.Game
{
	// Token: 0x0200058C RID: 1420
	public class HumanoidIKController : IKController
	{
		// Token: 0x06002C53 RID: 11347 RVA: 0x00148BBC File Offset: 0x00146DBC
		internal void Initialize(HumanoidReferencePoints humanoidReferencePoints)
		{
			if (this.m_main)
			{
				this.m_fbbik = (this.m_main as FullBodyBipedIK);
			}
			if (this.m_fbbik)
			{
				BipedReferences references = this.m_fbbik.references;
				if (humanoidReferencePoints.LeftMount != null)
				{
					references.leftHand = IKController.CreateHandIk("LeftHandIK", humanoidReferencePoints.LeftMount.transform).transform;
				}
				if (humanoidReferencePoints.RightMount != null)
				{
					references.rightHand = IKController.CreateHandIk("RightHandIK", humanoidReferencePoints.RightMount.transform).transform;
				}
				this.m_fbbik.SetReferences(references, null);
				this.m_fbbik.solver.iterations = 0;
			}
			if (this.m_grounder)
			{
				this.m_grounder.solver.maxFootRotationAngle = GlobalSettings.Values.Uma.MaxFootRotationAngle;
			}
			if (this.m_look)
			{
				this.m_look.solver.bodyWeight = GlobalSettings.Values.Uma.LookBodyWeight;
				this.m_look.solver.headWeight = GlobalSettings.Values.Uma.LookHeadWeight;
				this.m_look.solver.eyesWeight = GlobalSettings.Values.Uma.LookEyeWeight;
			}
		}
	}
}
