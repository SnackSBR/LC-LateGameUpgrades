using MoreShipUpgrades.Managers;
using MoreShipUpgrades.Misc;
using MoreShipUpgrades.Misc.Upgrades;
using MoreShipUpgrades.Misc.Util;
using MoreShipUpgrades.UpgradeComponents.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoreShipUpgrades.UpgradeComponents.TierUpgrades
{
    class FriendshipBooster : TierUpgrade, IUpgradeWorldBuilding
    {
        internal const string UPGRADE_NAME = "Friendship Booster";
        internal const string WORLD_BUILDING_TEXT = "\n\nTalk no jutsu\n\n";

        internal override void Start()
        {
            upgradeName = UpgradeBus.Instance.PluginConfiguration.OVERRIDE_UPGRADE_NAMES ? UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_OVERRIDE_NAME : UPGRADE_NAME;
            base.Start();
        }

        public string GetWorldBuildingText(bool shareStatus = false)
        {
            return WORLD_BUILDING_TEXT;
        }

        public static string GetFriendshipBoosterInfo(int level, int price)
        {
            switch (level)
            {
                case 1: return string.Format(AssetBundleHandler.GetInfoFromJSON("Friendship Booster1"), level, price, UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_WAIT.Value);
                case 2: return string.Format(AssetBundleHandler.GetInfoFromJSON("Friendship Booster2"), level, price, UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_SPEED.Value);
                case 3: return string.Format(AssetBundleHandler.GetInfoFromJSON("Friendship Booster3"), level, price, UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_RANGE.Value);
            }
            return "";
        }

        public override string GetDisplayInfo(int initialPrice = -1, int maxLevels = -1, int[] incrementalPrices = null)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(GetFriendshipBoosterInfo(1, initialPrice));
            for (int i = 0; i < maxLevels; i++)
                stringBuilder.Append(GetFriendshipBoosterInfo(i + 2, incrementalPrices[i]));
            return stringBuilder.ToString();
        }

        internal override bool CanInitializeOnStart()
        {
            return UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_PRICE.Value <= 0 &&
                UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_PRICE2.Value <= 0 &&
                UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_PRICE3.Value <= 0;
        }

        public static int GetUpgradedRange(int defaultValue)
        {
            if (!UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_ENABLED.Value) return defaultValue;
            if (!GetActiveUpgrade(UPGRADE_NAME)) return defaultValue;
            if (GetUpgradeLevel(UPGRADE_NAME) < 2) return defaultValue;
            return defaultValue + UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_RANGE.Value;
        }

        public static float GetUpgradedSpeed(float defaultValue)
        {
            if (!UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_ENABLED.Value) return defaultValue;
            if (!GetActiveUpgrade(UPGRADE_NAME)) return defaultValue;
            if (GetUpgradeLevel(UPGRADE_NAME) < 1) return defaultValue;
            return defaultValue + UpgradeBus.Instance.PluginConfiguration.FRIENDSHIP_BOOSTER_SPEED.Value;
        }
    }
}
