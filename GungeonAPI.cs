using Alexandria.DungeonAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GungeonAPI
{
    public static class GungeonAPI
    {
        public static void Init()
        {
            ShrineFactory.Init();

            var dirs = Directory.GetDirectories(BepInEx.Paths.PluginPath, "CustomRoomData", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                RoomFactory.LoadRoomsFromRoomDirectory("CustomRoomsMod", dir);
            }

        }
    }
}
