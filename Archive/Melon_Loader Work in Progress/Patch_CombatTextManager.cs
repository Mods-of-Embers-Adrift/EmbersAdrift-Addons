using HarmonyLib;
using SoL.Game.EffectSystem;
using SoL.Networking.Objects;
using SoL.Game.Pooling;

namespace DPSAddon
{
    [HarmonyPatch(typeof(CombatTextManager), nameof(CombatTextManager.InitializeCombatText))]
    public static class Patch_CombatTextManager
    {
        static void Postfix(NetworkEntity entity, EffectApplicationResult ear)
        {
            DpsEventListener.OnEffectApplied(entity, ear);
        }
    }
}
