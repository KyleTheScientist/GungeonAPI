using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
namespace GungeonAPI
{
    public static class StaticReferences
    {
        public static Dictionary<string, AssetBundle> AssetBundles;
        public static Dictionary<string, GenericRoomTable> RoomTables;
        public static SharedInjectionData subShopTable;
        public static Dictionary<string, string> roomTableMap = new Dictionary<string, string>()
        {
            { "special", "basic special rooms (shrines, etc)" },
            { "shop", "Shop Room Table" },
            { "secret", "secret_room_table_01" },

            { "gungeon", "Gungeon_RoomTable" },
            { "castle", "Castle_RoomTable" },
            //{ "mines", "Mines_RoomTable" },
            //{ "catacombs", "Catacomb_RoomTable" },
            //{ "forge", "Forge_RoomTable" },
            //{ "sewer", "Sewer_RoomTable" },
            //{ "cathedral", "Cathedral_RoomTable" },
            //{ "bullet_hell", "BulletHell_RoomTable" },
        };

        public static string[] assetBundleNames = new string[]
        {
            "shared_auto_001",
            "shared_auto_002",
            //"foyer_001",
            //"foyer_002",
            //"foyer_003",
        };

        public static void Init()
        {

            AssetBundles = new Dictionary<string, AssetBundle>();
            foreach (var name in assetBundleNames)
            {
                AssetBundles.Add(name, ResourceManager.LoadAssetBundle(name));
            }

            RoomTables = new Dictionary<string, GenericRoomTable>();
            foreach (var entry in roomTableMap)
            {
                var table = GetAsset<GenericRoomTable>(entry.Value);
                RoomTables.Add(entry.Key, table);
                Tools.Log(table.name);
                foreach (var r in table.includedRooms.elements)
                    Tools.Log($"\t{r.room}");
            }
            Tools.Print("Static references initialized.");
        }

        public static GenericRoomTable GetRoomTable(GlobalDungeonData.ValidTilesets tileset)
        {
            switch (tileset)
            {
                case GlobalDungeonData.ValidTilesets.GUNGEON:
                    return RoomTables["gungeon"];
                case GlobalDungeonData.ValidTilesets.CASTLEGEON:
                    return RoomTables["castle"];
                case GlobalDungeonData.ValidTilesets.MINEGEON:
                    return RoomTables["mines"];
                case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
                    return RoomTables["catacombs"];
                case GlobalDungeonData.ValidTilesets.FORGEGEON:
                    return RoomTables["forge"];
                case GlobalDungeonData.ValidTilesets.SEWERGEON:
                    return RoomTables["sewer"];
                case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
                    return RoomTables["cathedral"];
                case GlobalDungeonData.ValidTilesets.HELLGEON:
                    return RoomTables["bullet_hell"];
                default:
                    return RoomTables["gungeon"];
            }
        }

        public static T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
            T item = null;
            foreach (var bundle in AssetBundles.Values)
            {
                item = bundle.LoadAsset<T>(assetName);
                if (item != null)
                    break;
            }
            return item;
        }

    }
}
