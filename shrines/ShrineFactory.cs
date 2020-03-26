using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace GungeonAPI
{
    public class ShrineFactory
    {
        public string
            name,
            modID,
            spritePath,
            text, acceptText, declineText;
        public Action<PlayerController>
            OnAccept,
            OnDecline;
        public Vector3 talkPointOffset;
        public Vector3 offset = new Vector3(43.8f, 42.4f, 42.9f);
        public IntVector2 colliderOffset, colliderSize;
        public bool
            isToggle,
            usesCustomColliderOffsetAndSize;
        public Type interactableComponent = null;
        public bool isBreachShrine = true;
        public Dictionary<string, int> roomStyles;

        public static Dictionary<string, GameObject> builtShrines = new Dictionary<string, GameObject>();
        private static bool m_initialized, m_builtShrines;

        public static void Init()
        {
            if (m_initialized) return;
            DungeonHooks.OnFoyerAwake += PlaceBreachShrines;
            DungeonHooks.OnPreDungeonGeneration += (generator, dungeon, flow, dungeonSeed) =>
            {
                if (flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer)
                {
                    foreach (var cshrine in GameObject.FindObjectsOfType<CustomShrineData>())
                    {
                        if (!FakePrefab.IsFakePrefab(cshrine))
                            GameObject.Destroy(cshrine.gameObject);
                    }
                    m_builtShrines = false;
                }
            };
            m_initialized = true;
        }


        ///maybe add some value proofing here (name != null, collider != IntVector2.Zero)
        public GameObject Build()
        {
            try
            {
                //Get texture and create sprite
                Texture2D tex = ResourceExtractor.GetTextureFromResource(spritePath);
                var shrine = ItemAPI.SpriteBuilder.SpriteFromResource(spritePath, null, false);

                //Add (hopefully) unique ID to shrine for tracking
                string ID = $"{modID}:{name}".ToLower().Replace(" ", "_");
                shrine.name = name;

                //Position sprite 
                var sprite = shrine.GetComponent<tk2dSprite>();
                sprite.IsPerpendicular = true;
                sprite.PlaceAtPositionByAnchor(offset, tk2dBaseSprite.Anchor.LowerCenter);

                //Add speech bubble origin
                var talkPoint = new GameObject("talkpoint").transform;
                talkPoint.position = shrine.transform.position + talkPointOffset;
                talkPoint.SetParent(shrine.transform);

                //Set up collider
                if (!usesCustomColliderOffsetAndSize)
                {
                    IntVector2 spriteDimensions = new IntVector2(tex.width, tex.height);
                    colliderOffset = new IntVector2(0, 0);
                    colliderSize = new IntVector2(spriteDimensions.x, spriteDimensions.y / 2);
                }
                var body = ItemAPI.SpriteBuilder.SetUpSpeculativeRigidbody(sprite, colliderOffset, colliderSize);

                var data = shrine.AddComponent<CustomShrineData>();
                data.ID = ID;
                data.roomStyles = roomStyles;
                data.isBreachShrine = true;
                data.offset = offset;
                data.pixelColliders = body.specRigidbody.PixelColliders;
                data.factory = this;
                data.OnAccept = OnAccept;
                data.OnDecline = OnDecline;

                IPlayerInteractable interactable;
                //Register as interactable
                if (interactableComponent != null)
                    interactable = shrine.AddComponent(interactableComponent) as IPlayerInteractable;
                else
                {
                    var simpInt = shrine.AddComponent<SimpleInteractable>();
                    simpInt.isToggle = this.isToggle;
                    simpInt.OnAccept = this.OnAccept;
                    simpInt.OnDecline = this.OnDecline;
                    simpInt.text = this.text;
                    simpInt.acceptText = this.acceptText;
                    simpInt.declineText = this.declineText;
                    simpInt.talkPoint = talkPoint;
                    interactable = simpInt as IPlayerInteractable;
                }


                if (isBreachShrine)
                {
                    if (!RoomHandler.unassignedInteractableObjects.Contains(interactable))
                        RoomHandler.unassignedInteractableObjects.Add(interactable);
                }
                else
                {
                    //DungeonHandler.RegisterDefaultShrineRoom(shrine, ID, offset);
                }

                var prefab = FakePrefab.Clone(shrine);
                builtShrines.Add(ID, prefab);
                Tools.Print("Added shrine: " + ID);
                return shrine;
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
                return null;
            }
        }

        private static void PlaceBreachShrines()
        {
            if (m_builtShrines) return;
            Tools.Print("Placing breach shrines: ");
            foreach (var prefab in builtShrines.Values)
            {
                try
                {
                    if (!prefab.GetComponent<CustomShrineData>().isBreachShrine) continue;
                    Tools.Print($"    {prefab.name}");
                    var shrine = GameObject.Instantiate(prefab).GetComponent<CustomShrineData>();
                    shrine.gameObject.SetActive(true);
                    shrine.sprite.PlaceAtPositionByAnchor(shrine.offset, tk2dBaseSprite.Anchor.LowerCenter);
                    var interactable = shrine.GetComponent<IPlayerInteractable>();
                    if (interactable is SimpleInteractable)
                    {
                        Tools.Print(shrine.OnAccept);
                        Tools.Print(shrine.OnDecline);
                        ((SimpleInteractable)interactable).OnAccept = shrine.OnAccept;
                        ((SimpleInteractable)interactable).OnDecline = shrine.OnDecline;
                    }
                    if (!RoomHandler.unassignedInteractableObjects.Contains(interactable))
                        RoomHandler.unassignedInteractableObjects.Add(interactable);
                }
                catch (Exception e)
                {
                    Tools.PrintException(e);
                }
            }
            m_builtShrines = true;
        }

        public class CustomShrineData : BraveBehaviour
        {
            public string ID;
            public bool isBreachShrine;
            public Vector3 offset;
            public List<PixelCollider> pixelColliders;
            public Dictionary<string, int> roomStyles;
            public ShrineFactory factory;
            public Action<PlayerController>
                OnAccept,
                OnDecline;
        }
    }
}

