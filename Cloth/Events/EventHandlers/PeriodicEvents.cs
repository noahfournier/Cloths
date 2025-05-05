using System.Collections.Generic;
using Cloth.Entities;
using Cloth.Utils;
using Life;
using Life.Network;
using Mirror;

namespace Cloth.Events
{
    public class PeriodicEvents
    {
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();
        public PeriodicEvents()
        {
            Nova.server.OnPlayerSpawnCharacterEvent += OnPlayerSpawnCharacterEvent;
            Nova.server.OnPlayerDisconnectEvent += OnPlayerDisconnectEvent;
        }

        /// <summary>
        /// Handles the event when a player spawns a character.
        /// </summary>
        /// <param name="player">The player who spawned the character.</param>
        public async void OnPlayerSpawnCharacterEvent(Player player)
        {
            _players[player.conn.connectionId] = player;
            List<ClothRecord> outfit = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id, true);
            foreach (ClothRecord record in outfit)
            {
                ClothUtils.EquipClothing(player, record);
            }
        }

        /// <summary>
        /// Handles the event when a player disconnects.
        /// </summary>
        /// <param name="conn">The network connection of the disconnected player.</param>
        public void OnPlayerDisconnectEvent(NetworkConnection conn)
        {
            _players.Remove(conn.connectionId);
        }
    }
}
