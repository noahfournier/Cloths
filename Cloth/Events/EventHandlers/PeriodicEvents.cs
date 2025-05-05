using System;
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
        public Dictionary<int, Player> _players = new Dictionary<int, Player>();
        public PeriodicEvents()
        {
            Nova.server.OnPlayerSpawnCharacterEvent += OnPlayerSpawnCharacterEvent;
            Nova.server.OnPlayerDisconnectEvent += OnPlayerDisconnectEvent;
        }

        public async void OnPlayerSpawnCharacterEvent(Player player)
        {
            Console.WriteLine($"{player.FullName} vient d'arriver en ville.");
            _players[player.conn.connectionId] = player;
            List<ClothRecord> outfit = await CharacterInventories.GetInventoryForCharacterAsync(player.character.Id, true);
            foreach (ClothRecord record in outfit)
            {
                ClothUtils.EquipClothing(player, record);
            }
        }

        public void OnPlayerDisconnectEvent(NetworkConnection conn)
        {
            // Console.WriteLine($"{_players[conn.connectionId].FullName} vient de quitter.");
            _players.Remove(conn.connectionId);
        }
    }
}
