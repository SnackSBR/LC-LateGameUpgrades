using DunGen;
using HarmonyLib;
using MoreShipUpgrades.Managers;
using MoreShipUpgrades.Misc;
using MoreShipUpgrades.Patches.Enemies;
using MoreShipUpgrades.UpgradeComponents.Commands;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MoreShipUpgrades.Patches.RoundComponents
{
    [HarmonyPatch(typeof(RoundManager))]
    internal static class RoundManagerPatcher
    {
        static int previousDaysDeadline = TimeOfDay.Instance.daysUntilDeadline;
        static int DEFAULT_DAYS_DEADLINE = 4;
        static bool savedPrevious = false;
        static LguLogger logger = new LguLogger(nameof(RoundManagerPatcher));

        /// <summary>
        /// Shoutout to ustaalon (https://github.com/ustaalon) for pointing out the issue when increasing the amount of days before deadline affecting
        /// the enemy spawning
        /// </summary>
        [HarmonyPatch(nameof(RoundManager.PlotOutEnemiesForNextHour))]
        [HarmonyPatch(nameof(RoundManager.AdvanceHourAndSpawnNewBatchOfEnemies))]
        [HarmonyPrefix]
        static void ChangeDaysForEnemySpawns()
        {
            if (!UpgradeBus.Instance.PluginConfiguration.EXTEND_DEADLINE_ENABLED.Value) return; //  Don't bother changing something if we never touch it
            if (TimeOfDay.Instance.daysUntilDeadline < DEFAULT_DAYS_DEADLINE) return; // Either it's already fine or some other mod already changed the value to be acceptable
            logger.LogDebug("Changing deadline to allow spawning enemies.");
            previousDaysDeadline = TimeOfDay.Instance.daysUntilDeadline;
            TimeOfDay.Instance.daysUntilDeadline %= DEFAULT_DAYS_DEADLINE;
            savedPrevious = true;
        }

        [HarmonyPatch(nameof(RoundManager.PlotOutEnemiesForNextHour))]
        [HarmonyPatch(nameof(RoundManager.AdvanceHourAndSpawnNewBatchOfEnemies))]
        [HarmonyPostfix]
        static void UndoChangeDaysForEnemySpawns()
        {
            if (!UpgradeBus.Instance.PluginConfiguration.EXTEND_DEADLINE_ENABLED.Value) return; //  Don't bother changing something if we never touch it
            if (!savedPrevious) return;
            logger.LogDebug("Changing back the deadline...");
            TimeOfDay.Instance.daysUntilDeadline = previousDaysDeadline;
            savedPrevious = false;
        }

        [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
        [HarmonyPostfix]
        static void DespawnPropsAtEndOfRoundPostfix()
        {
            if (!UpgradeBus.Instance.PluginConfiguration.SCRAP_INSURANCE_ENABLED.Value) return;
            ScrapInsurance.TurnOffScrapInsurance();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        static void SpawnFriendlyHoarderBug()
        {
            EntranceTeleport mainDoor = Object.FindObjectsOfType<EntranceTeleport>().Where(obj => obj.gameObject.transform.position.y <= -170).First();
            Tile tile = null;
            Vector3 navMeshPosition = Vector3.zero;
            foreach (var item in RoundManager.Instance.dungeonGenerator.Generator.CurrentDungeon.MainPathTiles)
            {
                if (item.name.Contains("StartRoom"))
                {
                    tile = item;
                    Bounds bounds = tile.transform.TransformBounds(tile.TileBoundsOverride);
                    navMeshPosition = RoundManager.Instance.GetNavMeshPosition(bounds.m_Center, default, 5f, -1);
                    break;
                }
            }

            if (tile && Tools.SpawnMob("Hoarding bug", navMeshPosition, 1))
            {
                HoarderBugAI friendly = Object.FindObjectsOfType<HoarderBugAI>().First();

                if (friendly != null)
                {
                    var scanNode = friendly.gameObject.GetComponentInChildren<ScanNodeProperties>();
                    var enemyType = friendly.enemyType;
                    var navMesh = friendly.gameObject.GetComponentInChildren<NavMeshAgent>();

                    friendly.name = friendly.name.Replace("Hoarder", "FriendlyHoarder");
                    enemyType.enemyName = "Friendly Hoarding Bug";
                    scanNode.headerText = "Friendly Hoarding Bug";
                    scanNode.subText = "He's here to help";
                    scanNode.nodeType = 0;
                    friendly.nestPosition = navMeshPosition;
                    friendly.choseNestPosition = true;

                    Texture2D texture = AssetBundleHandler.GetGenericAsset<Texture2D>("Friendly Hoarder Bug Texture");

                    foreach (var item in friendly.GetComponentsInChildren<MeshRenderer>())
                    {
                        if(item.material.name.Contains("HoarderBugColor"))
                        {
                            item.gameObject.SetActive(false);
                            item.material.SetTexture("_MainTex", texture);
                            item.material.mainTexture = texture;
                            item.gameObject.SetActive(true);
                        }
                    }

                    foreach (var item in friendly.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        if (item.material.name.Contains("HoarderBugColor"))
                        {
                            item.gameObject.SetActive(false);
                            item.material.SetTexture("_MainTex", texture);
                            item.material.mainTexture = texture;
                            item.gameObject.SetActive(true);
                        }
                    }

                    HoarderBugAIPatcher.FriendlyID = friendly.NetworkObjectId;
                }
            }
        }
    }
}
