using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DisruptorUnity3d;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F1 RID: 1009
	public class SolServerConnection : MonoBehaviour
	{
		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06001ACD RID: 6861 RVA: 0x00054CE0 File Offset: 0x00052EE0
		public bool IsConnected
		{
			get
			{
				Socket socket = this.m_socket;
				return socket != null && socket.Connected;
			}
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x00054CF3 File Offset: 0x00052EF3
		private void Awake()
		{
			this.m_beginConnectCallback = new AsyncCallback(this.BeginConnectCallback);
			this.m_beginReceiveCallback = new AsyncCallback(this.BeginReceiveCallback);
			this.m_beginSendCallback = new AsyncCallback(this.BeginSendCallback);
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x00054D2B File Offset: 0x00052F2B
		private void OnDestroy()
		{
			if (this.m_fsm != null)
			{
				base.StopCoroutine(this.m_fsm);
			}
			this.Disconnect();
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x00109E30 File Offset: 0x00108030
		private void Update()
		{
			SolServerCommand solServerCommand;
			while (this.m_incomingCommands.TryDequeue(out solServerCommand))
			{
				if (!(solServerCommand == SolServerCommand.Empty))
				{
					Debug.Log("Received: " + solServerCommand.Text);
					CommandRouter.Route(solServerCommand);
				}
			}
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00054D47 File Offset: 0x00052F47
		private void AddByteToSendBuffer(byte octet)
		{
			this.m_sendByteBuffer.Add(octet);
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x00109E78 File Offset: 0x00108078
		private void AddByteArrayToSendBuffer(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				this.m_sendByteBuffer.Add(bytes[i]);
			}
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x00109EA4 File Offset: 0x001080A4
		public void Initialize(string address, int port, Action<bool> callback)
		{
			this.m_endPoint = new IPEndPoint(IPAddress.Parse(address), port);
			this.m_connectedCallback = callback;
			this.m_authenticated = false;
			if (this.m_initialized)
			{
				this.InitializeInternal();
				return;
			}
			this.m_reconnectWait = new WaitForSeconds(5f);
			this.m_initialized = true;
			this.InitializeInternal();
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00054D55 File Offset: 0x00052F55
		private void InitializeInternal()
		{
			base.StopAllCoroutines();
			this.m_state = SolServerConnection.ConnectionState.None;
			this.m_fsm = this.StateMachineCo();
			base.StartCoroutine(this.m_fsm);
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00054D7D File Offset: 0x00052F7D
		private IEnumerator StateMachineCo()
		{
			for (;;)
			{
				switch (this.m_state)
				{
				case SolServerConnection.ConnectionState.None:
					this.Connect();
					break;
				case SolServerConnection.ConnectionState.Connecting:
					yield return null;
					break;
				case SolServerConnection.ConnectionState.Connected:
					this.Receive();
					break;
				case SolServerConnection.ConnectionState.Receiving:
					if (!this.m_authenticated)
					{
						this.m_connectedCallback(false);
						this.m_authenticated = true;
					}
					else if (this.m_needsReauth)
					{
						this.m_connectedCallback(true);
						this.m_needsReauth = false;
					}
					yield return null;
					break;
				case SolServerConnection.ConnectionState.Reconnect:
					if (DateTime.UtcNow - this.m_lastReconnectAttempt > this.m_reconnectInterval)
					{
						this.m_state = SolServerConnection.ConnectionState.None;
						this.m_lastReconnectAttempt = DateTime.UtcNow;
						this.m_needsReauth = true;
					}
					else
					{
						yield return this.m_reconnectWait;
					}
					break;
				case SolServerConnection.ConnectionState.Disconnected:
					goto IL_12D;
				}
			}
			IL_12D:
			yield break;
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00054D8C File Offset: 0x00052F8C
		private void Connect()
		{
			this.m_state = SolServerConnection.ConnectionState.Connecting;
			this.m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.m_socket.BeginConnect(this.m_endPoint, this.m_beginConnectCallback, this.m_socket);
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x00109F00 File Offset: 0x00108100
		private void BeginConnectCallback(IAsyncResult ar)
		{
			try
			{
				((Socket)ar.AsyncState).EndConnect(ar);
				this.m_state = SolServerConnection.ConnectionState.Connected;
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception arg)
			{
				this.m_lastReconnectAttempt = DateTime.UtcNow;
				this.m_state = SolServerConnection.ConnectionState.Reconnect;
				Debug.LogWarning(string.Format("Exception in BeginConnectCallback: {0}", arg));
			}
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x00109F6C File Offset: 0x0010816C
		private void Receive()
		{
			try
			{
				this.m_state = SolServerConnection.ConnectionState.Receiving;
				StateObject stateObject = new StateObject(this.m_socket);
				this.m_socket.BeginReceive(stateObject.Buffer, 0, 1024, SocketFlags.None, this.m_beginReceiveCallback, stateObject);
			}
			catch (Exception arg)
			{
				Debug.LogWarning(string.Format("Exception in Receive: {0}", arg));
			}
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x00109FD4 File Offset: 0x001081D4
		private void BeginReceiveCallback(IAsyncResult ar)
		{
			if (this.m_state == SolServerConnection.ConnectionState.Disconnected)
			{
				return;
			}
			try
			{
				StateObject stateObject = (StateObject)ar.AsyncState;
				Socket workSocket = stateObject.WorkSocket;
				int num = workSocket.EndReceive(ar);
				if (num > 0)
				{
					stateObject.StoreBuffer(num);
					if (stateObject.IsReady)
					{
						this.ProcessBuffer(stateObject.RawMessage);
						stateObject.Reset();
					}
					workSocket.BeginReceive(stateObject.Buffer, 0, 1024, SocketFlags.None, this.m_beginReceiveCallback, stateObject);
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception arg)
			{
				this.m_lastReconnectAttempt = DateTime.UtcNow;
				this.m_state = SolServerConnection.ConnectionState.Reconnect;
				Debug.LogWarning(string.Format("Exception in BeginReceiveCallback: {0}", arg));
			}
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x0010A090 File Offset: 0x00108290
		public void Send(SolServerCommand cmd)
		{
			string text = JsonConvert.SerializeObject(cmd);
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			cmd.Args.ReturnToPool();
			this.m_sendByteBuffer.Clear();
			this.AddByteArrayToSendBuffer(bytes);
			this.m_sendByteBuffer.Add(4);
			Debug.Log("Sending: " + text);
			try
			{
				this.MarshalSending(this.m_sendByteBuffer.ToArray());
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode != SocketError.NotConnected)
				{
					throw ex;
				}
				this.m_lastReconnectAttempt = DateTime.UtcNow;
				this.m_state = SolServerConnection.ConnectionState.Reconnect;
			}
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00054DC1 File Offset: 0x00052FC1
		private void MarshalSending(byte[] data)
		{
			this.m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, this.m_beginSendCallback, this.m_socket);
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x0010A138 File Offset: 0x00108338
		private void BeginSendCallback(IAsyncResult ar)
		{
			try
			{
				((Socket)ar.AsyncState).EndSend(ar);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception arg)
			{
				this.m_lastReconnectAttempt = DateTime.UtcNow;
				this.m_state = SolServerConnection.ConnectionState.Reconnect;
				Debug.LogWarning(string.Format("Exception in BeginSendCallback: {0}", arg));
			}
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x00054DE1 File Offset: 0x00052FE1
		public void Disconnect()
		{
			this.Disconnect(false);
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x0010A1A0 File Offset: 0x001083A0
		public void Disconnect(bool saveSession)
		{
			if (!this.IsConnected)
			{
				return;
			}
			Dictionary<string, object> dictionary = SolServerCommandDictionaryPool.GetDictionary();
			dictionary.Add("save_session", saveSession);
			CommandClass.client.NewCommand(CommandType.disconnect, dictionary).Send(this);
			this.m_state = SolServerConnection.ConnectionState.Disconnected;
			if (this.m_socket != null)
			{
				try
				{
					this.m_socket.Shutdown(SocketShutdown.Both);
				}
				finally
				{
					this.m_socket.Close();
				}
			}
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x0010A218 File Offset: 0x00108418
		private void AddMessageToQueue(string msg)
		{
			SolServerCommand item = JsonConvert.DeserializeObject<SolServerCommand>(msg);
			item.Text = msg;
			this.m_incomingCommands.Enqueue(item);
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x0010A240 File Offset: 0x00108440
		private void ProcessBuffer(List<byte> buffer)
		{
			this.m_receiveByteBuffer.Clear();
			for (int i = 0; i < buffer.Count; i++)
			{
				byte b = buffer[i];
				if (b != 0)
				{
					if (b != 4)
					{
						this.m_receiveByteBuffer.Add(b);
					}
					else if (this.m_receiveByteBuffer.Count > 0)
					{
						string stringFromBufferSegment = this.GetStringFromBufferSegment(this.m_receiveByteBuffer.ToArray());
						this.m_receiveByteBuffer.Clear();
						this.AddMessageToQueue(stringFromBufferSegment);
					}
				}
			}
			if (this.m_receiveByteBuffer.Count > 0)
			{
				string stringFromBufferSegment2 = this.GetStringFromBufferSegment(this.m_receiveByteBuffer.ToArray());
				this.AddMessageToQueue(stringFromBufferSegment2);
			}
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x00054DEA File Offset: 0x00052FEA
		private string GetStringFromBufferSegment(byte[] data)
		{
			return Encoding.UTF8.GetString(data, 0, data.Length);
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x0010A2E0 File Offset: 0x001084E0
		private void AddToSendBuffer(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.AddByteArrayToSendBuffer(bytes);
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x0010A2FC File Offset: 0x001084FC
		private void AddToSendBuffer(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.AddByteArrayToSendBuffer(bytes);
		}

		// Token: 0x06001AE4 RID: 6884 RVA: 0x0010A318 File Offset: 0x00108518
		public void Ping(float x, float y, float z, int zoneId, byte subZoneId, byte role, byte level, byte emberRingIndex)
		{
			this.m_sendByteBuffer.Clear();
			this.m_sendByteBuffer.Add(7);
			this.AddToSendBuffer(x);
			this.AddToSendBuffer(y);
			this.AddToSendBuffer(z);
			this.AddToSendBuffer(zoneId);
			this.AddByteToSendBuffer(subZoneId);
			this.AddByteToSendBuffer(role);
			this.AddByteToSendBuffer(level);
			this.AddByteToSendBuffer(emberRingIndex);
			this.m_sendByteBuffer.Add(4);
			byte[] data = this.m_sendByteBuffer.ToArray();
			this.MarshalSending(data);
		}

		// Token: 0x04002207 RID: 8711
		private const byte kContinueByte = 0;

		// Token: 0x04002208 RID: 8712
		public const byte kTerminateByte = 4;

		// Token: 0x04002209 RID: 8713
		private const byte kPingByte = 7;

		// Token: 0x0400220A RID: 8714
		public SolServerConnectionType Type;

		// Token: 0x0400220B RID: 8715
		private bool m_initialized;

		// Token: 0x0400220C RID: 8716
		private Socket m_socket;

		// Token: 0x0400220D RID: 8717
		private IPEndPoint m_endPoint;

		// Token: 0x0400220E RID: 8718
		private SolServerConnection.ConnectionState m_state;

		// Token: 0x0400220F RID: 8719
		private IEnumerator m_fsm;

		// Token: 0x04002210 RID: 8720
		private WaitForSeconds m_reconnectWait;

		// Token: 0x04002211 RID: 8721
		private Action<bool> m_connectedCallback;

		// Token: 0x04002212 RID: 8722
		private readonly List<byte> m_receiveByteBuffer = new List<byte>();

		// Token: 0x04002213 RID: 8723
		private readonly List<byte> m_sendByteBuffer = new List<byte>();

		// Token: 0x04002214 RID: 8724
		private readonly RingBuffer<SolServerCommand> m_incomingCommands = new RingBuffer<SolServerCommand>(1024);

		// Token: 0x04002215 RID: 8725
		private AsyncCallback m_beginConnectCallback;

		// Token: 0x04002216 RID: 8726
		private AsyncCallback m_beginReceiveCallback;

		// Token: 0x04002217 RID: 8727
		private AsyncCallback m_beginSendCallback;

		// Token: 0x04002218 RID: 8728
		private DateTime m_lastReconnectAttempt = DateTime.MinValue;

		// Token: 0x04002219 RID: 8729
		private TimeSpan m_reconnectInterval = TimeSpan.FromSeconds(10.0);

		// Token: 0x0400221A RID: 8730
		private bool m_authenticated;

		// Token: 0x0400221B RID: 8731
		private bool m_needsReauth;

		// Token: 0x020003F2 RID: 1010
		public enum ConnectionState
		{
			// Token: 0x0400221D RID: 8733
			None,
			// Token: 0x0400221E RID: 8734
			Connecting,
			// Token: 0x0400221F RID: 8735
			Connected,
			// Token: 0x04002220 RID: 8736
			Receiving,
			// Token: 0x04002221 RID: 8737
			Reconnect,
			// Token: 0x04002222 RID: 8738
			Disconnected
		}
	}
}
