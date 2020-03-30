using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Dungeonator;
using Random = UnityEngine.Random;
using CustomShrineData = GungeonAPI.ShrineFactory.CustomShrineData;

namespace GungeonAPI
{
    public static class DungeonHandler
    {
        private static bool initialized = false;
        public static bool debugFlow = false;

        public static void Init()
        {
            if (!initialized)
            {
                //RoomFactory.LoadRoomsFromRoomDirectory();
                DungeonHooks.OnPreDungeonGeneration += OnPreDungeonGen;
                DungeonHooks.OnPostDungeonGeneration += OnPostDungeonGen;
                initialized = true;
            }
        }

        public static void OnPreDungeonGen(LoopDungeonGenerator generator, Dungeon dungeon, DungeonFlow flow, int dungeonSeed)
        {
            Tools.Print("Attempting to override floor layout...", "5599FF");
            //CollectDataForAnalysis(flow, dungeon);
            if (flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer)
            {
                if (debugFlow)
                {
                    flow = SampleFlow.CreateDebugFlow(dungeon);
                    generator.AssignFlow(flow);
                }
                else
                {
                    AddCustomRooms(flow);
                }
                Tools.Print("Dungeon name: " + dungeon.name);
                Tools.Print("Override Flow set to: " + flow.name);
            }
            dungeon = null;
        }

        public static void AddCustomRooms(DungeonFlow flow)
        {
            foreach (var room in RoomFactory.rooms.Values)
            {
                try
                {
                    var wRoom = new WeightedRoom()
                    {
                        room = room,
                        additionalPrerequisites = new DungeonPrerequisite[0],
                        weight = 100f
                    };

                    AddRoomToFlowTables(flow, wRoom, DungeonFlowNode.ControlNodeType.ROOM, room.category);
                }
                catch (Exception e)
                {
                    Tools.PrintException(e);
                }
            }
        }

        public static void AddRoomToFlowTables(DungeonFlow flow, WeightedRoom wRoom, DungeonFlowNode.ControlNodeType controlNodeType, PrototypeDungeonRoom.RoomCategory roomCategory)
        {
            if (flow.fallbackRoomTable?.includedRooms != null)
                flow.fallbackRoomTable.includedRooms.Add(wRoom);
            foreach (var node in flow.AllNodes)
            {

                if (node?.overrideRoomTable?.includedRooms == null) continue;

                if (node.nodeType == controlNodeType && node.roomCategory == roomCategory)
                    node.overrideRoomTable.includedRooms.Add(wRoom);
            }
        }

        public static void CollectDataForAnalysis(DungeonFlow flow, Dungeon dungeon)
        {
            try
            {
                var room2 = Tools.sharedAuto2.LoadAsset("Castle_Poop_Room_001") as PrototypeDungeonRoom;
                Tools.LogPropertiesAndFields(room2, "ROOM");
                int i = 0;
                foreach (var placedObject in room2.placedObjects)
                {
                    Tools.Log($"\n----------------Object #{i++}----------------");
                    Tools.LogPropertiesAndFields(placedObject, "PLACED OBJECT");
                    Tools.LogPropertiesAndFields(placedObject.placeableContents, "PLACEABLE CONTENT");
                    Tools.LogPropertiesAndFields(placedObject.placeableContents.variantTiers[0], "VARIANT TIERS");
                }

                Tools.Print("==LAYERS==");
                foreach (var layer in room2.additionalObjectLayers)
                {
                    Tools.LogPropertiesAndFields(layer);
                }
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
            }
            dungeon = null;
        }

        public static void OnPostDungeonGen()
        {
            GameManager.Instance.PrimaryPlayer.OnEnteredCombat += () => Tools.Print("Entered combat");
        }
    }
}
