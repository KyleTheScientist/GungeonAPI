using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonAPI
{
    public class GAPIModule : ETGModule
    {
        public static readonly string VERSION = "v0.0.6";

        public override void Exit()
        {
        }

        public override void Init()
        {
        }

        public override void Start()
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
                    Tools.Print($"Debug flow {status}", color);
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
                    Tools.BreakdownComponents(c);
                    Tools.LogPropertiesAndFields(c.GetComponent<SimpleInteractable>());
                });
                        
                ETGModConsole.Commands.AddUnit("roomname", (args) =>
                {
                    var room = GameManager.Instance.PrimaryPlayer.CurrentRoom;
                    Tools.Print(room.GetRoomName());
                });

                ETGModConsole.Commands.AddUnit("hidehitboxes", (args) => HitboxMonitor.DeleteHitboxDisplays());
                ETGModConsole.Commands.AddUnit("showhitboxes", (args) =>
                {
                    foreach(var obj in GameObject.FindObjectsOfType<SpeculativeRigidbody>())
                    {
                        if(obj && obj.sprite && Vector2.Distance(obj.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.sprite.WorldCenter) < 8)
                        {
                            Tools.Log(obj?.name);
                            HitboxMonitor.DisplayHitbox(obj);
                        }
                    }
                });

                Tools.Print($"GungeonAPI {VERSION} loaded.", "FF00FF", true);
            }
            catch (Exception e)
            {
                Tools.Print("Failed to load GungeonAPI", "FF0000", true);
                Tools.PrintException(e);
            }
        }

        public void DumpEnemyDatabase()
        {
            Tools.Print("Dumping enemy database.");
            for(int i = 0; i < EnemyDatabase.Instance.Entries.Count; i++)
            {
                var entry = EnemyDatabase.Instance.Entries[i];
                Tools.Log($"{entry.myGuid}\t{entry.name}\t{i}\t{entry.isNormalEnemy}\t{entry.isInBossTab}", "EnemyDump.txt");
            }
        }
        public static GameObject GetClosestCustomShrineObject()
        {
            Tools.Log("a");
            var player = GameManager.Instance.PrimaryPlayer;
            Tools.Log("b");

            var shrines = GameObject.FindObjectsOfType<ShrineFactory.CustomShrineController>();
            Tools.Log("c");
            float dist = float.MaxValue, d;
            ShrineFactory.CustomShrineController closest = null;
            foreach (var shrine in shrines)
            {
                Tools.Log("d");
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
            Tools.Log("e");
            Tools.Log(closest);
            return closest.gameObject;
        }

        public static GameObject GetClosestShrineObject()
        {
            Tools.Log("a");
            var player = GameManager.Instance.PrimaryPlayer;
            Tools.Log("b");

            var talkers = GameObject.FindObjectsOfType<AdvancedShrineController>();
            Tools.Log("c");
            float dist = float.MaxValue, d;
            AdvancedShrineController closest = null;
            foreach (var talker in talkers)
            {
                Tools.Log("d");
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
            Tools.Log("e");
            Tools.Log(closest);
            return closest.gameObject;
        }
    }
}
