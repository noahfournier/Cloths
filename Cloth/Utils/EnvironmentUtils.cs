using System.Collections.Generic;
using Life.CharacterSystem;
using Life.Network;
using UnityEngine;

namespace Cloth.Utils
{
    public static class EnvironmentUtils
    {
        /// <summary>
        /// Detects nearby players within a specified range.
        /// </summary>
        /// <param name="player">The player around whom to detect nearby players.</param>
        /// <returns>A dictionary containing the IDs and Player objects of nearby players.</returns>
        public static Dictionary<int, Player> DetectNearbyPlayers(Player player, bool withSelf = true)
        {
            float range = 3.0f;

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
    }
}
