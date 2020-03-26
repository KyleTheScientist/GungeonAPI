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
        private static readonly int roomWidth = 14, roomHeight = 14;
        private static Dictionary<string, PrototypeDungeonRoom> registeredShrineRooms = new Dictionary<string, PrototypeDungeonRoom>();

        public static void Init()
        {
            if (!initialized)
            {
                DungeonHooks.OnPreDungeonGeneration += OnPreDungeonGen;
                DungeonHooks.OnPostDungeonGeneration += OnPostDungeonGen;
                initialized = true;
            }
        }

        public static void RegisterDefaultShrineRoom(GameObject shrine, string ID, Vector2 offset)
        {
            Vector2 position = new Vector2(roomWidth / 2 + offset.x, roomHeight / 2 + offset.y);

            var protoroom = RoomFactory.CreateEmptyRoom(roomWidth, roomHeight);
            protoroom.placedObjectPositions.Add(position);
            protoroom.category = PrototypeDungeonRoom.RoomCategory.SPECIAL;
            protoroom.subCategorySpecial = PrototypeDungeonRoom.RoomSpecialSubCategory.UNSPECIFIED_SPECIAL;

            DungeonPrerequisite[] emptyReqs = new DungeonPrerequisite[0];
            protoroom.placedObjects.Add(new PrototypePlacedObjectData()
            {
                contentsBasePosition = position,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = emptyReqs,
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = new DungeonPlaceable()
                {
                    width = 2,
                    height = 2,
                    respectsEncounterableDifferentiator = true,
                    variantTiers = new List<DungeonPlaceableVariant>()
                    {
                        new DungeonPlaceableVariant()
                        {
                            percentChance = 1,
                            nonDatabasePlaceable = shrine,
                            prerequisites = emptyReqs,
                            materialRequirements= new DungeonPlaceableRoomMaterialRequirement[0]
                        }
                    }
                }
            });
            registeredShrineRooms.Add(ID, protoroom);
        }

        public static void OnPreDungeonGen(LoopDungeonGenerator generator, Dungeon dungeon, DungeonFlow flow, int dungeonSeed)
        {
            Tools.Print("Attempting to override floor layout...", "5599FF");
            CollectDataForAnalysis(flow, dungeon);
            if (flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer)
            {
                //flow = SampleFlow.CreateRoomTypeSampleFlow(dungeon);
                //flow = SampleFlow.CreateMazeFlow(dungeon);
                //flow = SampleFlow.CreateEntranceExitFlow(dungeon);
                //generator.AssignFlow(flow);
                Tools.Print("Dungeon name: " + dungeon.name);

                //var hub = new DungeonFlowNode(flow) { overrideExactRoom = RoomFactory.CreateEmptyRoom() };
                //flow.AddNodeToFlow(hub, flow.FirstNode);
                //flow.AddNodeToFlow(new DungeonFlowNode(flow) { overrideExactRoom = RoomFactory.Build("resource/rooms/diag.png") }, hub);
                ////foreach (var shrineID in registeredShrineRooms.Keys)
                //{
                //    //registeredShrineRooms[shrineID].overrideRoomVisualType = RoomFactory.GetStyleValue(dungeon.name, shrineID);
                //    //flow.AddNodeToFlow(new DungeonFlowNode(flow) { overrideExactRoom = registeredShrineRooms[shrineID] }, flow.FirstNode);
                //}
                Tools.Print("Override Flow set to: " + flow.name);
            }
            dungeon = null;
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
