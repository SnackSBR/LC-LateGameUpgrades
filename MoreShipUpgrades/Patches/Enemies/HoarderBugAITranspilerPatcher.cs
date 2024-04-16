using HarmonyLib;
using MoreShipUpgrades.Misc;
using MoreShipUpgrades.Misc.Util;
using MoreShipUpgrades.UpgradeComponents.TierUpgrades;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreShipUpgrades.Patches.Enemies
{
    [HarmonyPatch(typeof(HoarderBugAI))]
    internal static class HoarderBugAITranspilerPatcher
    {
        [HarmonyPatch(nameof(HoarderBugAI.DoAIInterval))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ChangeSearchRangeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo upgradedRange = typeof(FriendshipBooster).GetMethod(nameof(FriendshipBooster.GetUpgradedRange));
            List<CodeInstruction> codes = new(instructions);
            int index = 0;

            Tools.FindInteger(ref index, ref codes, findValue: 40, skip: true, errorMessage: "Couldn't skip the 40 value which is used as HasLineOfSightToPosition");
            Tools.FindInteger(ref index, ref codes, findValue: 40, addCode: upgradedRange, errorMessage: "Couldn't find the 40 value which is used as range");
            return codes.AsEnumerable();
        }

        [HarmonyPatch(nameof(HoarderBugAI.Update))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ChangeSpeedTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo upgradedRange = typeof(FriendshipBooster).GetMethod(nameof(FriendshipBooster.GetUpgradedSpeed));
            List<CodeInstruction> codes = new(instructions);
            int index = 0;

            Tools.FindFloat(ref index, ref codes, findValue: 6, addCode: upgradedRange, errorMessage: "Couldn't find the 6 value which is used as speed");
            Tools.FindFloat(ref index, ref codes, findValue: 6, addCode: upgradedRange, errorMessage: "Couldn't find the 6 value which is used as speed");
            return codes.AsEnumerable();
        }

        [HarmonyPatch(nameof(HoarderBugAI.KillEnemy))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> KillAndDestroyTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            int insertionIndex = -1;
            for (int i = 0; i < code.Count - 1; i++)
            {
                if (code[i].opcode == OpCodes.Ldc_I4_0 && code[i + 1].opcode == OpCodes.Call)
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex != -1)
            {
                code[insertionIndex] = new(OpCodes.Ldarg_1);
            }

            return code;
        }
    }
}
