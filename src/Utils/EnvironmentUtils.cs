using System;
using System.Collections.Generic;
using Clothes.Config;
using Life;
using Life.AreaSystem;
using Life.CharacterSystem;
using Life.Network;
using UnityEngine;

namespace Clothes.Utils
{
    public static class EnvironmentUtils
    {
        private static float range = 3.0f;

        /// <summary>
        /// Detects nearby players within a specified range.
        /// </summary>
        /// <param name="player">The player around whom to detect nearby players.</param>
        /// <returns>A dictionary containing the IDs and Player objects of nearby players.</returns>
        public static Dictionary<int, Player> DetectNearbyPlayers(Player player, bool withSelf = true)
        {

            Collider[] nearbyColliders = Physics.OverlapSphere(player.setup.transform.position, range);
            Dictionary<int, Player> nearbyPlayers = new Dictionary<int, Player>();

            foreach (Collider c in nearbyColliders)
            {
                CharacterSetup characterSetup = c.GetComponent<CharacterSetup>();
                if (characterSetup != null)
                {
                    nearbyPlayers[characterSetup.player.character.Id] = characterSetup.player;
                }
            }

            if(!withSelf) nearbyPlayers.Remove(player.character.Id);

            return nearbyPlayers;
        }


        public static AreaObject DetectNearbyWardrobe(Player player)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(player.setup.transform.position, range);

            foreach (Collider c in nearbyColliders)
            {
                AreaObject areaObject = c.GetComponent<AreaObject>();
                if (areaObject != null)
                {
                    if(ClothesConfig.Data.WardrobeItemIdsList.Contains(areaObject.itemId))
                    {
                        return areaObject;
                    }
                }   
            }

            return null;
        }
    }
}
