using MelonLoader;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CraftingLimitMod
{
    public class Main : MelonMod
    {
        public const int NEW_MAX = 1000;

        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll(typeof(CraftingLimitPatches));
        }
    }

    [HarmonyPatch]
    public static class CraftingLimitPatches
    {
        // Patch AmountCraftable
        [HarmonyPatch(typeof(SoL.Game.UI.Crafting.CraftingUI), "AmountCraftable")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> AmountCraftable_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                // Replace 'int num2 = 99;' (ldc.i4.s 99) with 'int num2 = 1000;'
                if (instr.opcode == OpCodes.Ldc_I4_S && (sbyte)instr.operand == 99)
                {
                    instr.opcode = OpCodes.Ldc_I4;
                    instr.operand = Main.NEW_MAX;
                }
                yield return instr;
            }
        }

        // Patch OnIncrementCountClicked
        [HarmonyPatch(typeof(SoL.Game.UI.Crafting.CraftingUI), "OnIncrementCountClicked")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnIncrementCountClicked_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                // Replace 'this.m_amountToCraft < 99' (ldc.i4.s 99) with 'this.m_amountToCraft < 1000'
                if (instr.opcode == OpCodes.Ldc_I4_S && (sbyte)instr.operand == 99)
                {
                    instr.opcode = OpCodes.Ldc_I4;
                    instr.operand = Main.NEW_MAX;
                }
                yield return instr;
            }
        }
    }
}