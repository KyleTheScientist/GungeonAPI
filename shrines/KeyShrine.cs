using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CustomShrineController = GungeonAPI.ShrineFactory.CustomShrineController;
namespace GungeonAPI
{
    public static class KeyShrine
    {

        public static void Add()
        {
            ShrineFactory sf = new ShrineFactory()
            {
                name = "Key Shrine",
                modID = "kts",
                text = "A shrine representing the key to happiness.",
                spritePath = "GungeonAPI/resource/shrine_heart_key.png",
                room = RoomFactory.BuildFromResource("GungeonAPI/resource/rooms/KeyShrine.room").room,
                acceptText = "Offer a heart to unlock your potential",
                declineText = "Walk away",
                OnAccept = Accept,
                OnDecline = null,
                CanUse = CanUse,
                offset = new Vector3(10, 0, 0),
                talkPointOffset = new Vector3(0, 3, 0),
                isToggle = false,
                isBreachShrine = false
            };
            //register shrine
            sf.Build();
        }

        public static bool CanUse(PlayerController player, GameObject shrine)
        {
            return shrine.GetComponent<CustomShrineController>().numUses == 0;
        }

        public static void Accept(PlayerController player, GameObject shrine)
        {
            float maxHealth = player.healthHaver.GetMaxHealth();
            if (maxHealth > 1)
            {
                player.healthHaver.SetHealthMaximum(maxHealth - 1);
                player.carriedConsumables.KeyBullets += 2;
                shrine.GetComponent<CustomShrineController>().numUses++;
                shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
                AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
            }
        }
    }
}
