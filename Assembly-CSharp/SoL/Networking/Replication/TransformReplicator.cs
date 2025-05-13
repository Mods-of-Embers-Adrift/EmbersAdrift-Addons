using System;
using NetStack.Serialization;
using SoL.Game;
using SoL.Game.Objects;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A8 RID: 1192
	public class TransformReplicator : Replicator
	{
		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06002139 RID: 8505 RVA: 0x000580F5 File Offset: 0x000562F5
		private bool PostSyncRotationNeedsRandom
		{
			get
			{
				return this.m_postSyncRotation == TransformReplicator.PostSyncRotation.RandomizeAllAxes || this.m_postSyncRotation == TransformReplicator.PostSyncRotation.RandomizeY;
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x0600213A RID: 8506 RVA: 0x0005810B File Offset: 0x0005630B
		private bool m_showPostData
		{
			get
			{
				return this.m_visuals != null;
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x0600213B RID: 8507 RVA: 0x00058119 File Offset: 0x00056319
		// (set) Token: 0x0600213C RID: 8508 RVA: 0x00058121 File Offset: 0x00056321
		public float SunkValue { get; private set; }

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x0600213D RID: 8509 RVA: 0x0005812A File Offset: 0x0005632A
		public Vector3? TargetPos
		{
			get
			{
				return this.m_targetPos;
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x0600213E RID: 8510 RVA: 0x00058132 File Offset: 0x00056332
		public Quaternion? TargetRot
		{
			get
			{
				return this.m_targetRot;
			}
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x0005813A File Offset: 0x0005633A
		public void ExternallySetTargetPosRot(Vector3 pos, Quaternion rot)
		{
			this.m_targetPos = new Vector3?(pos);
			this.m_targetRot = new Quaternion?(rot);
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x0004479C File Offset: 0x0004299C
		public override ReplicatorTypes Type
		{
			get
			{
				return ReplicatorTypes.Transform;
			}
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x00123084 File Offset: 0x00121284
		public override void SetDirtyFlags(DateTime timestamp)
		{
			if (this.m_netEntity == null)
			{
				base.Dirty = false;
				return;
			}
			Transform transform = this.m_netEntity.gameObject.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			if (position == this.m_cachedPos && rotation == this.m_cachedRot)
			{
				base.Dirty = (this.m_lastCache >= timestamp);
				return;
			}
			this.m_cachedPos = position;
			this.m_cachedRot = rotation;
			this.m_lastCache = DateTime.UtcNow;
			base.Dirty = true;
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x00123114 File Offset: 0x00121314
		public override BitBuffer PackData(BitBuffer outBuffer)
		{
			outBuffer = base.PackData(outBuffer);
			outBuffer.AddVector3(this.m_netEntity.gameObject.transform.position, NetworkManager.Range);
			if (this.m_useQuaternion)
			{
				outBuffer.AddQuaternion(this.m_netEntity.gameObject.transform.rotation);
			}
			else
			{
				outBuffer.AddFloat(this.m_netEntity.gameObject.transform.eulerAngles.y);
			}
			return outBuffer;
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x00123194 File Offset: 0x00121394
		public override BitBuffer ReadData(BitBuffer inBuffer)
		{
			inBuffer = base.ReadData(inBuffer);
			Vector3 vector = inBuffer.ReadVector3(NetworkManager.Range);
			Quaternion quaternion = this.m_useQuaternion ? inBuffer.ReadQuaternion() : Quaternion.Euler(new Vector3(0f, inBuffer.ReadFloat(), 0f));
			if (this.m_netEntity.IsServer || this.m_lastFrameReceived == Time.frameCount)
			{
				base.gameObject.transform.SetPositionAndRotation(vector, quaternion);
			}
			else if (!this.m_netEntity.IsLocal)
			{
				this.m_targetPos = new Vector3?(vector);
				this.m_targetRot = new Quaternion?(quaternion);
			}
			DateTime utcNow = DateTime.UtcNow;
			this.m_packetDelta = (float)(utcNow - this.m_lastReceive).TotalSeconds;
			this.m_lastReceive = utcNow;
			this.m_lastFrameReceived = Time.frameCount;
			float num = Mathf.Clamp(this.m_packetDelta, 0.1f, 1f);
			this.m_lerpScalar = Mathf.Lerp(5f, 1f, (num - 0.1f) / 0.9f);
			return inBuffer;
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x001232A4 File Offset: 0x001214A4
		public override BitBuffer PackInitialData(BitBuffer outBuffer)
		{
			outBuffer.AddVector3(this.m_netEntity.gameObject.transform.position, NetworkManager.Range);
			if (this.m_useQuaternion)
			{
				outBuffer.AddQuaternion(this.m_netEntity.gameObject.transform.rotation);
			}
			else
			{
				outBuffer.AddFloat(this.m_netEntity.gameObject.transform.eulerAngles.y);
			}
			return outBuffer;
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x0012331C File Offset: 0x0012151C
		public override BitBuffer ReadInitialData(BitBuffer inBuffer)
		{
			inBuffer = base.ReadInitialData(inBuffer);
			Vector3 vector = inBuffer.ReadVector3(NetworkManager.Range);
			Quaternion quaternion = this.m_useQuaternion ? inBuffer.ReadQuaternion() : Quaternion.Euler(new Vector3(0f, inBuffer.ReadFloat(), 0f));
			this.m_netEntity.gameObject.transform.SetPositionAndRotation(vector, quaternion);
			if (this.PostSyncAlignment())
			{
				vector = this.m_netEntity.gameObject.transform.position;
				quaternion = this.m_netEntity.gameObject.transform.rotation;
			}
			if (this.m_netEntity.IsLocal && LocalPlayer.Motor != null)
			{
				LocalPlayer.Motor.SetPositionAndRotation(vector, quaternion, true);
			}
			this.m_lastReceive = DateTime.UtcNow;
			return inBuffer;
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x00058154 File Offset: 0x00056354
		private void Update()
		{
			this.LerpTransform();
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x001233E8 File Offset: 0x001215E8
		private void LerpTransform()
		{
			if (GameManager.IsServer || !this.m_netEntity)
			{
				return;
			}
			float rawRotation = 0f;
			if (this.m_targetPos != null || this.m_targetRot != null)
			{
				Transform transform = this.m_netEntity.gameObject.transform;
				Vector3 targetPos = this.GetTargetPos(transform);
				Quaternion targetRot = this.GetTargetRot(transform);
				transform.SetPositionAndRotation(targetPos, targetRot);
				if (this.m_targetPos != null)
				{
					float sqrMagnitude = (transform.position - this.m_targetPos.Value).sqrMagnitude;
					if (sqrMagnitude >= 100f || sqrMagnitude <= 0.0001f)
					{
						transform.position = this.m_targetPos.Value;
						this.m_targetPos = null;
					}
				}
				if (this.m_targetRot != null)
				{
					if (Quaternion.Angle(transform.rotation, this.m_targetRot.Value) < 1f)
					{
						transform.rotation = this.m_targetRot.Value;
						this.m_targetRot = null;
					}
					else
					{
						rawRotation = this.m_targetRot.Value.eulerAngles.y - transform.eulerAngles.y;
					}
				}
			}
			if (!this.m_netEntity.IsLocal && base.GameEntity.AnimatorReplicator)
			{
				base.GameEntity.AnimatorReplicator.RawRotation = rawRotation;
			}
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x00123554 File Offset: 0x00121754
		private Vector3 GetTargetPos(Transform trans)
		{
			Vector3 position = trans.position;
			if (this.m_targetPos == null)
			{
				return position;
			}
			return Vector3.Lerp(position, this.m_targetPos.Value, Time.deltaTime * this.m_lerpScalar);
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x00123594 File Offset: 0x00121794
		private Quaternion GetTargetRot(Transform trans)
		{
			Quaternion rotation = trans.rotation;
			if (this.m_targetRot == null)
			{
				return rotation;
			}
			return Quaternion.Lerp(rotation, this.m_targetRot.Value, Time.deltaTime * this.m_lerpScalar);
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x001235D4 File Offset: 0x001217D4
		private bool PostSyncAlignment()
		{
			if (GameManager.IsServer || this.m_visuals == null)
			{
				return false;
			}
			System.Random random = null;
			RaycastHit? raycastHit = null;
			bool result = false;
			bool flag = !this.m_postSyncSink.IsZero;
			RaycastHit value;
			if (this.m_positionAtGroundOnSpawn && this.GetGroundHit(out value))
			{
				this.m_visuals.gameObject.transform.position = value.point;
				raycastHit = new RaycastHit?(value);
			}
			if (flag || this.PostSyncRotationNeedsRandom)
			{
				random = new System.Random((int)this.m_netEntity.NetworkId.Value);
			}
			if (flag)
			{
				this.SunkValue = Mathf.Lerp(this.m_postSyncSink.Min, this.m_postSyncSink.Max, (float)random.NextDouble());
				this.m_visuals.gameObject.transform.position -= Vector3.up * this.SunkValue;
				result = true;
			}
			switch (this.m_postSyncRotation)
			{
			case TransformReplicator.PostSyncRotation.AlignToGround:
			{
				RaycastHit value2;
				if (raycastHit == null && this.GetGroundHit(out value2))
				{
					raycastHit = new RaycastHit?(value2);
				}
				if (raycastHit != null)
				{
					float y = base.gameObject.transform.eulerAngles.y;
					this.m_visuals.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.Value.normal);
					this.m_visuals.transform.Rotate(0f, y, 0f, Space.Self);
					result = true;
				}
				break;
			}
			case TransformReplicator.PostSyncRotation.RandomizeAllAxes:
			{
				float x = Mathf.Lerp(0f, 360f, (float)random.NextDouble());
				float y2 = Mathf.Lerp(0f, 360f, (float)random.NextDouble());
				float z = Mathf.Lerp(0f, 360f, (float)random.NextDouble());
				this.m_visuals.transform.rotation = Quaternion.Euler(new Vector3(x, y2, z));
				result = true;
				break;
			}
			case TransformReplicator.PostSyncRotation.RandomizeY:
			{
				float y3 = Mathf.Lerp(0f, 360f, (float)random.NextDouble());
				this.m_visuals.transform.rotation = Quaternion.Euler(new Vector3(this.m_visuals.transform.eulerAngles.x, y3, this.m_visuals.transform.eulerAngles.z));
				result = true;
				break;
			}
			}
			return result;
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x00123848 File Offset: 0x00121A48
		private bool GetGroundHit(out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 2f, GlobalSettings.Values.Ik.GroundLayers, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x040025B0 RID: 9648
		private const float kLerpRate = 5f;

		// Token: 0x040025B1 RID: 9649
		private const float kSnapThresholdDistance = 100f;

		// Token: 0x040025B2 RID: 9650
		private const float kCloseEnoughDistance = 0.0001f;

		// Token: 0x040025B3 RID: 9651
		private const float kCloseEnoughAngle = 1f;

		// Token: 0x040025B4 RID: 9652
		[SerializeField]
		private bool m_useQuaternion;

		// Token: 0x040025B5 RID: 9653
		[SerializeField]
		private GameObject m_visuals;

		// Token: 0x040025B6 RID: 9654
		private const string kPostAlignmentGroup = "Alignment";

		// Token: 0x040025B7 RID: 9655
		[FormerlySerializedAs("m_alignToGroundOnSpawn")]
		[SerializeField]
		private bool m_positionAtGroundOnSpawn;

		// Token: 0x040025B8 RID: 9656
		[SerializeField]
		private TransformReplicator.PostSyncRotation m_postSyncRotation;

		// Token: 0x040025B9 RID: 9657
		[SerializeField]
		private MinMaxFloatRange m_postSyncSink = new MinMaxFloatRange(0f, 0f);

		// Token: 0x040025BB RID: 9659
		private Vector3 m_cachedPos = Vector3.zero;

		// Token: 0x040025BC RID: 9660
		private Quaternion m_cachedRot = Quaternion.identity;

		// Token: 0x040025BD RID: 9661
		private DateTime m_lastCache = DateTime.MinValue;

		// Token: 0x040025BE RID: 9662
		private DateTime m_lastReceive = DateTime.MinValue;

		// Token: 0x040025BF RID: 9663
		private int m_lastFrameReceived;

		// Token: 0x040025C0 RID: 9664
		private float m_packetDelta = 1f;

		// Token: 0x040025C1 RID: 9665
		private float m_lerpScalar = 1f;

		// Token: 0x040025C2 RID: 9666
		private Vector3? m_targetPos;

		// Token: 0x040025C3 RID: 9667
		private Quaternion? m_targetRot;

		// Token: 0x020004A9 RID: 1193
		private enum PostSyncRotation
		{
			// Token: 0x040025C5 RID: 9669
			None,
			// Token: 0x040025C6 RID: 9670
			AlignToGround,
			// Token: 0x040025C7 RID: 9671
			RandomizeAllAxes,
			// Token: 0x040025C8 RID: 9672
			RandomizeY
		}
	}
}
