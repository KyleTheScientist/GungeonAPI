using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using Alexandria.DungeonAPI;
using Alexandria.Misc;

namespace GungeonAPI
{
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInDependency("alexandria.etgmod.alexandria")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class GAPIModule : BaseUnityPlugin
    {
        public const string GUID = "kyle.etg.gapi";
        public const string NAME = "Custom Rooms";
        public const string VERSION = "1.0.2";
        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }
        public void GMStart(GameManager g)
        {
            try
            {
                GungeonAPI.Init();
                KeyShrine.Add();

                //Enable the debug flow
                ETGModConsole.Commands.AddUnit("debugflow", (args) =>
                {
                    DungeonHandler.debugFlow = !DungeonHandler.debugFlow;
                    string status = DungeonHandler.debugFlow ? "enabled" : "disabled";
                    string color = DungeonHandler.debugFlow ? "00FF00" : "FF0000";
                    DebugUtility.Print($"Debug flow {status}", color, true);
                });

                //This is useful for figuring out where you want your shrine to go in the breach
                ETGModConsole.Commands.AddUnit("getpos", (args) =>
                {
                    ETGModConsole.Log("Player position: " + GameManager.Instance.PrimaryPlayer.transform.position);
                    ETGModConsole.Log("Player center: " + GameManager.Instance.PrimaryPlayer.sprite.WorldCenter);
                });

                ETGModConsole.Commands.AddUnit("dissectshrine", (args) =>
                {
                    var c = GetClosestCustomShrineObject();
                    DebugUtility.BreakdownComponents(c);
                    DebugUtility.LogPropertiesAndFields(c.GetComponent<SimpleInteractable>());
                });

                ETGModConsole.Commands.AddUnit("roomname", (args) =>
                {
                    var room = GameManager.Instance.PrimaryPlayer.CurrentRoom;
                    DebugUtility.Print(room.GetRoomName());
                });

                ETGModConsole.Commands.AddUnit("hidehitboxes", (args) => HitboxMonitor.DeleteHitboxDisplays());
                ETGModConsole.Commands.AddUnit("showhitboxes", (args) =>
                {
                    foreach (var obj in GameObject.FindObjectsOfType<SpeculativeRigidbody>())
                    {
                        if (obj && obj.sprite && Vector2.Distance(obj.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter) < 8)
                        {
                            DebugUtility.Log(obj?.name);
                            HitboxMonitor.DisplayHitbox(obj);
                        }
                    }
                });

                DebugUtility.Print($"Custom Rooms {VERSION} loaded.", "FF00FF", true);
            }
            catch (Exception e)
            {
                DebugUtility.Print("Failed to load Custom Rooms Mod", "FF0000", true);
                DebugUtility.PrintException(e);
            }
        }

        public void DumpEnemyDatabase()
        {
            DebugUtility.Print("Dumping enemy database.");
            for (int i = 0; i < EnemyDatabase.Instance.Entries.Count; i++)
            {
                var entry = EnemyDatabase.Instance.Entries[i];
                DebugUtility.Log($"{entry.myGuid}\t{entry.name}\t{i}\t{entry.isNormalEnemy}\t{entry.isInBossTab}", "EnemyDump.txt");
            }
        }
        public static GameObject GetClosestCustomShrineObject()
        {
            DebugUtility.Log("a");
            var player = GameManager.Instance.PrimaryPlayer;
            DebugUtility.Log("b");

            var shrines = GameObject.FindObjectsOfType<ShrineFactory.CustomShrineController>();
            DebugUtility.Log("c");
            float dist = float.MaxValue, d;
            ShrineFactory.CustomShrineController closest = null;
            foreach (var shrine in shrines)
            {
                DebugUtility.Log("d");
                try
                {
                    d = Vector2.Distance(shrine.sprite.WorldCenter, player.sprite.WorldCenter);
                    if (shrine && d < dist)
                    {
                        closest = shrine;
                        dist = d;
                    }
                }
                catch { }
            }
            DebugUtility.Log("e");
            DebugUtility.Log(closest);
            return closest.gameObject;
        }

        public static GameObject GetClosestShrineObject()
        {
            DebugUtility.Log("a");
            var player = GameManager.Instance.PrimaryPlayer;
            DebugUtility.Log("b");

            var talkers = GameObject.FindObjectsOfType<AdvancedShrineController>();
            DebugUtility.Log("c");
            float dist = float.MaxValue, d;
            AdvancedShrineController closest = null;
            foreach (var talker in talkers)
            {
                DebugUtility.Log("d");
                try
                {
                    d = Vector2.Distance(talker.sprite.WorldCenter, player.sprite.WorldCenter);
                    if (talker && d < dist)
                    {
                        closest = talker;
                        dist = d;
                    }
                }
                catch { }
            }
            DebugUtility.Log("e");
            DebugUtility.Log(closest);
            return closest.gameObject;
        }
    }
}
