using System;
using System.Collections.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD7 RID: 3543
	[CreateAssetMenu(menuName = "SoL/Skeleton Delta Data")]
	internal class SkeletonDeltaData : ScriptableObject
	{
		// Token: 0x0600697C RID: 27004 RVA: 0x00217AC8 File Offset: 0x00215CC8
		private void OnValidate()
		{
			if (this.Data == null)
			{
				return;
			}
			foreach (ComparisonResult comparisonResult in this.Data)
			{
				comparisonResult.HidePosition = this.m_hidePosition;
				comparisonResult.HideRotation = this.m_hideRotation;
				comparisonResult.HideScale = this.m_hideScale;
			}
		}

		// Token: 0x0600697D RID: 27005 RVA: 0x00086C41 File Offset: 0x00084E41
		private void All()
		{
			this.Positions();
			this.Rotations();
			this.Scales();
		}

		// Token: 0x0600697E RID: 27006 RVA: 0x00086C55 File Offset: 0x00084E55
		private void Positions()
		{
			this.SetInternal(this.m_positionController, SkeletonModifier.SkeletonPropType.Position);
		}

		// Token: 0x0600697F RID: 27007 RVA: 0x00086C64 File Offset: 0x00084E64
		private void Rotations()
		{
			this.SetInternal(this.m_rotationController, SkeletonModifier.SkeletonPropType.Rotation);
		}

		// Token: 0x06006980 RID: 27008 RVA: 0x00086C73 File Offset: 0x00084E73
		private void Scales()
		{
			this.SetInternal(this.m_scaleController, SkeletonModifier.SkeletonPropType.Scale);
		}

		// Token: 0x06006981 RID: 27009 RVA: 0x00217B40 File Offset: 0x00215D40
		private void SetInternal(SkeletonDNAConverterPlugin controller, SkeletonModifier.SkeletonPropType propType)
		{
			if (controller == null)
			{
				return;
			}
			Vector3 defaultValue = (propType == SkeletonModifier.SkeletonPropType.Scale) ? Vector3.one : Vector3.zero;
			controller.skeletonModifiers = new List<SkeletonModifier>();
			foreach (ComparisonResult comparisonResult in this.Data)
			{
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				switch (propType)
				{
				case SkeletonModifier.SkeletonPropType.Position:
					vector = comparisonResult.PosSmall;
					vector2 = comparisonResult.PosLarge;
					break;
				case SkeletonModifier.SkeletonPropType.Rotation:
					vector = this.ModifyRotationVector(comparisonResult.RotSmall);
					vector2 = this.ModifyRotationVector(comparisonResult.RotLarge);
					break;
				case SkeletonModifier.SkeletonPropType.Scale:
					vector = comparisonResult.SizeSmall;
					vector2 = comparisonResult.SizeLarge;
					break;
				}
				if (!(vector == Vector3.zero) || !(vector2 == Vector3.zero))
				{
					string boneName = comparisonResult.BoneName;
					int hash = UMAUtils.StringToHash(boneName);
					SkeletonModifier skeletonModifier = new SkeletonModifier(boneName, hash, propType);
					SkeletonModifier.spVal spValForValues = this.GetSpValForValues(vector.x, vector2.x, defaultValue, propType);
					if (spValForValues != null)
					{
						skeletonModifier.valuesX = spValForValues;
					}
					SkeletonModifier.spVal spValForValues2 = this.GetSpValForValues(vector.y, vector2.y, defaultValue, propType);
					if (spValForValues2 != null)
					{
						skeletonModifier.valuesY = spValForValues2;
					}
					SkeletonModifier.spVal spValForValues3 = this.GetSpValForValues(vector.z, vector2.z, defaultValue, propType);
					if (spValForValues3 != null)
					{
						skeletonModifier.valuesZ = spValForValues3;
					}
					if (spValForValues != null || spValForValues2 != null || spValForValues3 != null)
					{
						controller.skeletonModifiers.Add(skeletonModifier);
					}
				}
			}
		}

		// Token: 0x06006982 RID: 27010 RVA: 0x00217CE0 File Offset: 0x00215EE0
		private Vector3 ModifyRotationVector(Vector3 input)
		{
			if (input.x > 300f)
			{
				input.x = 360f - input.x;
			}
			if (input.y > 300f)
			{
				input.y = 360f - input.y;
			}
			if (input.z > 300f)
			{
				input.z = 360f - input.z;
			}
			return input;
		}

		// Token: 0x06006983 RID: 27011 RVA: 0x00086C82 File Offset: 0x00084E82
		private void ResetAll()
		{
			this.ResetPositions();
			this.ResetRotations();
			this.ResetScales();
		}

		// Token: 0x06006984 RID: 27012 RVA: 0x00086C96 File Offset: 0x00084E96
		private void ResetPositions()
		{
			this.ResetInternal(this.m_positionController);
		}

		// Token: 0x06006985 RID: 27013 RVA: 0x00086CA4 File Offset: 0x00084EA4
		private void ResetRotations()
		{
			this.ResetInternal(this.m_rotationController);
		}

		// Token: 0x06006986 RID: 27014 RVA: 0x00086CB2 File Offset: 0x00084EB2
		private void ResetScales()
		{
			this.ResetInternal(this.m_scaleController);
		}

		// Token: 0x06006987 RID: 27015 RVA: 0x00086CC0 File Offset: 0x00084EC0
		private void ResetInternal(SkeletonDNAConverterPlugin controller)
		{
			if (controller != null)
			{
				controller.skeletonModifiers = new List<SkeletonModifier>();
			}
		}

		// Token: 0x06006988 RID: 27016 RVA: 0x00217D50 File Offset: 0x00215F50
		private SkeletonModifier.spVal GetSpValForValues(float small, float large, Vector3 defaultValue, SkeletonModifier.SkeletonPropType propType)
		{
			List<DNAEvaluator> dnaEvaluatorListForAxis = this.GetDnaEvaluatorListForAxis(small, large);
			if (dnaEvaluatorListForAxis != null)
			{
				SkeletonModifier.spVal spVal = new SkeletonModifier.spVal(defaultValue);
				if (propType == SkeletonModifier.SkeletonPropType.Rotation)
				{
					spVal.min = -360f;
					spVal.max = 360f;
				}
				else
				{
					spVal.min = -5f;
					spVal.max = 5f;
				}
				spVal.val.modifyingDNA = new DNAEvaluatorList(dnaEvaluatorListForAxis, DNAEvaluatorList.AggregationMethodOpts.Cumulative);
				return spVal;
			}
			return null;
		}

		// Token: 0x06006989 RID: 27017 RVA: 0x00217DB8 File Offset: 0x00215FB8
		private List<DNAEvaluator> GetDnaEvaluatorListForAxis(float small, float large)
		{
			if (small == 0f && large == 0f)
			{
				return null;
			}
			List<DNAEvaluator> list = new List<DNAEvaluator>();
			if (small != 0f)
			{
				list.Add(new DNAEvaluator("sizeSmall", DNAEvaluationGraph.Raw, small, DNAEvaluator.CalcOption.Add));
			}
			if (large != 0f)
			{
				list.Add(new DNAEvaluator("sizeLarge", DNAEvaluationGraph.Raw, large, DNAEvaluator.CalcOption.Add));
			}
			return list;
		}

		// Token: 0x04005BE7 RID: 23527
		[SerializeField]
		private bool m_hidePosition;

		// Token: 0x04005BE8 RID: 23528
		[SerializeField]
		private bool m_hideRotation;

		// Token: 0x04005BE9 RID: 23529
		[SerializeField]
		private bool m_hideScale;

		// Token: 0x04005BEA RID: 23530
		[SerializeField]
		private DynamicDNAConverterController m_parentController;

		// Token: 0x04005BEB RID: 23531
		[SerializeField]
		private SkeletonDNAConverterPlugin m_positionController;

		// Token: 0x04005BEC RID: 23532
		[SerializeField]
		private SkeletonDNAConverterPlugin m_rotationController;

		// Token: 0x04005BED RID: 23533
		[SerializeField]
		private SkeletonDNAConverterPlugin m_scaleController;

		// Token: 0x04005BEE RID: 23534
		public List<ComparisonResult> Data;

		// Token: 0x04005BEF RID: 23535
		private const string kButtonGroup = "BUTTONS";

		// Token: 0x04005BF0 RID: 23536
		private const string kButtonGroupTwo = "BUTTONS2";
	}
}
