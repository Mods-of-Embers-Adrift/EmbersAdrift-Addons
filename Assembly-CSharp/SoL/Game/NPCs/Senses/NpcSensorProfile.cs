using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs.Senses
{
	// Token: 0x02000832 RID: 2098
	[CreateAssetMenu(menuName = "SoL/Profiles/Sensors", fileName = "SensorProfile")]
	public class NpcSensorProfile : ScriptableObject
	{
		// Token: 0x17000DFD RID: 3581
		// (get) Token: 0x06003CC4 RID: 15556 RVA: 0x000692B7 File Offset: 0x000674B7
		private bool m_showVisualImmediate
		{
			get
			{
				return this.HasSensor(SensorType.VisualImmediate);
			}
		}

		// Token: 0x17000DFE RID: 3582
		// (get) Token: 0x06003CC5 RID: 15557 RVA: 0x000692C0 File Offset: 0x000674C0
		private bool m_showVisualPeripheral
		{
			get
			{
				return this.HasSensor(SensorType.VisualPeripheral);
			}
		}

		// Token: 0x17000DFF RID: 3583
		// (get) Token: 0x06003CC6 RID: 15558 RVA: 0x000692C9 File Offset: 0x000674C9
		private bool m_showAuditory
		{
			get
			{
				return this.HasSensor(SensorType.Auditory);
			}
		}

		// Token: 0x17000E00 RID: 3584
		// (get) Token: 0x06003CC7 RID: 15559 RVA: 0x000692D2 File Offset: 0x000674D2
		private bool m_showOlfactory
		{
			get
			{
				return this.HasSensor(SensorType.Olfactory);
			}
		}

		// Token: 0x06003CC8 RID: 15560 RVA: 0x00180F8C File Offset: 0x0017F18C
		public float GetMaxDistance()
		{
			if (this.m_maxDistance == null)
			{
				this.m_maxDistance = new float?(1f);
				if (this.HasSensor(SensorType.VisualImmediate))
				{
					this.m_maxDistance = new float?(Mathf.Max(this.m_maxDistance.Value, this.m_visualImmediate.Distance));
				}
				if (this.HasSensor(SensorType.VisualPeripheral))
				{
					this.m_maxDistance = new float?(Mathf.Max(this.m_maxDistance.Value, this.m_visualPeripheral.Distance));
				}
				if (this.HasSensor(SensorType.Auditory))
				{
					this.m_maxDistance = new float?(Mathf.Max(this.m_maxDistance.Value, this.m_auditory.Distance));
				}
				if (this.HasSensor(SensorType.Olfactory))
				{
					this.m_maxDistance = new float?(Mathf.Max(this.m_maxDistance.Value, this.m_olfactory.Distance));
				}
			}
			return this.m_maxDistance.Value;
		}

		// Token: 0x06003CC9 RID: 15561 RVA: 0x00181080 File Offset: 0x0017F280
		public SensorSettings GetSensorSettingsForType(SensorType type)
		{
			switch (type)
			{
			case SensorType.VisualImmediate:
				return this.m_visualImmediate;
			case SensorType.VisualPeripheral:
				return this.m_visualPeripheral;
			case SensorType.Auditory:
				return this.m_auditory;
			case SensorType.Olfactory:
				return this.m_olfactory;
			default:
				throw new ArgumentException("type");
			}
		}

		// Token: 0x06003CCA RID: 15562 RVA: 0x001810D0 File Offset: 0x0017F2D0
		public bool HasSensor(SensorType sensorType)
		{
			if (sensorType == SensorType.Auditory)
			{
				return false;
			}
			SensorTypeFlags flagForType = sensorType.GetFlagForType();
			return this.m_activeSensors.HasBitFlag(flagForType);
		}

		// Token: 0x04003B9A RID: 15258
		[SerializeField]
		private SensorTypeFlags m_activeSensors = SensorTypeFlags.All;

		// Token: 0x04003B9B RID: 15259
		private const string kVisualGroup = "Visual";

		// Token: 0x04003B9C RID: 15260
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x04003B9D RID: 15261
		[SerializeField]
		private SensorSettings m_visualImmediate;

		// Token: 0x04003B9E RID: 15262
		[SerializeField]
		private SensorSettings m_visualPeripheral;

		// Token: 0x04003B9F RID: 15263
		[SerializeField]
		private SensorSettings m_auditory;

		// Token: 0x04003BA0 RID: 15264
		[SerializeField]
		private SensorSettings m_olfactory;

		// Token: 0x04003BA1 RID: 15265
		private float? m_maxDistance;
	}
}
