using MoreShipUpgrades.Misc;
using MoreShipUpgrades.Patches.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace MoreShipUpgrades.UpgradeComponents.Items.FriendlyHoarderBug
{
    internal class FriendlyNest : PhysicsProp
    {
        HoarderBugAI friendly;
        SpawnableEnemyWithRarity hoarder = null;

        public override void Start()
        {
            base.Start();

            SetupItemAttributes();

            var list = RoundManager.Instance.currentLevel.Enemies;
            hoarder = list.Find((SpawnableEnemyWithRarity x) => x.enemyType.enemyName.Equals("Hoarding bug"));
        }

        private void SetupItemAttributes()
        {
            grabbable = true;
        }

        public override void GrabItem()
        {
            base.GrabItem();

            if (friendly != null)
            {
                if(friendly.heldItem != null)
                {
                    friendly.DropItemAndCallDropRPC(friendly.heldItem.itemGrabbableObject.GetComponent<NetworkObject>());
                }
                RoundManager.Instance.DespawnEnemyGameObject(friendly.thisNetworkObject);
                //friendly.KillEnemy(true);
                friendly = null;
            }
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
            if (!GameNetworkManager.Instance.localPlayerController.isInsideFactory) return;
            if (friendly != null) return;

            for (int i = 0; i < RoundManager.Instance.currentLevel.Enemies.Count; i++)
            {
                Debug.Log($"{RoundManager.Instance.currentLevel.Enemies[i].enemyType.enemyName}");
            }

            Vector3 navMeshPosition = RoundManager.Instance.GetNavMeshPosition(transform.position, default, 5f, -1);
            friendly = ((GameObject)RoundManager.Instance.SpawnEnemyGameObject(navMeshPosition, 0f, 1, hoarder.enemyType)).GetComponent<HoarderBugAI>();
            if (friendly != null)
            {
                var scanNode = friendly.gameObject.GetComponentInChildren<ScanNodeProperties>();
                var enemyType = Instantiate(friendly.enemyType);

                friendly.enemyType = enemyType;
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
                    if (item.material.name.Contains("HoarderBugColor"))
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

        /*public override void Update()
        {
            if(Keyboard.current[Key.LeftArrow].wasPressedThisFrame)
            {
                itemProperties.positionOffset.x += Keyboard.current[Key.LeftShift].IsPressed() ? -0.1f : 0.1f;
            }
            else if(Keyboard.current[Key.UpArrow].wasPressedThisFrame)
            {
                itemProperties.positionOffset.y += Keyboard.current[Key.LeftShift].IsPressed() ? -0.1f : 0.1f;
            }
            else if (Keyboard.current[Key.RightArrow].wasPressedThisFrame)
            {
                itemProperties.positionOffset.z += Keyboard.current[Key.LeftShift].IsPressed() ? -0.1f : 0.1f;
            }

            Debug.Log($"{itemProperties.positionOffset}");
        }*/
    }
}
