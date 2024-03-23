using HarmonyLib;
using MoreShipUpgrades.Managers;
using UnityEngine;

namespace MoreShipUpgrades.Patches.Enemies
{
    [HarmonyPatch(typeof(HoarderBugAI))]
    internal static class HoarderBugAIPatcher
    {
        public static ulong FriendlyID = 0;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HoarderBugAI.IsHoarderBugAngry))]
        private static void MakeHoarderBugSwarmAngry(ref bool __result)
        {
            if (ContractManager.Instance.contractType != "exterminator") return;

            if (ContractManager.Instance.contractLevel == RoundManager.Instance.currentLevel.PlanetName)
            {
                __result = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HoarderBugAI.IsHoarderBugAngry))]
        private static void MakeHoarderBugFriendly(ref bool __result, ref HoarderBugAI __instance)
        {
            if (FriendlyID != __instance.NetworkObject.NetworkObjectId) return;

            __result = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(HoarderBugAI.LateUpdate))]
        private static void DontLookAtThePlayer(ref HoarderBugAI __instance)
        {
            if (FriendlyID != __instance.NetworkObject.NetworkObjectId) return;

            __instance.watchingPlayer = null;
            __instance.angryAtPlayer = null;
            __instance.angryTimer = 0f;
            __instance.timeSinceSeeingAPlayer = 25f;
            __instance.detectPlayersInterval = 25f;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(HoarderBugAI.DetectAndLookAtPlayers))]
        private static bool DontDetectThePlayer(ref HoarderBugAI __instance)
        {
            if (FriendlyID != __instance.NetworkObject.NetworkObjectId) return true;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(HoarderBugAI.DetectNoise))]
        private static bool DontDetectNoise(ref HoarderBugAI __instance)
        {
            if (FriendlyID != __instance.NetworkObject.NetworkObjectId) return true;

            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(HoarderBugAI.OnCollideWithPlayer))]
        private static bool DontCollide(ref HoarderBugAI __instance)
        {
            if (FriendlyID != __instance.NetworkObject.NetworkObjectId) return true;

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HoarderBugAI.DoAIInterval))]
        private static void WaitLess(ref HoarderBugAI __instance)
        {
            if (FriendlyID != __instance.NetworkObject.NetworkObjectId) return;

            if(__instance.waitingAtNest && __instance.waitingAtNestTimer > 5f)
            {
                __instance.waitingAtNestTimer = 5f;
            }
        }
    }

}
