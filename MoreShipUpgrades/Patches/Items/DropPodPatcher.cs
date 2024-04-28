﻿using HarmonyLib;
using MoreShipUpgrades.Misc.Upgrades;
using MoreShipUpgrades.Misc.Util;
using MoreShipUpgrades.UpgradeComponents.OneTimeUpgrades;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            List<CodeInstruction> codes = new(instructions);
            int index = 0;

            Tools.FindFloat(ref index, ref codes, findValue: 20, addCode: initialTimer, errorMessage: "Couldn't find the 20 value which is used as first buy ship timer");
            Tools.FindFloat(ref index, ref codes, findValue: 40, addCode: upgradedTimer, errorMessage: "Couldn't find the 40 value which is used as ship timer");
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
