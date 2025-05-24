using System;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using SoL.Game.Messages;
using SoL.Game.UI.Chat;
using System.Text.RegularExpressions;
using SoL.Game;

namespace DPSAddon
{
    public static class ManualInjector
    {
        private const string LOG_PREFIX = "[DPS Patch]";
        private const string HARMONY_ID = "DPSAddon.ManualPatch";
        private static HarmonyLib.Harmony _harmony;
        private static bool _isInitialized;

        // Regular expressions for parsing combat messages
        private static readonly Regex DamageRegex = new Regex(@"Your .+ hits .+ for (\d+)");
        private static readonly Regex HealingRegex = new Regex(@"Your .+ heals .+ for (\d+)");

        public static bool Inject()
        {
            if (_isInitialized)
            {
                LogDebug("Already initialized");
                return true;
            }

            try
            {
                LogDebug("Attempting chat message patch...");

                _harmony = new HarmonyLib.Harmony(HARMONY_ID);
                var success = ApplyPatch();

                if (success)
                {
                    _isInitialized = true;
                }

                return success;
            }
            catch (Exception ex)
            {
                LogError("Failed to initialize injector", ex);
                return false;
            }
        }

        private static bool ApplyPatch()
        {
            try
            {
                // Try to find MessageManager.ChatQueue.AddToQueue
                var targetMethod = FindAddToQueueMethod();
                if (targetMethod == null)
                {
                    LogWarning("Could not find AddToQueue method");
                    return false;
                }

                var postfix = typeof(ManualInjector).GetMethod(nameof(AddToQueue_Postfix),
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (postfix == null)
                {
                    LogWarning("Could not find postfix method");
                    return false;
                }

                _harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
                LogDebug("Successfully patched AddToQueue");
                return true;
            }
            catch (Exception ex)
            {
                LogError("Failed to apply patch", ex);
                return false;
            }
        }

        private static MethodInfo FindAddToQueueMethod()
        {
            try
            {
                // First try MessageManager.ChatQueue
                var messageManagerType = typeof(MessageManager);
                var chatQueueProperty = messageManagerType.GetProperty("ChatQueue");
                if (chatQueueProperty != null)
                {
                    var queueType = chatQueueProperty.PropertyType;
                    return queueType.GetMethod("AddToQueue", new Type[] { typeof(MessageType), typeof(string) });
                }

                // If that fails, try finding the method directly
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name.StartsWith("SoL"))
                    {
                        var types = assembly.GetTypes();
                        foreach (var type in types)
                        {
                            if (type.Name == "MessageQueue")
                            {
                                return type.GetMethod("AddToQueue", new Type[] { typeof(MessageType), typeof(string) });
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                LogError("Error finding AddToQueue method", ex);
                return null;
            }
        }
        private static void AddToQueue_Postfix(MessageType type, string content)
        {
            try
            {
                // Only process relevant combat messages
                if (type != MessageType.MyCombatOut && type != MessageType.MyCombatIn) return;

                LogDebug($"Processing combat message: {content} | Type: {type}");

                string playerName = LocalPlayer.GameEntity?.CharacterData?.Name?.Value;
                if (string.IsNullOrEmpty(playerName)) return;

                // Handle outgoing combat (damage and healing others)
                if (type == MessageType.MyCombatOut)
                {
                    // Early exit if not player action
                    if (!content.Contains("<i>Your</i>")) return;

                    // Check for damage (HITS)
                    if (content.Contains("HITS"))
                    {
                        var damageMatch = Regex.Match(content, @"for.*?(\d+)\s*health");
                        if (damageMatch.Success && int.TryParse(damageMatch.Groups[1].Value, out int damage))
                        {
                            LogDebug($"Recording outgoing damage: {playerName} -> {damage}");
                            DPSTracker.RecordDamage(playerName, damage);
                        }
                    }
                    // Check for outgoing healing ("Your X restores Y of target's health")
                    else if (content.Contains("restores") && content.Contains("of your health"))
                    {
                        var healMatch = Regex.Match(content, @"restores\s*(\d+)\s*of your health");
                        if (healMatch.Success && int.TryParse(healMatch.Groups[1].Value, out int healing))
                        {
                            LogDebug($"Recording outgoing healing: {playerName} -> {healing}");
                            DPSTracker.RecordHealing(playerName, healing);
                        }
                    }
                }
                // Handle incoming combat (receiving healing)
                else if (type == MessageType.MyCombatIn)
                {
                    // Check for incoming healing
                    if (content.Contains("restores") && content.Contains("of your health"))
                    {
                        var healMatch = Regex.Match(content, @"restores\s*(\d+)\s*of your health");
                        if (healMatch.Success && int.TryParse(healMatch.Groups[1].Value, out int healing))
                        {
                            LogDebug($"Recording incoming healing: {playerName} -> {healing}");
                            DPSTracker.RecordHealing(playerName, healing);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Error processing chat message", ex);
            }
        }





        private static string GetPlayerNameFromMessage(string content)
        {
            try
            {
                if (LocalPlayer.GameEntity?.CharacterData?.Name != null)
                {
                    return LocalPlayer.GameEntity.CharacterData.Name.Value;
                }
                return "Unknown";
            }
            catch (Exception ex)
            {
                LogError("Error getting player name", ex);
                return "Unknown";
            }
        }

        public static void Cleanup()
        {
            if (!_isInitialized) return;

            try
            {
                if (_harmony != null)
                {
                    _harmony.UnpatchSelf();
                    _harmony = null;
                }
                _isInitialized = false;
                LogDebug("Cleanup completed");
            }
            catch (Exception ex)
            {
                LogError("Failed to cleanup patches", ex);
            }
        }

        private static void LogError(string message, Exception ex)
        {
            MelonLogger.Error($"{LOG_PREFIX} {message}: {ex.Message}");
            MelonLogger.Error($"{LOG_PREFIX} Stack trace: {ex.StackTrace}");
        }

        private static void LogWarning(string message)
        {
            MelonLogger.Warning($"{LOG_PREFIX} {message}");
        }

        private static void LogDebug(string message)
        {
            MelonLogger.Msg($"{LOG_PREFIX} {message}");
        }
    }
}
