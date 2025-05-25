using System;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using MelonLoader;
using SoL.Game.Messages;
using SoL.Game.UI.Chat;
using SoL.Game;

namespace DPSAddon
{
    public static class ManualInjector
    {
        private const string HARMONY_ID = "DPSAddon.ManualPatch";
        private static HarmonyLib.Harmony _harmony;
        private static bool _isInitialized;

        public static bool Inject()
        {
            if (_isInitialized)
            {
                MelonLogger.Msg("[DPS Patch] Already initialized");
                return true;
            }

            try
            {
                MelonLogger.Msg("[DPS Patch] Attempting chat message patch...");
                _harmony = new HarmonyLib.Harmony(HARMONY_ID);

                var targetMethod = FindAddToQueueMethod();
                if (targetMethod == null)
                {
                    MelonLogger.Warning("[DPS Patch] Could not find AddToQueue method");
                    return false;
                }

                var postfix = typeof(ManualInjector).GetMethod("AddToQueue_Postfix", BindingFlags.Static | BindingFlags.NonPublic);
                if (postfix == null)
                {
                    MelonLogger.Warning("[DPS Patch] Could not find postfix method");
                    return false;
                }

                _harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
                MelonLogger.Msg("[DPS Patch] Successfully patched AddToQueue");
                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[DPS Patch] Failed to initialize injector: {ex.Message}");
                MelonLogger.Error($"[DPS Patch] Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private static MethodInfo FindAddToQueueMethod()
        {
            try
            {
                var messageManagerType = typeof(MessageManager);
                var chatQueueProperty = messageManagerType.GetProperty("ChatQueue");
                if (chatQueueProperty != null)
                {
                    var queueType = chatQueueProperty.PropertyType;
                    return queueType.GetMethod("AddToQueue", new Type[] { typeof(MessageType), typeof(string) });
                }

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name.StartsWith("SoL"))
                    {
                        foreach (var type in assembly.GetTypes())
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
                MelonLogger.Error($"[DPS Patch] Error finding AddToQueue method: {ex.Message}");
                return null;
            }
        }

        private static void AddToQueue_Postfix(MessageType type, string content)
        {
            try
            {
                // Only process relevant combat messages
                if (type != MessageType.MyCombatOut && type != MessageType.MyCombatIn) return;

                // STRIP RICH TEXT TAGS before parsing!
                string strippedContent = DPSTracker.StripRichTextTags(content);

                MelonLogger.Msg($"[DPS Patch] Processing combat message: {strippedContent} | Type: {type}");

                string playerName = LocalPlayer.GameEntity?.CharacterData?.Name?.Value;
                if (string.IsNullOrEmpty(playerName))
                {
                    MelonLogger.Msg("[DPS Patch] Player name could not be determined, skipping.");
                    return;
                }

                // Outgoing (damage or healing others/self)
                if (type == MessageType.MyCombatOut)
                {
                    if (!strippedContent.Contains("Your")) // Now matches plain text after stripping tags
                    {
                        MelonLogger.Msg("[DPS Patch] Outgoing message does not contain 'Your', skipping.");
                        return;
                    }

                    if (strippedContent.Contains("HITS"))
                    {
                        MelonLogger.Msg($"[DPS Patch] Checking for outgoing damage in: {strippedContent}");
                        var damageMatch = Regex.Match(strippedContent, @"for.*?(\d+)\s*health");
                        if (damageMatch.Success && int.TryParse(damageMatch.Groups[1].Value, out int damage))
                        {
                            MelonLogger.Msg($"[DPS Patch] Recording outgoing damage: {playerName} -> {damage}");
                            DPSTracker.RecordDamage(playerName, damage);
                        }
                        else
                        {
                            MelonLogger.Msg($"[DPS Patch] Outgoing damage pattern not matched for: {strippedContent}");
                        }
                    }
                    else if (strippedContent.Contains("restores"))
                    {
                        MelonLogger.Msg($"[DPS Patch] Checking for outgoing healing in: {strippedContent}");
                        var otherHealMatch = Regex.Match(strippedContent, @"restores\s*(\d+)\s*of ([^']+)'s health");
                        var selfHealMatch = Regex.Match(strippedContent, @"restores\s*(\d+)\s*of your health", RegexOptions.IgnoreCase);

                        if (otherHealMatch.Success && int.TryParse(otherHealMatch.Groups[1].Value, out int amount))
                        {
                            string target = otherHealMatch.Groups[2].Value.Trim();
                            MelonLogger.Msg($"[DPS Patch] Recording outgoing healing: {playerName} heals {target} for {amount}");
                            DPSTracker.RecordHealing(target, amount);
                        }
                        else if (selfHealMatch.Success && int.TryParse(selfHealMatch.Groups[1].Value, out int selfAmount))
                        {
                            MelonLogger.Msg($"[DPS Patch] Recording outgoing self-healing: {playerName} for {selfAmount}");
                            DPSTracker.RecordHealing(playerName, selfAmount);
                        }
                        else
                        {
                            MelonLogger.Msg($"[DPS Patch] Outgoing healing pattern not matched for: {strippedContent}");
                        }
                    }
                }
                // Incoming (receiving healing)
                else if (type == MessageType.MyCombatIn)
                {
                    if (strippedContent.Contains("restores") && strippedContent.Contains("of your health"))
                    {
                        MelonLogger.Msg($"[DPS Patch] Checking for incoming healing in: {strippedContent}");
                        var healMatch = Regex.Match(strippedContent, @"restores\s*(\d+)\s*of your health", RegexOptions.IgnoreCase);
                        if (healMatch.Success && int.TryParse(healMatch.Groups[1].Value, out int healing))
                        {
                            MelonLogger.Msg($"[DPS Patch] Recording incoming healing: {playerName} -> {healing}");
                            DPSTracker.RecordHealing(playerName, healing);
                        }
                        else
                        {
                            MelonLogger.Msg($"[DPS Patch] Incoming healing pattern not matched for: {strippedContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[DPS Patch] Error processing chat message: {ex.Message}");
                MelonLogger.Error($"[DPS Patch] Stack trace: {ex.StackTrace}");
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
                MelonLogger.Msg("[DPS Patch] Cleanup completed");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[DPS Patch] Failed to cleanup patches: {ex.Message}");
            }
        }
    }
}