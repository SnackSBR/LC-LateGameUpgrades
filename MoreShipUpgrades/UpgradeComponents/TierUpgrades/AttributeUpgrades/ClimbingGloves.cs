﻿using MoreShipUpgrades.Managers;
using MoreShipUpgrades.Misc;
using MoreShipUpgrades.Misc.Upgrades;
using MoreShipUpgrades.UpgradeComponents.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoreShipUpgrades.UpgradeComponents.TierUpgrades.AttributeUpgrades
{
    internal class ClimbingGloves : GameAttributeTierUpgrade, IPlayerSync
    {
        internal const string UPGRADE_NAME = "Climbing Gloves";

        void Awake()
        {
            upgradeName = UPGRADE_NAME;
            logger = new LguLogger(UPGRADE_NAME);
            changingAttribute = GameAttribute.PLAYER_CLIMB_SPEED;
            initialValue = UpgradeBus.Instance.PluginConfiguration.INITIAL_CLIMBING_SPEED_BOOST.Value;
            incrementalValue = UpgradeBus.Instance.PluginConfiguration.INCREMENTAL_CLIMBING_SPEED_BOOST.Value;
        }

        public override string GetDisplayInfo(int initialPrice = -1, int maxLevels = -1, int[] incrementalPrices = null)
        {
            System.Func<int, float> infoFunction = level => UpgradeBus.Instance.PluginConfiguration.INITIAL_CLIMBING_SPEED_BOOST.Value + level * UpgradeBus.Instance.PluginConfiguration.INCREMENTAL_CLIMBING_SPEED_BOOST.Value;
            string infoFormat = "LVL {0} - ${1} - Increases the speed of climbing ladders by {2} units.\n";
            return Tools.GenerateInfoForUpgrade(infoFormat, initialPrice, incrementalPrices, infoFunction);
        }
    }
}
