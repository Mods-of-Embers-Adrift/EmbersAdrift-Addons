using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003ED RID: 1005
	public struct SolServerCommand : IEquatable<SolServerCommand>
	{
		// Token: 0x06001AB9 RID: 6841 RVA: 0x00054B47 File Offset: 0x00052D47
		public SolServerCommand(CommandClass cmdClass, CommandType cmdType)
		{
			this.CommandClass = cmdClass;
			this.Command = cmdType;
			this.State = true;
			this.Args = SolServerCommandDictionaryPool.GetDictionary();
			this.Text = null;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x00054B70 File Offset: 0x00052D70
		public SolServerCommand(CommandClass cmdClass, CommandType cmdType, Dictionary<string, object> args)
		{
			this.CommandClass = cmdClass;
			this.Command = cmdType;
			this.State = true;
			this.Args = args;
			this.Text = null;
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x00054B95 File Offset: 0x00052D95
		public void Send(SolServerConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			connection.Send(this);
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x00054BB7 File Offset: 0x00052DB7
		public void Send()
		{
			if (!this.State)
			{
				return;
			}
			SolServerConnectionManager.CurrentConnection.Send(this);
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x00109C54 File Offset: 0x00107E54
		public bool TryGetArgValue(string key, out string value)
		{
			value = null;
			object obj;
			if (this.Args != null && this.Args.TryGetValue(key, out obj))
			{
				value = obj.ToString();
				return true;
			}
			return false;
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x00109C88 File Offset: 0x00107E88
		public override string ToString()
		{
			return string.Format("CommandClass: {0}, Command: {1}, State: {2}, Args: {3}, Text: {4}", new object[]
			{
				this.CommandClass.ToString(),
				this.Command.ToString(),
				this.State,
				this.Args.ToString(),
				this.Text
			});
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00054BD2 File Offset: 0x00052DD2
		public bool Equals(SolServerCommand other)
		{
			return this.CommandClass == other.CommandClass && this.Command == other.Command && this.State == other.State && object.Equals(this.Args, other.Args);
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x00054C11 File Offset: 0x00052E11
		public override bool Equals(object obj)
		{
			return obj != null && obj is SolServerCommand && this.Equals((SolServerCommand)obj);
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x00109CF4 File Offset: 0x00107EF4
		public override int GetHashCode()
		{
			return (int)(((this.CommandClass * (CommandClass)397 ^ (CommandClass)this.Command) * (CommandClass)397 ^ (CommandClass)this.State.GetHashCode()) * (CommandClass)397 ^ (CommandClass)((this.Args != null) ? this.Args.GetHashCode() : 0));
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x00054C2E File Offset: 0x00052E2E
		public static bool operator ==(SolServerCommand a, SolServerCommand b)
		{
			return a.Equals(b);
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x00054C38 File Offset: 0x00052E38
		public static bool operator !=(SolServerCommand a, SolServerCommand b)
		{
			return !a.Equals(b);
		}

		// Token: 0x040021FE RID: 8702
		public static string kErrorKey = "err";

		// Token: 0x040021FF RID: 8703
		[JsonConverter(typeof(StringEnumConverter))]
		public CommandClass CommandClass;

		// Token: 0x04002200 RID: 8704
		[JsonConverter(typeof(StringEnumConverter))]
		public CommandType Command;

		// Token: 0x04002201 RID: 8705
		[JsonProperty(PropertyName = "state")]
		public bool State;

		// Token: 0x04002202 RID: 8706
		[JsonProperty(PropertyName = "args")]
		public readonly Dictionary<string, object> Args;

		// Token: 0x04002203 RID: 8707
		[JsonIgnore]
		public string Text;

		// Token: 0x04002204 RID: 8708
		public static SolServerCommand Empty = default(SolServerCommand);
	}
}
