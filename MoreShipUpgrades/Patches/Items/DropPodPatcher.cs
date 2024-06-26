﻿using HarmonyLib;
using MoreShipUpgrades.Misc.Upgrades;
using MoreShipUpgrades.Misc.Util;
using MoreShipUpgrades.UpgradeComponents.OneTimeUpgrades;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreShipUpgrades.Patches.Items
{
    [HarmonyPatch(typeof(ItemDropship))]
    internal static class DropPodPatcher
    {
        internal static List<int> orderedItems;
        [HarmonyPatch(nameof(ItemDropship.Update))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo upgradedTimer = typeof(FasterDropPod).GetMethod(nameof(FasterDropPod.GetUpgradedTimer));
            MethodInfo initialTimer = typeof(FasterDropPod).GetMethod(nameof(FasterDropPod.GetFirstOrderTimer));
            MethodInfo isUpgradeActive = typeof(FasterDropPod).GetMethod(nameof(FasterDropPod.IsUpgradeActive));
            FieldInfo playersManager = typeof(ItemDropship).GetField(nameof(ItemDropship.playersManager), BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo shipHasLander = typeof(StartOfRound).GetField(nameof(StartOfRound.shipHasLanded));

            List<CodeInstruction> codes = new(instructions);
            int index = 0;

            Tools.FindFloat(ref index, ref codes, findValue: 20, addCode: initialTimer, errorMessage: "Couldn't find the 20 value which is used as first buy ship timer");
            Tools.FindFloat(ref index, ref codes, findValue: 40, addCode: upgradedTimer, errorMessage: "Couldn't find the 40 value which is used as ship timer");
            bool found = false;
            for (; index < codes.Count && !found; index++)
            {
                if (codes[index].opcode == OpCodes.Ble_Un)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                codes[index].opcode = OpCodes.Brfalse;
                codes.Insert(index, new CodeInstruction(OpCodes.And));
                codes.Insert(index, new CodeInstruction(OpCodes.Or));
                codes.Insert(index, new CodeInstruction(OpCodes.Not));
                codes.Insert(index, new CodeInstruction(OpCodes.Call, isUpgradeActive));
                codes.Insert(index, new CodeInstruction(OpCodes.Ldfld, shipHasLander));
                codes.Insert(index, new CodeInstruction(OpCodes.Ldfld, playersManager));
                codes.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
                codes.Insert(index, new CodeInstruction(OpCodes.Cgt));
            }
            return codes.AsEnumerable();
        }

        const int MAXIMUM_ALLOWED_DELIVERED_ITEMS = 12;
        [HarmonyPatch(nameof(ItemDropship.ShipLandedAnimationEvent))]
        [HarmonyPrefix]
        static void ShipLandedAnimationEventPrefix(ItemDropship __instance)
        {
            if (!BaseUpgrade.GetActiveUpgrade(FasterDropPod.UPGRADE_NAME)) return;
            while(orderedItems.Count > 0 && __instance.itemsToDeliver.Count < MAXIMUM_ALLOWED_DELIVERED_ITEMS)
            {
                __instance.itemsToDeliver.Add(orderedItems[0]);
                orderedItems.RemoveAt(0);
            }
        }
    }
}
