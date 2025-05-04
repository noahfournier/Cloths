using System;
using Cloth.Entities;
using System.Collections.Generic;
using Life;
using Life.CharacterSystem;
using Life.Network;
using UnityEngine;

namespace Cloth.Utils
{
    public static class EnvironmentUtils
    {
        public static Dictionary<int, Player> Detect(Player player)
        {
            float range = 3.0f;

            // Récupère tous les colliders dans un rayon autour du joueur
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

            return nearbyPlayers;
        }
    }
}
