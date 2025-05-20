using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;

namespace SoL.Utilities
{
	// Token: 0x02000259 RID: 601
	public static class CommandRegistry
	{
		// Token: 0x06001356 RID: 4950 RVA: 0x000F6688 File Offset: 0x000F4888
		public static void Register(string command, Action<string[]> function, string shortDescription = null, string description = null, string[] aliases = null)
		{
			CommandRegistry.m_commandRegister.AddOrReplace(command, new Command
			{
				CommandText = command,
				Action = function,
				ShortDescription = shortDescription,
				Description = description,
				Aliases = aliases
			});
			if (aliases != null)
			{
				foreach (string key in aliases)
				{
					CommandRegistry.m_commandAliases.AddOrReplace(key, command);
				}
			}
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x000F66F8 File Offset: 0x000F48F8
		public static void UnRegister(string command)
		{
			Command command2;
			if (CommandRegistry.m_commandRegister.TryGetValue(command, out command2))
			{
				if (command2.Aliases != null)
				{
					foreach (string key in command2.Aliases)
					{
						CommandRegistry.m_commandAliases.Remove(key);
					}
				}
				CommandRegistry.m_commandRegister.Remove(command);
			}
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x0004FA6F File Offset: 0x0004DC6F
		public static bool IsRegistered(string command, bool includeAliases = true)
		{
			return CommandRegistry.m_commandRegister.ContainsKey(command) || (includeAliases && CommandRegistry.m_commandAliases.ContainsKey(command));
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x000F6750 File Offset: 0x000F4950
		public static void Execute(string command, params string[] args)
		{
			if (CommandRegistry.m_commandRegister.ContainsKey(command))
			{
				CommandRegistry.m_commandRegister[command].Action(args);
				return;
			}
			if (CommandRegistry.m_commandAliases.ContainsKey(command) && CommandRegistry.m_commandRegister.ContainsKey(CommandRegistry.m_commandAliases[command]))
			{
				CommandRegistry.m_commandRegister[CommandRegistry.m_commandAliases[command]].Action(args);
			}
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x0004FA90 File Offset: 0x0004DC90
		public static IEnumerable<Command> GetCommandList()
		{
			return CommandRegistry.m_commandRegister.Values;
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x000F67C8 File Offset: 0x000F49C8
		public static Command GetCommand(string command, bool includeAliases = true)
		{
			if (CommandRegistry.m_commandRegister.ContainsKey(command))
			{
				return CommandRegistry.m_commandRegister[command];
			}
			if (includeAliases && CommandRegistry.m_commandAliases.ContainsKey(command) && CommandRegistry.m_commandRegister.ContainsKey(CommandRegistry.m_commandAliases[command]))
			{
				return CommandRegistry.m_commandRegister[CommandRegistry.m_commandAliases[command]];
			}
			return default(Command);
		}

		// Token: 0x04001B8D RID: 7053
		private static readonly Dictionary<string, Command> m_commandRegister = new Dictionary<string, Command>();

		// Token: 0x04001B8E RID: 7054
		private static readonly Dictionary<string, string> m_commandAliases = new Dictionary<string, string>();
	}
}
