﻿using HarmonyLib;
using MoreShipUpgrades.Managers;
using MoreShipUpgrades.Misc.Upgrades;
using MoreShipUpgrades.UpgradeComponents.OneTimeUpgrades;

namespace MoreShipUpgrades.Patches.RoundComponents
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal static class TimeOfDayPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
        static void GenerateNewSales(TimeOfDay __instance)
        {
            if (UpgradeBus.Instance.PluginConfiguration.SHARED_UPGRADES.Value && (__instance.IsHost || __instance.IsServer))
            {
                int seed = UnityEngine.Random.Range(0, 999999);
                LguStore.Instance.GenerateSalesClientRpc(seed);
            }
            else
            {
                UpgradeBus.Instance.GenerateSales();
            }
        }

        [HarmonyPatch(nameof(TimeOfDay.SetBuyingRateForDay))]
        [HarmonyPostfix]
        private static void SetBuyingRateForDayPatch()
        {
            if (!UpgradeBus.Instance.PluginConfiguration.SIGURD_ENABLED.Value) return;
            if (!BaseUpgrade.GetActiveUpgrade(Sigurd.UPGRADE_NAME)) return;
            if (TimeOfDay.Instance.daysUntilDeadline == 0) return;

            System.Random random = new(StartOfRound.Instance.randomMapSeed);
            if (random.Next(0, 100) < 100)
                StartOfRound.Instance.companyBuyingRate += (UpgradeBus.Instance.PluginConfiguration.SIGURD_PERCENT.Value / 100);
        }
    }

}
